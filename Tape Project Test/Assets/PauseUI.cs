﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseUI : MonoBehaviour
{
    public GameObject m_PausePanel;
    public GameObject m_PauseImage;
    public GameObject m_PauseBackGround;
    public GameObject m_ReturnToGame;
    public GameObject m_ReturnToTitle;

    public AudioSource audioSource;

    GameObject lastSelectedGameObject = null;

    // Use this for initialization
    void Start ()
    {
        m_PausePanel.SetActive(false);
        m_PauseImage.SetActive(false);
        m_PauseBackGround.SetActive(false);
        m_ReturnToGame.SetActive(false);
        m_ReturnToTitle.SetActive(false);

        audioSource = gameObject.GetComponent<AudioSource>();

    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetButtonDown("Start") && PauseManager.Instance.Pausing())
        {
            PauseManager.Instance.Pause(false);
            
            m_PausePanel.SetActive(true);
            m_PauseImage.SetActive(true);
            m_PauseBackGround.SetActive(true);
            m_ReturnToGame.SetActive(true);
            m_ReturnToTitle.SetActive(true);

            audioSource.Play();

            // 一旦わざと選択を解除してから、Return to Gameを選択しなおす。
            //そうしないと、Highlightedトリガーが発行されないため
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(m_ReturnToGame);
        }

        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;

        if (currentSelected != lastSelectedGameObject && lastSelectedGameObject != null)
        {
            audioSource.Play();
        }
        lastSelectedGameObject = currentSelected;
    }
}
