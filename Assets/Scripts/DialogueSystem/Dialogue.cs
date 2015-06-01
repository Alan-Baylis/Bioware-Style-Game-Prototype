using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Dialogue {
	public int OrderID = -1;
	public string Name = "";
	public int NPC_ID = -1;
	public bool HasRequirements = false;
	public Dictionary<string, bool> RequiredKeys = new Dictionary<string, bool>();
	public string FailStateDialogue = "";
	public float startDelay = 0.5f;
	public Dictionary<int, DialogueNode> Nodes = new Dictionary<int, DialogueNode>();
	public Dictionary<string, int> labelLookup = new Dictionary<string, int>();

	public Dialogue(int order, string n, int npc, string dialogFailedState, float dly) {
		OrderID = order;
		Name = n;
		NPC_ID = npc;

		if (!string.IsNullOrEmpty (dialogFailedState)) {
			HasRequirements = true;

			FailStateDialogue = dialogFailedState;
		}

		startDelay = dly;
	}

	public string DialogueDisplay() {
		return "Name: " + Name + " NPC ID: " + NPC_ID + " Order ID: " + OrderID + "Has Requirement: " + HasRequirements;
	}
}
