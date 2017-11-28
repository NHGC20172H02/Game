using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public float m_Speed = 1f;
    public float m_JumpLimit = 50f;
    public float m_Angle = 30f;
    public PredictionLine m_Prediction;
    public Animator m_Animator;

    private RaycastHit m_hitinfo;
    private Vector3 move_start;
    private Vector3 move_end;
    private RaycastHit jump_target;         //ジャンプの対象
    private bool is_escape = false;

    public StringShooter m_Shooter;
    protected override void Start()
    {
        var instance = PlayerStateManager.GetInstance;
        instance.GroundTp.p_exeDelegate = GroundMove;
        instance.TreeTp.p_exeDelegate = TreeTpMove;
        instance.TreeFp.p_exeDelegate = TreeFpMove;
        instance.JumpTp.p_exeDelegate = JumpTpMove;
        instance.StringTp.p_exeDelegate = StringTpMove;
        instance.Falling.p_exeDelegate = FallingMove;
        m_Prediction.gameObject.SetActive(true);
    }

    protected override void Update()
    {

    }

    //地面にいる場合の移動
    void GroundMove()
    {
        m_Prediction.gameObject.SetActive(false);
        Vector3 start = transform.position + transform.up * 0.5f;
        int layerMask = LayerMask.GetMask(new string[] { "Ground" });
        Physics.Raycast(start, -transform.up, out m_hitinfo, 1f, layerMask);

        Move(Camera.main.transform.right, m_hitinfo.normal);

        //木に登る
        if (Physics.Raycast(start, transform.forward, out m_hitinfo, 1f))
        {
            transform.position = m_hitinfo.point;
            transform.rotation = Quaternion.LookRotation(Vector3.Cross(transform.right, m_hitinfo.normal), m_hitinfo.normal);
            var instance = PlayerStateManager.GetInstance;
            instance.StateProcassor.State = instance.TreeTp;
        }
    }

    //木の上の俯瞰カメラ状態での移動
    void TreeTpMove()
    {
        Vector3 start = transform.position + transform.up * 0.5f;
        int treeLayer = LayerMask.GetMask(new string[] { "Tree" });
        if(!Physics.Raycast(start, -transform.up, out m_hitinfo, 1f, treeLayer))
        {
            Physics.Raycast(start, -transform.up, out m_hitinfo, 1f);
        }

        var instance = PlayerStateManager.GetInstance;
        if (m_hitinfo.transform.tag == "Ground")
            instance.StateProcassor.State = instance.GroundTp;

        transform.position = Vector3.Lerp(transform.position, m_hitinfo.point, 0.2f);
        Move(Camera.main.transform.right, m_hitinfo.normal);

        //一人称視点変更
        if (Input.GetKeyDown(KeyCode.L))
            instance.StateProcassor.State = instance.TreeFp;

        Ray ray = new Ray(start, Camera.main.transform.forward);
        Jump(ray, m_hitinfo);
    }

    //木の上の一人称カメラ状態での操作
    void TreeFpMove()
    {
        Vector3 start = transform.position + transform.up * 0.5f;
        var instance = PlayerStateManager.GetInstance;
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (m_hitinfo.collider.tag == "Tree")
                instance.StateProcassor.State = instance.TreeTp;
            else if (m_hitinfo.collider.tag == "String")
                instance.StateProcassor.State = instance.StringTp;
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
            var instance = PlayerStateManager.GetInstance;
            if (jump_target.transform.tag == "String")
            {
                is_escape = false;
                m_hitinfo = jump_target;
                instance.StateProcassor.State = instance.StringTp;
                return;
            }
            instance.StateProcassor.State = instance.TreeTp;
        }

        int playerLayer = LayerMask.GetMask(new string[] { "Player" });
        if (jump_target.transform.tag == "String")
        {
            int side = jump_target.transform.GetComponent<StringUnit>().m_SideNumber;
            if (side == 1) return;
            //回避可能範囲
            if(Physics.CheckSphere(move_end, 3f, playerLayer))
            {
                //回避失敗
                if (Physics.CheckSphere(move_end, 1f, playerLayer) && !is_escape)
                {
                    int groundLayer = LayerMask.GetMask(new string[] { "Ground" });
                    Ray ray = new Ray(transform.position, -Vector3.up);
                    Physics.Raycast(ray, out m_hitinfo, 100f, groundLayer);
                    var instance = PlayerStateManager.GetInstance;
                    instance.StateProcassor.State = instance.Falling;
                    return;
                }
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Escape(true);
                }
            }
        }
        //else if (m_Prediction.m_HitStringPoint.point != Vector3.zero)
        //{
        //    if (m_Prediction.m_HitStringSide == 1) return;
        //    if (Physics.CheckSphere(m_Prediction.m_HitStringPoint.point, 3f, playerLayer))
        //    {
        //        if (Physics.CheckSphere(m_Prediction.m_HitStringPoint.point, 0.5f, playerLayer) && !is_escape)
        //        {
        //            int groundLayer = LayerMask.GetMask(new string[] { "Ground" });
        //            Ray ray = new Ray(transform.position, -Vector3.up);
        //            Physics.Raycast(ray, out m_hitinfo, 100f, groundLayer);
        //            var instance = PlayerStateManager.GetInstance;
        //            instance.StateProcassor.State = instance.Falling;
        //            return;
        //        }
        //        if (Input.GetKeyDown(KeyCode.Space))
        //        {
        //            move_end = m_Prediction.m_HitStringPoint.point;
        //            jump_target.normal = m_Prediction.m_HitStringPoint.normal;
        //            Escape(false);
        //        }
        //    }
        //}
    }

    //糸の上での移動
    void StringTpMove()
    {
        var instance = PlayerStateManager.GetInstance;
        if (m_hitinfo.collider == null)
            instance.StateProcassor.State = instance.Falling;
        var line = m_hitinfo.collider.GetComponent<LineRenderer>();
        Vector3 stringVec = (line.GetPosition(1) - line.GetPosition(0)).normalized;
        Vector3 stringVertical = Quaternion.Euler(0, 90, 0) * stringVec;
        Vector3 up = Vector3.Cross(Camera.main.transform.right, stringVec);
        float angle = Vector3.Angle(Camera.main.transform.right, stringVec);
        Vector3 vec = Vector3.zero;
        Vector3 move = Vector3.zero;
        if (angle > 45f && angle < 135f)
        {
            if (up.y > 0)
                stringVec = -stringVec;
            vec = stringVec;
            move = stringVec * Input.GetAxis("Vertical");
        }
        else if(angle < 45f)
        {
            vec = -stringVertical;
            move = stringVec * Input.GetAxis("Horizontal");
        }
        else if(angle > 135f)
        {
            vec = stringVertical;
            move = -stringVec * Input.GetAxis("Horizontal");
        }
        transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.forward, vec, 0.2f));
        transform.Translate(move * m_Speed * Time.deltaTime, Space.World);

        //一人称視点変更
        if (Input.GetKeyDown(KeyCode.L))
        {
            instance.StateProcassor.State = instance.TreeFp;
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
        int treeLayer = LayerMask.GetMask(new string[] { "Tree" });
        int stringLayer = LayerMask.GetMask(new string[] { "String" });
        //糸を狙うのかどうか
        if (Input.GetKey(KeyCode.K))
            jump = Physics.SphereCast(ray, 2f, out jump_target, m_JumpLimit, stringLayer);
        else
            jump = Physics.SphereCast(ray, 2f, out jump_target, m_JumpLimit, treeLayer);

        if (jump)
        {
            if (hit.transform == jump_target.transform) return;

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
                var instance = PlayerStateManager.GetInstance;
                instance.StateProcassor.State = instance.JumpTp;
            }
            return;
        }
        m_Prediction.gameObject.SetActive(false);
    }

    //回避
    private void Escape(bool targetString)
    {
        if (targetString)
            jump_target.collider.GetComponent<StringUnit>().SetSide(1);
        else
            m_Prediction.m_HitString.SetSide(1);
        elapse_time = 0;
        is_escape = true;
    }

    void OnTriggerEnter(Collider other)
    {
        //地面着地
        var instance = PlayerStateManager.GetInstance;
        if (instance.StateProcassor.State != instance.Falling) return;
        if (other.transform.tag == "Ground")
        {
            elapse_time = 0;
            instance.StateProcassor.State = instance.GroundTp;
        }
    }
}
