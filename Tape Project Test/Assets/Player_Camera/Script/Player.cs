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
    public Transform m_Camera;
    public LayerMask m_GroundLayer, m_TreeLayer, m_StringLayer, m_NetLayer;
    public PredictionLine m_Prediction;
    public Animator m_Animator;

    private PlayerStateManager m_StateManager;          //Player状態管理
    private RaycastHit m_hitinfo;                       //足元の情報
    private Vector3 m_prevPos;
    private Vector3 Gravity = Vector3.zero;             //重力
    private Vector3 move_start;                         //ジャンプ始点
    private Vector3 move_end;                           //ジャンプ終点
    private RaycastHit jump_target;
    private bool isEscape = false;
    private int waitFrame = 0;
    private float m_failureTime = 0;
    private float m_fallElapse = 0;
    private bool isLanding = false;

    public StringShooter m_Shooter;
    protected override void Start()
    {
        m_StateManager = PlayerStateManager.GetInstance;
        m_StateManager.GroundTp.p_exeDelegate = GroundMove;
        m_StateManager.TreeTp.p_exeDelegate = TreeTpMove;
        m_StateManager.TreeFp.p_exeDelegate = TreeFpMove;
        m_StateManager.JumpTp.p_exeDelegate = JumpTpMove;
        m_StateManager.StringTp.p_exeDelegate = StringTpMove;
        m_StateManager.Falling.p_exeDelegate = FallingMove;
        m_Prediction.gameObject.SetActive(true);
        //Instantiate(m_Prediction);
    }

    protected override void Update()
    {
    }

    //地面にいる場合の移動
    void GroundMove()
    {
        m_Animator.SetBool("IsString", false);
        m_Prediction.gameObject.SetActive(false);
        Vector3 start = transform.position + transform.up * 0.5f;
        //足元の情報取得（地面優先）
        if (!Physics.Raycast(start, -transform.up, out m_hitinfo, 1f, m_GroundLayer))
            Physics.Raycast(start, -transform.up, out m_hitinfo, 1f, m_TreeLayer);

        //位置補正
        transform.position = Vector3.Lerp(transform.position, m_hitinfo.point, 0.2f);
        Vector3 move = Vector3.zero;
        if(m_Animator.GetCurrentAnimatorStateInfo(0).IsName("GroundMove"))
            move = Move(m_Camera.right, m_hitinfo.normal);

        //木に登る
        if (Physics.Raycast(start, move, out m_hitinfo, 1f))
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
        Vector3 start = transform.position + transform.up * 0.5f;
        //足元の情報取得（優先順位 : 木->地面->糸(ネット)）
        if (!Physics.Raycast(start, -transform.up, out m_hitinfo, 1f, m_NetLayer))
            if (!Physics.Raycast(start, -transform.up, out m_hitinfo, 1f, m_TreeLayer))
                Physics.Raycast(start, -transform.up, out m_hitinfo, 1f, m_GroundLayer);

        if(m_hitinfo.collider == null)
        {
            transform.position = m_prevPos;
            return;
        }
        if (IsChangedNumber())
            return;
        m_prevPos = transform.position;
        //位置補正
        transform.position = Vector3.Lerp(transform.position, m_hitinfo.point, 0.2f);
        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("GroundMove"))
            Move(m_Camera.right, m_hitinfo.normal);

        //一人称視点変更
        if (Input.GetKeyDown(KeyCode.L) || Input.GetButtonDown("LB"))
            m_StateManager.StateProcassor.State = m_StateManager.TreeFp;

        Vector3 origin = start + (m_Camera.position - transform.position).normalized;
        Ray ray = new Ray(origin, m_Camera.forward);
        Jump(ray, m_hitinfo);

        if (m_hitinfo.collider.tag == "Ground")
            m_StateManager.StateProcassor.State = m_StateManager.GroundTp;
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

        Vector3 origin = start + (m_Camera.position - transform.position).normalized;
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
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump"))
                {
                    if(m_Prediction.m_IsString)
                        m_Prediction.m_HitString.SideUpdate(m_Shooter.m_SideNumber);
                    else
                        m_Prediction.m_HitNet.SideUpdate(m_Shooter.m_SideNumber);
                    m_Shooter.StringShoot(move_start, m_Prediction.m_HitStringPoint.point);
                    m_Animator.SetTrigger("Escape");
                    isEscape = true;
                }
            }
        }
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
        transform.Translate(move * m_Speed * Time.deltaTime, Space.World);
        transform.position = MoveRange(transform.position, line.GetPosition(0), line.GetPosition(1));

        //一人称視点変更
        if (Input.GetKeyDown(KeyCode.L) || Input.GetButtonDown("LB"))
        {
            m_StateManager.StateProcassor.State = m_StateManager.TreeFp;
        }

        Vector3 start = transform.position + transform.up * 0.5f;
        Vector3 origin = start + (m_Camera.position - transform.position).normalized;
        Ray ray = new Ray(origin, m_Camera.forward);
        Jump(ray, m_hitinfo);
    }

    //落下状態での動作
    void FallingMove()
    {
        if(m_failureTime < 0.5f)
        {
            m_failureTime += Time.deltaTime;
            return;
        }
        float dis = Vector3.Distance(move_start, move_end);
        //落下スピード
        Gravity.y += Physics.gravity.y * Time.deltaTime;
        float fallTime = Mathf.Abs(dis / Gravity.y);
        transform.Translate(Gravity * Time.deltaTime, Space.World);
        Vector3 forward = Vector3.Cross(m_Camera.right, m_hitinfo.normal);
        transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.forward, forward, 0.1f), m_hitinfo.normal);

        //m_fallElapse += Time.deltaTime;
        //if(m_fallElapse > fallTime)
        //{
        //    Gravity = Vector3.zero;
        //    elapse_time = 0;
        //    m_fallElapse = 0;
        //    m_failureTime = 0;
        //    m_Animator.SetTrigger("Landing");
        //    if (m_hitinfo.collider.tag == "Ground")
        //        m_StateManager.StateProcassor.State = m_StateManager.GroundTp;
        //    else
        //        m_StateManager.StateProcassor.State = m_StateManager.TreeTp;
        //}
    }

    //移動
    private Vector3 Move(Vector3 right, Vector3 up)
    {
        Vector3 forward = Vector3.Cross(right, up);
        transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.forward, forward, 0.3f), up);
        Vector3 move = forward * Input.GetAxis("Vertical") + right * Input.GetAxis("Horizontal");
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
        if (Input.GetKey(KeyCode.K) || Input.GetButton("RB"))
        {
            if (!(jump = Physics.Raycast(ray, out jump_target, m_JumpLimit, m_NetLayer)))
                jump = Physics.SphereCast(ray, 1f, out jump_target, m_JumpLimit, m_StringLayer);
        }
        else
            jump = Physics.SphereCast(ray, 1f, out jump_target, m_JumpLimit, m_TreeLayer);

        if (jump)
        {
            if (hit.collider.gameObject == jump_target.collider.gameObject)
            {
                m_Prediction.gameObject.SetActive(false);
                return;
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
            m_Prediction.Calculation();
            //ジャンプ
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump"))
            {
                m_Prediction.gameObject.SetActive(false);
                move_start = transform.position;
                move_end = jump_target.point;
                m_Animator.SetTrigger("Jump");
                m_StateManager.StateProcassor.State = m_StateManager.JumpTp;
            }
            return;
        }
        m_Prediction.gameObject.SetActive(false);
    }

    private bool Depression()
    {
        waitFrame++;
        if (waitFrame < 10) return false;
        return true;
    }

    //落下
    private void Fall()
    {
        m_Animator.SetTrigger("Failure");
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

    private StringUnit StringSearch(StringUnit stringUnit, GameObject hitinfo)
    {
        foreach(Connecter c in stringUnit.m_Child)
        {
            StringUnit s = c as StringUnit;
            if(s.gameObject == hitinfo.gameObject)
            {
                return s;
            }
        }
        return null;
    }

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

    void OnTriggerEnter(Collider other)
    {
        //地面着地
        if (m_StateManager.StateProcassor.State != m_StateManager.Falling) return;
        if (other.transform.tag == "Ground" || other.transform.tag == "Tree")
        {
            if (m_hitinfo.transform.tag != other.transform.tag) return;
            Gravity = Vector3.zero;
            elapse_time = 0;
            m_failureTime = 0;
            m_Animator.SetTrigger("Landing");
            if (other.transform.tag == "Ground")
                m_StateManager.StateProcassor.State = m_StateManager.GroundTp;
            else
                m_StateManager.StateProcassor.State = m_StateManager.TreeTp;
        }

    }

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(m_Prediction.m_HitStringPoint.point, 1f);
    }
}
