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
	private Vector3 m_PreStartPoint;
	private Vector3 m_PreEndPoint;
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
		if (Vector3.Distance(start, m_PreEndPoint) < m_NetDistanceLimit && Vector3.Angle(end - start, m_PreStartPoint - start) < m_NetAngleLimit)
		{
			Net net = Instantiate(m_Net).GetComponent<Net>();
			net.m_StringShooter = this;
			net.SetTriangle(m_PreStartPoint, end, start);
			net.SetSide(m_SideNumber);
			net.SetConnecter(m_Strings[m_Strings.Count - 1], stringUnit);
		}
		m_PreEndPoint = end;
		m_PreStartPoint = start;
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
}