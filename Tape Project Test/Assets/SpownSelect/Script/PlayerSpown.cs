using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpown : MonoBehaviour
{
    public static int spownData;
    public int num;

    public void OnClick()
    {
        spownData = num;
        SceneController.Instance.ChangeScenes(1);
    }
}
