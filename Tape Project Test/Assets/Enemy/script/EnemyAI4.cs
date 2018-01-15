using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyState;

public class EnemyAI4 : Character
{

    [SerializeField]
    float m_speed = 3f;

    public StringShooter m_Shooter;

    private Vector3 jump_start;
    private Vector3 jump_end;
    private RaycastHit jump_target;         //ジャンプの対象
    [Header("地面から跳べる木の探知範囲")]
    public float ground_detection = 30.0f;
    [Header("木の探知範囲")]
    public float tree_Detection = 150.0f;
    [Header("Playerの探知範囲")]
    public float player_Detection = 100.0f;

    [Header("Enemyの思考時間（通常時）")]
    public float thought_Time = 2.0f;
    [Header("Enemyの思考時間（後半時）")]
    public float latterHalf_Thought_Time = 1.0f;
    [Header("Enemyの思考時間（優勢時）")]
    public float predominance_Thought_Time = 2.0f;
    //糸を奪う失敗する確率(1～10)
    int m_netrob = 4;
    //糸を奪う優勢時の失敗する確率(1～10)
    int m_predominanceNetRob = 5;
    //糸を奪う後半時の失敗する確率(1～10)
    int m_latterHalfNetRob = 3;

    [Header("後半状態になるまでの時間(秒)")]
    public float latter_half_time = 45.0f;
    float time_limit;

    int m_randomCount;
    int startRan;
    int treeObj = 0;
    int netCount;
    int PlayerTree_Count;
    int EnemyTree_Count;

    int p_Thread_count_difference1;
    int p_Thread_count_difference2;

    int col_number;

    float distThread;
    float dist40;
    float dist50;
    float distNet;
    float myTreeDist;
    float playerDist;

    int m_moveCount;
    float wait_time;
    float dead_time;
    float m_moveTimer;
    float predominance_time;
    float m_ground_jump_time;
    bool m_moveStart = false;
    bool net_bool = false;
    bool dead_bool = false;
    bool player_onTree;

    float angle;

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

    GameObject eyeObj;

    GameObject myStringObj;
    GameObject stringObj1;
    GameObject stringNet;

    GameObject playerObj;

    [System.NonSerialized]
    public GameObject reObj;
    GameObject reObj2;

    Vector3 m_targetPos;

    List<int> count2;
    List<int> count3;

    List<int> mytreecount1;
    List<int> mytreecount2;

    Animator anim;

    StateProcessor m_StateProcessor = new StateProcessor();
    GroundMove m_GroundMove = new GroundMove();
    TreeDecision m_TreeDecision = new TreeDecision();
    TreeMove m_TreeMove = new TreeMove();
    ColorlessTree m_ColorlessTree = new ColorlessTree();
    SearchTree m_SearchTree = new SearchTree();
    SearchRandom m_SearchRandom = new SearchRandom();
    StringCount m_StringCount = new StringCount();
    Jumping m_Jumping = new Jumping();
    GroundJumping m_GroundJumping = new GroundJumping();
    JumpMove m_JumpMove = new JumpMove();

    ThreadJump m_ThreadJump = new ThreadJump();
    StringMove m_StringMove = new StringMove();
    StringJump m_StringJump = new StringJump();
    StringDecision m_StringDecision = new StringDecision();
    ThreadSearchTree m_ThreadSearchTree = new ThreadSearchTree();
    StringJumping m_StringJumping = new StringJumping();
    ThreadSearchRandom m_ThreadSearchRamdom = new ThreadSearchRandom();

    PredominanceDecision m_PredominanceDecision = new PredominanceDecision();
    PredominanceStringCount m_PredominanceStringCount = new PredominanceStringCount();
    PredominanceMyTree m_PredominanceMyTree = new PredominanceMyTree();
    PredominanceJump m_PredominanceJump = new PredominanceJump();
    PredominanceJumpMove m_PredominanceJumpMove = new PredominanceJumpMove();

    LatterHalfDecision m_LatterHalfDecision = new LatterHalfDecision();
    LatterHalfColorlessTree m_LatterHalfColorlessTree = new LatterHalfColorlessTree();

    AttackJump m_AttackJump = new AttackJump();
    AttackJumpMove m_AttackJumpMove = new AttackJumpMove();
    AttackRearJump m_AttackRearJump = new AttackRearJump();
    AttackRearJumpMove m_AttackRearJumpMove = new AttackRearJumpMove();

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
        m_Jumping.exeDelegate = Jumping;
        m_GroundJumping.exeDelegate = GroundJumping;
        m_JumpMove.exeDelegate = JumpMove;

        m_ThreadJump.exeDelegate = ThreadJump;
        m_StringMove.exeDelegate = StringMove;  
        m_StringJump.exeDelegate = StringJump;
        m_StringDecision.exeDelegate = StringDecision;
        m_ThreadSearchTree.exeDelegate = ThreadSearchTree;
        m_StringJumping.exeDelegate = StringJumping;
        m_ThreadSearchRamdom.exeDelegate = ThreadSearchRandom;

