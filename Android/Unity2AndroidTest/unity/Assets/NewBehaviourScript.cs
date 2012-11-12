using UnityEngine;
using System.Collections;

public class NewBehaviourScript : MonoBehaviour {
	private static a2uTest.Caller _StaticCaller;
	// Use this for initialization
	void Start () {
		_StaticCaller = new a2uTest.Caller("Static");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
	}
	
	void OnGUI () {
		if (GUILayout.Button("Click me", GUILayout.Width(500), GUILayout.Height(500))) {
			_StaticCaller.makeTests();
			
			a2uTest.Caller caller = new a2uTest.Caller("NOT static");
            caller.makeTests();
		}
	}
}
