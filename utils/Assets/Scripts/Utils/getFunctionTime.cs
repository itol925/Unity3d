using UnityEngine;
using System.Collections;

public class getFunctionTime : MonoBehaviour {

	// Use this for initialization
	void Start () {
		// 在一帧内，Time是测不出来时间的
		float t = Time.time;
		testFunction ();
		Debug.Log (string.Format ("1 total {0} ms", Time.time - t));

		// 方案2 Stopwatch 可以用
		System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch ();
		sw.Start ();
		testFunction ();
		sw.Stop ();
		Debug.Log (string.Format ("2 total {0} ms", sw.ElapsedMilliseconds));

		// 方案3 profiler 用 Profiler.BeginSample 和 Profiler.EndSample 但是就是要打开Profiler界面找到那一帧就可以看了
		Profiler.BeginSample ("testFunction");
		testFunction ();
		Profiler.EndSample ();
	}

	void testFunction(){
		for (int i = 0; i < 10000000; i++) {
		}
	}
}
