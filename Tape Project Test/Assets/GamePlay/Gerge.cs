using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gerge : MonoBehaviour {

    public float gerge;
    public float goukeigerge;

    public Image gaugeImageA;
    public Image gaugeImageB;

    float value = 0;

    

	// Use this for initialization
	void Start ()
    {
        
	}
	
	// Update is called once per frame
	void Update ()
    {
        //value += Input.GetAxis("Horizontal") * 1f * Time.deltaTime;
        value = Sample.getscore() / goukeigerge;
        value = Sample.scoreB / goukeigerge;
       
        //value = Sample.getscore();
        value = Mathf.Clamp01(value);
        gaugeImageA.fillAmount = 1 - value;
        gaugeImageB.fillAmount = value;

        
    }
}
