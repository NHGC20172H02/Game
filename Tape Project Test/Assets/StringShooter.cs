using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringShooter : MonoBehaviour {

	public float m_Radius;
	public GameObject m_Prefab;
	public int m_SideNumber;

	// Use this for initialization
	void Start () {
		
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
		stringUnit.SetLine(start, end);
		stringUnit.SetConnecter(startConnecter, endConnecter);
		stringUnit.SetSide(m_SideNumber);
	}

	private Connecter GetConnecter(Vector3 position)
	{
		Collider[] collider = Physics.OverlapSphere(position, m_Radius);
		foreach (var item in collider)
		{
			switch (item.tag)
			{
				case "Tree":
				case "String":
					return item.GetComponent<Connecter>();
				default:
					break;
			}
		}
		return null;
	}
}
