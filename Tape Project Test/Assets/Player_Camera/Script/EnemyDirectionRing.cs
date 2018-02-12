using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDirectionRing : MonoBehaviour {

    public Transform m_Player;
    public Transform m_Enemy;
    public Transform m_CameraPivot;
    public Transform m_Ring;

    static readonly Vector3 offset = new Vector3(0, 0.2f, 0);

    void LateUpdate () {
        m_Ring.position = m_Player.position + offset;
        Vector3 enemyPos = m_Enemy.position;
        Vector3 playerPos = m_Player.position;
        Vector3 dir = (enemyPos - playerPos);
        //Vector3 forward = Vector3.Cross(m_CameraPivot.right, Vector3.up);
        //float angle = Mathf.Atan2(dir.x, dir.z) - Mathf.Atan2(forward.x, forward.z);

        dir.y = 0;
        m_Ring.rotation = Quaternion.LookRotation(dir, Vector3.up);
    }
}
