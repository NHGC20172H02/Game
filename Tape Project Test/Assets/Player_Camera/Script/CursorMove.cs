using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorMove : MonoBehaviour {

    public RectTransform m_Icon1;
    public RectTransform m_Icon2;
    public Transform m_Player;
    public Transform m_Enemy;

    private float m_limit = 0;

	void Start () {
        m_limit = m_Player.GetComponent<Player>().m_JumpLimit;
	}
	
	void Update () {
        float dis = Vector3.Distance(m_Player.position, m_Enemy.position);
        float nRot = (dis - m_limit) / m_limit;
        nRot = Mathf.Clamp(nRot, 0, 1f);

        m_Icon1.rotation = Quaternion.Euler(0, 0, nRot * 180f);
        m_Icon2.rotation = Quaternion.Euler(0, 0, -nRot * 180f);
        
    }
}
