using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyState;

public class NPC1 : Character
{
    [SerializeField]
    float m_speed = 4f;
    [Header("地面から跳べる木の探知範囲")]
    public float ground_detection = 15.0f;

    GameObject NPCSpawn;

    GameObject nearObj;
    GameObject nearObj2;

    float wait_time;   //行動停止時間
    float m_gauge;     //自分がいる木のゲージ量(-がEnemy、+がPlayer)
    int m_randomCount;
    int spawn_true;
    float Start_Pos_Dist;
    float Spawn_time;
    int spawn_count;

    private Vector3 jump_start;
    private Vector3 jump_end;
    private RaycastHit jump_target;
    Vector3 m_targetPos;
    Vector3 ground_pos; //木にジャンプした時のポジション
    Vector3 Start_pos;  //スタート時の位置
    Animator m_animator;

    //EnemyState
    StateProcessor m_StateProcessor = new StateProcessor();

    TreeDecision m_TreeDecision = new TreeDecision();
    GroundMove m_GroundMove = new GroundMove();
    GroundJumping m_GroundJumping = new GroundJumping();
    Jumping m_Jumping = new Jumping();

    FallGroundMove m_FallGroundMove = new FallGroundMove();
    Fall m_Fall = new Fall();
    // Use this for initialization
    protected override void Start () {
        NPCSpawn = GameObject.Find("NPCSpawn");
        m_animator = GetComponent<Animator>();

        //スタート時の位置
        Start_pos = gameObject.transform.position;

        m_TreeDecision.exeDelegate = TreeDecision;
        m_StateProcessor.State = m_GroundMove;


        m_GroundMove.exeDelegate = GroundMove;
        m_GroundJumping.exeDelegate = GroundJumping;
        m_Jumping.exeDelegate = Jumping;

        m_FallGroundMove.exeDelegate = FallGroundMove;
        m_Fall.exeDelegate = Fall;
    }

    // Update is called once per frame
    protected override void Update () {
        //近かったオブジェクト（木）を取得
        nearObj = GetComponent<NearObj>().m_nearObj2;
        //2番目のオブジェクト（木）
        nearObj2 = GetComponent<NearObj>().m_nearObj3;

        NPCSpawn.GetComponent<NPC1Spawn>().wait_time = Spawn_time;
        NPCSpawn.GetComponent<NPC1Spawn>().spawn_true = spawn_true;
        NPCSpawn.GetComponent<NPC1Spawn>().spawn_count = spawn_count;

        //自分のいる木の情報
        if (nearObj != null)
        {
            //自分のいる木のゲージ量
            m_gauge = nearObj.GetComponent<Tree>().m_TerritoryRate;
        }

        m_StateProcessor.Execute();
    }


