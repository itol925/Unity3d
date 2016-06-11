using UnityEngine;
using System.Collections;
using BHaviourTree;
using System.IO;
using System.Collections.Generic;

public class Btree : MonoBehaviour {
	const string Hero = "Hero/Prefabs/Hero_Prefab";
	const string Monster1 = "Monster/prefabs/mon_goblinWizard";
	const string Monster2 = "Monster/prefabs/mon_orcWarrior";
	const string Monster3 = "Monster/prefabs/mon_orgeHitter";
	const string Monster4 = "Monster/prefabs/mon_trolCurer";
	const string Map1 = "Map/Prefab/map";

	static Btree instance;
	public static Btree Instance{
		get{ 
			if (instance == null) {
				GameObject go = GameObject.Find ("Btree");
				instance = go.GetComponent<Btree> ();
			}
			return instance;
		}
	}

	public ArrayList m_monstList = new ArrayList();
	BehavTree m_btree = null;
	public Map m_map = null;

	// Use this for initialization
	void Start()
	{
		LoadMap ();
		LoadMonsters();
		m_btree = LoadBehaviourTree();
	}

	// Update is called once per frame
	void Update()
	{
		if (m_btree != null)
		{
			for (int i = 0; i < m_monstList.Count; i++)
			{
				NPC npc = m_monstList[i] as NPC;
				if (npc.IsDied)
				{
					continue;
				}
				Input input = npc.CurrentInput;
				List<string> actions = m_btree.Run(input);
				for (int j = 0; j < actions.Count; j++)
				{
					string action = actions[j];
					switch (action)
					{
						case "escape":
							npc.Escape();
							break;
						case "attack":
							npc.Attack();
							break;
						case "patrol":
							npc.Patrol();
							break;
					}
				}
			}
		}
	}
	BehavTree LoadBehaviourTree()
	{
		string treePath = Application.dataPath + "/" + "Samples/Btree/trees/tree1.json";
		if (!File.Exists(treePath))
		{
			//Debug.LogError("behaviour tree file missed!");
			return null;
		}
		BehavTree tree = BehavTreeMagager.Instance.LoadTree(treePath);
		return tree;
	}

	void LoadMonsters()
	{
		NPC npc = CreateNPC(Monster1);
		npc.m_view.transform.position = new Vector3(-4, npc.m_view.transform.position.y, -4);
		NPCData data = new NPCData () { 
			Id = 1, 
			Attack = 20, 
			Defence = 10, 
			Height = 1.8f, 
			HP = 100, 
			Name = "NPC",
			speed = 0.015f
		};
		npc.InitData(data);
		m_monstList.Add(npc);
		//return;
		npc = CreateNPC(Monster2);
		npc.m_view.transform.position = new Vector3(0, npc.m_view.transform.position.y, 0);
		data = new NPCData() { 
			Id = 2, 
			Attack = 20, 
			Defence = 10, 
			Height = 1.8f, 
			HP = 100, 
			Name = "NPC1",
			speed = 0.02f
		};
		npc.InitData(data);
		m_monstList.Add(npc);

	}

	NPC CreateNPC(string path)
	{
		UnityEngine.Object perfab = Resources.Load(path);
		GameObject go = Instantiate(perfab) as GameObject;
		go.transform.SetParent(m_map.transform);

		NPC npc = go.AddComponent<NPC>();
		npc.m_view = go.AddComponent<NPCView> ();
		return npc;
	}

	void LoadMap(){
		UnityEngine.Object perfab = Resources.Load(Map1);
		GameObject go = Instantiate(perfab) as GameObject;
		go.transform.SetParent(transform);
		m_map = go.AddComponent<Map> ();
	}
}
