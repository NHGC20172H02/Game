using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyButtons : MonoBehaviour {

    public GameObject m_Button;
    public GameObject m_Button2;
    public GameObject m_Button3;

    public GameObject s_Text;
    public GameObject s_Text2;

    // Use this for initialization
    void Start ()
    { 
        
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(Input.GetKeyDown(KeyCode.Space))
        {
            m_Button.SetActive(true);
            m_Button2.SetActive(true);
            m_Button3.SetActive(true);

            s_Text.SetActive(false);
            s_Text2.SetActive(false);
        }
	}
}
