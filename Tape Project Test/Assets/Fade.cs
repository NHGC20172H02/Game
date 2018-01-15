using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    float fadeSpeed = 0.1f;
    float alfa;
    float red, green, blue, alpha;

    public float m_Timer = 20;

	// Use this for initialization
	void Start ()
    {

        red = GetComponent<Image>().color.r;
        green = GetComponent<Image>().color.g;
        blue = GetComponent<Image>().color.b;
        alpha = GetComponent<Image>().color.a;
    }
	
	// Update is called once per frame
	void Update ()
    {
        GetComponent<Image>().color = new Color(red, green, blue, alfa);

        m_Timer -= Time.deltaTime;

        if(m_Timer <= 10 && m_Timer >= 5)
        {
            alfa += fadeSpeed;
            
        }

        if(m_Timer <= 5 && m_Timer >= 0)
        {
            alfa += fadeSpeed;
           
        }
       
    }
}
