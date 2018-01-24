using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player {

    //地面にいる場合の移動
    void GroundMove()
    {
        ResetTrigger();
        m_Animator.SetBool("IsString", false);
        m_Prediction.gameObject.SetActive(false);
        m_WindLine.Stop();
        Vector3 start = transform.position + transform.up * 0.5f;
        //足元の情報取得（地面優先）
        if (!Physics.Raycast(start, -transform.up, out m_hitinfo, 1f, m_GroundLayer))
            Physics.Raycast(start, -transform.up, out m_hitinfo, 1f, m_TreeLayer);

        if (m_hitinfo.collider.tag == "Tree")
        {
            m_StateManager.StateProcassor.State = m_StateManager.TreeTp;
            return;
        }

        //位置補正
        transform.position = Vector3.Lerp(transform.position, m_hitinfo.point, 0.2f);
        Vector3 move = Vector3.zero;
        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("GroundMove"))
            move = Move(m_Camera.right, m_hitinfo.normal);

        RaycastHit hit;
        if (Physics.Raycast(start + Vector3.up * m_GroundJumpHeight, transform.forward, out hit, m_GroundJumpForward, m_TreeLayer))
        {
            m_Prediction.gameObject.SetActive(true);
            m_Prediction.SetParameter(transform.position, hit.point, m_Angle, m_Shooter.m_SideNumber);
            m_Prediction.Calculation();
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump"))
            {
                m_hitinfo = hit;
                move_start = transform.position;
                move_end = hit.point;
                m_Prediction.gameObject.SetActive(false);
                m_Animator.SetTrigger("NormalJump");
                JumpCalculation(move_start, move_end, m_Angle);
                m_StateManager.StateProcassor.State = m_StateManager.GroundJump;
                return;
            }
        }
        else
            m_Prediction.gameObject.SetActive(false);

        //木に登る
        if (Physics.Raycast(start, move, out m_hitinfo, 1f, m_TreeLayer))
        {
            transform.position = m_hitinfo.point;
            transform.rotation = Quaternion.LookRotation(Vector3.Cross(transform.right, m_hitinfo.normal), m_hitinfo.normal);
            m_StateManager.StateProcassor.State = m_StateManager.TreeTp;
        }
    }

}
