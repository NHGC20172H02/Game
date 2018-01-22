using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyState;

public class EnemyAI2_2 : Character
{
    [SerializeField]
    float m_speed = 1f;

    public StringShooter m_Shooter;

    private Vector3 jump_start;
    private Vector3 jump_end;
    private RaycastHit jump_target;         //ジャンプの対象

    int m_treeLeave;
    float m_jumpSpeed = 17.0f;
    float wait_time;
    float move_time;
    int one;
    int ran;

    int number;
    int m_side;
    bool pro;

    int m_moveCount = 1;
    float m_moveTimer;
    bool m_moveStart = false;

    Tree m_tree;

    GameObject nearObj0;
    GameObject nearObj;
    GameObject nearObj2;
    GameObject nearObj3;
    GameObject nearObj40;
    GameObject nearObj50;
    GameObject nearObj60;

    Vector3 m_targetPos;

    StateProcessor m_StateProcessor = new StateProcessor();
    GroundMove m_GroundMove = new GroundMove();
    TreeDecision m_TreeDecision = new TreeDecision();
    TreeMove m_TreeMove = new TreeMove();
    SearchTree m_SearchTree = new SearchTree();
    SearchRandom m_SearchRandom = new SearchRandom();
    Jumping m_Jumping = new Jumping();
    JumpMove m_JumpMove = new JumpMove();
    //StringMove m_StringMove = new StringMove();


    // Use this for initialization
    protected override void Start()
    {
        //m_targetPos = GetRandomPositionOnLevel();
        ran = Random.Range(1, 3);

        one = 0;

        m_StateProcessor.State = m_GroundMove;
        m_GroundMove.exeDelegate = GroundMove;
        m_TreeDecision.exeDelegate = TreeDecision;
        m_TreeMove.exeDelegate = TreeMove;
        m_SearchTree.exeDelegate = SearchTree;
        m_SearchRandom.exeDelegate = SearchRandom;
        m_Jumping.exeDelegate = Jumping;
        m_JumpMove.exeDelegate = JumpMove;
        //m_StringMove.exeDelegate = StringMove;
    }

    /*** 全状態共通の更新 ***/
    protected override void Update()
    {
        Debug.Log(m_StateProcessor.State.getStateName());

        //近かったオブジェクト（木）を取得
        nearObj2 = serchTag(this.gameObject, "Tree");
        //2番目のオブジェクト（木）
        nearObj3 = serchTag2(this.gameObject, "Tree");

        //近くの自分の陣地ではない木
        nearObj40 = serchTag3(this.gameObject, "Tree");
        //2番目の自分の陣地ではない木
        nearObj50 = serchTag4(this.gameObject, "Tree");
        //3番目の自分の陣地ではない木
        nearObj60 = serchTag5(this.gameObject, "Tree");

        //誰の陣地でもない近くの木
        nearObj0 = serchTag0(this.gameObject, "Tree");

        Debug.DrawLine(transform.position, m_targetPos, Color.blue);

        m_StateProcessor.Execute();
    }

