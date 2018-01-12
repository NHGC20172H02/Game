using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeScene : MonoBehaviour {

	public LoadingScene m_LoadingScene;
	public Image m_FadeImage;
	public float m_FadeTime;
	// Use this for initialization
	IEnumerator Start () {
		for (float t = 0; t < m_FadeTime; t+=Time.deltaTime)
		{
			yield return null;
			var color = m_FadeImage.color;
			color.a = t / m_FadeTime;
			m_FadeImage.color = color;
		}
		m_LoadingScene.LoadingStart();
		yield return new WaitUntil(() => m_LoadingScene.GetLoadingProgress() == 1);
		for (float t = 0; t < m_FadeTime; t += Time.deltaTime)
		{
			yield return null;
			var color = m_FadeImage.color;
			color.a = 1 - t / m_FadeTime;
			m_FadeImage.color = color;
		}
		m_LoadingScene.Finish();
	}

}
