using UnityEngine;
using System.Collections;
using System.IO;
using MagicBattle;

public class TerrainManager : MonoBehaviour {
	public Transform terrain;
	public TerrainData terData;
	
	private TreeInstance[] trInstance;
//	public Material mat;
//	
//	private Texture2D treeTex;

	// Use this for initialization
	void Start () {
		terrain = GameObject.Find ("Terrain").transform;
//		WriteTreeData ();
		ReadTreeData ();
		terData.treeInstances = trInstance;
	}

	void WriteTreeData() {
		BinaryWriter bw = new BinaryWriter (File.Open (Application.dataPath + GlobalInfo.treeDataFileName[(int)GlobalInfo.curBattleField], FileMode.Create));
		trInstance = terData.treeInstances;
		bw.Write (trInstance.Length);
		for(int i = 0;i < trInstance.Length;i++) {
			bw.Write(trInstance[i].color.a);
			bw.Write(trInstance[i].color.r);
			bw.Write(trInstance[i].color.g);
			bw.Write(trInstance[i].color.b);
			bw.Write(trInstance[i].heightScale);
			bw.Write(trInstance[i].widthScale);
			bw.Write(trInstance[i].lightmapColor.a);
			bw.Write(trInstance[i].lightmapColor.r);
			bw.Write(trInstance[i].lightmapColor.g);
			bw.Write(trInstance[i].lightmapColor.b);
			bw.Write(trInstance[i].position.x);
			bw.Write(trInstance[i].position.y);
			bw.Write(trInstance[i].position.z);
			bw.Write(trInstance[i].prototypeIndex);
		}
		bw.Close ();
	}

	void ReadTreeData() {
		BinaryReader br = new BinaryReader (File.Open (Application.dataPath + GlobalInfo.treeDataFileName[(int)GlobalInfo.curBattleField], FileMode.Open));
		trInstance = new TreeInstance[br.ReadInt32 ()];
		for(int i = 0;i < trInstance.Length;i++) {
			trInstance[i] = new TreeInstance();
			float a = br.ReadSingle();
			float r = br.ReadSingle();
			float g = br.ReadSingle();
			float b = br.ReadSingle();
			trInstance[i].color = new Color(r,g,b,a);
			trInstance[i].heightScale = br.ReadSingle();
			trInstance[i].widthScale = br.ReadSingle();
			a = br.ReadSingle();
			r = br.ReadSingle();
			g = br.ReadSingle();
			b = br.ReadSingle();
			trInstance[i].lightmapColor = new Color(r,g,b,a);
			r = br.ReadSingle();
			g = br.ReadSingle();
			b = br.ReadSingle();
			trInstance[i].position = new Vector3(r,g,b);
			trInstance[i].prototypeIndex = br.ReadInt32();
		}
		br.Close ();
	}

	// Update is called once per frame
	void DetectTreeCollision (Vector3 collidePoint) {
		float dist = 0.0f;
		int tIndex = 0;
		float sqrm = 0.0f;

//		trInstance = terData.treeInstances;
		Vector3 pos = terrain.position + new Vector3(trInstance[0].position.x * 1000.0f,trInstance[0].position.y * 600.0f,trInstance[0].position.z * 1500.0f);
		Vector3 b = pos - collidePoint;
		dist = b.sqrMagnitude;
		for(int i = 1;i < trInstance.Length;i++){
			pos = terrain.position + new Vector3(trInstance[i].position.x * 1000.0f,trInstance[i].position.y * 600.0f,trInstance[i].position.z * 1500.0f);
			b = pos - collidePoint;
			sqrm = b.sqrMagnitude;
			if(dist > sqrm){
				dist = sqrm;
				tIndex = i;
			}
		}

		if(dist < 2.0f){
//			int n = 0;
			trInstance[tIndex].position = Vector3.zero;
//			TreeInstance[] tmp = new TreeInstance[trInstance.Length - 1];
//			for(int i = 0;i < trInstance.Length;i++) {
//				if(i != tIndex){
//					n++;
//					if(n != 0)tmp[n - 1] = trInstance[i];
//				}
//			}
			terData.treeInstances = trInstance;
//			trInstance = tmp;
		}
	}

	void OnCollisionEnter(Collision col) {
		DetectTreeCollision (col.contacts [0].point);
	}
}
