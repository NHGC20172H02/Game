using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class MyButtons : MonoBehaviour {

    public GameObject m_GameStart;
    public GameObject m_Manual;
    public GameObject m_GameEnd;

    public GameObject titleCamera;
    public GameObject selectCamera;

    public GameObject m_Select;
    public GameObject m_Select2;
    public GameObject m_Select3;

    public GameObject s_Text;

    public Animator animstop;

    // Use this for initialization
    void Start ()
    {
        m_GameStart.SetActive(false);
        m_Manual.SetActive(false);
        m_GameEnd.SetActive(false);

        titleCamera.SetActive(true);
        selectCamera.GetComponent<Camera>().enabled = false;

        m_Select.SetActive(false);
        m_Select2.SetActive(true);
        m_Select3.SetActive(false);
    }
	
	// Update is called once per frame
	void Update ()
    {   
        if (animstop.isActiveAndEnabled)
        {
            AnimatorStateInfo a = animstop.GetCurrentAnimatorStateInfo(0);

            if (a.fullPathHash == Animator.StringToHash("Base Layer.PressButtonAnimation"))
            {
                if (a.normalizedTime >= 1.0f)
                {
                    m_GameStart.SetActive(true);
                    m_GameStart.GetComponent<Button>().Select();
                    m_Manual.SetActive(true);
                    m_GameEnd.SetActive(true);

                    s_Text.SetActive(false);
                }
            }
        }

	}
}
