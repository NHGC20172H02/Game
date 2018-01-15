using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DestroyUnit : MonoBehaviour
{
    public float m_Timer = 10;

	// Use this for initialization
	void Start ()
    {
        
	}
	
	// Update is called once per frame
	void Update ()
    {
        m_Timer -= Time.deltaTime;

        if(m_Timer <= 0)
        {
            GameObject[] units = GameObject.FindGameObjectsWithTag("String");

            foreach (GameObject unit in units)
            {
                Destroy(unit);
            }

            m_Timer = 10;
        }
        
    }
}
