using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class MyButtons : MonoBehaviour {

    public GameObject m_Button;
    public GameObject m_Button2;
    public GameObject m_Button3;

    public GameObject s_Text;

    public Animator animstop;

    // Use this for initialization
    void Start ()
    {
        m_Button.SetActive(false);
        m_Button2.SetActive(false);
        m_Button3.SetActive(false);
    }
	
	// Update is called once per frame
	void Update ()
    {   
        AnimatorStateInfo a = animstop.GetCurrentAnimatorStateInfo(0);

        if (a.fullPathHash == Animator.StringToHash("Base Layer.PressButtonAnimation"))
        {
            if(a.normalizedTime >= 1.0f)
            {
                m_Button.SetActive(true);
                m_Button.GetComponent<Button>().Select();
                m_Button2.SetActive(true);
                m_Button3.SetActive(true);
                
                s_Text.SetActive(false);
                
            }   
        }

	}
}
