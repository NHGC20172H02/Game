using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTreeGenerator : MonoBehaviour
{
	public PlayModeData m_PMD;
	public List<Vector2Int> m_FixedPositions = new List<Vector2Int>() { new Vector2Int(2, 5), new Vector2Int(3, 3), new Vector2Int(5, 4) };
	private List<Vector2Int> m_RandomList;
	private List<Vector2Int> m_IgnoreList;
	private List<Vector2Int> m_Positions;

	private void Start()
	{
		Generat(Random.Range(0, 3) * 2 + 5);
	}
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.LeftControl)) Generat(Random.Range(0, 3) * 2 + 5);
	}
	public void Generat(int count)
	{
		m_RandomList = new List<Vector2Int>();
		m_IgnoreList = new List<Vector2Int>();
		m_Positions = new List<Vector2Int>();

		foreach (var item in m_FixedPositions)
		{
			AddList(item);
		}

		for (int i = count - m_FixedPositions.Count; i > 0; i--)
		{
			AddList(m_RandomList[Random.Range(0, m_RandomList.Count)]);
		}

		m_PMD.m_TreePpositions = m_Positions;
	}

	private void AddList(Vector2Int position)
	{
		m_Positions.Add(position);

		Vector2Int[] offsets = new Vector2Int[5]
		{
			new Vector2Int(0, 0),
			new Vector2Int(1, 0),
			new Vector2Int(-1, 0),
			new Vector2Int(0, 1),
			new Vector2Int(0, -1)
		};
		foreach (var offset in offsets)
		{
			var pos = position + offset;
			if (pos.x < 0 || 8 <= pos.x) continue;
			if (pos.y < 0 || 8 <= pos.y) continue;
			if (m_IgnoreList.Contains(pos)) continue;
			m_IgnoreList.Add(pos);
		}

		offsets = new Vector2Int[12]
		{
			new Vector2Int(-2, 0),
			new Vector2Int(-2, 1),
			new Vector2Int(-2, -1),
			new Vector2Int(-1, 2),
			new Vector2Int(-1, -2),
			new Vector2Int(0, 2),
			new Vector2Int(0, -2),
			new Vector2Int(1, 2),
			new Vector2Int(1, -2),
			new Vector2Int(2, 1),
			new Vector2Int(2, -1),
			new Vector2Int(2, 0)
		};
		foreach (var offset in offsets)
		{
			var pos = position + offset;
			if (pos.x < 0 || 8 <= pos.x) continue;
			if (pos.y < 0 || 8 <= pos.y) continue;
			if (m_IgnoreList.Contains(pos)) continue;
			m_RandomList.Add(pos);
		}

		foreach (var item in m_IgnoreList)
		{
			m_RandomList.RemoveAll(pos => pos == item);
		}
	}
}
