using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

    [Header("ジャンプの速さ")]
    [Range(0.5f, 2.0f)]
    public float m_JumpSpeed = 1f;
    protected float elapse_time = 0;            //ジャンプの経過時間
    protected float flightDuration = 0;         //ジャンプの滞空時間
    protected bool isBodyblow = false;          //体当たりを食らったか
    protected Vector3 gravity = Vector3.zero;   //重力
    protected IEnumerator receiveBodyblow = null;
    static float BodyblowForce = 5f;            //体当たりの威力

    /*** ジャンプパラメータ ***/
    float targetDistance, V0, Vx, Vy;           //距離、初速度、速度
    Vector3 forward;

    protected virtual void Start () {
		
	}
	
	protected virtual void Update () {
		
	}

    //ジャンプの瞬間に呼び出す
    protected void JumpCalculation(Vector3 start, Vector3 end, float angle)
    {
        targetDistance = Vector3.Distance(start, end);

        //初速度
        V0 = targetDistance / (Mathf.Sin(2 * angle * Mathf.Deg2Rad) / 9.8f) * m_JumpSpeed;
        //移動量
        Vx = Mathf.Sqrt(V0) * Mathf.Cos(angle * Mathf.Deg2Rad);
        Vy = Mathf.Sqrt(V0) * Mathf.Sin(angle * Mathf.Deg2Rad);

        flightDuration = targetDistance / Vx / m_JumpSpeed;

        forward = (end - start).normalized;
    }

    //ジャンプ（start : 始点、end : 終点、normal : 着地点の法線ベクトル、angle : 射角）
    protected bool Projection(Vector3 start, Vector3 end, Vector3 normal, float angle)
    {
        targetDistance = Vector3.Distance(start, end);

        //初速度
        V0 = targetDistance / (Mathf.Sin(2 * angle * Mathf.Deg2Rad) / 9.8f);
        //速度
        Vx = Mathf.Sqrt(V0) * Mathf.Cos(angle * Mathf.Deg2Rad);
        Vy = Mathf.Sqrt(V0) * Mathf.Sin(angle * Mathf.Deg2Rad);

        flightDuration = targetDistance / Vx;

        forward = (end - start).normalized;
        var y = new Vector3(0, (Vy + (Physics.gravity.y * elapse_time * m_JumpSpeed)) * Time.deltaTime, 0);
        var z = forward * Vx * Time.deltaTime;
        transform.position += y + z;

        elapse_time += Time.deltaTime;

        //前方向、上方向の設定
        if(transform.tag == "Player")
        {
            if (elapse_time > 0.3f)
            {
                //transform.rotation = Quaternion.LookRotation(
                //    Vector3.Lerp(transform.forward, Vector3.Cross(Camera.main.transform.right, normal), 0.2f), normal);
                transform.rotation
                    = Quaternion.LookRotation(Vector3.Lerp(transform.forward, Vector3.Cross(transform.right, normal), 0.2f), normal);
            }
        }
        else
        {
            transform.rotation
                = Quaternion.LookRotation(Vector3.Lerp(transform.forward, Vector3.Cross(transform.right, normal), 0.2f), normal);
        }
        //滞空時間を経過時間が上回ったら着地
        if (elapse_time / m_JumpSpeed > flightDuration)
        {
            elapse_time = 0;
            flightDuration = 0;
            return true;
        }
        return false;
    }

    //体当たり情報を相手に送信
    protected void SendingBodyBlow(GameObject target)
    {
        Character character = null;
        if (target.tag == "Player")
        {
            if (target.GetComponent<Player>().IsOnTree())
            {
                character = target.GetComponent<Player>();
                character.GetComponent<Player>().m_Animator.SetTrigger("Failure");
            }
        }
        else if (target.tag == "Enemy")
        {
            character = target.GetComponent<EnemyAI40>();
            //character = target.GetComponent<BodyblowTestEnemy>();
        }

        if (character == null) return;
        character.isBodyblow = true;
        Vector3 reflection = Reflection(target.transform.position - transform.position, target.transform.up).normalized;
        character.receiveBodyblow = ReceiveBodyBlow(character, reflection);
        character.StartCoroutine(character.receiveBodyblow);
    }

    //反射ベクトル
    protected Vector3 Reflection(Vector3 forward, Vector3 normal)
    {
        return (forward - 2 * Vector3.Dot(forward, normal) * normal);
    }

    //体当たりを食らった際の処理
    IEnumerator ReceiveBodyBlow(Character target, Vector3 force)
    {
        while (true)
        {
            //target.gravity.y += Physics.gravity.y * Time.deltaTime;
            //target.transform.Translate(target.gravity * Time.deltaTime, Space.World);
            target.transform.Translate(force * BodyblowForce * Time.deltaTime, Space.World);
            yield return null;
        }
    }

    protected void ResetBodyblow()
    {
        if(receiveBodyblow != null)
        {
            StopCoroutine(receiveBodyblow);
        }
        gravity.y = 0;
        isBodyblow = false;
    }
}
