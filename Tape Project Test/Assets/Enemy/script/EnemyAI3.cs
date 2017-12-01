using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI3 : Character
{
    enum State
    {
        A = 1 << 0,
        B = 1 << 1,
        C = 1 << 2,
        D = 1 << 3,
        E = 1 << 4,
        F = 1 << 5,
        G = 1 << 6,
        H = 1 << 7,
        I = 1 << 8,
        J = 1 << 9,
        K = 1 << 10
    }
    State state = State.A;

    [SerializeField]
    float m_speed = 1f;

    public StringShooter m_Shooter;

    private Vector3 jump_start;
    private Vector3 jump_end;
    private RaycastHit jump_target;         //ジャンプの対象
    [Header("地面から跳べる木の探知範囲")]
    public float m_detection = 13.0f;

    [Header("木の探知範囲")]
    public float m_treeDetection = 50.0f;


    int m_treeLeave;
    float wait_time;
    int one;
    int startRan;
    int treeObj = 0;
    bool ground_jump;

    int number;

    float dist40;
    float dist50;

    int m_moveCount;
    float m_moveTimer;
    bool m_moveStart = false;

    float angle;

    int m_number;
    int sidenumber;

    GameObject nearObj0;
    [System.NonSerialized]
    public GameObject nearObj;
    GameObject nearObj2;
    GameObject nearObj3;
    GameObject nearObj40;
    GameObject nearObj50;
  
    GameObject stringObj;

    [System.NonSerialized]
    public GameObject reObj;

    Vector3 m_targetPos;
    Vector3 m_stringPos;
    Vector3 tmp;

    Animator anim;

    // Use this for initialization
    protected override void Start()
    {
        startRan = Random.Range(1, 3);

        anim = GetComponent<Animator>();

        one = 0;
    }


    protected override void Update()
    {
        //誰の陣地でもない近くの木
        nearObj0 = GetComponent<NearObj>().m_nearObj0;

        //今いる木
        nearObj = GetComponent<NearObj>().m_nearObj;

        //近かったオブジェクト（木）を取得
        nearObj2 = GetComponent<NearObj>().m_nearObj2;

        //2番目のオブジェクト（木）
        nearObj3 = GetComponent<NearObj>().m_nearObj3;

        //近くの自分の陣地ではない木
        nearObj40 = GetComponent<NearObj>().m_nearObj40;

        //2番目の自分の陣地ではない木
        nearObj50 = GetComponent<NearObj>().m_nearObj50;

        //近くの自分の糸
        stringObj = GetComponent<NearObj>().m_stringObj;

        

        if (treeObj == 1)//１つ前にいた木を保持
        {
            reObj = nearObj;
            treeObj = +1;
        }
        if (treeObj >= 3)
        {
            treeObj = 0;
        }

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

        //下にRayを飛ばしす
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 0.3f, -transform.up, out hit, 0.7f))
        {

            if (hit.transform.tag == "Tree")
            {
                transform.position = Vector3.Lerp(transform.position, hit.point, 0.2f);
                Quaternion qu = Quaternion.FromToRotation(transform.up, hit.normal);
                transform.rotation *= qu;

                nearObj = hit.collider.gameObject;
            }
            if (hit.transform.tag == "String")
            {
                transform.position = hit.point;
                transform.rotation = Quaternion.LookRotation(
                    Vector3.Cross(Vector3.right, hit.normal), hit.normal);
            }
        }
        //前にRayを出す
        RaycastHit hit2;
        if (Physics.SphereCast(transform.position, 0.7f, transform.forward, out hit2, 0.4f))
        {
            if (hit2.transform.tag == "Tree")
            {
                transform.position = hit2.point;
                transform.rotation = Quaternion.LookRotation(
                    Vector3.Cross(Vector3.right, hit2.normal), hit2.normal);

                nearObj = hit2.collider.gameObject;
                treeObj += 1;

                if (ground_jump == true)
                {
                    ground_jump = false;
                }

                anim.SetBool("jump", false);
            }

            if (hit2.transform.tag == "String")
            {
                anim.SetBool("string_wait", true);
                transform.position = hit2.point;
                transform.rotation = Quaternion.LookRotation(
                    Vector3.Cross(Vector3.right, hit2.normal), hit2.normal);
            }

            state = State.B;
        }
        

        Ray ray3 = new Ray(m_targetPos, this.transform.position - m_targetPos);
        Debug.DrawLine(ray3.origin, this.transform.position, Color.blue);


        if (state == State.A)
        {

            float dist = Vector3.Distance(nearObj2.transform.position, this.transform.position);

            if (dist <= m_detection)
            {
                anim.SetBool("move_front", false);

                if (ground_jump == true)
                {
                    wait_time += Time.deltaTime * 1;
                    if (wait_time >= 1)
                    {
                        m_targetPos = GetPosition3();
                        state = State.E;
                    }
                }
            }
            else
            {
                ground_jump = true;

                //ポジションの方に向く
                Quaternion targetRotation2 = Quaternion.LookRotation(m_targetPos - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation2, Time.deltaTime * 5.0f);

                transform.Translate(Vector3.forward * m_speed * Time.deltaTime);
            }

        }

        //木にいるときの行動判断
        if (state == State.B)
        {
            if (nearObj0 != null)
            {
                state = State.G;
            }

            if (nearObj40 == null || nearObj50 == null || one == 0)
            {
                state = State.D;
            }
            else
            {
                state = State.C;
            }
        }

        //無色の木を最優先
        if (state == State.G)
        {
            float dist = Vector3.Distance(nearObj0.transform.position, this.transform.position);

            if (dist <= m_treeDetection)
            {
                wait_time += Time.deltaTime * 1;
                if (wait_time >= 1)
                {
                    jump_start = this.transform.position;
                    m_targetPos = GetUpPosition00();
                    state = State.E;
                }
            }
            else
            {
                state = State.D;
            }
        }

        //自分の陣地ではない木を判断
        if (state == State.C)
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
                if (dist40 <= m_treeDetection)
                {
                    wait_time += Time.deltaTime * 1;

                    if (wait_time >= 1)
                    {
                        jump_start = this.transform.position;
                        m_targetPos = GetUpPosition40();
                        state = State.E;
                    }
                }
                else
                {
                    state = State.I;
                }
            }
            if (m_treeLeave == 5)
            {
                if (dist50 <= m_treeDetection)
                {
                    wait_time += Time.deltaTime * 1;

                    if (wait_time >= 1)
                    {
                        if (nearObj50 != null)
                        {
                            jump_start = this.transform.position;
                            m_targetPos = GetUpPosition50();
                            state = State.E;
                        }
                        else if (nearObj50 == null)
                        {
                            m_targetPos = GetUpPosition40();
                        }
                    }
                }
                else
                {
                    state = State.I;
                }
            }
        }

        //ランダムに飛ぶ処理
        if (state == State.D)
        {
            if (m_treeLeave != 5 && m_treeLeave != 4)
                m_treeLeave = Random.Range(4, 6);

            //乱数が５になったら
            if (m_treeLeave == 5)
            {
                wait_time += Time.deltaTime * 1;

                if (wait_time >= 1)
                {
                    jump_start = this.transform.position;

                    //近い木に飛ぶ
                    m_targetPos = GetUpPosition2();

                    state = State.E;
                }
            }
            if (m_treeLeave == 4) //乱数が４になったら
            {
                wait_time += Time.deltaTime * 1;

                if (wait_time >= 1)
                {
                    jump_start = this.transform.position;

                    //２番目の近くの木に飛ぶ
                    m_targetPos = GetUpPosition3();

                    state = State.E;
                }
            }
        }

        //糸へのジャンプ
        if(state == State.I)
        {
            if (stringObj != null)
            {
                //糸のジャンプするポジション
                m_stringPos = GetStringPosition() + stringObj.transform.forward * Random.Range(
                    1, stringObj.GetComponent<CapsuleCollider>().height - 1);

                m_targetPos = m_stringPos;

                state = State.E;
            }
        }

        //空中移動の瞬間
        if (state == State.E)
        {
            wait_time = 0;
            tmp = m_targetPos;

            if (one < 1)
                one = 1;


            //移動先と自分の間のray
            if (Physics.Raycast(m_targetPos, this.transform.position - m_targetPos, out hit))
            {

                //ヒットした場所のポイント
                if (hit.transform.gameObject == nearObj2.transform.gameObject)
                {
                    tmp = hit.point;
                    jump_target.point = tmp;
                }
                if (hit.transform.gameObject == nearObj3.transform.gameObject)
                {
                    tmp = hit.point;
                    jump_target.point = tmp;
                }


                if (hit.transform != gameObject.transform)
                {
                    m_treeLeave = 0;
                    state = State.H;
                }

                if (hit.transform == gameObject.transform) //飛びたいところの間に障害物がなければ
                {
                    m_treeLeave = 0;
                    anim.SetBool("jump", true);
                    anim.SetBool("string_wait", true);
                    state = State.F;
                }

            }
        }

        //空中移動中
        if (state == State.F)
        {
            if (Vector3.Distance(transform.position, jump_target.point) < 0.5f)
            {
                transform.position = tmp;
            }
            transform.position = Vector3.Slerp(transform.position, tmp, 0.04f);


            //ポジションの方に向く
            Quaternion targetRotation2 = Quaternion.LookRotation(tmp - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation2, Time.deltaTime * 15.0f);

            if (ground_jump == false)
            {
                jump_end = transform.position;
            }
        }


        //木での移動
        if (state == State.H)
        {
            bool randamStart = true;

            if (randamStart == true)
            {
                if (m_moveCount != 1 && m_moveCount != 2 && m_moveCount != 3 && m_moveCount != 4)
                    m_moveCount = Random.Range(1, 5);

                randamStart = false;
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
                if(m_moveTimer >= 3)
                {
                    m_moveStart = false;
                    randamStart = true;
                    m_moveTimer = 0;

                    state = State.D;
                }
            }
        }


        //糸の移動
        if(state == State.J)
        {
            //糸との角度
            angle = Vector3.Angle(stringObj.transform.forward, transform.forward);

            bool randamStart = true;

            if (randamStart == true)
            {
                if (m_moveCount != 1 && m_moveCount != 2 && m_moveCount != 3 && m_moveCount != 4)
                    m_moveCount = Random.Range(1, 5);

                randamStart = false;
            }

            if (angle <= 10)
            {
                if (randamStart == true)
                {
                    if (m_moveCount != 1 && m_moveCount != 2)
                        m_moveCount = Random.Range(1, 3);

                    randamStart = false;
                }

                if (m_moveCount == 1) //前移動
                {

                    transform.Translate(Vector3.forward * m_speed * Time.deltaTime);
                    m_moveStart = true;
                }
                if (m_moveCount == 2) //後ろ移動
                {

                    transform.Translate(-Vector3.forward * m_speed * Time.deltaTime);
                    m_moveStart = true;
                }
            }

            if (m_moveCount == 3) //右移動
            {
                
                transform.Translate(Vector3.right * m_speed * Time.deltaTime);
                m_moveStart = true;
            }
            if (m_moveCount == 4) //左移動
            {
                
                transform.Translate(Vector3.left * m_speed * Time.deltaTime);
                m_moveStart = true;
            }


            if (m_moveStart == true)
            {
                m_moveTimer += Time.deltaTime * 1;

                if (m_moveTimer >= 2)
                {
                    m_moveCount = 0;


                }
                if (m_moveTimer >= 3)
                {
                    m_moveStart = false;
                    randamStart = true;
                    m_moveTimer = 0;

                    state = State.D;
                }
            }
        }
    }



    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Tree")
        {
            m_treeLeave = 0;

            //糸を貼る
            m_Shooter.StringShoot(jump_start, jump_end);

            //state = State.B;
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
        return new Vector3(nearObj0.transform.position.x, Random.Range(4,20), nearObj0.transform.position.z);
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
