using UnityEngine;
using System.Collections;

public class Map : MonoBehaviour {
	const float minX = -5;
	const float maxX = 5;
	const float minZ = -5;
	const float maxZ = 5;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public Vector3 RandPosition(){
		Vector3 pos = new Vector3 (Util.Random(minX, maxX), 0.1f, Util.Random(minZ, maxZ));
		return pos;
	}
}
