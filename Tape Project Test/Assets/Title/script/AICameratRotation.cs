using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICameratRotation : MonoBehaviour
{
    public GameObject target;
    public float speed = 10.0f;
    public Camera AIcamera;
    public Vector3 targetPos;
    
    // Use this for initialization
    void Start ()
    {
        target = GameObject.Find("Enemy4");
        targetPos = target.transform.position;
    }
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 cameraPos = transform.position;

        //ターゲットの移動量分、カメラも移動
        transform.position += target.transform.position - targetPos;
        targetPos = target.transform.position;

        //ターゲットを中心にカメラを回転
        Vector3 axis = transform.TransformDirection(Vector3.down);
        transform.RotateAround(target.transform.position, axis, speed * Time.deltaTime / 2);
        

        //カメラとターゲットの距離
        Physics.Raycast(target.transform.position, -transform.forward,5.0f);
        
    }
}
