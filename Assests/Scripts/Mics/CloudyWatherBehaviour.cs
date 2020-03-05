using UnityEngine;
using System.Collections;
using MagicBattle;

public class CloudyWatherBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GlobalInfo.nightOrNoonFlag = -1;
		GlobalInfo.fogFlag = true;
	}
}