    /*** 地面移動 ***/
    private void GroundMove()
    {
        if (ran == 1)
        {
            m_targetPos = GetPosition();
            ran = 0;
        }
        if (ran == 2)
        {
            m_targetPos = GetPosition2();
            ran = 0;
        }

        //前にRayを出す
        RaycastHit hit;
        Ray ray2 = new Ray(transform.position, transform.forward);
        int treeLayer = LayerMask.GetMask(new string[] { "Tree" });
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1f, treeLayer))
        {
            transform.position = hit.point;
            transform.rotation = Quaternion.LookRotation(
                Vector3.Cross(transform.right, hit.normal), hit.normal);

            nearObj = hit.collider.gameObject;

            m_StateProcessor.State = m_TreeMove;
        }

        //ポジションの方に向く
        Quaternion targetRotation2 = Quaternion.LookRotation(m_targetPos - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation2, Time.deltaTime * 5.0f);

        transform.Translate(Vector3.forward * m_speed * Time.deltaTime);
    }

    /*** 木にいるときの行動判断 ***/
    private void TreeDecision()
    {
        if (nearObj40 == null || nearObj50 == null || one == 0)
        {
            m_StateProcessor.State = m_SearchRandom;
        }
        else
        {
            m_StateProcessor.State = m_SearchTree;
        }
    }

    /*** 木での移動 ***/
    private void TreeMove()
    {
        if (m_moveCount < 1 || m_moveCount > 4)
            m_moveCount = Random.Range(1, 5);

        RaycastHit hit;
        Ray ray = new Ray(transform.position + transform.up * 0.5f, -transform.up);
        int treeLayer = LayerMask.GetMask(new string[] { "Tree" });
        if (Physics.Raycast(ray, out hit, 1f, treeLayer))
        {
            if (hit.transform.tag == "Tree")
            {
                transform.position = Vector3.Lerp(transform.position, hit.point, 0.2f);
                transform.rotation = Quaternion.LookRotation(
                    Vector3.Lerp(transform.forward, Vector3.Cross(transform.right, hit.normal), 0.3f), hit.normal);            }
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
                m_moveTimer = 0;
                m_moveStart = false;
                m_StateProcessor.State = m_TreeDecision;
            }
        }
    }

    /*** 自分の木以外の木を検索 ***/
    private void SearchTree()
    {
        if (m_treeLeave != 4 && m_treeLeave != 5 && m_treeLeave != 6)
            m_treeLeave = Random.Range(4, 7);

        if (nearObj60 == null)
            m_treeLeave = Random.Range(4, 6);

        if (nearObj50 == null)
            m_treeLeave = 4;

        if (nearObj40 == null)
            m_StateProcessor.State = m_SearchRandom;

        if (m_treeLeave == 4)
        {
            wait_time += Time.deltaTime * 1;

            if (wait_time >= 1)
            {
                jump_start = this.transform.position;
                m_targetPos = GetUpPosition40();
                m_StateProcessor.State = m_Jumping;
            }
        }
        if (m_treeLeave == 5)
        {
            wait_time += Time.deltaTime * 1;

            if (wait_time >= 1)
            {
                jump_start = this.transform.position;
                m_targetPos = GetUpPosition50();
                m_StateProcessor.State = m_Jumping;
            }
        }
        if (m_treeLeave == 6)
        {
            wait_time += Time.deltaTime * 1;

            if (wait_time >= 1)
            {
                jump_start = this.transform.position;
                m_targetPos = GetUpPosition60();
                m_StateProcessor.State = m_Jumping;
            }
        }
    }

    /*** 木をランダムに検索 ***/
    private void SearchRandom()
    {
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

                m_StateProcessor.State = m_Jumping;
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

                m_StateProcessor.State = m_Jumping;
            }
        }
    }

    /*** ジャンプの瞬間 ***/
    private void Jumping()
    {
        wait_time = 0;

        if (one < 1)
            one = 1;

        int treeLayer = LayerMask.GetMask(new string[] { "Tree" });
        //移動先と自分の間のray
        if (Physics.Raycast(transform.position, m_targetPos - transform.position, out jump_target, 100f, treeLayer))
        {
            if (jump_target.transform == gameObject.transform)
            {
                m_treeLeave = 0;
                m_StateProcessor.State = m_SearchRandom;
            }

            if (jump_target.transform != gameObject.transform) //飛びたいところの間に障害物がなければ
            {
                jump_start = transform.position;
                jump_end = jump_target.point;
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
        if(Projection(jump_start, jump_end, jump_target.normal, 30.0f))
        {
            transform.position = jump_end;
            m_Shooter.StringShoot(jump_start, jump_end);
            m_StateProcessor.State = m_TreeDecision;
        }
    }

    /*** 糸の上での移動 ***/
    private void StringMove()
    {
        //まだない
    }


    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Tree")
        {
            m_treeLeave = 0;

            //糸を貼る
            //m_Shooter.StringShoot(jump_start, jump_end);

            nearObj = col.collider.gameObject;
        }
    }


    //指定されたタグの中で最も近いオブジェクト
    GameObject serchTag(GameObject nowObj, string tagName)
    {
        //距離用一時変換
        float tmpDis;

        //最も近いオブジェクトの距離
        float nearDis = 0;

        GameObject targetObj = null;

        //タグ指定されたオブジェクトを配列で取得
        foreach (GameObject obs in GameObject.FindGameObjectsWithTag(tagName))
        {
            //触れているオブジェクトを検索から外す
            if (nearObj == obs)
            {
                continue;
            }

            //自身と取得したオブジェクトの距離を取得
            tmpDis = Vector3.Distance(obs.transform.position, nowObj.transform.position);

            //オブジェクトの距離が近く、距離0であればオブジェクト名を取得
            if (nearDis == 0 || nearDis > tmpDis)
            {
                nearDis = tmpDis;
                targetObj = obs;
            }
        }
        //最も近かったオブジェクトを返す
        return targetObj;
    }

    //指定されたタグの中で2番目近いオブジェクト
    GameObject serchTag2(GameObject nowObj, string tagName)
    {
        //距離用一時変換
        float tmpDis;

        //最も近いオブジェクトの距離
        float nearDis = 0;

        GameObject targetObj = null;

        //タグ指定されたオブジェクトを配列で取得
        foreach (GameObject obs in GameObject.FindGameObjectsWithTag(tagName))
        {
            //触れているオブジェクトを検索から外す
            if (nearObj == obs)
            {
                continue;
            }
            else if (nearObj2 == obs)
            {
                continue;
            }

            //自身と取得したオブジェクトの距離を取得
            tmpDis = Vector3.Distance(obs.transform.position, nowObj.transform.position);

            //オブジェクトの距離が近く、距離0であればオブジェクト名を取得
            if (nearDis == 0 || nearDis > tmpDis)
            {
                nearDis = tmpDis;
                targetObj = obs;
            }

        }
        //3番目近かったオブジェクトを返す
        return targetObj;
    }

    //自分の陣地ではない近くの木
    GameObject serchTag3(GameObject nowObj, string tagName)
    {
        GameObject targetObj = null;
        float tmpDis;
        float nearDis = 0;

        foreach (GameObject obs in GameObject.FindGameObjectsWithTag(tagName))
        {

            if (nearObj == obs)
            {
                continue;
            }
            int number = obs.GetComponent<Tree>().m_SideNumber;
            int sidenumber = GetComponent<StringShooter>().m_SideNumber;

            if (number != sidenumber)
            {
                //自身と取得したオブジェクトの距離を取得
                tmpDis = Vector3.Distance(obs.transform.position, nowObj.transform.position);

                if (nearDis == 0 || nearDis > tmpDis)
                {
                    nearDis = tmpDis;
                    targetObj = obs;
                }
            }
        }
        return targetObj;
    }
    //自分の陣地ではない2番目の木
    GameObject serchTag4(GameObject nowObj, string tagName)
    {
        GameObject targetObj = null;
        float tmpDis;
        float nearDis = 0;

        foreach (GameObject obs in GameObject.FindGameObjectsWithTag(tagName))
        {

            if (nearObj == obs)
            {
                continue;
            }
            else if (nearObj40 == obs)
            {
                continue;
            }

            int number = obs.GetComponent<Tree>().m_SideNumber;
            int sidenumber = GetComponent<StringShooter>().m_SideNumber;

            if (number != sidenumber)
            {
                //自身と取得したオブジェクトの距離を取得
                tmpDis = Vector3.Distance(obs.transform.position, nowObj.transform.position);

                if (nearDis == 0 || nearDis > tmpDis)
                {
                    nearDis = tmpDis;
                    targetObj = obs;
                }
            }
        }
        return targetObj;
    }

    //自分の陣地ではない3番目の木
    GameObject serchTag5(GameObject nowObj, string tagName)
    {
        GameObject targetObj = null;
        float tmpDis;
        float nearDis = 0;

        foreach (GameObject obs in GameObject.FindGameObjectsWithTag(tagName))
        {

            if (nearObj == obs)
            {
                continue;
            }
            else if (nearObj40 == obs)
            {
                continue;
            }
            else if (nearObj50 == obs)
            {
                continue;
            }

            int number = obs.GetComponent<Tree>().m_SideNumber;
            int sidenumber = GetComponent<StringShooter>().m_SideNumber;

            if (number != sidenumber)
            {
                //自身と取得したオブジェクトの距離を取得
                tmpDis = Vector3.Distance(obs.transform.position, nowObj.transform.position);

                if (nearDis == 0 || nearDis > tmpDis)
                {
                    nearDis = tmpDis;
                    targetObj = obs;
                }
            }
        }
        return targetObj;
    }

    //誰の陣地でもない近くの木
    GameObject serchTag0(GameObject nowObj, string tagName)
    {
        GameObject targetObj = null;
        float tmpDis;
        float nearDis = 0;

        foreach (GameObject obs in GameObject.FindGameObjectsWithTag(tagName))
        {

            if (nearObj == obs)
            {
                continue;
            }
            int number = obs.GetComponent<Tree>().m_SideNumber;
            int sidenumber = GetComponent<StringShooter>().m_SideNumber;

            if (number == 0)
            {
                //自身と取得したオブジェクトの距離を取得
                tmpDis = Vector3.Distance(obs.transform.position, nowObj.transform.position);

                if (nearDis == 0 || nearDis > tmpDis)
                {
                    nearDis = tmpDis;
                    targetObj = obs;
                }
            }
        }
        return targetObj;
    }


    //ランダムにポジションを指定
    public Vector3 GetRandomPositionOnLevel()
    {
        //levelSizeの距離の範囲内にPosition生成
        float levelSize = 15f;
        return new Vector3(Random.Range(-levelSize, levelSize),
            0.2f, Random.Range(-levelSize, levelSize));
    }

    //登ってる木
    public Vector3 GetUpPosition()
    {
        return new Vector3(nearObj.transform.position.x, 20, nearObj.transform.position.z);
    }


    //近くの木
    public Vector3 GetUpPosition2()
    {
        return new Vector3(nearObj2.transform.position.x, Random.Range(2, 20), nearObj2.transform.position.z);
    }
    //２番目の近くの木
    public Vector3 GetUpPosition3()
    {
        return new Vector3(nearObj3.transform.position.x, Random.Range(2, 20), nearObj3.transform.position.z);
    }


    //誰の陣地でもない近くの木
    public Vector3 GetUpPosition00()
    {
        return new Vector3(nearObj0.transform.position.x, Random.Range(2, 20), nearObj0.transform.position.z);
    }
    //自分の陣地ではない近くの木
    public Vector3 GetUpPosition40()
    {
        return new Vector3(nearObj40.transform.position.x, Random.Range(2, 20), nearObj40.transform.position.z);
    }
    //自分の陣地ではない２番目の近くの木
    public Vector3 GetUpPosition50()
    {
        return new Vector3(nearObj50.transform.position.x, Random.Range(2, 20), nearObj50.transform.position.z);
    }
    //自分の陣地ではない２番目の近くの木
    public Vector3 GetUpPosition60()
    {
        return new Vector3(nearObj60.transform.position.x, Random.Range(2, 20), nearObj60.transform.position.z);
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
}
