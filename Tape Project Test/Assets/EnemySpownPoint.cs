using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpownPoint : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
	    if(EnemySpown.enemySpownData == 4)
        {
            Vector3 enemySpownPosition = GameObject.Find("EnemySpownPoint1").transform.position;
            GameObject.Find("Enemy2_H").transform.position = new Vector3(enemySpownPosition.x, enemySpownPosition.y, enemySpownPosition.z);
            var enemyspownRotation = GameObject.Find("EnemySpownPoint1").transform.rotation;
            GameObject.Find("Enemy2_H").transform.rotation = enemyspownRotation;
        }

        if (EnemySpown.enemySpownData == 5)
        {
            Vector3 enemySpownPosition2 = GameObject.Find("EnemySpownPoint2").transform.position;
            GameObject.Find("Enemy2_H").transform.position = new Vector3(enemySpownPosition2.x, enemySpownPosition2.y, enemySpownPosition2.z);
            var enemySpownRotation2 = GameObject.Find("EnemySpownPoint2").transform.rotation;
            GameObject.Find("Enemy2_H").transform.rotation = enemySpownRotation2;
        }

        if (EnemySpown.enemySpownData == 6)
        {
            Vector3 enemySpownPosition3 = GameObject.Find("EnemySpownPoint3").transform.position;
            GameObject.Find("Enemy2_H").transform.position = new Vector3(enemySpownPosition3.x, enemySpownPosition3.y, enemySpownPosition3.z);
            var enemySpownRotation3 = GameObject.Find("EnemySpownPoint3").transform.rotation;
            GameObject.Find("Enemy2_H").transform.rotation = enemySpownRotation3;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
