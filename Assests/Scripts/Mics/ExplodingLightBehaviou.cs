using UnityEngine;
using System.Collections;
using MagicBattle;

public class ExplodingLightBehaviou : MonoBehaviour {
	private float psTime = 0.0f;

	// Use this for initialization
	void Start () {
		if(GlobalInfo.nightOrNoonFlag == 1)
			light.enabled = false;
		else
			light.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (psTime < 3.0f) {
			psTime += Time.deltaTime;
			light.intensity = light.intensity / (psTime * 5.0f);
		}else{
			GameObject.Destroy(gameObject);
		}
	}
}
