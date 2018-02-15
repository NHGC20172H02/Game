using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum JumpMode
{
    NormalJump,         //通常
    CapturingJump,      //占領ジャンプ
    StringJump,          //糸ジャンプ
    Bodyblow
}

public partial class Player : Character
{
    [Header("移動速度")]
    public float m_Speed = 5f;
    [Header("ジャンプの限界距離")]
    public float m_JumpLimit = 50f;
    [Header("ジャンプの射角")]
    public float m_Angle = 30f;
    [Header("地面から木にジャンプする際の高さ")]
    public float m_GroundJumpHeight = 5f;
    [Header("地面から木にジャンプする際の前方向の距離")]
    public float m_GroundJumpForward = 5f;
    [Header("体当たりの速さ")]
    public float m_BodyblowSpeed = 30f;
    public float jumpLower = 5f;
    public Transform m_Camera;
    public GameObject m_CameraPivot;
    public GameObject m_Enemy;
    public LayerMask m_GroundLayer, m_TreeLayer, m_StringLayer, m_NetLayer, m_EnemyLayer;
    public PredictionLine m_Prediction;
    public Animator m_Animator;
    public StringShooter m_Shooter;
    public PlayerStateManager m_StateManager;           //Player状態管理
    public AudioSource m_AudioSource;
    public List<AudioClip> m_AudioClips;                //0:回避、1:攻撃、2:ジャンプ着地、3:落下着地、4:風、5:敵狙った時、6:Lボタン切り替え
    public GameObject m_EscapeSphere;                   //回避の範囲
    public ParticleSystem m_WindLine;
    public AnimationCurve m_AttackSpeed;

    static readonly float MIN = -100f;                   //行動範囲
    static readonly float MAX = 100f;                    //行動範囲
    static readonly float EscapeTime = 0.5f;             //回避入力の間隔
    private Vector3 m_center;                           //中心点
    private RaycastHit m_hitinfo;                       //足元の情報
    private RaycastHit m_prevHit;
    private Vector3 move_start;                         //ジャンプ始点
    private Vector3 move_end;                           //ジャンプ終点
    private RaycastHit jump_target;
    private JumpMode m_JumpMode = JumpMode.NormalJump;
    private bool isTargetString = false;
    private bool isEscape = false;
    private int waitFrame = 0;
    private float m_failureTime = 0;
    private bool isLanding = false;
    private bool isFlyable = false;
    private float m_escapeInterval = 0;
    private float m_treeWaitTime = 0;                   //同じ木の滞在時間
    private float m_attackRate = 0;
    private TargetCategory m_category = TargetCategory.Connecter;
    private List<GameObject> m_trees = new List<GameObject>();
    private int m_jumpableNum = -1;

    protected override void Start()
    {
        m_center = transform.position + transform.up * 0.5f;
        m_StateManager.GroundTp.p_exeDelegate = GroundMove;
        m_StateManager.TreeTp.p_exeDelegate = TreeTpMove;
        m_StateManager.TreeFp.p_exeDelegate = TreeFpMove;
        m_StateManager.JumpTp.p_exeDelegate = JumpTpMove;
        m_StateManager.StringTp.p_exeDelegate = StringTpMove;
        m_StateManager.Falling.p_exeDelegate = FallingMove;
        m_StateManager.BodyBlow.p_exeDelegate = BodyBlowMove;
        m_StateManager.GroundJump.p_exeDelegate = GroundJump;
        m_StateManager.ProximityAttack.p_exeDelegate = ProximityAttack;
        m_Prediction.SetActive(true);
        foreach(GameObject g in GameObject.FindGameObjectsWithTag("Tree"))
        {
            m_trees.Add(g);
        }
    }

    protected override void Update()
    {
        m_center = transform.position + transform.up * 0.5f;
        transform.position = MoveRange(transform.position, new Vector3(MIN, 0, MIN), new Vector3(MAX, 50f, MAX));
        if (isBodyblow)
        {
            m_Prediction.SetActive(false);
            m_StateManager.StateProcassor.State = m_StateManager.Falling;
        }
        //Debug.Log(m_StateManager.StateProcassor.State);
    }

    //移動
    private Vector3 Move(Vector3 right, Vector3 up)
    {
        Vector3 forward = Vector3.Cross(right, up);
        transform.rotation = Quaternion.LookRotation(forward, up);
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
        Vector3 move = forward * vertical + transform.right * horizontal;
        //アニメーション
        m_Animator.SetFloat("MoveX", horizontal);
        m_Animator.SetFloat("MoveZ", vertical);
        vertical = Mathf.Abs(vertical);
        horizontal = Mathf.Abs(horizontal);
        transform.Translate(move * m_Speed * Time.deltaTime, Space.World);
        return move;
    }

