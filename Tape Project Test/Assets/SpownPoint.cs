using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpownPoint : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        if(PlayerSpown.spownData == 1)
        {
            Vector3 spownPosition = GameObject.Find("SpownPoint1").transform.position;
            GameObject.Find("PlayerCamera").transform.position = new Vector3(spownPosition.x, spownPosition.y, spownPosition.z);
            var spownRotation = GameObject.Find("SpownPoint1").transform.rotation;
            GameObject.Find("PlayerCamera").transform.rotation = spownRotation;
        }

        if (PlayerSpown.spownData == 2)
        {
            Vector3 spownPosition2 = GameObject.Find("SpownPoint2").transform.position;
            GameObject.Find("PlayerCamera").transform.position = new Vector3(spownPosition2.x, spownPosition2.y, spownPosition2.z);
            var spownRotation2 = GameObject.Find("SpownPoint2").transform.rotation;
            GameObject.Find("PlayerCamera").transform.rotation = spownRotation2;
        }

        if (PlayerSpown.spownData == 3)
        {
            Vector3 spownPosition3 = GameObject.Find("SpownPoint3").transform.position;
            GameObject.Find("PlayerCamera").transform.position = new Vector3(spownPosition3.x, spownPosition3.y, spownPosition3.z);
        }
    }
}
