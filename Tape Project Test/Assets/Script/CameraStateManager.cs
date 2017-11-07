using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CameraState;

public class CameraStateManager : MonoBehaviour {

    private static CameraStateManager stateManager;
    public static CameraStateManager GetInstance
    {
        get
        {
            if (stateManager == null)
            {
                GameObject g = new GameObject("CameraStateManager");
                stateManager = g.AddComponent<CameraStateManager>();
            }
            return stateManager;
        }
    }

    private StateProcessor m_stateProcessor = new StateProcessor();
    public StateProcessor StateProcassor
    {
        get { return m_stateProcessor; }
    }

    private GroundTp m_GroundTp = new GroundTp();
    public GroundTp GroundTp
    {
        get { return m_GroundTp; }
    }

    private TreeTp m_TreeTp = new TreeTp();
    public TreeTp TreeTp
    {
        get { return m_TreeTp; }
    }
    private TreeFp m_TreeFp = new TreeFp();
    public TreeFp TreeFp
    {
        get { return m_TreeFp; }
    }
    private JumpTp m_JumpTp = new JumpTp();
    public JumpTp JumpTp
    {
        get { return m_JumpTp; }
    }

	// Use this for initialization
	void Start () {
        m_stateProcessor.State = GroundTp;
	}
	
	// Update is called once per frame
	void Update () {
        if (m_stateProcessor.State == null) return;
        
        m_stateProcessor.Execute();
    }

    void LateUpdate()
    {
        if (m_stateProcessor.State == null) return;

        m_stateProcessor.C_Execute();
    }
}
