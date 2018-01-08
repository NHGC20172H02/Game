using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleScene : MonoBehaviour {
	public Animator m_UIAnimator;
	public float m_Timer = 180;
	public Text m_TimerUI;

    enum GameState
    {
		Ready,
        Play,
        Result,
    }

    GameState state = GameState.Play;

    // Use this for initialization
    IEnumerator Start ()
    {
		state = GameState.Ready;
		PauseManager.Instance.Pause(false);

		yield return new WaitForSeconds(2.0f);

		state = GameState.Play;
		PauseManager.Instance.Pause(true);

		yield return new WaitUntil(() => (m_Timer <= 0.0f));

		PauseManager.Instance.Pause(false);
		m_UIAnimator.SetTrigger("Finish");

		yield return new WaitForSeconds(2.0f);

		state = GameState.Result;
		SceneManager.LoadScene("Result", LoadSceneMode.Additive);
	}

	// Update is called once per frame
	void Update ()
    {
        if (state == GameState.Play)
        {
            m_TimerUI.text = ((int)m_Timer).ToString();
            m_Timer -= Time.deltaTime;
        }
    }
}
