using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogueNode {
	public string type;
	public Dialogue parent;
	public int ID;
	public List<DialogueNode> NextNode = new List<DialogueNode>();
	public string dialogue = "";
	public string responseLabel = "";
	public bool endNode = false;
	public string keyToSet = "";

	public DialogueNode(Dialogue p, int i, string d) {
		parent = p;
		ID = i;
		dialogue = d;
	}

	// TODO add a sound file area so you can play a sound on script start
	// TODO add a delay so you can delay the start of the text / VO
}
