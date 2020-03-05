using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MagicBattle;

public class MapToTexture : MonoBehaviour {
	private Mesh myMesh;
	private Vector3[] vertex;
	private int[] triangles;
	private Vector2[] uvs;
	private float mapXPosition;
	private float mapYPosition;
	private Texture2D mainText;
	private List<Vector3> pointVector = new List<Vector3>();
	private List<Vector3> currentPositionVector = new List<Vector3>();
	private int currentTriangleNumber = 0;
	
	public float originMapAlphaPercent = 0.5f;
	public float newMapAlphaPercent = 0.5f;	
	public Texture2D[] newMap;
	const float EP = 0.01f;
	// Use this for initialization
	void Start () {
		myMesh = GetComponent<MeshFilter>().mesh;
		vertex = myMesh.vertices;
		triangles = myMesh.triangles;
		uvs = myMesh.uv;
		mainText = (Texture2D)Instantiate(renderer.material.mainTexture);
		foreach(int tp in triangles) pointVector.Add(vertex[tp]);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	//When Shell Attacked
	void OnShellAttacked(ShellAttackedSendMsgParam param) {
		return;
		FindPointInTriangle(param.attackedPoint);
		CalculateUVPos(param.attackedPoint);
		StartCoroutine("AddTextureToMap");
	}
	
	//Calculate the current point in triangle and returns the triangle number
	void FindPointInTriangle(Vector3 currentPosition){
		int triangleCount = 0;
		Vector3[] tempVector = new Vector3[3];
		while(triangleCount < triangles.Length + 1){
			if(triangleCount % 3 == 0 && triangleCount > 0){
				Vector3[] squareVector = new Vector3[3];
				for(int i = 0; i < 3; i ++) squareVector[i] = currentPosition - transform.TransformPoint(tempVector[i]);
				float totalSquare = 0;
				totalSquare += Vector3.Cross(squareVector[0],squareVector[1]).magnitude;
				totalSquare += Vector3.Cross(squareVector[1],squareVector[2]).magnitude;
				totalSquare += Vector3.Cross(squareVector[0],squareVector[2]).magnitude;
				Vector3 widthVector = transform.TransformPoint(tempVector[0]) - transform.TransformPoint(tempVector[1]);
				Vector3 heightVector = transform.TransformPoint(tempVector[1]) - transform.TransformPoint(tempVector[2]);
				float currentSquare = Vector3.Cross(widthVector,heightVector).magnitude;
				if((totalSquare > currentSquare - EP) && (totalSquare < currentSquare + EP) && triangleCount < triangles.Length){
					currentTriangleNumber = triangleCount;
					for(int i = 0; i < 3; i ++) currentPositionVector.Add(transform.TransformPoint(tempVector[i]));
				}
			}
			if(triangleCount < triangles.Length) tempVector[triangleCount % 3] = pointVector[triangleCount];
			triangleCount ++;
		}
	}
	
	//Calculates the current UV pos in texture by area of triangle
	void CalculateUVPos(Vector3 currentPosition){
		if(currentTriangleNumber != 0){
			Vector2[] currentUVPos = new Vector2[3];
			int[] currentPos = new int[3];
			for(int i = 3; i > 0; i --){
				currentPos[3 - i] = triangles[currentTriangleNumber - i];
				currentUVPos[3 - i] = uvs[currentPos[3 - i]];
			}
			Vector3 rankVal = CalculateUVRank(currentUVPos);
			Vector3 preUV = CalculatePosByRate((int)rankVal.x,(int)rankVal.z,(int)rankVal.y,currentPosition,currentUVPos);
			if(preUV!= Vector3.zero){
				mapXPosition = (currentUVPos[(int)rankVal.z].x * preUV.z + preUV.x) / (1 + preUV.z);
				mapYPosition = (currentUVPos[(int)rankVal.z].y * preUV.z + preUV.y) / (1 + preUV.z);
				mapXPosition = Mathf.Clamp(mapXPosition,Min(currentUVPos).x,Max(currentUVPos).x);
				mapYPosition = Mathf.Clamp(mapYPosition,Min(currentUVPos).y,Max(currentUVPos).y);
			}
		}else{
			mapXPosition = 0;
			mapYPosition = 0;
		}
	}
	
	//This function calculate the min val
	Vector2 Min(Vector2[] posArray){
		Vector2 temp = posArray[0];
		foreach(Vector2 pt in posArray){
			if(temp.x > pt.x) temp.x = pt.x;
			if(temp.y > pt.y) temp.y = pt.y;
		}
		return temp;
	}
	
	//This function calculate the max val
	Vector2 Max(Vector2[] posArray){
		Vector2 temp = posArray[0];
		foreach(Vector2 pt in posArray){
			if(temp.x < pt.x) temp.x = pt.x;
			if(temp.y < pt.y) temp.y = pt.y;
		}
		return temp;
	}
	
	//This function calculates the current pos by vector rate
	Vector3 CalculatePosByRate(int maxIndex,int midIndex,int minIndex,Vector3 currentPosition,Vector2[] uvPos){
		if(currentPositionVector.Count < 2) return Vector3.zero;
		Vector3 temp = currentPosition - currentPositionVector[midIndex];
		float minSquareVal = Vector3.Cross(currentPositionVector[minIndex] - currentPositionVector[midIndex],temp).magnitude;
		float maxSquareVal = Vector3.Cross(temp,currentPositionVector[maxIndex] - currentPositionVector[midIndex]).magnitude;
		float rate = Mathf.Abs(maxSquareVal / minSquareVal);
		Vector3 returnPositionVal = Vector3.zero;
		returnPositionVal.x = (currentPositionVector[maxIndex].x + rate * currentPositionVector[minIndex].x) / (1 + rate);
		returnPositionVal.y = (currentPositionVector[maxIndex].y + rate * currentPositionVector[minIndex].y) / (1 + rate);
		returnPositionVal.z = (currentPositionVector[maxIndex].z + rate * currentPositionVector[minIndex].z) / (1 + rate);
		Vector3 newTemp = returnPositionVal - currentPosition;
		Vector3 returnUVPosVal = Vector3.zero;
		returnUVPosVal.x = (uvPos[maxIndex].x + rate * uvPos[minIndex].x) / (1 + rate);
		returnUVPosVal.y = (uvPos[maxIndex].y + rate * uvPos[minIndex].y) / (1 + rate);
		returnUVPosVal.z = newTemp.magnitude / temp.magnitude;
		return returnUVPosVal;
	}
	
	//This function calculates the ranks of uvs position
	Vector3 CalculateUVRank(Vector2[] uvPos){
		Vector3 retVal = Vector3.zero;
		Vector2 temp = uvPos[0];
		for(int i = 1; i < 3; i ++){
			if(temp.x < uvPos[i].x) retVal.x = i;		
			if(temp.x > uvPos[i].x) retVal.y = i;		
		}
		for(int i = 0; i < 3; i ++){
			if((i != retVal.x) && (i != retVal.y)) retVal.z = i;
		}
		return retVal;
	}
	
	//This function adds texture to the current map
	IEnumerator AddTextureToMap(){
		int mapMaxIndex = 0;
		if(newMap.Length >= 0) mapMaxIndex = newMap.Length;
		int currentMapIndex = Random.Range(0,mapMaxIndex);
		Texture2D currentNewMap = newMap[currentMapIndex];
		int xPos = Mathf.RoundToInt(mainText.width * mapXPosition -currentNewMap.width * 0.5f);
		int yPos = Mathf.RoundToInt(mainText.height * mapYPosition - currentNewMap.height * 0.5f);
		for(int i = 0; i < currentNewMap.width; i ++)
			for(int j = 0; j < currentNewMap.height; j++){
				Color newCol = currentNewMap.GetPixel(i,j);
				Color originCol = mainText.GetPixel(i + xPos,j + yPos);
				Color currentCol = newCol * newMapAlphaPercent + originCol * originMapAlphaPercent;
				if(newCol.a == 1 && originCol.a > 0) mainText.SetPixel(i + xPos ,j + yPos,currentCol);
			}
		mainText.Apply(true);
		yield return new WaitForSeconds(0.1f);
		renderer.material.mainTexture = mainText;
		currentPositionVector.Clear();
	}
}