using UnityEngine;
using System.Collections;

public class TankPosAndRotManager : MonoBehaviour {
	private const float TIMEOUT = 0.5f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(networkView.isMine) {
			networkView.RPC("SetPosAndRot",RPCMode.Others,transform.position,transform.rotation);
		}
	}

	[RPC]
	void SetPosAndRot(Vector3 pos,Quaternion rot) {
		if (networkView.isMine)return;
		transform.position = pos;
		transform.rotation = rot;
	}
}
