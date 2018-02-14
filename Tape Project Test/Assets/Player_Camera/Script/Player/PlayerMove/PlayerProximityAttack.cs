using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player {

	void ProximityAttack()
    {
        Collider[] cols = Physics.OverlapBox(m_center, new Vector3(1f, 0.6f, 1f), transform.rotation, m_EnemyLayer);
        foreach(Collider col in cols)
        {
            SendingBodyBlow(col.gameObject);
            Debug.Log("hit");
        }

        Ray ray = new Ray(m_center, -transform.up);
        Physics.Raycast(ray, out m_hitinfo, 1f, m_TreeLayer);
        if(m_hitinfo.collider == null)
            AttackCancel();

        Vector3 forward = Vector3.Cross(m_Camera.right, m_hitinfo.normal);
        m_attackRate += Time.deltaTime;
        float speed = m_AttackSpeed.Evaluate(m_attackRate);
        transform.rotation = Quaternion.LookRotation(forward, m_hitinfo.normal);
        transform.Translate(forward * speed * Time.deltaTime, Space.World);

        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("GroundMove"))
            AttackCancel();
    }

    private void AttackCancel()
    {
        m_attackRate = 0;
        m_StateManager.StateProcassor.State = m_StateManager.TreeTp;
    }
}
