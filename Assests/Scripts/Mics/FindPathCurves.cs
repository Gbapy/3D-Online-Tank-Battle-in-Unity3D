using UnityEngine;
using System.Collections;
using System.IO;
using MagicBattle;

public class FindPathCurves : MonoBehaviour {
	public GameObject pathCurvePref;
	public Transform basePoint;
	public Material aiMat;
	private const int TERRAIN_LAYER = 16;

	// Use this for initialization
	void Start () {
		FinePathCurves ();
		ReadPathCurve ();
	}

	void ReadPathCurve() {
		byte[] aiMapData = new byte[1000000];
		Texture2D tex = new Texture2D (1000, 1000, TextureFormat.ARGB32, false);
		tex.filterMode = FilterMode.Point;
		BinaryReader br = new BinaryReader (File.Open (Application.dataPath + "/Depot.aiMap", FileMode.Open));
		br.Read (aiMapData, 0, 1000000);
		br.Close ();
		for(int i=0;i<1000;i++) {
			for(int j=0;j<1000;j++) {
				if(aiMapData[i * 1000 + j] == 255) {
//					GameObject.Instantiate(pathCurvePref,new Vector3 (basePoint.position.x + i, 310.0f, basePoint.position.z - j),Quaternion.identity);
					tex.SetPixel(i+1,1000 - j,Color.black);
				}else{
					tex.SetPixel(i+1,1000 - j,Color.white);
				}
			}
		}
		tex.Apply ();
		aiMat.mainTexture = tex;
	}

	void FinePathCurves() {
		RaycastHit[] mHit;
		RaycastHit rayHit;
		bool flag = false;
		byte[] aiMap = new byte[1000000];
		for(int i=10;i<1000;i++){
			for(int j=10;j<1000;j++){
				flag = false;
				Physics.Raycast(new Ray(basePoint.position + new Vector3(i,100,-j),Vector3.down),out rayHit);
				if(rayHit.collider != null){
					if(rayHit.collider.gameObject.layer == TERRAIN_LAYER){
						if(rayHit.point.y >= 298.0f && rayHit.point.y <= 302.0f){
							float ang = Vector3.Angle(Vector3.up,rayHit.normal);
							if(ang <= 30.0f) {
								mHit = Physics.SphereCastAll(new Ray(rayHit.point + new Vector3(0,30.0f,0),Vector3.down),10.0f);
								foreach(RaycastHit a in mHit) {
									if(a.collider.gameObject.layer != TERRAIN_LAYER) {
										flag = true;
									}
								}
							}else{
								flag = true;
							}
						}else{
							flag = true;
						}
					}else{
						flag = true;
					}
				}else{
					flag = true;
				}
				if(!flag) {
//					GameObject a = (GameObject)GameObject.Instantiate(pathCurvePref,rayHit.point + new Vector3(0,5.0f,0),Quaternion.identity);
					aiMap[i * 1000 + j] = 255;
//					tex.SetPixel(i,1000-j,Color.black);
				}else{
					aiMap[i * 1000 + j] = 0;
//					tex.SetPixel(i,1000-j,Color.white);
				}
			}
		}
		BinaryWriter bw = new BinaryWriter(File.Create(Application.dataPath + "/Depot.aiMap"));
		bw.Write(aiMap);
		bw.Close();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
