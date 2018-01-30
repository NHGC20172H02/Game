using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player {

    //地面上でのジャンプ
    void GroundJump()
    {
        if (!Depression()) return;
        if (Projection(move_start, move_end, jump_target.normal, m_Angle))
        {
            waitFrame = 0;
            m_Animator.ResetTrigger("Landing");
            m_StateManager.StateProcassor.State = m_StateManager.TreeTp;
            return;
        }
        float dif = Mathf.Abs(flightDuration - elapse_time / flightDuration);
        if (dif < 0.3f)
        {
            m_Animator.SetTrigger("Landing");
        }
    }
}
