using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DestroyUnit : MonoBehaviour
{
    public float m_Timer = 90;

    
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
            GameObject[] nets = GameObject.FindGameObjectsWithTag("Net");

            foreach (GameObject unit in units)
            {
                unit.GetComponent<StringUnit>().Delete();
                
            }

            foreach (GameObject net in nets)
            {
                net.GetComponent<Net>().Delete();

            }

            m_Timer = 90;
        }
        
    }
}
