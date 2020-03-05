using UnityEngine;
using System.Collections;
using MagicBattle;
	
public class WaponSelectButtonBehaviour : MonoBehaviour {
	public float left = 0.0f;
	public float top = 0.0f;
	public float width = 0.0f;
	public float height = 0.0f;
	public GameObject prefab;
	public Transform exhibitionPos;
	public EquipmentKind equipmentKind = EquipmentKind.PlayerTank1;
	public Transform exhibitionCamera;
	public float camDistance = 40.0f;
	public float camHeight = 3.0f;
	public bool defaultEquipment = false;
	
	// Use this for initialization
	void Start () {
		if(defaultEquipment)
			OnMouseUp();
	}
	
	// Update is called once per frame
	void Update () {
		if(GlobalInfo.logged && !GlobalInfo.gameStarted){
			guiTexture.enabled = true;
			guiTexture.pixelInset = new Rect(Screen.width * left -  Screen.width * 0.5f,Screen.height * 0.5f - Screen.height * height - Screen.height * top,Screen.width * width,Screen.height * height);
		}else{
			guiTexture.enabled = false;
		}
	}
	
	void OnMouseUp() {
		GlobalInfo.userInfo.equipment = equipmentKind;
		if(GlobalInfo.curExhibitionEquip != null)
			DestroyImmediate(GlobalInfo.curExhibitionEquip);
		GlobalInfo.curExhibitionEquip = (GameObject)Instantiate(prefab,exhibitionPos.position,exhibitionPos.rotation);
		exhibitionCamera.SendMessage("OnSetCamDistance",camDistance,SendMessageOptions.DontRequireReceiver);
		exhibitionCamera.SendMessage("OnSetCamHeight",camHeight,SendMessageOptions.DontRequireReceiver);
	}
}
