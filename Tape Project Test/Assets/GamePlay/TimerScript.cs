using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TimerScript : MonoBehaviour {
    [SerializeField]int m_Count;
    Text m_Text;

    // Use this for initialization
    void Start()
    {
        m_Text = this.GetComponent<Text>();
    }

    public void CountUp()
    {
        m_Count--;
        m_Text.text = m_Count.ToString();
    }
    // Update is called once per frame
    void Update()
    {
        CountUp();
        if(m_Count <= 0)
        {
            m_Count = 0;
        }

        //if(m_Count ==0)
        //{
        //    SceneManager.LoadScene("Result");
        //}
    }
}
