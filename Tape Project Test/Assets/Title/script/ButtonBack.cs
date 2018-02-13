using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonBack : MonoBehaviour
{
    
    public GameObject currentSelect6;
    public GameObject currentSelect7;

    public GameObject backGameStart;
    public GameObject backManual;
    public GameObject backGameEnd;
    public GameObject backTitle;
    //public GameObject backTitleCamera;
    public GameObject backSelect4;
    public GameObject backSelect5;

    // Use this for initialization
    void Start()
    {
        gameObject.SetActive(false);

        GameStart gameStart = FindObjectOfType<GameStart>();
        MyButtons myButtons = FindObjectOfType<MyButtons>();
        
        currentSelect6 = myButtons.m_Select6;
        currentSelect7 = myButtons.m_Select7;

        backGameStart = myButtons.m_GameStart;
        backManual = myButtons.m_Manual;
        backGameEnd = myButtons.m_GameEnd;
        backTitle = gameStart.m_Title;
        //backTitleCamera = gameStart.m_TitleCamera;
        backSelect4 = myButtons.m_Select4;
        backSelect5 = myButtons.m_Select5;
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    public void OnClick()
    {
        backGameStart.SetActive(true);
        backManual.SetActive(true);
        backGameEnd.SetActive(true);
        backTitle.SetActive(true);
        //backTitleCamera.SetActive(true);
        
        backSelect4.SetActive(true);
        backSelect5.SetActive(true);
        
        currentSelect6.SetActive(false);
        currentSelect7.SetActive(false);
        
    }
}
