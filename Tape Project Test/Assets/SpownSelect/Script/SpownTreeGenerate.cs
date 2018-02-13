using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpownTreeGenerate : MonoBehaviour
{
    public PlayModeData m_PMD;
    public GameObject m_Prefab;
    private List<GameObject> m_Trees;

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
            m_Trees.Add(Instantiate(m_Prefab, new Vector3(treeSpown.x * 50 + 300.0f,treeSpown.y * 50 + 100,0f), Quaternion.identity, transform));
        }
    }
}
