using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manual : MonoBehaviour
{
    public GameObject m_Title;
    public GameObject m_Select4;
    public GameObject m_Select5;
    public GameObject m_ManualFont;
    public GameObject m_ManualBack;
    public GameObject m_ManualLeft;
    public GameObject m_ManualRight;
    public GameObject m_Manual2;
    public GameObject m_Manual3;
    public GameObject m_Manual4;
    public GameObject m_ManualPanel;

    public GameObject m_StartButton;
    public GameObject m_ManualButton;
    public GameObject m_GameEndButton;
    

    public void OnClick()
    {
        m_Title.SetActive(false);
        m_Select4.SetActive(false);
        m_Select5.SetActive(false);
        m_StartButton.SetActive(false);
        m_ManualButton.SetActive(false);
        m_GameEndButton.SetActive(false);

        m_Manual2.SetActive(false);
        m_Manual3.SetActive(false);
        m_Manual4.SetActive(false);
        m_ManualFont.SetActive(true);
        m_ManualBack.SetActive(true);
        m_ManualLeft.SetActive(true);
        m_ManualRight.SetActive(true);
        m_ManualPanel.SetActive(true);
    }

	// Use this for initialization
	void Start ()
    {
       
    }
	
	// Update is called once per frame
	void Update ()
    {
       
	}
}
