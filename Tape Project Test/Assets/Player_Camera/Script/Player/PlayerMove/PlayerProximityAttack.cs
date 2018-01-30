using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player {

	void ProximityAttack()
    {
        Collider[] cols = Physics.OverlapBox(m_center, new Vector3(0.5f, 0.3f, 0.5f), Quaternion.identity, m_EnemyLayer);
        foreach(Collider col in cols)
        {
            //SendingBodyBlow(col.gameObject);
            Debug.Log("hit");
        }

        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("GroundMove"))
            m_StateManager.StateProcassor.State = m_StateManager.TreeTp;
    }
}
