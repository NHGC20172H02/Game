using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerritoryGaugeController : MonoBehaviour
{

	public Player m_Player;

	public TerritoryGaugeUI m_TGUIMiniL;
	public TerritoryGaugeUI m_TGUIMiniR;

	public TerritoryGaugeUI m_TGUIMy;

	public Image m_TreeSideL;
	public Image m_TreeSideR;

	public Sprite[] m_TreeSide;

	void LateUpdate()
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
				active = true;
			}
		}
		active = m_TGUIMy.gameObject.activeSelf && active;
		m_TGUIMiniL.gameObject.SetActive(active);
		m_TGUIMiniR.gameObject.SetActive(active);
		if (active)
		{
			m_TreeSideL.sprite = m_TreeSide[stay.GetComponent<Tree>().m_SideNumber];
			m_TreeSideR.sprite = m_TreeSide[target.GetComponent<Tree>().m_SideNumber];
		}
		m_TreeSideL.enabled = active;
		m_TreeSideR.enabled = active;
	}
}
