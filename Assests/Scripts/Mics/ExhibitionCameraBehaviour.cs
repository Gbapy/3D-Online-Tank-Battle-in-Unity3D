using UnityEngine;
using System.Collections;
using MagicBattle;

public class ExhibitionCameraBehaviour : MonoBehaviour {
	public Transform exhibitionPos;
	public float rpm = Mathf.PI / 3.0f;
	public Transform backGround;
	
	private float h_Rot = 0.0f;
	private float distance = 40.0f;
	private float height = 0.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(GlobalInfo.exhibitionFlag){
			camera.enabled = true;
			camera.rect = new Rect(0.1f,0.23f,0.55f,0.64f);
			h_Rot += Time.deltaTime * rpm;
			transform.position = exhibitionPos.position + new Vector3(distance * Mathf.Cos(h_Rot),height,distance * Mathf.Sin(h_Rot));
			transform.LookAt(exhibitionPos.position + new Vector3(0,height,0));
			backGround.position = exhibitionPos.position + new Vector3((80.0f - distance) * Mathf.Cos(h_Rot + Mathf.PI),height,(80.0f - distance) * Mathf.Sin(h_Rot + Mathf.PI));
			backGround.LookAt(exhibitionPos.position + new Vector3(0,height,0));
		}else{
			camera.enabled = false;
			if(GlobalInfo.curExhibitionEquip != null){
				DestroyImmediate(GlobalInfo.curExhibitionEquip);
			}
			enabled = false;
		}
	}
	
	void OnSetCamDistance(float dist){
		distance = dist;
	}
	
	void OnSetCamHeight(float hgt){
		height = hgt;
	}	
}
