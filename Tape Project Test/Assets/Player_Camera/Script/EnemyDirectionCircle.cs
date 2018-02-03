using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDirectionCircle : MonoBehaviour
{
    static readonly Vector3 offset = new Vector3(0, 0.2f, 0);

    public Transform m_Player;
    public Transform m_Enemy;
    public Transform m_CameraPivot;
    public Camera m_MainCamera;
    public GameObject m_Circle;
    public Transform m_Ring;

    private RectTransform m_directionIcon; 

    void Start()
    {
        m_directionIcon = m_Circle.transform.GetChild(0).GetComponent<RectTransform>();
    }

    void LateUpdate()
    {
        float dis = Vector3.Distance(m_Player.position, m_MainCamera.transform.position);
        if(dis < 2.5f)
        {
            m_Ring.gameObject.SetActive(false);
            m_Circle.SetActive(true);
            CircleUpdate();
        }
        else
        {
            m_Circle.gameObject.SetActive(false);
            m_Ring.gameObject.SetActive(true);
            RingUpdate();

        }
    }

    private void CircleUpdate()
    {
        Vector3 enemyPos = m_Enemy.position;
        //ビューポート変換
        Vector3 viewPos = m_MainCamera.WorldToViewportPoint(enemyPos);
        //カメラに映っていたら表示しない
        if (viewPos.x > 0 && viewPos.x < 1.0f && viewPos.y > 0 && viewPos.y < 1.0f && viewPos.z > 0)
        {
            m_directionIcon.gameObject.SetActive(false);
            return;
        }
        else
            m_directionIcon.gameObject.SetActive(true);

        Vector3 playerPos = m_Player.position;
        Vector3 dir = (enemyPos - playerPos).normalized;
        //Playerの前方向ベクトル
        Vector3 forward = Vector3.Cross(m_CameraPivot.transform.right, Vector3.up);

        //前方向から敵の方向の角度(Degree)
        float angle = Mathf.Atan2(dir.x, dir.z) - Mathf.Atan2(forward.x, forward.z);

        //スクリーン座標系での敵への方向ベクトル
        Vector3 screenDir = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.up) * Vector3.forward;
        screenDir = new Vector3(screenDir.x, screenDir.z, 0);

        //上下反転
        if (m_CameraPivot.transform.forward.y > 0 && screenDir.y > 0)
        {
            screenDir = new Vector3(screenDir.x, -screenDir.y, 0);
            angle -= Mathf.PI + angle * 2;
        }
        //移動
        m_directionIcon.localPosition = screenDir * 250f;

        float posX = m_directionIcon.localPosition.x;
        float posY = m_directionIcon.localPosition.y;

        //敵の方向へ回転
        m_directionIcon.localRotation = Quaternion.AngleAxis(-angle * Mathf.Rad2Deg, Vector3.forward);
    }

    private void RingUpdate()
    {
        m_Ring.position = m_Player.position + offset;
        Vector3 enemyPos = m_Enemy.position;
        Vector3 playerPos = m_Player.position;
        Vector3 dir = (enemyPos - playerPos);

        dir.y = 0;
        m_Ring.rotation = Quaternion.LookRotation(dir, Vector3.up);
    }
}
