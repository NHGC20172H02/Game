using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerritoryGaugeUI : MonoBehaviour
{
	public Image m_GaugeIn;

	public Sprite m_GaugeBlue;
	public Sprite m_GaugeRed;

	public Animator m_Animator;

	public void SetTree(Tree tree)
	{
		if (tree == null) return;
		float rate = tree.m_TerritoryRate;
		if (rate >= 0)
		{
			m_GaugeIn.sprite = m_GaugeBlue;
		}
		else
		{
			m_GaugeIn.sprite = m_GaugeRed;
		}
		float fillAmount = Mathf.Abs(rate) * 0.01f;// 0~100 -> 0~1
		m_Animator.SetBool("Active", m_GaugeIn.fillAmount != fillAmount);
		m_GaugeIn.fillAmount = fillAmount;
	}
}
