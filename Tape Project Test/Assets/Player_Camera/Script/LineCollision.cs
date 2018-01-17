using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//予測線のCollision
public class LineCollision : MonoBehaviour {

    private PredictionLine m_Parent;

	void Start () {
        GameObject parent = gameObject.transform.parent.gameObject;
        m_Parent = parent.GetComponent<PredictionLine>();
	}
	
    void OnCollisionStay(Collision collision)
    {
        if (m_Parent == null) return;
        m_Parent.ReceiveChildCollision(collision);
    }

    void OnCollisionExit(Collision collision)
    {
        m_Parent.m_HitString = null;
        m_Parent.m_HitNet = null;
        //m_Parent.m_HitStringPoint = Vector3.zero;
    }

    void Update () {
		
	}
}
