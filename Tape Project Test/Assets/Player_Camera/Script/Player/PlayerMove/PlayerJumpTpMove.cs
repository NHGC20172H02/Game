using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player {

    //ジャンプ中の動作
    void JumpTpMove()
    {
        if (!Depression()) return;
        if (Projection(move_start, move_end, jump_target.normal, m_Angle))
        {
            JumpReset();
            m_Animator.ResetTrigger("Landing");
            m_AudioSource.PlayOneShot(m_AudioClips[2]);
            if (jump_target.collider.tag == "String")
            {
                m_hitinfo = jump_target;
                m_StateManager.StateProcassor.State = m_StateManager.StringTp;
                return;
            }
            m_StateManager.StateProcassor.State = m_StateManager.TreeTp;
            return;
        }
        float dif = Mathf.Abs(flightDuration - elapse_time / m_JumpSpeed);
        if (dif < 0.3f && !isLanding)
        {
            m_Animator.SetTrigger("Landing");
            isLanding = true;
            if (jump_target.collider.tag == "String")
                m_Animator.SetBool("IsString", true);
            else
                m_Animator.SetBool("IsString", false);
        }

        int playerLayer = LayerMask.GetMask(new string[] { "Player" });
        if (m_Prediction.m_HitStringPoint != Vector3.zero)
        {
            m_EscapeSphere.SetActive(true);
            m_EscapeSphere.transform.position = m_Prediction.m_HitStringPoint;
            if (Physics.CheckSphere(m_Prediction.m_HitStringPoint, 5f, playerLayer))
            {
                //回避行動
                if ((Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump")) && m_escapeInterval <= 0)
                {
                    if (m_Prediction.m_IsString)
                        m_Prediction.m_HitString.SideUpdate(m_Shooter.m_SideNumber);
                    else
                        m_Prediction.m_HitNet.SideUpdate(m_Shooter.m_SideNumber);
                    m_Shooter.StringShoot(move_start, m_Prediction.m_HitStringPoint);
                    m_Animator.SetTrigger("Escape");
                    m_AudioSource.PlayOneShot(m_AudioClips[0]);
                    m_EscapeSphere.SetActive(false);
                    isEscape = true;
                }
                else if (Physics.CheckSphere(m_Prediction.m_HitStringPoint, 0.3f, playerLayer) && !isEscape)
                {
                    waitFrame = 0;
                    m_Prediction.m_HitStringPoint = Vector3.zero;
                    m_EscapeSphere.SetActive(false);
                    m_WindLine.Stop();
                    Fall();
                    return;
                }
            }
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump"))
            {
                m_escapeInterval = EscapeTime;
            }
            m_escapeInterval -= Time.deltaTime;
        }
    }

}
