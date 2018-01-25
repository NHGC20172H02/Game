using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyState;

public partial class EnemyAI4 : Character
{

    [SerializeField]
    float m_speed = 5f;

    public StringShooter m_Shooter;

    private Vector3 jump_start;
    private Vector3 jump_end;
    private RaycastHit jump_target;         //ジャンプの対象
    [Header("地面から跳べる木の探知範囲")]
    public float ground_detection = 30.0f;
    [Header("木の探知範囲")]
    public float tree_Detection = 100.0f;
    [Header("Playerの探知範囲")]
    public float player_Detection = 80.0f;

    [Header("Enemyのゲージのためる量（通常時）")]
    public float thought_Gauge = -100.0f;
    [Header("Enemyのゲージのためる量（優勢時）")]
    public float predominance_Thought_Gauge = -100.0f;
    //糸を奪う失敗する確率(1～10)
    int m_netrob = 4;

    int m_randomCount;
    int startRan;
    int treeObj = 0;
    int netCount;         //糸を奪う時の確率（randomカウント）
    int PlayerTree_Count; //Playerの木の本数
    int EnemyTree_Count;  //Enemyの木の本数

    //木に付いている糸の数の差
    int Thread_count_difference1;
    int Thread_count_difference2;
    int Thread_count_difference3;

    int col_number;

    float distThread; //近くの相手の糸の距離
    float dist40;     //近くの自分の陣地ではない木との距離
    float dist50;     //2番目の近くの自分の陣地ではない木との距離
    float distNet;    // ネットとの距離
    float myTreeDist;
    float playerDist;
    float m_gauge; //自分がいる木のゲージ量(-がEnemy、+がPlayer)
    float near_Gauge; //近くの木のゲージ量
    float playerNearDist = 15; //Playerとの距離（近くの範囲）

    int m_moveCount;   //歩く時の方向決め
    float wait_time;   //行動停止時間
    float dead_time;   //死んでる時間
    float m_moveTimer; //歩いてる時間
    float m_ground_jump_time;

    bool m_moveStart = false;
    bool net_bool = false;
    bool dead_bool = false;
    bool player_onTree;
    bool bodyBlow = true;
    bool noGaugeJump = false;

    //検索した木
    GameObject nearObj0;
    [System.NonSerialized]
    public GameObject nearObj;
    GameObject nearObj2;
    GameObject nearObj3;
    GameObject nearObj4;
    GameObject nearObj40;
    GameObject nearObj50;
    GameObject myTreeObj;
    GameObject myTreeObj2;
    GameObject myTreeObj3;

    //飛ぼうとしている木を入れるもの
    GameObject eyeObj;

    GameObject stringObj1; //近くの相手の糸
    GameObject stringNet;  //近くの相手のネット

    GameObject playerObj;

    [System.NonSerialized]
    public GameObject reObj;
    GameObject reObj2;

    //飛ぶ座標
    Vector3 m_targetPos;
    Vector3 m_playerTarget;

    //相手の木に付いている糸の数
    List<int> count2; 
    List<int> count3;
    List<int> count4;

    //自分の木に付いている糸の数
    List<int> mytreecount1;
    List<int> mytreecount2;
    List<int> mytreecount3;

    Animator anim;

    //EnemyState
    StateProcessor m_StateProcessor = new StateProcessor();
    GroundMove m_GroundMove = new GroundMove();
    TreeDecision m_TreeDecision = new TreeDecision();
    TreeMove m_TreeMove = new TreeMove();
    ColorlessTree m_ColorlessTree = new ColorlessTree();
    SearchTree m_SearchTree = new SearchTree();
    SearchRandom m_SearchRandom = new SearchRandom();
    StringCount m_StringCount = new StringCount();
    SearchMyTreeGauge m_SearchMyTreeGauge = new SearchMyTreeGauge();
    SearchTreeGauge m_SearchTreeGauge = new SearchTreeGauge();

    Jumping m_Jumping = new Jumping();
    GroundJumping m_GroundJumping = new GroundJumping();
    JumpMove m_JumpMove = new JumpMove();

    PredominanceDecision m_PredominanceDecision = new PredominanceDecision();
    PredominanceStringCount m_PredominanceStringCount = new PredominanceStringCount();
    PredominanceMyTree m_PredominanceMyTree = new PredominanceMyTree();
    
    AttackJump m_AttackJump = new AttackJump();
    AttackJumpMove m_AttackJumpMove = new AttackJumpMove();

    FallGroundMove m_FallGroundMove = new FallGroundMove();
    Fall m_Fall = new Fall();
    //Falling m_Falling = new Falling();

    // Use this for initialization
    protected override void Start()
    {
        startRan = Random.Range(1, 3);
        anim = GetComponent<Animator>();

        m_StateProcessor.State = m_GroundMove;
        m_GroundMove.exeDelegate = GroundMove;
        m_TreeDecision.exeDelegate = TreeDecision;
        m_TreeMove.exeDelegate = TreeMove;
        m_ColorlessTree.exeDelegate = ColorlessTree;
        m_SearchTree.exeDelegate = SearchTree;
        m_SearchRandom.exeDelegate = SearchRandom;
        m_StringCount.exeDelegate = StringCount;
        m_SearchMyTreeGauge.exeDelegate = SearchMyTreeGauge;
        m_SearchTreeGauge.exeDelegate = SearchTreeGauge;

        m_Jumping.exeDelegate = Jumping;
        m_GroundJumping.exeDelegate = GroundJumping;
        m_JumpMove.exeDelegate = JumpMove;

        m_PredominanceDecision.exeDelegate = PredominanceDecision;
        m_PredominanceStringCount.exeDelegate = PredominanceStringCount;
        m_PredominanceMyTree.exeDelegate = PredominanceMyTree;
        
        m_AttackJump.exeDelegate = AttackJump;
        m_AttackJumpMove.exeDelegate = AttackJumpMove;

        m_FallGroundMove.exeDelegate = FallGroundMove;
        m_Fall.exeDelegate = Fall;
        //m_Falling.exeDelegate = Falling;

        m_StateProcessor.State = m_GroundMove;
    }

