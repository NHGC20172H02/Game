using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationCamera : MonoBehaviour {

    public float m_Hight;
    public Transform target;
    public float speed = 10.0f;

	// Use this for initialization
	void Start ()
    {
        m_Hight = this.gameObject.transform.position.y;
	}
	
	// Update is called once per frame
	void Update ()
    {

        Vector3 axis = transform.TransformDirection(Vector3.down);
        transform.RotateAround(target.position, axis, speed * Time.deltaTime / 2);
	}
}
