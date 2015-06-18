using UnityEngine;
using System.Collections;

public class TalkController : MonoBehaviour {

	float targetInRange = 3f;
	float rayOffset = 0.6f;

	GameObject talkTarget;
	PlayerStats player;

	GameObject DialogueManagerObject;
	DialogueManager dialogueManager;

	Vector2 direction;
	
	void Start() {
		player = GetComponent<PlayerStats>();
		DialogueManagerObject = GameObject.FindGameObjectWithTag("DialogueManager");
		dialogueManager = DialogueManagerObject.GetComponent<DialogueManager>();
	}

	void Update () {
		Debug.DrawRay(transform.position + (gameObject.transform.rotation * Vector3.forward * rayOffset), (gameObject.transform.rotation * Vector3.forward) * targetInRange, Color.red);
	}

	public bool Talk() {
		Ray ray = new Ray(transform.position + (gameObject.transform.rotation * Vector3.forward * rayOffset), (gameObject.transform.rotation * Vector3.forward) * targetInRange);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, targetInRange)) {
			if (hit.collider.tag == "NPC") {
				//Debug.Log("Set Talk Target to " + hit.collider.name);
				talkTarget = hit.collider.gameObject;

				// Position the camera
				GetComponent<CameraManager>().SetTalkTarget(talkTarget);

				// Disable movement and switch controls to talk controls
				player.playerState = PlayerStats.PlayerState.Talking;

				if (!talkTarget.GetComponent<NPCManager>().StartingDialogue()) {
					player.playerState = PlayerStats.PlayerState.Movement;
					return false;
				}

				return true;
			}
		}
		return false;
	}

	public void CancelTalk() {
		player.playerState = PlayerStats.PlayerState.Movement;
		dialogueManager.EndDialogue();
	}

	public void UpdateDirection(Vector2 dir) {
		direction = dir;
		if (dialogueManager != null) {
			dialogueManager.DialogueActionUIView(dir);
		}
	}

	public void TalkAction() {
		dialogueManager.DialogueAction(direction);
	}
}
