﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static UnityEngine.GraphicsBuffer;

class IdleState : IState

{

    private Enemy parent;

    public void Enter(Enemy parent)
    {
        this.parent = parent;
       
        this.parent.Reset();
    }

    public void Exit()
    {
        
    }

    public void Update()
    {
        //Change into follow state if the player is close
        if (parent.MyTarget != null)
        {
            parent.ChangeState(new PathState());
        }
    }
}

