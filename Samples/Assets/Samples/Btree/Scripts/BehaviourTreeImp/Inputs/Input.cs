using System;
using BHaviourTree;
using UnityEngine;

public class Input {
	private NPC m_npc;
	public NPC NPC{
		get { return m_npc; }
		set { m_npc = value; }
    }

	public NPC Target{ get; set;}
}

