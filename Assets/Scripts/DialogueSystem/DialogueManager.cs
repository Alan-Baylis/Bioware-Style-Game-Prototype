using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour {

	bool displayDialogue = false;
	
	public GameObject backgroundObject;
	public GameObject buttonBackgroundObject;
	public GameObject dialogueTextObject;
	
	public List<GameObject> dialogueResponseObject = new List<GameObject>();

	Text dialogueText;

	Vector2 choiceDirection = Vector2.zero;

	public Dialogue currentDialogue;
	DialogueNode currentNode;

	bool ActionWasPressed = false;

	PlayerStats player;
	NPCManager NPC;

	int nodeCount = 0;

	void Start () {
		dialogueText = dialogueTextObject.GetComponent<Text>();
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
	}

	void Update () {
		if (player.playerState == PlayerStats.PlayerState.Talking) {
			if (currentNode == null) {
				displayDialogue = false;
				//Debug.Log("No current Dialogue");
			}

			if (displayDialogue) {
				// if its a question node show the questions and text
				if (currentNode.NextNode.Count > 1) {
					// There are answers to a question or a hub here
					dialogueTextObject.GetComponent<RectTransform>().position = new Vector3(dialogueTextObject.GetComponent<RectTransform>().position.x, 150f, dialogueTextObject.GetComponent<RectTransform>().position.z);

					for (int i = 0; i < currentNode.NextNode.Count; i++) {
						dialogueResponseObject[i].SetActive(true);
						dialogueResponseObject[i].GetComponent<Text>().text = currentNode.NextNode[i].dialogue;
					}
					
					backgroundObject.SetActive(false);
					buttonBackgroundObject.SetActive(true);
				} else {
					dialogueTextObject.GetComponent<RectTransform>().position = new Vector3(dialogueTextObject.GetComponent<RectTransform>().position.x, 15f, dialogueTextObject.GetComponent<RectTransform>().position.z);
					
					backgroundObject.SetActive(true);
					buttonBackgroundObject.SetActive(false);

					for (int i = 0; i < dialogueResponseObject.Count; i++) {
						dialogueResponseObject[i].SetActive(false);
					}
				}

				dialogueText.text = currentNode.dialogue;

				if (ActionWasPressed) {
					ActionWasPressed = false;

					if (currentNode.endNode) {
						//Debug.Log("End Node Hit");
						CheckForKey();
						EndDialogue();
					} else if (currentNode.NextNode.Count > 1) {
						CheckForKey();
						// Response Node
						// get the answer based on the direction
						if (choiceDirection.magnitude > GameManager.Instance.ControllerDeadZone) {
							// Get the direction and map it to an options
							int tempID = 0;
							float angle = Vector2.Angle(new Vector2(0f, 1f), choiceDirection);
							if (angle <= 45) {
								tempID = 3;
							} else if ((angle > 45) && (angle <= 135)) {
								if (choiceDirection.x > 0) {
									tempID = 2;
								} else {
									tempID = 1;
								}
							} else {
								tempID = 0;
							}

							if (tempID < currentNode.NextNode.Count) {
								AdvanceNodes(currentNode.NextNode[tempID]);
							}
						}
					} else {
						CheckForKey();
						AdvanceNodes ();
					}
				}
			}
		}
	}

	void CheckForKey() {
		if ((currentNode.keyToSet.Length > 0) && (GameManager.Instance.dialogueFlags.ContainsKey(currentNode.keyToSet))) {
			GameManager.Instance.dialogueFlags[currentNode.keyToSet] = true;
			Debug.Log("Setting Key: " + currentNode.keyToSet);
		}
	}
	
	public void StartDialogue(Dialogue d, NPCManager npc) {
		displayDialogue = true;
		currentDialogue = d;
		currentNode = currentDialogue.Nodes[0];
		NPC = npc;

		backgroundObject.SetActive(true);
		dialogueTextObject.SetActive(true);
		buttonBackgroundObject.SetActive(true);
	}

	public void EndDialogue() {
		displayDialogue = false;
		
		backgroundObject.SetActive(false);
		dialogueTextObject.SetActive(false);
		buttonBackgroundObject.SetActive(false);

		player.playerState = PlayerStats.PlayerState.Movement;
		nodeCount = 0;
		NPC.dialogueIndex++;
	}
	
	void AdvanceNodes() {
		currentNode = currentDialogue.Nodes[nodeCount].NextNode[0];
		nodeCount = currentDialogue.Nodes[nodeCount].NextNode[0].ID;
		//Debug.Log("Node " + currentNode.ID + " Goes to Node: " + currentNode.NextNode[0].ID);
	}
	
	void AdvanceNodes(int option) {
		currentNode = currentDialogue.Nodes[nodeCount].NextNode[option];
		nodeCount = option;
	}
	
	void AdvanceNodes(DialogueNode node) {
		currentNode = node;
		nodeCount = node.ID;
		//Debug.Log("Node " + currentNode.ID + " Goes to Node: " + currentNode.NextNode[0].ID);
	}

	// get the input sent from the player input handler, Action1 for advancing text, on question direction and action1
	public void DialogueAction(Vector2 direction) {
		if (direction.magnitude > GameManager.Instance.ControllerDeadZone) {
			choiceDirection = direction;
		} else {
			choiceDirection = Vector2.zero;
		}

		ActionWasPressed = true;
	}
}
