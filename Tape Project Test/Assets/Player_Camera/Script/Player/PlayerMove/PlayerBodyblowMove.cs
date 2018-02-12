using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player {

    //体当たり状態
    void BodyBlowMove()
    {
        //if (!Depression()) return;
        if (Projection(move_start, move_end, jump_target.normal, m_Angle))
        {
            JumpReset();
            m_StateManager.StateProcassor.State = m_StateManager.TreeTp;
            return;
        }
        LayerMask playerLayer = LayerMask.GetMask(new string[] { "Player" });
        if (Physics.CheckSphere(move_end, 2f, playerLayer) && !isLanding)
        {
            if (Physics.CheckSphere(move_end, 2f, m_EnemyLayer))
            {
                m_Animator.SetTrigger("Tackle");
                m_AudioSource.PlayOneShot(m_AudioClips[1]);
                SendingBodyBlow(m_enemy.gameObject);
                isLanding = true;
            }
            else
            {
                m_Animator.SetTrigger("Failure");
                m_Animator.SetTrigger("Landing");
            }
        }
    }

}
