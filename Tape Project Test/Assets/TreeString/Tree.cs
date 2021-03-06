using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tree : Connecter
{

	public string m_Zahyou;
	public Image m_Gauge;
	public Sprite m_GaugeRed;
	public Sprite m_GaugeBlue;

	public float m_TerritoryRateMAX;
	public float m_TerritoryRate;
	public float m_TerritoryRatePre;

	public Renderer m_Renderer;

	public List<Connecter> m_Connecting = new List<Connecter>();

	private int[,] m_ActivCounts = new int[2, 3];//[side, string=0 net=1]
	public int[,] m_ConnectCounts = new int[2, 2];//[side, string=0 net=1]
	public bool[] m_IsHitChara = new bool[] { false, false };
	public bool[] m_IsMoveChara = new bool[] { false, false };

	private static Vector2 PosMax = new Vector2(100, 100);
	private static Vector2 PosMin = new Vector2(-100, -100);
	private static string[] Xstring = new string[] { "A", "B", "C", "D", "E", "F", "G", "H" };
	private static string[] Ystring = new string[] { "8", "7", "6", "5", "4", "3", "2", "1" };

	float m_AddRateSpider = 10;//2.5f; // 0.4s 1%
	float m_AddRateString = 1.25f; // 0.8s 1%
	float m_DownRateNeutral = 0;//1; // 1.0s 1%
	float m_DownRate = 1/1.5f; // 1.5s 1%
	float m_DownNPC = 2;

	bool m_IsHitNPC;

	public GameObject m_UIEffect;

	private void Start()
	{
		TerritoryManager.Instance.m_Trees.Add(this);
		m_Type = Type.Tree;

		//座標計算
		Vector2 pos = new Vector2(transform.position.x - PosMin.x, transform.position.z - PosMin.y);
		Vector2 div = new Vector2((PosMax.x - PosMin.x) / Xstring.Length, (PosMax.y - PosMin.y) / Ystring.Length);
		m_Zahyou = Xstring[(int)(pos.x / div.x)] +":"+ Ystring[(int)(pos.y / div.y)];
	}

	private void Update()
	{
		Senryoutyu.Instance.Active(m_IsHitChara[0] && m_TerritoryRate < 100); 
		TerritoryUpdate();

		m_IsHitChara = new bool[] { false, false };

		int sideNumber = m_SideNumber;
		if (m_SideNumber == 0 && m_TerritoryRatePre < 100 && m_TerritoryRate >= 100)
		{
			sideNumber = 1;
		}
		if (m_SideNumber == 0 && m_TerritoryRatePre > -100 && m_TerritoryRate <= -100)
		{
			sideNumber = 2;
		}
		if(m_SideNumber == 1 && m_TerritoryRate < 0)
		{
			sideNumber = 0;
		}
		if (m_SideNumber == 2 && m_TerritoryRate > 0)
		{
			sideNumber = 0;
		}
		m_TerritoryRate = Mathf.Min(m_TerritoryRate, 100);
		m_TerritoryRate = Mathf.Max(m_TerritoryRate, -100);
		m_TerritoryRatePre = m_TerritoryRate;

		m_Gauge.color = m_TerritoryRate >= 0 ? Color.blue : Color.red;
		m_Gauge.fillAmount = Mathf.Abs(m_TerritoryRate / 100f);

		if (m_SideNumber != sideNumber)ChangeSide(sideNumber);
		SetOutLineColor(1);
	}
	private void TerritoryUpdate()
	{
		int i = Mathf.Abs(m_ConnectCounts[0, 0] + m_ConnectCounts[0, 1] - m_ConnectCounts[1, 0] - m_ConnectCounts[1, 1]);
		TouchedSpider(0);
		TouchedSpider(1);
		TouchedString(0);
		TouchedString(1);
		if (m_IsHitNPC)
		{
			m_TerritoryRate = Mathf.Max(0, Mathf.Abs(m_TerritoryRate) - m_DownNPC * Time.deltaTime) * Mathf.Sign(m_TerritoryRate);
			m_IsHitNPC = false;
		}
		if (m_SideNumber == 0)
		{
			if (m_IsHitChara[0] == false && m_IsHitChara[1] == false && m_TerritoryRate != 0)
			{
				m_TerritoryRate = Mathf.Max(0, Mathf.Abs(m_TerritoryRate) - m_DownRateNeutral * Time.deltaTime) * Mathf.Sign(m_TerritoryRate);
			}
		}
		else if (m_SideNumber == 1)
		{
			if(m_ConnectCounts[0, 0] == 0)
			{
				m_TerritoryRate = Mathf.Max(0, m_TerritoryRate - m_DownRate * Time.deltaTime);
			}
		}
		else
		{
			if (m_ConnectCounts[1, 0] == 0)
			{
				m_TerritoryRate = Mathf.Min(0, m_TerritoryRate + m_DownRate * Time.deltaTime);
			}
		}

	}
	private void TouchedSpider(int i)
	{
		if (m_IsHitChara[i])
			m_TerritoryRate += m_AddRateSpider * Time.deltaTime * (i == 0 ? 1 : -1.3f);
	}
	private void TouchedString(int i)
	{
		if (m_ConnectCounts[i, 0] + m_ConnectCounts[i, 1] > m_ConnectCounts[Mathf.Abs(i-1), 0] + m_ConnectCounts[Mathf.Abs(i - 1), 1])
			m_TerritoryRate += m_AddRateString * Time.deltaTime * (i == 0 ? 1 : -1);
	}
	public void Jumped(float meter)
	{
		m_TerritoryRate -= meter;
		m_TerritoryRateMAX = Mathf.Max(0, m_TerritoryRateMAX - meter);

	}

	private void ChangeSide(int sideNumber)
	{
		m_Renderer.material = m_Materials[sideNumber];

		var e = Instantiate(m_UIEffect, transform);
		e.transform.position = transform.position;
		Destroy(e, 2);
		switch (sideNumber)
		{
			case 0:
				LogManager.Instance.Create(m_Zahyou + "の木が中立になった", Color.black, false);
				break;
			case 1:
				LogManager.Instance.Create(m_Zahyou + "の木を占領した", Color.blue, false);
				break;
			case 2:
				LogManager.Instance.Create(m_Zahyou + "の木が敵に奪われた", Color.red, false);
				break;
			default:
				break;
		}

		m_SideNumber = sideNumber;

		var trees = new List<Tree>();
		foreach (var connecter in m_Connecting)
		{
			foreach (var tree in connecter.m_ConnectingTree)
			{
				if (tree == this || trees.Contains(tree)) continue;
				trees.Add(tree);
			}
		}
		foreach (var tree in trees)
		{
			tree.ConnectionUpdate();
		}

	}

	public override void ConnectionUpdate()
	{
		m_ConnectingTree.Clear();
		m_ActivCounts = new int[,] { { 0, 0, 0 }, { 0, 0, 0 } };
		m_ConnectCounts = new int[,] { { 0, 0 }, { 0, 0 } };
		foreach (var connecter in m_Connecting)
		{
			foreach (var tree in connecter.m_ConnectingTree)
			{
				if (tree == this) continue;
				if (connecter.m_SideNumber == tree.m_SideNumber)
				{
					AddTree(tree);
					m_ActivCounts[connecter.m_SideNumber - 1, connecter.m_Type == Type.String ? 0 : 1]++;
				}
			}
			m_ConnectCounts[connecter.m_SideNumber - 1, connecter.m_Type == Type.String ? 0 : 1]++;
		}
		foreach (var tree in m_ConnectingTree)
		{
			m_ActivCounts[tree.m_SideNumber-1, 2]++;
		}
	}
	public override void SideUpdate(int sideNumber)
	{
		ConnectionUpdate();
	}


	private void OnTriggerStay(Collider other)
	{
		StringShooter ss = other.GetComponent<StringShooter>();
		if (ss != null)
		{
			m_IsHitChara[ss.m_SideNumber - 1] = true;
			m_IsMoveChara[ss.m_SideNumber - 1] = false;// ss.m_IsMoving;
		}
		if (other.tag == "NPC") m_IsHitNPC = true;
	}
	public void SetOutLineColor(float a)
	{
		Color color = m_Materials[m_SideNumber].GetColor("_OutlineColor");
		color.r *= a;
		color.g *= a;
		color.b *= a;
		GetComponent<Renderer>().material.SetColor("_OutlineColor",color);
	}
}
