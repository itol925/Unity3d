using System;
using BHaviourTree;

public class ActionEscape : BActionNode {
    public string m_action = "";

    public ActionEscape() { 
        this.m_name = "ActionEscape";
    }

    protected override void OnEnter(object input) {
        
    }
    protected override ActionResult Excute(object input, ref string param) {
        param = m_action;
        return ActionResult.SUCCESS;
    }
}