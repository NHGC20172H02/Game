using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnSelect : MonoBehaviour {

	public GameObject m_Enemy;

	public GameObject m_AnimationIcon;

	public GameObject m_Player;

	public GameObject m_Ring;

	public Sprite m_EnemyArrow;
	public Sprite m_TreeArrow;

	public SpownTreeGenerate spownTreeGenerate;

	public bool m_IsSelected;
	// Use this for initialization
	void Start () {
		m_IsSelected = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void StartSelected()
	{
		if (m_IsSelected == false) StartCoroutine(Selected());
	}
	public IEnumerator Selected()
	{
		m_IsSelected = true;
		yield return null;
		Instantiate(m_Ring, m_Player.transform.position, Quaternion.identity, transform);
		yield return new WaitForSeconds(1.5f);
		var icon = Instantiate(m_AnimationIcon, m_Enemy.transform.position, Quaternion.identity, transform).GetComponent<IconAnimation>();
		icon.SetPPos(m_Player.transform.position);
		icon.SetColor(Color.red);
		icon.m_Arrow.sprite = m_EnemyArrow;

		yield return new WaitForSeconds(1.5f);
		foreach (var item in spownTreeGenerate.m_Trees)
		{
			icon = Instantiate(m_AnimationIcon, item.transform.position, Quaternion.identity, transform).GetComponent<IconAnimation>();
			icon.SetPPos(m_Player.transform.position);
			icon.SetColor(Color.white);
			icon.m_Arrow.sprite = m_TreeArrow;
			yield return new WaitForSeconds(0.2f);
		}
		yield return new WaitForSeconds(1.5f);
		SceneController.Instance.ChangeScenes(1);

	}
}
