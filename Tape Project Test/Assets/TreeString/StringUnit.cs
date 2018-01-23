using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringUnit : Connecter
{


	public LineRenderer m_LineRenderer;
	public CapsuleCollider m_Collider;

	public float m_Cost;

	public Vector3 m_PointA;
	public Vector3 m_PointB;

	public GameObject m_StringStockPrefab;
	public StringStock m_StringStock;
	private void Start()
	{
		TerritoryManager.Instance.m_Strings.Add(this);
		m_Type = Type.String;
	}
	public void Create(StringShooter stringShooter, Vector3 start, Vector3 end, Transform Cartridge)
	{
		m_StringShooter = stringShooter;
		m_PointA = start;
		m_PointB = end;

		SetLine(start, end);

		m_Cost = Vector3.Distance(start, end);

		if (Cartridge != null)
		{
			m_StringStock = Instantiate(m_StringStockPrefab, Cartridge).GetComponent<StringStock>();
			m_StringStock.SetWidth(4.8f * m_Cost * 0.1f);
		}
		foreach (var item in m_ConnectingTree)
		{
			if (!item.m_Connecting.Contains(this)) item.m_Connecting.Add(this);
		}
		SetSide(stringShooter.m_SideNumber);
	}
	private void SetLine(Vector3 A, Vector3 B)
	{
		m_LineRenderer.positionCount = 2;
		m_LineRenderer.SetPositions(new Vector3[] { A, B });
		var d = Vector3.Distance(A, B);
		m_Collider.height = d;
		m_Collider.center = new Vector3(0, 0, d * 0.5f);
	}

	public override void SideUpdate(int sideNumber)
	{
		if (m_SideNumber == sideNumber) return;

		SetSide(sideNumber);

		RemoveShooter();

		m_StartConnecter.SideUpdate(sideNumber);
		m_EndConnecter.SideUpdate(sideNumber);
		foreach (var item in m_Child)
		{
			item.SideUpdate(sideNumber);
		}
	}

	public override void Delete()
	{
		foreach (var item in m_Child.ToArray())
		{
			item.Delete();
		}
		m_StartConnecter.RemoveString(this);
		m_EndConnecter.RemoveString(this);
		TerritoryManager.Instance.m_Strings.Remove(this);
		RemoveShooter();
		Destroy(gameObject);
	}

	private void RemoveShooter()
	{
		if (m_StringStock != null) m_StringStock.Delete(m_SideNumber);

		if (m_StringShooter != null)
		{
			m_StringShooter.m_Strings.Remove(this);
			m_StringShooter.m_Cost -= m_Cost;
			m_StringShooter = null;
		}
	}
}
