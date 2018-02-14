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

	public Image m_FlashMask;

	public JumpProgressSpider m_SpiderIcon;

	public Image m_JumpOKNG;
	public Sprite m_OK;
	public Sprite m_NG;

	void Start()
	{
		m_FlashMask.GetComponent<Animation>().Play();	
	}
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
				m_TreeSideL.sprite = m_TreeSide[stay.GetComponent<Tree>().m_SideNumber];
			}
		}
		m_TGUIMy.gameObject.SetActive(active);
		m_TGUIMiniL.gameObject.SetActive(active);
		m_TreeSideL.gameObject.SetActive(active);


		active = false;
		if (target != null && stay != target)
		{
			var connecter = target.GetComponent<Connecter>();
			if (connecter.m_Type == Connecter.Type.Tree)
			{
				m_TGUIMiniR.SetTree((Tree)connecter);
				active = true;
				m_TreeSideR.sprite = m_TreeSide[target.GetComponent<Tree>().m_SideNumber];
				target.GetComponent<Tree>().SetOutLineColor(1 - m_FlashMask.color.a);
				m_SpiderIcon.SetProgress(m_Player.JumpProgress());
			}
		}
		//active = m_TGUIMy.gameObject.activeSelf && active;
		m_TGUIMiniR.gameObject.SetActive(active);
		m_TreeSideR.gameObject.SetActive(active);

		m_JumpOKNG.sprite = m_Player.IsFlyable() ? m_OK : m_NG;
	}
}
