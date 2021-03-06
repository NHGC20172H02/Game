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

	public Transform m_StringPParent;
	public Transform m_StringEParent;

	public GameObject m_StringP;
	public GameObject m_StringE;

	public List<GameObject> m_stringPs;
	public List<GameObject> m_stringEs;

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
				var cc = ((Tree)connecter).m_ConnectCounts;
				if (cc[0, 0]+cc[0,1] != m_stringPs.Count)
				{
					foreach (var item in m_stringPs)
					{
						Destroy(item);
					}
					m_stringPs.Clear();
					for (int i = 0; i < cc[0, 0] + cc[0,1]; i++)
					{
						if (i == 13) break;
						m_stringPs.Add(Instantiate(m_StringP, m_StringPParent));
					}
				}
				if (cc[1, 0] + cc[1,1] != m_stringEs.Count)
				{
					foreach (var item in m_stringEs)
					{
						Destroy(item);
					}
					m_stringEs.Clear();
					for (int i = 0; i < cc[1, 0] + cc[1,1]; i++)
					{
						if (i == 13) break;
						m_stringEs.Add(Instantiate(m_StringE, m_StringEParent));
					}
				}
			}
		}
		m_TGUIMy.gameObject.SetActive(active);
		m_TGUIMiniL.gameObject.SetActive(active);
		m_TreeSideL.gameObject.SetActive(active);


		active = false;
		if (target != null && stay != target)
		{
			var connecter = target.GetComponent<Connecter>();
			switch (connecter.m_Type)
			{
				case Connecter.Type.none:
					break;
				case Connecter.Type.Tree:
					m_TGUIMiniR.SetTree((Tree)connecter);
					active = true;
					m_TreeSideR.sprite = m_TreeSide[target.GetComponent<Tree>().m_SideNumber];
					target.GetComponent<Tree>().SetOutLineColor(1 - m_FlashMask.color.a);
					m_SpiderIcon.SetProgress(m_Player.JumpProgress());
					break;
				case Connecter.Type.String:
					if(target.GetComponent<StringUnit>().m_SideNumber==1)
					target.GetComponent<StringUnit>().SetColor(1 - m_FlashMask.color.a);
					break;
				case Connecter.Type.Net:
					if (target.GetComponent<Net>().m_SideNumber == 1)
						target.GetComponent<Net>().SetColor(1 - m_FlashMask.color.a);
					break;
				default:
					break;
			}
		}
		//active = m_TGUIMy.gameObject.activeSelf && active;
		m_TGUIMiniR.gameObject.SetActive(active);
		m_TreeSideR.GetComponent<Image>().enabled = active;
		foreach (var item in m_TreeSideR.GetComponentsInChildren<Image>())
		{
			item.enabled = active;
		}

		m_JumpOKNG.sprite = m_Player.IsFlyable() ? m_OK : m_NG;
	}
}
