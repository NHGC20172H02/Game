using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUI : MonoBehaviour
{
    public GameObject m_PausePanel;
    public GameObject m_PauseBackGround;
    public GameObject m_ReturnToGame;
    public GameObject m_ReturnToTitle;

	// Use this for initialization
	void Start ()
    {
        m_PausePanel.SetActive(false);
        m_PauseBackGround.SetActive(false);
        m_ReturnToGame.SetActive(false);
        m_ReturnToTitle.SetActive(false);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetButtonDown("Start"))
        {
            m_PausePanel.SetActive(true);
            m_PauseBackGround.SetActive(true);
            m_ReturnToGame.SetActive(true);
            m_ReturnToTitle.SetActive(true);

            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(m_ReturnToGame);
        }
    }
}
