using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringShooter : MonoBehaviour {

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

	// Use this for initialization
	void Start () {
		m_Strings = new List<StringUnit>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			m_SideNumber = (m_SideNumber + 4) % 3;
		}
	}

	public void StringShoot(Vector3 start,Vector3 end)
	{
		Connecter startConnecter = GetConnecter(start);
		Connecter endConnecter = GetConnecter(end);
		Quaternion look = Quaternion.LookRotation(end - start);
		StringUnit stringUnit = Instantiate(m_StringUnit, start, look).GetComponent<StringUnit>();
		stringUnit.m_StringShooter = this;
		stringUnit.SetLine(start, end);
		stringUnit.SetCost((int)Vector3.Distance(start, end));
		stringUnit.SetSide(m_SideNumber);
		stringUnit.SetConnecter(startConnecter, endConnecter);
		m_Strings.Add(stringUnit);
		m_Cost += stringUnit.m_Cost;
		while (m_Cost>m_MaxCost)
		{
			StringUnit firstStringUnit = m_Strings[0];
			m_Strings.RemoveAt(0);
			firstStringUnit.Delete();
		}
		if(Vector3.Distance(start, m_PreEndPoint)< m_NetDistanceLimit && Vector3.Angle(end - start, m_PreStartPoint - start) < m_NetAngleLimit)
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
		Collider[] collider = Physics.OverlapSphere(position, m_Radius);
		Connecter result = null;
		foreach (var item in collider)
		{
			switch (item.tag)
			{
				case "Tree":
					return item.GetComponent<Connecter>();
				case "String":
					result = item.GetComponent<Connecter>();
					break;
				default:
					break;
			}
		}
		return result;
	}
}