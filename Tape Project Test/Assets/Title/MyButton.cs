using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MyButton : MonoBehaviour {

    public void OnClick()
    {
        //ログに表示
        Debug.Log("Button click");

        //非表示にする
        //gameObject.SetActive(false);

        //Button2を表示
        //MyCanvas.SetActive("Button2", true);

        //シーン移動
        SceneManager.LoadScene("Scene");
    }

    // Use this for initialization
    void Start ()
    {
        MyCanvas.SetActive("Button",false);

    }
	
	// Update is called once per frame
	void Update ()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    MyCanvas.SetActive("Button2", true);
        //}

        if (Input.GetKeyDown(KeyCode.Space))
        {
            MyCanvas.SetActive("Button", true);
            Debug.Log("Xbutton Down");
        }

        
    }
}
