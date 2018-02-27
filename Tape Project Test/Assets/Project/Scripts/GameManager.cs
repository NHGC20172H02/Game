using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager> {

	[SerializeField]
	private bool m_CursorVisible = false;
	[SerializeField]
	private CursorLockMode m_CursorLockMode = CursorLockMode.Locked;

	[SerializeField]
	private bool m_AFKActive = true;
	[SerializeField]
	private float m_AFKTime = 60;
	private float m_Time;

	[SerializeField]
	private bool m_MultiSceneHotKeyActive = true;

	override protected void Awake()
	{
		base.Awake();
		if (CheckInstance()) DontDestroyOnLoad(gameObject);
	}

	// Use this for initialization
	void Start () {
		m_Time = 0;
		Cursor.visible = m_CursorVisible;
		Cursor.lockState = m_CursorLockMode;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)||Input.GetButtonDown("Start")&&Input.GetButtonDown("Back")) Exit();
		HotKey();
		AFK();
	}

	public void Exit()
	{
		Application.Quit();
	}

	private void HotKey()
	{
		if (m_MultiSceneHotKeyActive) return;
		if (Input.GetKeyDown(KeyCode.Alpha0)) SceneController.Instance.ChangeScenes(0);
		if (Input.GetKeyDown(KeyCode.Alpha1)) SceneController.Instance.ChangeScenes(1);
		if (Input.GetKeyDown(KeyCode.Alpha2)) SceneController.Instance.ChangeScenes(2);
		if (Input.GetKeyDown(KeyCode.Alpha3)) SceneController.Instance.ChangeScenes(3);
		if (Input.GetKeyDown(KeyCode.Alpha4)) SceneController.Instance.ChangeScenes(4);
		if (Input.GetKeyDown(KeyCode.Alpha5)) SceneController.Instance.ChangeScenes(5);
		if (Input.GetKeyDown(KeyCode.Alpha6)) SceneController.Instance.ChangeScenes(6);
		if (Input.GetKeyDown(KeyCode.Alpha7)) SceneController.Instance.ChangeScenes(7);
		if (Input.GetKeyDown(KeyCode.Alpha8)) SceneController.Instance.ChangeScenes(8);
		if (Input.GetKeyDown(KeyCode.Alpha9)) SceneController.Instance.ChangeScenes(9);
	}
	private void AFK()
	{
		if (m_AFKActive) return;
		m_Time += Time.deltaTime;
		if (Input.anyKey) m_Time = 0;
		if (m_Time > m_AFKTime && SceneController.instance.GetActiveScenesIndex() != 0) SceneController.Instance.ChangeScenes(0);
	}
}
