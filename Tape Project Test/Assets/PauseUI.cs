using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseUI : MonoBehaviour
{
    public GameObject m_PausePanel;
    public GameObject m_PauseBackGround2;
    
    public GameObject m_PauseBackGround;
    public GameObject m_ReturnToGame;
    public GameObject m_ReturnToTitle;
    public GameObject m_ManualLeft;
    public GameObject m_ManualRight;
    public GameObject m_ManualFont;
    public GameObject m_Manual2;
    public GameObject m_Manual3;
    public GameObject m_Manual4;
    public GameObject m_Manual5;
    
    public AudioSource audioSource;

    GameObject lastSelectedGameObject = null;

    public GameObject[] m_Pages;
    int m_CurrentPage = 0;

    public static bool m_Pfrag;
    
    private AudioSource manualSource;

    enum state
    {
        none,
        push,
    }

    state m_State = state.none;

    // Use this for initialization
    void Start ()
    {
        
        m_PausePanel.SetActive(false);
        m_PauseBackGround2.SetActive(false);
        m_PauseBackGround.SetActive(false);
        m_ReturnToGame.SetActive(false);
        m_ReturnToTitle.SetActive(false);
        m_ManualLeft.SetActive(false);
        m_ManualRight.SetActive(false);
        m_ManualFont.SetActive(false);
        m_Manual2.SetActive(false);
        m_Manual3.SetActive(false);
        m_Manual4.SetActive(false);
        m_Manual5.SetActive(false);

        m_Pfrag = false;
        

        audioSource = gameObject.GetComponent<AudioSource>();

        
    }

    // Update is called once per frame
    void Update ()
    {
        if (Input.GetButtonDown("Start") && m_State == state.push)
        {
            m_PausePanel.SetActive(false);
            m_PauseBackGround.SetActive(false);
            m_PauseBackGround2.SetActive(false);
            m_ReturnToGame.SetActive(false);
            m_ReturnToTitle.SetActive(false);
            m_ManualFont.SetActive(false);
            m_Manual2.SetActive(false);
            m_Manual3.SetActive(false);
            m_Manual4.SetActive(false);
            m_Manual5.SetActive(false);
            m_ManualLeft.SetActive(false);
            m_ManualRight.SetActive(false);
            PauseUI.m_Pfrag = false;
            m_CurrentPage = 0;
            m_State = state.none;

            PauseManager.Instance.Pause(true);
            return;
        }

        //スタートボタンポーズ、表示とセットするもの
        if (Input.GetButtonDown("Start") && PauseManager.Instance.Pausing())
        {
            PauseManager.Instance.Pause(false);

            m_State = state.push;

            m_PausePanel.SetActive(true);
            m_PauseBackGround2.SetActive(true);
            m_ManualFont.SetActive(true);
            m_CurrentPage = 0;

            m_PauseBackGround.SetActive(true);
            m_ReturnToGame.SetActive(true);
            m_ReturnToTitle.SetActive(true);
            m_ManualLeft.SetActive(true);
            m_ManualRight.SetActive(true);

            m_Pfrag = true;
            

            audioSource.Play();

            // 一旦わざと選択を解除してから、Return to Gameを選択しなおす。
            //そうしないと、Highlightedトリガーが発行されないため
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(m_ReturnToGame);
        }

        

        

        if (Input.GetButtonDown("RB") && !PauseManager.Instance.Pausing()&& m_Pfrag == true)
        {
            m_CurrentPage++;
            if (m_CurrentPage >= m_Pages.Length)
            {
                m_CurrentPage = 0;
            }
            UpdatePage();
            audioSource.Play();
        }
        if (Input.GetButtonDown("LB") && !PauseManager.Instance.Pausing() && m_Pfrag == true)
        {
            m_CurrentPage--;
            if (m_CurrentPage < 0)
            {
                m_CurrentPage = m_Pages.Length - 1;
            }
            UpdatePage();
            audioSource.Play();
        }

        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;

        if (currentSelected != lastSelectedGameObject && lastSelectedGameObject != null)
        {
            audioSource.Play();
        }
        lastSelectedGameObject = currentSelected;
    }

    void UpdatePage()
    {
        for (int i = 0; i < m_Pages.Length; i++)
        {
            m_Pages[i].SetActive(i == m_CurrentPage);
        }
    }
}
