using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player {

    private Vector3 m_enemyBottom = Vector3.zero;

    //体当たり状態
    void BodyBlowMove()
    {
        var enemy = m_Enemy.GetComponent<EnemyAI4>();
        if (enemy.TreeDist())
        {
            move_end = m_Enemy.transform.position;
            m_enemyBottom = -m_Enemy.transform.up;
        }
        if (BodyBlow(transform.position, move_end, m_enemyBottom))
        {
            if (move_end == m_Enemy.transform.position)
            {
                SendingBodyBlow(m_Enemy);
                m_AudioSource.PlayOneShot(m_AudioClips[1]);
            }
            Ray ray = new Ray(move_end - m_enemyBottom, m_enemyBottom);
            Physics.Raycast(ray, out jump_target, 2f, m_TreeLayer);
            //move_end = jump_target.point;
            m_hitinfo = jump_target;
            m_Animator.SetTrigger("Landing");
            JumpReset();
            m_StateManager.StateProcassor.State = m_StateManager.TreeTp;
        }
    }

    bool BodyBlow(Vector3 start, Vector3 end, Vector3 normal)
    {
        Vector3 dir = (end - start).normalized;
        transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        transform.Translate(dir * m_BodyblowSpeed * Time.deltaTime, Space.World);
        LayerMask playerLayer = LayerMask.GetMask(new string[] { "Player" });

        var enemy = m_Enemy.GetComponent<EnemyAI4>();
        if (Physics.CheckBox(m_center, new Vector3(0.5f, 0.5f, 0.5f), transform.rotation, m_EnemyLayer) && enemy.TreeDist())
        {
            m_Animator.SetTrigger("Tackle");
            return true;
        }
        else if(Physics.CheckSphere(end, 1f, playerLayer))
        {
            return true;
        }

        return false;
    }

}
