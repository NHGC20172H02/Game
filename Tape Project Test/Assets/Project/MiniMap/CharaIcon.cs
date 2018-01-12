using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaIcon : MonoBehaviour {

	public Transform m_Position;
	public Transform m_Rotation;
	private float m_Y;

	// Use this for initialization
	void Start () {
		m_Y = transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = m_Position.position;
		pos.y = m_Y;
		transform.position = pos;

		pos = m_Rotation.forward;
		pos.y = 0;
		transform.LookAt(transform.position + Vector3.down, pos);
	}
}
