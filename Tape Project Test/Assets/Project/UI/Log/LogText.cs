using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogText : MonoBehaviour {
	public Text m_Text;
	public float m_Time;
	private float m_Timer;
	private Color m_Color;
	// Use this for initialization
	void Start () {
		m_Timer = 0;
		m_Color = m_Text.color;
	}
	
	// Update is called once per frame
	void Update () {
		float t = m_Timer / m_Time;
		m_Color.a = 1.0f - t * t * t;
		m_Text.color = m_Color;

		if (t>=1) Destroy(gameObject);

		m_Timer += Time.deltaTime;
	}
}
