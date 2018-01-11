using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button100m : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        Debug.Log("Button100m click");

        gameObject.SetActive(false);

    }
	
	// Update is called once per frame
	void Update ()
    {
        
    }

    public void OnClick()
    {
		SceneController.Instance.ChangeScenes(1);

	}
}
