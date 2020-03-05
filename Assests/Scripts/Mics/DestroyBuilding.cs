using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MagicBattle;
public class DestroyBuilding : MonoBehaviour {
	private Mesh myMesh;
	public GameObject explosion;
	private Vector3[] vertice;
	private Vector2[] uvs; 
	private int[]triangles;
	private Vector3[] normals;
	public float defensivePower = 90.0f;
	public GameObject colpart;
	public float thickness=1f;
	//Only one faces
	private Vector3[] Basicvertice;
	private Vector2[] Basicuvs;
	private int[]Basictriangles;
	private int Basictricn;
	private Vector3[] Basicnormals;
	//for side
	private int[]sidetriangles;
	private int sidetricn;
	private Vector3[]sidenormals;
	private float destructedAmount = 0.0f;
	//for Operate Mesh
	private Vector3[] operatingvertice;
	private Vector2[] operatinguvs;
	private int[]operatingtriangles;
	private int operatingtricn;
	private int operatingvertcn;
	private Vector3[] operatingnormals;
	private int unsplitcount;
	//for thickivity
	private Vector3[] thickvertice;
	private Vector2[] thickuvs;
	private int[]thicktriangles;
	private int thicktricn;
	private Vector3[]thicknormals;
	private MeshCollider myMeshCollider;
	//for split
	public int roundcount=2;
	private float a=2f; 
	private float c=2.5f;
	public float unsplitwidth=0.5f;
	private	List<int> splitlist=new List<int>();
	private int cnpertri;
	//for destroy
	public int piececount=6;
	public int shotcount=3;
	public float destroyradius=5f;
	public float shotradius=5f;
	public float collisionscale=5f;
	private bool startflag=true;
	private bool splitted = false;
	
	// Use this for initialization
	void Start () {
		//Get MeshInfo 
		StartCoroutine("InitOperation");
	}
	
	IEnumerator InitOperation() {
		myMeshCollider = gameObject.GetComponent<MeshCollider>();
		GetMesh();  
		
  		//Detect only one surface
		Detectonlyonesurface();  
		yield return new WaitForSeconds(0.2f);
 		//Split the Triangles
 		SplitMesh();
		yield return new WaitForSeconds(0.2f);
  		//Show Thickvity
 	 	gainThickivity();
		yield return new WaitForSeconds(0.2f);
// 		//add Sidess
	  	addSides(); 
		yield return new WaitForSeconds(0.2f);
		SetMesh();
		yield return new WaitForSeconds(0.2f);
		myMeshCollider.sharedMesh = myMesh; 
		yield return new WaitForSeconds(0.2f);
		splitted = true;
	}
	
