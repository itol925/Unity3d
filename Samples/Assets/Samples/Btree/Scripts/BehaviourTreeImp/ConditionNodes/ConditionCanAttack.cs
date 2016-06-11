using System;
using BHaviourTree;

class ConditionCanAttack : BConditionNode{
    public int attackDistance = 0;

    public ConditionCanAttack() { 
        this.m_name = "ConditionCanAttack";
    }

    protected override void OnEnter(object input) {
        
    }

    protected override ActionResult Excute(object input, ref string param) {
        Input ipt = input as Input;
		NPC defender = ipt.NPC.FindNPCByDistance (attackDistance);
		if (defender != null) { // 条件相同
			ipt.Target = defender;
            return ActionResult.SUCCESS;
        } else { 
            return ActionResult.FAILURE;
        }        
    }
}
