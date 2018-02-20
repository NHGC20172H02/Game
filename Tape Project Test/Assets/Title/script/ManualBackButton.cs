using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ManualBackButton : MonoBehaviour
{
   
    public GameObject m_ManualLeft;
    public GameObject m_ManualRight;
    public GameObject m_ManualFont;
    public GameObject m_Manual2;
    public GameObject m_Manual3;
    public GameObject m_Manual4;
    public GameObject m_Manual5;
    public GameObject m_ManualBack;
    public GameObject m_ManualPanel;
    public GameObject m_Title;
    public GameObject m_Select4;
    public GameObject m_Select5;
    public GameObject m_StartButton;
    public GameObject m_ManualButton;
    public GameObject m_GameEndButton;

    public GameObject[] m_Pages;
    int m_CurrentPage = 0;

    private AudioSource manualSource;

    // Use this for initialization
    void Start ()
    {
        manualSource = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(m_ManualBack);

        if (Input.GetButtonDown("RB"))
        {
            m_CurrentPage++;
            if (m_CurrentPage >= m_Pages.Length)
            {
                m_CurrentPage = 0;
            }
            UpdatePage();
            manualSource.Play();
        }
        if (Input.GetButtonDown("LB"))
        {
            m_CurrentPage--;
            if (m_CurrentPage < 0)
            {
                m_CurrentPage = m_Pages.Length - 1;
            }
            UpdatePage();
            manualSource.Play();
        }

       
    }

    void UpdatePage()
    {
        for (int i = 0; i < m_Pages.Length; i++)
        {
            m_Pages[i].SetActive(i == m_CurrentPage);
        }
    }

    public void OnClick()
    {
        m_ManualFont.SetActive(false);
        m_ManualLeft.SetActive(false);
        m_ManualRight.SetActive(false);
        m_Manual2.SetActive(false);
        m_Manual3.SetActive(false);
        m_Manual4.SetActive(false);
        m_Manual5.SetActive(false);
        m_ManualPanel.SetActive(false);
        m_ManualBack.SetActive(false);
        m_CurrentPage = 0;

        m_Title.SetActive(true);
        m_Select4.SetActive(true);
        m_Select5.SetActive(true);
        m_StartButton.SetActive(true);
        m_ManualButton.SetActive(true);
        m_GameEndButton.SetActive(true);

        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(m_StartButton);
    }
}
