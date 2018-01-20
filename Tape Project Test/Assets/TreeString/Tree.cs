using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tree : Connecter {
	public string m_Zahyou;
	public Image m_Gauge;

	public float m_TerritoryRate;
	public float m_TerritoryRatePre;

	public Renderer m_Renderer;

	private int[,] m_ChildCounts = new int[2,2];//[side, string=0 net=1]
	private bool[] m_IsHitChara = new bool[] { false, false };

	private void Start()
	{
		TerritoryManager.Instance.m_Trees.Add(this);
		m_Zahyou = new string[]{"1","2" }[1];
	}

private void Update()
	{
		m_TerritoryRate += m_IsHitChara[0] ? Time.deltaTime * 20 : 0;
		m_TerritoryRate -= m_IsHitChara[1] ? Time.deltaTime * 20 : 0;
		if(m_IsHitChara[0]==false && m_IsHitChara[1] == false && Mathf.Abs(m_TerritoryRate) < 50 && m_TerritoryRate!=0)
		{
			m_TerritoryRate = Mathf.Max(0,Mathf.Abs(m_TerritoryRate) - Time.deltaTime * 5) * Mathf.Sign(m_TerritoryRate);
		}
		m_IsHitChara = new bool[] { false, false };
		int sideNumber = m_SideNumber;
		if (m_SideNumber == 0 && m_TerritoryRatePre < 50 && m_TerritoryRate >= 50 ||
			m_SideNumber != 0 && m_TerritoryRatePre < 0 && m_TerritoryRate >= 0)
		{
			m_TerritoryRate = Mathf.Max(m_TerritoryRate, 50);
			sideNumber = 1;
		}
		if (m_SideNumber == 0 && m_TerritoryRatePre > -50 && m_TerritoryRate <= -50 ||
			m_SideNumber != 0 && m_TerritoryRatePre > 0 && m_TerritoryRate <= 0)
		{
			m_TerritoryRate = Mathf.Min(m_TerritoryRate, -50);
			sideNumber = 2;
		}
		m_TerritoryRate = Mathf.Max(Mathf.Min(m_TerritoryRate, 100), -100);
		m_TerritoryRatePre = m_TerritoryRate;

		m_Gauge.color = m_TerritoryRate >= 0 ? Color.blue : Color.red;
		m_Gauge.fillClockwise = m_TerritoryRate >= 0;
		m_Gauge.fillAmount = Mathf.Abs(m_TerritoryRate / 100f);

		if (m_SideNumber == sideNumber) return;

		//SetSide(sideNumber);
		m_Renderer.material = m_Materials[sideNumber];

		if(m_SideNumber == 0)
		{
			if (sideNumber == 1)
			{
				LogManager.Instance.Create(m_Zahyou + "の木がプレイヤーの陣地になった", Color.blue, false);
			}
			else
			{
				LogManager.Instance.Create(m_Zahyou + "の木が敵の陣地になった", Color.yellow, false);
			}
		}
		else
		{
			if (sideNumber == 1)
			{
				LogManager.Instance.Create(m_Zahyou + "の木がプレイヤーの陣地になった", Color.blue, false);
			}
			else
			{
				LogManager.Instance.Create(m_Zahyou + "の木が敵に奪われた", Color.red, true);
			}
		}
		m_SideNumber = sideNumber;

	}
	public override void ChildUpdate()
	{
		m_ChildCounts = new int[,] { { 0, 0 }, { 0, 0 } };
		foreach (var item in m_Child)
		{
			var su = (StringUnit)item;
			if(su.m_StartConnecter == this && su.m_EndConnecter != this && su.m_EndConnecter is Tree ||
				su.m_EndConnecter == this && su.m_StartConnecter != this && su.m_StartConnecter is Tree)
			{
				m_ChildCounts[item.m_SideNumber - 1, 0]++;
			}
		}
	}

	private void OnTriggerStay(Collider other)
	{
		StringShooter ss = other.GetComponent<StringShooter>();
		if(ss != null)
		{
			m_IsHitChara[ss.m_SideNumber - 1] = true;
		}
	}

}
