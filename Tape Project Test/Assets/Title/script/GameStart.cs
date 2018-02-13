using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameStart : MonoBehaviour {

    public GameObject m_Manual;
    public GameObject m_GameEnd;

    //public GameObject m_TitleCamera;

    public GameObject m_Title;

    public GameObject panel;
    
    public GameObject select4;
    public GameObject select5;
    public GameObject select6;
    public GameObject select7;


    // Use this for initialization
    void Start ()
    {

        MyButtons myButtons = FindObjectOfType<MyButtons>();

        panel.SetActive(true);
        
        select4 = myButtons.m_Select4;
        select5 = myButtons.m_Select5;
        select6 = myButtons.m_Select6;
        select7 = myButtons.m_Select7;
    }

	// Update is called once per frame
	void Update ()
    {
       

    }

    public void OnClick()
    {
        SceneController.Instance.ChangeScenes(3);


        m_Title.SetActive(false);
        gameObject.SetActive(false);
        m_Manual.SetActive(false);
        m_GameEnd.SetActive(false);

        panel.SetActive(true);
        select4.SetActive(false);
        select5.SetActive(false);
        select6.SetActive(true);
        select7.SetActive(true);
    }
}
