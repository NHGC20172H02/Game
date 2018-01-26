using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StringStock : MonoBehaviour {

	public Image m_Image;
	public Sprite m_PlayerImage;
	public Sprite m_EnemyImage;
	public float m_Width;

	public float m_Height;
	public float m_WidthMAX;
	// Use this for initialization
	IEnumerator Start () {
		for (float timer = 0; timer < 1.0f; timer += Time.deltaTime)
		{
			m_Image.rectTransform.sizeDelta = new Vector2(m_Width*timer, m_Height);
			yield return null;
		}
		transform.localScale = Vector3.one;
	}

	public void Delete(int side)
	{
		m_Image.sprite = side == 1 ? m_PlayerImage : m_EnemyImage;
		StartCoroutine(Delete());
	}
	
	IEnumerator Delete()
	{
		for (float timer = 1; timer > 0.0f; timer-=Time.deltaTime)
		{
			m_Image.rectTransform.sizeDelta = new Vector2(m_Width*timer, m_Height);
			yield return null;
		}
		Destroy(gameObject);
	}

	public void SetCost(float cost)
	{
		m_Width = cost * 0.001f * m_WidthMAX;
	}
}
