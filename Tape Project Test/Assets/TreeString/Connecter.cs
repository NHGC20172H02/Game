using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connecter : MonoBehaviour {

	public int m_SideNumber = 0;
	public List<StringUnit> m_Child;
	public Material[] m_Materials;

	virtual public void SideUpdate(int sideNumber)
	{

	}
	virtual public void ChildUpdate()
	{

	}
	public void SetSide(int sideNumber)
	{
		m_SideNumber = sideNumber;
		GetComponent<Renderer>().material = m_Materials[m_SideNumber];
	}

	public void AddString(StringUnit stringUnit)
	{
		m_Child.Add(stringUnit);
		ChildUpdate();
	}
	public void RemoveString(StringUnit stringUnit)
	{
		m_Child.Remove(stringUnit);
		ChildUpdate();
	}
}