	void GetMesh()
	{
		myMesh= GetComponent<MeshFilter>().mesh;
		vertice=myMesh.vertices;
		uvs=myMesh.uv ;
	
		normals=myMesh.normals;
		triangles=myMesh.triangles;
	}
	void SetMesh()
	{
		myMesh.vertices=vertice; 
		myMesh.uv =uvs; 
		myMesh.triangles=triangles; 
		myMesh.normals=normals;   
	}
	void Detectonlyonesurface()
	{
		int i=0,j=0;int k;
		int looplen=triangles.Length/3;
		List<int>unnecessaryindice=new List<int>();
		Vector3 []ivertice=new Vector3[3];
		Vector3[] jvertice=new Vector3[3];
		for(i=0;i<looplen;i++){
			if(!unnecessaryindice.Contains(i)){
				k=i*3;
				ivertice[0]=vertice[triangles[k]];
				ivertice[1]=vertice[triangles[k+1]];
				ivertice[2]=vertice[triangles[k+2]];
				for( j=0;j<looplen;j++)
				{
					if(i!=j){
						k=j*3;
						jvertice[0]=vertice[triangles[k]];
						jvertice[1]=vertice[triangles[k+1]];
						jvertice[2]=vertice[triangles[k+2]];	
						if(equalvertice(ivertice,jvertice))
							unnecessaryindice.Add(j);
					}	 
							
				 }
			}
			 
		}
		int[]temptriangles=new int[looplen*3-unnecessaryindice.Count*3];
		int temptricn=0; int p;
		for(i=0;i<looplen;i++){
			 if(!unnecessaryindice.Contains(i))
			 {
				p=i*3;
				temptriangles[temptricn]=triangles[p];
				temptriangles[temptricn+1]=triangles[p+1];
				temptriangles[temptricn+2]=triangles[p+2];
				temptricn+=3;
			 }
			
		}
		Basictricn=temptricn;
		Basictriangles=temptriangles;
		Basicuvs=uvs;
		Basicvertice=vertice;	
		triangles=temptriangles; 
		Basicnormals=normals;
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
	void gainThickivity()
	{
		operatingtricn=operatingtriangles.Length;
		thicktriangles=new int[operatingtricn*2-unsplitcount*3];
		thickuvs=new Vector2[operatinguvs.Length+operatingtricn-unsplitcount*3];
		thickvertice=new Vector3[operatingvertice.Length+operatingtricn-unsplitcount*3];
		thicknormals=new Vector3[operatingnormals.Length+operatingtricn-unsplitcount*3];
		int i=0,j=0;
		for(i=0;i<operatinguvs.Length;i++)
		{
			thickuvs[i]=uvs[i];
			thickvertice[i]=vertice[i];
			
		}
		for(j=0;j<operatingtricn;j++){
			thicktriangles[j]=operatingtriangles[j];
		}
		for(int k=0;k<operatingnormals.Length;k++)
		{
			thicknormals[k]=operatingnormals[k];
			
		}
		int tmpnormalcn=operatingnormals.Length;
		for(int k=Basicnormals.Length;k<operatingnormals.Length;k++)
		{
			thicknormals[tmpnormalcn]=-operatingnormals[k];
			tmpnormalcn++;
		}
		 
		//current i=uv,vertice length and j=triangle length

	 	Vector3 temptrinormal;
		int p,pk;
		for(int k=0;k<operatingtricn/3-unsplitcount;k++)
		{
			p=(unsplitcount+k)*3;
			pk=k*3;
			thickuvs[i+pk]=uvs[operatingtriangles[p]];
			thickuvs[i+pk+1]=uvs[operatingtriangles[p+1]];
			thickuvs[i+pk+2]=uvs[operatingtriangles[p+2]];
			thicktriangles[j+pk]=i+pk;
			thicktriangles[j+pk+1]=i+pk+2;
			thicktriangles[j+pk+2]=i+pk+1;
			temptrinormal= getNormal(operatingvertice[operatingtriangles[p]],operatingvertice[operatingtriangles[p+1]],operatingvertice[operatingtriangles[p+2]]);
			thickvertice[i+pk]=vertice[triangles[p]]-temptrinormal*thickness/transform.localScale.x;
			thickvertice[i+pk+1]=vertice[triangles[p+1]]-temptrinormal*thickness/transform.localScale.x;
			thickvertice[i+pk+2]=vertice[triangles[p+2]]-temptrinormal*thickness/transform.localScale.x; 
		}
		vertice=thickvertice;
		uvs=thickuvs;
		triangles=thicktriangles;

		thicktricn=thicktriangles.Length/6; 
		normals=thicknormals;
	}
	Vector3 getNormal(Vector3 tv0,Vector3 tv1,Vector3 tv2)
	{
		Vector3 v1,v2,v3;		 
			v1 =tv1 -tv0;	
			v2 = tv2 - tv0;
			v3 = Vector3.Cross(v1,v2);
			v3.Normalize();
		Bounds bound=new Bounds(v1,v2);
		
		 
		return v3;
	}
	bool checkvertex(int tripos)
	{
		int cn=0; int k=0;
		Vector3 []vertice=new Vector3[3];
		for(int i=0;i<3;i++)
			vertice[i]= thickvertice[thicktriangles[tripos+i]];
		for(int j=0;j<3;j++){
			for(int i=0;i<Basictricn;i++)
			{
				k=i*3;
				Vector3 c1=Vector3.Cross(vertice[j]-Basicvertice[Basictriangles[k]],vertice[j]-Basicvertice[Basictriangles[k+1]]);
				Vector3 c2=Vector3.Cross(vertice[j]-Basicvertice[Basictriangles[k]],vertice[j]-Basicvertice[Basictriangles[k+2]]);
			    Vector3 c3=Vector3.Cross(vertice[j]-Basicvertice[Basictriangles[k+1] ],vertice[j]-Basicvertice[Basictriangles[k+2]]);
				c1.Normalize();
				c2.Normalize();
				c3.Normalize();
				if(c1==Vector3.zero){
					
					cn++;
					break;
				}
				else if(c2==Vector3.zero)
				{
					cn++;
					break;
				}
				else if(c3==Vector3.zero)
				{
					cn++;
					break;
				}
 
			}
		}
		if(cn>=2) return true;
		else return false;
	}
	
	void addSides()
	{
		int halftricn=thicktriangles.Length/6;
		sidetriangles=new int[halftricn*18];
		int tmpsidtricn=0;
		int diff=operatingtriangles.Length-unsplitcount*3;
		for(int ind=unsplitcount;ind<halftricn;ind++)
		{
			int tripos=ind*3;			
			if(checkvertex(tripos)){
				tmpsidtricn=(ind-unsplitcount)*18;
				//vo,vo',v1
	 			sidetriangles[tmpsidtricn]=thicktriangles[tripos];
				sidetriangles[tmpsidtricn+1]=thicktriangles[tripos+diff];
				sidetriangles[tmpsidtricn+2]=thicktriangles[tripos+1];
			 
					//v0',v1',v1
	  			sidetriangles[tmpsidtricn+3]=thicktriangles[tripos+diff];
				sidetriangles[tmpsidtricn+4]=thicktriangles[tripos+diff+2];
				sidetriangles[tmpsidtricn+5]=thicktriangles[tripos+1];
			 
					//v2,v1,v2'
	 			sidetriangles[tmpsidtricn+6]=thicktriangles[tripos+2];
				sidetriangles[tmpsidtricn+7]=thicktriangles[tripos+1];
				sidetriangles[tmpsidtricn+8]=thicktriangles[tripos+diff+1];
			 	 
					
					//v1,v1',v2'
	 			sidetriangles[tmpsidtricn+9]=thicktriangles[tripos+1];
				sidetriangles[tmpsidtricn+10]=thicktriangles[tripos+diff+2];
				sidetriangles[tmpsidtricn+11]=thicktriangles[tripos+diff+1];
			  
					
					//v2,v2',v0
	  			sidetriangles[tmpsidtricn+12]=thicktriangles[tripos+2];
				sidetriangles[tmpsidtricn+13]=thicktriangles[tripos+diff+1];
				sidetriangles[tmpsidtricn+14]=thicktriangles[tripos ];
	 
					//v2'v0'v0
	  			sidetriangles[tmpsidtricn+15]=thicktriangles[tripos+diff+1];
				sidetriangles[tmpsidtricn+16]=thicktriangles[tripos+diff];
				sidetriangles[tmpsidtricn+17]=thicktriangles[tripos ];
				 

			} 
		}
		int[]temptri=new int[thicktriangles.Length+sidetriangles.Length];
		int tempi=0;
		foreach(int a in thicktriangles)
		{
			temptri[tempi]=a;
			tempi++;
		}
		foreach(int a in sidetriangles)
		{
			temptri[tempi]=a;
			tempi++;
		}
		triangles=temptri;
	//	triangles=sidetriangles;
			 
	}

 	void OnShellAttacked(ShellAttackedSendMsgParam param)
	{
		if(!splitted) return;
		StartCoroutine("AttackedBehaviour",param);
	}

	IEnumerator AttackedBehaviour(ShellAttackedSendMsgParam param) {
		yield return new WaitForSeconds(0.0f);
		destructedAmount += GlobalInfo.shellProperty[(int)param.attackedShellKind].destructionPower;
		GameObject.Instantiate (explosion, param.attackedPoint, Quaternion.identity);
		if(destructedAmount >= defensivePower){
			destructedAmount = 0.0f;
			Vector3 hitpoint = transform.InverseTransformPoint(param.attackedPoint);
			HouseActivityParam p=new HouseActivityParam();
			p.hitpoint = hitpoint;
			p.radius = shotradius;
			StartCoroutine("houseactivity",p); 
		}		
	}
	
	bool splittriangleok(Vector3 tv0, Vector3 tv1,Vector3 tv2)
	{
		float s= Vector3.Cross(tv0-tv1,tv0-tv2).magnitude;
		float h1,h2,h3;
		h1=s/(tv0-tv1).magnitude;
		h2=s/(tv0-tv2).magnitude;
		h3=s/(tv2-tv1).magnitude;
		if(h1<unsplitwidth/transform.localScale.x) 
			return true;
		else if(h2<unsplitwidth/transform.localScale.x)
 			return true;
		else if(h3< unsplitwidth/transform.localScale.x)
			 return true;
		return false;
	}
	
	void SplitMesh()
	{

		List<int> unsplitlist=new List<int>();
		int i=0;
		int p;
		Basictricn=Basictriangles.Length/3;
		for(i=0;i<Basictricn;i++)
		{
			p=i*3;
			if( splittriangleok(Basicvertice[Basictriangles[p]],Basicvertice[Basictriangles[p+1]],Basicvertice[Basictriangles[p+2]]))
			{
				unsplitlist.Add(i);
	 		}
			else
			{
				splitlist.Add(i);
			}
		} 
		
 		  cnpertri=(int)Mathf.Pow(4f,roundcount);
 		int cnvertpertri=0;
 		for(int pp=0;pp<roundcount;pp++)
 			cnvertpertri+=3*(int)Mathf.Pow(4f,pp);
 		int totalvertcn=cnvertpertri*(splitlist.Count)+Basicvertice.Length;	
		int totatrilcn=cnpertri*splitlist.Count*3+unsplitlist.Count*3;
		operatinguvs=new Vector2[totalvertcn];
		operatingvertice=new Vector3[totalvertcn];
		operatingnormals=new Vector3[totalvertcn];
 		operatingtriangles=new int[totatrilcn]; 	

  		operatingtricn=0;
		for(int j=0;j<Basicvertice.Length;j++)
		{
			operatinguvs[j]=Basicuvs[j];
			operatingvertice[j]=Basicvertice[j];
			operatingnormals[j]=Basicnormals[j];
		} 
		foreach(int ind in unsplitlist)
		{	
			p=ind*3;
			operatingtriangles[operatingtricn]= Basictriangles[p];
			operatingtriangles[operatingtricn+1]= Basictriangles[p+1];
			operatingtriangles[operatingtricn+2]= Basictriangles[p+2];
			operatingtricn+=3;
			
		}
  
 		int basicvertcn=Basicvertice.Length;		
		operatingvertcn=Basicvertice.Length;
 		int tmpcn=0;  
 		foreach(int ind in splitlist)
		{
				p=ind*3;
		 		splitTriangles( Basictriangles[p],Basictriangles[p+1],Basictriangles[p+2],roundcount);
				for(int j=0;j<cnvertpertri;j++)
				{
					operatingnormals[basicvertcn+tmpcn*cnvertpertri+j]=getNormal(Basicvertice[Basictriangles[p]],Basicvertice[Basictriangles[p+1]],Basicvertice[Basictriangles[p+2]]);
				}
				tmpcn++;
 			 
		}
  		vertice=operatingvertice;
 		uvs=operatinguvs;
 		triangles=operatingtriangles;  
		normals=operatingnormals;
		unsplitcount=unsplitlist.Count;
		
	}
	void splitTriangles(int ind0,int ind1,int ind2,int n )
	{
		
		if(n<=0)
		{
			operatingtriangles[operatingtricn]=ind0;
			operatingtriangles[operatingtricn+1]=ind1;
			operatingtriangles[operatingtricn+2]=ind2;
			operatingtricn+=3;
		}
		else{
			
			Vector3 posf,poss,post;	
			Vector2 fu,su,tu;
			int ind;
			float ra=Random.Range(a,c);
			float rb=Random.Range(a,c);
			float rc=Random.Range(a,c);
			posf=operatingvertice[ind2]+(operatingvertice[ind0]-operatingvertice[ind2])/ra;
			poss=operatingvertice[ind1]+(operatingvertice[ind0]-operatingvertice[ind1])/rb;
			post=operatingvertice[ind2]+(operatingvertice[ind1]-operatingvertice[ind2])/rc;
			fu=operatinguvs[ind2]+(operatinguvs[ind0]-operatinguvs[ind2])/ra;	
			su=operatinguvs[ind1]+(operatinguvs[ind0]-operatinguvs[ind1])/rb;
			tu=operatinguvs[ind2]+(operatinguvs[ind1]-operatinguvs[ind2])/rc;	
			ind=operatingvertcn;
			operatingvertice[operatingvertcn]=posf;
			operatingvertice[operatingvertcn+1]=poss;
			operatingvertice[operatingvertcn+2]=post;
			operatinguvs[operatingvertcn]=fu;
			operatinguvs[operatingvertcn+1]=su;
			operatinguvs[operatingvertcn+2]=tu;			
			 operatingvertcn+=3;
			splitTriangles(ind0,ind+1,ind,n-1);			
			splitTriangles(ind,ind+1,ind+2,n-1);
			splitTriangles(ind+1,ind1,ind+2,n-1);
			splitTriangles(ind,ind+2,ind2,n-1);
		}	
	}
	
	// Update is called once per frame
 
	IEnumerator houseactivity(HouseActivityParam param)
	{
		
		
		ReBuildHouse(param);	
		GetComponent<MeshCollider> ().enabled = true;
		yield return new WaitForSeconds(0.01f);
		shotcount--;					
		if(shotcount<=0){
			StartCoroutine("DestroyHouse",param.hitpoint);
		}else{
		     if (myMeshCollider != null)
			 { 
			 	myMeshCollider.sharedMesh = myMesh;	
				yield return new WaitForSeconds(0.01f);
			 }
			 myMeshCollider.convex=false; 
			yield return new WaitForSeconds(0.01f);
		}
	}
	
	void OnHit(HouseActivityParam param) {
		if(!splitted) return;
		GetComponent<MeshCollider> ().enabled = false;
		networkView.RPC("OnHitRPC",RPCMode.Others,param.globalHitPoint,param.origin);
		Vector3 hitPoint=transform.InverseTransformPoint(param.globalHitPoint);
		HouseActivityParam p=new HouseActivityParam();
		p.hitpoint = hitPoint;
		p.globalHitPoint = param.globalHitPoint;
		p.radius = collisionscale;
		p.colflag = true;
		p.destroy = false;
		p.origin = param.origin;
		StartCoroutine("houseactivity",p); 		
	}
	
	[RPC]
	void OnHitRPC(Vector3 globalHitPoint,Vector3 origin) {
		if(!splitted) return;
		Vector3 hitPoint = transform.InverseTransformPoint(globalHitPoint);
		HouseActivityParam p=new HouseActivityParam();
		p.hitpoint = hitPoint;
		p.globalHitPoint = globalHitPoint;
		p.radius = collisionscale;
		p.colflag = true;
		p.destroy = false;
		p.origin = origin;	
		StartCoroutine("houseactivity",p);
	}
	
	void ReBuildHouse(HouseActivityParam param)
	{ 
		lock(myMesh){
			Vector3 hitpoint=param.hitpoint;
			float radius=param.radius/transform.localScale.x;
			bool destroy=param.destroy;
			bool colflag=param.colflag;
			List<int> indice=new List<int>();
			List<int> splitindice ;
			int p;
			indice.Clear(); 
			if(startflag){
				for(int i=0;i<unsplitcount;i++)
				{
					p=i*3;
					thicktriangles[p]=0;thicktriangles[p+1]=0;thicktriangles[p+2]=0;		
						 
				}
				startflag=false;
			}
			int diff=operatingtriangles.Length-unsplitcount*3;
			DetectTriangle(hitpoint,radius,out splitindice); 
			foreach(int ind in splitindice)
			{
				int tmpi=splitlist.IndexOf(ind);
				for(int i=tmpi*cnpertri+unsplitcount;i<tmpi*cnpertri+cnpertri+unsplitcount;i++)
				{
					for(int j=0;j<3;j++){ 
						p=i*3;
						if(Mathf.Abs(thickvertice[thicktriangles[p+j]].x-hitpoint.x) <radius){		
							 if(Mathf.Abs(thickvertice[thicktriangles[p+j]].y-hitpoint.y) <radius)
								 if(Mathf.Abs(thickvertice[thicktriangles[p+j]].z-hitpoint.z) <radius)
								{
									
									indice.Add(i);
									indice.Add(i+diff/3); 
									break;
								}				
						}			 
					}	
				}
			}
			if(indice.Count!=0){
				if(colflag) 
				{
					getoutpieceatcollision (indice,param);	 
				}
				else if(destroy) 
					getoutpiece(indice); 
				removetriangle(indice); 
			
				SetMesh();
			}
		}
 
	}
	void getoutpiece(List<int> indice )
	{
		lock(myMesh){
			List<int>refinelists=new List<int>();
		 	List<Vector3> normaltype=new List<Vector3>();
			Vector3 indnormal;
			int n=indice.Count;
			int ind;
		 	for(int i=0;i<n;i+=2)
			{
				ind=indice[i] ;
				indnormal=thicknormals[thicktriangles[ind*3 ]];
				if(!normaltype.Contains(indnormal))
				{
					 normaltype.Add (indnormal);
				}
			} 
	 		int normalcn=normaltype.Count;
			for(int spcn=0;spcn<normalcn;spcn++){
				Vector3[] removedVertice =new Vector3[n*3+n/2*18];
				Vector2 []removedUvs =new Vector2[n*3+n/2*18];
				int []removedtriangles =new int[n*3 +n/2*18];
				Vector3 []removednormals=new Vector3[n*3+n/2*18];
				int tmpcn=0;
			 
				for(int i=0;i<n ;i+=2)
				{ 
					ind=indice[i]*3; 
					indnormal=thicknormals[thicktriangles[ind]];
					if(indnormal==normaltype[spcn]){										 
						for(int j=0;j<3;j++){
							removedVertice[tmpcn+j]=thickvertice[thicktriangles[ind+j]];
							removedUvs[tmpcn+j]=thickuvs[thicktriangles[ind+j]];
							removedtriangles[tmpcn+j]=tmpcn+j;
							removednormals[tmpcn+j]=thicknormals[thicktriangles[ind+j]];
						}
						tmpcn+=3;				 
						ind=indice[i+1]*3;
						for(int j=0;j<3;j++){
							removedVertice[tmpcn+j]=thickvertice[thicktriangles[ind+j]];
							removedUvs[tmpcn+j]=thickuvs[thicktriangles[ind+j]];
							removedtriangles[tmpcn+j]=tmpcn+j;
							removednormals[tmpcn+j]=thicknormals[thicktriangles[ind+j]];
						}
						tmpcn+=3;
						int pos=tmpcn-6;
						  	 		//0,3,4
						removedtriangles[tmpcn ]=pos;		 
						removedtriangles[tmpcn+1]=pos+3;
						removedtriangles[tmpcn+2]=pos+1;
						tmpcn+=3;
									//0,4,1
						removedtriangles[tmpcn ]=pos+3;		 
						removedtriangles[tmpcn+1]=pos+5;
						removedtriangles[tmpcn+2]=pos+1;
						tmpcn+=3;			
									//2,4,5
						removedtriangles[tmpcn ]=pos+2;		 
						removedtriangles[tmpcn+1]=pos+1;
						removedtriangles[tmpcn+2]=pos+4;
						tmpcn+=3;		
									//2,1,4
						removedtriangles[tmpcn ]=pos+1;		 
						removedtriangles[tmpcn+1]=pos+5;
						removedtriangles[tmpcn+2]=pos+4;
						tmpcn+=3;
									//2,5,3
						removedtriangles[tmpcn ]=pos+2;		 
						removedtriangles[tmpcn+1]=pos+4;
						removedtriangles[tmpcn+2]=pos ;
						tmpcn+=3;
									//0,2,3	
						removedtriangles[tmpcn ]=pos+4;		 
						removedtriangles[tmpcn+1]=pos+3;
						removedtriangles[tmpcn+2]=pos ;
						tmpcn+=3;
					}
				}
			
		 
		 		Mesh newMesh=new Mesh();
				newMesh.vertices=removedVertice;
				newMesh.uv=removedUvs;
				newMesh.triangles=removedtriangles; 
				newMesh.normals=removednormals;
				newMesh.RecalculateBounds();  
				GameObject newGameObject= new GameObject("Debri");
				newGameObject.transform.rotation=transform.rotation;
				newGameObject.transform.position=transform.position;
				newGameObject.transform.localScale=gameObject.transform.localScale;	
				newGameObject.AddComponent<Rigidbody>();
				newGameObject.AddComponent<MeshFilter>();
				newGameObject.AddComponent<MeshRenderer>();
				newGameObject.AddComponent<MeshCollider>();
	 			MeshFilter newMeshFilter = newGameObject.GetComponent<MeshFilter>();
				
				if (newMeshFilter != null)
				{
					newMeshFilter.mesh = newMesh;
				}
				MeshCollider newMeshcolider=newGameObject.GetComponent<MeshCollider>();
				newMeshcolider.sharedMesh=newMesh;
				newMeshcolider.convex=false;
				MeshRenderer newMeshRenderer=newGameObject.GetComponent<MeshRenderer>() ;
				newMeshRenderer.material = renderer.material;
				newGameObject.rigidbody.mass=tmpcn/10f+1f;
				Object.Destroy(newGameObject.GetComponent<MeshCollider>(),6.0f);
				Object.Destroy(newGameObject.GetComponent<Rigidbody>(),6.0f);				
			//	newGameObject.AddComponent<DebriActivity>();
	 			Destroy(newGameObject,10f); 
				 
			}
		}
	}
	
	void getoutpieceatcollision(List<int> indice,HouseActivityParam param)
	{ 
		lock(myMesh){
				int n=indice.Count;
				int ind;
			for(int i=0;i<n;i+=2){
				Vector3[] removedVertice =new Vector3[6];
				Vector2 []removedUvs =new Vector2[6];
				int []removedtriangles =new int[24];
				Vector3 []removednormals=new Vector3[6];
		 	 	int tmpcn=0;
				 ind=indice[i]*3;
				 for(int j=0;j<3;j++){
					removedVertice[tmpcn+j]=thickvertice[thicktriangles[ind+j]];
					removedUvs[tmpcn+j]=thickuvs[thicktriangles[ind+j]];
					removedtriangles[tmpcn+j]=tmpcn+j;
					removednormals[tmpcn+j]=thicknormals[thicktriangles[ind+j]];
				}
				tmpcn+=3; 
				Vector3 particlepos=transform.TransformPoint((removedVertice[0]+removedVertice[1]+removedVertice[2])/3f); 
				ind=indice[i+1]*3;
				for(int j=0;j<3;j++){
					removedVertice[tmpcn+j]=thickvertice[thicktriangles[ind+j]];
					removedUvs[tmpcn+j]=thickuvs[thicktriangles[ind+j]];
					removedtriangles[tmpcn+j]=tmpcn+j;
					removednormals[tmpcn+j]=thicknormals[thicktriangles[ind+j]];
				}
				tmpcn+=3;
				int pos=tmpcn-6;
				 //0,3,4
				removedtriangles[tmpcn ]=pos;		 
				removedtriangles[tmpcn+1]=pos+3;
				removedtriangles[tmpcn+2]=pos+1;
				tmpcn+=3;
				//0,4,1
				removedtriangles[tmpcn ]=pos+3;		 
				removedtriangles[tmpcn+1]=pos+5;
				removedtriangles[tmpcn+2]=pos+1;
				tmpcn+=3;			
				//2,4,5
				removedtriangles[tmpcn ]=pos+2;		 
				removedtriangles[tmpcn+1]=pos+1;
				removedtriangles[tmpcn+2]=pos+4;
				tmpcn+=3;		
				//2,1,4
				removedtriangles[tmpcn ]=pos+1;		 
				removedtriangles[tmpcn+1]=pos+5;
				removedtriangles[tmpcn+2]=pos+4;
				tmpcn+=3;
				//2,5,3
				removedtriangles[tmpcn ]=pos+2;		 
				removedtriangles[tmpcn+1]=pos+4;
				removedtriangles[tmpcn+2]=pos ;
				tmpcn+=3;
				//0,2,3	
				removedtriangles[tmpcn ]=pos+4;		 
				removedtriangles[tmpcn+1]=pos+3;
				removedtriangles[tmpcn+2]=pos ;
				tmpcn+=3;
				Mesh newMesh=new Mesh();
				newMesh.vertices=removedVertice;
				newMesh.uv=removedUvs;
				newMesh.triangles=removedtriangles; 
				newMesh.normals=removednormals;
				newMesh.RecalculateBounds();  
				GameObject newGameObject= new GameObject("DebriPiece");
				newGameObject.transform.rotation=transform.rotation;
				newGameObject.transform.position=transform.position;
				newGameObject.transform.localScale=gameObject.transform.localScale;	
				newGameObject.AddComponent<Rigidbody>();
				newGameObject.AddComponent<MeshFilter>();
				newGameObject.AddComponent<MeshRenderer>();
				//newGameObject.AddComponent<MeshCollider>();		
	 			MeshFilter newMeshFilter = newGameObject.GetComponent<MeshFilter>();
				
				if (newMeshFilter != null)
				{
					newMeshFilter.mesh = newMesh;
				}
//				MeshCollider newMeshcolider=newGameObject.GetComponent<MeshCollider>();
//				newMeshcolider.sharedMesh=newMesh;
//				newMeshcolider.convex=false;
				MeshRenderer newMeshRenderer=newGameObject.GetComponent<MeshRenderer>() ;
				newMeshRenderer.material = renderer.material; 
				newGameObject.rigidbody.mass=tmpcn / 300f + 0.001f; 
				Vector3 tmp = param.globalHitPoint - param.origin;
				tmp.Normalize();
				newGameObject.rigidbody.AddForce(tmp * tmpcn * Random.Range(0.01f,0.05f),ForceMode.Impulse);
				GameObject go = (GameObject)Instantiate(colpart,particlepos,Quaternion.identity);
				go.transform.parent = newGameObject.transform;
				Destroy(go,3.0f);
//				Object.Destroy(newGameObject.GetComponent<MeshCollider>(),3.0f);
				Object.Destroy(newGameObject.GetComponent<Rigidbody>(),3.0f);
				Destroy(newGameObject,10.0f); 
			}
		}
	}
	
	IEnumerator DestroyHouse(Vector3 hitpoint)
	{
		Object.Destroy(GetComponent<MeshCollider>());
		List<Vector3>upvertice=new List<Vector3>();
		int ind;
		HouseActivityParam p=new HouseActivityParam();
		
		for(int i=0;i<thicktriangles.Length/6;i+=3)
		{
			  ind=i*3;
			if(thickvertice[thicktriangles[ind]].y>hitpoint.y)
			{
				if(thickvertice[thicktriangles[ind+1]].y>hitpoint.y)
					if(thickvertice[thicktriangles[ind+2]].y>hitpoint.y)
						 upvertice.Add(thickvertice[thicktriangles[ind ]]);
			}
		} 
		int n=upvertice.Count;
		float tmpspeed=transform.localScale.y/n;
		if(upvertice.Count!=0){
			for(int i=0;i<n;i+=12){
			 	
				p.hitpoint=upvertice[i];
				p.radius=destroyradius;
				p.colflag=false;
				p.destroy=true;
	//			StartCoroutine("ReBuildHouse",p); 
 				ReBuildHouse(p);
				if(i%4==0)
					yield return new WaitForSeconds(0f);
			}
	
		}
		
		Vector3 piecevert;
		for(int i=0;i<piececount;i++)
		{	
			int index=(Random.Range(0,thicktriangles.Length/6-1))*3;
			piecevert=thickvertice[thicktriangles[index ]];
			if((!upvertice.Contains(piecevert))&&(thicktriangles[index ]!=0))
			{
				p.hitpoint=piecevert;
				p.radius=destroyradius;
				p.colflag=false;
				p.destroy=true;
				ReBuildHouse(p);
				yield return new WaitForSeconds(0.01f);
			} 	 
		}
		Object.Destroy(GetComponent<MeshRenderer>());
 		Destroy(gameObject,10.0f);
	}
	void removetriangle(List<int> indice  )
	{
		lock(myMesh){
			int n=indice.Count;
			if(n==0) return;
			int[]remainthicktriangles=new int[thicktriangles.Length-n*3];
			int[]remainsidetriangles=new int[sidetriangles.Length];
		 	int operatingtrilen=thicktricn*3; 
			int p;
			foreach(int a in indice){
				  p=a*3;
				thicktriangles[p]=0;thicktriangles[p+1]=0;thicktriangles[p+2]=0;
				 
			} 
			//add side triangles
			int af ;
			int tmpsidtricn=0;
			int diff=operatingtriangles.Length-unsplitcount*3;
			int ind;
	 		for(int j=0;j<n;j++)
			{
				
				ind=indice[j];
					j++;
					for(int i=0;i<18;i++)
						sidetriangles[(ind-unsplitcount)*18+i]=0; 
					af=(ind-unsplitcount+4)/4*4+unsplitcount;
					for(int i=af-1;i>=af-4;i--)
					{
						if(!indice.Contains(i)){
							int tripos=i*3;
							sidetriangles[tmpsidtricn]=thicktriangles[tripos];
							sidetriangles[tmpsidtricn+1]=thicktriangles[tripos+diff];
							sidetriangles[tmpsidtricn+2]=thicktriangles[tripos+1];
							tmpsidtricn+=3;
								//v0',v1',v1
				  			sidetriangles[tmpsidtricn]=thicktriangles[tripos+diff];
							sidetriangles[tmpsidtricn+1]=thicktriangles[tripos+diff+2];
							sidetriangles[tmpsidtricn+2]=thicktriangles[tripos+1];
							tmpsidtricn+=3;
								//v2,v1,v2'
				 			sidetriangles[tmpsidtricn]=thicktriangles[tripos+2];
							sidetriangles[tmpsidtricn+1]=thicktriangles[tripos+1];
							sidetriangles[tmpsidtricn+2]=thicktriangles[tripos+diff+1];
							tmpsidtricn+=3;	 
								
								//v1,v1',v2'
				 			sidetriangles[tmpsidtricn]=thicktriangles[tripos+1];
							sidetriangles[tmpsidtricn+1]=thicktriangles[tripos+diff+2];
							sidetriangles[tmpsidtricn+2]=thicktriangles[tripos+diff+1];
							tmpsidtricn+=3; 
								
								//v2,v2',v0
				  			sidetriangles[tmpsidtricn]=thicktriangles[tripos+2];
							sidetriangles[tmpsidtricn+1]=thicktriangles[tripos+diff+1];
							sidetriangles[tmpsidtricn+2]=thicktriangles[tripos ];
							tmpsidtricn+=3;
								//v2'v0'v0
				  			sidetriangles[tmpsidtricn]=thicktriangles[tripos+diff+1];
							sidetriangles[tmpsidtricn+1]=thicktriangles[tripos+diff];
							sidetriangles[tmpsidtricn+2]=thicktriangles[tripos ];
							tmpsidtricn+=3;
						}
				}
			}
	 
			int[]temptri=new int[thicktriangles.Length+sidetriangles.Length];
			int tempi=0;
			foreach(int a in thicktriangles)
			{
				temptri[tempi]=a;
				tempi++;
			}
			foreach(int a in sidetriangles)
			{
				temptri[tempi]=a;
				tempi++;
			}
			triangles=temptri;
		}
	}	
	
	void DetectTriangle(Vector3 point, float radius,out List<int>indice)
	{
		indice=new List<int>();
		Vector3 v1,v2,v3;
		int k;
		foreach(int ind in splitlist)
		{
			k=ind*3;
			v1=Basicvertice[Basictriangles[k]];
			v2=Basicvertice[Basictriangles[k+1]];
			v3=Basicvertice[Basictriangles[k+2]];
			if(pointIntriangle(v1,v2,v3,point,radius))
				indice.Add(ind);
		}
	}
	
	bool pointIntriangle(Vector3 v1,Vector3 v2,Vector3 v3,Vector3 point,float radius)
	{
		if((v1-v2==Vector3.zero)||(v1-v3==Vector3.zero)||(v3-v2==Vector3.zero)) return false;
 
		Vector3 cv1=Vector3.Cross(v1-point,v2-point);
		Vector3 cv2=Vector3.Cross(v2-point,v3-point);
		Vector3 cv3=Vector3.Cross(v3-point,v1-point);
		float s1=cv1.magnitude;
		float s2=cv2.magnitude;
		float s3=cv3.magnitude;
 
		float ep=0.001f;
		if(Mathf.Abs(s1+s2+s3-Vector3.Cross(v1-v2,v1-v3).magnitude)<ep)
			return true;
		if((s1/(v1-v2).magnitude<radius)||(s2/(v3-v2).magnitude<radius)||(s3/(v3-v1).magnitude<radius))
		{
			return true;
		}
		return false;
	}
	 
}
