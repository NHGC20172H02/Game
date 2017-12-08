﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Net : Connecter {

	public Connecter m_StartConnecter;
	public Connecter m_EndConnecter;

	public MeshFilter m_MeshFilter;
	public MeshCollider m_Collider;

	public StringShooter m_StringShooter;

	public void SetTriangle(Vector3 A, Vector3 B, Vector3 Corner)
	{
		var mesh = new Mesh();
		mesh.vertices = new Vector3[]
		{
			A,B,Corner,
			A,B,Corner

		};
		mesh.triangles = new int[]
		{
			0,1,2,
			3,5,4
		};
		mesh.uv = new Vector2[]
		{
			new Vector2(0,1),
			new Vector2(1,1),
			new Vector2(0.5f,0),
			new Vector2(0,1),
			new Vector2(1,1),
			new Vector2(0.5f,0)
		};
		mesh.RecalculateNormals();
		m_MeshFilter.sharedMesh = mesh;
		m_Collider.sharedMesh = mesh;
	}
	public void SetConnecter(Connecter Start, Connecter End)
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
		Destroy(gameObject);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			SideUpdate(other.GetComponentInParent<StringShooter>().m_SideNumber);
		}
	}


}
