using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapScale : MonoBehaviour {

	private RectTransform m_RectT;
	private Vector2 m_BaseSize;
	private int m_Index;

	public List<float> m_Scales;


	// Use this for initialization
	void Start () {
		m_RectT = GetComponent<RectTransform>();
		m_BaseSize = m_RectT.sizeDelta;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Left"))
		{
			m_Index += m_Scales.Count - 1;
			m_Index %= m_Scales.Count;
			m_RectT.sizeDelta = m_BaseSize * m_Scales[m_Index];
		}
		if (Input.GetButtonDown("Right"))
		{
			m_Index += m_Scales.Count + 1;
			m_Index %= m_Scales.Count;
			m_RectT.sizeDelta = m_BaseSize * m_Scales[m_Index];
		}
	}
}
