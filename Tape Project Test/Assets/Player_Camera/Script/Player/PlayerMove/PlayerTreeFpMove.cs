using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player {

    //木の上の一人称カメラ状態での操作
    void TreeFpMove()
    {
        if (IsChangedNumber())
            return;
        m_treeWaitTime += Time.deltaTime;
        Vector3 start = transform.position + transform.up * 0.5f;
        if (Input.GetKeyDown(KeyCode.L) || Input.GetButtonDown("LB"))
        {
            if (m_hitinfo.collider.tag == "Tree" || m_hitinfo.collider.tag == "Net")
                m_StateManager.StateProcassor.State = m_StateManager.TreeTp;
            else if (m_hitinfo.collider.tag == "String")
                m_StateManager.StateProcassor.State = m_StateManager.StringTp;
        }

        Vector3 origin = start + (m_Camera.position - start).normalized;
        Ray ray = new Ray(origin, m_Camera.forward);
        if (m_hitinfo.collider != null)
            Jump(ray, m_hitinfo);
    }

}
