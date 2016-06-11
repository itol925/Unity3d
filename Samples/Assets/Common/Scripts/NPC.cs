using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NPCView))]

public class NPC : MonoBehaviour {
	public NPCData m_data;
	public NPCData m_refData;

	public NPCView m_view;

	private Input m_input = new Input();
	void Start() {
		m_input.NPC = this;
	}

	public void InitData(NPCData data) {
		m_data = data;
		m_refData = data;
	}

	public NPC FindNPCByDistance(int distance) {
		ArrayList list = Btree.Instance.m_monstList;
		for (int i = 0; i < list.Count; i++) {
			NPC npc = list [i] as NPC;
			if (npc.m_data.Id == m_data.Id) {
				continue;
			}
			float dis = Vector3.Distance (m_view.transform.localPosition, npc.m_view.transform.localPosition);
			if (dis < distance) {
				return npc;
			}
		}
		return null;
	}

	public void Escape() {
		if(IsDied){
			return;
		}
		//Debug.Log ("Escape");
		m_view.MoveTo (Btree.Instance.m_map.RandPosition());
	}
	public void Attack() { 
		//Debug.Log ("Attack");
		if(IsDied){
			return;
		}

		NPC defender = CurrentInput.Target;
		if (defender == null) {
			return;
		}

		if (defender.IsDied) {
			return;		
		}

		m_view.Attack(defender);
	}

	public void BeHited(NPC attacker){
		m_view.HeHited ();
		int hurt = (attacker.m_data.Attack - m_data.Defence);
		if (hurt < 0) {
			hurt = 0;		
		}
		m_data.HP -= hurt;
		Debug.Log ("hp:" + m_data.HP);
		if (m_data.HP < 0) {
			m_data.HP = 0;
		}
	}
	public void Patrol() { 
		if(IsDied){
			return;
		}
		m_view.MoveTo (Btree.Instance.m_map.RandPosition());
		//Debug.Log ("Patrol");
	}
	public void Dead(){
		if(IsDied){
			return;
		}		
		m_view.Died ();
	}

	public bool IsDied { 
		get {
			return m_data.HP <= 0;
		}
	}


	public Input CurrentInput { 
		get {
			return m_input;
		}
	}
}
