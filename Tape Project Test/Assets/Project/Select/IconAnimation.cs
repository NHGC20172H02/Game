using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconAnimation : MonoBehaviour {

	public Image m_Circle;
	public Image m_Line;
	public Image m_Arrow;

	public void SetPPos( Vector3 position)
	{
		transform.rotation = Quaternion.FromToRotation(Vector3.up, (position - transform.position).normalized);
		float f = Vector3.Distance(position, transform.position) - 0;
		m_Arrow.transform.position = position;
		m_Arrow.transform.localPosition += Vector3.down * (32 + 7);
		m_Line.rectTransform.sizeDelta = new Vector2(5, Vector3.Distance(m_Arrow.transform.localPosition, m_Line.transform.localPosition));
	}
	public void SetColor(Color color)
	{
		m_Circle.color = color;
		m_Line.color = color;
	}
}
