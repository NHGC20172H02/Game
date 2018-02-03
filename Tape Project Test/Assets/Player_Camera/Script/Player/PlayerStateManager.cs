using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerState;

public class PlayerStateManager : MonoBehaviour {

    private static PlayerStateManager stateManager;
    public static PlayerStateManager GetInstance
    {
        get { return stateManager; }
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
    private StringTp m_StringTp = new StringTp();
    public StringTp StringTp
    {
        get { return m_StringTp; }
    }
    private Falling m_falling = new Falling();
    public Falling Falling
    {
        get { return m_falling; }
    }
    private BodyBlow m_bodyBlow = new BodyBlow();
    public BodyBlow BodyBlow
    {
        get { return m_bodyBlow; }
    }
    private GroundJump m_groundJump = new GroundJump();
    public GroundJump GroundJump
    {
        get { return m_groundJump; }
    }
    private ProximityAttack m_proximityAttack = new ProximityAttack();
    public ProximityAttack ProximityAttack
    {
        get { return m_proximityAttack; }
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
