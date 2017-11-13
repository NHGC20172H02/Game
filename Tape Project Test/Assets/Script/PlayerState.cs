using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerState
{
    public class StateProcessor
    {
        private PlayerState m_State;
        public PlayerState State
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

    public abstract class PlayerState
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

    //地上にいる状態
    public class GroundTp : PlayerState
    {
        public override string getStateName()
        {
            return "GroundTp";
        }
    }
    //木の上にいる状態（俯瞰）
    public class TreeTp : PlayerState
    {
        public override string getStateName()
        {
            return "TreeTp";
        }
    }
    //木の上にいる状態（一人称視点）
    public class TreeFp : PlayerState
    {
        public override string getStateName()
        {
            return "TreeFp";
        }
    }
    //ジャンプ中の状態
    public class JumpTp : PlayerState
    {
        public override string getStateName()
        {
            return "JumpTp";
        }
    }
    //糸の上にいる状態
    public class StringTp : PlayerState
    {
        public override string getStateName()
        {
            return "StringTp";
        }
    }
}
