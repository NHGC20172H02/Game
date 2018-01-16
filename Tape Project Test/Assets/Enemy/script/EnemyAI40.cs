using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyState;


//Playerの代わりのEnemy
public class EnemyAI40 : Character
{

    [SerializeField]
    float m_speed = 1f;

    public StringShooter m_Shooter;

    private Vector3 jump_start;
    private Vector3 jump_end;
    private RaycastHit jump_target;         //ジャンプの対象
    [Header("地面から跳べる木の探知範囲")]
    public float m_detection = 30.0f;

    [Header("木の探知範囲")]
    public float tree_Detection = 150.0f;

    [Header("後半状態になるまでの時間(秒)")]
    public float latter_half_time = 90.0f;
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
    bool m_moveStart = false;
    bool net_bool = false;

    float angle;

    GameObject nearObj0;
    [System.NonSerialized]
    public GameObject nearObj;
    GameObject nearObj2;
    GameObject nearObj3;
    GameObject nearObj40;
    GameObject nearObj50;
    GameObject myTreeObj;
    GameObject myTreeObj2;

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

    StringUnit stringUnit;

    Animator anim;

    StateProcessor m_StateProcessor = new StateProcessor();
    GroundMove m_GroundMove = new GroundMove();
    TreeDecision m_TreeDecision = new TreeDecision();
    TreeMove m_TreeMove = new TreeMove();
    ColorlessTree m_ColorlessTree = new ColorlessTree();
    SearchTree m_SearchTree = new SearchTree();
    SearchRandom m_SearchRandom = new SearchRandom();
    Jumping m_Jumping = new Jumping();
    GroundJumping m_GroundJumping = new GroundJumping();
    JumpMove m_JumpMove = new JumpMove();

    StringCount m_StringCount = new StringCount();

    PredominanceDecision m_PredominanceDecision = new PredominanceDecision();
    PredominanceStringCount m_PredominanceStringCount = new PredominanceStringCount();
    PredominanceMyTree m_PredominanceMyTree = new PredominanceMyTree();
    PredominanceJump m_PredominanceJump = new PredominanceJump();
    PredominanceJumpMove m_PredominanceJumpMove = new PredominanceJumpMove();

    LatterHalfDecision m_LatterHalfDecision = new LatterHalfDecision();

    AttackJump m_AttackJump = new AttackJump();
    AttackJumpMove m_AttackJumpMove = new AttackJumpMove();

    FallGroundMove m_FallGroundMove = new FallGroundMove();
    Fall m_Fall = new Fall();

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
        m_Jumping.exeDelegate = Jumping;
        m_GroundJumping.exeDelegate = GroundJumping;
        m_JumpMove.exeDelegate = JumpMove;
       
        m_StringCount.exeDelegate = StringCount;
        
        m_PredominanceDecision.exeDelegate = PredominanceDecision;
        m_PredominanceStringCount.exeDelegate = PredominanceStringCount;
        m_PredominanceMyTree.exeDelegate = PredominanceMyTree;
        m_PredominanceJump.exeDelegate = PredominanceJump;
        m_PredominanceJumpMove.exeDelegate = PredominanceJumpMove;

        m_LatterHalfDecision.exeDelegate = LatterHalfDecision;
        
        m_AttackJump.exeDelegate = AttackJump;
        m_AttackJumpMove.exeDelegate = AttackJumpMove;

        m_FallGroundMove.exeDelegate = FallGroundMove;
        m_Fall.exeDelegate = Fall;

