using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour {

    Vector3 velocity;

	// Use this for initialization
	void Start () {
        velocity = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
        velocity.y += Physics.gravity.y * Time.deltaTime;

        transform.Translate(velocity * Time.deltaTime);
    }
}
