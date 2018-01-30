using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum JumpMode
{
    NormalJump,         //通常
    CapturingJump,      //占領ジャンプ
    StringJump          //糸ジャンプ
}

public partial class Player : Character
{
    [Header("移動速度")]
    public float m_Speed = 5f;
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
    public List<AudioClip> m_AudioClips;                //0:回避、1:攻撃、2:ジャンプ着地、3:落下着地、4:風、5:歩き
    public GameObject m_EscapeSphere;                   //回避の範囲
    public ParticleSystem m_WindLine;

    static readonly float MIN = -100f;                   //行動範囲
    static readonly float MAX = 100f;                    //行動範囲
    static readonly float EscapeTime = 0.5f;             //回避入力の間隔
    private Vector3 m_center;                           //中心点
    private RaycastHit m_hitinfo;                       //足元の情報
    private Vector3 m_prevPos;
    private Vector3 move_start;                         //ジャンプ始点
    private Vector3 move_end;                           //ジャンプ終点
    private RaycastHit jump_target;
    private JumpMode m_JumpMode = JumpMode.NormalJump;
    private bool isTarget = false;
    private bool isEscape = false;
    private int waitFrame = 0;
    private float m_failureTime = 0;
    private float m_attackTime = 0;
    private bool isLanding = false;
    private Collider m_enemy = null;
    private float m_escapeInterval = 0;

    protected override void Start()
    {
        m_center = transform.position + transform.up * 0.5f;
        m_StateManager.GroundTp.p_exeDelegate = GroundMove;
        m_StateManager.TreeTp.p_exeDelegate = TreeTpMove;
        m_StateManager.TreeFp.p_exeDelegate = TreeFpMove;
        m_StateManager.JumpTp.p_exeDelegate = JumpTpMove;
        m_StateManager.StringTp.p_exeDelegate = StringTpMove;
        m_StateManager.Falling.p_exeDelegate = FallingMove;
        m_StateManager.BodyBlow.p_exeDelegate = BodyBlowMove;
        m_StateManager.GroundJump.p_exeDelegate = GroundJump;
        m_StateManager.ProximityAttack.p_exeDelegate = ProximityAttack;
        m_Prediction.SetActive(true);
    }

    protected override void Update()
    {
        m_center = transform.position + transform.up * 0.5f;
        transform.position = MoveRange(transform.position, new Vector3(MIN, 0, MIN), new Vector3(MAX, 50f, MAX));
        if (isBodyblow)
        {
            m_StateManager.StateProcassor.State = m_StateManager.Falling;
        }
        //Debug.Log(m_StateManager.StateProcassor.State);
    }

    //移動
    private Vector3 Move(Vector3 right, Vector3 up)
    {
        Vector3 forward = Vector3.Cross(right, up);
        transform.rotation = Quaternion.LookRotation(forward, up);
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
        Vector3 move = forward * vertical + transform.right * horizontal;
        //アニメーション
        m_Animator.SetFloat("MoveX", horizontal);
        m_Animator.SetFloat("MoveZ", vertical);
        vertical = Mathf.Abs(vertical);
        horizontal = Mathf.Abs(horizontal);
        if ((vertical >= 0.6f || horizontal >= 0.6f) && !m_AudioSource.isPlaying)
        {
            m_AudioSource.loop = true;
            m_AudioSource.PlayOneShot(m_AudioClips[5]);
        }
        else if(move == Vector3.zero)
        {
            m_AudioSource.loop = false;
        }
        transform.Translate(move * m_Speed * Time.deltaTime, Space.World);
        return move;
    }

