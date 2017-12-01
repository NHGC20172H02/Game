using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringShooter : MonoBehaviour {

	public float m_Radius;
	public GameObject m_Prefab;
	public int m_SideNumber;
	public int m_MaxCost = 200;
	public int m_Cost;
	public List<StringUnit> m_Strings;

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
		StringUnit stringUnit = Instantiate(m_Prefab, start, look).GetComponent<StringUnit>();
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