using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringUnit : Connecter {

	public Connecter m_StartConnecter;
	public Connecter m_EndConnecter;

	public LineRenderer m_LineRenderer;
	public CapsuleCollider m_Collider;

	public int m_Cost;
	public StringShooter m_StringShooter;

	public Vector3 m_PointA;
	public Vector3 m_PointB;

	public GameObject m_StringStockPrefab;
	public StringStock m_StringStock;
	private void Start()
	{
		TerritoryManager.Instance.m_Strings.Add(this);
	}
	public void Create(StringShooter stringShooter, Vector3 start, Vector3 end, Transform Cartridge)
	{
		m_StringShooter = stringShooter;
		m_PointA = start;
		m_PointB = end;
		StretchReturn();
		float distance = Vector3.Distance(start, end);
		m_Cost = (int)(distance * 0.1f);
		m_Collider.height = distance;
		m_Collider.center = new Vector3(0, 0, distance * 0.5f);
		if (Cartridge != null)
		{
			m_StringStock = Instantiate(m_StringStockPrefab, Cartridge).GetComponent<StringStock>();
			m_StringStock.SetWidth(4.8f * m_Cost * (100 / m_StringShooter.m_MaxCost));
		}
	}

	public void SetConnecter(Connecter Start,Connecter End)
	{
		m_StartConnecter = Start;
		m_EndConnecter = End;
		m_StartConnecter.AddString(this);
		m_EndConnecter.AddString(this);
	}

	public override void SideUpdate(int sideNumber)
	{
		if (m_SideNumber == sideNumber) return;
		SetSide(sideNumber);
		if (m_StringShooter != null)
		{
			if (m_StringStock != null)
			{
				m_StringStock.Delete(m_SideNumber);
			}
			m_StringShooter.m_Strings.Remove(this);
			m_StringShooter.m_Cost -= m_Cost;
			m_StringShooter = null;
		}
		m_StartConnecter.SideUpdate(sideNumber);
		m_EndConnecter.SideUpdate(sideNumber);
		foreach (var item in m_Child)
		{
			item.SideUpdate(sideNumber);
		}
	}

	public override void Delete()
	{
        if (m_StringShooter != null)
        {
            m_StringShooter.m_Strings.Remove(this);
            m_StringShooter.m_Cost -= m_Cost;
        }

        foreach (var item in m_Child.ToArray())
		{
			item.Delete();
		}
		m_StartConnecter.RemoveString(this);
		m_EndConnecter.RemoveString(this);
		//m_StringShooter.m_Strings.Remove(this);
		//m_StringShooter.m_Cost -= m_Cost;
		Destroy(gameObject);
		TerritoryManager.Instance.m_Strings.Remove(this);
		if (m_StringStock != null)
		{
			m_StringStock.Delete(m_SideNumber);
		}

        
    }
	public void Stretch(Vector3 point)
	{
		m_LineRenderer.positionCount = 9;
		m_LineRenderer.SetPositions(new Vector3[] {
			m_PointA,
			GetStretchPoint(m_PointA,m_PointA+(point-m_PointA).normalized,point-transform.forward*2,point,0.5f),
			GetStretchPoint(m_PointA,m_PointA+(point-m_PointA).normalized,point-transform.forward*2,point,0.75f),
			GetStretchPoint(m_PointA,m_PointA+(point-m_PointA).normalized,point-transform.forward*2,point,0.875f),
			point,
			GetStretchPoint(point,point+transform.forward*2,m_PointB+(point-m_PointB).normalized,m_PointB,0.125f),
			GetStretchPoint(point,point+transform.forward*2,m_PointB+(point-m_PointB).normalized,m_PointB,0.25f),
			GetStretchPoint(point,point+transform.forward*2,m_PointB+(point-m_PointB).normalized,m_PointB,0.5f),
			m_PointB
		});
	}

	public void StretchReturn()
	{
		m_LineRenderer.positionCount = 2;
		m_LineRenderer.SetPositions(new Vector3[] { m_PointA, m_PointB });
	}

	private Vector3 GetStretchPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		var t_ = 1f - t;
		return t_ * t_*t_*p0+
			3f*t_*t_*t*p1+
			3f*t_*t*t*p2+
			t*t*t * p3;
	}
}