    // Update is called once per frame
    protected override void Update()
    {
        //誰の陣地でもない近くの木
        nearObj0 = GetComponent<NearObj>().m_nearObj0;

        //近かったオブジェクト（木）を取得
        nearObj2 = GetComponent<NearObj>().m_nearObj2;
        if (nearObj2 == null) return;

        //2番目のオブジェクト（木）
        nearObj3 = GetComponent<NearObj>().m_nearObj3;
        if (nearObj3 == null) return;

        //3番目のオブジェクト（木）
        nearObj4 = GetComponent<NearObj>().m_nearObj4;
        if (nearObj4 == null) return;

        //近くの自分の陣地ではない木
        nearObj40 = GetComponent<NearObj>().m_nearObj40;

        //2番目の自分の陣地ではない木
        nearObj50 = GetComponent<NearObj>().m_nearObj50;

        //近くの自分の陣地の木
        myTreeObj = GetComponent<NearObj>().m_myTreeObj;

        //近くの自分の陣地の木
        myTreeObj2 = GetComponent<NearObj>().m_myTreeObj2;

        //近くの自分の陣地の木
        myTreeObj3 = GetComponent<NearObj>().m_myTreeObj3;

        //近くの相手の糸
        stringObj1 = GetComponent<NearObj>().m_stringObj1;

        //近くの相手のネット
        stringNet = GetComponent<NearObj>().m_stringNet;

        //Playerの取得
        playerObj = GameObject.Find("PlayerCamera/Player");
        //Playerの取得
        //playerObj = GameObject.FindGameObjectWithTag("Player");

        //Playerの木の本数
        PlayerTree_Count = TerritoryManager.Instance.GetTreeCount(1);
        
        //Enemyの木の本数
        EnemyTree_Count = TerritoryManager.Instance.GetTreeCount(2);

        //近くの相手の木の糸の数
        if (nearObj2 != null)
        {
            count2 = new List<int> { 0, 0, 0 };
            foreach (var item in nearObj2.GetComponent<Tree>().m_Child)
            {
                count2[item.m_SideNumber]++;
            }
        }
        if (nearObj3 != null)
        {
            count3 = new List<int> { 0, 0, 0 };
            foreach (var item in nearObj3.GetComponent<Tree>().m_Child)
            {
                count3[item.m_SideNumber]++;
            }
        }
        if (nearObj4 != null)
        {
            count4 = new List<int> { 0, 0, 0 };
            foreach (var item in nearObj4.GetComponent<Tree>().m_Child)
            {
                count4[item.m_SideNumber]++;
            }
        }

        //近くの自分の木の糸の数
        if (myTreeObj != null)
        {
            mytreecount1 = new List<int> { 0, 0, 0 };
            foreach (var item in myTreeObj.GetComponent<Tree>().m_Child)
            {
                mytreecount1[item.m_SideNumber]++;
            }
        }
        if (myTreeObj2 != null)
        {
            mytreecount2 = new List<int> { 0, 0, 0 };
            foreach (var item in myTreeObj2.GetComponent<Tree>().m_Child)
            {
                mytreecount2[item.m_SideNumber]++;
            }
        }
        if (myTreeObj3 != null)
        {
            mytreecount3 = new List<int> { 0, 0, 0 };
            foreach (var item in myTreeObj2.GetComponent<Tree>().m_Child)
            {
                mytreecount3[item.m_SideNumber]++;
            }
        }

        //自分のいる木のゲージ量
        if(nearObj != null)
        m_gauge = nearObj.GetComponent<Tree>().m_TerritoryRate;

        //１つ前にいた木を保持
        if (treeObj == 1 && reObj == null)
        {
            reObj = nearObj;
        }
        if (treeObj >= 2)
        {
            reObj2 = nearObj;
        }
        if (treeObj >= 3)
        {
            reObj = reObj2;
            treeObj = 2;
        }


        //playerとの距離と木にいるか
        if (playerObj != null)
        {
            playerDist = Vector3.Distance(playerObj.transform.position, this.transform.position);
            player_onTree = true;
            //player_onTree = playerObj.GetComponent<Player>().IsOnTree();
        }
        //近くのネットとの距離
        if (stringNet != null)
        {
            distNet = Vector3.Distance(stringNet.transform.position, this.transform.position);
        }
        //近くの相手の糸の距離
        if (stringObj1 != null)
        { 
            distThread = Vector3.Distance(stringObj1.transform.position, this.transform.position);
        }

        //Playerに当たった時
        if (dead_bool == false)
        {
            if (playerDist >= 0.1f && playerDist <= 1)
            {
                if(isBodyblow)
                m_StateProcessor.State = m_Fall;
            }
        }


        //if (Input.GetKeyDown(KeyCode.G))
        //{
        //    m_StateProcessor.State = m_Fall;
        //}
        //Debug.Log(nearObj.transform.name);
        Debug.Log(m_StateProcessor.State);
        Debug.DrawLine(transform.position, m_targetPos, Color.blue);

        m_StateProcessor.Execute();
    }




