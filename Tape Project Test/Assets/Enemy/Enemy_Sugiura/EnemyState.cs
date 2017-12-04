using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyState
{
    public class StateProcessor
    {
        private EnemyState m_State;
        public EnemyState State
        {
            get { return m_State; }
            set { m_State = value; }
        }

        //実行
        public void Execute()
        {
            State.Execute();
        }
    }

    public abstract class EnemyState
    {
        //デリゲート
        public delegate void executeState();
        public executeState exeDelegate;

        //実行
        public virtual void Execute()
        {
            if (exeDelegate != null)
            {
                exeDelegate();
            }
        }

        //ステート名の取得
        public abstract string getStateName();
    }

    //地上にいる状態
    public class GroundMove : EnemyState
    {
        public override string getStateName()
        {
            return "GroundMove";
        }
    }

    public class TreeDecision : EnemyState
    {
        public override string getStateName()
        {
            return "TreeDecision";
        }
    }

    public class TreeMove : EnemyState
    {
        public override string getStateName()
        {
            return "TreeMove";
        }
    }

    public class ColorlessTree : EnemyState
    {
        public override string getStateName()
        {
            return "ColorlessTree";
        }
    }

    public class SearchTree : EnemyState
    {
        public override string getStateName()
        {
            return "SearchTree";
        }
    }

    public class SearchRandom : EnemyState
    {
        public override string getStateName()
        {
            return "SearchRandom";
        }
    }

    public class ThreadJump : EnemyState
    {
        public override string getStateName()
        {
            return "ThreadJump";
        }
    } 

    public class Jumping : EnemyState
    {
        public override string getStateName()
        {
            return "Jumping";
        }
    }

    public class GroundJumping : EnemyState
    {
        public override string getStateName()
        {
            return "GroundJumping";
        }
    }

    public class JumpMove : EnemyState
    {
        public override string getStateName()
        {
            return "JumpMove";
        }
    }

    public class StringMove : EnemyState
    {
        public override string getStateName()
        {
            return "StringMove";
        }
    }
}
