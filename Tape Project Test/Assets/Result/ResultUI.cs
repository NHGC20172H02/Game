using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultUI : MonoBehaviour
{

	public int m_StringLengthScore = 10;
	public int m_StringCountScore = 30;
	public int m_TreeCountScore = 100;

	public Text m_PStringLength;
	public Text m_PStringCount;
	public Text m_PTreeCount;
	public Text m_PScore;

	public Text m_EStringLength;
	public Text m_EStringCount;
	public Text m_ETreeCount;
	public Text m_EScore;

	public Image m_PWL;
	public Image m_EWL;

	public Sprite m_PWin;
	public Sprite m_EWin;
	public Sprite m_PLose;
	public Sprite m_ELose;

	public AudioSource m_BGM;
	public AudioSource m_SE;
	public AudioClip m_WinBGM;
	public AudioClip m_LoseBGM;

	public Animator m_Animator;

	// Use this for initialization
	void Start()
	{
		TerritoryManager territoryManager = TerritoryManager.Instance;

		int PScore = territoryManager.GetTreeCount(1);

		m_PTreeCount.text = PScore.ToString();

		int EScore = territoryManager.GetTreeCount(2);

		m_ETreeCount.text = EScore.ToString();

		m_BGM.clip = m_LoseBGM;
		if (PScore > EScore)
		{
			m_PWL.sprite = m_PWin;
			m_EWL.sprite = m_ELose;
			m_BGM.clip = m_WinBGM;
		}
		else if (PScore < EScore)
		{
			m_PWL.sprite = m_PLose;
			m_EWL.sprite = m_EWin;
		}
		m_BGM.Play();

		GameObject.Find("Ground").GetComponent<AudioSource>().enabled = false;
	}
	private void Update()
	{
		if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Wait Animation") && Input.anyKeyDown)
		{
			m_Animator.SetTrigger("Finish");
			SceneController.Instance.ChangeScenes(0);
		}
	}
	public void PlaySE()
	{
		//m_SE.Play();
	}
}
