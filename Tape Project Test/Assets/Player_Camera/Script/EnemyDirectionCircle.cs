using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDirectionCircle : MonoBehaviour
{
    static readonly float offset = 0.2f;
    static readonly Vector3 sideTextPos = new Vector3(60.0f, 0, 0);
    static readonly float ringRadius = 1.5f;
    static readonly float treeRingRadius = 1.3f;

    public Transform m_Player;
    public Transform m_Enemy;
    public Transform m_CameraPivot;
    public Camera m_MainCamera;
    public GameObject m_Circle;
    public Transform m_Ring;
    public Transform m_RingArrow;
    public GameObject m_TreeArrowPrefab;
    public GameObject m_TreeArrowIcon;
    public RectTransform m_DirectionIcon;
    public RectTransform m_IconText;
    public Material m_RingMaterial;
    public AnimationCurve m_Gradation_Yellow;
    public GameObject m_DangerText;

    private List<GameObject> m_trees = new List<GameObject>();
    private List<GameObject> m_treeRingArrows = new List<GameObject>();
    private List<GameObject> m_treeArrowIcons = new List<GameObject>();

    private bool isGradation = false;
    private float gradationRate = 0;
    private float gradationSpeed = 0.2f;

    void Start()
    {
        m_RingMaterial.color = new Color(0f, 0f, 1f);
        m_trees.AddRange(GameObject.FindGameObjectsWithTag("Tree"));
        for (int i = 0; i < m_trees.Count; i++)
        {
            m_treeRingArrows.Add(Instantiate(m_TreeArrowPrefab, m_Ring));
            m_treeArrowIcons.Add(Instantiate(m_TreeArrowIcon, m_Circle.transform.parent));
        }
    }

    void LateUpdate()
    {
        if (!isGradation)
            isGradation = m_Enemy.GetComponent<EnemyAI4>().AttackPreparation();
        AttackAlert();

        float dis = Vector3.Distance(m_Player.position, m_MainCamera.transform.position);
        if(dis < 2.5f)
        {
            m_Ring.gameObject.SetActive(false);
            m_RingArrow.gameObject.SetActive(false);
            m_Circle.SetActive(true);
            foreach (GameObject t in m_treeArrowIcons)
            {
                if (t == null) continue;
                t.SetActive(true);
            }
            CircleUpdate();
        }
        else
        {
            foreach (GameObject t in m_treeArrowIcons)
            {
                if (t == null) continue;
                t.SetActive(false);
            }
            m_Circle.gameObject.SetActive(false);
            m_Ring.gameObject.SetActive(true);
            m_RingArrow.gameObject.SetActive(true);
            RingUpdate();
        }

        m_DangerText.GetComponent<Flashing>().FlashUpdate();
    }

    //サークル状態での更新
    private void CircleUpdate()
    {
        bool isActive = false;
        Vector3 position = Vector3.zero;
        Quaternion rotation = Quaternion.identity;

        CircleIconUpdate(m_Enemy, ref isActive, ref position, ref rotation, true);
        m_DirectionIcon.gameObject.SetActive(isActive);
        if (isActive)
        {
            m_DirectionIcon.localPosition = position;
            m_DirectionIcon.localRotation = rotation;
        }

        for (int i = 0; i < m_trees.Count; i++)
        {
            Tree tree = m_trees[i].GetComponent<Tree>();
            if (tree.m_SideNumber != 0)
            {
                if (m_treeArrowIcons[i] != null)
                    Destroy(m_treeArrowIcons[i]);
                continue;
            }
            if (tree.m_SideNumber == 0 && m_treeArrowIcons[i] == null)
                m_treeArrowIcons[i] = Instantiate(m_TreeArrowPrefab);
            CircleIconUpdate(m_trees[i].transform, ref isActive, ref position, ref rotation, false);
            m_treeArrowIcons[i].SetActive(isActive);
            if (!isActive) continue;
            m_treeArrowIcons[i].transform.localPosition = position;
            m_treeArrowIcons[i].transform.localRotation = rotation;
        }
    }

    void CircleIconUpdate(Transform target, ref bool isActive, ref Vector3 position, ref Quaternion rotation, bool isEnemy)
    {
        Vector3 targetPos = target.position;
        //ビューポート変換
        Vector3 viewPos = m_MainCamera.WorldToViewportPoint(targetPos);
        //カメラに映っていたら表示しない
        if (viewPos.x > 0 && viewPos.x < 1.0f && viewPos.y > 0 && viewPos.y < 1.0f && viewPos.z > 0)
        {
            isActive = false;
            if (isEnemy)
                m_IconText.gameObject.SetActive(false);
            return;
        }
        else
        {
            isActive = true;
        }

        Vector3 playerPos = m_Player.position;
        Vector3 dir = (targetPos - playerPos).normalized;
        //Playerの前方向ベクトル
        Vector3 forward = Vector3.Cross(m_CameraPivot.transform.right, Vector3.up);

        //前方向から対象の方向の角度(Degree)
        float angle = Mathf.Atan2(dir.x, dir.z) - Mathf.Atan2(forward.x, forward.z);

        //スクリーン座標系での対象への方向ベクトル
        Vector3 screenDir = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.up) * Vector3.forward;
        screenDir = new Vector3(screenDir.x, screenDir.z, 0);

        //上下反転
        if (m_CameraPivot.transform.forward.y > 0/* && screenDir.y > 0*/)
        {
            screenDir = new Vector3(screenDir.x, -screenDir.y, 0);
            angle -= Mathf.PI + angle * 2;
        }
        //移動
        position = screenDir * 250f;

        float posX = position.x;
        float posY = position.y;

        //対象の方向へ回転
        rotation = Quaternion.AngleAxis(-angle * Mathf.Rad2Deg, Vector3.forward);

        if (!isEnemy) return;

        //Textの位置を更新
        Vector3 sidePos;
        if (screenDir.x > 0)
            sidePos = sideTextPos;
        else
            sidePos = -sideTextPos;
        m_IconText.localPosition = m_DirectionIcon.localPosition + sidePos;
    }

    //リング状態での更新
    private void RingUpdate()
    {
        Vector3 pos = Vector3.zero;
        Quaternion rotation = Quaternion.identity;
        ArrowUpdate(m_Enemy, ringRadius, ref pos, ref rotation);
        m_RingArrow.position = pos;
        m_RingArrow.rotation = rotation;

        for(int i = 0; i < m_trees.Count; i++)
        {
            Tree tree = m_trees[i].GetComponent<Tree>();
            if (tree.m_SideNumber != 0)
            {
                if (m_treeRingArrows[i] != null)
                    Destroy(m_treeRingArrows[i]);
                continue;
            }
            if (tree.m_SideNumber == 0 && m_treeRingArrows[i] == null)
                m_treeRingArrows[i] = Instantiate(m_TreeArrowPrefab);
            ArrowUpdate(m_trees[i].transform, treeRingRadius, ref pos, ref rotation);
            m_treeRingArrows[i].transform.position = pos;
            m_treeRingArrows[i].transform.rotation = rotation;
        }
    }

    private void ArrowUpdate(Transform target, float radius, ref Vector3 position, ref Quaternion rotation)
    {
        m_Ring.position = m_Player.position + offset * m_Player.up;
        Vector3 targetPos = target.position;
        Vector3 playerPos = m_Player.position;
        Vector3 dir = (targetPos - playerPos);
        //平面での敵の方向
        dir.y = 0;

        Vector3 forward = Vector3.Cross(m_Player.right, m_CameraPivot.up);
        forward.y = 0;
        Vector3 up = Vector3.Cross(forward, dir);
        //角度算出（-180度 ∼ 180度）
        float angle = Vector3.Angle(forward, dir) * (up.y < 0 ? -1 : 1);
        //リング基準の方向
        Vector3 ringToDir = Quaternion.AngleAxis(angle, m_Player.up) * m_Ring.forward;
        rotation = Quaternion.LookRotation(ringToDir, m_Ring.up);
        position = playerPos + offset * m_Player.up + ringToDir * radius;
    }

    private void AttackAlert()
    {
        if (!isGradation) return;
        m_DangerText.SetActive(true);
        gradationRate = Mathf.Clamp(gradationRate + gradationSpeed * Time.deltaTime, 0, 1f);
        Color color = m_RingMaterial.color;
        color.g = m_Gradation_Yellow.Evaluate(gradationRate);
        m_RingMaterial.color = color;

        if (gradationRate == 1f || !m_Enemy.GetComponent<EnemyAI4>().AttackPreparation())
        {
            m_DangerText.SetActive(false);
            gradationRate = 0;
            isGradation = false;
        }
    }
}
