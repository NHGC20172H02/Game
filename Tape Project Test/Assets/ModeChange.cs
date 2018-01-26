using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeChange : MonoBehaviour
{
    public float b;
	// Use this for initialization
	void Start ()
    {
        
        b = gameObject.GetComponent<Tree>().m_TerritoryRate;

        if (ModeSelect.ModeData == 1)
        {   
            b = +100;
        }

        if (ModeSelect.ModeData == 3)
        {  
            b = -100;
        }
        gameObject.GetComponent<Tree>().m_TerritoryRate = b;
    }
}
