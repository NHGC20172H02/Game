using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerritoryManager : SingletonMonoBehaviour<TerritoryManager> {

	public List<Tree> m_Trees = new List<Tree>();
	public List<StringUnit> m_Strings = new List<StringUnit>();
	public List<Net> m_Nets = new List<Net>();

	void Start () {
		Initialize();
	}

	public void Initialize()
	{
		m_Trees.Clear();
		m_Strings.Clear();
		m_Nets.Clear();
	}

	public int GetTreeCount(int sideNumber)
	{
		int count = 0;
		foreach (var item in m_Trees)
		{
			count += item.m_SideNumber == sideNumber ? 1 : 0;
		}
		return count;
	}
}
