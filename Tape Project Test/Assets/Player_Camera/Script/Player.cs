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
    public LayerMask m_GroundLayer;
    public LayerMask m_TreeLayer;
    public LayerMask m_StringLayer;
    public PredictionLine m_Prediction;
    public Animator m_Animator;

    private PlayerStateManager m_StateManager;       //Player状態管理
    private RaycastHit m_hitinfo;                    //足元の情報
    private Vector3 move_start;                      //ジャンプ始点
    private Vector3 move_end;                        //ジャンプ終点
    private RaycastHit jump_target;
    private bool is_escape = false;

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

        Move(Camera.main.transform.right, m_hitinfo.normal);

        //木に登る
        if (Physics.Raycast(start, transform.forward, out m_hitinfo, 1f))
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
        //足元の情報取得（木を優先）
        if(!Physics.Raycast(start, -transform.up, out m_hitinfo, 1f, m_TreeLayer))
            Physics.Raycast(start, -transform.up, out m_hitinfo, 1f, m_GroundLayer);

        if (m_hitinfo.collider.tag == "Ground")
            m_StateManager.StateProcassor.State = m_StateManager.GroundTp;

        //位置補正
        transform.position = Vector3.Lerp(transform.position, m_hitinfo.point, 0.2f);
        Move(Camera.main.transform.right, m_hitinfo.normal);

        //一人称視点変更
        if (Input.GetKeyDown(KeyCode.L))
            m_StateManager.StateProcassor.State = m_StateManager.TreeFp;

        Ray ray = new Ray(start, Camera.main.transform.forward);
        Jump(ray, m_hitinfo);
    }

    //木の上の一人称カメラ状態での操作
    void TreeFpMove()
    {
        Vector3 start = transform.position + transform.up * 0.5f;
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (m_hitinfo.collider.tag == "Tree")
                m_StateManager.StateProcassor.State = m_StateManager.TreeTp;
            else if (m_hitinfo.collider.tag == "String")
                m_StateManager.StateProcassor.State = m_StateManager.StringTp;
        }

        Ray ray = new Ray(start, Camera.main.transform.forward);
        Jump(ray, m_hitinfo);
    }

    //ジャンプ中の動作
    void JumpTpMove()
    {
        if (Projection(move_start, move_end, jump_target.normal, m_Angle) || is_escape)
        {
            transform.position = jump_target.point;
            transform.rotation = Quaternion.LookRotation(Vector3.Cross(Camera.main.transform.right, jump_target.normal), jump_target.normal);
            m_Shooter.StringShoot(move_start, move_end);
            m_Animator.SetTrigger("Landing");
            if (jump_target.collider.tag == "String")
            {
                is_escape = false;
                m_hitinfo = jump_target;
                m_StateManager.StateProcassor.State = m_StateManager.StringTp;
                m_Animator.SetBool("IsString", true);
                return;
            }
            m_StateManager.StateProcassor.State = m_StateManager.TreeTp;
            m_Animator.SetBool("IsString", false);
        }

        int playerLayer = LayerMask.GetMask(new string[] { "Player" });
        if (jump_target.collider.tag == "String")
        {
            int side = jump_target.collider.GetComponent<StringUnit>().m_SideNumber;
            if (side == 1) return;
            //回避可能範囲
            if(Physics.CheckSphere(move_end, 3f, playerLayer))
            {
                //回避失敗
                if (Physics.CheckSphere(move_end, 1f, playerLayer) && !is_escape)
                {
                    Fall();
                    return;
                }
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Escape(true);
                }
            }
        }
        else if (m_Prediction.m_HitStringPoint.point != Vector3.zero)
        {
            if (m_Prediction.m_HitStringSide == 1) return;
            if (Physics.CheckSphere(m_Prediction.m_HitStringPoint.point, 3f, playerLayer))
            {
                if (Physics.CheckSphere(m_Prediction.m_HitStringPoint.point, 0.5f, playerLayer) && !is_escape)
                {
                    Fall();
                    return;
                }
                //回避行動
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Vector3 dir = (m_Prediction.m_HitStringPoint.point - transform.position).normalized;
                    Ray ray = new Ray(transform.position, dir);
                    Physics.Raycast(ray, out jump_target);
                    move_end = jump_target.point;
                    Escape(false);
                }
            }
        }
    }

    //糸の上での移動
    void StringTpMove()
    {
        m_Animator.SetBool("IsString", true);
        if (m_hitinfo.collider == null)
            m_StateManager.StateProcassor.State = m_StateManager.Falling;
        var line = m_hitinfo.collider.GetComponent<LineRenderer>();
        //糸のベクトル
        Vector3 stringVec = (line.GetPosition(1) - line.GetPosition(0)).normalized;
        //糸のベクトルを90度回転したベクトル
        Vector3 stringVertical = Quaternion.Euler(0, 90, 0) * stringVec;
        //上方向ベクトル
        Vector3 up = Vector3.Cross(Camera.main.transform.right, stringVec);
        float angle = Vector3.Angle(Camera.main.transform.right, stringVec);
        Vector3 vec = Vector3.zero;
        Vector3 move = Vector3.zero;
        //４方向に向きを変えていく
        if (angle > 45f && angle < 135f)
        {
            if (up.y > 0)
                stringVec = -stringVec;
            vec = stringVec;
            move = stringVec * Input.GetAxis("Vertical");
            m_Animator.SetFloat("StringMoveZ", Input.GetAxis("Vertical"));
        }
        else if(angle < 45f)
        {
            vec = -stringVertical;
            move = stringVec * Input.GetAxis("Horizontal");
            m_Animator.SetFloat("StringMoveX", Input.GetAxis("Horizontal"));
        }
        else if(angle > 135f)
        {
            vec = stringVertical;
            move = -stringVec * Input.GetAxis("Horizontal");
            m_Animator.SetFloat("StringMoveX", Input.GetAxis("Horizontal"));
        }
        transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.forward, vec, 0.2f));
        transform.Translate(move * m_Speed * Time.deltaTime, Space.World);

        //一人称視点変更
        if (Input.GetKeyDown(KeyCode.L))
        {
            m_StateManager.StateProcassor.State = m_StateManager.TreeFp;
        }

        Vector3 start = transform.position + transform.up * 0.5f;
        Ray ray = new Ray(start, Camera.main.transform.forward);
        Jump(ray, m_hitinfo);
    }

    //落下状態での動作
    void FallingMove()
    {
        //落下スピード
        Vector3 fallSpeed = Physics.gravity.y * Vector3.up;
        transform.Translate(fallSpeed * Time.deltaTime, Space.World);
        Vector3 forward = Vector3.Cross(Camera.main.transform.right, m_hitinfo.normal);
        transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.forward, forward, 0.1f), m_hitinfo.normal);
    }

    //移動
    private void Move(Vector3 right, Vector3 up)
    {
        Vector3 forward = Vector3.Cross(right, up);
        transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.forward, forward, 0.3f), up);
        Vector3 move = forward * Input.GetAxis("Vertical") + right * Input.GetAxis("Horizontal");
        //アニメーション
        m_Animator.SetFloat("MoveX", Input.GetAxis("Horizontal"));
        m_Animator.SetFloat("MoveZ", Input.GetAxis("Vertical"));
        transform.Translate(move * m_Speed * Time.deltaTime, Space.World);
    }

    //ジャンプ
    private void Jump(Ray ray, RaycastHit hit)
    {
        bool jump;
        //糸を狙うのかどうか
        if (Input.GetKey(KeyCode.K))
            jump = Physics.SphereCast(ray, 2f, out jump_target, m_JumpLimit, m_StringLayer);
        else
            jump = Physics.SphereCast(ray, 2f, out jump_target, m_JumpLimit, m_TreeLayer);
        if (jump)
        {
            if (hit.collider == jump_target.collider) return;

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

    //回避
    private void Escape(bool targetString)
    {
        elapse_time = 0;
        is_escape = true;
    }

    //落下
    private void Fall()
    {
        //落下先情報取得（木を優先）
        Ray ray = new Ray(transform.position, -Vector3.up);
        if(!Physics.Raycast(ray, out m_hitinfo, 100f, m_TreeLayer))
            Physics.Raycast(ray, out m_hitinfo, 100f, m_GroundLayer);
        m_StateManager.StateProcassor.State = m_StateManager.Falling;
    }

    void OnTriggerEnter(Collider other)
    {
        //地面着地
        if (m_StateManager.StateProcassor.State != m_StateManager.Falling) return;
        if (other.transform.tag == "Ground")
        {
            elapse_time = 0;
            m_Animator.SetTrigger("Landing");
            m_StateManager.StateProcassor.State = m_StateManager.GroundTp;
        }

    }
}
