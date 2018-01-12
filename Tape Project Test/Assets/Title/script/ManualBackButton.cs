using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualBackButton : MonoBehaviour
{
    public GameObject m_ManualFont;
    public GameObject m_ManualBack;
    public GameObject m_Title;
    public GameObject m_Select4;
    public GameObject m_Select5;
    public GameObject m_StartButton;
    public GameObject m_ManualButton;
    public GameObject m_GameEndButton;

    // Use this for initialization
    void Start ()
    {
        
    }
	
	// Update is called once per frame
	void Update ()
    {
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(m_ManualBack);
    }

    public void OnClick()
    {
        m_ManualFont.SetActive(false);
        m_ManualBack.SetActive(false);

        m_Title.SetActive(true);
        m_Select4.SetActive(true);
        m_Select5.SetActive(true);
        m_StartButton.SetActive(true);
        m_ManualButton.SetActive(true);
        m_GameEndButton.SetActive(true);

        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(m_StartButton);
    }
}
