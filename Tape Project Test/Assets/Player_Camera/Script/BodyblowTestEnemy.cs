using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyblowTestEnemy : Character {

    public GameObject target;

	// Use this for initialization
	protected override void Start () {
		
	}
	
	// Update is called once per frame
	protected override void Update  () {
        if (Input.GetKeyDown(KeyCode.T))
        {
            //体当たり成功時に呼び出すやつ(引数:GameObject型)
            SendingBodyBlow(target);
        }

        //体当たりを食らっている間の動作(ここは自由に書く)
        if (isBodyblow)
        {
            /***ここはTestでやっているだけ**********************************/
            //gravity.y += Physics.gravity.y * Time.deltaTime;
            //transform.Translate(gravity * Time.deltaTime, Space.World);
            /***************************************************************/

            //例えばUpdateでisBodyblowがtrueになるとStateを落下状態に変えるとか
            //State = Falling;
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if(collider.tag == "Ground")
        {
            //体当たり着地時に呼び出す
            ResetBodyblow();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 v = Reflection(target.transform.position - transform.position, target.transform.up).normalized;
        Gizmos.DrawRay(target.transform.position, v);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, target.transform.position);
    }
}
