using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashing : MonoBehaviour {

    public CanvasRenderer m_Renderer;
    public AnimationCurve m_AlertInterval;

    private float time = 0;

	public void FlashUpdate () {
        float alpha = m_AlertInterval.Evaluate(time);
        m_Renderer.SetAlpha(alpha);
        time += Time.deltaTime;
	}

    public void TimeReset()
    {
        time = 0;
    }
}
