﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileController : MonoBehaviour {
	// This class will create the tiled level for the sound source to
	// raycast within.
	public GameObject Empty_tile;
	public GameObject Solid_tile;
	public int grid_size = 10;

	private List<GameObject> Tiles;

	// Use this for initialization
	void Start () {
		Tiles = new List<GameObject>();

		Generatelevel();
	}

	// generate random level
	private void Generatelevel(){
		GameObject tile;
		// layout the tilse
		for (int j = - grid_size; j < grid_size; j++) {
			for (int i = -grid_size; i < grid_size; i++) {
				if (Random.value < 0.9f){
					tile = GameObject.Instantiate<GameObject>(Empty_tile);
				}
				else{
					tile = GameObject.Instantiate<GameObject>(Solid_tile);
				}
				tile.transform.SetParent(this.transform, false);
				tile.transform.Translate( new Vector3(i, 0.0f, j));
				Tiles.Add(tile);
			}
		}

	}

}