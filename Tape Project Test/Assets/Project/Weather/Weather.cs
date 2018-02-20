using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weather : MonoBehaviour {
	public enum type
	{
		hare,
		kumori,
		raiu,
		ame,
		kiri,
		yuuyake
	}

	public type m_Type;
	// Use this for initialization
	IEnumerator Start () {

		while (true)
		{
			yield return new WaitForSeconds(1);
			switch (m_Type)
			{
				case type.hare:
					break;
				case type.kumori:
					break;
				case type.raiu:
					if (Random.Range(0.0f, 1.0f) > 0.9)
					{
						if (TerritoryManager.Instance.m_Strings.Count > 2)
							TerritoryManager.Instance.m_Strings[Random.Range(0, TerritoryManager.Instance.m_Strings.Count - 1)].Delete();
					}
					if (Random.Range(0.0f, 1.0f) > 0.95)
					{
						TerritoryManager.Instance.m_Trees[Random.Range(0, TerritoryManager.Instance.m_Trees.Count - 1)].m_TerritoryRate = 0;
					}
					break;
				case type.ame:
					if (Random.Range(0.0f, 1.0f) > 0.9)
					{
						if (TerritoryManager.Instance.m_Strings.Count > 2)
							TerritoryManager.Instance.m_Strings[Random.Range(0, TerritoryManager.Instance.m_Strings.Count - 1)].Delete();
					}
					break;
				case type.kiri:
					break;
				case type.yuuyake:
					break;
				default:
					break;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	}
}
