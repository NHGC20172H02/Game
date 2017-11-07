using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [SerializeField]
    float m_speed = 1f;

    [SerializeField]
    float m_jumpDistance = 50f;

    [SerializeField]
    LayerMask layerMask;

    private Vector3 jump_start;
    private Vector3 jump_end;
    private RaycastHit jump_target;         //ジャンプの対象

	public StringShooter m_Shooter;
	void Start () {
        var instance = CameraStateManager.GetInstance;
        instance.GroundTp.p_exeDelegate = GroundMove;
        instance.TreeTp.p_exeDelegate = TreeTpMove;
        instance.TreeFp.p_exeDelegate = TreeFpMove;
        instance.JumpTp.p_exeDelegate = JumpTpMove;
	}
	
	void Update () {

    }

    //地面にいる場合の移動
    void GroundMove()
    {
        transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.forward, Camera.main.transform.forward, 0.3f));
        Vector3 move = Camera.main.transform.forward * Input.GetAxis("Vertical") + Camera.main.transform.right * Input.GetAxis("Horizontal");
        transform.Translate(move * Time.deltaTime * m_speed, Space.World);

        RaycastHit hit;
        if (Physics.Raycast(transform.position + transform.up * 0.5f, transform.forward, out hit, 1))
        {
            transform.position = hit.point;
            transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.forward, Vector3.Cross(transform.right, hit.normal), 0.2f), hit.normal);
            var instance = CameraStateManager.GetInstance;
            instance.StateProcassor.State = instance.TreeTp;
        }
    }

    //木の上の俯瞰カメラ状態での移動
    void TreeTpMove()
    {
        RaycastHit hit;
        Vector3 start = transform.position + transform.up * 0.5f;
        Physics.Raycast(start, -transform.up, out hit, 1);

        transform.position = Vector3.Lerp(transform.position, hit.point, 0.5f);
        transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.forward, Vector3.Cross(Camera.main.transform.right, hit.normal), 0.3f), hit.normal);
        
        Vector3 forward = Vector3.Cross(Camera.main.transform.right, hit.normal);
        Vector3 move = forward * Input.GetAxis("Vertical") + Camera.main.transform.right * Input.GetAxis("Horizontal");
        transform.Translate(move * Time.deltaTime * m_speed, Space.World);

        //一人称視点変更
        var instance = CameraStateManager.GetInstance;
        if (Input.GetKeyDown(KeyCode.L))
        {
            instance.StateProcassor.State = instance.TreeFp;
        }

        start = transform.position + transform.up * 0.5f;
        Ray ray = new Ray(start, Camera.main.transform.forward);
        //if (Physics.Raycast(ray, out jump_target, m_jumpDistance))
        //{
        //    if (hit.transform == jump_target.transform) return;

        //    //ジャンプ
        //    if (Input.GetKey(KeyCode.Space))
        //    {
        //        jump_start = transform.position;
        //        jump_end = jump_target.point;
        //        instance.StateProcassor.State = instance.JumpTp;
        //    }
        //}
        if (Physics.SphereCast(ray, 2f, out jump_target, m_jumpDistance))
        {
            if (hit.transform == jump_target.transform) return;

            //ジャンプ
            if (Input.GetKey(KeyCode.Space))
            {
                jump_start = transform.position;
                jump_end = jump_target.point;
                instance.StateProcassor.State = instance.JumpTp;
            }
        }
    }

    //木の上の一人称カメラ状態での操作
    void TreeFpMove()
    {
        RaycastHit hit;
        Vector3 start = transform.position + transform.up * 0.5f;
        Physics.Raycast(start, -transform.up, out hit, 1);
        var instance = CameraStateManager.GetInstance;

        if (Input.GetKeyDown(KeyCode.L))
        {
            instance.StateProcassor.State = instance.TreeTp;
        }

        start = transform.position + transform.up * 0.5f;
        Ray ray = new Ray(start, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out jump_target, m_jumpDistance))
        {
            if (hit.transform == jump_target.transform) return;

            //ジャンプ
            if (Input.GetKey(KeyCode.Space))
            {
                jump_start = transform.position;
                jump_end = jump_target.point;
                instance.StateProcassor.State = instance.JumpTp;
            }
        }
    }

    //木から木へとジャンプする際の動作
    void JumpTpMove()
    {
        if(Vector3.Distance(transform.position, jump_target.point) < 0.5f)
        {
            transform.position = jump_target.point;
            transform.rotation = Quaternion.LookRotation(Vector3.Cross(Camera.main.transform.right, jump_target.normal), jump_target.normal);
            var instance = CameraStateManager.GetInstance;
            instance.StateProcassor.State = instance.TreeTp;
			m_Shooter.StringShoot(jump_start, jump_end);
		}

        transform.position = Vector3.Slerp(transform.position, jump_target.point, 0.1f);
        transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.forward, Vector3.Cross(Camera.main.transform.right, jump_target.normal), 0.5f), jump_target.normal);
    }
}
