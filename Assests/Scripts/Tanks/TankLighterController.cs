using UnityEngine;
using System.Collections;
using MagicBattle;

public class TankLighterController : MonoBehaviour {
	public GameObject lighter;
	private bool lightUpFlag = false;

	// Use this for initialization
	void Start () {
		if (!networkView.isMine)return;
		if (GlobalInfo.nightOrNoonFlag == 1) {
			lightUpFlag = false;
			lighter.SetActive(false);
		} else if(GlobalInfo.nightOrNoonFlag == 0) {
			lightUpFlag = true;
			lighter.SetActive(true);
			networkView.RPC("OnSetLighterStateRPC",RPCMode.Others,true);
		}
	}

	// Update is called once per frame
	void Update () {
		if (!networkView.isMine)return;
		if (GlobalInfo.nightOrNoonFlag == 0) {
			if (Input.GetKeyDown (KeyCode.V)) {
				lightUpFlag = !lightUpFlag;
				if (lightUpFlag) {
					lighter.SetActive (true);
				} else {
					lighter.SetActive (false);
				}
				networkView.RPC("OnSetLighterStateRPC",RPCMode.Others,lightUpFlag);
			}
		}	
	}
	
	[RPC]
	void OnSetLighterStateRPC(bool state) {
		lighter.SetActive (state);
	}
}
