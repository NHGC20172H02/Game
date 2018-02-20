using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDirectionIcon : MonoBehaviour
{
    public Transform m_Player;
    public Transform m_Enemy;
    public Transform m_CameraPivot;
    public Camera m_MainCamera;
    public RectTransform m_DirectionIcon;

    void Update()
    {
        Vector3 enemyPos = m_Enemy.position;
        //ビューポート変換
        Vector3 viewPos = m_MainCamera.WorldToViewportPoint(enemyPos);
        //カメラに映っていたら表示しない
        if (viewPos.x > 0 && viewPos.x < 1.0f && viewPos.y > 0 && viewPos.y < 1.0f && viewPos.z > 0)
        {
            m_DirectionIcon.gameObject.SetActive(false);
            return;
        }
        else
            m_DirectionIcon.gameObject.SetActive(true);

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
        m_DirectionIcon.localPosition = screenDir * 1000f;

        float posX = m_DirectionIcon.localPosition.x;
        float posY = m_DirectionIcon.localPosition.y;
        //画面内に収める
        m_DirectionIcon.localPosition = new Vector2(Mathf.Clamp(posX, -600, 600), Mathf.Clamp(posY, -330, 330));

        //敵の方向へ回転
        m_DirectionIcon.localRotation = Quaternion.AngleAxis(-angle * Mathf.Rad2Deg, Vector3.forward);
    }
}
