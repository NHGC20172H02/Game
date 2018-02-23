using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseObject : MonoBehaviour {

	public List<MonoBehaviour> m_Scripts;
	public List<Animator> m_Animator;
    public List<ParticleSystem> m_Particles;

    private Character m_character = null;

	void Start () {
		PauseManager.Instance.AddObject(this);
	}

	public void Pause(bool pause)
	{
		foreach (var item in m_Scripts)
		{
            if (item != null)
            {
                item.enabled = pause;
                if (item.tag == "Player" || item.tag == "Enemy")
                {
                    m_character = item.GetComponent<Character>();
                }
            }
        }
		foreach (var item in m_Animator)
		{
            if(item != null)
            {
                item.enabled = pause;
            }
        }
        foreach(var item in m_Particles)
        {
            if(item != null)
            {
                if (pause && item.isPaused)
                {
                    item.Play();
                }
                else if (item.isPlaying)
                {
                    item.Pause();
                }
            }
        }
        if(m_character != null)
        {
            if (m_character.GetReceiveBodyblow == null) return;
            if (pause)
                m_character.StartCoroutine(m_character.GetReceiveBodyblow);
            else
                m_character.StopCoroutine(m_character.GetReceiveBodyblow);
        }
	}

}
