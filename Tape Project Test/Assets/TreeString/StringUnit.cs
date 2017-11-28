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

	public void SetLine(Vector3 Start, Vector3 End)
	{
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

	public void Delete()
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


}
