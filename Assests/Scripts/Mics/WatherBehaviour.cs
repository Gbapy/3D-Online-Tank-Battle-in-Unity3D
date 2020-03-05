using UnityEngine;
using System.Collections;
using System.IO;
using MagicBattle;

public class WatherBehaviour : MonoBehaviour {	
	public int mapWidth;
	public int mapHeight;
	public string aiMapFileName;
	public Material noonMat;
	public Material nightMat;
	public Color nightFogColor;
	public Color nightAmbientColot;
	public Color nightGlobalLightColor;
	public Color nightGlobalFogColor;
	public Color noonFogColor;
	public Color noonAmbientColot;
	public Color noonGlobalLightColor;
	public Color noonGlobalFogColor;
	public Terrain ter;

	// Use this for initialization
	void Start () {
		if(Network.isServer){
			OnSetWather();
		}else{
			GlobalInfo.rpcControl.RPC("OnRequestWatherInfoRPC",RPCMode.Server);
		}
//		ReadPathCurve ();
	}
	
	void ReadPathCurve() {
		byte[] aiMapRawData = new byte[mapWidth * mapHeight];
		GlobalInfo.aiMapData = new bool[mapWidth, mapHeight];
		BinaryReader br = new BinaryReader (File.Open (Application.dataPath + "/" +aiMapFileName, FileMode.Open));
		br.Read (aiMapRawData, 0, mapWidth * mapHeight);
		br.Close ();
		GlobalInfo.mapWidth = mapWidth;
		GlobalInfo.mapHeight = mapHeight;
		for(int i=0;i<mapWidth;i++) {
			for(int j=0;j<mapHeight;j++) {
				if(aiMapRawData[i * mapHeight + j] == 255) {
					GlobalInfo.aiMapData[i,j] = true;
				}else{
					GlobalInfo.aiMapData[i,j] = false;
				}
			}
		}
	}

	void OnSetWather() {
		Light globalLight = GameObject.FindGameObjectWithTag("GlobalLight").light;
		RenderSettings.fog = true;
		if(GlobalInfo.nightOrNoonFlag == 1){
			RenderSettings.skybox = noonMat;
			RenderSettings.fogColor = noonFogColor;
			RenderSettings.ambientLight = noonAmbientColot;
			globalLight.color = noonGlobalLightColor;
//			globalLight.shadowStrength = 1.0f;
			Camera.main.GetComponent<ColorCorrectionCurves>().enabled = true;
			Camera.main.GetComponent<GlobalFog>().globalFogColor = noonGlobalFogColor;
		}else if(GlobalInfo.nightOrNoonFlag == 0){
			RenderSettings.skybox = nightMat;
			RenderSettings.fogColor = new Color(0,0,0,1.0f);
			globalLight.color = Color.black;
			globalLight.intensity = 1.0f;
			RenderSettings.ambientLight = new Color(0.039f,0.0471f,0.0824f,1);
			//9.984f,12.032f,21.0944f
//			globalLight.shadowStrength = 1.0f;
			Camera.main.GetComponent<ColorCorrectionCurves>().enabled = false;
			Camera.main.GetComponent<GlobalFog>().globalFogColor = new Color(0.123f,0.0157f,0.1f);
		}
		Camera.main.GetComponent<UltraRedRayCamera> ().enabled = false;
		Camera.main.GetComponent<GlobalFog> ().enabled = true;
		if(GlobalInfo.fogFlag){
			Camera.main.GetComponent<GlobalFog>().globalDensity = 0.05f;
			Camera.main.GetComponent<GlobalFog>().startDistance = 20.0f;
			Camera.main.GetComponent<GlobalFog>().heightScale = 600.0f;
		}else{
			Camera.main.GetComponent<GlobalFog>().globalDensity = 0.0f;
			Camera.main.GetComponent<GlobalFog>().startDistance = 30.0f;
			Camera.main.GetComponent<GlobalFog>().heightScale = 100.0f;
		}
	}
}
