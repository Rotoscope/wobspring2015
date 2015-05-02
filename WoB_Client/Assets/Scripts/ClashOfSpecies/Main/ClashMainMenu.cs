﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ClashPlayerElement {
	public int id;
	public string name;
	public bool isSelected;
}

public class ClashMainMenu : MonoBehaviour {
	public GameObject playerToggle;
	public List<ClashPlayerElement> playerList;
	public Transform contentPanel;
	int selectedPlayer = -1;
	int defendingTerrain = -1;
    ToggleGroup toggleGroup = null;
	ClashPreviewController pctrl;
	GameObject required_object;

	void Awake() {}

	void Start () {}

	void Update() {}

	//protocol does this
	//gets only the player name and terrain name from the valid defense table
	void RetrievePlayerList() {
		NetworkManager.Send (ClashPlayerListProtocol.Prepare (), (res) => {
			var response = res as ResponseClashPlayerList;
			foreach (var pair in response.players) {
				ClashPlayerElement element = new ClashPlayerElement();
				element.id = pair.Key;
				element.name = pair.Value;
				element.isSelected = false;
				playerList.Add (element);
			}
			Debug.Log ("playerList.Count = " + playerList.Count);
			PopulateScrollView();
		});
	}

	void PopulateScrollView() {
		foreach (var playerElement in playerList) {
			GameObject newToggle = Instantiate(playerToggle) as GameObject;
			ClashPlayerToggle toggle = newToggle.GetComponent<ClashPlayerToggle>();
			toggle.label.text = playerElement.name;
			toggle.player_name = playerElement.name;
			toggle.player_id = playerElement.id;
			toggle.toggle.isOn = playerElement.isSelected;
			toggle.toggle.onValueChanged.AddListener((value) => ToggleAction(toggle, value));
			toggle.toggle.group = toggleGroup;
			newToggle.transform.SetParent(contentPanel);
		}
	}

	public void ToggleAction(ClashPlayerToggle toggle, bool state) {
		if (state) {
			GetDefenseConfig(toggle.player_id);
		} else {
			selectedPlayer = -1;
			defendingTerrain = -1;
		}
		pctrl.text.enabled = !state;

		//pctrl.display = state ? Resources.Load("", typeof(Sprite)) : null;
		//Debug.Log (selectedPlayer);
		//Debug.Log (defendingTerrain);
	}

	public void GetDefenseConfig(int player_id) {
		NetworkManager.Send (ClashPlayerViewProtocol.Prepare (player_id), (res) => {
			var response = res as ResponseClashPlayerView;
            /*
			selectedPlayer = response.playerId;
			defendingTerrain = response.terrain;
            defenseSpecies = response.layout;
			Debug.Log (selectedPlayer);
			Debug.Log (defendingTerrain);
			Debug.Log ("Images/ClashOfSpecies/" + pd.terrain_list[defendingTerrain].name);
			Texture s = Resources.Load("Images/ClashOfSpecies/" + pd.terrain_list[defendingTerrain].name) as Texture;
			if(s == null) Debug.Log("Texture is null");
			else pctrl.display.texture = s;
            */
		});
	}
}