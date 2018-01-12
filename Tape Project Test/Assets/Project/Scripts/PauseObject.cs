using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseObject : MonoBehaviour {

	public List<MonoBehaviour> m_Scripts;
	public List<Animator> m_Animator;

	void Start () {
		PauseManager.Instance.AddObject(this);
	}

	public void Pause(bool pause)
	{
		foreach (var item in m_Scripts)
		{
            if (item != null)
            {
                item.enabled = pause;
            }
		}
		foreach (var item in m_Animator)
		{
			item.enabled = pause;
		}
	}

}
