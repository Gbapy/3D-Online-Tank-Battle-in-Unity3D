using UnityEngine;
using System.Collections;
using MagicBattle;

public class NightLightBehaviour : MonoBehaviour {
	public GameObject lightObj;
	// Use this for initialization
	void Start () {
		if(GlobalInfo.nightOrNoonFlag != 0)
			lightObj.light.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(GlobalInfo.nightOrNoonFlag == 0) {
			lightObj.light.intensity = Random.Range(0.5f,1.0f);
		}
	}
}
