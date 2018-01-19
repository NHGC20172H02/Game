using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogManager : SingletonMonoBehaviour<LogManager> {

	public Transform m_LogParent;
	public GameObject m_Prefab;

	void Start () {
		
	}
	
	void Update () {
		
	}

	public GameObject Create(string message)
	{
		if (m_LogParent == null) return null;
		if (m_Prefab == null) return null;
		GameObject log = Instantiate(m_Prefab, m_LogParent);
		log.GetComponent<Text>().text = message;
		return log;
	}
}
