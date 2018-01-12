using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerritoryManager : SingletonMonoBehaviour<TerritoryManager> {

	public List<Tree> m_Trees = new List<Tree>();
	public List<StringUnit> m_Strings = new List<StringUnit>();
	public List<Net> m_Nets = new List<Net>();

	override protected void Awake(){
		base.Awake();
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

	public int GetNetCount(int sideNumber)
	{
		int count = 0;
		foreach (var item in m_Nets)
		{
			count += item.m_SideNumber == sideNumber ? 1 : 0;
		}
		return count;
	}

	public int GetStringCount(int sideNumber)
	{
		int count = 0;
		foreach (var item in m_Strings)
		{
			count += item.m_SideNumber == sideNumber ? 1 : 0;
		}
		return count;
	}

	public int GetStringLenth(int sideNumber)
	{
		int count = 0;
		foreach (var item in m_Strings)
		{
			count += item.m_SideNumber == sideNumber ? item.m_Cost : 0;
		}
		return count;
	}
}
