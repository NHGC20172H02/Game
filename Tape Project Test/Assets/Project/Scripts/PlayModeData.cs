using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayModeData : ScriptableObject {

	public List<Vector2Int> m_TreePpositions;

	public int m_Cost;

	public int cost = 25;
	public void setCost(int cost_)
	{
		cost = cost_;
	}
}
