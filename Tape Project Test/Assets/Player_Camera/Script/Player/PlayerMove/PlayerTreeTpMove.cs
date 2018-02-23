using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player {
    //木の上の俯瞰カメラ状態での移動
    //void TreeTpMove()
    //{
    //    ResetTrigger();
    //    m_Animator.SetBool("IsString", false);
    //    if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Jumpair"))
    //        m_Animator.SetTrigger("Landing");
    //    Vector3 start = transform.position + transform.up * 0.5f;
    //    //足元の情報取得（優先順位 : ネット->木->地面）
    //    if (!Physics.Raycast(start, -transform.up, out m_hitinfo, 1f, m_TreeLayer))
    //        if (!Physics.Raycast(start, -transform.up, out m_hitinfo, 1f, m_NetLayer))
    //            Physics.Raycast(start, -transform.up, out m_hitinfo, 1f, m_GroundLayer);

    //    if (m_hitinfo.collider == null)
    //    {
    //        transform.position = m_prevHit.point;
    //        return;
    //    }
    //    else
    //        m_prevHit = m_hitinfo;

    //    if (IsChangedNumber())
    //        return;

    //    m_treeWaitTime += Time.deltaTime;

    //    //位置補正
    //    transform.position = Vector3.Lerp(transform.position, m_hitinfo.point, 0.8f);
    //    Vector3 move = Vector3.zero;
    //    if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("GroundMove"))
    //        move = Move(m_Camera.right, m_hitinfo.normal);

    //    RaycastHit prevHit = m_hitinfo;
    //    if (Physics.Raycast(start, move, out m_hitinfo, 1f, m_GroundLayer))
    //    {
    //        transform.position = m_hitinfo.point;
    //        transform.rotation = Quaternion.LookRotation(Vector3.Cross(m_Camera.right, m_hitinfo.transform.up), m_hitinfo.transform.up);
    //        m_StateManager.StateProcassor.State = m_StateManager.GroundTp;
    //        return;
    //    }
    //    else
    //        m_hitinfo = prevHit;

    //    //一人称視点変更
    //    if (Input.GetKeyDown(KeyCode.L) || Input.GetButtonDown("Right Stick Click"))
    //        m_StateManager.StateProcassor.State = m_StateManager.TreeFp;

    //    Vector3 dir = Vector3.zero;
    //    var cols = Physics.OverlapSphere(m_center, 3f, m_EnemyLayer);
    //    foreach (Collider c in cols)
    //    {
    //        dir = c.transform.position - transform.position;
    //    }
    //    float angle = Vector3.Angle(transform.forward, dir);
    //    //近接攻撃（テスト）
    //    if (angle < 30f && Input.GetKeyDown(KeyCode.H))
    //    {
    //        m_Animator.SetTrigger("Proximity");
    //        m_StateManager.StateProcassor.State = m_StateManager.ProximityAttack;
    //        return;
    //    }

    //    Vector3 origin = start + (m_Camera.position - start).normalized;
    //    Ray ray = new Ray(origin, m_Camera.forward);
    //    int[] layers = new int[1];
    //    layers[0] = m_StringLayer;
    //    if (Input.GetKeyDown(KeyCode.F) || Input.GetButtonDown("A Button"))
    //    {
    //        IntersectString(layers);
    //    }

    //    if (m_hitinfo.collider != null)
    //        Jump(ray, m_hitinfo);
    //}

    void TreeTpMove()
    {
        ResetTrigger();
        m_Animator.SetBool("IsString", false);
        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Jumpair"))
            m_Animator.SetTrigger("Landing");

        Vector3 move = Vector3.zero;
        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("GroundMove"))
        {
            move = Move(m_Camera.right, m_hitinfo.normal) * m_Speed * Time.deltaTime;
            bool isMove = false;
            //移動先の情報取得（優先順位 : 木->ネット->地面）
            if (!(isMove = Physics.Raycast(m_center + move, -transform.up, out m_hitinfo, 1f, m_TreeLayer)))
                if (!(isMove = Physics.Raycast(m_center + move, -transform.up, out m_hitinfo, 1f, m_NetLayer)))
                    isMove = Physics.Raycast(m_center + move, -transform.up, out m_hitinfo, 1f, m_GroundLayer);

            if (isMove)
            {
                transform.Translate(move, Space.World);
                transform.rotation = Quaternion.LookRotation(Vector3.Cross(m_Camera.right, m_hitinfo.normal), m_hitinfo.normal);
                transform.position = Vector3.Lerp(transform.position, m_hitinfo.point, 0.8f);
            }
        }
        
        if (m_hitinfo.collider == null)
        {
            m_hitinfo = m_prevHit;
            transform.position = m_hitinfo.point;
            transform.rotation = Quaternion.LookRotation(Vector3.Cross(m_Camera.right, m_hitinfo.normal), m_hitinfo.normal);
            return;
        }

        m_prevHit = m_hitinfo;
        //transform.position = Vector3.Lerp(transform.position, m_hitinfo.point, 0.2f);
        //transform.rotation = Quaternion.LookRotation(Vector3.Cross(m_Camera.right, m_hitinfo.normal), m_hitinfo.normal);

        if (IsChangedNumber())
            return;

        m_treeWaitTime += Time.deltaTime;

        RaycastHit moveHit;
        if (Physics.Raycast(m_center, move, out moveHit, 1f, m_GroundLayer))
        {
            m_hitinfo = moveHit;
            transform.position = m_hitinfo.point;
            transform.rotation = Quaternion.LookRotation(Vector3.Cross(m_Camera.right, m_hitinfo.transform.up), m_hitinfo.transform.up);
            m_StateManager.StateProcassor.State = m_StateManager.GroundTp;
            return;
        }

        //一人称視点変更
        if (Input.GetKeyDown(KeyCode.L) || Input.GetButtonDown("Right Stick Click"))
            m_StateManager.StateProcassor.State = m_StateManager.TreeFp;

        //近接攻撃方向
        Vector3 dir = Vector3.zero;
        var cols = Physics.OverlapSphere(m_center, 3f, m_EnemyLayer);
        foreach (Collider c in cols)
        {
            dir = c.transform.position - transform.position;
        }
        float angle = Vector3.Angle(transform.forward, dir);
        //近接攻撃（テスト）
        if (angle < 30f && Input.GetKeyDown(KeyCode.H))
        {
            m_Animator.SetTrigger("Proximity");
            m_StateManager.StateProcassor.State = m_StateManager.ProximityAttack;
            return;
        }

        Vector3 origin = m_center + (m_Camera.position - m_center).normalized;
        Ray ray = new Ray(origin, m_Camera.forward);
        int[] layers = new int[1];
        layers[0] = m_StringLayer;
        if (Input.GetKeyDown(KeyCode.F) || Input.GetButtonDown("A Button"))
        {
            IntersectString(layers);
        }

        if (m_hitinfo.collider != null)
            Jump(ray, m_hitinfo);
    }


}
