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
    public GameObject currentSelect3;
    public GameObject currentSelectCamera;

    public GameObject backGameStart;
    public GameObject backManual;
    public GameObject backGameEnd;
    public GameObject backTitle;
    public GameObject backTitleCamera;
    public GameObject backPanel;
    public GameObject backSelect2;

    // Use this for initialization
    void Start()
    {
        gameObject.SetActive(false);

        current100m  =    gameObject.GetComponent<GameStart>().m_100mButton;
        current50m   =    gameObject.GetComponent<GameStart>().m_50mButton;
        current25m =    gameObject.GetComponent<GameStart>().m_25mButton;
        back      =    gameObject.GetComponent<GameStart>().m_BackButton;
        currentSelectCamera = gameObject.GetComponent<GameStart>().m_SelectCamera;
        currentSelect = gameObject.GetComponent<MyButtons>().m_Select;
        currentSelect3 = gameObject.GetComponent<MyButtons>().m_Select3;

        backGameStart = gameObject.GetComponent<MyButtons>().m_GameStart;
        backManual = gameObject.GetComponent<MyButtons>().m_Manual;
        backGameEnd = gameObject.GetComponent<MyButtons>().m_GameEnd;
        backTitle = gameObject.GetComponent<GameStart>().m_Title;
        backTitleCamera = gameObject.GetComponent<GameStart>().m_TitleCamera;
        backPanel = gameObject.GetComponent<GameStart>().panel;
        backSelect2 = gameObject.GetComponent<MyButtons>().m_Select2;
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
            backSelect2.SetActive(true);

            // 
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(backGameStart);


            current100m.SetActive(false);
            current50m.SetActive(false);
            current25m.SetActive(false);
            back.SetActive(false);
            currentSelect.SetActive(false);
            currentSelect3.SetActive(false);
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
        backSelect2.SetActive(true);

        // 
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(backGameStart);

        current100m.SetActive(false);
        current50m.SetActive(false);
        current25m.SetActive(false);
        back.SetActive(false);
        currentSelect.SetActive(false);
        currentSelect3.SetActive(false);
        currentSelectCamera.GetComponent<Camera>().enabled = false;
    }
}
