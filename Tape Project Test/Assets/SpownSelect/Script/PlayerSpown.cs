using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSpown : MonoBehaviour
{
    public static Vector2 spownPos;

    public float m_Speed = 200f;

    public GameObject m_SpownOK;
    public GameObject m_SpownNG;

    RectTransform m_RectTransform;
    
    private Vector2 playerPos;
    
    GameObject[] trees;

    Button buttonA;

    Image image;

    public AudioSource audioSource;

    enum state
    {
        selectNow,
        decision,
    }

    state m_State = state.selectNow;

    //Camera c = Camera.main;

    public void OnClick()
    {
        if (m_State == state.selectNow)
        {
            //バトルシーンに遷移
            //SceneController.Instance.ChangeScenes(1);
            m_State = state.decision;
            audioSource.Play();
        }

              
    }

    void Start()
    {
        m_RectTransform = GetComponent<RectTransform>();

        image = GetComponent<Image>();
        
        buttonA = GameObject.Find("PlayerSpown").GetComponent<Button>();

        m_SpownOK.SetActive(false);
        m_SpownNG.SetActive(false);

        audioSource = gameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        if (m_State == state.decision)
        {
            return;
        }

        if(Input.GetButtonDown("Cancel"))
        {
            //タイトルシーンに遷移
            SceneController.Instance.ChangeScenes(0);
        }

        //ツリータグを探す
        trees = GameObject.FindGameObjectsWithTag("SpownSelectTree");

        //オブジェクトの座標
        playerPos = gameObject.transform.position;

        float dis = 0.0f;

        //プレイヤーアイコンに一番近いツリーのタグを探す
        for (int i=0;i<trees.Length;i++)
        {    
            float l_dis = Vector2.Distance(playerPos, trees[i].transform.position);

            if(dis == 0.0f || dis > l_dis)
            {
                dis = l_dis;
            }
        }

        //ツリーとプレイヤーの距離がdis以下だったら色変更とボタン機能を消す
        if (dis <= 50.0f)
        {
            image.color = Color.red;
            buttonA.enabled = false;
            m_SpownNG.SetActive(true);
            m_SpownOK.SetActive(false);
        }
        
        else
        {
            image.color = Color.blue;
            buttonA.enabled = true;
            m_SpownOK.SetActive(true);
            m_SpownNG.SetActive(false);
        }

        //移動(トランスフォームだとシーンビューとゲームビューで移動量が変わる可能性があるため)
        float x = m_RectTransform.anchoredPosition.x;
        float y = m_RectTransform.anchoredPosition.y;

        //移動する
        x += Input.GetAxis("Horizontal") * m_Speed * Time.deltaTime;
        y += Input.GetAxis("Vertical") * m_Speed * Time.deltaTime;

        //移動制限
        x = Mathf.Clamp(x, -167.0f, -119.0f);
        y = Mathf.Clamp(y, -189.0f, 138.0f);

        //移動した座標を現在の座標に変換
        m_RectTransform.anchoredPosition = new Vector2(x, y);

        //現在の座標
        spownPos = new Vector2(x, y);
    }
}
