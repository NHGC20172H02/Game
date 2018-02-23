using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetCategory
{
    Connecter = 0,  //木、糸、ネット
    Enemy,          //敵
    JumpableTree,   //ジャンプ可能な木
    None            //届かない場合
}

//予測線
public class PredictionLine : MonoBehaviour {

    public LineRenderer m_LineRenderer;
    //public Transform m_Cursor;
    public RectTransform m_Cursor;
    public Camera m_UICamera;
    public Vector3 m_HitStringPoint;
    public StringUnit m_HitString;
    public Net m_HitNet;
    public bool m_IsString = false;
    public GameObject m_Collision;
    public List<Color> m_Colors;
    public List<GameObject> m_AttackableImage;
    public List<Material> m_Materials;

    private Vector3 m_start;             //始点
    private Vector3 m_end;               //終点
    private float m_angle;               //射角
    private Vector3 m_top;              //曲線の頂点
    private Vector3 m_forward;          //予測線の方向
    private List<GameObject> m_collisions;
    private int m_shooterNum = 0;
    private bool m_isCursorActive = true;

    void Start () {
        m_collisions = new List<GameObject>();
        for(int i = 0; i < 8; i++)
        {
            m_collisions.Add(Instantiate(m_Collision, transform));
        }
    }
	
	void Update () {
    }

    public void Calculation()
    {
        Prediction();
        SetLines();
        //カーソル関連
        //m_Cursor = transform.GetChild(0);
        m_Cursor.gameObject.SetActive(m_isCursorActive);
        //m_Cursor.position = m_end - m_forward * 0.3f;
        Vector2 pos = Vector2.zero;
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, m_end);
        var parentRect = m_Cursor.parent.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPos, m_UICamera, out pos);
        m_Cursor.localPosition = pos;
        //m_Cursor.rotation = Quaternion.LookRotation(m_forward);
    }

    //始点、終点、射角、シューター番号、色の設定
    public void SetParameter(Vector3 start, Vector3 end, float angle, int shooterNum, 
        JumpMode mode = JumpMode.NormalJump, TargetCategory category = TargetCategory.Connecter, bool isAttackable = false)
    {
        m_start = start;
        m_end = end;
        m_angle = angle;
        m_shooterNum = shooterNum;
        m_LineRenderer.material = m_Materials[0];
        if(mode != JumpMode.Bodyblow)
            m_LineRenderer.material.SetColor("_TintColor", m_Colors[(int)mode]);
        m_isCursorActive = (category == TargetCategory.Enemy || category == TargetCategory.JumpableTree);
        if (category == TargetCategory.None || (category == TargetCategory.Enemy && !isAttackable))
        {
            m_LineRenderer.material = m_Materials[1];
            if(mode != JumpMode.Bodyblow)
                m_LineRenderer.material.SetColor("_TintColor", m_Colors[(int)mode] / 2);
        }
        if (category != TargetCategory.Enemy)
        {
            m_AttackableImage[0].SetActive(false);
            m_AttackableImage[1].SetActive(false);
            return;
        }
        m_AttackableImage[0].SetActive(isAttackable);
        m_AttackableImage[1].SetActive(!isAttackable);
    }

    //弾道計算
    private void Prediction()
    {
        float target_Distance = Vector3.Distance(m_start, m_end);

        //初速度
        float V0 = target_Distance / (Mathf.Sin(2 * m_angle * Mathf.Deg2Rad) / 9.8f);
        float vSin = Mathf.Sqrt(V0) * Mathf.Sin(m_angle * Mathf.Deg2Rad);
        //最高到達点
        float h = vSin * vSin / (2 * -Physics.gravity.y);

        m_forward = (m_end - m_start).normalized;
        m_top = m_start + m_forward * target_Distance / 2 + new Vector3(0, h, 0);
        //曲線の頂点を通る処理
        m_top.x = (4 * m_top.x - m_start.x - m_end.x) / 2;
        m_top.y = (4 * m_top.y - m_start.y - m_end.y) / 2;
    }

    //ベジェ曲線計算
    private Vector3 BezierCurve(float t)
    {
        if (t > 1f)
            t = 1f;

        Vector3 result = Vector3.zero;
        float cmp = 1f - t;
        result.x = cmp * cmp * m_start.x + 2 * cmp * t * m_top.x + t * t * m_end.x;
        result.y = cmp * cmp * m_start.y + 2 * cmp * t * m_top.y + t * t * m_end.y;
        result.z = cmp * cmp * m_start.z + 2 * cmp * t * m_top.z + t * t * m_end.z;
        return result;
    }

    //LineRendererにポイントを設定
    private void SetLines()
    {
        var posList = new List<Vector3>();

        posList.Add(m_start);
        float len = 0f;
        int stringLayer = LayerMask.GetMask(new string[] { "String" });
        //Lineを10本表示
        for(int i = 1; i < 11; i++)
        {
            len += 0.1f;
            posList.Add(BezierCurve(len));
            if (i > 9 || i < 2) continue;
            //予測線のCollision設定
            Vector3 dir = (posList[i] - posList[i - 1]).normalized;
            float dis = Vector3.Distance(posList[i], posList[i - 1]);
            m_collisions[i - 2].transform.position = posList[i] - dir * dis / 2;
            m_collisions[i - 2].transform.rotation = Quaternion.LookRotation(dir);
            m_collisions[i - 2].GetComponent<BoxCollider>().size = new Vector3(0.1f, 0.1f, dis);
        }

        m_LineRenderer.positionCount = posList.Count;
        m_LineRenderer.SetPositions(posList.ToArray());
    }

    //子のCollision情報受け取り
    public void ReceiveChildCollision(Collision collision)
    {
        int hitNumber = 0;
        foreach (ContactPoint point in collision.contacts)
        {
            if (collision.transform.tag == "Net")
            {
                m_HitNet = collision.transform.GetComponent<Net>();
                m_IsString = false;
                hitNumber = m_HitNet.m_SideNumber;
            }
            else if(collision.transform.tag == "String")
            {
                m_HitString = collision.transform.GetComponent<StringUnit>();
                m_IsString = true;
                hitNumber = m_HitString.m_SideNumber;
            }
            if (hitNumber == m_shooterNum) return;
            m_HitStringPoint = point.point;
        }
    }

    //表示するかどうか
    public void SetActive(bool active)
    {
        m_Cursor.gameObject.SetActive(active);
        gameObject.SetActive(active);
    }
}
