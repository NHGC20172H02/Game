﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [Header("移動速度")]
    public float m_Speed = 1f;
    [Header("ジャンプの限界距離")]
    public float m_JumpLimit = 50f;
    [Header("ジャンプの射角")]
    public float m_Angle = 30f;
    public float jumpLower = 5f;
    public Transform m_Camera;
    public LayerMask m_GroundLayer, m_TreeLayer, m_StringLayer, m_NetLayer, m_EnemyLayer;
    public PredictionLine m_Prediction;
    public Animator m_Animator;
    public StringShooter m_Shooter;

    private float MIN = -80f;
    private float MAX = 80f;
    private PlayerStateManager m_StateManager;          //Player状態管理
    private RaycastHit m_hitinfo;                       //足元の情報
    private Vector3 m_prevPos;
    private Vector3 Gravity = Vector3.zero;             //重力
    private Vector3 move_start;                         //ジャンプ始点
    private Vector3 move_end;                           //ジャンプ終点
    private RaycastHit jump_target;
    private bool isCursor = false;
    private bool isEscape = false;
    private int waitFrame = 0;
    private float m_failureTime = 0;
    private float m_fallElapse = 0;
    private float m_attackTime = 0;
    private bool isLanding = false;
    private Collider m_enemy = null;

    protected override void Start()
    {
        m_StateManager = PlayerStateManager.GetInstance;
        m_StateManager.GroundTp.p_exeDelegate = GroundMove;
        m_StateManager.TreeTp.p_exeDelegate = TreeTpMove;
        m_StateManager.TreeFp.p_exeDelegate = TreeFpMove;
        m_StateManager.JumpTp.p_exeDelegate = JumpTpMove;
        m_StateManager.StringTp.p_exeDelegate = StringTpMove;
        m_StateManager.Falling.p_exeDelegate = FallingMove;
        m_StateManager.BodyBlow.p_exeDelegate = BodyBlowMove;
        m_Prediction.gameObject.SetActive(true);
        //Instantiate(m_Prediction);
    }

    protected override void Update()
    {
        transform.position = MoveRange(transform.position, new Vector3(MIN, 0, MIN), new Vector3(MAX, 50f, MAX));
    }

    //地面にいる場合の移動
    void GroundMove()
    {
        m_Animator.SetBool("IsString", false);
        m_Prediction.gameObject.SetActive(false);
        Vector3 start = transform.position + transform.up * 0.5f;
        //足元の情報取得（地面優先）
        if (!Physics.Raycast(start, -transform.up, out m_hitinfo, 1f, m_GroundLayer))
            Physics.Raycast(start, -transform.up, out m_hitinfo, 1f, m_TreeLayer);

        //位置補正
        transform.position = Vector3.Lerp(transform.position, m_hitinfo.point, 0.2f);
        Vector3 move = Vector3.zero;
        if(m_Animator.GetCurrentAnimatorStateInfo(0).IsName("GroundMove"))
            move = Move(m_Camera.right, m_hitinfo.normal);

        //木に登る
        if (Physics.Raycast(start, move, out m_hitinfo, 1f))
        {
            transform.position = m_hitinfo.point;
            transform.rotation = Quaternion.LookRotation(Vector3.Cross(transform.right, m_hitinfo.normal), m_hitinfo.normal);
            m_StateManager.StateProcassor.State = m_StateManager.TreeTp;
        }
    }

    //木の上の俯瞰カメラ状態での移動
    void TreeTpMove()
    {
        m_Animator.SetBool("IsString", false);
        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Jumpair"))
            m_Animator.SetTrigger("Landing");        
        Vector3 start = transform.position + transform.up * 0.5f;
        //足元の情報取得（優先順位 : 木->地面->糸(ネット)）
        if (!Physics.Raycast(start, -transform.up, out m_hitinfo, 1f, m_NetLayer))
            if (!Physics.Raycast(start, -transform.up, out m_hitinfo, 1f, m_TreeLayer))
                Physics.Raycast(start, -transform.up, out m_hitinfo, 1f, m_GroundLayer);

        if(m_hitinfo.collider == null)
        {
            transform.position = m_prevPos;
            return;
        }
        if (m_hitinfo.collider.tag == "Ground")
            m_StateManager.StateProcassor.State = m_StateManager.GroundTp;
        if (IsChangedNumber())
            return;
        m_prevPos = transform.position;
        //位置補正
        transform.position = Vector3.Lerp(transform.position, m_hitinfo.point, 0.5f);
        Vector3 move = Vector3.zero;
        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("GroundMove"))
            move = Move(m_Camera.right, m_hitinfo.normal);

        RaycastHit prevHit = m_hitinfo;
        if (Physics.Raycast(start, move, out m_hitinfo, 1f, m_GroundLayer))
            m_StateManager.StateProcassor.State = m_StateManager.GroundTp;
        else
            m_hitinfo = prevHit;

        //一人称視点変更
        if (Input.GetKeyDown(KeyCode.L) || Input.GetButtonDown("LB"))
            m_StateManager.StateProcassor.State = m_StateManager.TreeFp;

        Vector3 origin = start + (m_Camera.position - start).normalized;
        Ray ray = new Ray(origin, m_Camera.forward);
        Jump(ray, m_hitinfo);
        Debug.DrawLine(ray.origin, ray.origin + ray.direction * 5f, Color.red);
    }

    //木の上の一人称カメラ状態での操作
    void TreeFpMove()
    {
        if (IsChangedNumber())
            return;
        Vector3 start = transform.position + transform.up * 0.5f;
        if (Input.GetKeyDown(KeyCode.L) || Input.GetButtonDown("LB"))
        {
            if (m_hitinfo.collider.tag == "Tree" || m_hitinfo.collider.tag == "Net")
                m_StateManager.StateProcassor.State = m_StateManager.TreeTp;
            else if (m_hitinfo.collider.tag == "String")
                m_StateManager.StateProcassor.State = m_StateManager.StringTp;
        }

        Vector3 origin = start + (m_Camera.position - start).normalized;
        Ray ray = new Ray(origin, m_Camera.forward);
        Jump(ray, m_hitinfo);
    }

    //ジャンプ中の動作
    void JumpTpMove()
    {
        if (!Depression()) return;
        if (Projection(move_start, move_end, jump_target.normal, m_Angle))
        {
            transform.position = jump_target.point;
            transform.rotation = Quaternion.LookRotation(Vector3.Cross(m_Camera.right, jump_target.normal), jump_target.normal);
            if(isEscape)
                m_Shooter.StringShoot(m_Prediction.m_HitStringPoint.point, move_end);
            else
                m_Shooter.StringShoot(move_start, move_end);
            waitFrame = 0;
            isEscape = false;
            isLanding = false;
            if (jump_target.collider.tag == "String")
            {
                m_hitinfo = jump_target;
                m_StateManager.StateProcassor.State = m_StateManager.StringTp;
                return;
            }
            m_StateManager.StateProcassor.State = m_StateManager.TreeTp;
            return;
        }
        float dif = Mathf.Abs(flightDuration - elapse_time);
        if (dif < 0.3f && !isLanding)
        {
            m_Animator.SetTrigger("Landing");
            isLanding = true;
            if (jump_target.collider.tag == "String")
                m_Animator.SetBool("IsString", true);
            else
                m_Animator.SetBool("IsString", false);
        }

        int playerLayer = LayerMask.GetMask(new string[] { "Player" });
        if (m_Prediction.m_HitStringPoint.point != Vector3.zero)
        {
            if (Physics.CheckSphere(m_Prediction.m_HitStringPoint.point, 3f, playerLayer))
            {
                if (Physics.CheckSphere(m_Prediction.m_HitStringPoint.point, 0.5f, playerLayer) && !isEscape)
                {
                    waitFrame = 0;
                    Fall();
                    return;
                }
                //回避行動
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump"))
                {
                    if(m_Prediction.m_IsString)
                        m_Prediction.m_HitString.SideUpdate(m_Shooter.m_SideNumber);
                    else
                        m_Prediction.m_HitNet.SideUpdate(m_Shooter.m_SideNumber);
                    m_Shooter.StringShoot(move_start, m_Prediction.m_HitStringPoint.point);
                    m_Animator.SetTrigger("Escape");
                    isEscape = true;
                }
            }
        }
    }

    //糸の上での移動
    void StringTpMove()
    {
        if (m_hitinfo.collider.tag == "Net")
            m_StateManager.StateProcassor.State = m_StateManager.TreeTp;
        if (IsChangedNumber())
            return;
        m_Animator.SetBool("IsString", true);
        if (m_hitinfo.collider.GetComponent<StringUnit>().m_SideNumber != m_Shooter.m_SideNumber)
            m_StateManager.StateProcassor.State = m_StateManager.Falling;
        var line = m_hitinfo.collider.GetComponent<LineRenderer>();
        //糸のベクトル
        Vector3 stringVec = (line.GetPosition(1) - line.GetPosition(0)).normalized;
        //上方向ベクトル
        Vector3 up = Vector3.Cross(m_Camera.right, stringVec);
        //糸のベクトルを90度回転したベクトル
        Vector3 stringVertical = Quaternion.Euler(0, 90f, 0) * stringVec;
        float angle = Vector3.Angle(m_Camera.right, stringVec);
        Vector3 vec = Vector3.zero;
        Vector3 move = Vector3.zero;
        //４方向に向きを変えていく
        if (angle > 45f && angle < 135f)
        {
            if (up.y > 0)
                stringVec = -stringVec;
            vec = stringVec;
            move = stringVec * Input.GetAxis("Vertical");
            IntersectString(Input.GetAxis("Horizontal"));
            m_Animator.SetFloat("StringMoveX", 0);
            m_Animator.SetFloat("StringMoveZ", Input.GetAxis("Vertical"));
        }
        else if(angle < 45f)
        {
            vec = -stringVertical;
            move = stringVec * Input.GetAxis("Horizontal");
            IntersectString(Input.GetAxis("Vertical"));
            m_Animator.SetFloat("StringMoveZ", 0);
            m_Animator.SetFloat("StringMoveX", Input.GetAxis("Horizontal"));
        }
        else if(angle > 135f)
        {
            vec = stringVertical;
            move = -stringVec * Input.GetAxis("Horizontal");
            IntersectString(Input.GetAxis("Vertical"));
            m_Animator.SetFloat("StringMoveZ", 0);
            m_Animator.SetFloat("StringMoveX", Input.GetAxis("Horizontal"));
        }
        transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.forward, vec, 0.2f));
        transform.Translate(move * m_Speed * 1.3f * Time.deltaTime, Space.World);
        transform.position = MoveRange(transform.position, line.GetPosition(0), line.GetPosition(1));

        //一人称視点変更
        if (Input.GetKeyDown(KeyCode.L) || Input.GetButtonDown("LB"))
        {
            m_StateManager.StateProcassor.State = m_StateManager.TreeFp;
        }

        Vector3 start = transform.position + transform.up * 0.5f;
        Vector3 origin = start + (m_Camera.position - transform.position).normalized;
        Ray ray = new Ray(origin, m_Camera.forward);
        Jump(ray, m_hitinfo);
    }

    //落下状態での動作
    void FallingMove()
    {
        if (m_failureTime < 0.5f)
        {
            m_failureTime += Time.deltaTime;
            return;
        }
        float dis = Vector3.Distance(move_start, move_end);
        //落下スピード
        Gravity.y += Physics.gravity.y * Time.deltaTime;
        float fallTime = Mathf.Abs(dis / Gravity.y);
        transform.Translate(Gravity * Time.deltaTime, Space.World);
        Vector3 forward = Vector3.Cross(m_Camera.right, m_hitinfo.normal);
        transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.forward, forward, 0.1f), m_hitinfo.normal);

        //m_fallElapse += Time.deltaTime;
        //if(m_fallElapse > fallTime)
        //{
        //    Gravity = Vector3.zero;
        //    elapse_time = 0;
        //    m_fallElapse = 0;
        //    m_failureTime = 0;
        //    m_Animator.SetTrigger("Landing");
        //    if (m_hitinfo.collider.tag == "Ground")
        //        m_StateManager.StateProcassor.State = m_StateManager.GroundTp;
        //    else
        //        m_StateManager.StateProcassor.State = m_StateManager.TreeTp;
        //}
    }
    //体当たり状態
    void BodyBlowMove()
    {
        if (!Depression()) return;
        if (Projection(move_start, move_end, jump_target.normal, m_Angle))
        {
            transform.position = move_end;
            transform.rotation = Quaternion.LookRotation(Vector3.Cross(m_Camera.right, jump_target.normal), jump_target.normal);
            if (isEscape)
                m_Shooter.StringShoot(m_Prediction.m_HitStringPoint.point, move_end);
            else
                m_Shooter.StringShoot(move_start, move_end);
            waitFrame = 0;
            isEscape = false;
            isLanding = false;
            if (jump_target.collider.tag == "String")
            {
                m_hitinfo = jump_target;
                m_StateManager.StateProcassor.State = m_StateManager.StringTp;
                return;
            }
            m_StateManager.StateProcassor.State = m_StateManager.TreeTp;
            return;
        }
        LayerMask playerLayer = LayerMask.GetMask(new string[] { "Player" });
        if (Physics.CheckSphere(move_end, 2f, playerLayer) && !isLanding)
        {
            m_Animator.SetTrigger("Tackle");
            foreach(Collider col in Physics.OverlapSphere(move_end, 1f, m_EnemyLayer))
            {
                if (m_enemy != col) continue;
                SendingBodyBlow(m_enemy);
            }
            isLanding = true;
            if (jump_target.collider.tag == "String")
                m_Animator.SetBool("IsString", true);
            else
                m_Animator.SetBool("IsString", false);
        }
    }

    //移動
    private Vector3 Move(Vector3 right, Vector3 up)
    {
        Vector3 forward = Vector3.Cross(right, up);
        transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.forward, forward, 0.3f), up);
        Vector3 move = forward * Input.GetAxis("Vertical") + right * Input.GetAxis("Horizontal");
        //アニメーション
        m_Animator.SetFloat("MoveX", Input.GetAxis("Horizontal"));
        m_Animator.SetFloat("MoveZ", Input.GetAxis("Vertical"));
        transform.Translate(move * m_Speed * Time.deltaTime, Space.World);
        return move;
    }

    //ジャンプ
    private void Jump(Ray ray, RaycastHit hit)
    {
        bool jump = false;
        //糸を狙うのかどうか
        if (Input.GetKeyDown(KeyCode.K) || Input.GetButtonDown("RB"))
            isCursor = !isCursor;
        if (isCursor)
        {
            if (!(jump = Physics.Raycast(ray, out jump_target, m_JumpLimit, m_NetLayer)))
                jump = Physics.SphereCast(ray, 1f, out jump_target, m_JumpLimit, m_StringLayer);
        }
        else
            //jump = Physics.SphereCast(ray, 1f, out jump_target, m_JumpLimit, m_TreeLayer);
            jump = Physics.Raycast(ray, out jump_target, m_JumpLimit, m_TreeLayer);

        if (Input.GetKey(KeyCode.K) || Input.GetButton("RB"))
        {
            m_attackTime += Time.deltaTime;
            if (m_attackTime > 1f)
                foreach(Collider col in Physics.OverlapSphere(jump_target.point, 3f, m_EnemyLayer))
                    m_enemy = col;
        }
        if (Input.GetKeyUp(KeyCode.K) || Input.GetButtonUp("RB"))
        {
            m_attackTime = 0;
            m_enemy = null;
        }

        if (jump)
        {
            if (hit.collider.gameObject == jump_target.collider.gameObject)
            {
                if (Vector3.Distance(transform.position, jump_target.point) < jumpLower)
                {
                    m_Prediction.gameObject.SetActive(false);
                    return;
                }
            }
            if (jump_target.transform.tag == "String")
                if (jump_target.transform.GetComponent<StringUnit>().m_SideNumber != m_Shooter.m_SideNumber)
                    return;
            if (jump_target.collider.tag == "Net")
                if (jump_target.transform.GetComponent<Net>().m_SideNumber != m_Shooter.m_SideNumber)
                    return;

            //予測線、カーソル表示
            m_Prediction.gameObject.SetActive(true);
            m_Prediction.SetParameter(transform.position, jump_target.point, m_Angle);
            if (m_enemy != null)
                m_Prediction.SetParameter(transform.position, m_enemy.transform.position, m_Angle);
            m_Prediction.Calculation();
            //ジャンプ
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump"))
            {
                m_Prediction.gameObject.SetActive(false);
                move_start = transform.position;
                move_end = jump_target.point;
                m_Animator.SetTrigger("Jump");
                if (m_enemy != null)
                {
                    Physics.Raycast(m_enemy.transform.position, -m_enemy.transform.up, out jump_target, 1f, m_TreeLayer);
                    move_end = jump_target.point;
                    m_StateManager.StateProcassor.State = m_StateManager.BodyBlow;
                    return;
                }
                m_StateManager.StateProcassor.State = m_StateManager.JumpTp;
            }
            return;
        }
        m_Prediction.gameObject.SetActive(false);
        m_enemy = null;
    }

    private bool Depression()
    {
        waitFrame++;
        if (waitFrame < 10) return false;
        return true;
    }

    //落下
    private void Fall()
    {
        m_Animator.SetTrigger("Failure");
        //落下先情報取得（木を優先）
        Ray ray = new Ray(transform.position, -Vector3.up);
        if(!Physics.Raycast(ray, out m_hitinfo, 100f, m_TreeLayer))
            Physics.Raycast(ray, out m_hitinfo, 100f, m_GroundLayer);
        move_start = transform.position;
        move_end = m_hitinfo.point;
        m_StateManager.StateProcassor.State = m_StateManager.Falling;
    }

    //移動範囲制限
    private Vector3 MoveRange(Vector3 targetPos, Vector3 value_1, Vector3 value_2)
    {
        if (value_1.x <= value_2.x)
            targetPos.x = Mathf.Clamp(targetPos.x, value_1.x, value_2.x);
        else if (value_1.x > value_2.x)
            targetPos.x = Mathf.Clamp(targetPos.x, value_2.x, value_1.x);
        if (value_1.y <= value_2.y)
            targetPos.y = Mathf.Clamp(targetPos.y, value_1.y, value_2.y);
        else if (value_1.y > value_2.y)
            targetPos.y = Mathf.Clamp(targetPos.y, value_2.y, value_1.y);
        if (value_1.z <= value_2.z)
            targetPos.z = Mathf.Clamp(targetPos.z, value_1.z, value_2.z);
        else if (value_1.z > value_2.z)
            targetPos.z = Mathf.Clamp(targetPos.z, value_2.z, value_1.z);

        return targetPos;
    }

    //糸の交差部分の移動
    private void IntersectString(float axis)
    {
        bool isCol = false;
        var colliders = Physics.OverlapSphere(transform.position, 0.1f, m_StringLayer);
        foreach(Collider col in colliders)
        {
            if (col != m_hitinfo.collider)
                isCol = true;
        }
        if (!isCol) return;
        if (axis != 0)
        {
            var unit = StringSearch(m_hitinfo.collider.GetComponent<StringUnit>(), colliders);
            Physics.SphereCast(unit.position + Vector3.up, 0.5f, Vector3.down, out m_hitinfo, 1f, m_StringLayer);
        }
    }
    //糸内のコネクタ検索
    private Transform StringSearch(StringUnit stringUnit, Collider[] colliders)
    {
        float shortest = 100000f;
        Transform result = null;
        foreach(Collider col in colliders)
        {
            foreach (Connecter c in stringUnit.m_Child)
            {
                if (col.gameObject == c.gameObject) continue;
                if (c.tag == "String")
                {
                    StringUnit s = c as StringUnit;
                    Comparision(s.m_PointA, s.transform, ref result, ref shortest);
                    Comparision(s.m_PointB, s.transform, ref result, ref shortest);
                }
                else if (c.tag == "Net")
                {
                    foreach (Connecter c_2 in c.m_Child)
                    {
                        if (col.gameObject == c_2.gameObject) continue;
                        StringUnit s_2 = c_2 as StringUnit;
                        Comparision(s_2.m_PointA, s_2.transform, ref result, ref shortest);
                        Comparision(s_2.m_PointB, s_2.transform, ref result, ref shortest);
                    }
                }
            }
            if (stringUnit.m_StartConnecter.tag == "String")
                Comparision(stringUnit.m_PointA, stringUnit.m_StartConnecter.transform, ref result, ref shortest);
            else if (stringUnit.m_StartConnecter.tag == "Net")
            {
                foreach (Connecter c in stringUnit.m_StartConnecter.m_Child)
                {
                    if (col.gameObject == c.gameObject) continue;
                    StringUnit s = c as StringUnit;
                    Comparision(s.m_PointA, s.transform, ref result, ref shortest);
                    Comparision(s.m_PointB, s.transform, ref result, ref shortest);
                }
            }
            if (stringUnit.m_EndConnecter.tag == "String")
                Comparision(stringUnit.m_PointB, stringUnit.m_EndConnecter.transform, ref result, ref shortest);
            else if (stringUnit.m_EndConnecter.tag == "Net")
            {
                foreach (Connecter c in stringUnit.m_EndConnecter.m_Child)
                {
                    if (col.gameObject == c.gameObject) continue;
                    StringUnit s = c as StringUnit;
                    Comparision(s.m_PointA, s.transform, ref result, ref shortest);
                    Comparision(s.m_PointB, s.transform, ref result, ref shortest);
                }
            }
        }
        return result;
    }
    //一番近い位置
    private void Comparision(Vector3 pos, Transform target, ref Transform result, ref float shortest)
    {
        if (target.tag == "String" || target.tag == "Net")
        {
            float distance = Vector3.Distance(pos, transform.position);
            if (shortest > distance)
            {
                shortest = distance;
                result = target;
            }
        }
    }

    //乗ってる糸の番号が変更した場合
    private bool IsChangedNumber()
    {
        if(m_hitinfo.collider.tag == "Net")
        {
           if (m_hitinfo.collider.GetComponent<Net>().m_SideNumber != m_Shooter.m_SideNumber)
            {
                Fall();
                return true;
            }
        }
        else if(m_hitinfo.collider.tag == "String")
        {
            if (m_hitinfo.collider.GetComponent<StringUnit>().m_SideNumber != m_Shooter.m_SideNumber)
            {
                Fall();
                return true;
            }
        }
        return false;
    }

    public bool IsOnTree()
    {
        return (m_hitinfo.collider.tag == "Tree");
    }

    void OnTriggerEnter(Collider other)
    {
        //地面着地
        if (m_StateManager.StateProcassor.State != m_StateManager.Falling) return;
        if (other.transform.tag == "Ground" || other.transform.tag == "Tree")
        {
            if (m_hitinfo.transform.tag != other.transform.tag) return;
            Gravity = Vector3.zero;
            elapse_time = 0;
            m_failureTime = 0;
            m_Animator.SetTrigger("Landing");
            if (other.transform.tag == "Ground")
                m_StateManager.StateProcassor.State = m_StateManager.GroundTp;
            else
                m_StateManager.StateProcassor.State = m_StateManager.TreeTp;
        }
    }

    void OnDrawGizmos()
    {
    }
}
