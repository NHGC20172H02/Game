using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    float fadeSpeed = 0.02f;
    float red, green, blue, alpha;

    public bool isFadeOut = false;
    public bool isFadeIn = false;

    Image fadeImage;
	// Use this for initialization
	void Start ()
    {
        fadeImage = GetComponent<Image>();
        red = fadeImage.color.r;
        green = fadeImage.color.g;
        blue = fadeImage.color.b;
        alpha = fadeImage.color.a;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (isFadeIn)
        {
            StartFadeIn();
        }

        if (isFadeOut)
        {
            StartFadeOut();
        }
    }

    void StartFadeIn()
    {
        alpha -= fadeSpeed;                
        SetAlpha();                      
        if (alpha <= 0)
        {                    
            isFadeIn = false;
            fadeImage.enabled = false;    
        }
    }

    void StartFadeOut()
    {
        fadeImage.enabled = true;  
        alpha += fadeSpeed;         
        SetAlpha();               
        if (alpha >= 1)
        {             
            isFadeOut = false;
        }
    }

    void SetAlpha()
    {
        fadeImage.color = new Color(red, green, blue, alpha);
    }
}
