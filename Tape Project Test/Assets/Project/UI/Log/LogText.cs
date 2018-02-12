using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogText : MonoBehaviour {
	public Text m_Text;
	public Text m_TimeStamp;
	public float m_Time;
	public Image m_Image;
	public Animator m_Animator;
	private float m_Timer;
	// Use this for initialization
	void Start () {
		m_Timer = 0;
	}
	
	// Update is called once per frame
	void Update () {
		float t = m_Timer / m_Time;
		float a = 1.0f - t * t * t;
		var c = m_Text.color;
		c.a = a;
		m_Text.color = c;
		c = m_Image.color;
		c.a = a;
		m_Image.color = c;
		if (t>=1) Destroy(gameObject);

		m_Timer += Time.deltaTime;
	}
}
