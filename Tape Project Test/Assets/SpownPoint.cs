using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpownPoint : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        //PlayerSpownで決めた座標をバトルシーンの座標に変換
        Vector3 spownPosition = new Vector3 (PlayerSpown.spownPos.x * 1.29f, 0 ,PlayerSpown.spownPos.y * 1.17f);
        GameObject.Find("Player").transform.position = spownPosition;
    }
}