    //ジャンプ
    private void Jump(Ray ray, RaycastHit hit)
    {
        bool jump = false;
        float addLimit = 0;
        //糸を狙うのかどうか
        if (Input.GetKeyUp(KeyCode.K) || Input.GetButtonUp("RB"))
        {
            m_attackTime = 0;
            m_enemy = null;
            isTarget = !isTarget;
        }
        if (hit.collider.tag == "Tree")
        {
            var tree = hit.collider.GetComponent<Tree>();
            if(tree.m_SideNumber == m_Shooter.m_SideNumber)
                addLimit = tree.m_TerritoryRate;
        }
        else if (hit.collider.tag == "String")
        {
            var s = hit.collider.GetComponent<StringUnit>();
            addLimit = Vector3.Distance(s.m_PointA, s.m_PointB);
        }

        if (isTarget)
        {
            if (!(jump = Physics.Raycast(ray, out jump_target, m_JumpLimit + addLimit, m_NetLayer)))
                jump = Physics.SphereCast(ray, 1f, out jump_target, m_JumpLimit + addLimit, m_StringLayer);
        }
        else
            jump = Physics.Raycast(ray, out jump_target, m_JumpLimit + addLimit, m_TreeLayer);

        if (Input.GetKey(KeyCode.K) || Input.GetButton("RB"))
        {
            m_attackTime += Time.deltaTime;
            if (m_attackTime > 1f)
            {
                foreach (Collider col in Physics.OverlapSphere(jump_target.point, 3f, m_EnemyLayer))
                {
                    m_enemy = col;
                }
            }
        }
        if (jump)
        {
            if (hit.collider.gameObject == jump_target.collider.gameObject)
            {
                if (Vector3.Distance(transform.position, jump_target.point) < jumpLower)
                {
                    m_Prediction.SetActive(false);
                    return;
                }
            }
            if (jump_target.transform.tag == "String" || jump_target.transform.tag == "Net")
                if (jump_target.transform.GetComponent<Connecter>().m_SideNumber != m_Shooter.m_SideNumber)
                    return;
            //if (jump_target.collider.tag == "Net")
            //    if (jump_target.transform.GetComponent<Net>().m_SideNumber != m_Shooter.m_SideNumber)
            //        return;

            float dis = Vector3.Distance(transform.position, jump_target.point);
            if (dis > m_JumpLimit)
            {
                if (hit.collider.tag == "Tree")
                    m_JumpMode = JumpMode.CapturingJump;
                else if (hit.collider.tag == "String")
                    m_JumpMode = JumpMode.StringJump;
            }
            else
                m_JumpMode = JumpMode.NormalJump;
            //予測線、カーソル表示
            m_Prediction.SetActive(true);
            if(!isTarget)
            {
                if(m_hitinfo.collider != jump_target.collider)
                    m_Prediction.SetParameter(transform.position, jump_target.point, m_Angle, m_Shooter.m_SideNumber, m_JumpMode, true, TargetCategory.Tree);
                m_Prediction.SetParameter(transform.position, jump_target.point, m_Angle, m_Shooter.m_SideNumber, m_JumpMode, true, TargetCategory.None);
            }
            else if(isTarget)
                m_Prediction.SetParameter(transform.position, jump_target.point, m_Angle, m_Shooter.m_SideNumber, m_JumpMode, true, TargetCategory.None);
            if (m_enemy != null)
                m_Prediction.SetParameter(transform.position, m_enemy.transform.position, m_Angle, m_Shooter.m_SideNumber, m_JumpMode, true, TargetCategory.Enemy);
            m_Prediction.Calculation();
            //ジャンプ
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump"))
            {
                m_Prediction.SetActive(false);
                move_start = transform.position;
                move_end = jump_target.point;
                m_Animator.SetTrigger("Jump");
                m_escapeInterval = 0;
                if (m_enemy != null)
                {
                    Physics.Raycast(m_enemy.transform.position, -m_enemy.transform.up, out jump_target, 1f, m_TreeLayer);
                    move_end = jump_target.point;
                    JumpCalculation(move_start, move_end, m_Angle);
                    m_StateManager.StateProcassor.State = m_StateManager.BodyBlow;
                    return;
                }
                JumpCalculation(move_start, move_end, m_Angle);
                m_StateManager.StateProcassor.State = m_StateManager.JumpTp;
            }
            return;
        }
        else if (!jump && Physics.Raycast(ray, m_JumpLimit + 100f, m_TreeLayer))
        {
            m_Prediction.SetActive(true);
            m_Prediction.SetParameter(
                transform.position,
                m_Camera.position + m_Camera.forward * (m_JumpLimit + addLimit), 
                m_Angle, m_Shooter.m_SideNumber, m_JumpMode, false);
            m_Prediction.Calculation();
            return;
        }
        m_Prediction.SetActive(false);
        m_Prediction.m_HitStringPoint = Vector3.zero;
        m_enemy = null;
    }

