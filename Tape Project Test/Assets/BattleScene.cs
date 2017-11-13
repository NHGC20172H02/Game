using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleScene : MonoBehaviour {
	public Tree[] m_Trees;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Return))
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
			SceneManager.LoadScene("Result");
		}
	}
}
