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

    public class ThreadSearchTree : EnemyState
    {
        public override string getStateName()
        {
            return "ThreadSearchTree";
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

    public class StringCount : EnemyState
    {
        public override string getStateName()
        {
            return "StringCount";
        }
    }

    public class StringJump : EnemyState
    {
        public override string getStateName()
        {
            return "StringJumpt";
        }
    }

    public class StringJumping : EnemyState
    {
        public override string getStateName()
        {
            return "StringJumping";
        }
    }

    public class StringDecision : EnemyState
    {
        public override string getStateName()
        {
            return "StringDecisiont";
        }
    }

    public class ThreadSearchRandom : EnemyState
    {
        public override string getStateName()
        {
            return "ThreadSearchRandom";
        }
    }

    public class Fall : EnemyState
    {
        public override string getStateName()
        {
            return "Fall";
        }
    }

    public class Falling : EnemyState
    {
        public override string getStateName()
        {
            return "Falling";
        }
    }

    public class FallGroundMove : EnemyState
    {
        public override string getStateName()
        {
            return "FallGroundMove";
        }
    }

    public class PredominanceDecision : EnemyState
    {
        public override string getStateName()
        {
            return "PredominanceDecision";
        }
    }

    public class PredominanceStringCount : EnemyState
    {
        public override string getStateName()
        {
            return "PredominanceStringCount";
        }
    }

    public class PredominanceMyTree : EnemyState
    {
        public override string getStateName()
        {
            return "PredominanceMyTree";
        }
    }

    public class PredominanceJump : EnemyState
    {
        public override string getStateName()
        {
            return "PredominanceJump";
        }
    }

    public class PredominanceJumpMove : EnemyState
    {
        public override string getStateName()
        {
            return "PredominanceJumpMove";
        }
    }

    public class LatterHalfDecision : EnemyState
    {
        public override string getStateName()
        {
            return "LatterHalfDecision";
        }
    }

    public class LatterHalfColorlessTree : EnemyState
    {
        public override string getStateName()
        {
            return "LatterHalfColorlessTree";
        }
    }

    public class AttackJump : EnemyState
    {
        public override string getStateName()
        {
            return "AttackJump";
        }
    }

    public class AttackJumpMove : EnemyState
    {
        public override string getStateName()
        {
            return "AttackJumpMove";
        }
    }
    public class AttackRearJump : EnemyState
    {
        public override string getStateName()
        {
            return "AttackRearJump";
        }
    }
    public class AttackRearJumpMove : EnemyState
    {
        public override string getStateName()
        {
            return "AttackRearJumpMove";
        }
    }
}
