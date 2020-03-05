using UnityEngine;
using System.Collections;
using MagicBattle;

public class SeaFloorBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GlobalInfo.water_Level = transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
