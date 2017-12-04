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

	public void SetLine(Vector3 Start, Vector3 End)
	{
		m_PointA = Start;
		m_PointB = End;
		m_LineRenderer.SetPositions(new Vector3[] { Start, End });
		m_Collider.height = Vector3.Distance(Start, End);
		m_Collider.center = new Vector3(0, 0, m_Collider.height * 0.5f);
	}

	public void SetCost(int Cost)
	{
		m_Cost = Cost;
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
		m_StringShooter.m_Strings.Remove(this);
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
		m_StringShooter.m_Cost -= m_Cost;
		Destroy(gameObject);
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
			SideUpdate(other.GetComponentInParent<StringShooter>().m_SideNumber);
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
