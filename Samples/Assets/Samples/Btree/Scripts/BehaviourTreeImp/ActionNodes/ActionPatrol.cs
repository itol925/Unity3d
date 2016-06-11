using System;
using BHaviourTree;

public class ActionPatrol : BActionNode {
    public string m_action = "";

    public ActionPatrol() { 
        this.m_name = "ActionPatrol";
    }

    protected override void OnEnter(object input) {
        
    }
    protected override ActionResult Excute(object input, ref string param) {
        param = m_action;
        return ActionResult.SUCCESS;
    }
}
