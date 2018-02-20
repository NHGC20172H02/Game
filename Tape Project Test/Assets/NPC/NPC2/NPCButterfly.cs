using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCButterfly : MonoBehaviour {

    [Header("降下、上昇の移動速度")]
    public float m_move_speed = 1.0f;
    [Header("ステージに停滞している時間")]
    public float Stagnation_time = 10.0f;

    float Descent_distance = 20.0f; //降下距離

    Vector3 vec3;
    Vector3 Start_pos;

    float Start_Pos_Dist;
    float wait_time;
    bool return_move;

	// Use this for initialization
	void Start () {
        return_move = false;

        vec3 = new Vector3(gameObject.transform.position.x, Descent_distance, gameObject.transform.position.z);
        Start_pos = gameObject.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        //降下
		if(gameObject.transform.position.y >= vec3.y && return_move == false)
        {
            transform.Translate(Vector3.down * m_move_speed * Time.deltaTime);
        }
        //停滞
        if(gameObject.transform.position.y <= vec3.y)
        {
            wait_time += Time.deltaTime * 1;
            if(wait_time >= Stagnation_time)
            {
                return_move = true;
            }
        }
        //上昇
        if(return_move == true)
        {
            transform.Translate(Vector3.up * m_move_speed * Time.deltaTime);

            Start_Pos_Dist = Vector3.Distance(Start_pos, this.transform.position);
            if (Start_Pos_Dist <= 1.0f) //初期位置に着いたら死亡
            {
                Destroy(this.gameObject);
            }
        }
    }
}
