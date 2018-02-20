using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpown : MonoBehaviour
{
    public static Vector2 spownPos2;

    private Vector2 enemyPos;

    GameObject[] trees;

    RectTransform m_RectTransform;

    float dis = 0.0f;

    float l_dis;

    Vector2 randomPos;

    float ramX;
    float ramY;

    // Use this for initialization
    void Start ()
    {
        m_RectTransform = GetComponent<RectTransform>();

        //ランダムの変数
        ramX = Random.Range(112.0f, 158.0f);
        ramY = Random.Range(-190.0f, 137.0f);
    }
	
	// Update is called once per frame
	void Update ()
    {
        //オブジェクトの座標
        enemyPos = gameObject.transform.position;
        //ランダムの座標
        randomPos = new Vector2(ramX, ramY);

        //******************敵と一番近い木を探す処理*****************

        //ツリータグを探す
        trees = GameObject.FindGameObjectsWithTag("SpownSelectTree");

        //エネミーに一番近いツリーのタグを探す
        for (int i = 0; i < trees.Length; i++)
        {
            l_dis = Vector2.Distance(enemyPos, trees[i].transform.position);

            if (dis == 0.0f || dis > l_dis)
            {
                dis = l_dis;
            }
        }
        //***********************************************************


        //*********敵の元の座標の近くに木があったらその近くにポジションを設定、
        //なければランダム（G、Hエリア内）にポジションを設定*********
        
        if(dis <= 60.0f)
        {
            randomPos = new Vector2(randomPos.x - 50.0f, randomPos.y + 50.0f);
        }

        enemyPos = randomPos;

        //ツリーと敵の距離がdis以下だったらその近くに座標を移動
        
        //************************************************************
        m_RectTransform.anchoredPosition = enemyPos;

        //********スポーンセレクトシーンで決めた座標をゲームプレイシーンの敵の座標に反映する**********
        
        spownPos2 = m_RectTransform.anchoredPosition;
        //********************************************************
    }
}