    /*** 地面移動 ***/
    private void GroundMove()
    {
        if (m_randomCount == 0)
            m_randomCount = Random.Range(1, 3);

        //歩く目標位置
        if (m_randomCount == 1)
        {
            m_targetPos = GetPosition();
        }
        if (m_randomCount == 2)
        {
            m_randomCount = 5;
            m_targetPos = GetPosition2();
        }

        float dist = Vector3.Distance(nearObj.transform.position, this.transform.position);

        RaycastHit hit;
        Ray ray = new Ray(transform.position + transform.up * 0.5f, -transform.up);
        if (Physics.Raycast(ray, out hit, 2.0f))
        {
            if (hit.transform.tag == "Ground")
            {
                transform.position = Vector3.Lerp(transform.position, hit.point, 0.2f);
                transform.rotation = Quaternion.LookRotation(
                    Vector3.Lerp(transform.forward, Vector3.Cross(transform.right, hit.normal), 0.3f), hit.normal);
            }
        }

        
        //木に飛び乗る
        if (dist <= ground_detection)
        {
            m_animator.SetBool("wait", true);

            wait_time += Time.deltaTime * 1;
            if (wait_time >= 1)
            {
                m_randomCount = 0;
                m_targetPos = GetPosition3();

                int treeLayer = LayerMask.GetMask(new string[] { "Tree" });
                //移動先と自分の間のray
                if (Physics.Raycast(transform.position, m_targetPos - transform.position, out jump_target, 100f, treeLayer))
                {
                    m_animator.SetBool("jump", true);
                    jump_start = transform.position;
                    ground_pos = jump_start;
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

    /*** 地面からジャンプ ***/
    private void GroundJumping()
    {
        wait_time = 0;
        m_animator.SetBool("jumpair", true);
        m_animator.SetBool("jump", false);

        if (Projection(jump_start, jump_end, jump_target.normal, 30.0f))
        {
            m_animator.SetBool("jumpair", false);
            transform.position = jump_end;
            m_StateProcessor.State = m_TreeDecision;
        }
    }


    private void TreeDecision()
    {
        //Playerに当たった時
        if (isBodyblow)
        {
            m_StateProcessor.State = m_Fall;
            return;
        }

        jump_start = transform.position;
        jump_end = ground_pos;

        wait_time += Time.deltaTime * 1;
        if (wait_time >= 15 || m_gauge == 1) //15秒間停止(ゲージを減らす) || ゲージの量が１になったら
        {
            m_animator.SetBool("jump", true);
            m_randomCount = 0;
            wait_time = 0;
            JumpCalculation(jump_start, jump_end, 30.0f);
            m_StateProcessor.State = m_Jumping;
        }
    }

    /*** ジャンプ ***/
    private void Jumping()
    {
        m_animator.SetBool("jumpair", true);
        m_animator.SetBool("jump", false);

        if (Projection(jump_start, jump_end, jump_target.normal, 30.0f))
        {
            m_animator.SetBool("jumpair", false);
            transform.position = jump_end;
            m_StateProcessor.State = m_FallGroundMove;
        }
    }

  

    /*** 落ちた後の地面の移動 帰還する ***/
    private void FallGroundMove()
    {
        Start_Pos_Dist = Vector3.Distance(Start_pos, this.transform.position);
        if(Start_Pos_Dist <= 2.0f) //初期位置に着いたら死亡
        {
            Spawn_time = 0;
            spawn_count = 0; 
            spawn_true = 0;
            Destroy(this.gameObject);
        }

        m_animator.SetBool("wait", false);
        m_animator.SetBool("dead", false);
        m_animator.SetBool("trap", false);

        RaycastHit hit;
        Ray ray = new Ray(transform.position + transform.up * 0.5f, -transform.up);
        if (Physics.Raycast(ray, out hit, 2.0f))
        {
            if (hit.transform.tag == "Ground")
            {
                transform.position = Vector3.Lerp(transform.position, hit.point, 0.5f);
                transform.rotation = Quaternion.LookRotation(
                    Vector3.Lerp(transform.forward, Vector3.Cross(transform.right, hit.normal), 0.5f), hit.normal);
            }
        }
        transform.Translate(Vector3.forward * m_speed * Time.deltaTime);
        //ポジションの方に向く
        Quaternion targetRotation2 = Quaternion.LookRotation(Start_pos - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation2, Time.deltaTime * 5.0f);
    }



    private void Fall()
    {
        AnimatorStateInfo animInfo = m_animator.GetCurrentAnimatorStateInfo(0);

        m_animator.SetBool("trap", true);
        if (animInfo.normalizedTime < 1.0f)
        {
            m_animator.SetBool("dead", true);
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
        Ray ray2 = new Ray(transform.position + transform.up * 0.5f, -transform.up);
        if (Physics.Raycast(ray2, out hit, 1.5f, groundLayer))
        {
            transform.position = Vector3.Lerp(transform.position, hit.point, 0.2f);
            transform.rotation = Quaternion.LookRotation(
                Vector3.Lerp(transform.forward, Vector3.Cross(transform.right, hit.normal), 0.3f), hit.normal);

            ResetBodyblow();
            m_StateProcessor.State = m_FallGroundMove;
        }
        //m_StateProcessor.State = m_FallingMove;
    }

    //近くの木のポジション
    public Vector3 GetPosition()
    {
        return new Vector3(nearObj.transform.position.x, nearObj.transform.position.y, nearObj.transform.position.z);
    }
    //２番目の近くの木のポジション
    public Vector3 GetPosition2()
    {
        return new Vector3(nearObj2.transform.position.x, nearObj2.transform.position.y, nearObj2.transform.position.z);
    }

    //近くの木にジャンプするポジション
    public Vector3 GetPosition3()
    {
        return new Vector3(nearObj.transform.position.x, 15.0f, nearObj.transform.position.z);
    }
}
