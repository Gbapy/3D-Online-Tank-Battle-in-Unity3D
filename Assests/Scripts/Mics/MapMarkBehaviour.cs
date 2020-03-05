using UnityEngine;
using System.Collections;
using MagicBattle;

public class MapMarkBehaviour : MonoBehaviour {
	public Transform target;
	public Transform map;
	public Transform mapBasePoint;
	public bool myMarkFlag = false;
	public Transform cam;

	private Vector2[] battleFieldSize = new Vector2[10];
	private bool mapInitialized = false;
	// Use this for initialization
	void Start () {
		transform.localPosition = map.localPosition + new Vector3 (0, 6, 0);

		if(myMarkFlag){
			cam.position = transform.position + new Vector3(0.0f,1.0f,0.0f);
			cam.localRotation=Quaternion.Euler(new Vector3(90,180,0));
			cam.parent = transform;
		}
		battleFieldSize[0] = new Vector2(1000.0f,1000.0f);
		battleFieldSize[1] = new Vector2(1000.0f,1500.0f);
		battleFieldSize[2] = new Vector2(1000.0f,1500.0f);
		battleFieldSize[3] = new Vector2(1000.0f,1500.0f);
		battleFieldSize[4] = new Vector2(1000.0f,1500.0f);
		battleFieldSize[5] = new Vector2(1000.0f,1500.0f);
		battleFieldSize[6] = new Vector2(1000.0f,1500.0f);
		battleFieldSize[7] = new Vector2(1000.0f,1500.0f);
		battleFieldSize[8] = new Vector2(1000.0f,1500.0f);
		battleFieldSize[9] = new Vector2(1000.0f,1500.0f);
		if(GlobalInfo.curBattleField == BattleFieldKind.TrainPaceNight) GlobalInfo.curBattleField = BattleFieldKind.TrainPlace; 
	}
	
	// Update is called once per frame
	void Update () {
		if (target != null) {
			Vector2 pos = Vector2.zero;
			Vector3 bs = GameObject.Find("BasePoint").transform.position;

			pos = new Vector2(target.position.x - bs.x,bs.z - target.position.z);
			pos = new Vector2(mapBasePoint.position.x +  pos.x * 10.0f / battleFieldSize[(int)GlobalInfo.curBattleField].x
			                  ,mapBasePoint.position.z - pos.y * 10.0f / battleFieldSize[(int)GlobalInfo.curBattleField].y);
			Vector3 tmp = new Vector3(target.forward.x,0,target.forward.z);
			tmp.Normalize();
			float ang = Vector3.Angle(tmp,Vector3.forward);
			if(Vector3.Angle(tmp,Vector3.right) > 90.0f) ang = -ang;
			transform.position = new Vector3(pos.x,map.localPosition.y + 0.1f,pos.y);
			ang += 180.0f;
			transform.localRotation = Quaternion.AngleAxis(ang,new Vector3(0,1,0)); 
			mapInitialized = true;
		}else{
			if(mapInitialized){
				DestroyImmediate(gameObject);
				if(myMarkFlag){
					GameObject[] go = GameObject.FindGameObjectsWithTag("map");
					foreach(GameObject a in go){
						DestroyImmediate(a);
					}			
				}
			}
		}
	}
}
