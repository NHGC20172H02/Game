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

    //Vector2 lefttop = new Vector2(120.0f, 150.0f);
    //Vector2 rightbottom = new Vector2(170.0f,-200.0f);


    float ramX;
    float ramY;

    // Use this for initialization
    void Start ()
    {
        //この座標をゲームプレイの初期スポーンにする
        //spownPos2 = gameObject.transform.position;

        m_RectTransform = GetComponent<RectTransform>();

        //ランダムの変数
        ramX = Random.Range(112.0f, 158.0f);
        ramY = Random.Range(-190.0f, 137.0f);
    }
	
	// Update is called once per frame
	void Update ()
    {

        //******************敵と一番近い木を探す処理*****************
        //オブジェクトの座標
        //enemyPos = gameObject.transform.position;

        //ツリータグを探す
        //trees = GameObject.FindGameObjectsWithTag("SpownSelectTree");

        ////エネミーに一番近いツリーのタグを探す
        //for (int i = 0; i < trees.Length; i++)
        //{
        //    l_dis = Vector2.Distance(enemyPos, trees[i].transform.position);

        //    if (dis == 0.0f || dis > l_dis)
        //    {
        //        dis = l_dis;
        //    }
        //}
        //***********************************************************


        //*********敵の元の座標の近くに木があったらその近くにポジションを設定、
        //なければランダム（G、Hエリア内）にポジションを設定*********

        //移動(トランスフォームだとシーンビューとゲームビューで移動量が変わる可能性があるため)
        //float x = m_RectTransform.anchoredPosition.x;
        //float y = m_RectTransform.anchoredPosition.y;

        //木のとなりの座標
        //x = l_dis + 30.0f;
        //y = l_dis + 30.0f;

        //ランダムの座標
        randomPos = new Vector2(ramX, ramY);

        //m_RectTransform.anchoredPosition = randomPos;

        //ツリーと敵の距離がdis以下だったらその近くに座標を移動
        //if (dis <= 60.0f)
        //{
        //    m_RectTransform.anchoredPosition = new Vector2(l_dis + 30.0f,l_dis + 30.0f);
        //    //enemyPos = new Vector2(x, y);
        //    enemyPos = m_RectTransform.anchoredPosition;
        //}

        //でなければランダムに設定した数値に座標がいく
        //else
        //{
        //    //m_RectTransform.anchoredPosition = randomPos;
        //    //enemyPos = m_RectTransform.anchoredPosition;
        //}
        //************************************************************
        m_RectTransform.anchoredPosition = randomPos;

        //********スポーンセレクトシーンで決めた座標をゲームプレイシーンの敵の座標に反映する**********
        //移動した座標を現在の座標に変換
        //m_RectTransform.anchoredPosition = new Vector2(x, y);
        //現在の座標
        //spownPos2 = new Vector2(x, y);
        spownPos2 = randomPos;
        //********************************************************
    }
}
