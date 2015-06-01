using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour {

	public enum PlayerState {
		Movement,
		Talking,
		Combat
	}
	public PlayerState playerState = PlayerState.Movement;
	public float speed = 100f;
	public float rotationSpeed = 100f;

	public float combatActivateRange = 40f; // Range to trigger player combat
	public bool inCombatRange = false;
}
