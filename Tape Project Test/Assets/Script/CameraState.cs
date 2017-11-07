using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CameraState
{
    public class StateProcessor
    {
        private CameraState m_State;
        public CameraState State
        {
            get { return m_State; }
            set { m_State = value; }
        }

        //実行
        public void Execute()
        {
            State.Execute();
        }

        public void C_Execute()
        {
            State.C_Execute();
        }
    }

    public abstract class CameraState
    {
        //デリゲート
        public delegate void executeState();
        public executeState p_exeDelegate;
        public executeState c_exeDelegate;

        //プレイヤー実行
        public virtual void Execute()
        {
            if(p_exeDelegate != null)
            {
                p_exeDelegate();
            }
        }

        //カメラ実行
        public virtual void C_Execute()
        {
            if (c_exeDelegate != null)
            {
                c_exeDelegate();
            }
        }

        //ステート名の取得
        public abstract string getStateName();
    }

    public class GroundTp : CameraState
    {
        public override string getStateName()
        {
            return "GroundTp";
        }
    }
    public class TreeTp : CameraState
    {
        public override string getStateName()
        {
            return "TreeTp";
        }
    }
    public class TreeFp : CameraState
    {
        public override string getStateName()
        {
            return "TreeFp";
        }
    }
    public class JumpTp : CameraState
    {
        public override string getStateName()
        {
            return "JumpTp";
        }
    }
}
