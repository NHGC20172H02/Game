using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonBack : MonoBehaviour
{
    public GameObject current100m;
    public GameObject current50m;
    public GameObject current25m;
    public GameObject back;
    public GameObject currentSelect;
    public GameObject currentSelect6;
    public GameObject currentSelect7;
    public GameObject currentSelectCamera;

    public GameObject backGameStart;
    public GameObject backManual;
    public GameObject backGameEnd;
    public GameObject backTitle;
    public GameObject backTitleCamera;
    public GameObject backPanel;
    public GameObject backSelect4;
    public GameObject backSelect5;

    // Use this for initialization
    void Start()
    {
        gameObject.SetActive(false);

        GameStart gameStart = FindObjectOfType<GameStart>();
        MyButtons myButtons = FindObjectOfType<MyButtons>();

        current100m  = gameStart.m_100mButton;
        current50m   = gameStart.m_50mButton;
        current25m = gameStart.m_25mButton;
        back      = gameStart.m_BackButton;
        currentSelectCamera = gameStart.m_SelectCamera;
        currentSelect = myButtons.m_Select;
        currentSelect6 = myButtons.m_Select6;
        currentSelect7 = myButtons.m_Select7;

        backGameStart = myButtons.m_GameStart;
        backManual = myButtons.m_Manual;
        backGameEnd = myButtons.m_GameEnd;
        backTitle = gameStart.m_Title;
        backTitleCamera = gameStart.m_TitleCamera;
        backPanel = gameStart.panel;
        backSelect4 = myButtons.m_Select4;
        backSelect5 = myButtons.m_Select5;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            backGameStart.SetActive(true);
            backManual.SetActive(true);
            backGameEnd.SetActive(true);
            backTitle.SetActive(true);
            backTitleCamera.SetActive(true);
            backPanel.SetActive(true);
            backSelect4.SetActive(true);
            backSelect5.SetActive(true);

            // 
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(backGameStart);


            current100m.SetActive(false);
            current50m.SetActive(false);
            current25m.SetActive(false);
            back.SetActive(false);
            currentSelect.SetActive(false);
            currentSelect6.SetActive(false);
            currentSelect7.SetActive(false);
            currentSelectCamera.GetComponent<Camera>().enabled = false;
        }
    }

    public void OnClick()
    {
        backGameStart.SetActive(true);
        backManual.SetActive(true);
        backGameEnd.SetActive(true);
        backTitle.SetActive(true);
        backTitleCamera.SetActive(true);
        backPanel.SetActive(true);
        backSelect4.SetActive(true);
        backSelect5.SetActive(true);

        // 
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(backGameStart);

        current100m.SetActive(false);
        current50m.SetActive(false);
        current25m.SetActive(false);
        back.SetActive(false);
        currentSelect.SetActive(false);
        currentSelect6.SetActive(false);
        currentSelect7.SetActive(false);
        currentSelectCamera.GetComponent<Camera>().enabled = false;
    }
}
