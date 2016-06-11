using System;
using BHaviourTree;

public class ActionAttack : BActionNode {
    public string m_action = "";

    public ActionAttack() { 
        this.m_name = "ActionAttack";
    }

    protected override void OnEnter(object input) {
        
    }
    protected override ActionResult Excute(object input, ref string param) {
        param = m_action;
        return ActionResult.SUCCESS;
    }
}