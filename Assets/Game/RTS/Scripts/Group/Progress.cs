﻿using UnityEngine;
using System.Collections;

public class Progress : MonoBehaviour {
	BuildingController build;
	public float yIncrease = 20;
	public int scale = 100;
	public int yScale = 10;
	public Texture[] texture = new Texture[0];
	public Color color = Color.green;
	
	Camera cam;
	int cur = 0;
	
	public void Start () {
		GameObject.Find("Player Manager").GetComponent<GUIManager>().AddProgress(this);
	}
	
	public void DisplayProgress (){
		if(!build){
			build = GetComponent<BuildingController>();
		}
		if(!cam){
			cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
		}
		Vector2 point = cam.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + yIncrease, transform.position.z));
		float progressReq = 0;
		float progressCur = 0;
		progressReq = build.progressReq;
		progressCur = build.progressCur;
		cur = (int)((texture.Length-1)*(progressCur/progressReq));
		if(texture[cur] != null){
			Color originalColor = GUI.color;
			GUI.color = color;
			GUI.DrawTexture(new Rect(point.x-scale/2, (Screen.height-point.y)-yScale/2, scale, yScale), texture[cur]);
			GUI.color = originalColor;
		}
	}
}