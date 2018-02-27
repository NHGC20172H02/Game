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
    public float ground_detection = 10.0f;
    [Header("木の探知範囲")]
    public float tree_Detection = 180.0f;
    [Header("Playerの探知範囲")]
    public float player_Detection = 100.0f;

    [Header("Enemyのゲージのためる量")]
    public float thought_Gauge = -100.0f;
    //糸を奪う失敗する確率(1～10)
    int m_netrob = 4;

    //攻撃を受けた後のダウン時間
    float down_time = 2.0f;

    float jump_wait = 1.0f;

    int m_randomCount;
    int jump_random;
    int walk_count;          //歩いた回数
    int netCount;            //糸を奪う時の確率（randomカウント）
    int PlayerTree_Count;    //Playerの木の本数
    int EnemyTree_Count;     //Enemyの木の本数
    int color_number;        //木の色
    int myString_number;     //Enemyの糸の色
    int playerString_number; //Playerの糸の色
    int player_number;       //Playerの乗っている木の色

    //木に付いている糸の数の差
    int Thread_count_difference1;
    int Thread_count_difference2;
    int Thread_count_difference3;

    float distThread; //近くの相手の糸の距離
    float dist40;     //近くの自分の陣地ではない木との距離
    float dist50;     //2番目の近くの自分の陣地ではない木との距離
    float distNet;    // ネットとの距離
    float myTreeDist;
    float myTreeDist2;
    float playerDist;
    float m_gauge;             //自分がいる木のゲージ量(-がEnemy、+がPlayer)
    float near_Gauge;          //木のゲージ量
    float near_Gauge2;         //木のゲージ量
    float playerTree_gauge;    //Playerのいる木のゲージ量
    float playerNearDist = 17; //Playerとの距離（近くの範囲）
    float dist0;
    float dist02;
    float difference_gauge;    //今いる木とPlayerのいる木のゲージ量の差

    int m_moveCount;   //歩く時の方向決め
    float wait_time;   //行動停止時間
    float dead_time;   //死んでる時間
    float m_moveTimer; //歩いてる時間
    float m_ground_jump_time;
    
    float attack_wait;         //攻撃行動までの時間
    float attack_timer;        //攻撃のクールタイム
    float attack_time = 45.0f; //攻撃のクールタイム(指定秒数)


    bool m_moveStart = false;
    bool string_Rob = false;
    bool player_onTree;
    bool bodyBlow = true;
    bool noGaugeJump = false;
    bool attack;

    bool speed_attack = false;
    bool tree_dist;

    //検索した木
    GameObject nearObj0;
    GameObject nearObj02;
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
    GameObject playerObj_Tree;

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
    RaycastHit m_hitinfo;
    RaycastHit m_hit;

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

    GroundJumping m_GroundJumping = new GroundJumping();
    Jumping m_Jumping = new Jumping();
    JumpMove m_JumpMove = new JumpMove();
    AttackJump m_AttackJump = new AttackJump();
    AttackJumpMove m_AttackJumpMove = new AttackJumpMove();

    PredominanceDecision m_PredominanceDecision = new PredominanceDecision();

    FallGroundMove m_FallGroundMove = new FallGroundMove();
    Fall m_Fall = new Fall();

    // Use this for initialization
    protected override void Start()
    {
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

        m_GroundJumping.exeDelegate = GroundJumping;
        m_Jumping.exeDelegate = Jumping;
        m_JumpMove.exeDelegate = JumpMove;
        m_AttackJump.exeDelegate = AttackJump;
        m_AttackJumpMove.exeDelegate = AttackJumpMove;

        m_PredominanceDecision.exeDelegate = PredominanceDecision;
        
        m_FallGroundMove.exeDelegate = FallGroundMove;
        m_Fall.exeDelegate = Fall;

        m_StateProcessor.State = m_GroundMove;
    }

    // Update is called once per frame
    protected override void Update()
    {
        //誰の陣地でもない近くの木
        nearObj0 = GetComponent<NearObj>().m_nearObj0;
        //誰の陣地でもない２番目に近くの木
        nearObj02 = GetComponent<NearObj>().m_nearObj02;

        //近かったオブジェクト（木）を取得
        nearObj2 = GetComponent<NearObj>().m_nearObj2;

        //2番目のオブジェクト（木）
        nearObj3 = GetComponent<NearObj>().m_nearObj3;

        //3番目のオブジェクト（木）
        nearObj4 = GetComponent<NearObj>().m_nearObj4;

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

        //自分の糸の色
        myString_number = GetComponent<StringShooter>().m_SideNumber;

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

        //自分のいる木の情報
        if (nearObj != null)
        {
            //自分のいる木のゲージ量
            m_gauge = nearObj.GetComponent<Tree>().m_TerritoryRate;
            //自分のいる木の色
            color_number = nearObj.GetComponent<Tree>().m_SideNumber;
        }
        //playerの情報
        if (playerObj != null)
        {
            playerDist = Vector3.Distance(playerObj.transform.position, this.transform.position);
            player_onTree = true;
            playerString_number = playerObj.GetComponent<StringShooter>().m_SideNumber;
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

        //近くの自分の木の距離
        if (myTreeObj != null)
        {
            myTreeDist = Vector3.Distance(myTreeObj.transform.position, this.transform.position);
        }
        //近くの２番目の自分の木の距離
        if (myTreeObj2 != null)
        {
            myTreeDist2 = Vector3.Distance(myTreeObj2.transform.position, this.transform.position);
        }

        if(attack_timer <= 17)
        {
            attack_timer += Time.deltaTime * 1;
        }


        int treeLayer = LayerMask.GetMask(new string[] { "Tree" });
        Ray ray = new Ray(transform.position + transform.up * 0.5f, -transform.up);
        if (Physics.Raycast(ray, out m_hit, 1f, treeLayer))
        {
            tree_dist = true;
            nearObj = m_hit.transform.gameObject;
        }
        else
        {
            tree_dist = false;
        }

        if (Physics.Raycast(ray, out m_hit, 1f))
        {
            if (m_hit.transform.tag == null)
            {
                dead_time = 1.0f;
                m_StateProcessor.State = m_Fall;
            }
        }

        //EnemyのY軸が0以下になったら
        if (gameObject.transform.position.y <= 0)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, 0.8f, gameObject.transform.position.z);
        }
        if (gameObject.transform.position.y >= 80)
        {
            m_StateProcessor.State = m_Fall;
        }

        //if (Input.GetKeyDown(KeyCode.G))
        //{
        //    m_StateProcessor.State = m_Fall;
        //}

        //Debug.Log(TreeDist());
        //Debug.Log(m_StateProcessor.State);
        Debug.DrawLine(transform.position + transform.up * 0.5f, m_targetPos, Color.blue);

        m_StateProcessor.Execute();
    }




    /*** 地面移動 ***/
    private void GroundMove()
    {
        if(m_randomCount == 0)
        m_randomCount = Random.Range(1, 3);
        m_speed = 5;

        anim.SetBool("move_front", true);

        //歩く目標位置
        if (m_randomCount == 1)
        {
            m_targetPos = GetPosition0();
        }
        if (m_randomCount == 2)
        {
            m_randomCount = 5;
            m_targetPos = GetPosition02();
        }

        
        RaycastHit hit;
        Ray ray = new Ray(transform.position + transform.up * 0.5f, -transform.up);
        if (Physics.Raycast(ray, out hit, 1.5f))
        {
            if (hit.transform.tag == "Ground")
            {
                transform.position = Vector3.Lerp(transform.position, hit.point, 0.2f);
                transform.rotation = Quaternion.LookRotation(
                    Vector3.Lerp(transform.forward, Vector3.Cross(transform.right, hit.normal), 0.3f), hit.normal);
            }
        }

        float dist = Vector3.Distance(nearObj2.transform.position, this.transform.position);
        //木に飛び乗る
        if (dist <= ground_detection)
        {
            anim.SetBool("move_front", false);

            wait_time += Time.deltaTime * 1;
            if (wait_time >= 1)
            {
                m_randomCount = 0;
                m_targetPos = GetPosition3();

                int treeLayer = LayerMask.GetMask(new string[] { "Tree" });
                //移動先と自分の間のray
                if (Physics.Raycast(transform.position, m_targetPos - transform.position, out jump_target, 100f, treeLayer))
                {
                    anim.SetBool("jump", true);
                    jump_start = transform.position;
                    jump_end = jump_target.point;
                } 
                JumpCalculation(jump_start, jump_end, 30.0f);
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
        anim.SetBool("jumpair", false);
        anim.SetBool("avoidance", false);
        anim.SetBool("move_front", true);

        anim.SetBool("Attack", false);
        anim.SetBool("move_back", false);
        anim.SetBool("move_left", false);
        anim.SetBool("move_right", false);

        m_speed = 5;

        RaycastHit hit;
        Ray ray = new Ray(transform.position + transform.up * 0.5f, -transform.up);
        if (Physics.Raycast(ray, out hit, 1.5f))
        {
            if (hit.transform.tag == "Ground")
            {
                transform.position = Vector3.Lerp(transform.position, hit.point, 0.2f);
                transform.rotation = Quaternion.LookRotation(
                    Vector3.Lerp(transform.forward, Vector3.Cross(transform.right, hit.normal), 0.3f), hit.normal);
            }
            else if (hit.transform.tag == "Tree")
            {
                //Playerに当たった時
                if (isBodyblow)
                {
                    down_time = 2.0f;
                    anim.SetBool("dead", true);
                    m_StateProcessor.State = m_Fall;
                    return;
                }

                nearObj = hit.collider.gameObject;

                transform.position = Vector3.Lerp(transform.position, hit.point, 0.2f);
                transform.rotation = Quaternion.LookRotation(
                    Vector3.Lerp(transform.forward, Vector3.Cross(transform.right, hit.normal), 0.3f), hit.normal);

                m_StateProcessor.State = m_TreeDecision;
                return;
            }
        }

        //落下後の移動先(近くの無職の木優先)
        if (m_randomCount != 1 && m_randomCount != 2)
            m_randomCount = Random.Range(1, 3);
        if (m_randomCount == 1 || nearObj == null)
        {
            m_targetPos = GetPosition();
        }
        else if (nearObj != null)
        {
            m_targetPos = new Vector3(nearObj.transform.position.x, nearObj.transform.position.y, nearObj.transform.position.z);
        }

        dead_time += Time.deltaTime * 1;
        if (dead_time >= down_time)//ダウン中の時間
        {
            anim.SetBool("dead", false);
            anim.SetBool("trap", false);

            wait_time += Time.deltaTime * 1;

            if (wait_time >= 1.0f)
            {
                anim.SetBool("move_front", true);

                float dist = Vector3.Distance(nearObj2.transform.position, this.transform.position);
                float dist2 = Vector3.Distance(nearObj.transform.position, this.transform.position);

                if (m_randomCount == 1)
                {
                    //木に飛び乗る
                    if (dist <= ground_detection)
                    {
                        anim.SetBool("move_front", false);

                        m_ground_jump_time += Time.deltaTime * 1;
                        if (m_ground_jump_time >= 1)
                        {
                            m_ground_jump_time = 0;
                            dead_time = 0;
                            m_randomCount = 0;
                            m_targetPos = GetPosition3();
                            jump_start = transform.position;
                            jump_end = jump_target.point;

                            int treeLayer = LayerMask.GetMask(new string[] { "Tree" });
                            //移動先と自分の間のray
                            if (Physics.Raycast(transform.position, m_targetPos - transform.position, out jump_target, 100f, treeLayer))
                            {
                                anim.SetBool("jump", true);
                                jump_start = transform.position;
                                jump_end = jump_target.point;
                            }
                            JumpCalculation(jump_start, jump_end, 30.0f);
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
                else //ジャンプする
                {
                    int treeLayer = LayerMask.GetMask(new string[] { "Tree" });

                    if (dist2 <= 5)
                    {
                        m_ground_jump_time = 0;
                        dead_time = 0;
                        m_randomCount = 0;
                        m_targetPos = new Vector3(nearObj.transform.position.x, 7.0f, nearObj.transform.position.z);
                        jump_start = transform.position;
                        jump_end = jump_target.point;

                        
                        //移動先と自分の間のray
                        if (Physics.Raycast(transform.position, m_targetPos - transform.position, out jump_target, 100f, treeLayer))
                        {
                            anim.SetBool("jump", true);
                            jump_start = transform.position;
                            jump_end = jump_target.point;
                        }
                        JumpCalculation(jump_start, jump_end, 30.0f);
                        m_StateProcessor.State = m_GroundJumping;
                    }
                    else
                    { 
                        //木に飛び乗る
                        if (dist <= ground_detection)
                        {
                            anim.SetBool("move_front", false);

                            m_ground_jump_time += Time.deltaTime * 1;
                            if (m_ground_jump_time >= 1)
                            {
                                m_ground_jump_time = 0;
                                dead_time = 0;
                                m_randomCount = 0;
                                m_targetPos = GetPosition3();
                                jump_start = transform.position;
                                jump_end = jump_target.point;

                                //移動先と自分の間のray
                                if (Physics.Raycast(transform.position, m_targetPos - transform.position, out jump_target, 100f, treeLayer))
                                {
                                    anim.SetBool("jump", true);
                                    jump_start = transform.position;
                                    jump_end = jump_target.point;
                                }
                                JumpCalculation(jump_start, jump_end, 30.0f);
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
        }
    }


    /*** 木にいるときの行動判断 ***/
    private void TreeDecision()
    {
        anim.SetBool("jump", false);
        anim.SetBool("jumpair", false);
        anim.SetBool("avoidance", false);
        anim.SetBool("Attack", false);

        //Playerに当たった時
        if (isBodyblow)
        {
            down_time = 2.0f;
            anim.SetBool("dead", true);
            m_StateProcessor.State = m_Fall;
            return;
        }

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

        //自分の糸の色
        int sidenumber = GetComponent<StringShooter>().m_SideNumber;
        //Playerのいる木を取得
        if (playerObj != null)
            playerObj_Tree = playerObj.GetComponent<Player>().GetOnTree(); 
        if (playerObj_Tree != null)
        {
            //Playerのいる木の色
            player_number = playerObj_Tree.GetComponent<Tree>().m_SideNumber;
            //Playerのいる木のゲージ量
            playerTree_gauge = playerObj_Tree.GetComponent<Tree>().m_TerritoryRate;
        }

        wait_time = 0;

        RaycastHit hit;
        Ray ray = new Ray(transform.position + transform.up * 0.5f, -transform.up);
        int treeLayer = LayerMask.GetMask(new string[] { "Tree" });
        if (Physics.Raycast(ray, out hit, 1.5f, treeLayer))
        {
            nearObj = hit.collider.gameObject;

            if (hit.transform.tag == "Tree")
            {
                transform.position = Vector3.Lerp(transform.position, hit.point, 0.2f);
                transform.rotation = Quaternion.LookRotation(
                    Vector3.Lerp(transform.forward, Vector3.Cross(transform.right, hit.normal), 0.3f), hit.normal);  
            }
        }


        //自分が優勢の時
        if (EnemyTree_Count > PlayerTree_Count)
        {
            m_StateProcessor.State = m_PredominanceDecision;
        }
        else if (playerDist > 25 && playerDist <= player_Detection &&
           player_onTree == playerObj.GetComponent<Player>().IsOnTree()) //Playerに攻撃、playerが木にいるか？
        {
            if (m_randomCount != 1 && m_randomCount != 2 && m_randomCount != 3)
                m_randomCount = Random.Range(1, 4);

            //playerが敵の木にいたら攻撃する
            if (sidenumber == player_number)
            {
                speed_attack = true;
                m_playerTarget = GetPlayerPosition();
                m_StateProcessor.State = m_AttackJump;
            }
            //else
            //{
            //    m_randomCount = 0;
            //    speed_attack = true;
            //    m_playerTarget = GetPlayerPosition();
            //    m_StateProcessor.State = m_AttackJump;
            //}
            else switch (m_randomCount)
                {
                    case 1: //ゲージを溜めてから攻撃
                        m_randomCount = 0;
                        m_playerTarget = GetPlayerPosition();
                        m_StateProcessor.State = m_AttackJump;
                        break;

                    case 2: //ゲージ関係なしに速攻攻撃
                        m_randomCount = 0;
                        speed_attack = true;
                        m_playerTarget = GetPlayerPosition();
                        m_StateProcessor.State = m_AttackJump;
                        break;

                    case 3: //無色の木に飛ぶ
                        m_randomCount = 0;
                        m_StateProcessor.State = m_ColorlessTree;
                        break;
                }
        }
        else if (nearObj0 != null) //無色の木がある場合
        {
            //ゲージで判断するか、糸で判断するか
            if (m_randomCount != 1 && m_randomCount != 2)
                m_randomCount = Random.Range(1, 3);

            if (playerDist <= playerNearDist) //playerが近くにいた場合近くの木に飛ぶ
            {
                m_randomCount = 0;
                m_StateProcessor.State = m_StringCount;
            }
            else if (m_randomCount == 1)
            {
                m_randomCount = 0;
                m_StateProcessor.State = m_ColorlessTree;
            }
            else
            {
                m_randomCount = 0;
                m_StateProcessor.State = m_SearchTreeGauge;
            }

            //近くの２本の木が自身の木だった場合
            if(nearObj2.GetComponent<Tree>().m_SideNumber == myString_number && 
                nearObj3.GetComponent<Tree>().m_SideNumber == myString_number)
            {
                m_randomCount = 0;
                m_StateProcessor.State = m_SearchTreeGauge;
            }

            //if (color_number == playerString_number)//今いる木が相手の木の場合
            //{
            //    noGaugeJump = true;
            //    m_StateProcessor.State = m_ColorlessTree;
            //}
        }
        else if (nearObj40 == null && nearObj50 == null) //青、白の木が近くにある場合
        {
            //ゲージで判断するか、糸で判断するか
            if (m_randomCount != 1 && m_randomCount != 2)
                m_randomCount = Random.Range(1, 3);
            if (m_randomCount == 1)
            {
                m_randomCount = 0;
                m_StateProcessor.State = m_SearchTreeGauge;
            }
            else
            {
                m_randomCount = 0;
                m_StateProcessor.State = m_StringCount;
            }

            //近くの２本の木が自身の木だった場合
            if (nearObj2.GetComponent<Tree>().m_SideNumber == myString_number &&
                nearObj3.GetComponent<Tree>().m_SideNumber == myString_number)
            {
                m_randomCount = 0;
                m_StateProcessor.State = m_SearchTreeGauge;
            }
            //m_StateProcessor.State = m_SearchTree;
        }
        else
        {
            m_StateProcessor.State = m_SearchRandom;
        }

        //今いる木が自分の色だったら
        if(color_number == myString_number)
        {
            noGaugeJump = true;

            //ゲージで判断するか、糸で判断するか
            if (m_randomCount != 1 && m_randomCount != 2)
                m_randomCount = Random.Range(1, 3);
            if (m_randomCount == 1)
            {
                m_randomCount = 0;
                m_StateProcessor.State = m_SearchTreeGauge;
            }
            else
            {
                m_randomCount = 0;
                m_StateProcessor.State = m_StringCount;
            }
        }


        //playerが一定時間同じ木にいたら攻撃
        //if (playerObj.GetComponent<Player>().IsOnTreeTime(5) == true && 
        //    playerDist > 25 && playerDist <= player_Detection &&
        //   player_onTree == playerObj.GetComponent<Player>().IsOnTree())
        //{
        //    if (m_randomCount != 1 && m_randomCount != 2)
        //        m_randomCount = Random.Range(1, 3);
        //    if (m_randomCount == 1)
        //    {
        //        m_randomCount = 0;
        //        m_playerTarget = GetPlayerPosition();
        //        m_StateProcessor.State = m_AttackJump;
        //    }
        //    else
        //    {
        //        m_randomCount = 0;
        //        //m_StateProcessor.State = m_SearchTree;
        //        m_StateProcessor.State = m_SearchTreeGauge;
        //    }
        //}

        
    }

    /*** 木での移動 ***/
    private void TreeMove()
    {
        bool randamStart = true;
        m_speed = 3;

        //Playerに当たった時
        if (isBodyblow)
        {
            down_time = 2.0f;
            anim.SetBool("dead", true);
            m_StateProcessor.State = m_Fall;
            return;
        }

        if (randamStart == true)
        {
            if (m_moveCount != 1 && m_moveCount != 2 && m_moveCount != 3 && m_moveCount != 4)
                m_moveCount = Random.Range(1, 5);

            randamStart = false;
        }

        RaycastHit hit;
        Ray ray = new Ray(transform.position + transform.up * 0.5f, -transform.up);
        int treeLayer = LayerMask.GetMask(new string[] { "Tree" });
        int groundLayer = LayerMask.GetMask(new string[] { "Ground" });

        if (Physics.SphereCast(ray, 0.5f, out hit, 1.5f, groundLayer))
        {
            transform.position = Vector3.Lerp(transform.position, hit.point, 0.2f);
            transform.rotation = Quaternion.LookRotation(
                Vector3.Lerp(transform.forward, Vector3.Cross(transform.right, hit.normal), 0.3f), hit.normal);

            anim.SetBool("move_front", false);
            anim.SetBool("move_back", false);
            anim.SetBool("move_left", false);
            anim.SetBool("move_right", false);
            m_StateProcessor.State = m_FallGroundMove;
        }
        else if(Physics.SphereCast(ray, 0.3f, out hit, 1.5f, treeLayer))
        {
            transform.position = Vector3.Lerp(transform.position, hit.point, 0.2f);
            transform.rotation = Quaternion.LookRotation(
                Vector3.Lerp(transform.forward, Vector3.Cross(transform.right, hit.normal), 0.3f), hit.normal);

            nearObj = hit.collider.gameObject;

            if(hit.collider.gameObject == null)
            {
                down_time = 1.0f;
                m_StateProcessor.State = m_Fall;
            }
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
        if(nearObj0 != null)
        {
            near_Gauge = nearObj0.GetComponent<Tree>().m_TerritoryRate;
            dist0 = Vector3.Distance(nearObj0.transform.position, this.transform.position);
        }
        
        if(nearObj02 != null)
        {
            near_Gauge2 = nearObj02.GetComponent<Tree>().m_TerritoryRate;
            dist02= Vector3.Distance(nearObj02.transform.position, this.transform.position);
        }
        

        if (playerDist <= playerNearDist && player_onTree == playerObj.GetComponent<Player>().IsOnTree())
            noGaugeJump = true;

        if (dist0 <= tree_Detection && nearObj0 != null && nearObj02 != null)
        {
            if(near_Gauge <= 1 || nearObj02 != null)
            {
                eyeObj = nearObj0;
                m_targetPos = GetUpPosition00();
                m_StateProcessor.State = m_Jumping;
            }
            else if(near_Gauge2 <= 1 && dist02 <= tree_Detection)
            {
                eyeObj = nearObj02;
                m_targetPos = GetUpPosition02();
                m_StateProcessor.State = m_Jumping;
            }
            else //相手のゲージが少ない方
            {
                if(near_Gauge2 >= near_Gauge && dist0 <= tree_Detection)
                {
                    eyeObj = nearObj0;
                    m_targetPos = GetUpPosition00();
                    m_StateProcessor.State = m_Jumping;
                }
                if(near_Gauge >= near_Gauge2 && dist02 <= tree_Detection)
                {
                    eyeObj = nearObj02;
                    m_targetPos = GetUpPosition02();
                    m_StateProcessor.State = m_Jumping;
                }
                else
                {
                    m_StateProcessor.State = m_SearchTree;
                }
            }
        }
        else
        {
            m_StateProcessor.State = m_SearchTree;
        }
    }

    /*** 近くの木の付いている糸で判別 ***/
    private void StringCount()
    {
        int sidenumber = GetComponent<StringShooter>().m_SideNumber;
        //GameObject player;
        //player = GameObject.FindWithTag("Player");
        //int playerNumber = player.GetComponent<StringShooter>().m_SideNumber;
        Thread_count_difference1 = count2[1] - count2[sidenumber]; //近くの木
        Thread_count_difference2 = count3[1] - count3[sidenumber]; //2番目の木
        Thread_count_difference3 = count4[1] - count4[sidenumber]; //2番目の木

        //playerと敵の糸の本数差（２本以内、playerの方が多かったら）
        if (Thread_count_difference1 >= -2 && Thread_count_difference1 <= 2 && count2[sidenumber] < count2[1]) //近くの木の糸の本数が２本以下
        {
            eyeObj = nearObj2;
            //近い木に飛ぶ
            m_targetPos = GetUpPosition2();
            m_StateProcessor.State = m_Jumping;
        }
        else if (Thread_count_difference2 >= -2 && Thread_count_difference2 <= 2 && count3[sidenumber] < count3[1]) //２番目の近くの木の糸の本数が２本以下
        {
            eyeObj = nearObj3;
            //２番目の近くの木に飛ぶ
            m_targetPos = GetUpPosition3();
            m_StateProcessor.State = m_Jumping;
        }
        else if (Thread_count_difference3 >= -2 && Thread_count_difference3 <= 2 && count4[sidenumber] < count4[1]) //３番目の近くの木の糸の本数が２本以下
        {
            eyeObj = nearObj4;
            //３番目の近くの木に飛ぶ
            m_targetPos = GetUpPosition4();
            m_StateProcessor.State = m_Jumping;
        }
        else
        {
            m_StateProcessor.State = m_SearchTree;
        }
    }

    /*** 近くの自分の木＋ゲージ量  必要ないかもしれない ***/
    private void SearchMyTreeGauge()
    {
        if(myTreeObj != null)
            near_Gauge = myTreeObj.GetComponent<Tree>().m_TerritoryRate;
        if(myTreeObj2 != null)
            near_Gauge2 = myTreeObj2.GetComponent<Tree>().m_TerritoryRate;

        if (near_Gauge >= near_Gauge2)
        {
            if (myTreeObj != null && myTreeDist <= tree_Detection)
            {
                eyeObj = myTreeObj;
                m_targetPos = MyTreePosition1();
                m_StateProcessor.State = m_Jumping;
            }
            else
            {
                m_StateProcessor.State = m_SearchTree;
            }
        }
        if(near_Gauge2 >= near_Gauge)
        {
            if (myTreeObj2 != null && myTreeDist2 <= tree_Detection)
            {
                eyeObj = myTreeObj;
                m_targetPos = MyTreePosition2();
                m_StateProcessor.State = m_Jumping;
            }
            else
            {
                m_StateProcessor.State = m_SearchTree;
            }
        }
    }
    /*** 近くの相手の木＋ゲージ量  必要ないかもしれない***/
    private void SearchTreeGauge()
    {
        if(nearObj40 != null)
        near_Gauge = nearObj40.GetComponent<Tree>().m_TerritoryRate;
        if(nearObj50 != null)
        near_Gauge2 = nearObj50.GetComponent<Tree>().m_TerritoryRate;

        if (playerDist <= playerNearDist && player_onTree == playerObj.GetComponent<Player>().IsOnTree())
            noGaugeJump = true;

        if (near_Gauge <= near_Gauge2)
        {
            if (nearObj40 != null && dist40 <= tree_Detection)
            {
                eyeObj = nearObj40;
                m_targetPos = GetUpPosition40();
                m_StateProcessor.State = m_Jumping;
            }
            else
            {
                m_StateProcessor.State = m_SearchTree;
            }
        }
        if (near_Gauge2 <= near_Gauge)
        {
            if (nearObj50 != null && dist50 <= tree_Detection)
            {
                eyeObj = nearObj50;
                m_targetPos = GetUpPosition40();
                m_StateProcessor.State = m_Jumping;
            }
            else
            {
                m_StateProcessor.State = m_SearchTree;
            }
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

        switch (m_randomCount)
        {
            case 4:
                if (dist40 <= tree_Detection)
                {
                    if (nearObj40 != null)
                    {
                        eyeObj = nearObj40;
                        m_randomCount = 0;
                        m_targetPos = GetUpPosition40();
                        m_StateProcessor.State = m_Jumping;
                        return;
                    }
                    else if (nearObj40 == null)
                    {
                        m_randomCount = 0;
                        m_StateProcessor.State = m_SearchRandom;
                        return;
                    }
                }
                else
                {
                    m_randomCount = 0;
                    m_StateProcessor.State = m_SearchRandom;
                    return;
                }
                break;

            case 5:
                if (dist50 <= tree_Detection)
                {
                    if (nearObj50 != null)
                    {
                        eyeObj = nearObj50;
                        m_randomCount = 0;
                        m_targetPos = GetUpPosition50();
                        m_StateProcessor.State = m_Jumping;
                        return;
                    }
                    else if (nearObj50 == null)
                    {
                        m_randomCount = 0;
                        m_StateProcessor.State = m_SearchRandom;
                        return;
                    }
                }
                else
                {
                    m_randomCount = 0;
                    m_StateProcessor.State = m_SearchRandom;
                    return;
                }
                break;
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



    /*** 地面からジャンプ ***/
    private void GroundJumping()
    {
        wait_time = 0;

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
        //Playerに当たった時
        if (isBodyblow)
        {
            down_time = 2.0f;
            anim.SetBool("dead", true);
            m_StateProcessor.State = m_Fall;
            return;
        }

        jump_random = 0;

        int treeLayer = LayerMask.GetMask(new string[] { "Tree" });

        Ray ray = new Ray(transform.position + transform.up * 0.5f, -transform.up);
        if (Physics.Raycast(ray, out m_hit, 2f, treeLayer))
        {
            m_hitinfo = m_hit;
            nearObj = m_hit.collider.gameObject;
        }

        //移動先と自分の間のray
        if (Physics.Raycast(transform.position + transform.up * 0.5f, m_targetPos - transform.position, out jump_target, tree_Detection, treeLayer))
        {
            jump_start = transform.position;
            jump_end = jump_target.point;

            if (jump_target.transform != eyeObj.transform)
            {
                if(walk_count <= 3)
                {
                    walk_count += 1;
                    m_randomCount = 0;
                    m_StateProcessor.State = m_TreeMove;
                }
                else
                {
                    walk_count = 0;
                    m_randomCount = 0;
                    m_StateProcessor.State = m_SearchRandom;
                }
            }
            else if (jump_target.transform == eyeObj.transform) //飛びたいところの間に障害物がなければ
            { 
                if (m_gauge <= thought_Gauge || color_number == myString_number)　//ゲージをためる、今いる木が自分の木なら
                {
                    //1秒後に攻撃
                    wait_time += Time.deltaTime * 1;
                    if (wait_time >= jump_wait)
                    {
                        m_randomCount = 0;
                        walk_count = 0;
                        string_Rob = true;
                        anim.SetBool("jump", true);
                        JumpCalculation(jump_start, jump_end, 30.0f);
                        m_hitinfo.collider.GetComponent<Tree>().m_TerritoryRate += JumpDemeritRate - 5; //ジャンプでのゲージの減り量
                        m_StateProcessor.State = m_JumpMove;
                        return;
                    }
                }

                
                //playerが近くにいた場合逃げる行為
                if (playerDist <= playerNearDist && player_onTree == playerObj.GetComponent<Player>().IsOnTree() &&
                    playerObj.GetComponent<Player>().IsAttack() == false)
                {
                    //1秒後に攻撃
                    wait_time += Time.deltaTime * 1;
                    if (wait_time >= jump_wait)
                    {
                        wait_time = 0;
                        m_randomCount = 0;
                        m_StateProcessor.State = m_SearchRandom;
                    }
                }

                //ゲージ関係なしにジャンプ
                if (noGaugeJump == true)
                {
                    //1秒後に攻撃
                    wait_time += Time.deltaTime * 1;
                    if (wait_time >= jump_wait)
                    {
                        noGaugeJump = false;
                        string_Rob = true;
                        walk_count = 0;
                        anim.SetBool("jump", true);
                        JumpCalculation(jump_start, jump_end, 30.0f);
                        m_hitinfo.collider.GetComponent<Tree>().m_TerritoryRate += JumpDemeritRate - 5; //ジャンプでのゲージの減り量
                        m_StateProcessor.State = m_JumpMove;
                    }
                }
            }
            else
            {
                m_StateProcessor.State = m_TreeDecision;
            }

            //Playerに攻撃、playerが木にいるか？
            if (playerDist > 25 && playerDist <= player_Detection &&
                player_onTree == playerObj.GetComponent<Player>().IsOnTree())
            {
                int sidenumber = GetComponent<StringShooter>().m_SideNumber;

                if (playerObj != null)
                    playerObj_Tree = playerObj.GetComponent<Player>().GetOnTree();
                if (playerObj_Tree != null)
                    player_number = playerObj_Tree.GetComponent<Tree>().m_SideNumber;

                //ランダムで攻撃か、木に飛ぶか決める
                if (m_randomCount != 1 && m_randomCount != 2)
                    m_randomCount = Random.Range(1, 3);

                //playerが赤い木にいるか？
                if (sidenumber == player_number)
                {
                    m_randomCount = 0;
                    speed_attack = true;
                    m_playerTarget = GetPlayerPosition();
                    m_StateProcessor.State = m_AttackJump;
                }
                //攻撃のクールタイムが45秒経ったら
                else if (attack_timer >= attack_time)
                {
                    attack_timer = 0;
                    speed_attack = true;
                    m_playerTarget = GetPlayerPosition();
                    m_StateProcessor.State = m_AttackJump;
                }       
                else
                {
                    m_randomCount = 0;
                    m_playerTarget = GetPlayerPosition();
                    m_StateProcessor.State = m_AttackJump;
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
        wait_time = 0;

        speed_attack = false;

        if (Projection(jump_start, jump_end, jump_target.normal, 30.0f))
        {
            transform.position = jump_end;
            m_Shooter.StringShoot(jump_start, jump_end);
            //m_hitinfo.collider.GetComponent<Tree>().m_TerritoryRate += Vector3.Distance(jump_start, jump_end);
            m_StateProcessor.State = m_TreeDecision;
        }

        int sidenumber = GetComponent<StringShooter>().m_SideNumber;

        //糸を奪う
        if (distThread >= 0.5f && distThread <= 2 || distNet >= 0.5f && distNet <= 2)
        {
            //奪う確率
            if (string_Rob == true)
            {
                netCount = Random.Range(1, 11);
                string_Rob = false;
            }

            if (netCount <= m_netrob) //失敗したとき
            {
                m_StateProcessor.State = m_Fall;
                return;
            }
            else //成功したとき
            {
                anim.SetBool("avoidance", true);

                stringObj1.GetComponent<StringUnit>().SideUpdate(sidenumber);
                stringNet.GetComponent<Net>().SideUpdate(sidenumber);

                //AnimatorStateInfo animInfo = anim.GetCurrentAnimatorStateInfo(0);
                //if (animInfo.normalizedTime < 1.0f)
                //{
                //    anim.SetBool("avoidance", false);
                //}
            }
        }
    }

    /*** 攻撃ジャンプ ***/
    private void AttackJump()
    {
        //Playerに当たった時
        if (isBodyblow)
        {
            down_time = 2.0f;
            anim.SetBool("dead", true);
            m_StateProcessor.State = m_Fall;
            return;
        }
        //攻撃のクールタイムが15秒経ったら
        if (attack_timer >= attack_time)
        {
            attack_timer = 0;
            speed_attack = true;
        }

        jump_random = 0;
        bodyBlow = true;

        int treeLayer = LayerMask.GetMask(new string[] { "Tree" });
        Ray ray = new Ray(transform.position + transform.up * 0.5f, -transform.up);
        if (Physics.Raycast(ray, out m_hit, 1f, treeLayer))
        {
            m_hitinfo = m_hit;
            nearObj = m_hit.collider.gameObject;
        }

        //移動先と自分の間のray
        if (Physics.Raycast(transform.position + transform.up * 0.5f, m_playerTarget - transform.position, out jump_target, tree_Detection))
        {
            jump_start = transform.position;
            jump_end = jump_target.point;

            if (jump_target.transform != playerObj.transform)
            {
                if (walk_count <= 3)
                {
                    walk_count += 1;
                    attack = false;
                    attack_wait = 0;
                    m_StateProcessor.State = m_TreeMove;
                }
                else
                {
                    walk_count = 0;
                    attack = false;
                    attack_wait = 0;
                    m_StateProcessor.State = m_SearchRandom;
                }
            }
            else if (jump_target.transform == playerObj.transform && playerDist <= player_Detection) //飛びたいところの間に障害物がなければ
            {
                //ゲージをためる || 今いる木が赤い木なら || speed_attackがtureなら
                if (m_gauge <= thought_Gauge || color_number == myString_number || speed_attack == true)
                {
                    attack = true;
      
                    //攻撃ジャンプまでの時間
                    if(attack_wait == 0)
                    {
                        attack_wait = Random.Range(0.5f, 1.5f);
                    }

                    //1秒後に攻撃
                    wait_time += Time.deltaTime * 1;
                    if (wait_time >= attack_wait && player_onTree == playerObj.GetComponent<Player>().IsOnTree())
                    {
                        string_Rob = true;
                        walk_count = 0;
                        anim.SetBool("jump", true);    
                        JumpCalculation(jump_start, jump_end, 30.0f);
                        m_hitinfo.collider.GetComponent<Tree>().m_TerritoryRate += JumpDemeritRate - 5; //ジャンプでのゲージの減り量
                        m_StateProcessor.State = m_AttackJumpMove;
                        return;
                    }
                    else if(wait_time >= attack_wait + 0.5f)
                    {
                        string_Rob = true;
                        m_StateProcessor.State = m_TreeDecision;
                    }
                }
                //playerが近くにいた場合
                if (playerDist <= playerNearDist && player_onTree == playerObj.GetComponent<Player>().IsOnTree() &&
                    playerObj.GetComponent<Player>().IsAttack() == false)
                {
                    attack = false;
                    attack_wait = 0;
                    //if (nearObj0 != null)
                    //{
                    //    m_StateProcessor.State = m_ColorlessTree;
                    //}
                    //else
                    //{
                    //    m_StateProcessor.State = m_SearchTreeGauge;
                    //}

                    m_StateProcessor.State = m_SearchRandom;
                }
            }
            else if (playerDist >= player_Detection)
            {
                attack = false;
                attack_wait = 0;
                m_StateProcessor.State = m_SearchTreeGauge;
            }
        }
        else
        {
            attack = false;
            attack_wait = 0;
            m_StateProcessor.State = m_TreeMove;
        }
    }

    /*** 攻撃ジャンプ移動中 ***/
    private void AttackJumpMove()
    {
        speed_attack = false;
        wait_time = 0;

        if (playerDist <= 3) //攻撃
        {
            anim.SetBool("Attack", true);
            if (bodyBlow == true)
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

        int treeLayer = LayerMask.GetMask(new string[] { "Tree" });

        Ray ray = new Ray(transform.position + transform.up * 0.5f, -transform.up);
        if (Physics.SphereCast(ray, 1, out m_hit, 2f, treeLayer))
        {
            transform.position = Vector3.Lerp(transform.position, m_hit.point, 0.2f);
            transform.rotation = Quaternion.LookRotation(
                Vector3.Lerp(transform.forward, Vector3.Cross(transform.right, m_hit.normal), 0.3f), m_hit.normal);
        }

        if (Projection(jump_start, jump_end, jump_target.normal, 30.0f))
        {
            attack = false;
            transform.position = jump_end;
            //m_Shooter.StringShoot(jump_start, jump_end);
            m_StateProcessor.State = m_TreeDecision;
        }

        int sidenumber = GetComponent<StringShooter>().m_SideNumber;

        //糸を奪う
        if (distThread >= 0.5f && distThread <= 2 || distNet >= 0.5f && distNet <= 2)
        {
            //奪う確率
            if (string_Rob == true)
            {
                netCount = Random.Range(1, 11);
                string_Rob = false;
            }

            if (netCount <= m_netrob) //失敗したとき
            {
                m_StateProcessor.State = m_Fall;
                return;
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
                stringNet.GetComponent<Net>().SideUpdate(sidenumber);
            }
        }
    }

    //攻撃しようとしているか？
    public bool AttackPreparation()
    {
        return attack;
    }
    //敵が木にいるか？
    public bool TreeDist()
    {
        return tree_dist;
    }



    /*** 優勢時の行動判断 ***/
    private void PredominanceDecision()
    {
        anim.SetBool("jump", false);
        anim.SetBool("jumpair", false);
        anim.SetBool("avoidance", false);
        anim.SetBool("Attack", false);

        wait_time = 0;

        //今いる木とPlayerのいる木のゲージ量の差
        if (nearObj != null && playerObj_Tree != null)
        {
            difference_gauge = playerTree_gauge - m_gauge;
        }
        //Playerに当たった時
        if (isBodyblow)
        {
            down_time = 2.0f;
            anim.SetBool("dead", true);
            m_StateProcessor.State = m_Fall;
            return;
        }

        if(jump_random != 1 && jump_random != 2)
        jump_random = Random.Range(1, 3);

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
            if(hit.transform.tag == null)
            {
                dead_time = 1.0f;
                m_StateProcessor.State = m_Fall;
            }
        }


        //自身が劣勢の時
        if (EnemyTree_Count < PlayerTree_Count)
        {
            m_StateProcessor.State = m_TreeDecision;

        }
        else if (playerDist > 25 && playerDist <= player_Detection && 
            player_onTree == playerObj.GetComponent<Player>().IsOnTree()) //Playerに攻撃
        {
            int sidenumber = GetComponent<StringShooter>().m_SideNumber;
            if (playerObj != null)
                playerObj_Tree = playerObj.GetComponent<Player>().GetOnTree();
            if (playerObj_Tree != null)
            {
                //Playerのいる木の色
                player_number = playerObj_Tree.GetComponent<Tree>().m_SideNumber;
                //Playerのいる木のゲージ量
                playerTree_gauge = playerObj_Tree.GetComponent<Tree>().m_TerritoryRate;
            }

            if (m_randomCount != 1 && m_randomCount != 2)
                m_randomCount = Random.Range(1, 3); 
            if(m_randomCount == 1)
            {
                m_randomCount = 0;
                //playerが赤い木にいたら
                if (sidenumber == player_number)
                {
                    speed_attack = true;
                }
                m_playerTarget = GetPlayerPosition();
                m_StateProcessor.State = m_AttackJump;
            }
            else if (difference_gauge >= 0)
            {
                m_randomCount = 0;
                speed_attack = true;
                m_playerTarget = GetPlayerPosition();
                m_StateProcessor.State = m_AttackJump;
            }
            else
            {
                m_randomCount = 0;
                speed_attack = true;
                m_playerTarget = GetPlayerPosition();
                m_StateProcessor.State = m_AttackJump;
                //m_StateProcessor.State = m_SearchTreeGauge;
            }
        }
        else if (nearObj0 != null) //無色の木
        {
            //ゲージで判断するか、糸で判断するか
            if (m_randomCount != 1 && m_randomCount != 2)
                m_randomCount = Random.Range(1, 3);
            if (playerDist <= playerNearDist) //playerが近くにいた場合近くの木に飛ぶ
            {
                m_randomCount = 0;
                m_StateProcessor.State = m_SearchRandom;
            }
            else if (m_randomCount == 1)
            {
                m_randomCount = 0;
                m_StateProcessor.State = m_ColorlessTree;
            }   
            else
            {
                m_randomCount = 0;
                m_StateProcessor.State = m_SearchTreeGauge;
            }
        }
        else if (nearObj40 == null && nearObj50 == null)
        {
            //ゲージで判断するか、糸で判断するか
            if (m_randomCount != 1 && m_randomCount != 2)
            m_randomCount = Random.Range(1, 3);
            if(m_randomCount == 1)
            {
                m_randomCount = 0;
                m_StateProcessor.State = m_SearchTreeGauge;
            }
            else
            {
                m_randomCount = 0;
                m_StateProcessor.State = m_StringCount;
            }
        }
        else
        {
            m_StateProcessor.State = m_SearchRandom;
        }

        //今いる木が自分の色だったら
        if (color_number == myString_number)
        {
            noGaugeJump = true;

            //ゲージで判断するか、糸で判断するか
            if (m_randomCount != 1 && m_randomCount != 2)
                m_randomCount = Random.Range(1, 3);
            if (m_randomCount == 1)
            {
                m_randomCount = 0;
                m_StateProcessor.State = m_SearchTreeGauge;
            }
            else
            {
                m_randomCount = 0;
                m_StateProcessor.State = m_StringCount;
            }
        }
    }



    //落下
    private void Fall()
    {
        speed_attack = false;
        attack = false;
        m_randomCount = 0;

        AnimatorStateInfo animInfo = anim.GetCurrentAnimatorStateInfo(0);
        anim.SetBool("move_front", false);
        anim.SetBool("move_back", false);
        anim.SetBool("move_left", false);
        anim.SetBool("move_right", false);

        anim.SetBool("trap", true);
        if (animInfo.normalizedTime < 1.0f)
        {
            anim.SetBool("dead", true);
        }

        int treeLayer = LayerMask.GetMask(new string[] { "Tree" });
        int groundLayer = LayerMask.GetMask(new string[] { "Ground" });

        //落下先情報取得（木を優先）
        Ray ray = new Ray(transform.position + transform.up * 1.0f, -Vector3.up);
        if (!Physics.Raycast(ray, out jump_target, 100f, treeLayer))
            Physics.Raycast(ray, out jump_target, 100f, groundLayer);

        //落下スピード
        Vector3 fallSpeed = Physics.gravity.y * Vector3.up;
        transform.Translate(fallSpeed * Time.deltaTime, Space.World);
        Vector3 forward = Vector3.Cross(transform.right, jump_target.normal);
        transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.forward, forward, 0.3f), jump_target.normal);

        RaycastHit hit;
        Ray ray2 = new Ray(transform.position + transform.up * 1.5f, -transform.up);
        if (Physics.Raycast(ray2, out hit, 2.5f,groundLayer))
        {
            transform.position = Vector3.Lerp(transform.position, hit.point, 0.2f);
            transform.rotation = Quaternion.LookRotation(
                Vector3.Lerp(transform.forward, Vector3.Cross(transform.right, hit.normal), 0.3f), hit.normal);

            anim.SetBool("jump", false);
            ResetBodyblow();
            m_StateProcessor.State = m_FallGroundMove;
        }
        //m_StateProcessor.State = m_FallingMove;
    }


    private void OnTriggerEnter(Collider col)
    {
        if (col.transform.tag == "Tree")
        {
            m_randomCount = 0;

            reObj2 = col.transform.gameObject;
            nearObj = col.transform.gameObject;
        }

        if (col.transform.tag == "Ground")
        {
            ResetBodyblow();
            attack = false;
            attack_wait = 0;
            if (m_StateProcessor.State != m_GroundMove)
                m_StateProcessor.State = m_FallGroundMove;
        }
    }

    //private void OnTriggerStay(Collider col)
    //{
    //    if (col.transform.tag != "Tree" && m_StateProcessor.State != m_GroundMove &&
    //        m_StateProcessor.State != m_FallGroundMove && m_StateProcessor.State != m_GroundJumping &&
    //        m_StateProcessor.State != m_JumpMove && m_StateProcessor.State != m_AttackJumpMove)
    //    {
    //        down_time = 1.0f;
    //        m_StateProcessor.State = m_Fall;
    //    }
    //}

    private void OnTriggerExit(Collider col)
    {
        if (col.transform.tag == "Tree")
        {
            if (m_StateProcessor.State == m_TreeMove)
            {
                down_time = 1.0f;
                anim.SetBool("jump", true);
                m_StateProcessor.State = m_Fall;
            }
        }
    }
}
