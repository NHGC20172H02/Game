using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTest : MonoBehaviour {
    public int s;

	// Use this for initialization
	void Start () {
        GetComponent<EnemySpawn>().SpaenEnemy(s);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
