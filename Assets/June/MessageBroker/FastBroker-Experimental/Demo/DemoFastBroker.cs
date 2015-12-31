using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DemoFastBroker : MonoBehaviour {

	private static int PACK2(int fragment1_2, int fragment3_4) {
		return (int)(fragment1_2 | (fragment3_4 << 16));
	}

	private static int PACK(int fragment1, int fragment2 = 0, int fragment3 = 0, int fragment4 = 0) {
		return (int)(fragment1 | (fragment2 << 8) | (fragment3 << 16) | (fragment4 << 24));
	}

	static int ROOT = 1;
	static int MESSAGE_1 = PACK(ROOT, 1);			// ((1 << 8)  | ROOT);
	static int MESSAGE_1_2 = PACK2(MESSAGE_1, 1);	// ((1 << 16) | MESSAGE_1);
	static int MESSAGE_2 = PACK(ROOT, 2);			// ((2 << 8)  | ROOT);
	static int ONLY_2 = PACK(2);

	// Use this for initialization
	void Start () {
		Debug.Log("[Demo] Start");
		FastBroker.Instance.Subscribe(ROOT, HandleMessage);
		FastBroker.Instance.Subscribe(MESSAGE_1_2, HandleMessageSpecific);
		FastBroker.Instance.Subscribe(ONLY_2, HandleMessage2);
	}

	public void HandleMessage(int message, IDictionary<string, object> parameters) {
		Debug.Log("[Demo] HandleMessage : " + message + " Params: " + (null != parameters));
	}

	public void HandleMessageSpecific(int message, IDictionary<string, object> parameters) {
		Debug.Log("[Demo] HandleMessageSpecific : " + message + " Params: " + (null != parameters));
	}

	public void HandleMessage2(int message, IDictionary<string, object> parameters) {
		Debug.Log("[Demo] HandleMessage2 : " + message + " Params: " + (null != parameters));
	}

	void Update () {
	
	}

	void OnGUI() {

		if(GUILayout.Button("Publish Root 0x00000001")) {
			Debug.Log("[Demo] Publish ROOT");
			FastBroker.Instance.Publish(ROOT, null);
		}

		if(GUILayout.Button("Publish Message1 0x00000101")) {
			Debug.Log("[Demo] Publish MESSAGE_1");
			FastBroker.Instance.Publish(MESSAGE_1, null);
		}

		if(GUILayout.Button("Publish Message1_2 0x00020101")) {
			Debug.Log("[Demo] Publish MESSAGE_1_2");
			FastBroker.Instance.Publish(MESSAGE_1_2, null);
		}

		if(GUILayout.Button("Publish Message_2 0x00000201")) {
			Debug.Log("[Demo] Publish MESSAGE_2");
			FastBroker.Instance.Publish(ONLY_2, null);
		}

	}
}
