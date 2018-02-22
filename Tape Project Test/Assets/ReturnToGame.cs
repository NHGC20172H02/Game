using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToGame : MonoBehaviour
{
    public GameObject m_PausePanel;
    
    public GameObject m_PauseBackGround;
    public GameObject m_PauseBackGround2;
    public GameObject m_ReturnToGame;
    public GameObject m_ReturnToTitle;
    public GameObject m_ManualFont;
    public GameObject m_Manual2;
    public GameObject m_Manual3;
    public GameObject m_Manual4;
    public GameObject m_Manual5;
    public GameObject m_ManualLeft;
    public GameObject m_ManualRight;

    // Use this for initialization
    void Start ()
    {
        
    }
	
	// Update is called once per frame
	void Update ()
    {
        
    }

    public void OnClick()
    {
        m_PausePanel.SetActive(false);
        m_PauseBackGround.SetActive(false);
        m_PauseBackGround2.SetActive(false);
        m_ReturnToGame.SetActive(false);
        m_ReturnToTitle.SetActive(false);
        m_ManualFont.SetActive(false);
        m_Manual2.SetActive(false);
        m_Manual3.SetActive(false);
        m_Manual4.SetActive(false);
        m_Manual5.SetActive(false);
        m_ManualLeft.SetActive(false);
        m_ManualRight.SetActive(false);
        PauseUI.m_Pfrag = false;

        PauseManager.Instance.Pause(true);
    }
}
