using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyState;

public partial class EnemyAI4 {

    int jumpPos_Min = 6;
    int jumpPos_Max = 22;

    //近くの木
    public Vector3 GetUpPosition2()
    {
        return new Vector3(nearObj2.transform.position.x, Random.Range(jumpPos_Min, jumpPos_Max), nearObj2.transform.position.z);
    }
    //2番目の近くの木
    public Vector3 GetUpPosition3()
    {
        return new Vector3(nearObj3.transform.position.x, Random.Range(jumpPos_Min, jumpPos_Max), nearObj3.transform.position.z);
    }
    //3番目の近くの木
    public Vector3 GetUpPosition4()
    {
        return new Vector3(nearObj4.transform.position.x, Random.Range(jumpPos_Min, jumpPos_Max), nearObj4.transform.position.z);
    }


    //誰の陣地でもない近くの木
    public Vector3 GetUpPosition00()
    {
        return new Vector3(nearObj0.transform.position.x, Random.Range(jumpPos_Min, jumpPos_Max), nearObj0.transform.position.z);
    }
    //誰の陣地でもない近くの木
    public Vector3 GetUpPosition02()
    {
        return new Vector3(nearObj02.transform.position.x, Random.Range(jumpPos_Min, jumpPos_Max), nearObj02.transform.position.z);
    }

    //自分の陣地ではない近くの木
    public Vector3 GetUpPosition40()
    {
        return new Vector3(nearObj40.transform.position.x, Random.Range(jumpPos_Min, jumpPos_Max), nearObj40.transform.position.z);
    }
    //自分の陣地ではない２番目の近くの木
    public Vector3 GetUpPosition50()
    {
        return new Vector3(nearObj50.transform.position.x, Random.Range(jumpPos_Min, jumpPos_Max), nearObj50.transform.position.z);
    }


    //自分の陣地の近くの木
    public Vector3 MyTreePosition1()
    {
        return new Vector3(myTreeObj.transform.position.x, Random.Range(jumpPos_Min, jumpPos_Max), myTreeObj.transform.position.z);
    }
    //自分の陣地の2番目に近くの木
    public Vector3 MyTreePosition2()
    {
        return new Vector3(myTreeObj2.transform.position.x, Random.Range(jumpPos_Min, jumpPos_Max), myTreeObj2.transform.position.z);
    }
    //自分の陣地の3番目に近くの木
    public Vector3 MyTreePosition3()
    {
        return new Vector3(myTreeObj3.transform.position.x, Random.Range(jumpPos_Min, jumpPos_Max), myTreeObj3.transform.position.z);
    }


    //近くの木のポジション
    public Vector3 GetPosition()
    {
        return new Vector3(nearObj2.transform.position.x, nearObj2.transform.position.y, nearObj2.transform.position.z);
    }
    //２番目の近くの木のポジション
    public Vector3 GetPosition2()
    {
        return new Vector3(nearObj3.transform.position.x, nearObj3.transform.position.y, nearObj3.transform.position.z);
    }


    //近くの木にジャンプするポジション
    public Vector3 GetPosition3()
    {
        return new Vector3(nearObj2.transform.position.x, 7.0f, nearObj2.transform.position.z);
    }


    //PlayerのPosition
    public Vector3 GetPlayerPosition()
    {
        return new Vector3(playerObj.transform.position.x, playerObj.transform.position.y, playerObj.transform.position.z);
    }
}
