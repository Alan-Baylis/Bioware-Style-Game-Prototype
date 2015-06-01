using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySight : MonoBehaviour {
	public float FieldOfView = 110f;
	public float sightRange = 25f;
	public bool enemyInSight = false;

	List<GameObject> enemy = new List<GameObject>();
	SphereCollider inRangeCollider;

	void Awake() {
		inRangeCollider = GetComponent<SphereCollider>();
		inRangeCollider.radius = sightRange;
	}

	void Update() {
		if (enemy.Count > 0) {
			CheckLineOfSight();
		} else {
			enemyInSight = false;
		}
	}
	
	void OnTriggerEnter(Collider other) {
		Debug.Log("Trigger Entered with tag: " + other.tag);
		if ((other.tag == "PlayerCollider") || (other.tag == "CompanionCollider")) {
			enemy.Add(other.gameObject);
		}
	}
	
	void OnTriggerExit(Collider other) {
		if ((other.tag == "PlayerCollider") || (other.tag == "CompanionCollider")) {
			enemy.Remove(other.gameObject);
		}
	}

	void CheckLineOfSight() {
		enemyInSight = false;
		foreach (GameObject en in enemy) {
			Vector3 dir = en.transform.position - transform.position;
			float angle = Vector3.Angle (dir, transform.forward);

			if (angle < FieldOfView * 0.5f) {
				enemyInSight = true;
				GetComponent<EnemyManager>().targets.Add(en);
			}
		}
	}
}
