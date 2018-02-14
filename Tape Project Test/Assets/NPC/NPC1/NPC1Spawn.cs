using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC1Spawn : MonoBehaviour {

    public Transform NPCSpawn1;
    public Transform NPCSpawn2;
    public Transform NPCSpawn3;
    public Transform NPCSpawn4;

    public GameObject NPC1;
    [System.NonSerialized]
    public float wait_time;
    [System.NonSerialized]
    public int spawn_true;
    [System.NonSerialized]
    public int spawn_count;

    float world_Timer;
    

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(world_Timer <= 42) //前半１分間
        {
            world_Timer += Time.deltaTime * 1;
        }

        if(wait_time <= 2 && world_Timer >= 4)
        wait_time += Time.deltaTime * 1;

        if(wait_time >= 2) //20秒後
        {
            //スポーンするかしないか
            if(spawn_true == 0)
            spawn_true = Random.Range(1, 3);

            if(spawn_true == 1) //スポーンする
            {
                if(spawn_count == 0)
                spawn_count = Random.Range(1, 5);
            }
            else //スポーンしない
            {
                wait_time = 0;
                spawn_true = 0;
            }
        }


        switch (spawn_count)
        {
            case 1:
                Instantiate(NPC1, NPCSpawn1.transform.position, NPCSpawn1.transform.rotation);
                spawn_true = 4;
                spawn_count = 10;
                break;

            case 2:
                Instantiate(NPC1, NPCSpawn2.transform.position, NPCSpawn2.transform.rotation); 
                spawn_true = 4;
                spawn_count = 10;
                break;

            case 3:
                Instantiate(NPC1, NPCSpawn3.transform.position, NPCSpawn3.transform.rotation);
                spawn_true = 4;
                spawn_count = 10;
                break;

            case 4:
                Instantiate(NPC1, NPCSpawn4.transform.position, NPCSpawn4.transform.rotation);
                spawn_true = 4;
                spawn_count = 10;
                break;
        }
	}
}
