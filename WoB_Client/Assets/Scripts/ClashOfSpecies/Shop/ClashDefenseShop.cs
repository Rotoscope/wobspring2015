﻿using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using SpeciesType = ClashSpecies.SpeciesType;

public class ClashDefenseShop : MonoBehaviour {

	private ClashGameManager manager;
    private ClashDefenseConfig pending = new ClashDefenseConfig();

	public GridLayoutGroup carnivoreGroup;
    public GridLayoutGroup herbivoreGroup;
	public GridLayoutGroup omnivoreGroup;
	public GridLayoutGroup plantGroup;
    public GridLayoutGroup terrainGroup;

	public HorizontalLayoutGroup selectedGroup;
	public HorizontalLayoutGroup selectedTerrain;

    public Image previewImage;
    public Text previewText;

	public GameObject shopElementPrefab;
	public GameObject selectedUnitPrefab;
	public GameObject selectedTerrainPrefab;

	void Awake() {
        manager = GameObject.Find("MainObject").GetComponent<ClashGameManager>();
;
        foreach (var species in manager.availableSpecies) {
            var item = (Instantiate(shopElementPrefab) as GameObject).GetComponent<ClashShopItem>();
            item.displayText.text = species.name;

            var texture = Resources.Load<Texture2D>("Images/" + species.name);
            item.displayImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            item.addButton.onClick.AddListener(() => {
                // If item exists in the list already, don't add.
                foreach (ClashSelectedUnit existing in selectedGroup.GetComponentsInChildren<ClashSelectedUnit>()) {
                    if (existing.label.text == item.displayText.text) {
                        return;
                    }
                }

                // Instantiated a selected item prefab and configure it.
                var selected = (Instantiate(selectedUnitPrefab) as GameObject).GetComponent<ClashSelectedUnit>();
                selected.transform.SetParent(selectedGroup.transform);
                selected.image.sprite = item.displayImage.sprite;
                selected.transform.localScale = Vector3.one;
                selected.label.text = item.displayText.text;
                selected.remove.onClick.AddListener(() => {
                    Destroy(selected.gameObject);
                });
            });

            var description = species.description;
            item.previewButton.onClick.AddListener(() => {
                previewImage.sprite = item.displayImage.sprite;
                previewText.text = description;
            }); 

            switch (species.type) {
                case SpeciesType.CARNIVORE:
                    item.transform.SetParent(carnivoreGroup.transform);
                    break;
                case SpeciesType.HERBIVORE:
                    item.transform.SetParent(herbivoreGroup.transform);
                    break;
                case SpeciesType.OMNIVORE: 
                    item.transform.SetParent(omnivoreGroup.transform);
                    break;
                case SpeciesType.PLANT: 
                    item.transform.SetParent(plantGroup.transform);
                    break;
                default: break;
            }

            item.transform.position = new Vector3(item.transform.position.x, item.transform.position.y, 0.0f);
            item.transform.localScale = Vector3.one;
        }

        // Setup the terrain items list.
        List<GameObject> terrains = new List<GameObject>(Resources.LoadAll<GameObject>("Prefabs/ClashOfSpecies/Terrains"));
        foreach (GameObject t in terrains) {
            var item = (Instantiate(shopElementPrefab) as GameObject).GetComponent<ClashShopItem>();
            
            var texture = Resources.Load<Texture2D>("Images/ClashOfSpecies/" + t.name);
            item.displayImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            item.displayText.text = t.name;

            item.addButton.onClick.AddListener(() => {
                // If a terrain has already been selected, destroy it first.
                var existing = selectedTerrain.GetComponentInChildren<ClashSelectedUnit>();
                if (existing != null) {
                    Destroy(existing.gameObject);
                }

                // Add the newly selected terrain object.
                var selected = (Instantiate(selectedUnitPrefab) as GameObject).GetComponent<ClashSelectedUnit>();
                selected.transform.SetParent(selectedTerrain.transform);
                selected.transform.position = new Vector3(item.transform.position.x, item.transform.position.y, 0.0f);
                selected.transform.localScale = Vector3.one;
                selected.image.sprite = item.displayImage.sprite;
                selected.label.text = item.displayText.text;
                selected.remove.onClick.AddListener(() => {
                    Destroy(selected.gameObject);
                });
            });

            item.previewButton.onClick.AddListener(() => {
                previewImage.sprite = item.displayImage.sprite;
                previewText.text = "Terrain";
            });

            item.transform.SetParent(terrainGroup.transform);
            item.transform.position = new Vector3(item.transform.position.x, item.transform.position.y, 0.0f);
            item.transform.localScale = Vector3.one;
        }
	}

    // Use this for initialization
    void Start() {
        if (manager.lastDefenseConfig != null) {
            // Populate with the last defense setup.
        }
	}

    void PlaceDefense() {
        if (selectedTerrain.transform.childCount == 1 && selectedGroup.transform.childCount == 5) {
            manager.pendingDefenseConfig.owner = manager.currentPlayer;
            manager.pendingDefenseConfig.terrain = selectedTerrain.GetComponentInChildren<ClashSelectedUnit>().label.name;
            manager.pendingDefenseConfig.layout = new Dictionary<ClashSpecies, Vector2>();
            foreach (ClashSelectedUnit csu in selectedGroup.GetComponentsInChildren<ClashSelectedUnit>()) {
                var species = manager.availableSpecies.Single(x => x.name == csu.label.text);
                manager.pendingDefenseConfig.layout.Add(species, new Vector2());
            }
            Game.LoadScene("ClashDefense");
        }
    }

    void BackToLobby() {
        Destroy(manager); 
        //TODO: Talk to lobby about which scene to load.
    }
}
