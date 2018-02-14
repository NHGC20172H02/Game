using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{

    public GameObject m_target;
    public Transform m_Spider;
    public Transform m_Camera;
    public float rotate_Speed = 100f;
    public float minAngle = -60, maxAngle = 60;

    private PlayerStateManager m_StateManager;
    private Animator m_Animator;
    private Vector3 target_pos;

    //三人称用のカメラとの距離
    private float g_distance;           //地面上
    private float t_distance;           //木の上
    private Vector3 m_velocity;

    // Use this for initialization
    void Start()
    {
        m_StateManager = m_target.GetComponent<Player>().m_StateManager;
        m_StateManager.GroundTp.c_exeDelegate = GroundMove;
        m_StateManager.TreeTp.c_exeDelegate = TreeTpMove;
        m_StateManager.TreeFp.c_exeDelegate = TreeFpMove;
        m_StateManager.JumpTp.c_exeDelegate = JumpTpMove;
        m_StateManager.StringTp.c_exeDelegate = TreeTpMove;
        m_StateManager.Falling.c_exeDelegate = TreeTpMove;
        m_StateManager.BodyBlow.c_exeDelegate = TreeTpMove;
        m_StateManager.GroundJump.c_exeDelegate = TreeTpMove;
        m_StateManager.ProximityAttack.c_exeDelegate = TreeTpMove;
        g_distance = Vector3.Distance(transform.position, m_Camera.position);
        t_distance = g_distance * 2f;
        m_Animator = m_target.GetComponent<Player>().m_Animator;
    }

    // Update is called once per frame
    void Update()
    {
        target_pos = m_target.transform.position + m_target.transform.up * 0.2f;
        transform.position = target_pos + Vector3.up * 1.2f;
        //自機基準回転
        if (Input.GetKey(KeyCode.E))
        {
            transform.RotateAround(target_pos, Vector3.up, rotate_Speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.RotateAround(target_pos, Vector3.up, -rotate_Speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.Z))
        {
            transform.RotateAround(target_pos, transform.right, rotate_Speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.X))
        {
            transform.RotateAround(target_pos, transform.right, -rotate_Speed * Time.deltaTime);
        }
        //コントローラ用
        float x = Input.GetAxis("Horizontal2");
        transform.RotateAround(target_pos, Vector3.up, x * rotate_Speed * Time.deltaTime);
        transform.RotateAround(target_pos, transform.right, Input.GetAxis("Vertical2") * rotate_Speed * Time.deltaTime);
        if(x != 0)
            m_Animator.SetFloat("MoveX", x);
        //角度制限
        float rotateY = (transform.eulerAngles.y > 180) ? transform.eulerAngles.y - 360 : transform.eulerAngles.y;
        rotateY = (rotateY < 0) ? rotateY + 360 : rotateY;
        float rotateX = (transform.eulerAngles.x > 180) ? transform.eulerAngles.x - 360 : transform.eulerAngles.x;
        float angleX = Mathf.Clamp(rotateX, minAngle, maxAngle);
        angleX = (angleX < 0) ? angleX + 360 : angleX;
        transform.rotation = Quaternion.Euler(angleX, rotateY, 0);

        Color current_color = m_Spider.GetComponent<Renderer>().material.color;
        m_Spider.GetComponent<Renderer>().material.color
            = new Color(current_color.r, current_color.g, current_color.b, Mathf.Lerp(current_color.a, 1f, 0.2f));
    }

    void GroundMove()
    {
        Vector3 to_pos = transform.position - transform.forward * g_distance;
        Move(to_pos, 1.0f, 0.4f, 0.6f);
    }

    void TreeTpMove()
    {
        if (Vector3.Distance(transform.position, m_Camera.position) < t_distance)
        {
            Vector3 to_pos = transform.position - transform.forward * t_distance;
            Move(to_pos, 1.0f, 0.2f, 0.6f);
        }

        //自機とカメラの間に木があるとカメラを自機によせる
        int treeLayer = LayerMask.GetMask(new string[] { "Tree" });
        int groundLayer = LayerMask.GetMask(new string[] { "Ground" });
        RaycastHit hit;
        if (Physics.Raycast(target_pos, m_Camera.position - target_pos, out hit, Vector3.Distance(m_Camera.position, target_pos), treeLayer)
            || Physics.Raycast(target_pos, m_Camera.position - target_pos, out hit, Vector3.Distance(m_Camera.position, target_pos), groundLayer))
        {
            m_Camera.position = Vector3.Lerp(m_Camera.position, hit.point + Vector3.up, 0.5f);
            Move(hit.point, 1.0f, 0.2f, 0.6f);
            Color current_color = m_Spider.GetComponent<Renderer>().material.color;
            m_Spider.GetComponent<Renderer>().material.color
                = new Color(current_color.r, current_color.g, current_color.b, Mathf.Lerp(current_color.a, 0.0f, 0.2f));
        }
    }

    void TreeFpMove()
    {
        Move(transform.position, 1.0f, 0.2f, 0.6f);
    }

    void JumpTpMove()
    {
        if (Vector3.Distance(transform.position, m_Camera.position) < t_distance)
        {
            Vector3 to_pos = transform.position + -transform.forward * t_distance;
            Move(to_pos, 1.0f, 0.2f, 0.6f);
            m_Camera.rotation = Quaternion.LookRotation(Vector3.Lerp(m_Camera.forward, transform.position - m_Camera.position, 0.3f));
        }
    }

    void Move(Vector3 targetPos, float stiffnes, float friction, float mass)
    {
        //バネの伸び具合を計算
        var stretch = m_Camera.position - targetPos;
        //バネの力を計算
        var force = -stiffnes * stretch;
        //加速度を追加
        var accelerration = force / mass;
        //移動速度を計算
        m_velocity = friction * (m_velocity + accelerration);
        //座標の更新
        m_Camera.position += m_velocity;
    }
}