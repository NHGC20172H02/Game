using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleScene : MonoBehaviour {
	public Tree[] m_Trees;
	public float m_Timer = 180;
	public Text m_TimerUI;

    enum GameState
    {
        Play,
        Result,
    }

    GameState state = GameState.Play;

    // Use this for initialization
    void Start ()
    {
        
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (state == GameState.Play)
        {
            if (Input.GetKeyDown(KeyCode.Return) /*|| m_Timer <= 0*/)
            {
                int score = 0;
                int scoreB = 0;
                foreach (var item in m_Trees)
                {
                    score += item.m_SideNumber == 1 ? 1 : 0;
                    scoreB += item.m_SideNumber == 2 ? 1 : 0;
                }
                Sample.score = score;
                Sample.scoreB = scoreB;

                //SceneManager.LoadScene("Result", LoadSceneMode.Additive);
            }
            m_TimerUI.text = ((int)m_Timer).ToString();
            m_Timer -= Time.deltaTime;

            if (m_Timer < 0)
            {
                state = GameState.Result;
                SceneManager.LoadScene("Result", LoadSceneMode.Additive);
            }
        }
    }
}
