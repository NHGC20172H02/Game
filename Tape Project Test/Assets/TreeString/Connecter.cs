using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connecter : MonoBehaviour {

	public int m_SideNumber = 0;
	public List<Connecter> m_Child;
	public Material[] m_Materials;

	public virtual void SideUpdate(int sideNumber){}
	public virtual void ChildUpdate(){}
	public virtual void Delete() {}

	public void SetSide(int sideNumber)
	{
		m_SideNumber = sideNumber;
		GetComponent<Renderer>().material = m_Materials[m_SideNumber];
	}

	public void AddString(Connecter connecter)
	{
		m_Child.Add(connecter);
		ChildUpdate();
	}
	public void RemoveString(Connecter connecter)
	{
		m_Child.Remove(connecter);
		ChildUpdate();
	}
}
