using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCNearObj : MonoBehaviour {
    [System.NonSerialized]
    public GameObject m_nearObj2;
    [System.NonSerialized]
    public GameObject m_nearObj3;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //近かったオブジェクト（木）を取得
        m_nearObj2 = RandomSerchTag(this.gameObject, "Tree");
        //2番目のオブジェクト（木）
        m_nearObj3 = RandomSerchTag2(this.gameObject, "Tree");
    }


    //指定されたタグの中で最も近いオブジェクト
    public GameObject RandomSerchTag(GameObject nowObj, string tagName)
    {
        //距離用一時変換
        float tmpDis;

        //最も近いオブジェクトの距離
        float nearDis = 0;

        GameObject targetObj = null;

        //タグ指定されたオブジェクトを配列で取得
        foreach (GameObject obs in GameObject.FindGameObjectsWithTag(tagName))
        {
            //自身と取得したオブジェクトの距離を取得
            tmpDis = Vector3.Distance(obs.transform.position, nowObj.transform.position);

            //オブジェクトの距離が近く、距離0であればオブジェクト名を取得
            if (nearDis == 0 || nearDis > tmpDis)
            {
                nearDis = tmpDis;
                targetObj = obs;
            }
        }
        //最も近かったオブジェクトを返す
        return targetObj;
    }

    //指定されたタグの中で2番目近いオブジェクト
    public GameObject RandomSerchTag2(GameObject nowObj, string tagName)
    {
        //距離用一時変換
        float tmpDis;

        //最も近いオブジェクトの距離
        float nearDis = 0;

        GameObject targetObj = null;

        //タグ指定されたオブジェクトを配列で取得
        foreach (GameObject obs in GameObject.FindGameObjectsWithTag(tagName))
        { 
            if (m_nearObj2 == obs)
            {
                continue;
            }
            //自身と取得したオブジェクトの距離を取得
            tmpDis = Vector3.Distance(obs.transform.position, nowObj.transform.position);

            //オブジェクトの距離が近く、距離0であればオブジェクト名を取得
            if (nearDis == 0 || nearDis > tmpDis)
            {
                nearDis = tmpDis;
                targetObj = obs;
            }

        }
        //3番目近かったオブジェクトを返す
        return targetObj;
    }
}
