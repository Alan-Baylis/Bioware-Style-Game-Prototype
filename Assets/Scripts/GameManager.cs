using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class GameManager : MonoBehaviour {

	public float ControllerDeadZone;

	List<Dialogue> DialogueList = new List<Dialogue>();
	List<GameObject> NameLabelList = new List<GameObject>();

	public Dictionary<string, bool> dialogueFlags = new Dictionary<string, bool>();
	public string dialogueFilePath = Application.dataPath + "/Resources/dialog.txt";

	static GameManager _instance;
	static public bool isActive { 
		get { 
			return _instance != null; 
		} 
	}
	
	static public GameManager Instance {
		get {
			if (_instance == null) {
				_instance = Object.FindObjectOfType(typeof(GameManager)) as GameManager;
				
				if (_instance == null) {
					GameObject go = new GameObject("_GameManager");
					DontDestroyOnLoad(go);
					_instance = go.AddComponent<GameManager>();
					Debug.Log("Debug: Creating A New Game Manager");
				}
				
				_instance.InitializeGameVariables();
			}
			return _instance;
		}
	}
	
	public void OnApplicationQuit() {
		_instance = null;
	}
	
	public void LoadState(string scene) {
		Application.LoadLevel(scene);
	}

	private void InitializeGameVariables() {
		// Setup Game Variables here
		ControllerDeadZone = 0.15f;

		// Load the dialogue from a file
		LoadDialogue(dialogueFilePath);
		NameLabelList = GameObject.FindGameObjectsWithTag("NameLabel").ToList();
		foreach (GameObject go in NameLabelList) {
			go.SetActive(false);
		}
	}

	public GameObject GetNameLabel() {
		foreach (GameObject go in NameLabelList) {
			if (!go.activeSelf) {
				go.SetActive(true);
				return go;
			}
		}

		return null;
	}

	void LoadDialogue(string filePath) {
		StreamReader dialogueFile = new StreamReader(filePath);
		Dialogue tempDiag = null;
		bool nodeMode = false;
		int NodeCount = 0;
		int lastNonResponseNode = 0;
		
		while (!dialogueFile.EndOfStream) {
			string line = dialogueFile.ReadLine();

			// Do stuff with the line
			// if the line starts with a [ its a dialogue object line
			
			if (line.Length > 0) {
				bool commentLine = false;
				int bracket_tracker = 0;

				if (line.TrimStart().StartsWith("//")) {
					commentLine = true;
				}

				if (!commentLine) {
					if (line[0] == char.Parse("[")) {
						//Debug.Log("----- Dialogue Line -----");
						// loop through the characters in the line and extract the data
						string ordid = "";
						string name = "";
						string npcid = "";
						string reqk = "";
						string keyreqstate = "";
						string reqgoto = "";
						string dly = "";

						for(int i = 1; i < line.Length; i ++) {
							if (line[i] == char.Parse("]")) {
								// set the tracking var
								bracket_tracker++;
								i += 1;
								//Debug.Log("Pass " + bracket_tracker + " | Requirement Key: " + reqk + " | Set Key: " + setk + " | Delay Key: " + dly);
							}
							
							if (i < line.Length) {
								if (line[i] != char.Parse("[")) {
									switch (bracket_tracker) {
									case 0:
										// Order id
										ordid += line[i];
										break;
									case 1:
										// Dialog Name
										name += line[i];
										break;
									case 2:
										// NPC id
										npcid += line[i];
										break;
									case 3:
										// req key
										reqk += line[i];
										break;
									case 4:
										// req key state
										keyreqstate += line[i];
										break;
									case 5:
										// dialog name
										reqgoto += line[i];
										break;
									case 6:
										// delay
										dly += line[i];
										break;
									default:
										break;
									}

									if (line.Substring(i).TrimStart().StartsWith("//")) {
										i = line.Length;
									}
								}
							}
						}

						tempDiag = new Dialogue(int.Parse(ordid), name, int.Parse(npcid), reqgoto, float.Parse(dly));
						// Parse the list of Required Keys and Key States and add them to the dictionary
						if (reqk.Contains(",")) {
							List<string> keys = reqk.Split(',').ToList();
							List<string> tempstates = keyreqstate.Split(',').ToList();
							List<bool> states = new List<bool>();
							foreach(string s in tempstates) {
								if (s.ToUpper() == "T") {
									states.Add(true);
								} else {
									states.Add(false);
								}
							}
							for (int x = 0; x < keys.Count; x++) {
								//Debug.Log(keys[x] + ": " + states[x]);
								tempDiag.RequiredKeys.Add(keys[x], states[x]);
								
								if (!dialogueFlags.ContainsKey(keys[x])) {
									dialogueFlags.Add(keys[x], false);
								}
							}
						} else {
							if (!string.IsNullOrEmpty(reqk)) {
								bool tempState;
								if (keyreqstate.ToUpper() == "T") {
									tempState = true;
								} else {
									tempState = false;
								}
								tempDiag.RequiredKeys.Add(reqk, tempState);

								if (!dialogueFlags.ContainsKey(reqk)) {
									dialogueFlags.Add(reqk, false);
								}
							}
						}
						//
						DialogueList.Add(tempDiag);
					} else if(line[0] == char.Parse("{")) {
						// Start of nodes
						nodeMode = true;
					} else if(line[0] == char.Parse("}")) {
						// End of nodes
						nodeMode = false;
						NodeCount = 0;
					}
					
					if ((nodeMode) && line[0] != char.Parse("{")) {
						if (line.Trim().Length > 1) {
							string diag = "";
							string labelName = "";
							string tempLine = "";
							string key = "";
							int start;
							int end;
							DialogueNode tempN = null;
							
							// check the first char for a flag
							switch (line.TrimStart()[0]) {
							case 'l':
								lastNonResponseNode = NodeCount;
								// create the node
								start = line.IndexOf("l") + "l ".Length;
								end = line.Substring(start).IndexOf(" ");
								labelName = line.Substring(start, end);

								start = line.IndexOf("\"") + 1;
								end = line.LastIndexOf("\"") - start;
								diag = line.Substring(start, end);

								
								tempN = new DialogueNode(tempDiag, NodeCount, diag);
								tempN.type = "label";
								
								if (line.Contains("[")) {
									start = line.IndexOf("[") + 1;
									end = line.LastIndexOf("]") - start;
									key = line.Substring(start, end);

									if (!dialogueFlags.ContainsKey(key)) {
										dialogueFlags.Add(key, false);
									}

									tempN.keyToSet = key;
									//Debug.Log("Node " + NodeCount + " Key: " + key);
								}

								if (line.TrimEnd().EndsWith("e")) {
									tempN.endNode = true;
									//Debug.Log(NodeCount + " End Node: " + diag);
								}

								if (!tempDiag.labelLookup.ContainsKey(labelName)) {
									tempDiag.labelLookup.Add(labelName, NodeCount);
								}
								
								if (line.TrimEnd().EndsWith("g")) {
									// set the next node
									start = line.LastIndexOf("\"") + "l ".Length;
									end = line.Length-2 - start;
									labelName = line.Substring(start, end);
									
									//Debug.Log("Node " + NodeCount + " Redirect Label: " + labelName);
									
									tempN.responseLabel = labelName;
									tempN.type = "redirect";

									if (!tempDiag.labelLookup.ContainsKey(labelName)) {
										tempDiag.labelLookup.Add(labelName, NodeCount);
									}
								}

								tempDiag.Nodes.Add(NodeCount, tempN);

								//Debug.Log(NodeCount + " Label: " + labelName + " | Label Node: " + diag);
								break;
							case 'r':
								start = line.IndexOf("r") + "r ".Length;
								end = line.Substring(start).IndexOf(" ");
								labelName = line.Substring(start, end); // for int lookup

								start = line.IndexOf("\"") + 1;
								end = line.LastIndexOf("\"") - start;
								diag = line.Substring(start, end);

								tempN = new DialogueNode(tempDiag, NodeCount, diag);
								tempN.responseLabel = labelName;
								tempN.type = "response";
								
								if (line.Contains("[")) {
									start = line.IndexOf("[") + 1;
									end = line.LastIndexOf("]") - start;
									key = line.Substring(start, end);
									
									if (!dialogueFlags.ContainsKey(key)) {
										dialogueFlags.Add(key, false);
									}

									tempN.keyToSet = key;
									//Debug.Log("Node " + NodeCount + " Key: " + key);
								}
								
								if (line.TrimEnd().EndsWith("g")) {
									// set the next node
									start = line.LastIndexOf("\"") + "l ".Length;
									end = line.Length-2 - start;
									labelName = line.Substring(start, end);
									
									//Debug.Log("Node " + NodeCount + " Redirect Label: " + labelName);
									
									tempN.responseLabel = labelName;
									tempN.type = "redirect";
								}

								tempDiag.Nodes.Add(NodeCount, tempN);

								//Debug.Log(NodeCount + " Label: " + labelName + " | Response Node: " + diag);
								break;
							default:
								lastNonResponseNode = NodeCount;
								start = line.IndexOf("\"") + 1;
								end = line.LastIndexOf("\"") - start;
								diag = line.Substring(start, end);

								tempN = new DialogueNode(tempDiag, NodeCount, diag);
								tempN.type = "basic";
								
								if (line.Contains("[")) {
									start = line.IndexOf("[") + 1;
									end = line.LastIndexOf("]") - start;
									key = line.Substring(start, end);
									
									if (!dialogueFlags.ContainsKey(key)) {
										dialogueFlags.Add(key, false);
									}

									tempN.keyToSet = key;
									//Debug.Log("Node " + NodeCount + " Key: " + key);
								}

								if (line.TrimEnd().EndsWith("g")) {
									// set the next node
									start = line.LastIndexOf("\"") + "l ".Length;
									end = line.Length-2 - start;
									labelName = line.Substring(start, end);
									
									//Debug.Log("Node " + NodeCount + " Redirect Label: " + labelName);
									
									tempN.responseLabel = labelName;
									tempN.type = "redirect";
								}
								if (line.TrimEnd().EndsWith("e")) {
									tempN.endNode = true;
									//Debug.Log(NodeCount + " End Node: " + diag);
								}

								tempDiag.Nodes.Add(NodeCount, tempN);
								

								break;
							}

							NodeCount++;
						}
					}
				}
			}
		}

		dialogueFile.Close();

		CalculateNextNodePoints();
	}

	void CalculateNextNodePoints() {
		//Debug.Log("---------------- Calculating Node Next Points");
		// loop through the dialogue list
		foreach (Dialogue d in DialogueList) {
			//Debug.Log("------------ New Dialogue");
			for(int i = 0; i < d.Nodes.Count; i++) {
				// get the first one, check the next for a type if its response add it and check the following
				if ((!d.Nodes[i].endNode) && (i+1 < d.Nodes.Count)) {
					if (d.Nodes[i].type == "redirect") {
						// It's a redirect do the lookup
						int lookupID = -1;
						DialogueNode temp;
						
						d.labelLookup.TryGetValue(d.Nodes[i].responseLabel.ToString(), out lookupID);
						d.Nodes.TryGetValue(lookupID, out temp);
						d.Nodes[i].NextNode.Add(temp);
						//Debug.Log("Redirect Node " + d.Nodes[i].ID + "'s Next Node is " + d.Nodes[i].NextNode[0].ID + " | Label: " + d.Nodes[i].responseLabel.ToString());
					} else if (d.Nodes[i].type != "response") {
						if (d.Nodes[i+1].type != "response") {
							d.Nodes[i].NextNode.Add(d.Nodes[i+1]);
						} else {
							bool done = false;
							for (int x = i+1; x < d.Nodes.Count; x++) {
								if ((!done) && (d.Nodes[x].type == "response")) {
									d.Nodes[i].NextNode.Add(d.Nodes[x]);
								} else {
									done = true;
								}
							}
						}
						
						// All DEBUG can be deleted
						/*string tempDebug = "Normal Node " + d.Nodes[i].ID + "'s Next Node is Nodes are ";
						for(int y = 0; y < d.Nodes[i].NextNode.Count; y++) {
							tempDebug += d.Nodes[i].NextNode[y].ID + ", ";
						}
						Debug.Log(tempDebug.Substring(0, tempDebug.Length-2));*/
					} else {
						// It's responsive do the lookup
						int lookupID = -1;
						DialogueNode temp;

						d.labelLookup.TryGetValue(d.Nodes[i].responseLabel.ToString(), out lookupID);
						d.Nodes.TryGetValue(lookupID, out temp);
						d.Nodes[i].NextNode.Add(temp);

						//Debug.Log("Response Node " + d.Nodes[i].ID + "'s Next Node is " + d.Nodes[i].NextNode[0].ID);
					}
				} else {
					//Debug.Log("End Node " + d.Nodes[i].ID + " is an end point.");
				}
			}
		}
	}

	public List<Dialogue> GetDialogueFromID(int id) {
		// Gets the list based on NPC ID
		List<Dialogue> tempDialogueList = new List<Dialogue>();
		for (int i = 0; i < DialogueList.Count; i++) {
			if (DialogueList[i].NPC_ID == id) {
				tempDialogueList.Add(DialogueList[i]);
			}
		}

		// Sorts the list based on Order ID
		tempDialogueList = tempDialogueList.OrderBy (x => x.OrderID).ToList();

		//Debug.Log("");
		//Debug.Log("NPC " + id + " Returns");
		//for (int i = 0; i < tempDialogueList.Count; i++) {
			//Debug.Log ("     Item " + tempDialogueList [i].OrderID + ": " + tempDialogueList [i].Name);
		//}

		return tempDialogueList;
	}
}
