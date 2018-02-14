using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Senryoutyu : SingletonMonoBehaviour<Senryoutyu> {

	Image m_Image;
	bool m_Active;
	// Use this for initialization
	void Start () {
		m_Image = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void LateUpdate () {
		m_Image.enabled = m_Active;
		m_Active = false;
	}

	public void Active(bool active)
	{
		m_Active = active ? true : m_Active;
	}
}
