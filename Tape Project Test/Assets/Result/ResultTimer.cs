using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultTimer : MonoBehaviour {

    [SerializeField] int r_Count;
    Text r_Text;

    // Use this for initialization
    void Start()
    {
        r_Text = this.GetComponent<Text>();
    }

    public void CountUp()
    {
        r_Count--;
        r_Text.text = r_Count.ToString();
    }
    // Update is called once per frame
    void Update()
    {
        CountUp();
        if (r_Count <= 0)
        {
            r_Count = 0;
        }

        if (r_Count == 0 || Input.GetKeyDown(KeyCode.T))
        {
			SceneController.Instance.ChangeScenes(0);
		}
	}
}
