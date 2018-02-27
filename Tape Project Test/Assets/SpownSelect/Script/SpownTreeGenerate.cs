using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpownTreeGenerate : MonoBehaviour
{
    public PlayModeData m_PMD;
    public GameObject m_Prefab;
    public List<GameObject> m_Trees;

    // Use this for initialization
    void Start ()
    {
        m_Trees = new List<GameObject>();
        Generate();
    }

    private void Generate()
    {
        foreach (var treeSpown in m_PMD.m_TreePpositions)
        {
			var tree = Instantiate(m_Prefab, new Vector2(0, 0), Quaternion.identity, transform);
			tree.GetComponent<RectTransform>().localPosition = new Vector2((treeSpown.x-3) * 46 - 23.0f, (treeSpown.y-3) * 46 - 23.0f);
			m_Trees.Add(tree);
            //m_Trees.Add(Instantiate(m_Prefab, new Vector3(treeSpown.x * 25 - 87.5f, 0, treeSpown.y * 25 - 87.5f), Quaternion.identity, transform));
        }
    }
}
