using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpowns : MonoBehaviour
{
    Animator animator_;

    float animTime;

	// Use this for initialization
	void Start ()
    {
        animator_ = GetComponent<Animator>();

        animator_.SetBool("Enemy", true);
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(Input.GetButtonDown("Fire2"))
        {
            animator_.SetBool("Enemy", false);
        }
	}
}
