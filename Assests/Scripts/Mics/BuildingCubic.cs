using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 public class middletriangle
{
	public middletriangle(){}
	public Vector3[] vertice=new Vector3[3];//triangle's verticepos
	public Vector2[] triangleuvs=new Vector2[3];//triangle's uvs
	public Vector3 surfnormal;

	public int index;
	public bool splitted=false;
	public middletriangle(Vector3 a,Vector3 b,Vector3 c,Vector2 au,Vector2 bu,Vector2 cu )
	{
 		vertice[0]=a;
		vertice[1]=b;
		vertice[2]=c;
 		triangleuvs[0]=au;
		triangleuvs[1]=bu;
		triangleuvs[2]=cu; 
	}

}
public class BuildingCubic : MonoBehaviour {
	private Vector3[] verts;
	private Vector2[] uvs;
	private int[] tris;
	private bool[] vertsSel;
	public float width=0.1f;
	private List<int> removeids;
 	public List<middletriangle> changedMesh=new List<middletriangle>();
 	public List<middletriangle> tempMesh=new List<middletriangle>();
	
 	private List<middletriangle> bc=new List<middletriangle>();
	private Mesh myMesh;
	int roundcount=3;
	int a=2;
	float b=4f;
	float c=2.5f;
	// Use this for initialization
	void Start () {
		myMesh = GetComponent<MeshFilter>().mesh;
		verts = myMesh.vertices;
		tris = myMesh.triangles;
		uvs=myMesh.uv;
		int trilen=tris.Length/3;
		for(int i = 0; i < trilen; i ++){	
			middletriangle a=new middletriangle();
			for(int j=0;j<3;j++)
			{
				a.vertice[j]=verts[tris[i*3+j]];
				a.triangleuvs[j]=uvs[tris[i*3+j]];
			}
			a.surfnormal=getNormal(a.vertice);
			changedMesh.Add(a);
		}
 		
		foreach(middletriangle a in changedMesh)
		{	 
	 
			if(!bc.Contains(a)){
				foreach(middletriangle b in changedMesh)
				{
					if(a!=b){
						if(equalvertice(a.vertice,b.vertice)){
							bc.Add(b);
						}
					}
				}
			}
			
		}	
		foreach(middletriangle b in bc)
		{
			 changedMesh.Remove(b);
		} 
		foreach(middletriangle c in changedMesh)
			splitTriangles(c,0);
		changedMesh.Clear();
		foreach(middletriangle c in tempMesh){
			
			changedMesh.Add(c);
		}
		CalcMesh();
		//GetComponent<MeshFilter>().mesh.vertices = verts;
	}
	
