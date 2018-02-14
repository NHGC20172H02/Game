using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimation : MonoBehaviour {

	void Start () {
		GetComponent<Animation>().Play();
	}

}
