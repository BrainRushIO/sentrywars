using UnityEngine;
using System.Collections;
public enum VRUISelectionAction {GotoMultiplayer, GotoTutorial, GotoSinglePlayer, PowerCore, Cannon, EnergyMine, Airport, Sniper, None};
public enum VRUISelectionActionType {Menu, Gameplay};
public class OnSelectAction : MonoBehaviour {
	public VRUISelectionAction thisVRUISelectionAction;
	public VRUISelectionActionType thisVRUISelectionActionType;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
