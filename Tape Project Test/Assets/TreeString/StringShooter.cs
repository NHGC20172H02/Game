using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringShooter : MonoBehaviour
{

	public float m_Radius;
	public GameObject m_StringUnit;
	public int m_SideNumber;
	public int m_MaxCost = 200;
	public int m_Cost;
	public List<StringUnit> m_Strings;
	public GameObject m_Net;
	public float m_NetAngleLimit = 40;
	public float m_NetDistanceLimit = 0.01f;
	public LayerMask layerMask;
	// Use this for initialization
	void Start()
	{
		m_Strings = new List<StringUnit>();
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			m_SideNumber = (m_SideNumber + 4) % 3;
		}
	}

	public void StringShoot(Vector3 start, Vector3 end)
	{
		start = SnapPoint(start);
		end = SnapPoint(end);
		Quaternion look = Quaternion.LookRotation(end - start);
		StringUnit stringUnit = Instantiate(m_StringUnit, start, look).GetComponent<StringUnit>();
		stringUnit.Create(this, start, end);
		stringUnit.SetSide(m_SideNumber);
		stringUnit.SetConnecter(GetConnecter(start), GetConnecter(end));
		m_Strings.Add(stringUnit);
		m_Cost += stringUnit.m_Cost;
		while (m_Cost > m_MaxCost)
		{
			StringUnit firstStringUnit = m_Strings[0];
			m_Strings.RemoveAt(0);
			firstStringUnit.Delete();
		}

		List<Collider> colliders = new List<Collider>(Physics.OverlapSphere(start, m_Radius, layerMask.value));
		foreach (var item in colliders)
		{
			var SU = item.GetComponent<StringUnit>();
			if (SU == null) continue;
			if (Vector3.Distance(start, SU.m_PointB) < m_NetDistanceLimit && Vector3.Angle(end - start, SU.m_PointA - start) < m_NetAngleLimit)
			{
				Net net = Instantiate(m_Net).GetComponent<Net>();
				net.m_StringShooter = this;
				net.SetTriangle(SU.m_PointA, end, start);
				net.SetSide(m_SideNumber);
				net.SetConnecter(SU, stringUnit);
			}
			if (Vector3.Distance(start, SU.m_PointA) < m_NetDistanceLimit && Vector3.Angle(end - start, SU.m_PointB - start) < m_NetAngleLimit)
			{
				Net net = Instantiate(m_Net).GetComponent<Net>();
				net.m_StringShooter = this;
				net.SetTriangle(SU.m_PointB, end, start);
				net.SetSide(m_SideNumber);
				net.SetConnecter(SU, stringUnit);
			}
		}
		colliders = new List<Collider>(Physics.OverlapSphere(end, m_Radius, layerMask.value));
		foreach (var item in colliders)
		{
			var SU = item.GetComponent<StringUnit>();
			if (SU == null) continue;
			if (Vector3.Distance(end, SU.m_PointB) < m_NetDistanceLimit && Vector3.Angle(start - end, SU.m_PointA - end) < m_NetAngleLimit)
			{
				Net net = Instantiate(m_Net).GetComponent<Net>();
				net.m_StringShooter = this;
				net.SetTriangle(SU.m_PointA, start, end);
				net.SetSide(m_SideNumber);
				net.SetConnecter(SU, stringUnit);
			}
			if (Vector3.Distance(end, SU.m_PointA) < m_NetDistanceLimit && Vector3.Angle(start - end, SU.m_PointB - end) < m_NetAngleLimit)
			{
				Net net = Instantiate(m_Net).GetComponent<Net>();
				net.m_StringShooter = this;
				net.SetTriangle(SU.m_PointB, start, end);
				net.SetSide(m_SideNumber);
				net.SetConnecter(SU, stringUnit);
			}
		}
	}

	private Connecter GetConnecter(Vector3 position)
	{
		List<Collider> colliders = new List<Collider>(Physics.OverlapSphere(position, m_Radius));
		var collider = colliders.Find((item) => item.tag == "Tree");
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
			result = Vector3.Distance(SU.m_PointA, position) < Vector3.Distance(result, position) ? SU.m_PointA : result;
			result = Vector3.Distance(SU.m_PointB, position) < Vector3.Distance(result, position) ? SU.m_PointB : result;
		}

		return (result == position + Vector3.forward * m_Radius) ? position : result;
	}
	private void CreateNet(Vector3 start, Vector3 end)
	{
	}
}