    //ジャンプ
    private void Jump(Ray ray, RaycastHit hit)
    {
        isFlyable = false;
        bool jump = false;
        bool bodyBlow = false;
        float addLimit = 0;
        //糸を狙うのかどうか
        if (Input.GetKeyUp(KeyCode.K) || Input.GetButtonDown("LB"))
        {
            //m_enemy = null;
            isTargetString = !isTargetString;
            m_AudioSource.PlayOneShot(m_AudioClips[6]);
        }
        if (hit.collider.tag == "Tree")
        {
            var tree = hit.collider.GetComponent<Tree>();
            if(tree.m_SideNumber == m_Shooter.m_SideNumber)
                addLimit = tree.m_TerritoryRate;
        }
        else if (hit.collider.tag == "String")
        {
            var s = hit.collider.GetComponent<StringUnit>();
            addLimit = Vector3.Distance(s.m_PointA, s.m_PointB);
        }

        List<GameObject> jumpable_tree = new List<GameObject>();
        jumpable_tree.Add(m_Enemy);
        foreach (GameObject g in m_trees)
        {
            if (Vector3.Distance(transform.position, g.transform.position) < m_JumpLimit + addLimit
                && g != m_hitinfo.collider.gameObject)
                jumpable_tree.Add(g);
        }

        if (isTargetString)
        {
            if (!(jump = Physics.Raycast(ray, out jump_target, m_JumpLimit + addLimit, m_NetLayer)))
                jump = Physics.SphereCast(ray, 1f, out jump_target, m_JumpLimit + addLimit, m_StringLayer);
        }
        else
            jump = Physics.Raycast(ray, out jump_target, m_JumpLimit + addLimit, m_TreeLayer);

        if (Input.GetKeyDown(KeyCode.J) || Input.GetButtonDown("RB"))
        {
            m_AudioSource.PlayOneShot(m_AudioClips[5]);
            if (m_jumpableNum == -1)
            {
                m_category = TargetCategory.Enemy;
                m_jumpableNum++;
            }
            else if(jumpable_tree.Count >= m_jumpableNum)
            {
                if (jumpable_tree.Count - 1 == m_jumpableNum)
                {
                    m_category = TargetCategory.Connecter;
                    m_jumpableNum = -1;
                    m_Prediction.SetActive(false);
                    return;
                }
                else
                {
                    m_category = TargetCategory.JumpableTree;
                    m_jumpableNum++;
                }
            }

        }
        if (Input.GetAxis("Horizontal2") != 0 || Input.GetAxis("Vertical2") != 0)
        {
            m_category = TargetCategory.Connecter;
            m_jumpableNum = -1;
            m_Prediction.SetActive(false);
            return;
        }
        if (m_category == TargetCategory.Enemy)
        {
            bodyBlow = true;
            jump = false;
            m_JumpMode = JumpMode.Bodyblow;
            Vector3 dir = m_Enemy.transform.position - m_center;
            m_CameraPivot.transform.rotation = Quaternion.LookRotation(Vector3.Lerp(m_CameraPivot.transform.forward, dir, 0.1f), Vector3.up);
        }
        else if (m_category == TargetCategory.JumpableTree)
        {
            bodyBlow = false;
            jump = true;
            if(jumpable_tree.Count > m_jumpableNum)
            {
                Vector3 posY = new Vector3(0, 10f, 0);
                Vector3 dir = (jumpable_tree[m_jumpableNum].transform.position + posY) - m_center;
                m_CameraPivot.transform.rotation = Quaternion.LookRotation(Vector3.Lerp(m_CameraPivot.transform.forward, dir, 0.1f), Vector3.up);
                Ray ableRay = new Ray(m_center, dir);
                float dis = Vector3.Distance(m_center, jumpable_tree[m_jumpableNum].transform.position + posY);
                jump = Physics.Raycast(ableRay, out jump_target, dis, m_TreeLayer);
            }
        }

        if (jump)
        {
            if (hit.collider.gameObject == jump_target.collider.gameObject)
            {
                if (Vector3.Distance(transform.position, jump_target.point) < jumpLower)
                {
                    m_Prediction.SetActive(false);
                    return;
                }
            }
            if (jump_target.transform.tag == "String" || jump_target.transform.tag == "Net")
                if (jump_target.transform.GetComponent<Connecter>().m_SideNumber != m_Shooter.m_SideNumber)
                    return;

            float dis = Vector3.Distance(transform.position, jump_target.point);
            if (dis > m_JumpLimit)
            {
                if (hit.collider.tag == "Tree")
                    m_JumpMode = JumpMode.CapturingJump;
                else if (hit.collider.tag == "String")
                    m_JumpMode = JumpMode.StringJump;
            }
            else
                m_JumpMode = JumpMode.NormalJump;
            //予測線、カーソル表示
            m_Prediction.SetActive(true);
            m_Prediction.SetParameter(transform.position, jump_target.point, m_Angle, m_Shooter.m_SideNumber, m_JumpMode, m_category);
            m_Prediction.Calculation();
            isFlyable = true;
            //ジャンプ
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump"))
            {
                m_WindLine.Play();
                m_AudioSource.PlayOneShot(m_AudioClips[4]);
                m_Prediction.SetActive(false);
                TreeRateMinus();
                move_start = transform.position;
                move_end = jump_target.point;
                m_Animator.SetTrigger("Jump");
                m_Animator.SetBool("IsJump", true);
                m_escapeInterval = 0;
                isFlyable = false;
                if (m_hitinfo.collider != jump_target.collider)
                    m_treeWaitTime = 0;
                JumpCalculation(move_start, move_end, m_Angle);
                m_StateManager.StateProcassor.State = m_StateManager.JumpTp;
            }
            return;
        }
        else if (bodyBlow)
        {
            //体当たり
            float len = Vector3.Distance(m_Enemy.transform.position, m_center);
            var enemy = m_Enemy.GetComponent<EnemyAI4>();
            Ray dirRay = new Ray(m_center + transform.forward, (m_Enemy.transform.position - m_center));
            bool isAttackable = len < m_JumpLimit && m_hitinfo.collider.gameObject != enemy.nearObj && !Physics.Raycast(dirRay, len - 1f, m_TreeLayer) && enemy.TreeDist();
            m_Prediction.SetActive(true);
            m_Prediction.SetParameter(transform.position, m_Enemy.transform.position, 1f, m_Shooter.m_SideNumber, JumpMode.NormalJump, m_category, isAttackable);
            m_Prediction.Calculation();
            if (isAttackable && (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump")))
            {
                //体当たり実行
                m_WindLine.Play();
                m_AudioSource.PlayOneShot(m_AudioClips[4]);
                m_Prediction.SetActive(false);
                if(m_hitinfo.collider.tag == "Tree")
                    m_hitinfo.collider.GetComponent<Tree>().m_TerritoryRate -= JumpDemeritRate;
                move_start = transform.position;
                move_end = m_Enemy.transform.position;
                m_Animator.SetTrigger("Jump");
                m_Animator.SetBool("IsJump", false);
                m_escapeInterval = 0;
                JumpCalculation(move_start, move_end, m_Angle);
                m_StateManager.StateProcassor.State = m_StateManager.BodyBlow;
            }
            return;
        }
        else if (!jump && Physics.Raycast(ray, m_JumpLimit + 100f, m_TreeLayer))
        {
            //届かない場合の予測線描画
            m_Prediction.SetActive(true);
            m_Prediction.SetParameter(
                transform.position,
                m_Camera.position + m_Camera.forward * (m_JumpLimit + addLimit), 
                m_Angle, m_Shooter.m_SideNumber, m_JumpMode, TargetCategory.None);
            m_Prediction.Calculation();
            return;
        }
        m_Prediction.SetActive(false);
        m_Prediction.m_HitStringPoint = Vector3.zero;
        //m_enemy = null;
    }

    private bool Depression()
    {
        waitFrame++;
        if (waitFrame < 5) return false;
        if(waitFrame == 5)
        {
            m_AudioSource.PlayOneShot(m_AudioClips[4]);
            if(m_StateManager.StateProcassor.State != m_StateManager.GroundJump)
            {
                m_WindLine.Play();
            }
        }
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

    //糸の交差部分移動
    private void IntersectString(int[] layerMask)
    {
        float shortest = 100000f;
        RaycastHit result = new RaycastHit();
        for (int i = 0; i < layerMask.Length; i++)
        {
            if (i > 0 && result.collider != null) break;
            foreach (RaycastHit hit in Physics.SphereCastAll(transform.position + Vector3.up, 1f, Vector3.down, 1f, layerMask[i]))
            {
                if ((hit.collider.tag == "String" || hit.collider.tag == "Net")
                    && (hit.collider.GetComponent<Connecter>().m_SideNumber != m_Shooter.m_SideNumber))
                    continue;

                bool isChild = false;
                var connecter = m_hitinfo.collider.GetComponent<Connecter>();
                GameObject hitObject = hit.collider.gameObject;
                foreach (var c in connecter.m_Child)
                {
                    if (c.gameObject != hitObject)
                        continue;
                    isChild = true;
                }

                if(!isChild && (hitObject == connecter.m_StartConnecter.gameObject || hitObject == connecter.m_EndConnecter.gameObject))
                    isChild = true;

                if (isChild)
                    Comparision(hit.point, hit, ref result, ref shortest);
            }
        }
        if (result.collider == null) return;
        RaycastHit treeHit;
        if(result.collider.tag == "Tree")
        {
            Vector3 dir = (m_center - m_hitinfo.transform.position).normalized;
            Ray ray = new Ray(m_center - dir, dir);
            Physics.Raycast(ray, out treeHit, 3f, m_TreeLayer);
            if (treeHit.collider != null)
                result = treeHit;
        }
        m_hitinfo = result;
        transform.rotation = Quaternion.LookRotation(Vector3.Cross(m_CameraPivot.transform.right, result.normal), result.normal);
        if (result.collider.tag == "Net" || result.collider.tag == "Tree")
        {
            m_StateManager.StateProcassor.State = m_StateManager.TreeTp;
        }
        else if(result.collider.tag == "String")
        {
            m_StateManager.StateProcassor.State = m_StateManager.StringTp;
        }
    }
    //一番近い位置を求める
    private void Comparision(Vector3 pos, RaycastHit target, ref RaycastHit result, ref float shortest)
    {
        if (m_hitinfo.collider == target.collider) return;
        float distance = Vector3.Distance(pos, transform.position);
        if (shortest > distance)
        {
            shortest = distance;
            result = target;
        }
    }

    //乗ってる糸の番号が変更した場合
    private bool IsChangedNumber()
    {
        if((m_hitinfo.collider.tag == "Net" && m_hitinfo.collider.GetComponent<Net>().m_SideNumber != m_Shooter.m_SideNumber)
            || (m_hitinfo.collider.tag == "String" && m_hitinfo.collider.GetComponent<StringUnit>().m_SideNumber != m_Shooter.m_SideNumber))
        {
            Fall();
            return true;
        }
        return false;
    }

    //アニメータートリガーをリセット
    private void ResetTrigger()
    {
        m_Animator.ResetTrigger("Jump");
        //m_Animator.ResetTrigger("Failure");
        m_Animator.ResetTrigger("Escape");
        m_Animator.ResetTrigger("Tackle");
        m_Animator.ResetTrigger("NormalJump");
    }
    private void TreeRateMinus()
    {
        switch (m_JumpMode)
        {
            case JumpMode.StringJump:
                {
                    StringAllMinus();
                    break;
                }
            default:
                {
                    if (m_hitinfo.collider != jump_target.collider && m_hitinfo.collider.tag == "Tree")
                        m_hitinfo.collider.GetComponent<Tree>().m_TerritoryRate -= JumpDemeritRate;
                    break;
                }
        }
    }

    //ジャンプパラメータをリセット
    private void JumpReset()
    {
        transform.position = move_end;
        transform.rotation = Quaternion.LookRotation(Vector3.Cross(m_Camera.right, jump_target.normal), jump_target.normal);
        switch (m_JumpMode)
        {
            case JumpMode.CapturingJump:
                {
                    break;
                }
            case JumpMode.Bodyblow:
                {
                    break;
                }
            case JumpMode.StringJump:
                {
                    StringAllMinus();
                    m_Shooter.StringShoot(move_start, move_end);
                    break;
                }
            default:
                {
                    if (isEscape)
                        m_Shooter.StringShoot(m_Prediction.m_HitStringPoint, move_end);
                    else
                        m_Shooter.StringShoot(move_start, move_end);
                    break;
                }
        }
        waitFrame = 0;
        m_jumpableNum = -1;
        isEscape = false;
        isLanding = false;
        m_Prediction.m_HitStringPoint = Vector3.zero;
        m_EscapeSphere.SetActive(false);
        m_category = TargetCategory.Connecter;
        m_WindLine.Stop();
    }
    //対象の糸に繋がっている木をすべてマイナス
    private void StringAllMinus()
    {
        List<Connecter> connecters = new List<Connecter>();
        Connecter origin = m_hitinfo.collider.GetComponent<Connecter>();
        connecters.Add(origin);
        StringSearch(origin, ref connecters);
    }
    private void StringSearch(Connecter connecter, ref List<Connecter> connecters)
    {
        foreach(Connecter c in connecter.m_Child)
        {
            IsUsedConnecter(c, ref connecters);
        }

        Connecter start = connecter.m_StartConnecter;
        Connecter end = connecter.m_EndConnecter;
        IsUsedConnecter(start, ref connecters);
        IsUsedConnecter(end, ref connecters);

    }
    //すでに調べた糸かどうか(調べていない場合は検索)
    private void IsUsedConnecter(Connecter connecter, ref List<Connecter> connecters)
    {
        foreach(Connecter c in connecters)
        {
            if (c.gameObject == connecter.gameObject) return;
        }
        if(connecter.gameObject.tag == "Tree")
        {
            connecter.GetComponent<Tree>().m_TerritoryRate -= StringDemeritRate;
            connecters.Add(connecter);
            return;
        }

        connecters.Add(connecter);
        StringSearch(connecter, ref connecters);
    }

    //落下着地時の各値リセット
    private void LandingReset(Collider other)
    {
        ResetBodyblow();
        elapse_time = 0;
        m_failureTime = 0;
        m_treeWaitTime = 0;
        m_jumpableNum = -1;
        m_AudioSource.PlayOneShot(m_AudioClips[3]);
        m_Animator.SetTrigger("Landing");
        m_category = TargetCategory.Connecter;
        if (other.transform.tag == "Ground")
            m_StateManager.StateProcassor.State = m_StateManager.GroundTp;
        else
            m_StateManager.StateProcassor.State = m_StateManager.TreeTp;
    }

    /*** プレイヤー以外が必要な関数 ***/
    //木に乗ってるかどうか
    public bool IsOnTree()
    {
        if (m_hitinfo.collider == null) return false;
        var state = m_StateManager.StateProcassor.State;
        return (m_hitinfo.collider.tag == "Tree" 
            && (state == m_StateManager.TreeTp || state == m_StateManager.TreeFp));
    }

    //自分の乗っている木を取得
    public GameObject GetOnTree()
    {
        if (m_hitinfo.collider == null) return null;
        var state = m_StateManager.StateProcassor.State;
        if (m_hitinfo.collider.tag == "Tree" )//&& (state == m_StateManager.TreeTp || state == m_StateManager.TreeFp))
            return m_hitinfo.collider.gameObject;
        return null;
    }
    //ターゲットしている木を取得
    public GameObject GetTargetTree()
    {
        if (jump_target.collider == null) return null;
        if (m_hitinfo.collider == null) return null;
        var state = m_StateManager.StateProcassor.State;
        //if (state == m_StateManager.TreeTp || state == m_StateManager.TreeFp
        //    || state == m_StateManager.StringTp || state == m_StateManager.GroundTp)
            return jump_target.collider.gameObject;
        return null;
    }
    //同じ木にプレイヤーが一定時間滞在しているか（引数 : 秒数）
    public bool IsOnTreeTime(float second)
    {
        if (m_hitinfo.collider == null) return false;
        if (m_hitinfo.collider.tag != "Tree") return false;
        var state = m_StateManager.StateProcassor.State;
        if (!(state == m_StateManager.TreeTp || state == m_StateManager.TreeFp)) return false;
        if (m_treeWaitTime < second) return false;
        return true;
    }
    //攻撃中かどうか
    public bool IsAttack()
    {
        return m_StateManager.StateProcassor.State == m_StateManager.BodyBlow;
    }
    //ジャンプ可能かどうか
    public bool IsFlyable()
    {
        return isFlyable;
    }

	// ジャンプ進行率
	public float JumpProgress()
	{
		return flightDuration == 0 ? 0 : elapse_time/m_JumpSpeed / flightDuration;
	}

    //ターゲットしている糸を取得
    public GameObject GetTargetString()
    {
        if (m_hitinfo.collider == null) return null;
        if (jump_target.collider == null) return null;
        if (jump_target.collider.tag != "String") return null;
        return jump_target.collider.gameObject;
    }

    /*******************************************/

    void OnTriggerEnter(Collider other)
    {
        //地面着地
        if (m_StateManager.StateProcassor.State != m_StateManager.Falling) return;
        if (other.transform.tag == "Ground" || other.transform.tag == "Tree")
        {
            if (m_hitinfo.transform.tag != other.transform.tag) return;
            LandingReset(other);
        }
    }
}
