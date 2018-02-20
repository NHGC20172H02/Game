using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpownPoint : MonoBehaviour
{ 

    // Use this for initialization
    void Start ()
    {
        Vector3 spownPosition2 = new Vector3(EnemySpown.spownPos2.x * 0.5f, 0, EnemySpown.spownPos2.y * 0.4f);
        GameObject.Find("Enemy4").transform.position = spownPosition2;
    }
}
