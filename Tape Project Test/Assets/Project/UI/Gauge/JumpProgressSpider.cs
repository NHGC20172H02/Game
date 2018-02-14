using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpProgressSpider : MonoBehaviour {

	public AnimationCurve m_XCurve = new AnimationCurve();
	public AnimationCurve m_YCurve = new AnimationCurve();
	private Vector3 localposition;

	private void Start()
	{
		localposition = transform.localPosition;
	}
	public void SetProgress(float f)
	{
		var pos = transform.localPosition;
		pos.x = m_XCurve.Evaluate(f);
		pos.y = m_YCurve.Evaluate(f);
		transform.localPosition = pos + localposition;
	}
}
