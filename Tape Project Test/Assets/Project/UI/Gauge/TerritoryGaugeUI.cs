using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerritoryGaugeUI : MonoBehaviour
{
	public Image m_GaugeIn;
	public Image m_GaugeOver;

	public Sprite m_GaugeBlue;
	public Sprite m_GaugeBlueOver;
	public Sprite m_GaugeRed;
	public Sprite m_GaugeRedOver;

	public void SetTree(Tree tree)
	{
		if (tree == null) return;
		float rate = tree.m_TerritoryRate;
		if (rate >= 0)
		{
			m_GaugeIn.sprite = m_GaugeBlue;
			m_GaugeOver.sprite = m_GaugeBlueOver;
		}
		else
		{
			m_GaugeIn.sprite = m_GaugeRed;
			m_GaugeOver.sprite = m_GaugeRedOver;
		}
		float fillAmount = Mathf.Abs(rate) * 0.01f;// 0~100 -> 0~1
		m_GaugeIn.fillAmount = fillAmount;
		m_GaugeOver.fillAmount = fillAmount - 1;
	}
}
