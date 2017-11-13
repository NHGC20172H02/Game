using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{

    [SerializeField]
    GameObject m_target;

    [SerializeField]
    float rotate_Speed = 100f;

    Vector3 target_pos;

    //三人称用のカメラとの距離
    float g_distance;           //地面上
    float t_distance;           //木の上

    // Use this for initialization
    void Start()
    {
        var instance = PlayerStateManager.GetInstance;
        instance.GroundTp.c_exeDelegate = GroundMove;
        instance.TreeTp.c_exeDelegate = TreeTpMove;
        instance.TreeFp.c_exeDelegate = TreeFpMove;
        instance.JumpTp.c_exeDelegate = JumpTpMove;
        instance.StringTp.c_exeDelegate = TreeTpMove;
        g_distance = Vector3.Distance(transform.position, Camera.main.transform.position) * 1.1f;
        t_distance = g_distance * 2f;
    }

    // Update is called once per frame
    void Update()
    {
        target_pos = m_target.transform.position + m_target.transform.up * 0.2f;
        transform.position = target_pos + Vector3.up * 1.2f;
        //自機基準回転
        if (Input.GetKey(KeyCode.E))
        {
            transform.RotateAround(target_pos, transform.up, rotate_Speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.RotateAround(target_pos, transform.up, -rotate_Speed * Time.deltaTime);
        }
        //if (Input.GetKey(KeyCode.Z))
        //{
        //    transform.RotateAround(target_pos, transform.right, rotate_Speed * Time.deltaTime);
        //}
        //if (Input.GetKey(KeyCode.X))
        //{
        //    transform.RotateAround(target_pos, transform.right, -rotate_Speed * Time.deltaTime);
        //}
        Color current_color = m_target.transform.GetChild(0).GetComponent<Renderer>().material.color;
        m_target.transform.GetChild(0).GetComponent<Renderer>().material.color 
            = new Color(current_color.r, current_color.g, current_color.b, Mathf.Lerp(current_color.a, 1f, 0.2f));
    }

    void GroundMove()
    {
        if (Vector3.Distance(transform.position, Camera.main.transform.position) > g_distance)
        {
            Vector3 to_pos = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z - g_distance);
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, to_pos, 0.2f);
            //Camera.main.transform.LookAt(transform.position);
        }
    }

    void TreeTpMove()
    {
        if (Vector3.Distance(transform.position, Camera.main.transform.position) < t_distance)
        {
            Vector3 to_pos = transform.position - transform.forward * t_distance;
            Transform c_transform = Camera.main.transform;
            Camera.main.transform.position = Vector3.Lerp(c_transform.position, to_pos, 0.3f);
        }

        //自機とカメラの間に木があるとカメラを自機によせる
        int treeLayer = LayerMask.GetMask(new string[] { "Tree" });
        RaycastHit hit;
        if (Physics.Raycast(
            target_pos,
            Camera.main.transform.position - target_pos,
            out hit,
            Vector3.Distance(Camera.main.transform.position, target_pos),
            treeLayer))
        {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, hit.point, 0.5f);
            Color current_color = m_target.transform.GetChild(0).GetComponent<Renderer>().material.color;
            m_target.transform.GetChild(0).GetComponent<Renderer>().material.color
                = new Color(current_color.r, current_color.g, current_color.b, Mathf.Lerp(current_color.a, 0.0f, 0.2f));
        }
    }

    void TreeFpMove()
    {
        Transform c_transform = Camera.main.transform;
        Camera.main.transform.position = Vector3.Lerp(c_transform.position, transform.position, 0.3f);
    }

    void JumpTpMove()
    {
        if (Vector3.Distance(transform.position, Camera.main.transform.position) < t_distance)
        {
            Vector3 to_pos = transform.position + -transform.forward * t_distance;
            Transform c_transform = Camera.main.transform;
            Camera.main.transform.position = Vector3.Lerp(c_transform.position, to_pos, 0.3f);
            Camera.main.transform.rotation = Quaternion.LookRotation(Vector3.Lerp(c_transform.forward, transform.position - c_transform.position, 0.3f));
        }
    }
}