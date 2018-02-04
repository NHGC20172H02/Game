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
	private int[,] m_ConnectCounts = new int[2, 2];//[side, string=0 net=1]
	public bool[] m_IsHitChara = new bool[] { false, false };
	public bool[] m_IsMoveChara = new bool[] { false, false };

	private static Vector2 PosMax = new Vector2(100, 100);
	private static Vector2 PosMin = new Vector2(-100, -100);
	private static string[] Xstring = new string[] { "A", "B", "C", "D", "E", "F", "G", "H" };
	private static string[] Ystring = new string[] { "8", "7", "6", "5", "4", "3", "2", "1" };

	private void Start()
	{
		TerritoryManager.Instance.m_Trees.Add(this);
		m_Type = Type.Tree;

		//座標計算
		Vector2 pos = new Vector2(transform.position.x - PosMin.x, transform.position.z - PosMin.y);
		Vector2 div = new Vector2((PosMax.x - PosMin.x) / Xstring.Length, (PosMax.y - PosMin.y) / Ystring.Length);
		m_Zahyou = Xstring[(int)(pos.x / div.x)] + Ystring[(int)(pos.y / div.y)];
	}

	private void Update()
	{
		int i = Mathf.Abs(m_ConnectCounts[0, 0] + m_ConnectCounts[0, 1] - m_ConnectCounts[1, 0] - m_ConnectCounts[1, 1]);
		if (m_SideNumber == 0)
		{
			if (m_IsHitChara[0] && !m_IsMoveChara[0])
			{
				m_TerritoryRate += Time.deltaTime * 2;
				if (m_ConnectCounts[0, 0] + m_ConnectCounts[0, 1] > m_ConnectCounts[1, 0] + m_ConnectCounts[1, 1])
				{
					float f = 2.4f - (Mathf.Min(m_ActivCounts[0, 2], i) + Mathf.Min(m_ActivCounts[0, 0], i) + Mathf.Min(m_ActivCounts[0, 1], i)) * 0.2f;
					m_TerritoryRate += Time.deltaTime / f;
				}
			}
			if (m_IsHitChara[1] && !m_IsMoveChara[1])
			{
				m_TerritoryRate -= Time.deltaTime * 2;
				if (m_ConnectCounts[0, 0] + m_ConnectCounts[0, 1] < m_ConnectCounts[1, 0] + m_ConnectCounts[1, 1])
				{
					float f = 2.4f - (Mathf.Min(m_ActivCounts[1, 2], i) + Mathf.Min(m_ActivCounts[1, 0], i) + Mathf.Min(m_ActivCounts[1, 1], i)) * 0.2f;
					m_TerritoryRate -= Time.deltaTime / f;
				}
			}
			if (m_IsHitChara[0] == false && m_IsHitChara[1] == false && Mathf.Abs(m_TerritoryRate) < 100 && m_TerritoryRate != 0)
			{
				m_TerritoryRate = Mathf.Max(0, Mathf.Abs(m_TerritoryRate) - Time.deltaTime * 100 / 60f) * Mathf.Sign(m_TerritoryRate);
			}
		}
		else if (m_SideNumber == 2)
		{
			if (m_IsHitChara[0] && !m_IsMoveChara[0])
			{
				m_TerritoryRate += Time.deltaTime * 2;
			}
		}
		else
		{
			if (m_IsHitChara[1] && !m_IsMoveChara[1])
			{
				m_TerritoryRate -= Time.deltaTime * 2;
			}

		}

		m_IsHitChara = new bool[] { false, false };

		int sideNumber = m_SideNumber;
		if (m_SideNumber == 0 && m_TerritoryRatePre < 100 && m_TerritoryRate >= 100)
		{
			m_TerritoryRate = Mathf.Max(m_TerritoryRate, 100);
			sideNumber = 1;
		}
		if (m_SideNumber == 0 && m_TerritoryRatePre > -100 && m_TerritoryRate <= -100)
		{
			m_TerritoryRate = Mathf.Min(m_TerritoryRate, -100);
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
		m_TerritoryRatePre = m_TerritoryRate;

		m_Gauge.sprite = m_TerritoryRate >= 0 ? m_GaugeBlue : m_GaugeRed;
		m_Gauge.fillAmount = Mathf.Abs(m_TerritoryRate / 100f);

		if (m_SideNumber == sideNumber) return;

		ChangeSide(sideNumber);
	}
	private void ChangeSide(int sideNumber)
	{
		m_Renderer.material = m_Materials[sideNumber];
		LogGenerate(sideNumber, m_SideNumber);
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
	}
	private void LogGenerate(int newSide, int preSide)
	{
		if (preSide == 0)
		{
			if (newSide == 1)
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
			if (newSide == 1)
			{
				LogManager.Instance.Create(m_Zahyou + "の木がプレイヤーの陣地になった", Color.blue, false);
			}
			else
			{
				LogManager.Instance.Create(m_Zahyou + "の木が敵に奪われた", Color.red, true);
			}
		}
	}
}
