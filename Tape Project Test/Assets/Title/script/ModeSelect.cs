using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ModeSelect : MonoBehaviour
{
    public static int ModeData;
    public int num;

    // Use this for initialization
    void Start ()
    {
        gameObject.SetActive(false);
       
    }

    void Update()
    {

    }

    public void OnClick()
    {
        ModeData = num;
        SceneController.Instance.ChangeScenes(3);
	}
}
