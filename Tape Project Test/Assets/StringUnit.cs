﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringUnit : Connecter {

	public Connecter m_StartConnecter;
	public Connecter m_EndConnecter;

	public LineRenderer m_LineRenderer;
	public CapsuleCollider m_Collider;

	public Material[] m_Materials;

	public void SetLine(Vector3 Start, Vector3 End)
	{
		m_LineRenderer.SetPositions(new Vector3[] { Start, End });
		m_Collider.height = Vector3.Distance(Start, End);
		m_Collider.center = new Vector3(0, 0, m_Collider.height * 0.5f);
	}

	public void SetConnecter(Connecter Start,Connecter End)
	{
		m_StartConnecter = Start;
		m_EndConnecter = End;
		m_StartConnecter.m_Child.Add(this);
		m_EndConnecter.m_Child.Add(this);
	}

	public void SetSide(int sideNumber)
	{
		ChengeSide(sideNumber);
		m_StartConnecter.SideUpdate();
		m_EndConnecter.SideUpdate();
	}

	public override void SideUpdate()
	{

	}

	private void ChengeSide(int i)
	{
		m_SideNumber = i;
		GetComponent<Renderer>().material = m_Materials[m_SideNumber];
	}

}
