using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackManager : MonoBehaviour {

	/*public GameObject attackProjector;
	float attackDelay = 0.1f;
	List<GameObject> listOfTargets = new List<GameObject>();
	bool aiming = false;

	void Start() {
		// Get the projector based on the weapon equiped
	}

	void Update () {
	
	}

	public void Aiming() {
		attackProjector.SetActive(true);
		aiming = true;
	}

	public void CancelAttack() {
		attackProjector.SetActive(false);
		aiming = false;
	}

	public void Attack() {
		if (aiming) {
			Invoke ("LaunchAttack", attackDelay);
		}
	}

	void LaunchAttack() {
		attackProjector.SetActive(false);
		Debug.Log ("Attacking " + listOfTargets.Count + " Enemies!");
		listOfTargets.Clear();
	}
	
	void OnTriggerEnter(Collider other) {
		if (GetComponent<PlayerStats> ().playerState == PlayerStats.PlayerState.Combat) {
			if (other.tag == "Hostile") {
				// Add the target to the list
				listOfTargets.Add(other.gameObject);
			}
		}
	}
	
	void OnTriggerExit(Collider other) {
		if (GetComponent<PlayerStats> ().playerState == PlayerStats.PlayerState.Combat) {
			if (other.tag == "Hostile") {
				// Remove the target from the list
				listOfTargets.Remove(other.gameObject);
			}
		}
	}*/
}
