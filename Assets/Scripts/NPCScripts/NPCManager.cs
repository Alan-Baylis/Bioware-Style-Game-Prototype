using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class NPCManager : MonoBehaviour {

	public string npcName;
	
	public int npcID = -1;
	List<Dialogue> NpcDialogue = new List<Dialogue>();
	public int dialogueIndex = 0;
	GameObject DialogueManagerObject;
	DialogueManager dialogueManager;

	GameObject OverheadNameObject;
	Text overheadName;

	bool triggered = false;

	public Vector3 nameOffset = Vector3.zero;
	public float range = 10f;
	Vector3 targetToLookAt;
	GameObject playerObject;

	void Start () {
		//overheadName = OverheadNameObject.GetComponent<Text>();

		playerObject = GameObject.FindGameObjectWithTag("Player");

		DialogueManagerObject = GameObject.FindGameObjectWithTag("DialogueManager");
		dialogueManager = DialogueManagerObject.GetComponent<DialogueManager>();

		//Debug.Log (Name + " Getting Dialogue");
		NpcDialogue = GameManager.Instance.GetDialogueFromID(npcID);
	}

	void Update () {
		Debug.DrawRay (transform.position, transform.forward * 2f, Color.red);
	}

	void LateUpdate () {
		if (OverheadNameObject) {
			// Make the text face the camera and be above the model
			OverheadNameObject.transform.position = transform.position + nameOffset;
			OverheadNameObject.transform.rotation = Camera.main.transform.rotation;

			// Make the object face the player
			targetToLookAt = playerObject.transform.position - transform.position;
			targetToLookAt.y = 0f;
			
			// Make the NPC face the player
			gameObject.transform.rotation = Quaternion.LookRotation (targetToLookAt);
		}
		
		if ((playerObject.transform.position - transform.position).magnitude < range) {
			// Player in range
			TriggerEnter ();
		} else {
			TriggerExit ();
		}
	}

	public bool StartingDialogue() {
		if (NpcDialogue.Count > 0) {
			int DialogueCheckCounter = -1;
			Dialogue DialogueToCheck = NpcDialogue[0];
			bool done = false;

			while (!done) {
				DialogueCheckCounter++;
				if (DialogueCheckCounter > NpcDialogue.Count) {
					done = true;
				}

				if (DialogueToCheck.HasRequirements) {
					List<string> keyList = new List<string>(DialogueToCheck.RequiredKeys.Keys);
					bool requirementMet = true;
					for (int i = 0; i < keyList.Count; i++) {
						if (!string.IsNullOrEmpty(keyList[i])) {
							bool flagValue = false;
							GameManager.Instance.dialogueFlags.TryGetValue(keyList[i], out flagValue);
							if (flagValue != DialogueToCheck.RequiredKeys[keyList[i]]) {
								requirementMet = false;
							}
						}
					}

					if (requirementMet) {
						dialogueManager.StartDialogue(DialogueToCheck, GetComponent<NPCManager>());
						return true;
					} else {
						DialogueToCheck = GetDialogueFromList(DialogueToCheck.FailStateDialogue);
						
						if (DialogueToCheck == null) {
							//Debug.Log("No dialogue by that name on this npc.");
							//Debug.Log(DialogueToCheck.DialogueDisplay());
							return false;
						}
					}
				} else {
					//Debug.Log("No Requirements, Dialogue Sent");
					//Debug.Log(DialogueToCheck.DialogueDisplay());
					dialogueManager.StartDialogue(DialogueToCheck, GetComponent<NPCManager>());
					return true;
				}
			}
		}

		return false;
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

	Dialogue GetDialogueFromList(string NextDialogue) {
		foreach (Dialogue d in NpcDialogue) {
			if (d.Name == NextDialogue) {
				return d;
			}
		}

		return null;
	}
}
