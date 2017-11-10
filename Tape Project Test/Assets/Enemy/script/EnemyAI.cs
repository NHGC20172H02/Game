using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {

    enum State
    {
        A = 1 << 0,
        B = 1 << 1,
        C = 1 << 2
    }
    
    [Header("通常の移動速度を入力")]
    public float m_Speed;
    [Header("飛び降りる時（空中移動）のスピード")]
    public float m_jumpSpeed = 17.0f;
    //[Header("木の探知範囲")]
    //public float m_detection = 2.0f;

    public StringShooter m_Shooter;

    //乱数
    int m_treeLeave;

    //初期の速さを保存
    float insSpeed;

    float jumpTime;
    float m_rotate;

    bool treele;
    bool near;

    Transform m_treeTag;

    GameObject nearObj;
    GameObject nearObj2;
    GameObject nearObj3;
 
    Vector3 m_targetPos;
    Vector3 tmp;
    Vector3 jump_start;
    Vector3 jump_end;

    State state = State.A;

    [SerializeField]
    Transform CenterOfBalance;
    [SerializeField]
    GameObject wo;

    //public Transform bigTreeTag;

    // Use this for initialization
    void Start () {
        m_targetPos = GetRandomPositionOnLevel();
        //ポジションの方に向く
        Quaternion targetRotation2 = Quaternion.LookRotation(m_targetPos - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation2, Time.deltaTime);

        state = State.A;
        

        //初期のm_Speed（通常速度）をinsSpeedに入れておく
        //以降m_Speedは変数になる
        insSpeed = m_Speed;

       
    }

    // Update is called once per frame
    void Update()
    {
        //int xDeistance = Mathf.RoundToInt(this.transform.position.x - nearObj.transform.position.x);
        //int zDeistance = Mathf.RoundToInt(this.transform.position.z - nearObj.transform.position.z);

        //if (xDeistance <= m_detection && xDeistance >= -m_detection &&
        //    zDeistance <= m_detection && zDeistance >= -m_detection)
        //{

        //}

        Vector3 V = GetUpPositionSample();
        Debug.Log(V);


        RaycastHit hit;
        //ポイントから自分へRayを飛ばす
        Ray ray = new Ray(m_targetPos, this.transform.position - m_targetPos);
        Debug.DrawLine(ray.origin, this.transform.position, Color.blue);

        //2番目に近かったオブジェクト（木）を取得
        nearObj = serchTag(this.gameObject, "Tree");

        //3番目のオブジェクト（木）
        nearObj3 = serchTag2(this.gameObject, "Tree");

        //向いている方向に移動
        //transform.Translate(Vector3.forward * m_Speed * Time.deltaTime);


        Debug.DrawLine(transform.position, transform.position - transform.up * 3, Color.black);
        //下にRayを飛ばしす
        if (Physics.Raycast(transform.position + transform.up * 0.5f, transform.position - transform.up * 3, out hit, 1, 1 << 10))
        {
            transform.position = Vector3.Lerp(transform.position, hit.point, 0.5f);

            Debug.DrawLine(-transform.up, -transform.up - transform.position, Color.black);
            if (hit.transform != transform)
            {
                //壁に張り付く
                transform.position = hit.point;
                Quaternion q = Quaternion.FromToRotation(transform.up, transform.right);
                transform.rotation *= q;
                //transform.rotation = Quaternion.LookRotation(-Vector3.Cross(transform.up, hit.normal), -hit.normal);
            }

            if (hit.collider.gameObject.transform.tag == "Tree")
            {

                m_treeLeave = 0;
                nearObj2 = hit.collider.gameObject;

                m_rotate = Random.Range(0, 2);

                state = State.B;
            }
            if (hit.collider.gameObject.transform.tag == "plane")
            {
                m_treeLeave = 0;
                m_rotate = 0;
                state = State.A;
            }
        }

        //前にRayを出す
        if (Physics.SphereCast(transform.position, 0.5f, transform.forward, out hit, 1, 1 << 10))
        {
            if (hit.transform != transform)
            {
                //壁に張り付く
                transform.position = hit.point;
                Quaternion q = Quaternion.FromToRotation(transform.up, transform.right);
                transform.rotation *= q;//Quaternion.LookRotation(-Vector3.Cross(transform.up, hit.normal), -hit.normal);

            }

            if (hit.collider.gameObject.tag == "Tree")
            {
                //糸を貼る
                m_Shooter.StringShoot(jump_start, jump_end);

                m_treeLeave = 0;
                m_Speed = insSpeed;

                nearObj2 = hit.collider.gameObject;
            }
        }


        //if (Physics.Raycast(CenterOfBalance.position, transform.position - transform.up * 3, out hit, 1, 1 << 10))
        //{
        //    Quaternion qu = Quaternion.FromToRotation(transform.up, hit.normal);
        //    transform.rotation *= qu;
        //}


        //地面の徘徊
        if (state == State.A)
        {
            //前にRayを飛ばす
            //if (Physics.Raycast(transform.position, transform.forward, out hit, 1))
            //{
            //    if (hit.transform != transform)
            //    {
            //        //壁に張り付く
            //        transform.position = hit.point;
            //        transform.rotation = Quaternion.LookRotation(Vector3.Cross(transform.right, hit.normal), hit.normal);

            //    }

            //    if (hit.collider.gameObject.tag == "Tree")
            //    {
            //        nearObj2 = hit.collider.gameObject;
            //    }
            //}


            m_Speed = insSpeed;

            //向いている方向に移動
            transform.Translate(Vector3.forward * m_Speed * Time.deltaTime);

            //ランダム徘徊
            float sprDistanceToTarget = Vector3.SqrMagnitude(transform.position - m_targetPos);
            if (sprDistanceToTarget < 20)
            {
                m_targetPos = GetRandomPositionOnLevel();
            }

            //ポジションの方に向く
            Quaternion targetRotation2 = Quaternion.LookRotation(m_targetPos - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation2, Time.deltaTime * 5.0f);
        }


        //木にいるときの行動
        if (state == State.B)
        {
            //if (Physics.Raycast(m_targetPos, transform.position, out hit))
            //{
            //    //ポイントから自身が見えなかったらループ
            //    while (hit.transform != gameObject.transform)
            //    {
            //        m_treeLeave = Random.Range(0, 20);

            //        if (Physics.Raycast(m_targetPos, transform.position, out hit))
            //            break;
            //    }
            //}


            //if (Physics.Raycast(CenterOfBalance.position, transform.position - transform.up * 3, out hit, 1, 1 << 10))
            //{
            //    Quaternion qu = Quaternion.FromToRotation(transform.up, hit.normal);
            //    transform.rotation *= qu;
            //}



            m_targetPos = GetUpPosition();

            
            if (m_treeLeave != 5 || m_treeLeave != 4)
                m_treeLeave = Random.Range(0, 25000);

            transform.Translate(Vector3.forward * m_Speed * Time.deltaTime);

            //Quaternion targetRotation2 = Quaternion.LookRotation(m_targetPos - transform.position);
            //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation2, Time.deltaTime * 5.0f);


            //木でどう動くか
            if (m_rotate == 0)
            {

                //前
                //transform.Translate(Vector3.forward * m_Speed * Time.deltaTime);

                //Quaternion targetRotation2 = Quaternion.LookRotation(m_targetPos - transform.position);
                //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation2, Time.deltaTime * 5.0f);
            }
            if (m_rotate == 1)
            {
                //右
                //transform.Translate(Vector3.right * m_Speed * Time.deltaTime);
                
            }
            if (m_rotate == 0)
            {
                //左
                //transform.Translate(Vector3.left * m_Speed * Time.deltaTime);
                
            }

            //飛ぶ処理
            //乱数が５になったら
            if (m_treeLeave == 5)
            {

                jump_start = transform.position;


                //近い木に飛ぶ
                m_targetPos = GetUpPosition3();

                state = State.C;

            }
            else if (m_treeLeave == 4) //乱数が４になったら
            {

                jump_start = transform.position;


                //２番目の近くの木に飛ぶ
                m_targetPos = GetUpPosition4();

                state = State.C;
            }
        }

        //空中移動
        if (state == State.C)
        {
            tmp = m_targetPos;

            //前にRayを出す
            //if (Physics.SphereCast(transform.position, 1.0f, transform.forward, out hit, 1))
            //{
            //    if (hit.transform != transform)
            //    {
            //        //壁に張り付く
            //        transform.position = hit.point;
            //        transform.rotation = Quaternion.LookRotation(Vector3.Cross(transform.right, hit.normal), hit.normal);

            //    }

            //    if (hit.collider.gameObject.tag == "Tree")
            //    {
            //        //糸を貼る
            //        m_Shooter.StringShoot(jump_start, jump_end);

            //        m_treeLeave = 0;
            //        m_Speed = insSpeed;

            //        nearObj2 = hit.collider.gameObject;
            //    }
            //}


            //移動先と自分の間のray
            if (Physics.Raycast(m_targetPos, this.transform.position - m_targetPos, out hit))
            {
                //ヒットした場所のポイント
                if (hit.transform == nearObj.transform)
                {
                    tmp = hit.point;
                }

                if (hit.transform == nearObj3.transform)
                {
                    tmp = hit.point;
                }


                if (hit.transform != gameObject.transform)
                {
                    m_treeLeave = 0;
                    state = State.B;
                }
                else if (hit.transform == gameObject.transform) //飛びたいところの間に障害物がなければ
                {

                    //ポジションの方に向く
                    Quaternion targetRotation2 = Quaternion.LookRotation(tmp - transform.position);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation2, Time.deltaTime * 10.0f);

                    transform.Translate(Vector3.forward * m_Speed * Time.deltaTime);

                    m_Speed = m_jumpSpeed;
                    //if(m_Speed >= 6)
                    //m_Speed -= Time.deltaTime + 0.1f;
                }

                jump_end = tmp;
            }
        }
        
    }


    private void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "Tree")
        {

            m_treeLeave = 0;
            m_Speed = insSpeed;

            m_rotate = Random.Range(0, 2);

            state = State.B;

            nearObj2 = col.collider.gameObject;
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
            if(nearObj2 == obs)
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

    //指定されたタグの中で3番目近いオブジェクト
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
            else if(nearObj2 == obs)
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
        return new Vector3(nearObj2.transform.position.x, 20, nearObj2.transform.position.z);
    }

    //近くの木
    public Vector3 GetUpPosition3()
    {
        //Vector3 ne = nearObj.transform.InverseTransformPoint(nearObj.transform.position);
        //ne.y = Random.Range(2, 20);

        //return new Vector3(ne.x, ne.y, ne.z);

        return new Vector3(nearObj.transform.position.x, Random.Range(2,20), nearObj.transform.position.z);
    }

    //２番目の近くの木
    public Vector3 GetUpPosition4()
    {
        //Vector3 ne = nearObj.transform.InverseTransformPoint(nearObj.transform.position);
        //ne.y = Random.Range(2, 20);

        //return new Vector3(ne.x, ne.y, ne.z);

        
        return new Vector3(nearObj3.transform.position.x, Random.Range(2, 20), nearObj3.transform.position.z);
    }

    //枝
    public Vector3 GetUpPositionSample()
    {
        Vector3 localPos = wo.transform.localPosition;

        return new Vector3(localPos.x, localPos.y, localPos.z);
    }
}