        m_PredominanceDecision.exeDelegate = PredominanceDecision;
        m_PredominanceStringCount.exeDelegate = PredominanceStringCount;
        m_PredominanceMyTree.exeDelegate = PredominanceMyTree;
        m_PredominanceJump.exeDelegate = PredominanceJump;
        m_PredominanceJumpMove.exeDelegate = PredominanceJumpMove;

        m_LatterHalfDecision.exeDelegate = LatterHalfDecision;
        m_LatterHalfColorlessTree.exeDelegate = LatterHalfColorlessTree;

        m_AttackJump.exeDelegate = AttackJump;
        m_AttackJumpMove.exeDelegate = AttackJumpMove;
        m_AttackRearJump.exeDelegate = AttackRearJump;
        m_AttackRearJumpMove.exeDelegate = AttackRearJumpMove;

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
        if (nearObj3 == null) return;

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

        //近くの自分の糸
        myStringObj = GetComponent<NearObj>().m_myStringObj;

        //近くの相手の糸
        stringObj1 = GetComponent<NearObj>().m_stringObj1;

        //近くの相手のネット
        stringNet = GetComponent<NearObj>().m_stringNet;

        //Player
        playerObj = GameObject.FindGameObjectWithTag("Player");

        //相手の木の糸の数
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

