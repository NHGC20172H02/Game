using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player {

    private Vector3 m_enemyBottom = Vector3.zero;

    //体当たり状態
    void BodyBlowMove()
    {
        //if (!Depression()) return;
        //if (Projection(move_start, move_end, jump_target.normal, m_Angle))
        //{
        //    JumpReset();
        //    m_StateManager.StateProcassor.State = m_StateManager.TreeTp;
        //    return;
        //}

        var enemy = m_Enemy.GetComponent<EnemyAI4>();
        if (enemy.TreeDist())
        {
            move_end = m_Enemy.transform.position;
            m_enemyBottom = -m_Enemy.transform.up;
        }
        if (BodyBlow(transform.position, move_end, m_enemyBottom))
        {
            if (move_end == m_Enemy.transform.position)
                SendingBodyBlow(m_Enemy);
            Ray ray = new Ray(move_end, m_enemyBottom);
            Physics.Raycast(ray, out jump_target, 1f, m_TreeLayer);
            move_end = jump_target.point;
            JumpReset();
            m_StateManager.StateProcassor.State = m_StateManager.TreeTp;
        }
        //LayerMask playerLayer = LayerMask.GetMask(new string[] { "Player" });
        //if (Physics.CheckSphere(move_end, 2f, playerLayer) && !isLanding)
        //{
        //    if (Physics.CheckSphere(move_end, 2f, m_EnemyLayer))
        //    {
        //        m_Animator.SetTrigger("Tackle");
        //        m_AudioSource.PlayOneShot(m_AudioClips[1]);
        //        SendingBodyBlow(m_Enemy);
        //        isLanding = true;
        //    }
        //    else
        //    {
        //        m_Animator.SetTrigger("Failure");
        //        m_Animator.SetTrigger("Landing");
        //    }
        //}
    }

    bool BodyBlow(Vector3 start, Vector3 end, Vector3 normal)
    {
        //float dis = Vector3.Distance(start, end);

        ////初速度
        //float V0 = dis / (Mathf.Sin(2 * m_Angle * Mathf.Deg2Rad) / 9.8f);
        ////速度
        //float Vx = Mathf.Sqrt(V0) * Mathf.Cos(m_Angle * Mathf.Deg2Rad);
        //float Vy = Mathf.Sqrt(V0) * Mathf.Sin(m_Angle * Mathf.Deg2Rad);

        //flightDuration = dis / Vx;

        //Vector3 forward = (end - start).normalized;
        //var y = new Vector3(0, (Vy + (Physics.gravity.y * elapse_time * m_JumpSpeed)) * Time.deltaTime, 0);
        //var z = forward * Vx * Time.deltaTime;
        //transform.position += y + z;

        //elapse_time += Time.deltaTime;

        ////前方向、上方向の設定
        //if (transform.tag == "Player")
        //{
        //    if (elapse_time > 0.1f)
        //    {
        //        transform.rotation = Quaternion.LookRotation(
        //            Vector3.Lerp(transform.forward, Vector3.Cross(Camera.main.transform.right, normal), 0.2f), normal);
        //        //transform.rotation
        //        //    = Quaternion.LookRotation(Vector3.Lerp(transform.forward, Vector3.Cross(transform.right, normal), 0.2f), normal);
        //    }
        //}
        ////滞空時間を経過時間が上回ったら着地
        //if (elapse_time / m_JumpSpeed > flightDuration)
        //{
        //    elapse_time = 0;
        //    flightDuration = 0;
        //    return true;
        //}
        //return false;



        Vector3 dir = (end - start).normalized;
        transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        transform.Translate(dir * 30f * Time.deltaTime, Space.World);
        LayerMask playerLayer = LayerMask.GetMask(new string[] { "Player" });

        if (Physics.CheckBox(m_center, new Vector3(0.5f, 0.5f, 0.5f), transform.rotation, m_EnemyLayer))
        {
            m_Animator.SetTrigger("Tackle");
            return true;
        }
        else if(Physics.CheckSphere(end, 1f, playerLayer))
        {
            return true;
        }

        return false;
    }

}