    /*** 地面移動 ***/
    private void GroundMove()
    {
        anim.SetBool("move_front", true);
   
        if (startRan == 1)
        {
            m_targetPos = GetPosition();
            startRan = 0;
        }
        if (startRan == 2)
        {
            m_targetPos = GetPosition2();
            startRan = 0;
        }

        
        RaycastHit hit;
        Ray ray = new Ray(transform.position + transform.up * 0.5f, -transform.up);
        if (Physics.Raycast(ray, out hit, 1f))
        {
            if (hit.transform.tag == "Ground")
            {
                transform.position = Vector3.Lerp(transform.position, hit.point, 0.2f);
                transform.rotation = Quaternion.LookRotation(
                    Vector3.Lerp(transform.forward, Vector3.Cross(transform.right, hit.normal), 0.3f), hit.normal);
            }
        }


        float dist = Vector3.Distance(nearObj2.transform.position, this.transform.position);

        if (dist <= ground_detection)
        {
            anim.SetBool("move_front", false);

            wait_time += Time.deltaTime * 1;
            if (wait_time >= 1)
            {
                m_targetPos = GetPosition3();
                m_StateProcessor.State = m_GroundJumping;
            }
        }
        else
        {
            transform.Translate(Vector3.forward * m_speed * Time.deltaTime);
            //ポジションの方に向く
            Quaternion targetRotation2 = Quaternion.LookRotation(m_targetPos - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation2, Time.deltaTime * 5.0f);

        }
    }

