using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDirectionUI : MonoBehaviour {

    public Camera m_Camera;
    public Transform m_Enemy;
    public Transform m_CameraPivot;
    public RectTransform m_AlertImage;
    public Vector3 UpPosition;
    public Vector3 BottomPosition;
    public Vector3 RightPosition;
    public Vector3 LeftPosition;
	
	void Update () {
        Vector3 right = m_CameraPivot.right;
        Vector3 forward = Vector3.Cross(right, Vector3.up);
        Vector3 dir = (m_Enemy.position - m_CameraPivot.position).normalized;
        dir.y = 0;
        Vector3 up = Vector3.Cross(forward, dir);
        float angle = Vector3.Angle(forward, dir);
        Vector3 viewPos = m_Camera.WorldToViewportPoint(m_Enemy.position);

        //カメラに映っているか、前にいるか
        var r = m_AlertImage.localRotation;
        if ((viewPos.x > 0 && viewPos.x < 1.0f && viewPos.y > 0 && viewPos.y < 1.0f && viewPos.z > 0)
            || angle < 45f)
        {
            m_AlertImage.localPosition = UpPosition;
            m_AlertImage.localRotation = Quaternion.AngleAxis(180, Vector3.forward);
        }
        else if (angle > 45f && angle < 135f)
        {
            //右にいるか
            if (up.y > 0)
            {
                m_AlertImage.localPosition = RightPosition;
                m_AlertImage.localRotation = Quaternion.AngleAxis(90, Vector3.forward);
                return;
            }
            //左にいるか
            m_AlertImage.localPosition = LeftPosition;
            m_AlertImage.localRotation = Quaternion.AngleAxis(-90, Vector3.forward);
        }
        else if (angle > 135f)
        {
            //後ろにいるか
            m_AlertImage.localPosition = BottomPosition;
            m_AlertImage.localRotation = Quaternion.AngleAxis(0, Vector3.forward);
        }
    }
}
