using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

    protected float elapse_time = 0;            //ジャンプの経過時間
    protected float flightDuration = 0;         //ジャンプの滞空時間
    protected bool isBodyblow = false;     //体当たりを食らったか

    protected virtual void Start () {
		
	}
	
	protected virtual void Update () {
		
	}

    //ジャンプ（start : 始点、end : 終点、normal : 着地点の法線ベクトル、angle : 射角）
    protected bool Projection(Vector3 start, Vector3 end, Vector3 normal, float angle)
    {
        float target_Distance = Vector3.Distance(start, end);

        //初速度
        float V0 = target_Distance / (Mathf.Sin(2 * angle * Mathf.Deg2Rad) / 9.8f);
        //速度
        float Vx = Mathf.Sqrt(V0) * Mathf.Cos(angle * Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(V0) * Mathf.Sin(angle * Mathf.Deg2Rad);

        flightDuration = target_Distance / Vx;

        Vector3 forward = (end - start).normalized;
        var z = forward * Vx * Time.deltaTime;
        transform.position += new Vector3(0, (Vy + (Physics.gravity.y * elapse_time)) * Time.deltaTime, 0) + z;

        elapse_time += Time.deltaTime;

        //前方向、上方向の設定
        if(transform.tag == "Player")
        {
            if (elapse_time > 0.3f)
                transform.rotation = Quaternion.LookRotation(
                    Vector3.Lerp(transform.forward, Vector3.Cross(Camera.main.transform.right, normal), 0.5f), normal);
        }
        else
        {
            transform.rotation
                = Quaternion.LookRotation(Vector3.Lerp(transform.forward, Vector3.Cross(transform.right, normal), 0.2f), normal);
        }
        //滞空時間を経過時間が上回ったら着地
        if (elapse_time > flightDuration)
        {
            elapse_time = 0;
            flightDuration = 0;
            return true;
        }
        return false;
    }

    //体当たり情報を相手に送信
    protected void SendingBodyBlow(Collider collider)
    {
        Character character = null;
        if (collider.tag == "Player")
        {
            character = collider.GetComponent<Player>();
        }
        else if (collider.tag == "Enemy")
        {
            //character = collider.GetComponent<EnemyAI40>();
            collider.GetComponent<BoxCollider>().isTrigger = false;
            collider.GetComponent<Rigidbody>().useGravity = true;
            Vector3 reflection = Reflection(collider.transform.position - transform.position, collider.transform.up);
            StartCoroutine(AddReflection(collider, reflection));
        }

        if (character == null) return;
        character.isBodyblow = true;
    }

    //反射ベクトル
    protected Vector3 Reflection(Vector3 forward, Vector3 normal)
    {
        return (forward - 2 * Vector3.Dot(forward, normal) * normal);
    }

    IEnumerator AddReflection(Collider collider, Vector3 force)
    {
        float time = 5;
        while (true)
        {
            collider.GetComponent<Rigidbody>().AddForce(force * time, ForceMode.Force);
            time -= 0.1f;
            yield return null;
        }
    }
}