    /*** 落ちた後の地面の移動 ***/
    private void FallGroundMove()
    {
        anim.SetBool("jump", false);
        anim.SetBool("trap", false);
        anim.SetBool("jumpair", false);
        anim.SetBool("avoidance", false);
        anim.SetBool("move_front", true);

        RaycastHit hit;
        Ray ray = new Ray(transform.position + transform.up * 0.5f, -transform.up);
        if (Physics.Raycast(ray, out hit, 1f))
        {
            if (hit.transform.tag == "Ground")
            {
                transform.position = Vector3.Lerp(transform.position, hit.point, 0.2f);
                transform.rotation = Quaternion.LookRotation(
                    Vector3.Lerp(transform.forward, Vector3.Cross(transform.right, hit.normal), 0.3f), hit.normal);
            }
            if (hit.transform.tag == "Tree")
            {
                transform.position = Vector3.Lerp(transform.position, hit.point, 0.2f);
                transform.rotation = Quaternion.LookRotation(
                    Vector3.Lerp(transform.forward, Vector3.Cross(transform.right, hit.normal), 0.3f), hit.normal);
            }
        }

        dead_time += Time.deltaTime * 1;
        if (dead_time >= 1.0f)
        {
            anim.SetBool("dead", false);

            wait_time += Time.deltaTime * 1;

            if (wait_time >= 0.5f)
            {
                anim.SetBool("move_front", true);

                m_targetPos = GetPosition();


                float dist = Vector3.Distance(nearObj2.transform.position, this.transform.position);

                if (dist <= ground_detection)
                {
                    anim.SetBool("move_front", false);

                    m_ground_jump_time += Time.deltaTime * 1;
                    if (m_ground_jump_time >= 1)
                    {
                        m_ground_jump_time = 0;
                        dead_time = 0;
                        m_targetPos = GetPosition3();
                        m_StateProcessor.State = m_GroundJumping;
                    }
                }
                else
                {
                    transform.Translate(Vector3.forward * m_speed * Time.deltaTime);
                    //ポジションの方に向く
                    Quaternion targetRotation2 = Quaternion.LookRotation(m_targetPos - transform.position);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation2, Time.deltaTime * 5.0f);
                }
            }
        }
    }



    /*** 木にいるときの行動判断 ***/
    private void TreeDecision()
    {
        anim.SetBool("jump", false);
        anim.SetBool("jumpair", false);
        anim.SetBool("avoidance", false);
        anim.SetBool("Attack", false);

        //近くの自分の陣地ではない木との距離
        if (nearObj40 != null)
        {
            dist40 = Vector3.Distance(nearObj40.transform.position, this.transform.position);
        }
        //2番目に近い自分の陣地ではない木との距離
        if (nearObj50 != null)
        {
            dist50 = Vector3.Distance(nearObj50.transform.position, this.transform.position);
        }

        wait_time = 0;

        RaycastHit hit;
        Ray ray = new Ray(transform.position + transform.up * 0.5f, -transform.up);
        int treeLayer = LayerMask.GetMask(new string[] { "Tree" });
        if (Physics.SphereCast(ray, 1f, out hit, 1f, treeLayer))
        {
            if (hit.transform.tag == "Tree")
            {
                transform.position = Vector3.Lerp(transform.position, hit.point, 0.2f);
                transform.rotation = Quaternion.LookRotation(
                    Vector3.Lerp(transform.forward, Vector3.Cross(transform.right, hit.normal), 0.3f), hit.normal);

                nearObj = hit.collider.gameObject;
                dead_bool = false;
            }
            else
            {
                m_StateProcessor.State = m_Fall;
            }
        }

        //自分が優勢の時
        if (EnemyTree_Count > PlayerTree_Count)
        {
            m_StateProcessor.State = m_PredominanceDecision;
        }
        //劣勢時
        if (EnemyTree_Count < PlayerTree_Count)
        {
            if (playerDist <= playerNearDist && player_onTree == playerObj.GetComponent<Player>().IsOnTree()) //playerが近くにいた場合近くの木に飛ぶ
            {
                m_StateProcessor.State = m_SearchRandom;
            }
            else
            {
                m_StateProcessor.State = m_SearchTree;
            }
        }
        else if (nearObj0 != null) //無色の木
        {
            if (playerDist <= 20) //playerが近くにいた場合近くの木に飛ぶ
            {
                m_StateProcessor.State = m_SearchRandom;
            }
            else
            {
                m_StateProcessor.State = m_ColorlessTree;
            }
        }
        else if (playerDist > 20 && playerDist <= player_Detection &&
           player_onTree == playerObj.GetComponent<Player>().IsOnTree()) //Playerに攻撃、playerが木にいるか？
        {
            if (m_gauge <= -75f)
            {
                m_playerTarget = GetPlayerPosition();
                m_StateProcessor.State = m_AttackJump;
            }
            else
            {
                if (m_randomCount != 1 && m_randomCount != 3)
                    m_randomCount = Random.Range(1, 3);
                if (m_randomCount == 1)
                {
                    m_randomCount = 0;
                    m_playerTarget = GetPlayerPosition();
                    m_StateProcessor.State = m_AttackJump;
                }
                else
                {
                    m_randomCount = 0;
                    m_StateProcessor.State = m_SearchTree;
                    //m_StateProcessor.State = m_SearchTreeGauge;
                }
            }
        }
        else if (playerDist <= playerNearDist && player_onTree == playerObj.GetComponent<Player>().IsOnTree()) //playerが近くにいた場合近くの木に飛ぶ
        {
            m_StateProcessor.State = m_SearchRandom;
        }
        else if (nearObj40 == null && nearObj50 == null) //青、白の木が近くにある場合
        {
            //m_StateProcessor.State = m_SearchTreeGauge;
            m_StateProcessor.State = m_SearchTree;
        }
        else
        {
            m_StateProcessor.State = m_SearchRandom;
        }
    }

    /*** 木での移動 ***/
    private void TreeMove()
    {
        bool randamStart = true;

        if (randamStart == true)
        {
            if (m_moveCount != 1 && m_moveCount != 2 && m_moveCount != 3 && m_moveCount != 4)
                m_moveCount = Random.Range(1, 5);

            randamStart = false;
        }

        RaycastHit hit;
        Ray ray = new Ray(transform.position + transform.up * 0.5f, -transform.up);
        int treeLayer = LayerMask.GetMask(new string[] { "Tree" });
        if (Physics.SphereCast(ray, 1f, out hit, 1f, treeLayer))
        {
            transform.position = Vector3.Lerp(transform.position, hit.point, 0.2f);
            transform.rotation = Quaternion.LookRotation(
                Vector3.Lerp(transform.forward, Vector3.Cross(transform.right, hit.normal), 0.3f), hit.normal);

            //if (hit.transform.tag == "Tree")
            //{
            //    transform.position = Vector3.Lerp(transform.position, hit.point, 0.2f);
            //    transform.rotation = Quaternion.LookRotation(
            //        Vector3.Lerp(transform.forward, Vector3.Cross(transform.right, hit.normal), 0.3f), hit.normal);
            //}
            //if(hit.transform.gameObject == null)
            //{
            //    m_StateProcessor.State = m_Fall;
            //}
        }

        if (m_moveCount == 1) //前移動
        {
            anim.SetBool("move_front", true);
            transform.Translate(Vector3.forward * m_speed * Time.deltaTime);
            m_moveStart = true;
        }
        if (m_moveCount == 2) //後ろ移動
        {
            anim.SetBool("move_back", true);
            transform.Translate(-Vector3.forward * m_speed * Time.deltaTime);
            m_moveStart = true;
        }
        if (m_moveCount == 3) //右移動
        {
            anim.SetBool("move_right", true);
            transform.Translate(Vector3.right * m_speed * Time.deltaTime);
            m_moveStart = true;
        }
        if (m_moveCount == 4) //左移動
        {
            anim.SetBool("move_left", true);
            transform.Translate(Vector3.left * m_speed * Time.deltaTime);
            m_moveStart = true;
        }


        if (m_moveStart == true)
        {
            m_moveTimer += Time.deltaTime * 1;

            if (m_moveTimer >= 1.0f)
            {
                m_moveCount = 0;

                anim.SetBool("move_front", false);
                anim.SetBool("move_back", false);
                anim.SetBool("move_left", false);
                anim.SetBool("move_right", false);
            }
            if (m_moveTimer >= 1.5f)
            {
                m_moveStart = false;
                randamStart = true;
                m_moveTimer = 0;

                m_StateProcessor.State = m_TreeDecision;
            }
        }
    }

    /*** 無色の木を検索 ***/
    private void ColorlessTree()
    {
        float dist = Vector3.Distance(nearObj0.transform.position, this.transform.position);

        if (dist <= tree_Detection)
        {
            eyeObj = nearObj0;
            m_targetPos = GetUpPosition00();
            m_StateProcessor.State = m_Jumping;
        }
        else
        {
            m_StateProcessor.State = m_SearchTree;
        }
    }

    /*** 自分の木以外の木を検索 ***/
    private void SearchTree()
    {
        if (m_randomCount != 5 && m_randomCount != 4)
            m_randomCount = Random.Range(4, 6);

        if (nearObj40 != null)
        {
            //近くの自分の陣地ではない木との距離
            dist40 = Vector3.Distance(nearObj40.transform.position, this.transform.position);
        }
        if (nearObj50 != null)
        {
            //2番目に近い自分の陣地ではない木との距離
            dist50 = Vector3.Distance(nearObj50.transform.position, this.transform.position);
        }

        if (m_randomCount == 4)
        {
            if (dist40 <= tree_Detection)
            {
                if (nearObj40 != null)
                {
                    eyeObj = nearObj40;
                    m_randomCount = 0;
                    m_targetPos = GetUpPosition40();
                    m_StateProcessor.State = m_Jumping;
                }
                else if (nearObj40 == null)
                {
                    m_randomCount = 0;
                    m_StateProcessor.State = m_StringCount;
                }
            }
            else
            {
                m_randomCount = 0;
                m_StateProcessor.State = m_StringCount;
            }
        }
        if (m_randomCount == 5)
        {
            if (dist50 <= tree_Detection)
            {
                if (nearObj50 != null)
                {
                    eyeObj = nearObj50;
                    m_randomCount = 0;
                    m_targetPos = GetUpPosition50();
                    m_StateProcessor.State = m_Jumping;
                }
                else if (nearObj50 == null)
                {
                    m_randomCount = 0;
                    m_StateProcessor.State = m_StringCount;
                }
            }
            else
            {
                m_randomCount = 0;
                m_StateProcessor.State = m_StringCount;
            }
        }
    }

    /*** 木をランダムに検索 ***/
    private void SearchRandom()
    {
        if (m_randomCount != 5 && m_randomCount != 4 && m_randomCount != 6)
            m_randomCount = Random.Range(4, 7);
        if (playerDist <= playerNearDist && player_onTree == playerObj.GetComponent<Player>().IsOnTree())
            noGaugeJump = true;

        //乱数が５になったら
        if (m_randomCount == 5)
        {
            eyeObj = nearObj2;
            m_randomCount = 0;
            //近い木に飛ぶ
            m_targetPos = GetUpPosition2();

            m_StateProcessor.State = m_Jumping;
        }
        if (m_randomCount == 4) //乱数が４になったら
        {
            eyeObj = nearObj3;
            m_randomCount = 0;
            //２番目の近くの木に飛ぶ
            m_targetPos = GetUpPosition3();

            m_StateProcessor.State = m_Jumping;
        }
        if (m_randomCount == 6)
        {
            eyeObj = nearObj3;
            m_randomCount = 0;
            //3番目の近くの木に飛ぶ
            m_targetPos = GetUpPosition4();

            m_StateProcessor.State = m_Jumping;
        }
    }

    /*** 近くの木の付いている糸の判別 ***/
    private void StringCount()
    {
        int sidenumber = GetComponent<StringShooter>().m_SideNumber;
        //GameObject player;
        //player = GameObject.FindWithTag("Player");
        //int playerNumber = player.GetComponent<StringShooter>().m_SideNumber;
        Thread_count_difference1 = count2[1] - count2[sidenumber]; //近くの木
        Thread_count_difference2 = count3[1] - count3[sidenumber]; //2番目の木
        Thread_count_difference3 = count4[1] - count4[sidenumber]; //2番目の木

        if (Thread_count_difference1 >= 0 && Thread_count_difference1 <= 2 && count2[sidenumber] > count2[1]) //近くの木の糸の本数が２本以下
        {
            eyeObj = nearObj2;
            //近い木に飛ぶ
            m_targetPos = GetUpPosition2();
            m_StateProcessor.State = m_Jumping;
        }
        else if (Thread_count_difference2 >= 0 && Thread_count_difference2 <= 2 && count3[sidenumber] > count3[1]) //２番目の近くの木の糸の本数が２本以下
        {
            eyeObj = nearObj3;
            //２番目の近くの木に飛ぶ
            m_targetPos = GetUpPosition3();
            m_StateProcessor.State = m_Jumping;
        }
        else if (Thread_count_difference3 >= 0 && Thread_count_difference3 <= 2 && count4[sidenumber] > count4[1]) //３番目の近くの木の糸の本数が２本以下
        {
            eyeObj = nearObj4;
            //３番目の近くの木に飛ぶ
            m_targetPos = GetUpPosition4();
            m_StateProcessor.State = m_Jumping;
        }
        else
        {
            m_StateProcessor.State = m_SearchRandom;
        }
    }

    /*** 近くの自分の木＋ゲージ量  必要ないかもしれない ***/
    private void SearchMyTreeGauge()
    {
        if(myTreeObj != null)
        near_Gauge = myTreeObj.GetComponent<Tree>().m_TerritoryRate;
        if (near_Gauge >= -50f)
        {
            if (myTreeObj != null)
            {
                eyeObj = myTreeObj;
                m_targetPos = MyTreePosition1();
                m_StateProcessor.State = m_Jumping;
            }
            else if (myTreeObj == null)
            {
                m_StateProcessor.State = m_SearchRandom;
            }
        }
        else
        {
            m_StateProcessor.State = m_SearchTree;
        }
    }
    /*** 近くの相手の木＋ゲージ量  必要ないかもしれない***/
    private void SearchTreeGauge()
    {
        if(nearObj40 != null)
        near_Gauge = nearObj40.GetComponent<Tree>().m_TerritoryRate;

        if (near_Gauge <= 50f)
        {
            if (dist40 <= tree_Detection)
            {
                if (nearObj40 != null)
                {
                    eyeObj = nearObj40;
                    m_targetPos = GetUpPosition40();
                    m_StateProcessor.State = m_Jumping;
                }
                else if (nearObj40 == null && nearObj50 != null)
                {
                    if (dist50 <= tree_Detection)
                    {
                        eyeObj = nearObj50;
                        m_targetPos = GetUpPosition50();
                        m_StateProcessor.State = m_Jumping;
                    }
                    else
                    {
                        m_StateProcessor.State = m_SearchRandom;
                    }
                }
                else
                {
                    m_StateProcessor.State = m_SearchRandom;
                }
            }
            else
            {
                m_StateProcessor.State = m_SearchRandom;
            }
        }
        else
        {
            m_StateProcessor.State = m_SearchTree;
        }
    }



    /*** 地面からジャンプ ***/
    private void GroundJumping()
    {
        wait_time = 0;

        int treeLayer = LayerMask.GetMask(new string[] { "Tree" });
        //移動先と自分の間のray
        if (Physics.Raycast(transform.position, m_targetPos - transform.position, out jump_target, 100f, treeLayer))
        {
            anim.SetBool("jump", true);
            jump_start = transform.position;
            jump_end = jump_target.point;
        }
        else
        {
            m_StateProcessor.State = m_TreeDecision;
        }


        AnimatorStateInfo animInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (animInfo.normalizedTime < 1.0f)
        {
            anim.SetBool("jumpair", true);
        }


        if (Projection(jump_start, jump_end, jump_target.normal, 30.0f))
        {
            transform.position = jump_end;
            m_StateProcessor.State = m_TreeDecision;
        }
    }

    /*** ジャンプの瞬間 ***/
    private void Jumping()
    {
        int treeLayer = LayerMask.GetMask(new string[] { "Tree" });
        //移動先と自分の間のray
        if (Physics.Raycast(transform.position, m_targetPos - transform.position, out jump_target, tree_Detection, treeLayer))
        {
            if (jump_target.transform != eyeObj.transform)
            {
                m_StateProcessor.State = m_TreeMove;
            }
            else if (jump_target.transform == eyeObj.transform) //飛びたいところの間に障害物がなければ
            {
                if (m_gauge <= thought_Gauge)　//ゲージをためる
                {
                    net_bool = true;
                    anim.SetBool("jump", true);
                    jump_start = transform.position;
                    jump_end = jump_target.point;
                    m_StateProcessor.State = m_JumpMove;
                }
                if (playerDist <= playerNearDist && player_onTree == playerObj.GetComponent<Player>().IsOnTree())
                {
                    m_StateProcessor.State = m_SearchRandom;
                }

                if (noGaugeJump == true)
                {
                    noGaugeJump = false;
                    net_bool = true;
                    anim.SetBool("jump", true);
                    jump_start = transform.position;
                    jump_end = jump_target.point;
                    m_StateProcessor.State = m_JumpMove;
                }
            }
        }
        else
        {
            m_StateProcessor.State = m_SearchRandom;
        }
    }

    /*** ジャンプ移動中 ***/
    private void JumpMove()
    {
        anim.SetBool("jumpair", true);
        anim.SetBool("jump", false);
        if (Projection(jump_start, jump_end, jump_target.normal, 30.0f))
        { 
            transform.position = jump_end;
            //m_Shooter.StringShoot(jump_start, jump_end);
            m_StateProcessor.State = m_TreeDecision;
        }

        int sidenumber = GetComponent<StringShooter>().m_SideNumber;

        //糸を奪う
        if (distThread >= 0.5f && distThread <= 2 || distNet >= 0.5f && distNet <= 2)
        {
            //奪う確率
            if (net_bool == true)
            {
                netCount = Random.Range(1, 11);
                net_bool = false;
            }

            if (netCount <= m_netrob) //失敗したとき
            {
                m_StateProcessor.State = m_Fall;
            }
            else //成功したとき
            {
                anim.SetBool("avoidance", true);

                stringObj1.GetComponent<StringUnit>().SideUpdate(sidenumber);

                AnimatorStateInfo animInfo = anim.GetCurrentAnimatorStateInfo(0);
                if (animInfo.normalizedTime < 1.0f)
                {
                    anim.SetBool("avoidance", false);
                }
            }
        }
    }

    /*** 攻撃ジャンプ ***/
    private void AttackJump()
    {
        bodyBlow = true;

        //移動先と自分の間のray
        if (Physics.Raycast(transform.position, m_playerTarget - transform.position, out jump_target, tree_Detection))
        {
            if (jump_target.transform != playerObj.transform)
            {
                m_StateProcessor.State = m_TreeDecision;
            }
            else if (jump_target.transform == playerObj.transform) //飛びたいところの間に障害物がなければ
            {
                if (m_gauge <= thought_Gauge)　//ゲージをためる
                {
                    dead_bool = true;
                    net_bool = true;
                    anim.SetBool("jump", true);
                    jump_start = transform.position;
                    jump_end = jump_target.point;
                    m_StateProcessor.State = m_AttackJumpMove;
                }
                if (playerDist <= playerNearDist && player_onTree == playerObj.GetComponent<Player>().IsOnTree())
                {
                    m_StateProcessor.State = m_SearchRandom;
                }
            }
        }
        else
        {
            m_StateProcessor.State = m_SearchRandom;
        }
    }

    /*** 攻撃ジャンプ移動中 ***/
    private void AttackJumpMove()
    {
        if (playerDist <= 5) //攻撃
        {
            anim.SetBool("Attack", true);
            if(bodyBlow == true)
            {
                SendingBodyBlow(playerObj);
                bodyBlow = false;
            }
            
        }
        else
        {
            anim.SetBool("jumpair", true);
            anim.SetBool("jump", false);
        }

        if (Projection(jump_start, jump_end, jump_target.normal, 30.0f))
        {
            transform.position = jump_end;
            m_Shooter.StringShoot(jump_start, jump_end);
            m_StateProcessor.State = m_TreeDecision;
        }


        int sidenumber = GetComponent<StringShooter>().m_SideNumber;

        
        //糸を奪う
        if (distThread >= 0.5f && distThread <= 2 || distNet >= 0.5f && distNet <= 2)
        {
            //奪う確率
            if (net_bool == true)
            {
                netCount = Random.Range(1, 11);
                net_bool = false;
            }

            if (netCount <= m_netrob) //失敗したとき
            {
                m_StateProcessor.State = m_Fall;
            }
            else //成功したとき
            {
                anim.SetBool("avoidance", true);

                AnimatorStateInfo animInfo = anim.GetCurrentAnimatorStateInfo(0);
                if (animInfo.normalizedTime < 1.0f)
                {
                    anim.SetBool("avoidance", false);
                }

                stringObj1.GetComponent<StringUnit>().SideUpdate(sidenumber);
            }
        }

    }



    /*** 優勢時の行動判断 ***/
    private void PredominanceDecision()
    {
        anim.SetBool("jump", false);
        anim.SetBool("jumpair", false);
        anim.SetBool("avoidance", false);
        anim.SetBool("Attack", false);
        if (myTreeObj != null)
        {
            //近くの糸との距離
            myTreeDist = Vector3.Distance(myTreeObj.transform.position, this.transform.position);
        }

        RaycastHit hit;
        Ray ray = new Ray(transform.position + transform.up * 0.5f, -transform.up);
        int treeLayer = LayerMask.GetMask(new string[] { "Tree" });
        if (Physics.Raycast(ray, out hit, 1f, treeLayer))
        {
            if (hit.transform.tag == "Tree")
            {
                transform.position = Vector3.Lerp(transform.position, hit.point, 0.2f);
                transform.rotation = Quaternion.LookRotation(
                    Vector3.Lerp(transform.forward, Vector3.Cross(transform.right, hit.normal), 0.3f), hit.normal);

                nearObj = hit.collider.gameObject;
            }
            else
            {
                m_StateProcessor.State = m_Fall;
            }
        }


        if (PlayerTree_Count != 0)
        {
            //Playerの木の本数
            PlayerTree_Count = TerritoryManager.Instance.GetTreeCount(1);
        }
        if (EnemyTree_Count != 0)
        {
            //Enemyの木の本数
            EnemyTree_Count = TerritoryManager.Instance.GetTreeCount(2);
        }


        //自身が劣勢の時
        if (EnemyTree_Count < PlayerTree_Count)
        {
            m_StateProcessor.State = m_TreeDecision;

        }
        
        else if (playerDist > 20 && playerDist <= player_Detection && 
            player_onTree == playerObj.GetComponent<Player>().IsOnTree()) //Playerに攻撃
        {
            m_playerTarget = GetPlayerPosition();
            m_StateProcessor.State = m_AttackJump;
        }
        else if (nearObj0 != null) //無色の木
        {
            if (playerDist <= 20) //playerが近くにいた場合近くの木に飛ぶ
            {
                m_StateProcessor.State = m_SearchRandom;
            }
            else
            {
                m_StateProcessor.State = m_ColorlessTree;
            }
        }
        else if (playerDist <= playerNearDist && player_onTree == playerObj.GetComponent<Player>().IsOnTree())
        {
            m_StateProcessor.State = m_SearchRandom;
        }
        else if (m_gauge <= predominance_Thought_Gauge)
        {
            m_StateProcessor.State = m_PredominanceStringCount;
        }
        else
        {
            m_StateProcessor.State = m_SearchRandom;
        }
    }

    /*** 優勢時の近くの木の付いている糸の判別 ***/
    private void PredominanceStringCount()
    {
        int sidenumber = GetComponent<StringShooter>().m_SideNumber;

        if (m_randomCount != 5 && m_randomCount != 4 && m_randomCount != 6)
            m_randomCount = Random.Range(4, 7);

        //近くの木の付いている糸の差
        Thread_count_difference1 = mytreecount1[1] - mytreecount1[sidenumber];
        if (myTreeObj2 != null)
        {
            //2番目近くの木の付いている糸の差
            Thread_count_difference2 = mytreecount2[1] - mytreecount2[sidenumber];
        }
        if(myTreeObj3 != null)
        {
            //3番目近くの木の付いている糸の差
            Thread_count_difference3 = mytreecount3[1] - mytreecount3[sidenumber];
        }

        if (Thread_count_difference1 >= 0 && Thread_count_difference1 <= 2 &&
            mytreecount1[sidenumber] > mytreecount1[1] && m_randomCount == 4) //近くの自分の木の糸の本数が２本以下
        {
            eyeObj = myTreeObj;
            m_randomCount = 0;
            //近い木に飛ぶ
            m_targetPos = MyTreePosition1();

            m_StateProcessor.State = m_Jumping;
        }
        else if (myTreeObj2 != null && Thread_count_difference2 >= 0 && Thread_count_difference2 <= 2 && 
            mytreecount2[sidenumber] > mytreecount2[1] && m_randomCount == 5)//２番目に近くの自分の木の糸の本数が２本以下
        {
            eyeObj = myTreeObj2;
            m_randomCount = 0;
            //２番目の近くの木に飛ぶ
            m_targetPos = MyTreePosition2();

            m_StateProcessor.State = m_Jumping;
        }
        else if (myTreeObj3 != null && Thread_count_difference3 >= 0 && Thread_count_difference3 <= 2 && 
            mytreecount3[sidenumber] > mytreecount3[1] && m_randomCount == 6)//３番目に近くの自分の木の糸の本数が２本以下
        {
            eyeObj = myTreeObj3;
            m_randomCount = 0;
            //３番目の近くの木に飛ぶ
            m_targetPos = MyTreePosition3();

            m_StateProcessor.State = m_Jumping;
        }
        else
        {
            m_randomCount = 0;
            m_StateProcessor.State = m_PredominanceMyTree;
        }
    }

    /*** 優勢時の近くの自分の陣地の木 ***/
    private void PredominanceMyTree()
    {
        if (m_randomCount != 5 && m_randomCount != 4)
            m_randomCount = Random.Range(4, 6);

        if (m_randomCount == 4)
        {
            if (myTreeObj != null)
            {
                eyeObj = myTreeObj;
                m_targetPos = MyTreePosition1();
                m_StateProcessor.State = m_Jumping;
            }
            else if (myTreeObj == null)
            {
                m_StateProcessor.State = m_SearchRandom;
            }
        }
        if (m_randomCount == 5)
        {
            if (myTreeObj2 != null)
            {
                eyeObj = myTreeObj2;
                m_targetPos = MyTreePosition2();
                m_StateProcessor.State = m_Jumping;
            }
            else if (myTreeObj2 == null)
            {
                eyeObj = myTreeObj3;
                m_targetPos = MyTreePosition3();
                m_StateProcessor.State = m_Jumping;
            }
        }
    }




    //落下
    //private void Fall()
    //{
    //    AnimatorStateInfo animInfo = anim.GetCurrentAnimatorStateInfo(0);

    //    anim.SetBool("trap", true);

    //    int treeLayer = LayerMask.GetMask(new string[] { "Tree" });
    //    int groundLayer = LayerMask.GetMask(new string[] { "Ground" });

    //    //落下先情報取得（木を優先）
    //    Ray ray = new Ray(transform.position, -Vector3.up);
    //    if (!Physics.Raycast(ray, out jump_target, 100f, treeLayer))
    //        Physics.Raycast(ray, out jump_target, 100f, groundLayer);
    //    jump_start = transform.position;
    //    jump_end = jump_target.point;

    //    m_StateProcessor.State = m_Falling;
    //    //m_StateProcessor.State = m_FallingMove;
    //}

    //落下中
    //private void Falling()
    //{
    //    anim.SetBool("dead", true);

    //    float dis = Vector3.Distance(jump_start, jump_end);
    //    //落下スピード
    //    Vector3 fallSpeed = Physics.gravity.y * Vector3.up;
    //    transform.Translate(fallSpeed * Time.deltaTime, Space.World);
    //    Vector3 forward = Vector3.Cross(transform.right, jump_target.normal);
    //    transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.forward, forward, 0.1f), jump_target.normal);


    //    RaycastHit hit;
    //    Ray ray2 = new Ray(transform.position, -transform.up);
    //    if (Physics.Raycast(ray2, out hit, 0.5f))
    //    {
    //        if (hit.transform.tag == "Ground")
    //        {
    //            transform.position = Vector3.Lerp(transform.position, hit.point, 0.2f);
    //            transform.rotation = Quaternion.LookRotation(
    //                Vector3.Lerp(transform.forward, Vector3.Cross(transform.right, hit.normal), 0.3f), hit.normal);

    //            anim.SetBool("jump", false);

    //            m_StateProcessor.State = m_FallGroundMove;
    //        }
    //        if (hit.transform.tag == "Tree")
    //        {
    //            transform.position = Vector3.Lerp(transform.position, hit.point, 0.2f);
    //            transform.rotation = Quaternion.LookRotation(
    //                Vector3.Lerp(transform.forward, Vector3.Cross(transform.right, hit.normal), 0.3f), hit.normal);

    //            anim.SetBool("jump", false);

    //            m_StateProcessor.State = m_FallGroundMove;
    //        }
    //    }
    //}

    //落下
    private void Fall()
    {
        AnimatorStateInfo animInfo = anim.GetCurrentAnimatorStateInfo(0);

        anim.SetBool("trap", true);
        if (animInfo.normalizedTime < 1.0f)
        {
            anim.SetBool("dead", true);
        }

        int treeLayer = LayerMask.GetMask(new string[] { "Tree" });
        int groundLayer = LayerMask.GetMask(new string[] { "Ground" });

        //落下先情報取得（木を優先）
        Ray ray = new Ray(transform.position, -Vector3.up);
        if (!Physics.Raycast(ray, out jump_target, 100f, treeLayer))
            Physics.Raycast(ray, out jump_target, 100f, groundLayer);

        //落下スピード
        Vector3 fallSpeed = Physics.gravity.y * Vector3.up;
        transform.Translate(fallSpeed * Time.deltaTime, Space.World);
        Vector3 forward = Vector3.Cross(transform.right, jump_target.normal);
        transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.forward, forward, 0.1f), jump_target.normal);

        RaycastHit hit;
        Ray ray2 = new Ray(transform.position, -transform.up);
        if (Physics.Raycast(ray2, out hit, 1.0f,groundLayer))
        {
            transform.position = Vector3.Lerp(transform.position, hit.point, 0.2f);
            transform.rotation = Quaternion.LookRotation(
                Vector3.Lerp(transform.forward, Vector3.Cross(transform.right, hit.normal), 0.3f), hit.normal);

            anim.SetBool("jump", false);

            m_StateProcessor.State = m_FallGroundMove;
        }
        //m_StateProcessor.State = m_FallingMove;
    }


    private void OnTriggerEnter(Collider col)
    {
        if (col.transform.tag == "Tree")
        {
            m_randomCount = 0;

            col_number = col.gameObject.GetComponent<Tree>().m_SideNumber;
            reObj2 = col.gameObject.gameObject;
            nearObj = col.gameObject.gameObject;
        }

        if (col.transform.tag == "Ground")
        {
            ResetBodyblow();

            m_StateProcessor.State = m_GroundMove;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.transform.tag == "Tree")
        {
            if(m_StateProcessor.State == m_TreeMove)
            {
                m_StateProcessor.State = m_Fall;
            }
        }
    }

}
