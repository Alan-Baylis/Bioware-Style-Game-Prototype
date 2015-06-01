using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour {
	public string npcName;
	public Vector3 nameOffset = Vector3.zero;
	public float range = 10f;

	GameObject OverheadNameObject;
	Text overheadName;
	bool triggered = false;
	GameObject playerObject;
	
	public List<GameObject> targets = new List<GameObject>();
	
	void Start () {
		playerObject = GameObject.FindGameObjectWithTag("Player");
	}
	
	void LateUpdate () {
		if (OverheadNameObject) {
			OverheadNameObject.transform.position = transform.position + nameOffset;
			OverheadNameObject.transform.rotation = Camera.main.transform.rotation;
		}
		
		if ((playerObject.transform.position - transform.position).magnitude < range) {
			TriggerEnter ();
		} else {
			TriggerExit ();
		}

		// Make the object face the player
		Vector3 targetToLookAt = playerObject.transform.position - transform.position;
		targetToLookAt.y = 0f;
		
		// Make the NPC face the player
		gameObject.transform.rotation = Quaternion.LookRotation (targetToLookAt);

		Debug.DrawRay (transform.position, transform.forward * 5f, Color.red);
	}

	void TriggerEnter() {
		if (!triggered) {
			OverheadNameObject = GameManager.Instance.GetNameLabel();
			OverheadNameObject.GetComponent<Text>().text = npcName;
			triggered = true;
		}
	}
	
	void TriggerExit() {
		if (triggered) {
			OverheadNameObject.GetComponent<Text>().text = "";
			OverheadNameObject.SetActive(false);
			OverheadNameObject = null;
			triggered = false;
		}
	}
}
