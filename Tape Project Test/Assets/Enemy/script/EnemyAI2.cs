using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI2 : MonoBehaviour
{
    enum State
    {
        A = 1 << 0,
        B = 1 << 1,
        C = 1 << 2
    }
    State state = State.A;

    [SerializeField]
    float m_speed = 1f;

    [SerializeField]
    Transform CenterOfBalance;
    public StringShooter m_Shooter;

    private Vector3 jump_start;
    private Vector3 jump_end;
    private RaycastHit jump_target;         //ジャンプの対象

    //public StringShooter m_Shooter;


    int m_treeLeave;
    float insSpeed;
    float m_jumpSpeed = 17.0f;
    float wait_time;
    float move_time;

    GameObject nearObj;
    GameObject nearObj2;
    GameObject nearObj3;
    Vector3 m_targetPos;
    Vector3 tmp;
    // Use this for initialization
    void Start()
    {
        m_targetPos = GetRandomPositionOnLevel();
        insSpeed = m_speed;
    }


    void Update()
    {
        //2番目に近かったオブジェクト（木）を取得
        nearObj2 = serchTag(this.gameObject, "Tree");
        //3番目のオブジェクト（木）
        nearObj3 = serchTag2(this.gameObject, "Tree");

        //下にRayを飛ばしす
        RaycastHit hit;
        Ray ray = new Ray(transform.position, -transform.up);
        if (Physics.SphereCast(ray, 0.5f, out hit, 0.5f))
        {
            //if (hit.transform != this.transform)
            //{
            //    transform.position = Vector3.Lerp(transform.position, hit.point, 0.2f);
            //    Quaternion qu = Quaternion.FromToRotation(transform.up, hit.normal);
            //    transform.rotation *= qu;

            //    nearObj = hit.collider.gameObject;
            //}

            if (hit.transform.tag == "Tree")
            {
                transform.position = Vector3.Lerp(transform.position, hit.point, 0.2f);
                Quaternion qu = Quaternion.FromToRotation(transform.up, hit.normal);
                transform.rotation *= qu;

                nearObj = hit.collider.gameObject;
            }
            if (hit.transform.name == "Plane")
            {
                transform.position = Vector3.Lerp(transform.position, hit.point, 0.2f);
                Quaternion qu = Quaternion.FromToRotation(transform.up, hit.normal);
                transform.rotation *= qu;
            }
        }
        //前にRayを出す
        RaycastHit hit2;
        Ray ray2 = new Ray(transform.position, transform.forward);
        if (Physics.SphereCast(transform.position, 0.5f, transform.forward, out hit2, 0.5f, 1 << 10))
        {
            if (hit.transform != this.transform)
            {
                transform.position = hit2.point;
                transform.rotation = Quaternion.LookRotation(
                    Vector3.Cross(Vector3.right, hit2.normal), hit2.normal);

                nearObj = hit2.collider.gameObject;
            }
            state = State.B;
        }
        //Ray ray4 = new Ray(transform.position, transform.forward);
        //if (Physics.SphereCast(ray4, 0.5f, out hit2, 1, 1 << 10))
        //{
        //    transform.position = hit2.point;
        //    transform.rotation = Quaternion.LookRotation(
        //        Vector3.Cross(Vector3.right, hit2.normal), hit2.normal);
        //    nearObj = hit2.collider.gameObject;

        //    state = State.B;
        //}


        Ray ray3 = new Ray(m_targetPos, this.transform.position - m_targetPos);
        Debug.DrawLine(ray3.origin, this.transform.position, Color.blue);

        


        if (state == State.A)
        {
            m_speed = insSpeed;

            //ランダム徘徊
            float sprDistanceToTarget = Vector3.SqrMagnitude(transform.position - m_targetPos);
            if (sprDistanceToTarget < 20)
            {
                m_targetPos = GetRandomPositionOnLevel();
            }

            //ポジションの方に向く
            Quaternion targetRotation2 = Quaternion.LookRotation(m_targetPos - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation2, Time.deltaTime * 5.0f);

            transform.Translate(Vector3.forward * m_speed * Time.deltaTime);
        }

        //木にいるときの行動
        if (state == State.B)
        {
            if (Physics.Raycast(ray3.origin, transform.position, out hit, 1 << 10))
            {
                if (hit.transform == nearObj2.transform)
                {
                    jump_target.point = hit.point;
                }
            }

            m_speed = insSpeed;

            if (m_treeLeave != 5 && m_treeLeave != 4)
                m_treeLeave = Random.Range(4, 6);


            //飛ぶ処理
            //乱数が５になったら
            if (m_treeLeave == 5)
            {
                wait_time += Time.deltaTime * 1;
                if(wait_time >= 1 && wait_time < 2)
                {
                    transform.Translate(Vector3.forward * m_speed * Time.deltaTime);
                }

                if (wait_time >= 3)
                {
                    jump_start = this.transform.position;

                    //近い木に飛ぶ
                    m_targetPos = GetUpPosition2();

                    state = State.C;
                }
            }
            else if (m_treeLeave == 4) //乱数が４になったら
            {
                wait_time += Time.deltaTime * 1;
                
                if (wait_time >= 2)
                {
                    jump_start = this.transform.position;

                    //２番目の近くの木に飛ぶ
                    m_targetPos = GetUpPosition3();

                    state = State.C;
                }
            }
        }

        //空中移動
        if (state == State.C)
        {
            wait_time = 0;
            tmp = m_targetPos;


            //移動先と自分の間のray
            if (Physics.Raycast(m_targetPos, this.transform.position - m_targetPos, out hit))
            {
                //ヒットした場所のポイント
                if (hit.transform == nearObj2.transform)
                {
                    tmp = hit.point;
                }
                if (hit.transform == nearObj3.transform)
                {
                    tmp = hit.point;
                }
                
                
                if (hit.transform != gameObject.transform)
                {
                    Debug.Log("tree");
                    m_treeLeave = 0;
                    state = State.B;
                }
                if (hit.transform == gameObject.transform) //飛びたいところの間に障害物がなければ
                {
                    if (Vector3.Distance(transform.position, jump_target.point) < 0.5f)
                    {
                        transform.position = tmp;
                        transform.rotation = Quaternion.LookRotation(
                            Vector3.Cross(transform.right, jump_target.normal), jump_target.normal);
                    }
                    transform.position = Vector3.Slerp(transform.position, tmp, 0.06f);

                    //ポジションの方に向く
                    Quaternion targetRotation2 = Quaternion.LookRotation(tmp - transform.position);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation2, Time.deltaTime * 10.0f);
                }

                jump_end = transform.position;
                //jump_end = tmp;
            }
        }
        
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Tree")
        {
            m_treeLeave = 0;
            m_speed = insSpeed;

            //糸を貼る
            m_Shooter.StringShoot(jump_start, jump_end);

            state = State.B;

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

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(-transform.up * 0.2f + transform.position, 0.5f);
    }
}
