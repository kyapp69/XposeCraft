using UnityEngine;
using System.Collections;

public class CursorManager : MonoBehaviour {

	public CursorType[] cursor;
	string curState = "";
	Vector2 hotSpot = Vector2.zero;
	
	void OnDrawGizmos () {
		if(gameObject.name != "Cursor Manager"){
			gameObject.name = "Cursor Manager";
		}
	}
	
	// Use this for initialization
	void OnDrawGizmosSelected () {
		for(int x = 1; x < cursor.Length; x++){
			if(cursor[x].moveUp){
				cursor[x].moveUp = false;
				CursorType clone = cursor[x-1];
				cursor[x-1] = cursor[x];
				cursor[x] = clone;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		for(int x = 0; x < cursor.Length; x++){
			if(curState == cursor[x].tag){
				Cursor.SetCursor(cursor[x].texture, hotSpot, new CursorMode());
			}
		}
		curState = cursor[cursor.Length-1].tag;
	}
	
	public void CursorSet (string state){
		for(int x = 0; x < cursor.Length; x++){
			if(cursor[x].tag == curState){
				break;
			}
			else{
				if(cursor[x].tag == state){
					curState = state;
					break;
				}
			}
		}
	}
}