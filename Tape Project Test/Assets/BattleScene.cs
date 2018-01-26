using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleScene : MonoBehaviour {
	public Animator m_UIAnimator;
	public float m_Timer = 600;
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
		yield return null;

		state = GameState.Ready;
		PauseManager.Instance.Pause(false);

		yield return new WaitForSeconds(2.0f);

		state = GameState.Play;
		PauseManager.Instance.Pause(true);

		yield return new WaitUntil(() => (m_Timer <= 0.0f));

		state = GameState.Result;
		PauseManager.Instance.Pause(false);
		m_UIAnimator.SetTrigger("Finish");

		yield return new WaitForSeconds(2.0f);

		SceneController.Instance.AddScene("Result");
		m_UIAnimator.SetTrigger("Finish");
	}

	// Update is called once per frame
	void Update ()
    {
        if (state == GameState.Play)
        {
            m_Timer -= Time.deltaTime;
        }
		var time = (int)(m_Timer + 1);
		m_TimerUI.text = (time / 60).ToString("00") +":"+ (time % 60).ToString("00");
	}
}