    private bool Depression()
    {
        waitFrame++;
        if (waitFrame < 5) return false;
        if(waitFrame == 5)
        {
            m_AudioSource.PlayOneShot(m_AudioClips[4]);
            if(m_StateManager.StateProcassor.State != m_StateManager.GroundJump)
            {
                m_WindLine.Play();
            }
        }
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

    //糸の交差部分移動
    private void IntersectString(int[] layerMask)
    {
        float shortest = 100000f;
        RaycastHit result = new RaycastHit();
        for (int i = 0; i < layerMask.Length; i++)
        {
            if (i > 0 && result.collider != null) break;
            foreach (RaycastHit hit in Physics.SphereCastAll(transform.position + Vector3.up, 0.3f, Vector3.down, 1f, layerMask[i]))
            {
                if ((hit.collider.tag == "String" || hit.collider.tag == "Net")
                    && (hit.collider.GetComponent<Connecter>().m_SideNumber != m_Shooter.m_SideNumber))
                    continue;

                bool isChild = false;
                var connecter = m_hitinfo.collider.GetComponent<Connecter>();
                GameObject hitObject = hit.collider.gameObject;
                foreach (var c in connecter.m_Child)
                {
                    if (c.gameObject != hitObject)
                        continue;
                    isChild = true;
                }

                if(!isChild && (hitObject == connecter.m_StartConnecter.gameObject || hitObject == connecter.m_EndConnecter.gameObject))
                    isChild = true;

                if (isChild)
                    Comparision(hit.point, hit, ref result, ref shortest);
            }
        }
        if (result.collider == null) return;
        m_hitinfo = result;
        if (result.collider.tag == "Net" || result.collider.tag == "Tree")
        {
            m_StateManager.StateProcassor.State = m_StateManager.TreeTp;
        }
        else if(result.collider.tag == "String")
        {
            m_StateManager.StateProcassor.State = m_StateManager.StringTp;
        }
    }
    //一番近い位置を求める
    private void Comparision(Vector3 pos, RaycastHit target, ref RaycastHit result, ref float shortest)
    {
        if (m_hitinfo.collider == target.collider) return;
        float distance = Vector3.Distance(pos, transform.position);
        if (shortest > distance)
        {
            shortest = distance;
            result = target;
        }
    }

    //乗ってる糸の番号が変更した場合
    private bool IsChangedNumber()
    {
        if((m_hitinfo.collider.tag == "Net" && m_hitinfo.collider.GetComponent<Net>().m_SideNumber != m_Shooter.m_SideNumber)
            || (m_hitinfo.collider.tag == "String" && m_hitinfo.collider.GetComponent<StringUnit>().m_SideNumber != m_Shooter.m_SideNumber))
        {
            Fall();
            return true;
        }
        return false;
    }

    //アニメータートリガーをリセット
    private void ResetTrigger()
    {
        m_Animator.ResetTrigger("Jump");
        //m_Animator.ResetTrigger("Failure");
        m_Animator.ResetTrigger("Escape");
        m_Animator.ResetTrigger("Tackle");
        m_Animator.ResetTrigger("NormalJump");
    }
    //ジャンプパラメータをリセット
    private void JumpReset()
    {
        transform.position = move_end;
        transform.rotation = Quaternion.LookRotation(Vector3.Cross(m_Camera.right, jump_target.normal), jump_target.normal);
        switch (m_JumpMode)
        {
            case JumpMode.CapturingJump:
                {
                    m_hitinfo.collider.GetComponent<Tree>().m_TerritoryRate -= Vector3.Distance(move_start, move_end);
                    break;
                }
            case JumpMode.StringJump:
                {
                    m_hitinfo.collider.GetComponent<StringUnit>().Delete();
                    break;
                }
            default:
                {
                    if (isEscape)
                        m_Shooter.StringShoot(m_Prediction.m_HitStringPoint, move_end);
                    else
                        m_Shooter.StringShoot(move_start, move_end);
                    if (m_hitinfo.collider != jump_target.collider && m_hitinfo.collider.tag == "Tree")
                        m_hitinfo.collider.GetComponent<Tree>().m_TerritoryRate -= Vector3.Distance(move_start, move_end);
                    break;
                }
        }
        waitFrame = 0;
        isEscape = false;
        isLanding = false;
        m_Prediction.m_HitStringPoint = Vector3.zero;
        m_EscapeSphere.SetActive(false);
        m_WindLine.Stop();
    }
    //落下着地時の各値リセット
    private void LandingReset(Collider other)
    {
        ResetBodyblow();
        elapse_time = 0;
        m_failureTime = 0;
        m_AudioSource.PlayOneShot(m_AudioClips[3]);
        m_Animator.SetTrigger("Landing");
        if (other.transform.tag == "Ground")
            m_StateManager.StateProcassor.State = m_StateManager.GroundTp;
        else
            m_StateManager.StateProcassor.State = m_StateManager.TreeTp;
    }

    //木に乗ってるかどうか
    public bool IsOnTree()
    {
        if (m_hitinfo.collider == null) return false;
        var state = m_StateManager.StateProcassor.State;
        return (m_hitinfo.collider.tag == "Tree" 
            && (state == m_StateManager.TreeTp || state == m_StateManager.TreeFp));
    }

    //自分の乗っている木を取得
    public GameObject GetOnTree()
    {
        if (m_hitinfo.collider == null) return null;
        var state = m_StateManager.StateProcassor.State;
        if (m_hitinfo.collider.tag == "Tree" && (state == m_StateManager.TreeTp || state == m_StateManager.TreeFp))
            return m_hitinfo.collider.gameObject;
        return null;
    }
    //ターゲットしている木を取得
    public GameObject GetTargetTree()
    {
        if (jump_target.collider == null) return null;
        if (m_hitinfo.collider == null) return null;
        var state = m_StateManager.StateProcassor.State;
        if (state == m_StateManager.TreeTp || state == m_StateManager.TreeFp
            || state == m_StateManager.StringTp || state == m_StateManager.GroundTp)
            return jump_target.collider.gameObject;
        return null;
    }

    void OnTriggerEnter(Collider other)
    {
        //地面着地
        if (m_StateManager.StateProcassor.State != m_StateManager.Falling) return;
        if (other.transform.tag == "Ground" || other.transform.tag == "Tree")
        {
            if (m_hitinfo.transform.tag != other.transform.tag) return;
            LandingReset(other);
        }
    }
}
