using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleBGM : MonoBehaviour
{
    public AudioClip titleSound;
    private AudioSource titleSource;

	// Use this for initialization
	void Start ()
    {
        titleSource = gameObject.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        //if(Input.GetButtonDown("Fire2"))
        //{
        //    titleSource.PlayOneShot(titleSound);
        //}
        
	}
}
