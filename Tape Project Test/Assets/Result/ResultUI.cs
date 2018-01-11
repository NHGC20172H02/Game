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

	// Use this for initialization
	IEnumerator Start()
	{
		TerritoryManager territoryManager = TerritoryManager.Instance;

		int StringLength = territoryManager.GetStringLenth(1);
		int StringCount = territoryManager.GetStringCount(1) + territoryManager.GetNetCount(1);
		int TreeCount = territoryManager.GetTreeCount(1);
		int PScore = StringLength * m_StringLengthScore + StringCount * m_StringCountScore + TreeCount * m_TreeCountScore;

		m_PStringLength.text = StringLength.ToString();
		m_PStringCount.text = StringCount.ToString();
		m_PTreeCount.text = TreeCount.ToString();
		m_PScore.text = PScore.ToString();

		StringLength = territoryManager.GetStringLenth(2);
		StringCount = territoryManager.GetStringCount(2) + territoryManager.GetNetCount(2);
		TreeCount = territoryManager.GetTreeCount(2);
		int EScore = StringLength * m_StringLengthScore + StringCount * m_StringCountScore + TreeCount * m_TreeCountScore;

		m_EStringLength.text = StringLength.ToString();
		m_EStringCount.text = StringCount.ToString();
		m_ETreeCount.text = TreeCount.ToString();
		m_EScore.text = EScore.ToString();

		if (PScore > EScore)
		{
			m_PWL.sprite = m_PWin;
			m_EWL.sprite = m_ELose;
		}
		else if (PScore < EScore)
		{
			m_PWL.sprite = m_PLose;
			m_EWL.sprite = m_EWin;
		}

		yield return new WaitForSeconds(10);

		SceneController.Instance.ChangeScenes(0);
	}
}
