using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {

	public Camera playerCamera;

	GameObject target;

	public Vector3 OriginalCameraPosition = new Vector3(0f, 3f, -4.5f);
	public Vector3 TalkCameraPosition = new Vector3(-1f, 0.75f, -0.5f);
	public Vector3 CombatCameraPosition = new Vector3 (0f, 4f, -7f);
	public Vector3 OriginalCameraRotation = new Vector3(20f, 0f, 0f);
	Vector3 TalkCameraRotation = new Vector3(0f, 0f, 0f); // Needs to point towards the talk target
	Vector3 wantedPosition;

	float rotation_dampening = 15f;
	float dampening = 7f;

	bool smoothRotation = true;

	PlayerStats player;
	
	void Start() {
		player = GetComponent<PlayerStats>();
	}

	void LateUpdate () {
		switch (player.playerState) {
		case PlayerStats.PlayerState.Movement:
			wantedPosition = transform.position + (gameObject.transform.rotation * OriginalCameraPosition);
			playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, wantedPosition, Time.deltaTime * dampening);
			playerCamera.transform.rotation = Quaternion.Slerp (playerCamera.transform.rotation, transform.rotation * Quaternion.Euler(OriginalCameraRotation), Time.deltaTime * rotation_dampening);
			
			break;
		case PlayerStats.PlayerState.Talking:
			//TalkCameraRotation = target.transform.position - playerCamera.transform.position;
			TalkCameraRotation = target.transform.position - playerCamera.transform.position;
			TalkCameraRotation.z = 0f;
			Debug.DrawRay(playerCamera.transform.position, (target.transform.position - playerCamera.transform.position), Color.red);
			playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, transform.position + (gameObject.transform.rotation * TalkCameraPosition), Time.deltaTime * dampening);
			/*playerCamera.transform.rotation = */playerCamera.transform.LookAt(new Vector3(target.transform.position.x, target.transform.position.y+1f, target.transform.position.z));
			//playerCamera.transform.rotation = Quaternion.Slerp (playerCamera.transform.rotation, Quaternion.Euler(TalkCameraRotation), Time.deltaTime * rotation_dampening);
			break;
		case PlayerStats.PlayerState.Combat:
			wantedPosition = transform.position + (gameObject.transform.rotation * CombatCameraPosition);
			playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, wantedPosition, Time.deltaTime * dampening);
			playerCamera.transform.rotation = Quaternion.Slerp (playerCamera.transform.rotation, transform.rotation * Quaternion.Euler(OriginalCameraRotation), Time.deltaTime * rotation_dampening);
			
			break;
		}
	}

	public void SetTalkTarget(GameObject talkTarget) {
		target = talkTarget;
	}
}
