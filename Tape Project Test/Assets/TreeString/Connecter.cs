using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connecter : MonoBehaviour {

	public enum Type
	{
		none,
		Tree,
		String,
		Net
	}

	public Type m_Type = Type.none;
	public int m_SideNumber = 0;
	public StringShooter m_StringShooter;
	public Connecter m_StartConnecter;
	public Connecter m_EndConnecter;
	public List<Connecter> m_Child = new List<Connecter>();
	public List<Tree> m_ConnectingTree = new List<Tree>();
	public Material[] m_Materials;

	public virtual void SideUpdate(int sideNumber){}
	public virtual void ConnectionUpdate(){}
	public virtual void Delete() {}

	public void SetSide(int sideNumber)
	{
		m_SideNumber = sideNumber;
		GetComponent<Renderer>().material = m_Materials[m_SideNumber];
	}
	public void SetConnecter(Connecter Start, Connecter End)
	{
		m_StartConnecter = Start;
		m_EndConnecter = End;
		m_StartConnecter.AddString(this);
		m_EndConnecter.AddString(this);
	}

	public void AddString(Connecter connecter)
	{
		m_Child.Add(connecter);
		ConnectionUpdate();
	}
	public void RemoveString(Connecter connecter)
	{
		m_Child.Remove(connecter);
		ConnectionUpdate();
	}
	public void AddTree(Tree tree)
	{
		if (tree == null) return;
		if (!m_ConnectingTree.Contains(tree)) m_ConnectingTree.Add(tree);
	}
}
