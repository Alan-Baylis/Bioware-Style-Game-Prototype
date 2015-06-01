using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InteractWithController : MonoBehaviour {

	List<GameObject> interactableObjectsInRange = new List<GameObject>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Interact() {
		if (interactableObjectsInRange.Count > 0) {
			interactableObjectsInRange [0].GetComponent<InteractableObject> ().Interact ();
		}
	}

	void OnTriggerEnter(Collider other)  {
		if (other.gameObject.GetComponent<InteractableObject> ()) {
			if (!objectInList(other.gameObject)) {
				interactableObjectsInRange.Add(other.gameObject);
			}
		}
	}

	void OnTriggerExit(Collider other)  {
		if (other.GetComponent<InteractableObject>()) {
			if (objectInList(other.gameObject)) {
				// Remove it
				interactableObjectsInRange.Remove(other.gameObject);
			}
		}
	}

	bool objectInList(GameObject other) {
		foreach(GameObject go in interactableObjectsInRange) {
			if (go == other) {
				return true;
			}
		}
		
		return false;
	}
}