	bool equalvertice(Vector3[] aarr, Vector3[] barr)
	{
		
		foreach(Vector3 a in aarr)
		{
			bool flag1=false;
			foreach(Vector3 b in barr)
			{
				if(a==b) flag1=true;
			}
			if(flag1==false) return false;
		}
		return true;
		
	}
	Vector3 getNormal(Vector3[] vertice)
	{
		Vector3 v1,v2,v3;		 
			v1 = vertice[1] - vertice[0];	
			v2 = vertice[2] - vertice[0];
			v3 = Vector3.Cross(v1,v2);
			v3.Normalize();
		 
		return v3;
	}
	void CalcMesh()
	{
	 	int[]changedtriangle;
		Vector3[] changednormals;
		int triarraylength=changedMesh.Count*3;
		changedtriangle=new int [triarraylength*10];
		int k=0;
		Vector3[]changedVertice=new Vector3[triarraylength*10];
		Vector2[]changedUv=new Vector2[triarraylength*10];
		//changednormals=new Vector3[triarraylength];
		for(int i=0;i<triarraylength;i++)
		{
			changedtriangle[i]=i;
		}
		foreach(middletriangle a in changedMesh)
		{
			for(int j=0;j<3;j++)
			{
				changedVertice[k*3+j]=a.vertice[j];
				changedUv[k*3+j]=a.triangleuvs[j];	
			//	changednormals[k*3+j]=a.normal;
			}
			k++;					
		}		
	 
		foreach(middletriangle a in changedMesh)
		{
			for(int j=0;j<3;j++)
			{
				changedVertice[k*3+j]=a.vertice[j]-a.surfnormal*0.005f;
				changedUv[k*3+j]=a.triangleuvs[j];	
			
			//	changednormals[k*3+j]=a.normal;
			}
			changedtriangle[k*3+0]=k*3;
			changedtriangle[k*3+1]=k*3+2;
			changedtriangle[k*3+2]=k*3+1;
			
			k++;					
		}
	 	
		foreach(middletriangle a in changedMesh)
		{
			 	changedVertice[k*3+0]=a.vertice[0];
				changedUv[k*3+0]=a.triangleuvs[0];	
				changedVertice[k*3+1]=a.vertice[0]-a.surfnormal*0.005f;
				changedUv[k*3+1]=a.triangleuvs[0];	
				changedVertice[k*3+2]=a.vertice[1];
				changedUv[k*3+2]=a.triangleuvs[1];
			 
			changedtriangle[k*3+0]=k*3;
			changedtriangle[k*3+1]=k*3+1;
			changedtriangle[k*3+2]=k*3+2;
			
			k++;
			changedVertice[k*3+0]=a.vertice[0]-a.surfnormal*0.005f;
			changedUv[k*3+0]=a.triangleuvs[0];	
			changedVertice[k*3+1]=a.vertice[1]-a.surfnormal*0.005f;
				changedUv[k*3+1]=a.triangleuvs[1];	
				changedVertice[k*3+2]=a.vertice[1];
				changedUv[k*3+2]=a.triangleuvs[1];
			 
			changedtriangle[k*3+0]=k*3;
			changedtriangle[k*3+1]=k*3+1;
			changedtriangle[k*3+2]=k*3+2;
			
			k++;	
			
						 	changedVertice[k*3+0]=a.vertice[2];
				changedUv[k*3+0]=a.triangleuvs[2];	
				changedVertice[k*3+1]=a.vertice[1] ;
				changedUv[k*3+1]=a.triangleuvs[1];	
				changedVertice[k*3+2]=a.vertice[2]-a.surfnormal*0.005f;
				changedUv[k*3+2]=a.triangleuvs[2];
			 
			changedtriangle[k*3+0]=k*3;
			changedtriangle[k*3+1]=k*3+1;
			changedtriangle[k*3+2]=k*3+2;
			
			k++;
			changedVertice[k*3+0]=a.vertice[1] ;
			changedUv[k*3+0]=a.triangleuvs[1];	
			changedVertice[k*3+1]=a.vertice[1]-a.surfnormal*0.005f;
				changedUv[k*3+1]=a.triangleuvs[1];	
				changedVertice[k*3+2]=a.vertice[2]-a.surfnormal*0.005f;
				changedUv[k*3+2]=a.triangleuvs[2];
			 
			changedtriangle[k*3+0]=k*3;
			changedtriangle[k*3+1]=k*3+1;
			changedtriangle[k*3+2]=k*3+2;
			
			k++;
			
								 	changedVertice[k*3+0]=a.vertice[2];
				changedUv[k*3+0]=a.triangleuvs[2];	
				changedVertice[k*3+1]=a.vertice[2]-a.surfnormal*0.005f ;
				changedUv[k*3+1]=a.triangleuvs[2];	
				changedVertice[k*3+2]=a.vertice[0] ;
				changedUv[k*3+2]=a.triangleuvs[0];
			 
			changedtriangle[k*3+0]=k*3;
			changedtriangle[k*3+1]=k*3+1;
			changedtriangle[k*3+2]=k*3+2;
			
			k++;
			changedVertice[k*3+0]=a.vertice[2]-a.surfnormal*0.005f ;
			changedUv[k*3+0]=a.triangleuvs[2];	
			changedVertice[k*3+1]=a.vertice[0]-a.surfnormal*0.005f;
				changedUv[k*3+1]=a.triangleuvs[0];	
				changedVertice[k*3+2]=a.vertice[0] ;
				changedUv[k*3+2]=a.triangleuvs[0];
			 
			changedtriangle[k*3+0]=k*3;
			changedtriangle[k*3+1]=k*3+1;
			changedtriangle[k*3+2]=k*3+2;
			
			k++;	
			 
		}
	 	
		myMesh.Clear();
		myMesh.vertices=changedVertice;
		myMesh.triangles=changedtriangle;
		myMesh.uv=changedUv;
		myMesh.RecalculateNormals();
		myMesh.RecalculateBounds();
		
	 
	}
	// Update is called once per frame
	void Update () {
	
	}
	void splitTriangles(middletriangle tri,int n)
	{

		 Vector3 posf,poss,post;	
		 Vector2 fu,su,tu;
		if(n>=roundcount){	
			//	tri.index=tempMesh.Count;
				tempMesh.Add(tri);	
				return;
			}
			else
			{
				posf=tri.vertice[2]+(tri.vertice[0]-tri.vertice[2])/a;
				poss=tri.vertice[1]+(tri.vertice[0]-tri.vertice[1])/b;
				post=tri.vertice[2]+(tri.vertice[1]-tri.vertice[2])/c;
				fu=tri.triangleuvs[2]+(tri.triangleuvs[0]-tri.triangleuvs[2])/a;	
				su=tri.triangleuvs[1]+(tri.triangleuvs[0]-tri.triangleuvs[1])/b;
				tu=tri.triangleuvs[2]+(tri.triangleuvs[1]-tri.triangleuvs[2])/c;		
				
				middletriangle newtri1=new middletriangle(tri.vertice[0],poss,posf,tri.triangleuvs[0],su,fu );
				newtri1.surfnormal=tri.surfnormal;
				splitTriangles(newtri1,n+1);
				middletriangle newtri2=new middletriangle(posf,poss,post,fu,su,tu );
				newtri2.surfnormal=tri.surfnormal;
				splitTriangles(newtri2,n+1);
				middletriangle newtri3=new middletriangle(poss,tri.vertice[1],post,su,tri.triangleuvs[1],tu );
				newtri3.surfnormal=tri.surfnormal;
				splitTriangles(newtri3,n+1);
				middletriangle newtri4=new middletriangle(posf,post,tri.vertice[2],fu,tu,tri.triangleuvs[2] );
				newtri4.surfnormal=tri.surfnormal;
				splitTriangles(newtri4,n+1);
			}
	}
	
}
