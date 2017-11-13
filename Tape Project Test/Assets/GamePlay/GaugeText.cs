using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaugeText : MonoBehaviour
{
    Text m_Text;
    float m_Amount = 0;
    public CircleGaugeImage m_Gauge;

	// Use this for initialization
	void Start ()
    {
        m_Text = this.GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        m_Amount = m_Gauge.getAmount();
        m_Amount = m_Amount * 100;
        m_Amount = Mathf.Ceil(m_Amount);
        m_Text.text = m_Amount.ToString();
	}
}
