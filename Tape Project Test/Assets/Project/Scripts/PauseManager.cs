using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : SingletonMonoBehaviour<PauseManager> {

	List<PauseObject> m_PauseObjects = new List<PauseObject>();

	public void AddObject(PauseObject pauseObject)
	{
		m_PauseObjects.Add(pauseObject);
	}

	public void Pause(bool pause)
	{
		foreach (var pauseObject in m_PauseObjects)
		{
			pauseObject.Pause(pause);
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.O))
		{
			Pause(true);
		}
		if (Input.GetButtonDown("Start"))
		{
			Pause(false);
		}
	}
}
