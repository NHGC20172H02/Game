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


    int m_treeLeave;
    int startRan;
    int treeObj = 0;
    int PlayerTree_Count;
    int EnemyTree_Count;
    int netCount;

    float distThread;
    float dist40;
    float dist50;
    float distNet;

    int m_moveCount;
    float wait_time;
    float m_moveTimer;
    bool m_moveStart = false;
    bool startRambool = false;
    bool enemy_decision;
    bool net_bool = false;

    float tree_Count;

    float angle;

    GameObject nearObj0;
    [System.NonSerialized]
    public GameObject nearObj;
    GameObject nearObj2;
    GameObject nearObj3;
    GameObject nearObj40;
    GameObject nearObj50;

    GameObject eyeObj;

    GameObject stringObj;
    GameObject stringNet;

    [System.NonSerialized]
    public GameObject reObj;
    GameObject reObj2;

    Vector3 m_targetPos;

    List<int> count2;
    List<int> count3;

    Animator anim;

    StateProcessor m_StateProcessor = new StateProcessor();
    GroundMove m_GroundMove = new GroundMove();
    TreeDecision m_TreeDecision = new TreeDecision();
    TreeMove m_TreeMove = new TreeMove();
    ColorlessTree m_ColorlessTree = new ColorlessTree();
    SearchTree m_SearchTree = new SearchTree();
    SearchRandom m_SearchRandom = new SearchRandom();
    ThreadJump m_ThreadJump = new ThreadJump();
    Jumping m_Jumping = new Jumping();
    GroundJumping m_GroundJumping = new GroundJumping();
    JumpMove m_JumpMove = new JumpMove();
    StringMove m_StringMove = new StringMove();
    StringCount m_StringCount = new StringCount();
    StringJump m_StringJump = new StringJump();
    StringDecision m_StringDecision = new StringDecision();
    ThreadSearchTree m_ThreadSearchTree = new ThreadSearchTree();
    StringJumping m_StringJumping = new StringJumping();
    ThreadSearchRandom m_ThreadSearchRamdom = new ThreadSearchRandom();

    PredominanceDecision m_PredominanceDecision = new PredominanceDecision();

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

        m_ThreadJump.exeDelegate = ThreadJump;
        m_StringMove.exeDelegate = StringMove;
        m_StringCount.exeDelegate = StringCount;
        m_StringJump.exeDelegate = StringJump;
        m_StringDecision.exeDelegate = StringDecision;
        m_ThreadSearchTree.exeDelegate = ThreadSearchTree;
        m_StringJumping.exeDelegate = StringJumping;
        m_ThreadSearchRamdom.exeDelegate = ThreadSearchRandom;

        m_PredominanceDecision.exeDelegate = PredominanceDecision;

        m_FallGroundMove.exeDelegate = FallGroundMove;
        m_Fall.exeDelegate = Fall;
    }

    // Update is called once per frame
    protected override void Update()
    {
        //誰の陣地でもない近くの木
        nearObj0 = GetComponent<NearObj40>().m_nearObj0;

        //近かったオブジェクト（木）を取得
        nearObj2 = GetComponent<NearObj40>().m_nearObj2;

        //2番目のオブジェクト（木）
        nearObj3 = GetComponent<NearObj40>().m_nearObj3;

        //近くの自分の陣地ではない木
        nearObj40 = GetComponent<NearObj40>().m_nearObj40;

        //2番目の自分の陣地ではない木
        nearObj50 = GetComponent<NearObj40>().m_nearObj50;

        //近くの自分の糸
        stringObj = GetComponent<NearObj40>().m_stringObj;

        //近くの相手のネット
        stringNet = GetComponent<NearObj40>().m_stringNet;


        if (nearObj2 == null) return;
        count2 = new List<int> { 0, 0, 0 };
        foreach (var item in nearObj2.GetComponent<Tree>().m_Child)
        {
            count2[item.m_SideNumber]++;
        }
        if (nearObj3 == null) return;
        count3 = new List<int> { 0, 0, 0 };
        foreach (var item in nearObj3.GetComponent<Tree>().m_Child)
        {
            count3[item.m_SideNumber]++;
        }


        if (treeObj == 1 && reObj == null)//１つ前にいた木を保持
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

        if (nearObj40 == null || nearObj50 == null)
        {
            m_StateProcessor.State = m_TreeDecision;
        }

        //Playerの木の本数
        PlayerTree_Count = TerritoryManager.Instance.GetTreeCount(1);
        //Enemyの木の本数
        EnemyTree_Count = TerritoryManager.Instance.GetTreeCount(2);

        tree_Count += Time.deltaTime * 1;
        if (tree_Count >= 10.0f)
        {
            tree_Count = 0;

            if (enemy_decision == true)
            {
                if (EnemyTree_Count > PlayerTree_Count)
                {
                    m_StateProcessor.State = m_PredominanceDecision;
                }
                else
                {
                    m_StateProcessor.State = m_TreeDecision;
                }
            }
        }

        Debug.DrawLine(transform.position, m_targetPos, Color.blue);

        m_StateProcessor.Execute();
    }

    /*** 地面移動 ***/
    private void GroundMove()
    {
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
                anim.SetBool("jump", true);
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

    //糸から落ちた後の地面の移動
    private void FallGroundMove()
    {
        anim.SetBool("dead", false);

        wait_time += Time.deltaTime * 1;

        if (wait_time >= 0.5f)
        {
            anim.SetBool("move_front", true);

            m_targetPos = GetPosition();

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
    }

    /*** 木にいるときの行動判断 ***/
    private void TreeDecision()
    {
        anim.SetBool("jump", false);
        enemy_decision = true;

        if (stringObj != null)
        {
            //近くの糸との距離
            distThread = Vector3.Distance(stringObj.transform.position, this.transform.position);
        }
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
        }

        if (nearObj0 != null)
        {
            enemy_decision = false;
            m_StateProcessor.State = m_ColorlessTree;
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
            enemy_decision = false;
            m_StateProcessor.State = m_StringCount;
        }
        else
        {
            enemy_decision = false;
            m_StateProcessor.State = m_SearchTree;
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
            wait_time += Time.deltaTime * 1;
            if (wait_time >= 0.5f)
            {
                eyeObj = nearObj0;
                jump_start = this.transform.position;
                m_targetPos = GetUpPosition00();
                m_StateProcessor.State = m_Jumping;
            }
        }
        else
        {
            m_StateProcessor.State = m_TreeDecision;
        }
    }

    /*** 自分の木以外の木を検索 ***/
    private void SearchTree()
    {
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

        if (m_treeLeave != 5 && m_treeLeave != 4)
            m_treeLeave = Random.Range(4, 6);

        if (m_treeLeave == 4)
        {
            if (dist40 <= tree_Detection)
            {
                wait_time += Time.deltaTime * 1;

                if (wait_time >= 0.5f)
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
            }
            else
            {
                m_StateProcessor.State = m_StringCount;
            }
        }
        if (m_treeLeave == 5)
        {
            if (dist50 <= tree_Detection)
            {
                wait_time += Time.deltaTime * 1;

                if (wait_time >= 0.5f)
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
                        m_targetPos = GetUpPosition40();
                    }
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
        if (m_treeLeave != 5 && m_treeLeave != 4)
            m_treeLeave = Random.Range(4, 6);

        //乱数が５になったら
        if (m_treeLeave == 5)
        {
            wait_time += Time.deltaTime * 1;

            if (wait_time >= 0.5f)
            {
                eyeObj = nearObj2;

                //近い木に飛ぶ
                m_targetPos = GetUpPosition2();

                m_StateProcessor.State = m_Jumping;
            }
        }
        if (m_treeLeave == 4) //乱数が４になったら
        {
            wait_time += Time.deltaTime * 1;

            if (wait_time >= 0.5f)
            {
                eyeObj = nearObj3;

                //２番目の近くの木に飛ぶ
                m_targetPos = GetUpPosition3();

                m_StateProcessor.State = m_Jumping;
            }
        }

    }

    /*** 近くの木の付いている糸の判別 ***/
    private void StringCount()
    {
        int sidenumber = GetComponent<StringShooter>().m_SideNumber;
        int count_difference2 = count2[1] - count2[sidenumber];
        int count_difference3 = count3[1] - count3[sidenumber];

        if (count_difference2 >= 0 && count_difference2 <= 2 && count2[sidenumber] > count2[1]) //近くの木の糸の本数が２本以下
        {
            wait_time += Time.deltaTime * 1;

            if (wait_time >= 0.5f)
            {
                eyeObj = nearObj2;

                //近い木に飛ぶ
                m_targetPos = GetUpPosition2();

                m_StateProcessor.State = m_Jumping;
            }
        }
        else if (count_difference3 >= 0 && count_difference3 <= 2 && count3[sidenumber] > count3[1]) //２番目の近くの木の糸の本数が２本以下
        {
            wait_time += Time.deltaTime * 1;

            if (wait_time >= 0.5f)
            {
                eyeObj = nearObj3;

                //２番目の近くの木に飛ぶ
                m_targetPos = GetUpPosition3();

                m_StateProcessor.State = m_Jumping;
            }
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
            m_targetPos = GetStringPosition() + stringObj.transform.forward * Random.Range(
            1, stringObj.GetComponent<CapsuleCollider>().height - 1);

            eyeObj = stringObj;

            m_StateProcessor.State = m_StringJump;
        }
    }



    /*** 優勢時の行動判断 ***/
    private void PredominanceDecision()
    {

    }



    /*** 地面からジャンプ ***/
    private void GroundJumping()
    {
        wait_time = 0;

        int treeLayer = LayerMask.GetMask(new string[] { "Tree" });
        //移動先と自分の間のray
        if (Physics.Raycast(transform.position, m_targetPos - transform.position, out jump_target, 100f, treeLayer))
        {
            jump_start = transform.position;
            jump_end = jump_target.point;
        }
        else
        {
            m_StateProcessor.State = m_TreeDecision;
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

            if (jump_target.transform != eyeObj.transform)
            {
                m_treeLeave = 0;
                m_StateProcessor.State = m_TreeMove;
            }
            else if (jump_target.transform == eyeObj.transform) //飛びたいところの間に障害物がなければ
            {
                anim.SetBool("jump", true);
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
                m_treeLeave = 0;
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
        //else
        //{
        //    m_StateProcessor.State = m_TreeDecision;
        //}
    }





    /*** 糸にいるときの行動判断 ***/
    private void StringDecision()
    {
        anim.SetBool("string_wait", true);

        RaycastHit hit;
        Ray ray = new Ray(transform.position + transform.up * 0.5f, -transform.up);
        if (Physics.Raycast(ray, out hit, 1f))
        {
            if (hit.transform.tag == "String")
            {
                transform.position = Vector3.Lerp(transform.position, hit.point, 0.2f);
                transform.rotation = Quaternion.LookRotation(
                    Vector3.Lerp(transform.forward, Vector3.Cross(transform.right, hit.normal), 0.3f), hit.normal);
            }
            if (hit.transform == null)
            {
                Fall();
            }
        }


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


        if (m_treeLeave != 5 && m_treeLeave != 4)
            m_treeLeave = Random.Range(4, 6);


        if (m_treeLeave == 4)
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
        if (m_treeLeave == 5)
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
        if (m_treeLeave != 5 && m_treeLeave != 4)
            m_treeLeave = Random.Range(4, 6);

        //乱数が５になったら
        if (m_treeLeave == 5)
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
        if (m_treeLeave == 4) //乱数が４になったら
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
                m_treeLeave = 0;
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

    /*** 糸の上での移動 ***/
    private void StringMove()
    {
        //糸との角度
        angle = Vector3.Angle(stringObj.transform.forward, transform.forward);

        bool randamStart = true;

        RaycastHit hit;
        Ray ray = new Ray(transform.position + transform.up * 0.5f, -transform.up);
        if (Physics.SphereCast(ray, 1, out hit, 1f))
        {
            if (hit.transform.tag == "String")
            {
                transform.position = Vector3.Lerp(transform.position, hit.point, 0.2f);
                transform.rotation = Quaternion.LookRotation(
                    Vector3.Lerp(transform.forward, Vector3.Cross(transform.right, hit.normal), 0.3f), hit.normal);
            }
            if (hit.transform == null)
            {
                Fall();
            }
        }


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





    /*** ジャンプ移動中 ***/
    private void JumpMove()
    {
        if (Projection(jump_start, jump_end, jump_target.normal, 30.0f))
        {
            transform.position = jump_end;
            m_Shooter.StringShoot(jump_start, jump_end);
            m_StateProcessor.State = m_TreeDecision;
        }

        if (stringNet != null)
        {
            //近くのネットとの距離
            distNet = Vector3.Distance(stringNet.transform.position, this.transform.position);
        }
        //糸を奪う
        if (distNet >= 1 && distNet <= 2)
        {
            //奪う確率
            if (net_bool == true)
            {
                netCount = Random.Range(1, 20);
                net_bool = false;
            }

            if (netCount >= 1 && netCount <= 7) //失敗したとき
            {
                m_StateProcessor.State = m_Fall;
            }
            else //成功したとき
            {

            }
        }
    }

    //落下
    private void Fall()
    {
        anim.SetBool("dead", true);

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

                startRambool = false;
                anim.SetBool("jump", false);

                m_StateProcessor.State = m_FallGroundMove;

            }
        }
    }


    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Tree")
        {
            m_treeLeave = 0;

            reObj2 = col.collider.gameObject;
            nearObj = col.collider.gameObject;
        }
        if (col.gameObject.tag == "String")
        {
            m_treeLeave = 0;

            nearObj = col.collider.gameObject;
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
        return new Vector3(stringObj.transform.position.x, stringObj.transform.position.y, stringObj.transform.position.z);
    }
}

