﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PressAnyKeyAnimation : MonoBehaviour {

    Animator animator_;

    float animationTimer = 0.0f;

	// Use this for initialization
	void Start ()
    {
        animator_ = GetComponent<Animator>();

        animator_.SetBool("PressButtonAnimation", false);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            animationTimer++;

            animator_.SetBool("PressButtonAnimation", true);
            
            if(animationTimer > 10)
            {
                animator_.SetBool("PressButtonAnimation", false);
                animationTimer = 0;
            }
        }


    }
}
