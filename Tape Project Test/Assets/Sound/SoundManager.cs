using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : SingletonMonoBehaviour<SoundManager> {

	public AudioMixer m_AudioMixer;

	public float MasterVolume
	{
		get { float result; m_AudioMixer.GetFloat("MasterVolume", out result); return Mathf.InverseLerp(-80, 0, result); }
		set { m_AudioMixer.SetFloat("MasterVolume", Mathf.Lerp(-80, 0, value)); }
	}

	public float BGMVolume
	{
		get { float result; m_AudioMixer.GetFloat("BGMVolume", out result); return Mathf.InverseLerp(-80, 0, result); }
		set { m_AudioMixer.SetFloat("BGMVolume", Mathf.Lerp(-80, 0, value)); }
	}

	public float SEVolume
	{
		get { float result; m_AudioMixer.GetFloat("SEVolume", out result); return Mathf.InverseLerp(-80, 0, result); }
		set { m_AudioMixer.SetFloat("SEVolume", Mathf.Lerp(-80, 0, value)); }
	}

	public float SystemVolume
	{
		get { float result; m_AudioMixer.GetFloat("SystemVolume", out result); return Mathf.InverseLerp(-80, 0, result); }
		set { m_AudioMixer.SetFloat("SystemVolume", Mathf.Lerp(-80, 0, value)); }
	}
}
