using UnityEngine;
using System.Collections;

public class BombedSiteBehaviour : MonoBehaviour {
	private float psTime = 0.0f;
	
	const float LIFE_SPAN = 5.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		psTime += Time.deltaTime;
		if(psTime >= LIFE_SPAN){
			Destroy(gameObject);
		}
	}
}
