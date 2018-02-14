﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpownPoint : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        //PlayerSpownで決めた座標をバトルシーンの座標に変換
        Vector3 spownPosition = new Vector3 (PlayerSpown.spownPos.x * 0.5f, 0 ,PlayerSpown.spownPos.y * 0.6f);
        GameObject.Find("PlayerCamera").transform.position = spownPosition;

        var playerRotate = this.gameObject.transform.eulerAngles;
        GameObject.Find("PlayerCamera").transform.eulerAngles = playerRotate;

    }
}