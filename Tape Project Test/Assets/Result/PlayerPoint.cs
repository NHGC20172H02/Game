using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPoint : MonoBehaviour {

    Text pointText;
	public Text pointTextB;

	// Use this for initialization
	void Start ()
    {
        pointText = this.GetComponent<Text>();

        pointText.text = Sample.getscore().ToString();
		pointTextB.text = Sample.scoreB.ToString();
	}

	// Update is called once per frame
	void Update ()
    {
        
    }
}
