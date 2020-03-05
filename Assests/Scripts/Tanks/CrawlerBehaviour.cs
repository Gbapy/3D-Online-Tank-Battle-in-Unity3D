using UnityEngine;
using System.Collections;

public enum CrawlerVertexState{
	AutoModifiable = 0,
	Interpolate = 1,
	Fixed = 2
}

public class CrawlerVertexInfo{
	public CrawlerVertexState VertexInfo = CrawlerVertexState.Fixed;
	public int vertexIndex = -1;
	public int wheelIndex = -1;
}

public class CrawlerBehaviour : MonoBehaviour {
	public Transform wheelParent;

	private Mesh crawlerMesh;
	private System.Collections.Generic.List<CrawlerVertexInfo> crawlerVerticsInfo = new System.Collections.Generic.List<CrawlerVertexInfo>();
	private Transform[] wheel;
	private int wheelCount = 0;
	private Vector3[] crawlerVertics;
	private float wheelRadius = 0.0f;

	// Use this for initialization
	void Start () {
		InitCrawlerMeshInfo ();
		Vector3[] vert = new Vector3[crawlerVertics.Length];

		vert = crawlerVertics;
		foreach (CrawlerVertexInfo a in crawlerVerticsInfo) {
			if(a.VertexInfo == CrawlerVertexState.AutoModifiable) {
				vert[a.vertexIndex] =  new Vector3(crawlerVertics[a.vertexIndex].x,crawlerVertics[a.vertexIndex].y * 2.0f,crawlerVertics[a.vertexIndex].z);
			}
		}
		crawlerMesh.vertices = vert;
		crawlerMesh.RecalculateNormals ();
//		GetComponent<MeshFilter> ().sharedMesh = crawlerMesh;
	}
	
	// Update is called once per frame
	void Update () {
		for(int i = 0;i < wheelCount;i++) {
		}
	}

	void InitCrawlerMeshInfo() {
		crawlerMesh = GetComponent<MeshFilter> ().mesh;
		crawlerVertics = crawlerMesh.vertices;

		for (int i = 0; i<crawlerVertics.Length; i++) {
			CrawlerVertexInfo cvi = new CrawlerVertexInfo();
			cvi.vertexIndex = i;
			if(crawlerVertics[i].y >= 0.0f){
				cvi.VertexInfo = CrawlerVertexState.Fixed;
			}else{
				Vector3 tp = transform.TransformPoint(crawlerVertics[i]);
				float min = 10000.0f;
				int minI = -1;

				for(int j = 0;j < wheelCount;j++) {
					float dist = (wheel[j].position - tp).magnitude;
					if(dist < min){
						min = dist;minI = j; 
					}
				}
				cvi.VertexInfo = CrawlerVertexState.AutoModifiable;
				cvi.wheelIndex = minI;
			}
			crawlerVerticsInfo.Add(cvi);
		}
		foreach (CrawlerVertexInfo a in crawlerVerticsInfo) {
			if(a.VertexInfo == CrawlerVertexState.AutoModifiable){
				Vector3 tp0 = transform.TransformPoint(crawlerVertics[a.vertexIndex]);
				Vector3 tp1 = tp0 - wheel[a.wheelIndex].position;
				float ang = Vector3.Angle(tp1,-wheel[a.wheelIndex].up);
				tp1 = wheel[a.wheelIndex].position - wheel[a.wheelIndex].up * tp1.magnitude * Mathf.Cos(ang * Mathf.PI / 180.0f);
				Vector3 tp2 = tp0 - tp1;
				ang = Vector3.Angle(-wheel[a.wheelIndex].forward,tp2);
				tp2 = tp1 - wheel[a.wheelIndex].forward * tp2.magnitude * Mathf.Cos(ang * Mathf.PI / 180.0f);
				tp2 = tp2 - wheel[a.wheelIndex].position;
				ang = Vector3.Angle(tp2,-wheel[a.wheelIndex].up);
				if(ang > 15.0f){
					if(ang > 60.0f){
						a.VertexInfo = CrawlerVertexState.Fixed;
					}else{
						a.VertexInfo = CrawlerVertexState.Interpolate;
					}
				}
			}
		}
	}
}
