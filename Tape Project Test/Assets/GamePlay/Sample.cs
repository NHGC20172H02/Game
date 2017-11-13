using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Sample : MonoBehaviour
{

    public static int score = 0;

    public static int getscore()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            score += 1;
        }
        return score;
    }

    public void Initialize()
    {
        score = 0;
    }
    // Use this for initialization
    void Start ()
    {
        
	}
	
	// Update is called once per frame
	void Update ()
    {
        Debug.Log("PlayerPoints" + getscore());

        if(Input.GetKeyDown(KeyCode.S))
        {
            SceneManager.LoadScene("Result");
        }
	}
}
