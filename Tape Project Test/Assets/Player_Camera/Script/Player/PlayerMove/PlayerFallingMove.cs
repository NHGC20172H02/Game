using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player {

    //落下状態での動作
    void FallingMove()
    {
        if (m_failureTime < 0.5f)
        {
            m_failureTime += Time.deltaTime;
            return;
        }
        Ray ray = new Ray(transform.position + Vector3.up, Vector3.down);
        if (!Physics.Raycast(ray, out m_hitinfo, 100f, m_TreeLayer))
            Physics.Raycast(ray, out m_hitinfo, 100f, m_GroundLayer);
        //落下スピード
        gravity.y += Physics.gravity.y * Time.deltaTime;
        transform.Translate(gravity * Time.deltaTime, Space.World);
        Vector3 forward = Vector3.Cross(m_Camera.right, m_hitinfo.normal);
        transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.forward, forward, 0.4f), m_hitinfo.normal);
        if (transform.position.y < 0.3f)
        {
            LandingReset(m_hitinfo.collider);
        }
    }

}
