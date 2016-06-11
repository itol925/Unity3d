using UnityEngine;

enum ViewState {
	Idle,
	Moving,
	Attacking,
	BeHitting,
};

public class NPCView : MonoBehaviour {
	//红色血条贴图
	private Texture2D blood_red;
	//黑色血条贴图
	private Texture2D blood_black;

	private Vector2 m_bloodSize = new Vector2(30, 4);

	private Camera m_camera;

	private NPC m_npc;
	private Transform m_trans;
	private Animation m_ani;

	private ViewState m_viewState;

	private Vector3 m_targetPos;
	void Start()
	{
		m_camera = Camera.main;   //得到摄像机对象
		m_npc = GetComponent<NPC>();
		m_trans = transform;
		m_ani = GetComponent<Animation> ();

		blood_red = Resources.Load ("Images/blood") as Texture2D;
		blood_black = Resources.Load ("Images/blood2") as Texture2D; 
	}
	void FixedUpdate(){
		if (m_viewState == ViewState.Idle) {
		
		}else if(m_viewState == ViewState.Attacking){
			
		}else if(m_viewState == ViewState.BeHitting){
			
		}else if (m_viewState == ViewState.Moving) {
			float minDis = m_npc.m_data.speed;
			Vector3 direction = m_targetPos - m_trans.localPosition;
			float distance = direction.magnitude;
			if (distance >= minDis) {
				direction.Normalize ();
				direction = direction * m_npc.m_data.speed;
				m_trans.localPosition = m_trans.localPosition + direction;
			} else {
				m_viewState = ViewState.Idle;
			}
		}
	}

	public void MoveTo (Vector3 pos){
		if (m_viewState == ViewState.Moving) {
			return;
		}
		if (m_viewState == ViewState.Attacking) {
			return;
		}
		if (m_viewState == ViewState.BeHitting) {
			return;
		}
		//Debug.Log ("???? moveto:" + pos.ToString ());
		m_targetPos = pos;
		m_trans.LookAt (m_targetPos);
		m_viewState = ViewState.Moving;
		m_ani.Play ("Run");
	}

	public void Attack(NPC defender){
		if (m_viewState == ViewState.Attacking) {
			return;
		}
		if (m_viewState == ViewState.BeHitting) {
			return;
		}
		m_viewState = ViewState.Attacking;

		StartCoroutine(Util.DelayToInvokeDo(() => {
			m_viewState = ViewState.Idle;
		}, 3));
		
		m_ani.Play ("Attack01");
		m_trans.LookAt (defender.transform);
		defender.BeHited (m_npc);
	}

	public void HeHited(){
		if (m_viewState == ViewState.BeHitting) {
			return;
		}
		m_ani.Play ("Damage");
		m_viewState = ViewState.Attacking;
		StartCoroutine(Util.DelayToInvokeDo(() => {
			m_viewState = ViewState.Idle;
		}, 1.5f));
	}

	public void Died(){
		m_ani.Play ("Dead");
		StartCoroutine(Util.DelayToInvokeDo(() => {
			UnityEngine.Object.Destroy(gameObject);
		}, 4));
	}

	void OnGUI()
	{
		if (m_npc == null || m_npc.m_data == null) {
			return;
		}
		//得到NPC头顶在3D世界中的坐标
		//默认NPC坐标点在脚底下，所以这里加上npcHeight它模型的高度即可
		Vector3 worldPosition = new Vector3(m_trans.position.x, m_trans.position.y + m_npc.m_data.Height, m_trans.position.z);
		//根据NPC头顶的3D坐标换算成它在2D屏幕中的坐标
		Vector2 position = m_camera.WorldToScreenPoint(worldPosition);
		//得到真实NPC头顶的2D坐标
		position = new Vector2(position.x, Screen.height - position.y);
		//注解2
		//计算出血条的宽高
		//Vector2 bloodSize = GUI.skin.label.CalcSize (new GUIContent(blood_red));

		float blood_width = (float)m_npc.m_data.HP / (float)m_npc.m_refData.HP * m_bloodSize.x;
		//先绘制黑色血条
		GUI.DrawTexture(new Rect(position.x - (m_bloodSize.x / 2), position.y - m_bloodSize.y, m_bloodSize.x, m_bloodSize.y), blood_black);
		//在绘制红色血条
		GUI.DrawTexture(new Rect(position.x - (m_bloodSize.x / 2), position.y - m_bloodSize.y, blood_width, m_bloodSize.y), blood_red);

		//注解3
		//计算NPC名称的宽高
		Vector2 nameSize = GUI.skin.label.CalcSize(new GUIContent(m_npc.m_data.Name));
		//设置显示颜色为黄色
		GUI.color = Color.yellow;
		//绘制NPC名称
		GUI.Label(new Rect(position.x - (nameSize.x / 2), position.y - nameSize.y - m_bloodSize.y, nameSize.x, nameSize.y), m_npc.m_data.Name);
	}
}
