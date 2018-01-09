using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [Header("移動速度")]
    public float m_Speed = 1f;
    [Header("ジャンプの限界距離")]
    public float m_JumpLimit = 50f;
    [Header("ジャンプの射角")]
    public float m_Angle = 30f;
    [Header("地面から木にジャンプする際の高さ")]
    public float m_GroundJumpHeight = 5f;
    [Header("地面から木にジャンプする際の前方向の距離")]
    public float m_GroundJumpForward = 5f;
    public float jumpLower = 5f;
    public Transform m_Camera;
    public LayerMask m_GroundLayer, m_TreeLayer, m_StringLayer, m_NetLayer, m_EnemyLayer;
    public PredictionLine m_Prediction;
    public Animator m_Animator;
    public StringShooter m_Shooter;
    public PlayerStateManager m_StateManager;           //Player状態管理
    public AudioSource m_AudioSource;
    public List<AudioClip> m_AudioClips;                //0:ジャンプ、1:回避、2:回避失敗、3:タックル
    public Rigidbody m_Rigidbody;
    public int PlayerNum = 1;

    static readonly float MIN = -100f;                   //行動範囲
    static readonly float MAX = 100f;                    //行動範囲
    static readonly float EscapeTime = 2f;              //回避入力の間隔
    private RaycastHit m_hitinfo;                       //足元の情報
    private Vector3 m_prevPos;
    private Vector3 move_start;                         //ジャンプ始点
    private Vector3 move_end;                           //ジャンプ終点
    private RaycastHit jump_target;
    private bool isCursor = false;
    private bool isEscape = false;
    private int waitFrame = 0;
    private float m_failureTime = 0;
    private float m_attackTime = 0;
    private bool isLanding = false;
    private Collider m_enemy = null;
    private float escapeInterval = 0;

    protected override void Start()
    {
        m_StateManager.GroundTp.p_exeDelegate = GroundMove;
        m_StateManager.TreeTp.p_exeDelegate = TreeTpMove;
        m_StateManager.TreeFp.p_exeDelegate = TreeFpMove;
        m_StateManager.JumpTp.p_exeDelegate = JumpTpMove;
        m_StateManager.StringTp.p_exeDelegate = StringTpMove;
        m_StateManager.Falling.p_exeDelegate = FallingMove;
        m_StateManager.BodyBlow.p_exeDelegate = BodyBlowMove;
        m_StateManager.GroundJump.p_exeDelegate = GroundJump;
        m_Prediction.gameObject.SetActive(true);
    }

    protected override void Update()
    {
        transform.position = MoveRange(transform.position, new Vector3(MIN, 0, MIN), new Vector3(MAX, 50f, MAX));

        if (isBodyblow)
        {
            m_StateManager.StateProcassor.State = m_StateManager.Falling;
        }
    }

    //地面にいる場合の移動
    void GroundMove()
    {
        m_Animator.SetBool("IsString", false);
        m_Prediction.gameObject.SetActive(false);
        m_Rigidbody.velocity = Vector3.zero;
        Vector3 start = transform.position + transform.up * 0.5f;
        //足元の情報取得（地面優先）
        if (!Physics.Raycast(start, -transform.up, out m_hitinfo, 1f, m_GroundLayer))
            Physics.Raycast(start, -transform.up, out m_hitinfo, 1f, m_TreeLayer);

        if (m_hitinfo.collider.tag == "Tree")
        {
            m_StateManager.StateProcassor.State = m_StateManager.TreeTp;
            return;
        }

        //位置補正
        transform.position = Vector3.Lerp(transform.position, m_hitinfo.point, 0.2f);
        Vector3 move = Vector3.zero;
        if(m_Animator.GetCurrentAnimatorStateInfo(0).IsName("GroundMove"))
            move = Move(m_Camera.right, m_hitinfo.normal);

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    m_Animator.SetTrigger("NormalJump");
        //    m_StateManager.StateProcassor.State = m_StateManager.GroundJump;
        //    return;
        //}
        RaycastHit hit;
        if (Physics.Raycast(start + Vector3.up * m_GroundJumpHeight, transform.forward, out hit, m_GroundJumpForward, m_TreeLayer))
        {
            m_Prediction.gameObject.SetActive(true);
            m_Prediction.SetParameter(transform.position, hit.point, m_Angle);
            m_Prediction.Calculation();
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump"))
            {
                m_hitinfo = hit;
                move_start = transform.position;
                move_end = hit.point;
                m_Prediction.gameObject.SetActive(false);
                m_Animator.SetTrigger("NormalJump");
                m_StateManager.StateProcassor.State = m_StateManager.GroundJump;
                return;
            }
        }
        else
            m_Prediction.gameObject.SetActive(false);

        //木に登る
        if (Physics.Raycast(start, move, out m_hitinfo, 1f, m_TreeLayer))
        {
            transform.position = m_hitinfo.point;
            transform.rotation = Quaternion.LookRotation(Vector3.Cross(transform.right, m_hitinfo.normal), m_hitinfo.normal);
            m_StateManager.StateProcassor.State = m_StateManager.TreeTp;
        }
    }

    //木の上の俯瞰カメラ状態での移動
    void TreeTpMove()
    {
        m_Animator.SetBool("IsString", false);
        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Jumpair"))
            m_Animator.SetTrigger("Landing");        
        Vector3 start = transform.position + transform.up * 0.5f;
        //足元の情報取得（優先順位 : ネット->木->地面）
        if (!Physics.Raycast(start, -transform.up, out m_hitinfo, 1f, m_NetLayer))
            if (!Physics.Raycast(start, -transform.up, out m_hitinfo, 1f, m_TreeLayer))
                Physics.Raycast(start, -transform.up, out m_hitinfo, 1f, m_GroundLayer);

        if(m_hitinfo.collider == null)
        {
            transform.position = m_prevPos;
            //m_hitinfo = m_prevHit;
            return;
        }
        //if (m_hitinfo.collider.tag == "Ground")
        //{
        //    m_StateManager.StateProcassor.State = m_StateManager.GroundTp;
        //    return;
        //}

        if (IsChangedNumber())
            return;
        m_prevPos = transform.position;
        //位置補正
        transform.position = Vector3.Lerp(transform.position, m_hitinfo.point, 0.8f);
        //transform.position = m_hitinfo.point;
        Vector3 move = Vector3.zero;
        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("GroundMove"))
            move = Move(m_Camera.right, m_hitinfo.normal);

        RaycastHit prevHit = m_hitinfo;
        if (Physics.Raycast(start, move, out m_hitinfo, 1f, m_GroundLayer))
        {
            transform.position = m_hitinfo.point;
            transform.rotation = Quaternion.LookRotation(Vector3.Cross(m_Camera.right, m_hitinfo.transform.up), m_hitinfo.transform.up);
            m_StateManager.StateProcassor.State = m_StateManager.GroundTp;
            return;
        }
        else
            m_hitinfo = prevHit;

        //一人称視点変更
        if (Input.GetKeyDown(KeyCode.L) || Input.GetButtonDown("LB"))
            m_StateManager.StateProcassor.State = m_StateManager.TreeFp;

        Vector3 origin = start + (m_Camera.position - start).normalized;
        Ray ray = new Ray(origin, m_Camera.forward);
        int[] layers = new int[1];
        layers[0] = m_StringLayer;
        if (Input.GetKeyDown(KeyCode.F) || Input.GetButtonDown("Intersect"))
        {
            IntersectString(layers);
        }
        Jump(ray, m_hitinfo);
    }

    //木の上の一人称カメラ状態での操作
    void TreeFpMove()
    {
        if (IsChangedNumber())
            return;
        Vector3 start = transform.position + transform.up * 0.5f;
        if (Input.GetKeyDown(KeyCode.L) || Input.GetButtonDown("LB"))
        {
            if (m_hitinfo.collider.tag == "Tree" || m_hitinfo.collider.tag == "Net")
                m_StateManager.StateProcassor.State = m_StateManager.TreeTp;
            else if (m_hitinfo.collider.tag == "String")
                m_StateManager.StateProcassor.State = m_StateManager.StringTp;
        }

        Vector3 origin = start + (m_Camera.position - start).normalized;
        Ray ray = new Ray(origin, m_Camera.forward);
        Jump(ray, m_hitinfo);
    }

    //ジャンプ中の動作
    void JumpTpMove()
    {
        if (!Depression()) return;
        if (Projection(move_start, move_end, jump_target.normal, m_Angle))
        {
            transform.position = jump_target.point;
            transform.rotation = Quaternion.LookRotation(Vector3.Cross(m_Camera.right, jump_target.normal), jump_target.normal);
            if(isEscape)
                m_Shooter.StringShoot(m_Prediction.m_HitStringPoint.point, move_end);
            else
                m_Shooter.StringShoot(move_start, move_end);
            waitFrame = 0;
            isEscape = false;
            isLanding = false;
            if (jump_target.collider.tag == "String")
            {
                m_hitinfo = jump_target;
                m_StateManager.StateProcassor.State = m_StateManager.StringTp;
                return;
            }
            m_StateManager.StateProcassor.State = m_StateManager.TreeTp;
            return;
        }
        float dif = Mathf.Abs(flightDuration - elapse_time);
        if (dif < 0.3f && !isLanding)
        {
            m_Animator.SetTrigger("Landing");
            isLanding = true;
            if (jump_target.collider.tag == "String")
                m_Animator.SetBool("IsString", true);
            else
                m_Animator.SetBool("IsString", false);
        }

        int playerLayer = LayerMask.GetMask(new string[] { "Player" });
        if (m_Prediction.m_HitStringPoint.point != Vector3.zero)
        {
            if (Physics.CheckSphere(m_Prediction.m_HitStringPoint.point, 3f, playerLayer))
            {
                if (Physics.CheckSphere(m_Prediction.m_HitStringPoint.point, 0.5f, playerLayer) && !isEscape)
                {
                    waitFrame = 0;
                    Fall();
                    return;
                }
                //回避行動
                if ((Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump")) && escapeInterval <= 0)
                {
                    if(m_Prediction.m_IsString)
                        m_Prediction.m_HitString.SideUpdate(m_Shooter.m_SideNumber);
                    else
                        m_Prediction.m_HitNet.SideUpdate(m_Shooter.m_SideNumber);
                    m_Shooter.StringShoot(move_start, m_Prediction.m_HitStringPoint.point);
                    m_Animator.SetTrigger("Escape");
                    m_AudioSource.PlayOneShot(m_AudioClips[1]);
                    isEscape = true;
                    return;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump"))
        {
            escapeInterval = EscapeTime;
        }
        escapeInterval -= Time.deltaTime;
    }

    //糸の上での移動
    void StringTpMove()
    {
        if (m_hitinfo.collider.tag == "Net")
            m_StateManager.StateProcassor.State = m_StateManager.TreeTp;
        if (IsChangedNumber())
            return;
        m_Animator.SetBool("IsString", true);
        if (m_hitinfo.collider.GetComponent<StringUnit>().m_SideNumber != m_Shooter.m_SideNumber)
            m_StateManager.StateProcassor.State = m_StateManager.Falling;
        var line = m_hitinfo.collider.GetComponent<LineRenderer>();
        //糸のベクトル
        Vector3 stringVec = (line.GetPosition(1) - line.GetPosition(0)).normalized;
        //上方向ベクトル
        Vector3 up = Vector3.Cross(m_Camera.right, stringVec);
        //糸のベクトルを90度回転したベクトル
        Vector3 stringVertical = Quaternion.Euler(0, 90f, 0) * stringVec;
        float angle = Vector3.Angle(m_Camera.right, stringVec);
        Vector3 vec = Vector3.zero;
        Vector3 move = Vector3.zero;
        //４方向に向きを変えていく
        if (angle > 45f && angle < 135f)
        {
            if (up.y > 0)
                stringVec = -stringVec;
            vec = stringVec;
            move = stringVec * Input.GetAxis("Vertical");
            m_Animator.SetFloat("StringMoveX", 0);
            m_Animator.SetFloat("StringMoveZ", Input.GetAxis("Vertical"));
        }
        else if(angle < 45f)
        {
            vec = -stringVertical;
            move = stringVec * Input.GetAxis("Horizontal");
            m_Animator.SetFloat("StringMoveZ", 0);
            m_Animator.SetFloat("StringMoveX", Input.GetAxis("Horizontal"));
        }
        else if(angle > 135f)
        {
            vec = stringVertical;
            move = -stringVec * Input.GetAxis("Horizontal");
            m_Animator.SetFloat("StringMoveZ", 0);
            m_Animator.SetFloat("StringMoveX", Input.GetAxis("Horizontal"));
        }
        transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.forward, vec, 0.2f));
        transform.Translate(move * m_Speed * 1.3f * Time.deltaTime, Space.World);
        transform.position = MoveRange(transform.position, line.GetPosition(0), line.GetPosition(1));

        //一人称視点変更
        if (Input.GetKeyDown(KeyCode.L) || Input.GetButtonDown("LB"))
        {
            m_StateManager.StateProcassor.State = m_StateManager.TreeFp;
        }

        Vector3 start = transform.position + transform.up * 0.5f;
        Vector3 origin = start + (m_Camera.position - transform.position).normalized;
        Ray ray = new Ray(origin, m_Camera.forward);
        int[] layers = new int[2];
        layers[0] = m_StringLayer;
        layers[1] = m_NetLayer;
        if (Input.GetKeyDown(KeyCode.F) || Input.GetButtonDown("Intersect"))
        {
            IntersectString(layers);
        }
        Jump(ray, m_hitinfo);
    }

    //落下状態での動作
    void FallingMove()
    {
        if (m_failureTime < 0.5f)
        {
            m_failureTime += Time.deltaTime;
            return;
        }
        Ray ray = new Ray(transform.position, -Vector3.up);
        if (!Physics.Raycast(ray, out m_hitinfo, 100f, m_TreeLayer))
            Physics.Raycast(ray, out m_hitinfo, 100f, m_GroundLayer);
        float dis = Vector3.Distance(move_start, move_end);
        //落下スピード
        gravity.y += Physics.gravity.y * Time.deltaTime;
        float fallTime = Mathf.Abs(dis / gravity.y);
        transform.Translate(gravity * Time.deltaTime, Space.World);
        Vector3 forward = Vector3.Cross(m_Camera.right, m_hitinfo.normal);
        transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.forward, forward, 0.4f), m_hitinfo.normal);
    }
    //体当たり状態
    void BodyBlowMove()
    {
        if (!Depression()) return;
        if (Projection(move_start, move_end, jump_target.normal, m_Angle))
        {
            transform.position = move_end;
            transform.rotation = Quaternion.LookRotation(Vector3.Cross(m_Camera.right, jump_target.normal), jump_target.normal);
            if (isEscape)
                m_Shooter.StringShoot(m_Prediction.m_HitStringPoint.point, move_end);
            else
                m_Shooter.StringShoot(move_start, move_end);
            waitFrame = 0;
            isEscape = false;
            isLanding = false;
            if (jump_target.collider.tag == "String")
            {
                m_hitinfo = jump_target;
                m_StateManager.StateProcassor.State = m_StateManager.StringTp;
                return;
            }
            m_StateManager.StateProcassor.State = m_StateManager.TreeTp;
            return;
        }
        LayerMask playerLayer = LayerMask.GetMask(new string[] { "Player" });
        if (Physics.CheckSphere(move_end, 2f, playerLayer) && !isLanding)
        {
            if (!Physics.CheckSphere(move_end, 2f, m_EnemyLayer)) return;
            m_Animator.SetTrigger("Tackle");
            m_AudioSource.PlayOneShot(m_AudioClips[3]);
            SendingBodyBlow(m_enemy.gameObject);
            isLanding = true;
            if (jump_target.collider.tag == "String")
                m_Animator.SetBool("IsString", true);
            else
                m_Animator.SetBool("IsString", false);
        }
    }
    //地面上でのジャンプ
    void GroundJump()
    {
        if (!Depression()) return;
        //if(waitFrame == 5)
        //{
        //    m_Rigidbody.AddForce(transform.up * 150f);
        //    //m_Animator.speed = 0;
        //}
        //gravity.y += Physics.gravity.y * Time.deltaTime;
        //m_Rigidbody.AddForce(transform.up * gravity.y * Time.deltaTime * 50f);
        //if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("NormalJump")
        //    && m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f)
        //{
        //    if (Physics.Raycast(transform.position + transform.up * 0.5f, Vector3.down, 0.5f, m_GroundLayer))
        //    {
        //        m_Animator.SetTrigger("Landing");
        //    }
        //}
        //if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("GroundMove"))
        //{
        //    waitFrame = 0;
        //    gravity.y = 0;
        //    GetComponent<Rigidbody>().velocity = Vector3.zero;
        //    m_Animator.ResetTrigger("Landing");
        //    m_StateManager.StateProcassor.State = m_StateManager.GroundTp;
        //}

        if(Projection(move_start, move_end, m_hitinfo.normal, m_Angle))
        {
            waitFrame = 0;
            m_Animator.ResetTrigger("Landing");
            m_StateManager.StateProcassor.State = m_StateManager.TreeTp;
            return;
        }
        float dif = Mathf.Abs(flightDuration - elapse_time);
        if (dif < 0.3f)
        {
            m_Animator.SetTrigger("Landing");
        }

    }

    //移動
    private Vector3 Move(Vector3 right, Vector3 up)
    {
        Vector3 forward = Vector3.Cross(right, up);
        transform.rotation = Quaternion.LookRotation(forward, up);
        Vector3 move = forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal");
        //アニメーション
        m_Animator.SetFloat("MoveX", Input.GetAxis("Horizontal"));
        m_Animator.SetFloat("MoveZ", Input.GetAxis("Vertical"));
        transform.Translate(move * m_Speed * Time.deltaTime, Space.World);
        return move;
    }

    //ジャンプ
    private void Jump(Ray ray, RaycastHit hit)
    {
        bool jump = false;
        //糸を狙うのかどうか
        if (Input.GetKeyUp(KeyCode.K) || Input.GetButtonUp("RB"))
        {
            m_attackTime = 0;
            m_enemy = null;
            isCursor = !isCursor;
        }
        if (isCursor)
        {
            if (!(jump = Physics.Raycast(ray, out jump_target, m_JumpLimit, m_NetLayer)))
                jump = Physics.SphereCast(ray, 1f, out jump_target, m_JumpLimit, m_StringLayer);
        }
        else
            //jump = Physics.SphereCast(ray, 1f, out jump_target, m_JumpLimit, m_TreeLayer);
            jump = Physics.Raycast(ray, out jump_target, m_JumpLimit, m_TreeLayer);

        if (Input.GetKey(KeyCode.K) || Input.GetButton("RB"))
        {
            m_attackTime += Time.deltaTime;
            if (m_attackTime > 1f)
                foreach(Collider col in Physics.OverlapSphere(jump_target.point, 3f, m_EnemyLayer))
                    m_enemy = col;
        }

        if (jump)
        {
            if (hit.collider.gameObject == jump_target.collider.gameObject)
            {
                if (Vector3.Distance(transform.position, jump_target.point) < jumpLower)
                {
                    m_Prediction.gameObject.SetActive(false);
                    return;
                }
            }
            if (jump_target.transform.tag == "String")
                if (jump_target.transform.GetComponent<StringUnit>().m_SideNumber != m_Shooter.m_SideNumber)
                    return;
            if (jump_target.collider.tag == "Net")
                if (jump_target.transform.GetComponent<Net>().m_SideNumber != m_Shooter.m_SideNumber)
                    return;

            //予測線、カーソル表示
            m_Prediction.gameObject.SetActive(true);
            m_Prediction.SetParameter(transform.position, jump_target.point, m_Angle);
            if (m_enemy != null)
                m_Prediction.SetParameter(transform.position, m_enemy.transform.position, m_Angle);
            m_Prediction.Calculation();
            //ジャンプ
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump"))
            {
                m_Prediction.gameObject.SetActive(false);
                move_start = transform.position;
                move_end = jump_target.point;
                m_Animator.SetTrigger("Jump");
                escapeInterval = 0;
                if (m_enemy != null)
                {
                    Physics.Raycast(m_enemy.transform.position, -m_enemy.transform.up, out jump_target, 1f, m_TreeLayer);
                    move_end = jump_target.point;
                    m_StateManager.StateProcassor.State = m_StateManager.BodyBlow;
                    return;
                }
                m_StateManager.StateProcassor.State = m_StateManager.JumpTp;
            }
            return;
        }
        m_Prediction.gameObject.SetActive(false);
        m_enemy = null;
    }

    private bool Depression()
    {
        waitFrame++;
        if (waitFrame < 5) return false;
        if(waitFrame == 5)
            m_AudioSource.PlayOneShot(m_AudioClips[0]);
        return true;
    }

    //落下
    private void Fall()
    {
        m_Animator.SetTrigger("Failure");
        m_AudioSource.PlayOneShot(m_AudioClips[2]);
        //落下先情報取得（木を優先）
        Ray ray = new Ray(transform.position, -Vector3.up);
        if(!Physics.Raycast(ray, out m_hitinfo, 100f, m_TreeLayer))
            Physics.Raycast(ray, out m_hitinfo, 100f, m_GroundLayer);
        move_start = transform.position;
        move_end = m_hitinfo.point;
        m_StateManager.StateProcassor.State = m_StateManager.Falling;
    }

    //移動範囲制限
    private Vector3 MoveRange(Vector3 targetPos, Vector3 value_1, Vector3 value_2)
    {
        if (value_1.x <= value_2.x)
            targetPos.x = Mathf.Clamp(targetPos.x, value_1.x, value_2.x);
        else if (value_1.x > value_2.x)
            targetPos.x = Mathf.Clamp(targetPos.x, value_2.x, value_1.x);
        if (value_1.y <= value_2.y)
            targetPos.y = Mathf.Clamp(targetPos.y, value_1.y, value_2.y);
        else if (value_1.y > value_2.y)
            targetPos.y = Mathf.Clamp(targetPos.y, value_2.y, value_1.y);
        if (value_1.z <= value_2.z)
            targetPos.z = Mathf.Clamp(targetPos.z, value_1.z, value_2.z);
        else if (value_1.z > value_2.z)
            targetPos.z = Mathf.Clamp(targetPos.z, value_2.z, value_1.z);

        return targetPos;
    }

    //糸の交差部分移動
    private void IntersectString(int[] layerMask)
    {
        float shortest = 100000f;
        Transform result = null;
        for(int i = 0; i < layerMask.Length; i++)
        {
            if (i > 0 && result != null) continue;
            foreach (Collider col in Physics.OverlapSphere(transform.position, 0.5f, layerMask[i]))
            {
                if(col.tag == "Net")
                {
                    Net n = col.GetComponent<Net>();
                    StringUnit s = n.m_StartConnecter as StringUnit;
                    Comparision(s.m_PointA, n.transform, ref result, ref shortest);
                    Comparision(s.m_PointB, n.transform, ref result, ref shortest);
                    s = col.GetComponent<Net>().m_EndConnecter as StringUnit;
                    Comparision(s.m_PointA, n.transform, ref result, ref shortest);
                    Comparision(s.m_PointB, n.transform, ref result, ref shortest);
                }
                else if(col.tag == "String")
                {
                    StringUnit s = col.GetComponent<StringUnit>();
                    Comparision(s.m_PointA, s.transform, ref result, ref shortest);
                    Comparision(s.m_PointB, s.transform, ref result, ref shortest);
                }
            }
        }
        if (result == null) return;
        if(result.tag == "String")
        {
            Physics.SphereCast(result.position + Vector3.up, 0.5f, Vector3.down, out m_hitinfo, 1f, m_StringLayer);
            m_StateManager.StateProcassor.State = m_StateManager.StringTp;
        }
        else
        {
            Physics.SphereCast(result.position + Vector3.up, 0.5f, Vector3.down, out m_hitinfo, 1f, m_NetLayer);
            m_StateManager.StateProcassor.State = m_StateManager.TreeTp;
        }
    }
    //一番近い位置を求める
    private void Comparision(Vector3 pos, Transform target, ref Transform result, ref float shortest)
    {
        if (m_hitinfo.collider.gameObject == target.gameObject) return;
        if (target.tag == "String" || target.tag == "Net")
        {
            float distance = Vector3.Distance(pos, transform.position);
            if (shortest > distance)
            {
                shortest = distance;
                result = target;
            }
        }
    }

    //乗ってる糸の番号が変更した場合
    private bool IsChangedNumber()
    {
        if(m_hitinfo.collider.tag == "Net")
        {
           if (m_hitinfo.collider.GetComponent<Net>().m_SideNumber != m_Shooter.m_SideNumber)
            {
                Fall();
                return true;
            }
        }
        else if(m_hitinfo.collider.tag == "String")
        {
            if (m_hitinfo.collider.GetComponent<StringUnit>().m_SideNumber != m_Shooter.m_SideNumber)
            {
                Fall();
                return true;
            }
        }
        return false;
    }
    //木に乗ってるかどうか
    public bool IsOnTree()
    {
        return (m_hitinfo.collider.tag == "Tree");
    }

    void OnTriggerEnter(Collider other)
    {
        //地面着地
        if (m_StateManager.StateProcassor.State != m_StateManager.Falling) return;
        if (other.transform.tag == "Ground" || other.transform.tag == "Tree")
        {
            if (m_hitinfo.transform.tag != other.transform.tag) return;
            if(receiveBodyblow != null)
            {
                StopCoroutine(receiveBodyblow);
                receiveBodyblow = null;
            }
            isBodyblow = false;
            gravity = Vector3.zero;
            elapse_time = 0;
            m_failureTime = 0;
            m_Animator.SetTrigger("Landing");
            if (other.transform.tag == "Ground")
                m_StateManager.StateProcassor.State = m_StateManager.GroundTp;
            else
                m_StateManager.StateProcassor.State = m_StateManager.TreeTp;
        }
    }
}
