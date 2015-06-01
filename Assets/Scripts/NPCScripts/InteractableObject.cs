using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InteractableObject : MonoBehaviour {
	public string ObjectName = "";
	public Vector3 offsetPosition = Vector3.zero;
	public float range = 5f;

	public bool inInteractableRange = false;

	public delegate void InteractScript();
	public InteractScript interactScript;

	GameObject nameLabelReference;
	GameObject player;
	bool triggered = false;

	public bool scrolltext = false;
	public float scrolltimer = 0f;
	public float scrolldelay = 1.5f;
	public Vector3 scrollHeight = Vector3.zero;

	public enum InteractableType {
		GainItem,
		ChangeFlag,
		BothFlagAndItem,
		EventTrigger,
		CutSceneTrigger
	}
	public InteractableType interactType = InteractableType.GainItem;
	public bool useLootTable = false;
	public string itemToGiveToPlayer = "";
	public string flagToTrigger = "";
	public bool setFlagTo = false;
	public bool onlyInteractableOnce = false;

	bool used = false;

	void Start() {
		player = GameObject.FindGameObjectWithTag("Player");

		switch (interactType) {
		case InteractableType.GainItem:
			interactScript = GivePlayerItem;
			break;
		case InteractableType.ChangeFlag:
			interactScript = TriggerFlag;
			break;
		case InteractableType.BothFlagAndItem:
			interactScript += GivePlayerItem;
			interactScript += TriggerFlag;
			break;
		default:
			break;
		}
	}

	void LateUpdate () {
		if (nameLabelReference) {
			if (scrolltext) {
				scrolltimer += Time.deltaTime;
				nameLabelReference.transform.position = transform.position + offsetPosition + (scrollHeight * (scrolltimer / scrolldelay));
				Color c = nameLabelReference.GetComponent<Text>().color;
				c.a = 1f - (scrolltimer / scrolldelay);
				nameLabelReference.GetComponent<Text>().color = c;
			} else {
				nameLabelReference.transform.position = transform.position + offsetPosition;
			}

			nameLabelReference.transform.rotation = Camera.main.transform.rotation;
		}
		
		if (!used) {
			if ((player.transform.position - transform.position).magnitude < range) {
				// Player in range
				TriggerEnter ();
			} else {
				TriggerExit ();
			}
		} else {
			interactScript = null;
		}
	}
	
	void TriggerEnter() {
		if ((!triggered) && (!used)) {
			nameLabelReference = GameManager.Instance.GetNameLabel();
			nameLabelReference.GetComponent<Text>().text = ObjectName;
			triggered = true;
		}
	}
	
	void TriggerExit() {
		if (triggered) {
			nameLabelReference.GetComponent<Text>().text = "";
			nameLabelReference.SetActive(false);
			nameLabelReference = null;
			triggered = false;
		}
	}

	public void Interact() {
		Debug.Log("Player Interacts with " + ObjectName + " Which is a " + interactType);
		if (interactScript != null) {
			interactScript();

			if (onlyInteractableOnce) {
				used = true;
				Invoke ("TriggerExit", scrolldelay);
			}
		}
	}

	void GivePlayerItem() {
		Debug.Log("Player Gets " + itemToGiveToPlayer + " from the " + ObjectName);
		nameLabelReference.GetComponent<Text>().text = "+ " + itemToGiveToPlayer;// + "\n" + "Test";  // for adding multiple items, I can just add \n
		// scroll the text up in the y direction
		scrolltext = true;
		scrolltimer = 0f;
	}

	void TriggerFlag() {
		if (GameManager.Instance.dialogueFlags.ContainsKey(flagToTrigger)) {
			GameManager.Instance.dialogueFlags[flagToTrigger] = setFlagTo;
		}
	}
}
