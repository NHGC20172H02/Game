using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player {

    //糸の上での移動
    void StringTpMove()
    {
        if(m_hitinfo.collider == null)
        {
            Fall();
            return;
        }
        if (m_hitinfo.collider.tag == "Net")
            m_StateManager.StateProcassor.State = m_StateManager.TreeTp;
        if (IsChangedNumber())
            return;
        m_Animator.SetBool("IsString", true);
        if (m_hitinfo.collider.GetComponent<StringUnit>().m_SideNumber != m_Shooter.m_SideNumber)
            m_StateManager.StateProcassor.State = m_StateManager.Falling;
        var line = m_hitinfo.collider.GetComponent<LineRenderer>();
        //糸のベクトル
        Vector3 stringVec = (line.GetPosition(1) - line.GetPosition(0)).normalized;
        //上方向ベクトル
        Vector3 up = Vector3.Cross(m_Camera.right, stringVec);
        //糸のベクトルを90度回転したベクトル
        Vector3 stringVertical = Quaternion.Euler(0, 90f, 0) * stringVec;
        float angle = Vector3.Angle(m_Camera.right, stringVec);
        Vector3 vec = Vector3.zero;
        Vector3 move = Vector3.zero;
        //４方向に向きを変えていく
        if (angle > 45f && angle < 135f)
        {
            if (up.y > 0)
                stringVec = -stringVec;
            vec = stringVec;
            move = stringVec * Input.GetAxis("Vertical");
            m_Animator.SetFloat("StringMoveX", 0);
            m_Animator.SetFloat("StringMoveZ", Input.GetAxis("Vertical"));
        }
        else if (angle < 45f)
        {
            vec = -stringVertical;
            move = stringVec * Input.GetAxis("Horizontal");
            m_Animator.SetFloat("StringMoveZ", 0);
            m_Animator.SetFloat("StringMoveX", Input.GetAxis("Horizontal"));
        }
        else if (angle > 135f)
        {
            vec = stringVertical;
            move = -stringVec * Input.GetAxis("Horizontal");
            m_Animator.SetFloat("StringMoveZ", 0);
            m_Animator.SetFloat("StringMoveX", Input.GetAxis("Horizontal"));
        }
        transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.forward, vec, 0.2f));
        transform.Translate(move * m_Speed * 1.3f * Time.deltaTime, Space.World);
        transform.position = MoveRange(transform.position, line.GetPosition(0), line.GetPosition(1));

        //一人称視点変更
        if (Input.GetKeyDown(KeyCode.L) || Input.GetButtonDown("LB"))
        {
            m_StateManager.StateProcassor.State = m_StateManager.TreeFp;
        }

        Vector3 start = transform.position + transform.up * 0.5f;
        Vector3 origin = start + (m_Camera.position - transform.position).normalized;
        Ray ray = new Ray(origin, m_Camera.forward);
        int[] layers = new int[2];
        layers[0] = m_StringLayer;
        layers[1] = m_NetLayer;
        if (Input.GetKeyDown(KeyCode.F) || Input.GetButtonDown("Fire1"))
        {
            IntersectString(layers);
        }
        if (m_hitinfo.collider != null)
            Jump(ray, m_hitinfo);
    }

}
