using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearObj : MonoBehaviour {
    [System.NonSerialized]
    public GameObject m_nearObj0;
    [System.NonSerialized]
    public GameObject m_nearObj;
    [System.NonSerialized]
    public GameObject m_nearObj2;
    [System.NonSerialized]
    public GameObject m_nearObj3;
    [System.NonSerialized]
    public GameObject m_nearObj4;
    [System.NonSerialized]
    public GameObject m_nearObj40;
    [System.NonSerialized]
    public GameObject m_nearObj50;

    [System.NonSerialized]
    public GameObject m_myTreeObj;
    [System.NonSerialized]
    public GameObject m_myTreeObj2;
    [System.NonSerialized]
    public GameObject m_myTreeObj3;

    [System.NonSerialized]
    public GameObject m_myStringObj;
    [System.NonSerialized]
    public GameObject m_stringObj1;
    [System.NonSerialized]
    public GameObject m_stringNet;

    [System.NonSerialized]
    public GameObject m_reObj;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        //今いる木
        if(GetComponent<EnemyAI4>() != null)
        m_nearObj = GetComponent<EnemyAI4>().nearObj;
        if (GetComponent<EnemyAI_E>() != null)
            m_nearObj = GetComponent<EnemyAI_E>().nearObj;
        if (GetComponent<EnemyAI_N>() != null)
            m_nearObj = GetComponent<EnemyAI_N>().nearObj;

        //近かったオブジェクト（木）を取得
        m_nearObj2 = serchTag(this.gameObject, "Tree");
        //2番目のオブジェクト（木）
        m_nearObj3 = serchTag2(this.gameObject, "Tree");
        //3番目のオブジェクト（木）
        m_nearObj4 = RandomSerchTag3(this.gameObject, "Tree");

        //近くの自分の陣地ではない木
        m_nearObj40 = serchTag3(this.gameObject, "Tree");
        //2番目の自分の陣地ではない木
        m_nearObj50 = serchTag4(this.gameObject, "Tree");

        //誰の陣地でもない近くの木
        m_nearObj0 = serchTag0(this.gameObject, "Tree");

        //近くの自分の陣地の木
        m_myTreeObj = MyTreeSerch(this.gameObject, "Tree");

        //2番目に近くの自分の陣地の木
        m_myTreeObj2 = MyTreeSerch2(this.gameObject, "Tree");

        //2番目に近くの自分の陣地の木
        m_myTreeObj3 = MyTreeSerch3(this.gameObject, "Tree");

        //近くの自分の糸
        m_myStringObj = stringTag0(this.gameObject, "String");

        //近くの相手の糸
        m_stringObj1 = stringTag1(this.gameObject, "String");

        //近くの相手のネット
        m_stringNet = stringNet(this.gameObject, "Net");

        //１つ前にいた木を保持
        if (GetComponent<EnemyAI4>() != null)
            m_reObj = GetComponent<EnemyAI4>().reObj;
        if (GetComponent<EnemyAI_E>() != null)
            m_reObj = GetComponent<EnemyAI_E>().reObj;
        if (GetComponent<EnemyAI_N>() != null)
            m_reObj = GetComponent<EnemyAI_N>().reObj;
    }


    //指定されたタグの中で最も近いオブジェクト
    public GameObject serchTag(GameObject nowObj, string tagName)
    {
        //距離用一時変換
        float tmpDis;

        //最も近いオブジェクトの距離
        float nearDis = 0;

        GameObject targetObj = null;

        //タグ指定されたオブジェクトを配列で取得
        foreach (GameObject obs in GameObject.FindGameObjectsWithTag(tagName))
        {
            //触れているオブジェクトを検索から外す
            if (m_nearObj == obs)
            {
                continue;
            }
            if (m_reObj == obs)
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
        //最も近かったオブジェクトを返す
        return targetObj;
    }

    //指定されたタグの中で2番目近いオブジェクト
    public GameObject serchTag2(GameObject nowObj, string tagName)
    {
        //距離用一時変換
        float tmpDis;

        //最も近いオブジェクトの距離
        float nearDis = 0;

        GameObject targetObj = null;

        //タグ指定されたオブジェクトを配列で取得
        foreach (GameObject obs in GameObject.FindGameObjectsWithTag(tagName))
        {
            //触れているオブジェクトを検索から外す
            if (m_nearObj == obs)
            {
                continue;
            }
            else if (m_nearObj2 == obs)
            {
                continue;
            }
            if (m_reObj == obs)
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

    //指定されたタグの中で3番目近いオブジェクト
    public GameObject RandomSerchTag3(GameObject nowObj, string tagName)
    {
        //距離用一時変換
        float tmpDis;

        //最も近いオブジェクトの距離
        float nearDis = 0;

        GameObject targetObj = null;

        //タグ指定されたオブジェクトを配列で取得
        foreach (GameObject obs in GameObject.FindGameObjectsWithTag(tagName))
        {
            //触れているオブジェクトを検索から外す
            if (m_nearObj == obs)
            {
                continue;
            }
            else if (m_nearObj2 == obs)
            {
                continue;
            }
            else if(m_nearObj3 == obs)
            {
                continue;
            }
            if (m_reObj == obs)
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

    //自分の陣地ではない近くの木
    public GameObject serchTag3(GameObject nowObj, string tagName)
    {
        GameObject targetObj = null;
        float tmpDis;
        float nearDis = 0;

        foreach (GameObject obs in GameObject.FindGameObjectsWithTag(tagName))
        {

            if (m_nearObj == obs)
            {
                continue;
            }
            int number = obs.GetComponent<Tree>().m_SideNumber;
            int sidenumber = GetComponent<StringShooter>().m_SideNumber;

            if (number != sidenumber)
            {
                //自身と取得したオブジェクトの距離を取得
                tmpDis = Vector3.Distance(obs.transform.position, nowObj.transform.position);

                if (nearDis == 0 || nearDis > tmpDis)
                {
                    nearDis = tmpDis;
                    targetObj = obs;
                }
            }
        }
        return targetObj;
    }
    //自分の陣地ではない2番目の木
    public GameObject serchTag4(GameObject nowObj, string tagName)
    {
        GameObject targetObj = null;
        float tmpDis;
        float nearDis = 0;

        foreach (GameObject obs in GameObject.FindGameObjectsWithTag(tagName))
        {

            if (m_nearObj == obs)
            {
                continue;
            }
            else if (m_nearObj40 == obs)
            {
                continue;
            }

            int number = obs.GetComponent<Tree>().m_SideNumber;
            int sidenumber = GetComponent<StringShooter>().m_SideNumber;

            if (number != sidenumber)
            {
                //自身と取得したオブジェクトの距離を取得
                tmpDis = Vector3.Distance(obs.transform.position, nowObj.transform.position);

                if (nearDis == 0 || nearDis > tmpDis)
                {
                    nearDis = tmpDis;
                    targetObj = obs;
                }
            }
        }
        return targetObj;
    }


    //誰の陣地でもない近くの木
    public GameObject serchTag0(GameObject nowObj, string tagName)
    {
        GameObject targetObj = null;
        float tmpDis;
        float nearDis = 0;

        foreach (GameObject obs in GameObject.FindGameObjectsWithTag(tagName))
        {

            if (m_nearObj == obs)
            {
                continue;
            }
            int number = obs.GetComponent<Tree>().m_SideNumber;
            int sidenumber = GetComponent<StringShooter>().m_SideNumber;

            if (number == 0)
            {
                //自身と取得したオブジェクトの距離を取得
                tmpDis = Vector3.Distance(obs.transform.position, nowObj.transform.position);

                if (nearDis == 0 || nearDis > tmpDis)
                {
                    nearDis = tmpDis;
                    targetObj = obs;
                }
            }
        }
        return targetObj;
    }


    //近くの自分の陣地の木
    public GameObject MyTreeSerch(GameObject nowObj, string tagName)
    {
        GameObject targetObj = null;
        float tmpDis;
        float nearDis = 0;

        foreach (GameObject obs in GameObject.FindGameObjectsWithTag(tagName))
        {

            if (m_nearObj == obs)
            {
                continue;
            }
            int number = obs.GetComponent<Tree>().m_SideNumber;
            int sidenumber = GetComponent<StringShooter>().m_SideNumber;

            if (number == sidenumber)
            {
                //自身と取得したオブジェクトの距離を取得
                tmpDis = Vector3.Distance(obs.transform.position, nowObj.transform.position);

                if (nearDis == 0 || nearDis > tmpDis)
                {
                    nearDis = tmpDis;
                    targetObj = obs;
                }
            }
        }
        return targetObj;
    }

    //2番目に近くの自分の陣地の木
    public GameObject MyTreeSerch2(GameObject nowObj, string tagName)
    {
        GameObject targetObj = null;
        float tmpDis;
        float nearDis = 0;

        foreach (GameObject obs in GameObject.FindGameObjectsWithTag(tagName))
        {

            if (m_nearObj == obs)
            {
                continue;
            }
            else if(m_myTreeObj == obs)
            {
                continue;
            }

            int number = obs.GetComponent<Tree>().m_SideNumber;
            int sidenumber = GetComponent<StringShooter>().m_SideNumber;

            if (number == sidenumber)
            {
                //自身と取得したオブジェクトの距離を取得
                tmpDis = Vector3.Distance(obs.transform.position, nowObj.transform.position);

                if (nearDis == 0 || nearDis > tmpDis)
                {
                    nearDis = tmpDis;
                    targetObj = obs;
                }
            }
        }
        return targetObj;
    }
    //3番目に近くの自分の陣地の木
    public GameObject MyTreeSerch3(GameObject nowObj, string tagName)
    {
        GameObject targetObj = null;
        float tmpDis;
        float nearDis = 0;

        foreach (GameObject obs in GameObject.FindGameObjectsWithTag(tagName))
        {

            if (m_nearObj == obs)
            {
                continue;
            }
            else if (m_myTreeObj == obs)
            {
                continue;
            }
            else if(m_myTreeObj2 == obs)
            {
                continue;
            }

            int number = obs.GetComponent<Tree>().m_SideNumber;
            int sidenumber = GetComponent<StringShooter>().m_SideNumber;

            if (number == sidenumber)
            {
                //自身と取得したオブジェクトの距離を取得
                tmpDis = Vector3.Distance(obs.transform.position, nowObj.transform.position);

                if (nearDis == 0 || nearDis > tmpDis)
                {
                    nearDis = tmpDis;
                    targetObj = obs;
                }
            }
        }
        return targetObj;
    }


    //近くの自分の糸
    public GameObject stringTag0(GameObject nowObj, string tagName)
    {
        GameObject targetObj = null;
        float tmpDis;
        float nearDis = 0;

        foreach (GameObject obs in GameObject.FindGameObjectsWithTag(tagName))
        {


            int number = obs.GetComponent<StringUnit>().m_SideNumber;
            int sidenumber = GetComponent<StringShooter>().m_SideNumber;

            if (number == sidenumber)
            {
                //自身と取得したオブジェクトの距離を取得
                tmpDis = Vector3.Distance(obs.transform.position, nowObj.transform.position);

                if (nearDis == 0 || nearDis > tmpDis)
                {
                    nearDis = tmpDis;
                    targetObj = obs;
                }
            }
        }
        return targetObj;
    }

    //近くの相手の糸
    public GameObject stringTag1(GameObject nowObj, string tagName)
    {
        GameObject targetObj = null;
        float tmpDis;
        float nearDis = 0;

        foreach (GameObject obs in GameObject.FindGameObjectsWithTag(tagName))
        {


            int number = obs.GetComponent<StringUnit>().m_SideNumber;
            int sidenumber = GetComponent<StringShooter>().m_SideNumber;

            if (number != sidenumber)
            {
                //自身と取得したオブジェクトの距離を取得
                tmpDis = Vector3.Distance(obs.transform.position, nowObj.transform.position);

                if (nearDis == 0 || nearDis > tmpDis)
                {
                    nearDis = tmpDis;
                    targetObj = obs;
                }
            }
        }
        return targetObj;
    }

    //近くの相手のネットを取得
    public GameObject stringNet(GameObject nowObj, string tagName)
    {
        GameObject targetObj = null;
        float tmpDis;
        float nearDis = 0;

        foreach (GameObject obs in GameObject.FindGameObjectsWithTag(tagName))
        {
            int number = obs.GetComponent<Connecter>().m_SideNumber;
            int sidenumber = GetComponent<StringShooter>().m_SideNumber;

            if (number != sidenumber)
            {
                //自身と取得したオブジェクトの距離を取得
                tmpDis = Vector3.Distance(obs.transform.position, nowObj.transform.position);

                if (nearDis == 0 || nearDis > tmpDis)
                {
                    nearDis = tmpDis;
                    targetObj = obs;
                }
            }
        }
        return targetObj;
    }
}
