using UnityEngine;
using System.Collections;
using InControl;

public class PlayerInput : MonoBehaviour {

	InputDevice device;
	PlayerStats player;
	ThirdPersonController tpc;
	TalkController talkc;
	InteractWithController iwc;
	AttackManager am;

	void Start() {
		player = GetComponent<PlayerStats>();
		tpc = GetComponent<ThirdPersonController>();
		talkc = GetComponent<TalkController>();
		iwc = GetComponent<InteractWithController>();
		//am = GetComponent<AttackManager>();
	}

	void Update () {
		device = InputManager.ActiveDevice;
		switch (player.playerState) {
		case PlayerStats.PlayerState.Movement:
			CheckForMovementInput();
			break;
		case PlayerStats.PlayerState.Talking:
			CheckForTalkInput();
			break;
		case PlayerStats.PlayerState.Combat:
			//CheckForCombatInput();
			break;
		}
	}

	void CheckForMovementInput() {
		tpc.UpdateMoveDirection(device.LeftStick.Value);
		tpc.UpdateLookDirection(device.RightStickX.Value);

		if (device.Action1.WasPressed) {
			//Talk
			if (!talkc.Talk()) {
				// Not talking interact!
				GetComponent<InteractWithController>().Interact();
			}
		}
	}

	void CheckForTalkInput() {
		tpc.UpdateMoveDirection(Vector2.zero);
		talkc.UpdateDirection(device.LeftStick.Value);

		if (device.Action1.WasPressed) {
			//Talk
			talkc.TalkAction();
		}

		if (device.Action2.WasPressed) {
			//Talk
			talkc.CancelTalk();
		}
	}

	
	
	void CheckForCombatInput() {
		tpc.UpdateMoveDirection(device.LeftStick.Value);
		tpc.UpdateLookDirection(device.RightStickX.Value);
		
		if (device.Action1.WasPressed) {
			// Dodge

		}

		if (device.RightTrigger.WasPressed) {
			// Show Ability
			//am.Aiming();
		}

		if (device.RightTrigger.WasReleased) {
			// Launch Ability
			//am.Attack();
		}

		if (device.RightTrigger.IsPressed && device.Action1.WasPressed) {
			// Cancel Ability
			//am.CancelAttack();
		}
	}
}
