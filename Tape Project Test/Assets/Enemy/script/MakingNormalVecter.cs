using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakingNormalVecter : MonoBehaviour {

    List<GameObject> vectors = new List<GameObject>();

    Mesh mesh { set { this.mesh = value; } get { return GetComponent<MeshFilter>().sharedMesh; } }

    private Vector3[] vertices, normals;


    // Use this for initialization
    void Start () {
        var vector = Resources.Load("Vector");

        vertices = mesh.vertices;
        normals = mesh.normals;


        mesh.vertices = vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            vectors.Add((GameObject)Instantiate(vector, this.transform.position + vertices[i], Quaternion.identity));
            vectors[i].transform.LookAt(normals[i]);
            vectors[i].transform.rotation *= Quaternion.Euler(90, 0, 90);
            vectors[i].transform.parent = this.transform;

        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
