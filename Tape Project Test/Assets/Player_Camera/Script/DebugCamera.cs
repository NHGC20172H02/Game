using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCamera : MonoBehaviour {

    public enum FreeCameraState
    {
        LookChase = 0,
        FreeMove,
        FreeRotate,
        None
    }
    public enum DebugCameraTarget
    {
        Player = 0,
        Enemy
    }

    [Header("対象についていくか")]
    public bool IsMoveChase = true;
    [Header("自由な状態でのモード選択")]
    public FreeCameraState m_State_ = FreeCameraState.LookChase;
    [Header("ターゲットの設定")]
    public DebugCameraTarget m_CameraTarget = DebugCameraTarget.Player;
    public float m_Speed = 2f;
    public float m_RotateSpeed = 100f;
    public float m_ChaseRotateSpeed = 100f;
    public float m_MinAngle = -60f;
    public float m_MaxAngle = 60f;
    public List<GameObject> m_Targets;
    public List<MonoBehaviour> m_Scripts;
    public List<Animator> m_Animators;

    private GameObject target_ = null;
    private Vector3 targetPos_ = Vector3.zero;
    private Vector3 offset_ = Vector3.zero;
    private Vector3 velocity_ = Vector3.zero;
    
	void Start () {
        target_ = m_Targets[0];
        targetPos_ = target_.transform.position + Vector3.up;
        offset_ = transform.position - target_.transform.position;
	}
	
    void FixedUpdate()
    {
        TargetChange();
    }

    void LateUpdate ()
    {
        switch (IsMoveChase)
        {
            case true:
                {
                    CameraChase();
                    break;
                }
            case false:
                {
                    CameraControll();
                    break;
                }
        }
	}

    private void TargetChange()
    {
        int targetNum = (int)m_CameraTarget;
        target_ = m_Targets[targetNum];
        for(int i = 0; i < m_Targets.Count; i++)
        {
            if(i == targetNum)
            {
                m_Scripts[i].enabled = true;
                m_Animators[i].enabled = true;
            }
            else
            {
                m_Scripts[i].enabled = false;
                m_Animators[i].enabled = false;
            }
        }
    }

    private void CameraChase()
    {
        transform.position += target_.transform.position + Vector3.up - targetPos_;
        targetPos_ = target_.transform.position + Vector3.up;

        //回転
        float x = Input.GetAxis("Horizontal2");
        transform.RotateAround(targetPos_, Vector3.up, x * m_ChaseRotateSpeed * Time.deltaTime);
        transform.RotateAround(targetPos_, transform.right, Input.GetAxis("Vertical2") * m_ChaseRotateSpeed * Time.deltaTime);

        //角度制限
        float rotateY = (transform.eulerAngles.y > 180) ? transform.eulerAngles.y - 360 : transform.eulerAngles.y;
        rotateY = (rotateY < 0) ? rotateY + 360 : rotateY;
        float rotateX = (transform.eulerAngles.x > 180) ? transform.eulerAngles.x - 360 : transform.eulerAngles.x;
        float angleX = Mathf.Clamp(rotateX, m_MinAngle, m_MaxAngle);
        angleX = (angleX < 0) ? angleX + 360 : angleX;
        transform.rotation = Quaternion.Euler(angleX, rotateY, 0);
        transform.LookAt(targetPos_);
    }

    private void CameraControll()
    {
        switch (m_State_)
        {
            case FreeCameraState.LookChase:
                {
                    transform.LookAt(target_.transform.position);
                    break;
                }
            case FreeCameraState.FreeMove:
                {
                    FreeMove();
                    break;
                }
            case FreeCameraState.FreeRotate:
                {
                    FreeRotate();
                    break;
                }
            case FreeCameraState.None:
                {
                    m_State_ = FreeCameraState.LookChase;
                    break;
                }
        }

    }

    private void FreeMove()
    {
        Vector3 move = transform.right * Input.GetAxis("Horizontal2");
        if (0 < Input.GetAxis("Zoom"))
        {
            move += -transform.forward * Input.GetAxis("Vertical2");
        }
        else
        {
            move += -transform.up * Input.GetAxis("Vertical2");
        }
        transform.Translate(move * m_Speed * Time.deltaTime, Space.World);
    }

    private void FreeRotate()
    {
        transform.Rotate(Vector3.up, m_RotateSpeed * Input.GetAxis("Horizontal2"));
        transform.Rotate(Vector3.right, m_RotateSpeed * Input.GetAxis("Vertical2"));
        transform.rotation = Quaternion.LookRotation(transform.forward, Vector3.up);
    }
}
