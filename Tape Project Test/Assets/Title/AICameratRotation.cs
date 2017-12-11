using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICameratRotation : MonoBehaviour {

    public float m_Hight;
    public Transform target;
    public float speed = 10.0f;
    //public GameObject AIcamera;

    //Ray
    //protected Ray _ray;
    //protected RaycastHit _rayhit;

    //エディタで通常時のシェーダと透明用のシェーダを指定する
    //public Shader defaultShader;    //通常時
    //public Shader alphaShader;      //透明時

    // Use this for initialization
    void Start ()
    {
        m_Hight = this.gameObject.transform.position.y;
    }
	
	// Update is called once per frame
	void Update ()
    {
        //ターゲットを中心にカメラを回転
        Vector3 axis = transform.TransformDirection(Vector3.down);
        transform.RotateAround(target.position, axis, speed * Time.deltaTime / 2);

        //カメラの後方にレイを飛ばし近くのオブジェクトに当たったら
        //当たった位置にカメラを移動
        //RaycastHit hit;
        //if (Physics.Raycast(target.transform.position, -transform.forward, out hit))
        //{
        //    transform.position = hit.point;
        //}

        //_ray = new Ray(AIcamera.transform.position, -(AIcamera.transform.position - target.transform.position));
        //if (Physics.Raycast(_ray, out _rayhit))
        //{
        //    if (_rayhit.collider.tag == "Tree")
        //    {
        //        //Rayが当たっている間は対象オブジェクトのシェーダをalphaShaderにする
        //        Shader targetShader = _rayhit.collider.gameObject.GetComponent<Renderer>().material.shader;
        //        if (targetShader != alphaShader)
        //        {
        //            targetShader = alphaShader;
        //        }
        //    }
        //    else
        //    {
        //        //Rayが当たってない時はdefaultShaderに戻す
        //    }
        //}
    }
}
