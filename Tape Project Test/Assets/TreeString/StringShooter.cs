using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringShooter : MonoBehaviour
{

	public float m_Radius;
	public GameObject m_StringUnit;
	public int m_SideNumber;
	public int m_MaxCost = 200;
	public float m_Cost;
	public List<StringUnit> m_Strings;
	public GameObject m_Net;
	public float m_NetAngleLimit = 40;
	public int m_NetCostLimit = 30;
	public LayerMask layerMask;
	public Transform m_Cartridge;
	public PlayModeData m_PlayModeData;

	public bool m_IsMoving;
	public Vector3 m_Prepos;

	// Use this for initialization
	void Start()
	{
		m_Strings = new List<StringUnit>();
		m_MaxCost = 1000;
	}

	// Update is called once per frame
	void Update()
	{
		m_IsMoving = 0.001f < Vector3.Distance(m_Prepos, transform.position);
		m_Prepos = transform.position;
	}

	public void StringShoot(Vector3 start, Vector3 end)
	{
		start = SnapPoint(start);
		end = SnapPoint(end);
		Quaternion look = Quaternion.LookRotation(end - start);
		StringUnit stringUnit = Instantiate(m_StringUnit, start, look).GetComponent<StringUnit>();
		stringUnit.AddTree(GetConnectingTree(start));
		stringUnit.AddTree(GetConnectingTree(end));
		stringUnit.Create(this, start, end,m_Cartridge);
		stringUnit.SetConnecter(GetConnecter(start,stringUnit.GetComponent<Collider>()), GetConnecter(end, stringUnit.GetComponent<Collider>()));
		m_Strings.Add(stringUnit);
		m_Cost += stringUnit.m_Cost;
		while (m_Cost > m_MaxCost)
		{
			StringUnit firstStringUnit = m_Strings[0];
			//m_Strings.RemoveAt(0);
			firstStringUnit.Delete();
		}
		if (/*(stringUnit.m_StartConnecter is Tree || stringUnit.m_EndConnecter is Tree && false) &&*/ stringUnit.m_Cost <= m_NetCostLimit)
		{
			List<Collider> colliders = new List<Collider>(Physics.OverlapSphere(start, m_Radius, layerMask.value));
			colliders.Remove(stringUnit.m_Collider);
			foreach (var item in colliders)
			{
				var SU = item.GetComponent<StringUnit>();
				if (SU == null) continue;
				if (SU.m_SideNumber != m_SideNumber) continue;
				if (SU.m_Cost > m_NetCostLimit) continue;
				if (SU.m_PointA == start && SU.m_PointB == end || SU.m_PointA == end && SU.m_PointB == start) continue;
				if (Vector3.Angle(end - start, SU.m_PointA - start) < m_NetAngleLimit)
				{
					Net net = Instantiate(m_Net).GetComponent<Net>();
					net.AddTree(GetConnectingTree(SU.m_PointA));
					net.AddTree(GetConnectingTree(start));
					net.AddTree(GetConnectingTree(end));
					net.Create(this, SU.m_PointA, end, start);
					net.SetConnecter(SU, stringUnit);
				}
				if (Vector3.Angle(end - start, SU.m_PointB - start) < m_NetAngleLimit)
				{
					Net net = Instantiate(m_Net).GetComponent<Net>();
					net.AddTree(GetConnectingTree(SU.m_PointB));
					net.AddTree(GetConnectingTree(start));
					net.AddTree(GetConnectingTree(end));
					net.Create(this, SU.m_PointB, end, start);
					net.SetConnecter(SU, stringUnit);
				}
			}
			colliders = new List<Collider>(Physics.OverlapSphere(end, m_Radius, layerMask.value));
			colliders.Remove(stringUnit.m_Collider);
			foreach (var item in colliders)
			{
				var SU = item.GetComponent<StringUnit>();
				if (SU == null) continue;
				if (SU.m_SideNumber != m_SideNumber) continue;
				if (SU.m_Cost > m_NetCostLimit) continue;
				if (SU.m_PointA == start && SU.m_PointB == end || SU.m_PointA == end && SU.m_PointB == start) continue;
				if (Vector3.Angle(start - end, SU.m_PointA - end) < m_NetAngleLimit)
				{
					Net net = Instantiate(m_Net).GetComponent<Net>();
					net.AddTree(GetConnectingTree(SU.m_PointA));
					net.AddTree(GetConnectingTree(start));
					net.AddTree(GetConnectingTree(end));
					net.Create(this, SU.m_PointA, start, end);
					net.SetConnecter(SU, stringUnit);
				}
				if (Vector3.Angle(start - end, SU.m_PointB - end) < m_NetAngleLimit)
				{
					Net net = Instantiate(m_Net).GetComponent<Net>();
					net.AddTree(GetConnectingTree(SU.m_PointB));
					net.AddTree(GetConnectingTree(start));
					net.AddTree(GetConnectingTree(end));
					net.Create(this, SU.m_PointB, start, end);
					net.SetConnecter(SU, stringUnit);
				}
			}
		}
	}

	private Connecter GetConnecter(Vector3 position, Collider ignoercollider)
	{
		List<Collider> colliders = new List<Collider>(Physics.OverlapSphere(position, m_Radius));
		colliders.Remove(ignoercollider);
		var	collider = colliders.Find((item) => item.tag == "Tree");
		if (collider == null) collider = colliders.Find((item) => item.tag == "Net");
		if (collider == null) collider = colliders.Find((item) => item.tag == "String");
		return collider.GetComponent<Connecter>();
	}
	private Vector3 SnapPoint(Vector3 position)
	{
		Vector3 result = position + Vector3.forward * m_Radius;
		List<Collider> colliders = new List<Collider>(Physics.OverlapSphere(position, m_Radius, layerMask.value));
		foreach (var item in colliders)
		{
			var SU = item.GetComponent<StringUnit>();
			if (SU == null) continue;
			if (SU.m_SideNumber != m_SideNumber) continue;
			result = Vector3.Distance(SU.m_PointA, position) < Vector3.Distance(result, position) ? SU.m_PointA : result;
			result = Vector3.Distance(SU.m_PointB, position) < Vector3.Distance(result, position) ? SU.m_PointB : result;
		}

		return (result == position + Vector3.forward * m_Radius) ? position : result;
	}
	private Tree GetConnectingTree(Vector3 position)
	{
		var c = Physics.OverlapSphere(position, m_Radius, LayerMask.GetMask(new string[] { "Tree" }));
		return c.Length == 0 ? null : c[0].GetComponent<Tree>();
	}
}