using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpownSelect : MonoBehaviour
{

    public AudioSource audioSource;
    GameObject lastSelectedGameObject = null;

    // Use this for initialization
    void Start ()
    {
        audioSource = gameObject.GetComponent<AudioSource>();

        
    }
	
	// Update is called once per frame
	void Update ()
    {

        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;

        if (currentSelected != lastSelectedGameObject && lastSelectedGameObject != null)
        {
            audioSource.Play();
        }
        lastSelectedGameObject = currentSelected;
    }
}
