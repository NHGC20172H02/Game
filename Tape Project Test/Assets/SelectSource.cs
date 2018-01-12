using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectSource : MonoBehaviour
{
    private AudioSource selectSource;

	// Use this for initialization
	void Start ()
    {
        selectSource = gameObject.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
