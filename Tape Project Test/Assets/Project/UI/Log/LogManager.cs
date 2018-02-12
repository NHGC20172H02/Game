using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogManager : SingletonMonoBehaviour<LogManager> {

	public Transform m_LogParent;
	public GameObject m_Prefab;
	public BattleScene m_BattleScene;

	void Start () {
		
	}
	
	void Update () {
		
	}

	public void Create(string message,Color color,bool flash)
	{
		if (m_LogParent == null) return;
		if (m_Prefab == null) return;
		GameObject l = Instantiate(m_Prefab, m_LogParent);
		LogText log = l.GetComponent<LogText>();
		log.m_TimeStamp.text = m_BattleScene.m_TimerUI.text;
		log.m_Text.text = message;
		log.m_Image.color = color;
		log.m_Animator.SetBool("Flash",flash);
	}
}
