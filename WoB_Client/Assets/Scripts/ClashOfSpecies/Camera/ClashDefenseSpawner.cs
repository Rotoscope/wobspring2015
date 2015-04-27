﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;


public class ClashDefenseSpawner : MonoBehaviour {
	public ClashDefenseController cdc;
	GameObject required_object, unit;
	ClashPersistentData pd;

	void Awake() {
		required_object = GameObject.Find ("Persistent Object");
		
		if (required_object == null) {
			Application.LoadLevel ("ClashSplash");
		}
	}
	
	// Use this for initialization
	void Start () {
		pd = required_object.GetComponent<ClashPersistentData> ();
		//Debug.Log (EventSystem.current.IsPointerOverGameObject ());
	}

	
	// Update is called once per frame
	void Update () {
		SpawnInvader ();
	}

    private void SpawnInvader() {
		if (Input.GetButtonDown("Fire1") && !EventSystem.current.IsPointerOverGameObject()) {	//mouse 1 pressed
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit, 1000.0f) && hit.collider.gameObject.tag == "Terrain") {
				Toggle active_toggle = cdc.toggleGroup.GetActiveToggle();
				if(active_toggle != null) {
					int list_index = active_toggle.GetComponent<ClashDefenseToggle>().list_index;
					bool isDeployed = pd.defenderInfo.defense[list_index].isDeployed;
					if(!isDeployed) {
						switch(pd.defenderInfo.defense[list_index].prefab_id) {
						case 0:
							unit = Instantiate(Resources.Load ("Prefabs/ClashOfSpecies/Unit/Plant", typeof(GameObject)), hit.point, Quaternion.identity) as GameObject;
							break;
						case 1:
							unit = Instantiate(Resources.Load ("Prefabs/ClashOfSpecies/Unit/Carnivore", typeof(GameObject)), hit.point, Quaternion.identity) as GameObject;
							break;
						case 2:
							unit = Instantiate(Resources.Load ("Prefabs/ClashOfSpecies/Unit/Herbivore", typeof(GameObject)), hit.point, Quaternion.identity) as GameObject;
							break;
						case 3:
							unit = Instantiate(Resources.Load ("Prefabs/ClashOfSpecies/Unit/Omnivore", typeof(GameObject)), hit.point, Quaternion.identity) as GameObject;
							break;
						}
						unit.tag = "Ally";
						pd.defenderInfo.defense[list_index].isDeployed = true;
						cdc.toggleGroup.GetActiveToggle().GetComponent<ClashDefenseToggle>().toggle.isOn = false;
						cdc.toggleGroup.GetActiveToggle().GetComponent<ClashDefenseToggle>().toggle.interactable = false;
					}
				}
            }
        }
    }
}