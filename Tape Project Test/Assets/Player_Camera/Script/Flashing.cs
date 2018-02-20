using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashing : MonoBehaviour {

    public CanvasRenderer m_Renderer;
    public float m_Interval = 1f;

    private float m_nextTime;

	void Start () {
        m_nextTime = Time.time;
	}
	
	public void FlashUpdate () {
		if(Time.time > m_nextTime)
        {
            float alpha = m_Renderer.GetAlpha();

            if(alpha == 1f)
                m_Renderer.SetAlpha(0);
            else
                m_Renderer.SetAlpha(1f);

            m_nextTime += m_Interval;
        }
	}
}
