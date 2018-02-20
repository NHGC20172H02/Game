using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Button25m : MonoBehaviour {

    public void OnClick()
    {
        //ログに表示
        Debug.Log("Button25m click");

		//シーン移動
		SceneController.Instance.ChangeScenes(3);
    }

    // Use this for initialization
    void Start ()
    {
        gameObject.SetActive(false);

    }
	
	// Update is called once per frame
	void Update ()
    {

    }
}
