using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : Connecter {

	public override void ChildUpdate()
	{
		List<int> count = new List<int> { 0, 0, 0 };
		foreach (var item in m_Child)
		{
			count[item.m_SideNumber]++;
		}
		int sideNumber = m_SideNumber;
		for (int i = 0; i < count.Count; i++)
		{
			if (count[sideNumber] < count[i])
			{
				sideNumber = i;
			}
		}
		SetSide(sideNumber);
	}

}
