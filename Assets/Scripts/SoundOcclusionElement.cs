using UnityEngine;
using System.Collections;

public class SoundOcclusionElement : MonoBehaviour {
	// This script is to handle the inner workings of each Sound occlusion element
	// this script will handle what happens when it is hit by a sound ray
	public bool hit = false;
	public int num_recursions = -1;

	// Use this for initialization
	void Start () {
		this.gameObject.layer = 0;
	}

	// on ray cast hit 
	public void isHit(){
		Debug.Log("SOE called isHit");
		hit = true;
		this.gameObject.layer = 2;
	}

	// Update is called once per frame
	void Update () {
		
	}
}
