using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{

    GameObject Enemy_H;
    public PlayModeData modeData;

    int cost;
    public int Hard;
    public int Normal;
    public int Easy;

    // Use this for initialization
    void Start()
    {
        cost = modeData.cost;

        Enemy_H = GameObject.FindGameObjectWithTag("Enemy");
    }

    // Update is called once per frame
    void Update()
    {
        if (cost == Hard)
        {
            Destroy(Enemy_H.GetComponent<EnemyAI_E>());
            Destroy(Enemy_H.GetComponent<EnemyAI_N>());
        }
        if (cost == Normal)
        {
            Destroy(Enemy_H.GetComponent<EnemyAI_E>());
            Destroy(Enemy_H.GetComponent<EnemyAI4>());
        }
        if (cost == Easy)
        {
            Destroy(Enemy_H.GetComponent<EnemyAI_N>());
            Destroy(Enemy_H.GetComponent<EnemyAI4>());
        }
    }
}
