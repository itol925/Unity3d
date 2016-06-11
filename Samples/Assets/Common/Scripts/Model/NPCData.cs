
public enum NPC_STATE { 
	Idle, 
	Attacking, 
	Patroling,
	Hited
}

public class NPCData {
	public int Id = 0;
	public int HP = 0;
	public int Attack = 0;
	public int Defence = 0;
	public float Height = 0;
	public string Name = "";
	public float speed = 0.01f;
}
