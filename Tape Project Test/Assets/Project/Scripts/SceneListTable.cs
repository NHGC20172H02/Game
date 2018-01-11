using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.SerializableAttribute]
public class SceneList
{
	public List<string> Scenes = new List<string>();
	public SceneList(List<string> list)
	{
		Scenes = list;
	}
}

public class SceneListTable : ScriptableObject
{
	[SerializeField]
	public List<SceneList> m_Table = new List<SceneList>();

}
