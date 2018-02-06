using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StringMeter : MonoBehaviour {

	public Text m_Text;
	public StringShooter m_StringShooter;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		m_Text.text = (m_StringShooter.m_MaxCost - m_StringShooter.m_Cost).ToString("0000.0");
	}
}
