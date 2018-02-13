using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGenerator : MonoBehaviour {

	public PlayModeData m_PMD;
	public GameObject m_Prefab;
	private List<GameObject> m_Trees;
	// Use this for initialization
	void Start () {
		m_Trees = new List<GameObject>();
		Generate();
	}
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.LeftControl)) {
			Generate();
		}
	}
	private void Generate()
	{
		foreach (var item in m_Trees)
		{
			Destroy(item);
		}
		m_Trees.Clear();
		foreach (var item in m_PMD.m_TreePpositions)
		{
            m_Trees.Add(Instantiate(m_Prefab, new Vector3(item.x * 25 - 87.5f, 0, item.y * 25 - 87.5f), Quaternion.identity, transform));
        }
	}
}
