using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStart : MonoBehaviour {

    public GameObject m_25mButton;
    public GameObject m_50mButton;
    public GameObject m_100mButton;

    
    public GameObject m_Manual;
    public GameObject m_GameEnd;

    public GameObject titleCamera;
    public GameObject selectCamera;

    public GameObject m_Title;

    // Use this for initialization
    void Start ()
    {
        m_25mButton.SetActive(false);
        m_50mButton.SetActive(false);
        m_100mButton.SetActive(false);

        //titleCamera = GameObject.Find("MainCamera");
        //selectCamera = GameObject.Find("AICamera");

        titleCamera.SetActive(true);
        selectCamera.SetActive(false);
        
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void OnClick()
    {

        //ログに表示
        Debug.Log("Button click");

        m_25mButton.SetActive(true);
        m_50mButton.SetActive(true);
        m_100mButton.SetActive(true);

        m_100mButton.GetComponent<Button>().Select();

        m_Title.SetActive(false);
        gameObject.SetActive(false);
        m_Manual.SetActive(false);
        m_GameEnd.SetActive(false);

        titleCamera.SetActive(false);
        selectCamera.SetActive(true);
    }
}
