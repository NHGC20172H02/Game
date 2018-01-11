using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayModeData : ScriptableObject {

	public int cost = 25;
	public void setCost(int cost_)
	{
		cost = cost_;
	}
}
