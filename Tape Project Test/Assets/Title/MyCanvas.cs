using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCanvas : MonoBehaviour {

    static Canvas _canvas;
    
    // Use this for initialization
    void Start ()
    {
        //Canvasコンポーネントを持つ
        _canvas = GetComponent<Canvas>();
        
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    //表示・非表示を設定する
    public static void SetActive(string name, bool b)
    {
        foreach (Transform child in _canvas.transform)
        {
            //子の要素をたどる
            if (child.name == name)
            {
                //指定した名前と一致
                //表示フラグを設定
                child.gameObject.SetActive(b);
                return;
            }
        }
        //指定したオブジェクトが見つからない
        Debug.LogWarning("Not found objectname:" + name);
    }

    
}
