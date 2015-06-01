using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CombatManager : MonoBehaviour {
	// Check for combat range

	public float delayBetweenChecks = 0.5f;

	List<GameObject> hostileList = new List<GameObject>();
	PlayerStats playerStats;


	void Start () {
		playerStats = GetComponent<PlayerStats>();
		StartCoroutine (UpdateCombatState());
	}

	void Update () {
	}

	void StillInCombatCheck() {
		// TODO Check to see if there are any enemies aggroed
		bool hasTarget = false;
		foreach (GameObject hostile in hostileList) {
			if (hostile.GetComponent<EnemyManager>().targets.Count > 0) {
				hasTarget = true;
			}
		}

		if ((!playerStats.inCombatRange) && (!hasTarget)) {
			playerStats.playerState = PlayerStats.PlayerState.Movement;
		}
	}

	private IEnumerator UpdateCombatState(){
		while (true) {
			hostileList = GameObject.FindGameObjectsWithTag("Hostile").ToList();

			playerStats.inCombatRange = false;
			foreach (GameObject hostile in hostileList) {
				if (Vector3.Distance(transform.position, hostile.transform.position) <= playerStats.combatActivateRange) {
					playerStats.playerState = PlayerStats.PlayerState.Combat;
					playerStats.inCombatRange = true;
				}
			}

			StillInCombatCheck();

			yield return new WaitForSeconds(delayBetweenChecks);     
		}
	}
}
