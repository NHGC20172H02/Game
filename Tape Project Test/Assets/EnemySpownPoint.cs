using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpownPoint : MonoBehaviour
{ 

    // Use this for initialization
    void Start ()
    {
        Vector3 spownPosition2 = new Vector3(Enemyspown.spownPos2.x * 1.29f, 0, Enemyspown.spownPos2.y * 1.17f);
        GameObject.Find("Enemy4").transform.position = spownPosition2;
    }
}
