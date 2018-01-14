using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : Connecter {
	public Renderer m_MapRenderer;
	public Material[] m_MapMaterials;
	public Transform m_Log;
	public GameObject m_LogText;
	private void Start()
	{
		TerritoryManager.Instance.m_Trees.Add(this);
	}
	public override void ChildUpdate()
	{
		List<int> count = new List<int> { 0, 0, 0 };
		foreach (var item in m_Child)
		{
			count[item.m_SideNumber]++;
		}
		int sideNumber = m_SideNumber;
		for (int i = 0; i < count.Count; i++)
		{
			if (count[sideNumber] < count[i])
			{
				sideNumber = i;
			}
		}
		if (m_SideNumber == sideNumber) return;

		SetSide(sideNumber);
		if(m_MapRenderer != null)
		{
			m_MapRenderer.material = m_MapMaterials[sideNumber];
		}
		if(m_Log != null)
		{
			Instantiate(m_LogText, m_Log).GetComponent<LogText>().m_Text.text = sideNumber == 1?"木を奪った！":"木が奪われた！";
		}
	}

}