        //自分の木の糸の数
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
                mytreecount1[item.m_SideNumber]++;
            }
        }

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


        //経過時間
        if (latter_half_time + 1 >= time_limit)
        {
            time_limit += Time.deltaTime * 1;
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
        //Debug.Log(distNet);
        //Debug.Log(m_StateProcessor.State);
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

        //自分が優勢の時
        if (EnemyTree_Count > PlayerTree_Count)
        {
            m_StateProcessor.State = m_PredominanceDecision;
        }
        else if(latter_half_time <= time_limit) //後半時
        {
            m_StateProcessor.State = m_LatterHalfDecision;
        }  
        else if (nearObj0 != null)
        {
            wait_time += Time.deltaTime * 1;
            if (wait_time >= thought_Time)
            {
                m_StateProcessor.State = m_ColorlessTree;
            }     
        }
        else if (playerDist >= 10 && playerDist <= player_Detection) //Playerに攻撃
        {
            wait_time += Time.deltaTime * 1;
            if (wait_time >= thought_Time)
            {
                if (player_onTree == playerObj.GetComponent<Player>().IsOnTree())
                {
                    if (playerDist <= player_Detection * 2 / 3)  //設定距離の以下の距離だったら
                    {
                        if (m_randomCount == 0)
                            m_randomCount = Random.Range(1, 3);
                        if (m_randomCount == 2)
                        {
                            m_randomCount = 0;
                            m_targetPos = GetPlayerPosition();
                            m_StateProcessor.State = m_AttackJump;
                        }
                        else
                        {
                            m_randomCount = 0;
                            m_targetPos = GetPlayerPosition();
                            m_StateProcessor.State = m_SearchRandom;
                        }
                    }
                    else
                    {
                        if (m_randomCount == 0)
                            m_randomCount = Random.Range(1, 4);
                        if (m_randomCount == 3)
                        {
                            m_randomCount = 0;
                            m_targetPos = GetPlayerPosition();
                            m_StateProcessor.State = m_AttackJump;
                        }
                        else
                        {
                            m_randomCount = 0;
                            m_targetPos = GetPlayerPosition();
                            m_StateProcessor.State = m_SearchRandom;
                        }
                    }
                }
                else
                {
                    m_randomCount = 0;
                    m_targetPos = GetPlayerPosition();
                    m_StateProcessor.State = m_SearchRandom;
                }
            }
        }
        //else if (dist40 >= tree_Detection && dist50 >= tree_Detection)
        //{
        //    m_treeLeave = Random.Range(1, 6);

        //    if (m_treeLeave == 1 || m_treeLeave == 2)
        //    {
        //        m_StateProcessor.State = m_StringCount;
        //    }

        //    //ジャンプ距離内に糸があり、自分の陣地ではない木がない場合
        //    if (m_treeLeave == 3 || m_treeLeave == 4 || m_treeLeave == 5)
        //        if (distThread <= tree_Detection)
        //            m_StateProcessor.State = m_ThreadJump;
        //        else
        //        {
        //            m_StateProcessor.State = m_SearchRandom;
        //        }
        //}
        else if (nearObj40 == null && nearObj50 == null)
        {
            wait_time += Time.deltaTime * 1;
            if (wait_time >= thought_Time)
            {
                m_StateProcessor.State = m_StringCount;
            }
        }
        else
        {
            wait_time += Time.deltaTime * 1;
            if (wait_time >= thought_Time)
            {
                m_StateProcessor.State = m_SearchTree;
            }
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
        if (Physics.Raycast(ray, out hit, 1f, treeLayer))
        {
            if (hit.transform.tag == "Tree")
            {
                transform.position = Vector3.Lerp(transform.position, hit.point, 0.2f);
                transform.rotation = Quaternion.LookRotation(
                    Vector3.Lerp(transform.forward, Vector3.Cross(transform.right, hit.normal), 0.3f), hit.normal);
            }
            if(hit.transform.gameObject == null)
            {
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

            if (m_moveTimer >= 2)
            {
                m_moveCount = 0;

                anim.SetBool("move_front", false);
                anim.SetBool("move_back", false);
                anim.SetBool("move_left", false);
                anim.SetBool("move_right", false);
            }
            if (m_moveTimer >= 2.5f)
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
            jump_start = this.transform.position;
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
                    jump_start = this.transform.position;
                    m_targetPos = GetUpPosition40();
                    m_StateProcessor.State = m_Jumping;
                }
                else if (nearObj40 == null)
                {
                    m_StateProcessor.State = m_StringCount;
                }
            }
            else
            {
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
                    jump_start = this.transform.position;
                    m_targetPos = GetUpPosition50();
                    m_StateProcessor.State = m_Jumping;
                }
                else if (nearObj50 == null)
                {
                    //eyeObj = nearObj40;
                    //jump_start = this.transform.position;
                    //m_targetPos = GetUpPosition40();
                    //m_StateProcessor.State = m_Jumping;
                    m_StateProcessor.State = m_StringCount;
                }
            }
            else
            {
                m_StateProcessor.State = m_StringCount;
            }
        }
    }

    /*** 木をランダムに検索 ***/
    private void SearchRandom()
    {
        if (m_randomCount != 5 && m_randomCount != 4 && m_randomCount != 6)
            m_randomCount = Random.Range(4, 7);

        //乱数が５になったら
        if (m_randomCount == 5)
        {
                eyeObj = nearObj2;

                //近い木に飛ぶ
                m_targetPos = GetUpPosition2();

                m_StateProcessor.State = m_Jumping;   
        }
        if (m_randomCount == 4) //乱数が４になったら
        {
            eyeObj = nearObj3;

            //２番目の近くの木に飛ぶ
            m_targetPos = GetUpPosition3();

            m_StateProcessor.State = m_Jumping;
        }
        if (m_randomCount == 6)
        {
            eyeObj = nearObj3;

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
        int count_difference2 = count2[1] - count2[sidenumber];
        int count_difference3 = count3[1] - count3[sidenumber];

        if (count_difference2 >= 0 && count_difference2 <= 2 && count2[sidenumber] > count2[1]) //近くの木の糸の本数が２本以下
        {
            eyeObj = nearObj2;
            jump_start = this.transform.position;
            //近い木に飛ぶ
            m_targetPos = GetUpPosition2();

            m_StateProcessor.State = m_Jumping;
        }
        else if (count_difference3 >= 0 && count_difference3 <= 2 && count3[sidenumber] > count3[1]) //２番目の近くの木の糸の本数が２本以下
        {
            eyeObj = nearObj3;
            jump_start = this.transform.position;
            //２番目の近くの木に飛ぶ
            m_targetPos = GetUpPosition3();

            m_StateProcessor.State = m_Jumping;
        }
        else
        {
            m_StateProcessor.State = m_SearchRandom;
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
        wait_time = 0;

        int treeLayer = LayerMask.GetMask(new string[] { "Tree" });
        //移動先と自分の間のray
        if (Physics.Raycast(transform.position, m_targetPos - transform.position, out jump_target, tree_Detection, treeLayer))
        {
            if(latter_half_time <= wait_time) //後半時
            {
                if(jump_target.transform != nearObj)
                {
                    net_bool = true;
                    anim.SetBool("jump", true);
                    jump_start = transform.position;
                    jump_end = jump_target.point;
                    m_StateProcessor.State = m_JumpMove;
                }
                else
                {
                    m_StateProcessor.State = m_TreeMove;
                }
            }
            else if (jump_target.transform != eyeObj.transform)
            {
                m_randomCount = 0;
                m_StateProcessor.State = m_TreeMove;
            }
            else if (jump_target.transform == eyeObj.transform) //飛びたいところの間に障害物がなければ
            {
                net_bool = true;
                anim.SetBool("jump", true);
                jump_start = transform.position;
                jump_end = jump_target.point;
                //nearObj = jump_target.collider.gameObject;
                m_StateProcessor.State = m_JumpMove;
            }
        }
        else
        {
            m_StateProcessor.State = m_TreeDecision;
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

                stringObj1.GetComponent<StringUnit>().SideUpdate(sidenumber);

                AnimatorStateInfo animInfo = anim.GetCurrentAnimatorStateInfo(0);
                if (animInfo.normalizedTime < 1.0f)
                {
                    anim.SetBool("avoidance", false);
                }
            }
        }

        //後半時
        if (latter_half_time <= wait_time)
        {
            //糸を奪う
            if (distThread >= 0.5f && distThread <= 1 || distNet >= 0.5f && distNet <= 1)
            {
                //奪う確率
                if (net_bool == true)
                {
                    netCount = Random.Range(1, 11);
                    net_bool = false;
                }

                if (netCount <= m_latterHalfNetRob) //失敗したとき
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

                    stringObj1.GetComponent<Connecter>().m_SideNumber = sidenumber;
                }
            }

        }
    }

    /*** 攻撃ジャンプ ***/
    private void AttackJump()
    {
        wait_time = 0;

        //移動先と自分の間のray
        if (Physics.Raycast(transform.position, m_targetPos - transform.position, out jump_target, tree_Detection))
        {
            
            if (jump_target.transform != playerObj.transform)
            {
                m_randomCount = 0;
                m_StateProcessor.State = m_SearchRandom;
            }
            else if (jump_target.transform == playerObj.transform) //飛びたいところの間に障害物がなければ
            {
                dead_bool = true;
                net_bool = true;
                anim.SetBool("jump", true);
                jump_start = transform.position;
                jump_end = jump_target.point;
                m_StateProcessor.State = m_AttackJumpMove;
            }
        }
        else
        {
            m_StateProcessor.State = m_TreeDecision;
        }
    }

    /*** 攻撃ジャンプ移動中 ***/
    private void AttackJumpMove()
    {
        if (playerDist <= 5) //攻撃
        {
            anim.SetBool("Attack", true);
            SendingBodyBlow(playerObj);
        }
        else
        {
            anim.SetBool("jumpair", true);
            anim.SetBool("jump", false);
        }

        if (Projection(jump_start, jump_end, jump_target.normal, 30.0f))
        {
            transform.position = jump_end;
            m_StateProcessor.State = m_AttackRearJump;
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

        //後半時
        if (latter_half_time <= wait_time)
        {
            //糸を奪う
            if (distThread >= 0.5f && distThread <= 1 || distNet >= 0.5f && distNet <= 1)
            {
                //奪う確率
                if (net_bool == true)
                {
                    netCount = Random.Range(1, 11);
                    net_bool = false;
                }

                if (netCount <= m_latterHalfNetRob) //失敗したとき
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
    }

    /*** 攻撃後のジャンプの瞬間 ***/
    private void AttackRearJump()
    {
        anim.SetBool("Attack", false);
        wait_time += Time.deltaTime * 1;
        if (wait_time >= 1)
        {
            if (m_randomCount != 5 && m_randomCount != 4)
                m_randomCount = Random.Range(4, 6);

            //乱数が５になったら
            if (m_randomCount == 5)
            {
                eyeObj = nearObj2;

                //近い木に飛ぶ
                m_targetPos = GetUpPosition2();
            }
            if (m_randomCount == 4) //乱数が４になったら
            {
                eyeObj = nearObj3;

                //２番目の近くの木に飛ぶ
                m_targetPos = GetUpPosition3();
            }

            int treeLayer = LayerMask.GetMask(new string[] { "Tree" });
            //移動先と自分の間のray
            if (Physics.Raycast(transform.position, m_targetPos - transform.position, out jump_target, tree_Detection, treeLayer))
            {
                net_bool = true;
                anim.SetBool("jump", true);
                jump_start = transform.position;
                jump_end = jump_target.point;
                m_StateProcessor.State = m_AttackRearJumpMove;
            }
        }
    }

    /*** 攻撃後のジャンプ中 ***/
    private void AttackRearJumpMove()
    {
        anim.SetBool("jumpair", true);
        anim.SetBool("jump", false);
        if (Projection(jump_start, jump_end, jump_target.normal, 30.0f))
        {
            dead_bool = false;
            m_randomCount = 0;
            transform.position = jump_end;
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

        predominance_time += Time.deltaTime * 1;

        //相手が優勢の時、又は、同じの時
        if (EnemyTree_Count < PlayerTree_Count || EnemyTree_Count == PlayerTree_Count)
        {
            if(latter_half_time <= time_limit)//後半時
            {
                m_StateProcessor.State = m_LatterHalfDecision;
            }
            else
            {
                predominance_time = 0;
                m_StateProcessor.State = m_TreeDecision;
            }
        }
        else if (predominance_time >= predominance_Thought_Time)
        {
            if (myTreeDist <= tree_Detection)
            {
                predominance_time = 0;
                m_StateProcessor.State = m_PredominanceStringCount;
            }
            else
            {
                predominance_time = 0;
                m_StateProcessor.State = m_PredominanceMyTree;
            }
        }
    }

    /*** 優勢時の近くの木の付いている糸の判別 ***/
    private void PredominanceStringCount()
    {
        int sidenumber = GetComponent<StringShooter>().m_SideNumber;

        //近くの木の付いている糸の差
        p_Thread_count_difference1 = mytreecount1[1] - mytreecount1[sidenumber];
        if (myTreeObj2 != null)
        {
            //2番目近くの木の付いている糸の差
            p_Thread_count_difference2 = mytreecount2[1] - mytreecount2[sidenumber];
        }

        if (p_Thread_count_difference1 >= 0 && p_Thread_count_difference1 <= 2 &&
            mytreecount1[sidenumber] > mytreecount1[1]) //近くの自分の木の糸の本数が２本以下
        {
            eyeObj = nearObj2;
            jump_start = this.transform.position;
            //近い木に飛ぶ
            m_targetPos = MyTreePosition1();

            m_StateProcessor.State = m_PredominanceJump;
        }
        else if (myTreeObj2 != null && p_Thread_count_difference1 >= 0 &&
            p_Thread_count_difference1 <= 2 && mytreecount1[sidenumber] > mytreecount1[1])//２番目に近くの自分の木の糸の本数が２本以下
        {
            eyeObj = nearObj3;
            jump_start = this.transform.position;
            //２番目の近くの木に飛ぶ
            m_targetPos = MyTreePosition2();

            m_StateProcessor.State = m_PredominanceJump;
        }
        else
        {
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
                jump_start = this.transform.position;
                m_targetPos = MyTreePosition1();
                m_StateProcessor.State = m_PredominanceJump;
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
                jump_start = this.transform.position;
                m_targetPos = MyTreePosition2();
                m_StateProcessor.State = m_PredominanceJump;
            }
            else if (myTreeObj2 == null)
            {
                eyeObj = myTreeObj3;
                jump_start = this.transform.position;
                m_targetPos = MyTreePosition3();
                m_StateProcessor.State = m_PredominanceJump;
            }
        }
    }

    /*** 優勢時のジャンプの瞬間 ***/
    private void PredominanceJump()
    {
        wait_time = 0;

        int treeLayer = LayerMask.GetMask(new string[] { "Tree" });
        //移動先と自分の間のray
        if (Physics.Raycast(transform.position, m_targetPos - transform.position, out jump_target, tree_Detection, treeLayer))
        {

            if (jump_target.transform != eyeObj.transform)
            {
                m_randomCount = 0;
                m_StateProcessor.State = m_TreeMove;
            }
            else if (jump_target.transform == eyeObj.transform) //飛びたいところの間に障害物がなければ
            {
                net_bool = true;
                anim.SetBool("jump", true);
                jump_start = transform.position;
                jump_end = jump_target.point;
                m_StateProcessor.State = m_PredominanceJumpMove;
            }
        }
        else
        {
            m_StateProcessor.State = m_PredominanceDecision;
        }
    }

    /*** 優勢時のジャンプ移動中 ***/
    private void PredominanceJumpMove()
    {
        anim.SetBool("jumpair", true);
        anim.SetBool("jump", false);
        if (Projection(jump_start, jump_end, jump_target.normal, 30.0f))
        {
            transform.position = jump_end;
            m_Shooter.StringShoot(jump_start, jump_end);
            m_StateProcessor.State = m_PredominanceDecision;
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

            if (netCount <= m_predominanceNetRob) //失敗したとき
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




    /*** 後半の行動判断 ***/
    private void LatterHalfDecision()
    {
        anim.SetBool("jump", false);
        anim.SetBool("jumpair", false);
        anim.SetBool("avoidance", false);
        anim.SetBool("Attack", false);

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

        if (playerObj != null)
        {
            //playerとの距離
            playerDist = Vector3.Distance(playerObj.transform.position, this.transform.position);
        }

        //自分が優勢の時
        if (EnemyTree_Count > PlayerTree_Count)
        {
            m_StateProcessor.State = m_PredominanceDecision;
        }
        else if (nearObj0 != null)
        {
            wait_time += Time.deltaTime * 1;
            if (wait_time >= latterHalf_Thought_Time)
            {
                m_StateProcessor.State = m_LatterHalfColorlessTree;
            }
        }
        else if (playerDist >= 10 && playerDist <= player_Detection) //Playerに攻撃
        {
            wait_time += Time.deltaTime * 1;
            if (wait_time >= latterHalf_Thought_Time)
            {
                if (player_onTree == true)
                {
                    if (playerDist <= player_Detection * 2 / 3)  //設定距離の半分以下の距離だったら
                    {
                        if (m_randomCount == 0)
                            m_randomCount = Random.Range(1, 3);
                        if (m_randomCount == 2)
                        {
                            m_randomCount = 0;
                            m_targetPos = GetPlayerPosition();
                            m_StateProcessor.State = m_AttackJump;
                        }
                        else
                        {
                            m_randomCount = 0;
                            m_targetPos = GetPlayerPosition();
                            m_StateProcessor.State = m_SearchRandom;
                        }
                    }
                    else
                    {
                        if (m_randomCount == 0)
                            m_randomCount = Random.Range(1, 4);
                        if (m_randomCount == 3)
                        {
                            m_randomCount = 0;
                            m_targetPos = GetPlayerPosition();
                            m_StateProcessor.State = m_AttackJump;
                        }
                        else
                        {
                            m_randomCount = 0;
                            m_targetPos = GetPlayerPosition();
                            m_StateProcessor.State = m_SearchRandom;
                        }
                    }
                }
                else
                {
                    m_randomCount = 0;
                    m_targetPos = GetPlayerPosition();
                    m_StateProcessor.State = m_SearchRandom;
                }
            }
        }
        else
        {
            wait_time += Time.deltaTime * 1;
            if (wait_time >= latterHalf_Thought_Time)
            {
                m_StateProcessor.State = m_SearchTree;
            }
        }
    }

    /*** 後半の無色の木を検索 ***/
    private void LatterHalfColorlessTree()
    {
        float dist = Vector3.Distance(nearObj0.transform.position, this.transform.position);

        if (dist <= tree_Detection)
        {
            eyeObj = nearObj0;
            jump_start = this.transform.position;
            m_targetPos = GetUpPosition00();
            m_StateProcessor.State = m_Jumping;
        }
        else
        {
            m_StateProcessor.State = m_SearchRandom;
        }
    }





    /*** 近くの糸を検索 ***/
    private void ThreadJump()
    {
        wait_time += Time.deltaTime * 1;

        if (wait_time >= 0.5f)
        {
            //糸のジャンプするポジション
            m_targetPos = GetStringPosition() + myStringObj.transform.forward * Random.Range(
            1, myStringObj.GetComponent<CapsuleCollider>().height - 1);

            eyeObj = myStringObj;

            m_StateProcessor.State = m_StringJump;
        }
    }

    /*** 糸の上での移動 ***/
    private void StringMove()
    {
        //糸との角度
        angle = Vector3.Angle(myStringObj.transform.forward, transform.forward);

        bool randamStart = true;

        //RaycastHit hit;
        //Ray ray = new Ray(transform.position + transform.up * 0.5f, -transform.up);
        //if (Physics.SphereCast(ray, 1, out hit, 1f))
        //{
        //    if (hit.transform.tag == "String")
        //    {
        //        transform.position = Vector3.Lerp(transform.position, hit.point, 0.2f);
        //        transform.rotation = Quaternion.LookRotation(
        //            Vector3.Lerp(transform.forward, Vector3.Cross(transform.right, hit.normal), 0.3f), hit.normal);
        //    }
        //    if (hit.transform == null)
        //    {
        //        m_StateProcessor.State = m_Fall;
        //    }
        //}


        if (angle <= 44 || angle >= 136)
        {
            if (randamStart == true)
            {
                if (m_moveCount != 1 && m_moveCount != 2)
                    m_moveCount = Random.Range(1, 3);

                randamStart = false;
            }

            if (m_moveCount == 1) //前移動
            {
                anim.SetBool("string_front", true);
                transform.Translate(Vector3.forward * m_speed * Time.deltaTime);
                m_moveStart = true;
            }
            if (m_moveCount == 2) //後ろ移動
            {
                anim.SetBool("string_back", true);
                transform.Translate(-Vector3.forward * m_speed * Time.deltaTime);
                m_moveStart = true;
            }
        }
        else if (angle >= 45 || angle <= 135)
        {
            if (randamStart == true)
            {
                if (m_moveCount != 3 && m_moveCount != 4)
                    m_moveCount = Random.Range(3, 5);

                randamStart = false;
            }

            if (m_moveCount == 3) //右移動
            {
                anim.SetBool("string_right", true);
                transform.Translate(Vector3.right * m_speed * Time.deltaTime);
                m_moveStart = true;
            }
            if (m_moveCount == 4) //左移動
            {
                anim.SetBool("string_left", true);
                transform.Translate(Vector3.left * m_speed * Time.deltaTime);
                m_moveStart = true;
            }
        }
        else
        {
            m_moveStart = true;
        }


        if (m_moveStart == true)
        {
            m_moveTimer += Time.deltaTime * 1;

            if (m_moveTimer >= 2)
            {
                m_moveCount = 0;

                anim.SetBool("string_front", false);
                anim.SetBool("string_back", false);
                anim.SetBool("string_right", false);
                anim.SetBool("string_left", false);
            }
            if (m_moveTimer >= 0.5f)
            {
                m_moveStart = false;
                randamStart = true;
                m_moveTimer = 0;

                m_StateProcessor.State = m_TreeDecision;
            }
        }
    }

    /*** 糸にいるときの行動判断 ***/
    private void StringDecision()
    {
        anim.SetBool("string_wait", true);

        //RaycastHit hit;
        //Ray ray = new Ray(transform.position + transform.up * 0.5f, -transform.up);
        //if (Physics.Raycast(ray, out hit, 1f))
        //{
        //    if (hit.transform.tag == "String")
        //    {
        //        transform.position = Vector3.Lerp(transform.position, hit.point, 0.2f);
        //        transform.rotation = Quaternion.LookRotation(
        //            Vector3.Lerp(transform.forward, Vector3.Cross(transform.right, hit.normal), 0.3f), hit.normal);
        //    }
        //    if (hit.transform == null)
        //    {
        //        m_StateProcessor.State = m_Fall;
        //    }
        //}


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


        if (dist40 <= tree_Detection || dist50 <= tree_Detection)
        {
            m_StateProcessor.State = m_ThreadSearchTree;
        }
        else
        {
            m_StateProcessor.State = m_ThreadSearchRamdom;
        }

    }

    /*** 糸の上にいるとき自分の木以外の木を検索 ***/
    private void ThreadSearchTree()
    {
        if (nearObj40 != null)
        {
            dist40 = Vector3.Distance(nearObj40.transform.position, this.transform.position);
        }
        if (nearObj50 != null)
        {
            dist50 = Vector3.Distance(nearObj50.transform.position, this.transform.position);
        }


        if (m_randomCount != 5 && m_randomCount != 4)
            m_randomCount = Random.Range(4, 6);


        if (m_randomCount == 4)
        {
            wait_time += Time.deltaTime * 1;

            if (wait_time >= 0.5f)
            {
                eyeObj = nearObj40;
                jump_start = this.transform.position;
                m_targetPos = GetUpPosition40();
                m_StateProcessor.State = m_StringJumping;
            }
        }
        if (m_randomCount == 5)
        {

            wait_time += Time.deltaTime * 1;

            if (wait_time >= 0.5f)
            {
                if (nearObj50 != null)
                {
                    eyeObj = nearObj50;
                    jump_start = this.transform.position;
                    m_targetPos = GetUpPosition50();
                    m_StateProcessor.State = m_StringJumping;
                }
                else if (nearObj50 == null)
                {
                    eyeObj = nearObj40;
                    m_targetPos = GetUpPosition40();
                }
            }

        }
    }

    /*** 糸の上にいるとき木をランダムに検索 ***/
    private void ThreadSearchRandom()
    {
        if (m_randomCount != 5 && m_randomCount != 4)
            m_randomCount = Random.Range(4, 6);

        //乱数が５になったら
        if (m_randomCount == 5)
        {
            wait_time += Time.deltaTime * 1;

            if (wait_time >= 0.5f)
            {
                eyeObj = nearObj2;

                //近い木に飛ぶ
                m_targetPos = GetUpPosition2();

                m_StateProcessor.State = m_StringJumping;
            }
        }
        if (m_randomCount == 4) //乱数が４になったら
        {
            wait_time += Time.deltaTime * 1;

            if (wait_time >= 0.5f)
            {
                eyeObj = nearObj3;

                //２番目の近くの木に飛ぶ
                m_targetPos = GetUpPosition3();

                m_StateProcessor.State = m_StringJumping;
            }
        }
    }

    /*** 糸の上にいるときジャンプの瞬間 ***/
    private void StringJumping()
    {
        wait_time = 0;

        int treeLayer = LayerMask.GetMask(new string[] { "Tree" });
        //移動先と自分の間のray
        if (Physics.Raycast(transform.position, m_targetPos - transform.position, out jump_target, tree_Detection, treeLayer))
        {

            if (jump_target.transform != eyeObj.transform)
            {
                m_randomCount = 0;
                m_StateProcessor.State = m_StringMove;
            }
            else if (jump_target.transform == eyeObj.transform) //飛びたいところの間に障害物がなければ
            {
                anim.SetBool("string_wait", false);
                jump_start = transform.position;
                jump_end = jump_target.point;
                m_StateProcessor.State = m_JumpMove;
            }
        }
        //else
        //{
        //    m_StateProcessor.State = m_TreeDecision;
        //}
    }

    /*** 糸へのジャンプの瞬間 ***/
    private void StringJump()
    {
        wait_time = 0;

        //移動先と自分の間のray
        if (Physics.Raycast(transform.position, m_targetPos - transform.position, out jump_target, tree_Detection))
        {

            if (jump_target.transform != eyeObj.transform)
            {
                m_randomCount = 0;
                m_StateProcessor.State = m_SearchRandom;
            }
            else if (jump_target.transform == eyeObj.transform) //飛びたいところの間に障害物がなければ
            {
                anim.SetBool("jump", true);
                jump_start = transform.position;
                jump_end = jump_target.point;
                if (Projection(jump_start, jump_end, jump_target.normal, 30.0f))
                {
                    transform.position = jump_end;
                    m_Shooter.StringShoot(jump_start, jump_end);
                    m_StateProcessor.State = m_StringDecision;
                }
            }
        }
        else
        {
            m_StateProcessor.State = m_TreeDecision;
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
        if (Physics.Raycast(ray2, out hit, 0.5f,groundLayer))
        {
            transform.position = Vector3.Lerp(transform.position, hit.point, 0.2f);
            transform.rotation = Quaternion.LookRotation(
                Vector3.Lerp(transform.forward, Vector3.Cross(transform.right, hit.normal), 0.3f), hit.normal);

            anim.SetBool("jump", false);

            m_StateProcessor.State = m_FallGroundMove;
        }
        //m_StateProcessor.State = m_FallingMove;
    }


    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Tree")
        {
            m_randomCount = 0;

            col_number = col.gameObject.GetComponent<Tree>().m_SideNumber;
            reObj2 = col.collider.gameObject;
            nearObj = col.collider.gameObject;
        }

        if(col.gameObject.tag == "Ground")
        {
            ResetBodyblow();
        }

        int sidenumber = GetComponent<StringShooter>().m_SideNumber;
        if (col.gameObject.tag == "String" || col.gameObject.tag == "Net" && col_number != sidenumber)
        {
            m_randomCount = 0;

            if (stringNet != null)
            {
                //近くのネットとの距離
                distNet = Vector3.Distance(stringNet.transform.position, this.transform.position);
            }
            if (stringObj1)
            {
                //近くの相手の糸の距離
                distThread = Vector3.Distance(stringObj1.transform.position, this.transform.position);
            }

            //糸を奪う
            //if (distThread >= 0.5f && distThread <= 1 || distNet >= 0.5f && distNet <= 1)
            //{
            //    //奪う確率
            //    if (net_bool == true)
            //    {
            //        netCount = Random.Range(1, 11);
            //        net_bool = false;
            //    }

            //    if (netCount <= 4) //失敗したとき
            //    {
            //        m_StateProcessor.State = m_Fall;
            //    }
            //    else //成功したとき
            //    {
            //        anim.SetBool("avoidance", true);

            //        AnimatorStateInfo animInfo = anim.GetCurrentAnimatorStateInfo(0);
            //        if (animInfo.normalizedTime < 1.0f)
            //        {
            //            anim.SetBool("avoidance", false);
            //        }

            //        stringObj1.GetComponent<StringUnit>().m_SideNumber = sidenumber;
            //    }
            //}
        }
    }




    //近くの木
    public Vector3 GetUpPosition2()
    {
        return new Vector3(nearObj2.transform.position.x, Random.Range(4, 22), nearObj2.transform.position.z);
    }
    //2番目の近くの木
    public Vector3 GetUpPosition3()
    {
        return new Vector3(nearObj3.transform.position.x, Random.Range(4, 22), nearObj3.transform.position.z);
    }
    //3番目の近くの木
    public Vector3 GetUpPosition4()
    {
        return new Vector3(nearObj4.transform.position.x, Random.Range(4, 22), nearObj4.transform.position.z);
    }


    //誰の陣地でもない近くの木
    public Vector3 GetUpPosition00()
    {
        return new Vector3(nearObj0.transform.position.x, Random.Range(4, 22), nearObj0.transform.position.z);
    }
    //自分の陣地ではない近くの木
    public Vector3 GetUpPosition40()
    {
        return new Vector3(nearObj40.transform.position.x, Random.Range(4, 22), nearObj40.transform.position.z);
    }
    //自分の陣地ではない２番目の近くの木
    public Vector3 GetUpPosition50()
    {
        return new Vector3(nearObj50.transform.position.x, Random.Range(4, 22), nearObj50.transform.position.z);
    }


    //自分の陣地の近くの木
    public Vector3 MyTreePosition1()
    {
        return new Vector3(myTreeObj.transform.position.x, Random.Range(4, 22), myTreeObj.transform.position.z);
    }

    //自分の陣地の2番目に近くの木
    public Vector3 MyTreePosition2()
    {
        return new Vector3(myTreeObj2.transform.position.x, Random.Range(4, 22), myTreeObj2.transform.position.z);
    }

    //自分の陣地の3番目に近くの木
    public Vector3 MyTreePosition3()
    {
        return new Vector3(myTreeObj3.transform.position.x, Random.Range(4, 22), myTreeObj3.transform.position.z);
    }


    //近くの木のポジション
    public Vector3 GetPosition()
    {
        return new Vector3(nearObj2.transform.position.x, nearObj2.transform.position.y, nearObj2.transform.position.z);
    }
    //２番目の近くの木のポジション
    public Vector3 GetPosition2()
    {
        return new Vector3(nearObj3.transform.position.x, nearObj3.transform.position.y, nearObj3.transform.position.z);
    }


    //近くの木にジャンプするポジション
    public Vector3 GetPosition3()
    {
        return new Vector3(nearObj2.transform.position.x, 7.0f, nearObj2.transform.position.z);
    }


    //近くの自分の糸にジャンプするポジション
    public Vector3 GetStringPosition()
    {
        return new Vector3(myStringObj.transform.position.x, myStringObj.transform.position.y, myStringObj.transform.position.z);
    }

    //PlayerのPosition
    public Vector3 GetPlayerPosition()
    {
        return new Vector3(playerObj.transform.position.x, playerObj.transform.position.y, playerObj.transform.position.z);
    }
}
