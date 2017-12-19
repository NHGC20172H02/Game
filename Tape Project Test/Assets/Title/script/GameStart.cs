using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public GameObject select2;
    public GameObject select3;

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
        select2 = gameObject.GetComponent<MyButtons>().m_Select2;
        select3 = gameObject.GetComponent<MyButtons>().m_Select3;
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
        select2.SetActive(false);
        select3.SetActive(true);
    }
}
