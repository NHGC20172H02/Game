using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gerge : MonoBehaviour {

    public float gerge;
    public float maxgerge;

    Image gaugeImage;

    float value = 0;

	// Use this for initialization
	void Start ()
    {
        gaugeImage = this.GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        value += Input.GetAxis("Horizontal") * 1f * Time.deltaTime;
        value = Mathf.Clamp01(value);
        gaugeImage.fillAmount = value;
	}
}
