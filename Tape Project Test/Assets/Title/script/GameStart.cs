using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameStart : MonoBehaviour {

    public GameObject m_25mButton;
    public GameObject m_50mButton;
    public GameObject m_100mButton;
    public GameObject m_BackButton;

    public GameObject m_Manual;
    public GameObject m_GameEnd;

    public GameObject m_TitleCamera;
    public GameObject m_SelectCamera;

    public GameObject m_Title;

    public GameObject panel;

    public GameObject select;
    public GameObject select4;
    public GameObject select5;
    public GameObject select6;
    public GameObject select7;


    // Use this for initialization
    void Start ()
    {
        m_25mButton.SetActive(false);
        m_50mButton.SetActive(false);
        m_100mButton.SetActive(false);
        m_BackButton.SetActive(false);


        m_TitleCamera = GetComponent<MyButtons>().titleCamera;
        m_SelectCamera = GetComponent<MyButtons>().selectCamera;

        panel.SetActive(true);

        select = gameObject.GetComponent<MyButtons>().m_Select;
        select4 = gameObject.GetComponent<MyButtons>().m_Select4;
        select5 = gameObject.GetComponent<MyButtons>().m_Select5;
        select6 = gameObject.GetComponent<MyButtons>().m_Select6;
        select7 = gameObject.GetComponent<MyButtons>().m_Select7;

        
    }

	// Update is called once per frame
	void Update ()
    {
       

    }

    public void OnClick()
    {

        m_25mButton.SetActive(true);
        m_50mButton.SetActive(true);
        m_100mButton.SetActive(true);
        m_BackButton.SetActive(true);
        select.SetActive(true);
        

        m_SelectCamera.GetComponent<Camera>().enabled = true;

        // 
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(m_100mButton);

        m_Title.SetActive(false);
        gameObject.SetActive(false);
        m_Manual.SetActive(false);
        m_GameEnd.SetActive(false);

        m_TitleCamera.SetActive(false);

        panel.SetActive(false);
        select4.SetActive(false);
        select5.SetActive(false);
        select6.SetActive(true);
        select7.SetActive(true);

        
    }
}
