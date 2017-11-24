using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public float m_speed = 1f;

    public float m_jumpLimit = 50f;

    public Canvas m_cursor;

    private RaycastHit m_hitinfo;
    private Vector3 jump_start;
    private Vector3 jump_end;
    private RaycastHit jump_target;         //ジャンプの対象

    public StringShooter m_Shooter;
    protected override void Start()
    {
        var instance = PlayerStateManager.GetInstance;
        instance.GroundTp.p_exeDelegate = GroundMove;
        instance.TreeTp.p_exeDelegate = TreeTpMove;
        instance.TreeFp.p_exeDelegate = TreeFpMove;
        instance.JumpTp.p_exeDelegate = JumpTpMove;
        instance.StringTp.p_exeDelegate = StringTpMove;
    }

    protected override void Update()
    {

    }

    //地面にいる場合の移動
    void GroundMove()
    {
        m_cursor.gameObject.SetActive(false);
        //RaycastHit m_hitinfo;
        Vector3 start = transform.position + transform.up * 0.5f;
        int layerMask = LayerMask.GetMask(new string[] { "Ground" });
        Physics.Raycast(start, -transform.up, out m_hitinfo, 1f, layerMask);

        transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.forward, Vector3.Cross(Camera.main.transform.right, m_hitinfo.normal), 0.3f), m_hitinfo.normal);
        Vector3 move = Camera.main.transform.forward * Input.GetAxis("Vertical") + Camera.main.transform.right * Input.GetAxis("Horizontal");
        transform.Translate(move * Time.deltaTime * m_speed, Space.World);

        if (Physics.Raycast(start, transform.forward, out m_hitinfo, 1))
        {
            transform.position = m_hitinfo.point;
            transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.forward, Vector3.Cross(transform.right, m_hitinfo.normal), 0.2f), m_hitinfo.normal);
            var instance = PlayerStateManager.GetInstance;
            instance.StateProcassor.State = instance.TreeTp;
        }
    }

    //木の上の俯瞰カメラ状態での移動
    void TreeTpMove()
    {
        RaycastHit hit;
        Vector3 start = transform.position + transform.up * 0.5f;
        int treeLayer = LayerMask.GetMask(new string[] { "Tree" ,"Net"});
        Physics.Raycast(start, -transform.up, out hit, 1, treeLayer);

        var instance = PlayerStateManager.GetInstance;
        if (hit.transform.tag == "Ground")
        {
            instance.StateProcassor.State = instance.GroundTp;
        }

        transform.position = Vector3.Lerp(transform.position, hit.point, 0.2f);
        Vector3 forward = Vector3.Cross(Camera.main.transform.right, hit.normal);
        transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.forward, forward, 0.3f), hit.normal);

        Vector3 move = forward * Input.GetAxis("Vertical") + Camera.main.transform.right * Input.GetAxis("Horizontal");
        transform.Translate(move * Time.deltaTime * m_speed, Space.World);

        //一人称視点変更
        if (Input.GetKeyDown(KeyCode.L))
        {
            instance.StateProcassor.State = instance.TreeFp;
        }

        Ray ray = new Ray(start, Camera.main.transform.forward);
        Jump(ray, hit);
    }

    //木の上の一人称カメラ状態での操作
    void TreeFpMove()
    {
        RaycastHit hit;
        Vector3 start = transform.position + transform.up * 0.5f;
        Physics.Raycast(start, -transform.up, out hit, 1);
        var instance = PlayerStateManager.GetInstance;

        if (Input.GetKeyDown(KeyCode.L))
        {
            instance.StateProcassor.State = instance.TreeTp;
        }

        Ray ray = new Ray(start, Camera.main.transform.forward);
        Jump(ray, hit);
    }

    //木から木へとジャンプする際の動作
    void JumpTpMove()
    {
        if (Projection(jump_start, jump_end, jump_target.normal, 30f))
        {
            transform.position = jump_target.point;
            transform.rotation = Quaternion.LookRotation(Vector3.Cross(Camera.main.transform.right, jump_target.normal), jump_target.normal);
            var instance = PlayerStateManager.GetInstance;
            instance.StateProcassor.State = instance.TreeTp;
            m_Shooter.StringShoot(jump_start, jump_end);
        }
    }

    //糸の上での移動
    void StringTpMove()
    {

    }

    //木から木へとジャンプ
    private void Jump(Ray ray, RaycastHit hit)
    {
        int treeLayer = LayerMask.GetMask(new string[] { "Tree", "Net" });
        if (Physics.SphereCast(ray, 2f, out jump_target, m_jumpLimit, treeLayer))
        {
            if (hit.transform == jump_target.transform) return;

            //カーソル表示
            m_cursor.gameObject.SetActive(true);
            m_cursor.transform.position = jump_target.point + jump_target.normal * 0.1f;
            m_cursor.transform.rotation = Quaternion.LookRotation(jump_target.normal);

            //ジャンプ
            if (Input.GetKey(KeyCode.Space))
            {
                m_cursor.gameObject.SetActive(false);
                jump_start = transform.position;
                jump_end = jump_target.point;
                var instance = PlayerStateManager.GetInstance;
                instance.StateProcassor.State = instance.JumpTp;
            }
            return;
        }
        m_cursor.gameObject.SetActive(false);
    }
}
