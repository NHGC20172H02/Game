using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerritoryGaugeController : MonoBehaviour
{

	public Player m_Player;

	public TerritoryGaugeUI m_TGUIMiniL;
	public TerritoryGaugeUI m_TGUIMiniR;

	public TerritoryGaugeUI m_TGUIMy;
	public TerritoryGaugeUI m_TGUITarget;

	void Update()
	{
		var stay = m_Player.GetOnTree();
		var target = m_Player.GetTargetTree();

		var active = false;
		if (stay != null)
		{
			var connecter = stay.GetComponent<Connecter>();
			if (connecter.m_Type == Connecter.Type.Tree)
			{
				m_TGUIMiniL.SetTree((Tree)connecter);
				m_TGUIMy.SetTree((Tree)connecter);
				active = true;
			}
		}
		m_TGUIMy.gameObject.SetActive(active);


		active = false;
		if (target != null && stay != target)
		{
			var connecter = target.GetComponent<Connecter>();
			if (connecter.m_Type == Connecter.Type.Tree)
			{
				m_TGUIMiniR.SetTree((Tree)connecter);
				m_TGUITarget.SetTree((Tree)connecter);
				active = true;
			}
		}
		m_TGUITarget.gameObject.SetActive(active);

		active = m_TGUIMy.gameObject.activeSelf && m_TGUITarget.gameObject.activeSelf;
		m_TGUIMiniL.gameObject.SetActive(active);
		m_TGUIMiniR.gameObject.SetActive(active);
	}
}
