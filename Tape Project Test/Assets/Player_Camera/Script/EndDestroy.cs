using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndDestroy : MonoBehaviour {

    public BattleScene m_Timer;
    public List<GameObject> m_GameObjects;

	void Update () {
		if(m_Timer.m_Timer <= 0.0f)
        {
            foreach(var item in m_GameObjects)
            {
                Destroy(item);
            }
        }
	}
}
