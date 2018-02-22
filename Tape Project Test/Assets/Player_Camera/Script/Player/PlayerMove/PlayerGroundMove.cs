using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player {

    //地面にいる場合の移動
    void GroundMove()
    {
        ResetTrigger();
        m_Animator.SetBool("IsString", false);
        m_Prediction.SetActive(false);
        m_WindLine.Stop();
        //足元の情報取得（地面優先）
        if (!Physics.Raycast(m_center, -transform.up, out m_hitinfo, 1f, m_GroundLayer))
            Physics.Raycast(m_center, -transform.up, out m_hitinfo, 1f, m_TreeLayer);

        if (m_hitinfo.collider.tag == "Tree")
        {
            m_StateManager.StateProcassor.State = m_StateManager.TreeTp;
            return;
        }

        //位置補正
        transform.position = Vector3.Lerp(transform.position, m_hitinfo.point, 0.2f);
        Vector3 move = Vector3.zero;
        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("GroundMove"))
        {
            move = Move(m_Camera.right, m_hitinfo.normal) * m_Speed * Time.deltaTime;
            transform.Translate(move, Space.World);
            transform.rotation = Quaternion.LookRotation(Vector3.Cross(m_Camera.right, m_hitinfo.normal), m_hitinfo.normal);
        }

        if (Physics.Raycast(m_center + Vector3.up * m_GroundJumpHeight, transform.forward, out jump_target, m_GroundJumpForward, m_TreeLayer))
        {
            m_Prediction.SetActive(true);
            m_Prediction.SetParameter(transform.position, jump_target.point, m_Angle, m_Shooter.m_SideNumber);
            m_Prediction.Calculation();
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump"))
            {
                move_start = transform.position;
                move_end = jump_target.point;
                m_Prediction.SetActive(false);
                m_Animator.SetTrigger("NormalJump");
                m_AudioSource.PlayOneShot(m_AudioClips[4]);
                JumpCalculation(move_start, move_end, m_Angle);
                m_StateManager.StateProcassor.State = m_StateManager.GroundJump;
                return;
            }
        }
        else
            m_Prediction.SetActive(false);

        RaycastHit hit;
        //木に登る
        if (Physics.Raycast(m_center, move, out hit, 1f, m_TreeLayer))
        {
            m_hitinfo = hit;
            transform.position = m_hitinfo.point;
            transform.rotation = Quaternion.LookRotation(Vector3.Cross(m_Camera.right, m_hitinfo.normal), m_hitinfo.normal);
            m_StateManager.StateProcassor.State = m_StateManager.TreeTp;
        }
    }

}
