using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// シーン操作
public class SceneController : SingletonMonoBehaviour<SceneController>
{
	[SerializeField]
	private SceneListTable m_SceneListTable;
	[SerializeField]
	private string m_LoadingSceneName = "Loading";

	private List<string> m_LoadedScenes = new List<string>();

	private AsyncOperation m_Async;
	private Scene m_Core;

	private int m_Index = 0;

	[HideInInspector]
	public float m_LoadingProgress;
	[HideInInspector]
	public bool m_IsLoading;
	[HideInInspector]
	public bool m_IsLoadingAnimation;

	override protected void Awake()
	{
		base.Awake();
		if (CheckInstance()) DontDestroyOnLoad(gameObject);
	}

#if UNITY_EDITOR
	void Start(){
#else
	IEnumerator Start()
	{
		var startScene = SceneManager.GetSceneByBuildIndex(0);
		foreach (var scene in m_SceneListTable.m_Table[0].Scenes)
		{
			if(startScene != SceneManager.GetSceneByName(scene))
			{
				yield return SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
			}
		}
#endif

		m_IsLoading = false;
		for (int i = 0; i < SceneManager.sceneCount; i++)
		{
			m_LoadedScenes.Add(SceneManager.GetSceneAt(i).name);
		}
	}

	// マルチシーン切り替え
	public void ChangeScenes(int i)
	{
		if (m_SceneListTable.m_Table.Count > i) StartCoroutine(LoadScene(i));
	}

	// マルチシーンアンロードとロード
	private IEnumerator LoadScene(int i)
	{
		if (m_IsLoading) yield break;
		m_IsLoading = true;
		m_IsLoadingAnimation = true;
		m_LoadingProgress = 0;

		// ローディングシーンのロード
		m_Async = SceneManager.LoadSceneAsync(m_LoadingSceneName, LoadSceneMode.Additive);
		yield return new WaitUntil(() => m_Async.isDone);

		yield return new WaitUntil(() => !m_IsLoadingAnimation);

		SceneManager.SetActiveScene(SceneManager.GetSceneByName(m_LoadingSceneName));
		int progresscount = m_LoadedScenes.Count + m_SceneListTable.m_Table[i].Scenes.Count;
		int count = 0;

		//アンロード
		foreach (var scene in m_LoadedScenes)
		{
			m_Async = SceneManager.UnloadSceneAsync(scene);
			while (!m_Async.isDone)
			{
				m_LoadingProgress = (m_Async.progress + count) / progresscount;
				yield return null;
			}
			count++;
		}
		m_LoadedScenes.Clear();

		m_Index = i;

		List<AsyncOperation> asyncs = new List<AsyncOperation>();
		//ロード
		foreach (var scene in m_SceneListTable.m_Table[i].Scenes)
		{
			m_Async = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
			m_Async.allowSceneActivation = false;
			while (m_Async.progress<0.9f)
			{
				m_LoadingProgress = (m_Async.progress + count) / progresscount;
				yield return null;
			}
			m_LoadedScenes.Add(scene);
			asyncs.Add(m_Async);
			count++;
		}
		m_LoadingProgress = 1;
		yield return new WaitForSeconds(0.2f);
		foreach (var async in asyncs)
		{
			async.allowSceneActivation = true;
		}
		m_LoadingProgress = 1;
		m_Async = asyncs[0];
		yield return new WaitUntil(() => m_Async.isDone);

		SceneManager.SetActiveScene(SceneManager.GetSceneByName(m_SceneListTable.m_Table[i].Scenes[0]));
		m_IsLoadingAnimation = true;

		yield return new WaitForSeconds(0.2f);
		yield return new WaitUntil(() => !m_IsLoadingAnimation);
		SceneManager.SetActiveScene(SceneManager.GetSceneByName(m_SceneListTable.m_Table[i].Scenes[0]));
		// ローディングシーンのアンロード
		m_Async = SceneManager.UnloadSceneAsync(m_LoadingSceneName);
		yield return new WaitUntil(() => m_Async.isDone);

		m_LoadingProgress = 0;
		m_IsLoading = false;
	}

	// シーン追加
	public AsyncOperation AddScene(string name)
	{
		m_LoadedScenes.Add(name);
		return SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
	}

	// シーン破棄
	public AsyncOperation RemoveScene(string name)
	{
		m_LoadedScenes.Remove(name);
		return SceneManager.UnloadSceneAsync(name);
	}

	public int GetActiveScenesIndex()
	{
		return m_Index;
	}
}
