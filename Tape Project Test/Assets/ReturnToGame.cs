using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToGame : MonoBehaviour
{
    public GameObject m_PausePanel;
    public GameObject m_PauseBackGround;
    public GameObject m_ReturnToGame;
    public GameObject m_ReturnToTitle;

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void OnClick()
    {
        m_PausePanel.SetActive(false);
        m_PauseBackGround.SetActive(false);
        m_ReturnToGame.SetActive(false);
        m_ReturnToTitle.SetActive(false);

        PauseManager.Instance.Pause(true);
    }
}
