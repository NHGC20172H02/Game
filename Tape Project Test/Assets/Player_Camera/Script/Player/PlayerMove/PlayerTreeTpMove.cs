using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player {
    //木の上の俯瞰カメラ状態での移動
    void TreeTpMove()
    {
        ResetTrigger();
        m_Animator.SetBool("IsString", false);
        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Jumpair"))
            m_Animator.SetTrigger("Landing");
        Vector3 start = transform.position + transform.up * 0.5f;
        //足元の情報取得（優先順位 : ネット->木->地面）
        if (!Physics.Raycast(start, -transform.up, out m_hitinfo, 1f, m_TreeLayer))
            if (!Physics.Raycast(start, -transform.up, out m_hitinfo, 1f, m_NetLayer))
                Physics.Raycast(start, -transform.up, out m_hitinfo, 1f, m_GroundLayer);

        if (m_hitinfo.collider == null)
        {
            transform.position = m_prevPos;
            return;
        }

        if (IsChangedNumber())
            return;

        m_treeWaitTime += Time.deltaTime;

        m_prevPos = transform.position;
        //位置補正
        transform.position = Vector3.Lerp(transform.position, m_hitinfo.point, 0.8f);
        Vector3 move = Vector3.zero;
        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("GroundMove"))
            move = Move(m_Camera.right, m_hitinfo.normal);

        RaycastHit prevHit = m_hitinfo;
        if (Physics.Raycast(start, move, out m_hitinfo, 1f, m_GroundLayer))
        {
            transform.position = m_hitinfo.point;
            transform.rotation = Quaternion.LookRotation(Vector3.Cross(m_Camera.right, m_hitinfo.transform.up), m_hitinfo.transform.up);
            m_StateManager.StateProcassor.State = m_StateManager.GroundTp;
            return;
        }
        else
            m_hitinfo = prevHit;

        //一人称視点変更
        if (Input.GetKeyDown(KeyCode.L) || Input.GetButtonDown("LB"))
            m_StateManager.StateProcassor.State = m_StateManager.TreeFp;

        //近接攻撃（テスト）
        if (Input.GetKeyDown(KeyCode.H))
        {
            m_Animator.SetTrigger("Proximity");
            m_StateManager.StateProcassor.State = m_StateManager.ProximityAttack;
            return;
        }

        Vector3 origin = start + (m_Camera.position - start).normalized;
        Ray ray = new Ray(origin, m_Camera.forward);
        int[] layers = new int[1];
        layers[0] = m_StringLayer;
        if (Input.GetKeyDown(KeyCode.F) || Input.GetButtonDown("Fire1"))
        {
            IntersectString(layers);
        }

        if (m_hitinfo.collider != null)
            Jump(ray, m_hitinfo);
    }

}
