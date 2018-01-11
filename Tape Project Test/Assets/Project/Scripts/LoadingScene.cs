using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScene : MonoBehaviour {
	private bool m_StartLoading;
	private bool m_IsEnd;
	IEnumerator Start () {
		m_StartLoading = false;
		m_IsEnd = false;
		yield return new WaitUntil(() => m_StartLoading);
		SceneController.Instance.m_IsLoadingAnimation = false;

		yield return new WaitUntil(() => SceneController.Instance.m_LoadingProgress == 1);
		//Debug.Log("load100%");
		yield return new WaitUntil(() => m_IsEnd);
		//Debug.Log("fadeEND");
		SceneController.Instance.m_IsLoadingAnimation = false;

	}

	// ローディング開始アニメーション終了
	public void LoadingStart()
	{
		m_StartLoading = true;
	}

	// ローディング終了アニメーション終了
	public void Finish()
	{
		m_IsEnd = true;
	}
	public float GetLoadingProgress()
	{
		return SceneController.Instance.m_LoadingProgress;
	}
}
