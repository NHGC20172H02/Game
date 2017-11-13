using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyButton2 : MonoBehaviour {

    public void OnClick()
    {
        Debug.Log("Button2 click");
        //非表示にする
        //gameObject.SetActive(false);
        
        //Buttonを表示
        //MyCanvas.SetActive("Button", true);
        
    }

    // Use this for initialization
    void Start ()
    {
        gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update ()
    {
		//if(Input.GetKeyDown(KeyCode.Space))
  //      {
  //          gameObject.SetActive(true);
  //      }
	}
    
}
