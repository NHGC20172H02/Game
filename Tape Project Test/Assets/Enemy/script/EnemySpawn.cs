using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour {

    public GameObject Enemy_E;
    public GameObject Enemy_N;
    public GameObject Enemy_H;

    GameObject m_EnemyAI;
    GameObject spawnEnemy;

    int s;
    bool isStarted = false;

    // Use this for initialization
    void Start () {
        isStarted = true;
        DontDestroyOnLoad(this);

        m_EnemyAI = SpaenEnemy(s);

        Instantiate(m_EnemyAI, this.transform.position, Quaternion.identity);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public GameObject SpaenEnemy(int spawn)
    {
        if(spawn == 1)
        {
            spawnEnemy = Enemy_H;
        }
        if(spawn == 2)
        {
            spawnEnemy = Enemy_N;
        }
        if(spawn == 3)
        {
            spawnEnemy = Enemy_E;
        }

        return spawnEnemy;
    }

    private void OnLevelWasLoaded(int level)
    {
        if (isStarted)
        {
            Destroy(this.gameObject);
        }
    }
}