        m_StateProcessor.State = m_GroundMove;
    }

    // Update is called once per frame
    protected override void Update()
    {
        //誰の陣地でもない近くの木
        nearObj0 = GetComponent<NearObj40>().m_nearObj0;

        //近かったオブジェクト（木）を取得
        nearObj2 = GetComponent<NearObj40>().m_nearObj2;
        if (nearObj2 == null) return;

        //2番目のオブジェクト（木）
        nearObj3 = GetComponent<NearObj40>().m_nearObj3;
        if (nearObj3 == null) return;

        //近くの自分の陣地ではない木
        nearObj40 = GetComponent<NearObj40>().m_nearObj40;

        //2番目の自分の陣地ではない木
        nearObj50 = GetComponent<NearObj40>().m_nearObj50;

        //近くの自分の陣地の木
        myTreeObj = GetComponent<NearObj40>().m_myTreeObj;

        //近くの自分の陣地の木
        myTreeObj2 = GetComponent<NearObj40>().m_myTreeObj2;

        //近くの自分の糸
        myStringObj = GetComponent<NearObj40>().m_myStringObj;

        //近くの相手の糸
        stringObj1 = GetComponent<NearObj40>().m_stringObj1;

        //近くの相手のネット
        stringNet = GetComponent<NearObj40>().m_stringNet;

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

        //playerとの距離
        if (playerObj != null)
        {
            playerDist = Vector3.Distance(playerObj.transform.position, this.transform.position);
        }

        //近くのネットとの距離
        if (stringNet != null)
        {
            distNet = Vector3.Distance(stringNet.transform.position, this.transform.position);
        }
        //近くの相手の糸の距離
        if (stringObj1)
        {
            distThread = Vector3.Distance(stringObj1.transform.position, this.transform.position);
        }


        if (Input.GetKeyDown(KeyCode.G))
        {
            m_StateProcessor.State = m_Fall;
        }

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

        if (dist <= m_detection)
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

    /*** 糸から落ちた後の地面の移動 ***/
    private void FallGroundMove()
    {
        anim.SetBool("jump", false);
        anim.SetBool("trap", false);
        anim.SetBool("jumpair", false);
        anim.SetBool("avoidance", false);

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

                if (dist <= m_detection)
                {
                    anim.SetBool("move_front", false);

                    wait_time += Time.deltaTime * 1;
                    if (wait_time >= 1)
                    {
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

        if(PlayerTree_Count != 0)
        {
            //Playerの木の本数
            PlayerTree_Count = TerritoryManager.Instance.GetTreeCount(2);
        }
        if(EnemyTree_Count != 0)
        {
            //Enemyの木の本数
            EnemyTree_Count = TerritoryManager.Instance.GetTreeCount(1);
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
            else if (hit.transform == null)
            {
                m_StateProcessor.State = m_Fall;
            }
        }

        //自分が優勢の時
        if (EnemyTree_Count > PlayerTree_Count)
        {
            m_StateProcessor.State = m_PredominanceDecision;
        }
        else if (latter_half_time <= wait_time) //後半時
        {
            m_StateProcessor.State = m_LatterHalfDecision;
        } 
        else if (nearObj0 != null)
        {
            wait_time += Time.deltaTime * 1;
            if (wait_time >= 2.0f)
            {
                m_StateProcessor.State = m_ColorlessTree;
            }
        }
        else if (playerDist >= 10 && playerDist <= tree_Detection) //Playerに攻撃
        {
            m_targetPos = GetPlayerPosition();
            m_StateProcessor.State = m_AttackJump;
        }
        else if (nearObj40 == null && nearObj50 == null)
        {
            m_StateProcessor.State = m_StringCount;
        }
        else
        {
            wait_time += Time.deltaTime * 1;
            if (wait_time >= 2.0f)
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
            m_StateProcessor.State = m_TreeMove;
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
                    eyeObj = nearObj40;
                    jump_start = this.transform.position;
                    m_targetPos = GetUpPosition40();
                    m_StateProcessor.State = m_Jumping;
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
        if (m_randomCount != 5 && m_randomCount != 4)
            m_randomCount = Random.Range(4, 6);

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

    }

    /*** 近くの木の付いている糸の判別 ***/
    private void StringCount()
    {
        int sidenumber = GetComponent<StringShooter>().m_SideNumber;
        //GameObject player;
        //player = GameObject.FindWithTag("Player");
        //int playerNumber = player.GetComponent<StringShooter>().m_SideNumber;
        int count_difference2 = count2[2] - count2[sidenumber];
        int count_difference3 = count3[2] - count3[sidenumber];

        if (count_difference2 >= 0 && count_difference2 <= 2 && count2[sidenumber] > count2[2]) //近くの木の糸の本数が２本以下
        {
            eyeObj = nearObj2;
            jump_start = this.transform.position;
            //近い木に飛ぶ
            m_targetPos = GetUpPosition2();

            m_StateProcessor.State = m_Jumping;
        }
        else if (count_difference3 >= 0 && count_difference3 <= 2 && count3[sidenumber] > count3[2]) //２番目の近くの木の糸の本数が２本以下
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
            if (latter_half_time <= wait_time) //後半時
            {
                if (jump_target.transform != nearObj)
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
                netCount = Random.Range(1, 10);
                net_bool = false;
            }

            if (netCount <= 4) //失敗したとき
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
                    netCount = Random.Range(1, 4);
                    net_bool = false;
                }

                if (netCount == 1) //失敗したとき
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
        //playerとの距離
        float playerDist = Vector3.Distance(playerObj.transform.position, this.transform.position);

        if (playerDist <= 1)
        {
            anim.SetBool("Attack", true);
        }
        else
        {
            anim.SetBool("jumpair", true);
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
                netCount = Random.Range(1, 10);
                net_bool = false;
            }

            if (netCount <= 4) //失敗したとき
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
                    netCount = Random.Range(1, 4);
                    net_bool = false;
                }

                if (netCount == 1) //失敗したとき
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
            else if (hit.transform == null)
            {
                m_StateProcessor.State = m_Fall;
            }
        }

        wait_time += Time.deltaTime * 1;

        if (PlayerTree_Count != 0)
        {
            //Playerの木の本数
            PlayerTree_Count = TerritoryManager.Instance.GetTreeCount(2);
        }
        if (EnemyTree_Count != 0)
        {
            //Enemyの木の本数
            EnemyTree_Count = TerritoryManager.Instance.GetTreeCount(1);
        }

        //相手が優勢の時、又は、同じの時
        if (EnemyTree_Count < PlayerTree_Count || EnemyTree_Count == PlayerTree_Count)
        {
            if (latter_half_time <= time_limit)
            {
                m_StateProcessor.State = m_LatterHalfDecision;
            }
            else
            {
                wait_time = 0;
                m_StateProcessor.State = m_TreeDecision;
            }
        }
        else if (wait_time >= 2)
        {
            if (myTreeDist <= tree_Detection)
            {
                wait_time = 0;
                m_StateProcessor.State = m_PredominanceStringCount;
            }
            else
            {
                wait_time = 0;
                m_StateProcessor.State = m_PredominanceMyTree;
            }
        }
    }

    /*** 優勢時の近くの木の付いている糸の判別 ***/
    private void PredominanceStringCount()
    {
        int sidenumber = GetComponent<StringShooter>().m_SideNumber;

        //近くの木の付いている糸の差
        p_Thread_count_difference1 = mytreecount1[2] - mytreecount1[sidenumber];
        if (myTreeObj2 != null)
        {
            //2番目近くの木の付いている糸の差
            p_Thread_count_difference2 = mytreecount2[2] - mytreecount2[sidenumber];
        }

        if (p_Thread_count_difference1 >= 0 && p_Thread_count_difference1 <= 2 &&
            mytreecount1[sidenumber] > mytreecount1[2]) //近くの自分の木の糸の本数が２本以下
        {
            eyeObj = nearObj2;
            jump_start = this.transform.position;
            //近い木に飛ぶ
            m_targetPos = MyTreePosition1();

            m_StateProcessor.State = m_PredominanceJump;
        }
        else if (myTreeObj2 != null && p_Thread_count_difference1 >= 0 &&
            p_Thread_count_difference1 <= 2 && mytreecount1[sidenumber] > mytreecount1[2])//２番目に近くの自分の木の糸の本数が２本以下
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
                eyeObj = myTreeObj;
                jump_start = this.transform.position;
                m_targetPos = MyTreePosition1();
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
                netCount = Random.Range(1, 3);
                net_bool = false;
            }

            if (netCount == 1) //失敗したとき
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
            else if (hit.transform == null)
            {
                m_StateProcessor.State = m_Fall;
            }
        }

        if (PlayerTree_Count != 0)
        {
            //Playerの木の本数
            PlayerTree_Count = TerritoryManager.Instance.GetTreeCount(2);
        }
        if (EnemyTree_Count != 0)
        {
            //Enemyの木の本数
            EnemyTree_Count = TerritoryManager.Instance.GetTreeCount(1);
        }

        //自分が優勢の時
        if (EnemyTree_Count > PlayerTree_Count)
        {
            m_StateProcessor.State = m_PredominanceDecision;
        }  
        else if (nearObj0 != null)
        {
            wait_time += Time.deltaTime * 1;
            if (wait_time >= 1.0f)
            {
                m_StateProcessor.State = m_ColorlessTree;
            }
        }
        else if (playerDist >= 10 && playerDist <= tree_Detection) //Playerに攻撃
        {
            m_targetPos = GetPlayerPosition();
            m_StateProcessor.State = m_AttackJump;
        }
        else
        {
            wait_time += Time.deltaTime * 1;
            if (wait_time >= 1.0f)
            {
                m_StateProcessor.State = m_SearchTree;
            }
        }
    }




    //落下
    private void Fall()
    {
        AnimatorStateInfo animInfo = anim.GetCurrentAnimatorStateInfo(0);

        anim.SetBool("trap", true);
        if (animInfo.normalizedTime < 1.0f)
        {
            anim.SetBool("dead", true);
            //anim.SetBool("jump", false);
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
        if (Physics.Raycast(ray2, out hit, 0.5f))
        {
            if (hit.transform.tag == "Ground")
            {
                transform.position = Vector3.Lerp(transform.position, hit.point, 0.2f);
                transform.rotation = Quaternion.LookRotation(
                    Vector3.Lerp(transform.forward, Vector3.Cross(transform.right, hit.normal), 0.3f), hit.normal);

                anim.SetBool("jump", false);

                m_StateProcessor.State = m_FallGroundMove;
            }
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

        if (col.gameObject.tag == "Player")
        {
            m_StateProcessor.State = m_Fall;
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
            if (distThread >= 0.5f && distThread <= 1 || distNet >= 0.5f && distNet <= 1)
            {
                //奪う確率
                if (net_bool == true)
                {
                    netCount = Random.Range(1, 11);
                    net_bool = false;
                }

                if (netCount <= 4) //失敗したとき
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

                    stringObj1.GetComponent<StringUnit>().m_SideNumber = sidenumber;
                }
            }
        }
    }


    //近くの木
    public Vector3 GetUpPosition2()
    {
        return new Vector3(nearObj2.transform.position.x, Random.Range(4, 20), nearObj2.transform.position.z);
    }
    //２番目の近くの木
    public Vector3 GetUpPosition3()
    {
        return new Vector3(nearObj3.transform.position.x, Random.Range(4, 20), nearObj3.transform.position.z);
    }


    //誰の陣地でもない近くの木
    public Vector3 GetUpPosition00()
    {
        return new Vector3(nearObj0.transform.position.x, Random.Range(4, 20), nearObj0.transform.position.z);
    }
    //自分の陣地ではない近くの木
    public Vector3 GetUpPosition40()
    {
        return new Vector3(nearObj40.transform.position.x, Random.Range(4, 20), nearObj40.transform.position.z);
    }
    //自分の陣地ではない２番目の近くの木
    public Vector3 GetUpPosition50()
    {
        return new Vector3(nearObj50.transform.position.x, Random.Range(4, 20), nearObj50.transform.position.z);
    }


    //自分の陣地の近くの木
    public Vector3 MyTreePosition1()
    {
        return new Vector3(myTreeObj.transform.position.x, Random.Range(4, 20), myTreeObj.transform.position.z);
    }

    //自分の陣地の2番目に近くの木
    public Vector3 MyTreePosition2()
    {
        return new Vector3(myTreeObj2.transform.position.x, Random.Range(4, 20), myTreeObj2.transform.position.z);
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

