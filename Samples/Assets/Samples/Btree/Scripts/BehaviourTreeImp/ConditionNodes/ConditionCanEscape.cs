using System;
using BHaviourTree;

class ConditionCanEscape : BConditionNode{
	public float RemainHP = 0;

    public ConditionCanEscape() { 
        this.m_name = "ConditionCanEscape";
    }

    protected override void OnEnter(object input) {
        
    }

    protected override ActionResult Excute(object input, ref string param) {
        Input ipt = input as Input;
		if (ipt.NPC.m_data.HP < RemainHP) {
			return ActionResult.SUCCESS;
		} else {
			return ActionResult.FAILURE;
		}
    }
}
