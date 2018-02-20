using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class MyButtons : MonoBehaviour {

    public GameObject m_GameStart;
    public GameObject m_Manual;
    public GameObject m_ManualLeft;
    public GameObject m_ManualRight;
    public GameObject m_ManualFont;
    public GameObject m_Manual2;
    public GameObject m_Manual3;
    public GameObject m_Manual4;
    public GameObject m_Manual5;
    public GameObject m_ManualBackButton;
    public GameObject m_ManualPanel;
    public GameObject m_GameEnd;
    
    public GameObject m_Select4;
    public GameObject m_Select5;
    public GameObject m_Select6;
    public GameObject m_Select7;

    public GameObject s_Text;

    public Animator animstop;

    public AudioSource audioSource;

    GameObject lastSelectedGameObject = null;

    
    // Use this for initialization
    void Start ()
    {
       
        m_GameStart.SetActive(false);
        m_Manual.SetActive(false);
        m_ManualLeft.SetActive(false);
        m_ManualRight.SetActive(false);
        m_ManualFont.SetActive(false);
        m_Manual2.SetActive(false);
        m_Manual3.SetActive(false);
        m_Manual4.SetActive(false);
        m_Manual5.SetActive(false);
        m_ManualBackButton.SetActive(false);
        m_ManualPanel.SetActive(false);
        m_GameEnd.SetActive(false);
       
        m_Select4.SetActive(true);
        m_Select5.SetActive(true);
        m_Select6.SetActive(false);
        m_Select7.SetActive(false);

        audioSource = gameObject.GetComponent<AudioSource>();
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

                    UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(m_GameStart);
                }
            }
        }


        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;

        if (currentSelected != lastSelectedGameObject && lastSelectedGameObject != null)
        {
            audioSource.Play();
        }
        lastSelectedGameObject = currentSelected;

    }
}
