using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleGaugeImage : MonoBehaviour
{
    public float circlegauge = 10.0f;

    Image circlegaugeImage;

    [SerializeField] float value = 0;

    // Use this for initialization
    void Start ()
    {
		circlegaugeImage = this.GetComponent<Image>();
        
    }
	
	// Update is called once per frame
	void Update ()
    {
		if(Input.GetKeyDown(KeyCode.A))
        {
            circlegaugeImage.fillAmount += (circlegauge / 150.0f);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            circlegaugeImage.fillAmount -= (circlegauge / 150.0f);
        }

        clamp();
    }

    void clamp()
    {
        if (value >= 1) { value = 1; }
        if (value <= 0) { value = 0; }
    }

    public float getAmount()
    {
        return circlegaugeImage.fillAmount;
    }
}
