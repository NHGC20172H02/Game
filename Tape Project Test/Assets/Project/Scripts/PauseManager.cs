using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : SingletonMonoBehaviour<PauseManager> {

	List<PauseObject> m_PauseObjects = new List<PauseObject>();

    private bool m_Pause;

	public void AddObject(PauseObject pauseObject)
	{
		m_PauseObjects.Add(pauseObject);
	}

	public void Pause(bool pause)
	{
        m_Pause = pause;
		foreach (var pauseObject in m_PauseObjects)
		{
			pauseObject.Pause(pause);
		}
	}

	private void Update()
	{
		//if (Input.GetKeyDown(KeyCode.O))
		//{
		//	Pause(true);
		//}
		//if (Input.GetButtonDown("Start"))
		//{
		//	Pause(false);
		//}
	}

    public bool Pausing()
    { 
        return m_Pause;
    }
}
