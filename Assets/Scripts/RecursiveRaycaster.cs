using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public class RecursiveRaycaster : MonoBehaviour {
	// This script is used to recursively cast rays in any and all directions
	// up to a specified number if times, each time a ray hits a solid object it 
	// will spawn a reflected ray.

	public int num_rays = 8;			// to be equally dispersed radially
	public int num_recursions = 2;		// number of bounces
	public float line_display_time = 10.0f; // in seconds
	
	private List<Tuple<Vector3, int>> rays;	// contains the info for each ray, its current direction and level of recusion.

	// Use this for initialization
	void Start () {
		// angle displacement between succesive rays
		float angle = 2.0f * Mathf.PI / 8;
		Vector3 dir = new Vector3(0.0f, 0.0f, 1.0f);
		Quaternion rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, angle));

		rays = new List<Tuple<Vector3, int>>();
		for (int i = 0; i < num_rays; i++) {
			Tuple<Vector3, int> temp_ray = new Tuple<Vector3, int>(dir, i);
			rays.Add(temp_ray);
			// rotate the dir for the next ray
			dir = rotation * dir;
		}
	}

	// This function should be called on button press to cast a single a ray from the sound
	// source, this function draws the required lines.
	// This function should recurse if encountering an empty tile and return when it either
	// fails to encounter a tile or hits a solid object.
	public void CastSingleRay(int ray_index){
		// create ray and hit objects
		Vector3 dir = rays[ray_index].First;
		Ray ray = new Ray(this.transform.position, dir);
		RaycastHit hit;
		bool recurse = true;	// set to false when ray hits a solid tile or nothing.
		// Fire the ray, and detect what it hits
		while (recurse == true){
			recurse = Physics.Raycast(ray, out hit, Mathf.Infinity);
			// draw the ray
			Debug.DrawRay(this.transform.position, hit.point - this.transform.position, Color.red, 6.0f);
			// if the ray is solid reflect the ray and increment the reflection
			if (!recurse){
				break;
			}
			else if(hit.collider.CompareTag("Solid")){
				Debug.Log("hit solid object");
				// calc the reflected ray direction and increment the recurse number
				rays[ray_index].First = dir - 2.0f * Vector3.Dot(dir, hit.normal) * hit.normal;
				rays[ray_index].Second += 1;
				// draw a debug line to show that reflect has worked
				ray = new Ray(hit.point, rays[ray_index].First);
				Vector3 point = hit.point;
				Debug.DrawRay(point, rays[ray_index].First, Color.green, 6.0f);
				recurse = false;
			}
			else if (hit.collider.CompareTag("Empty")){
				// If the object is empty fire another ray in the same direction and turn the 
				// layer off to avoid raycast hits
				Debug.Log("hit empty object");
				hit.collider.gameObject.GetComponent<SoundOcclusionElement>().isHit();
			}
		}
	}
	
	// Update is called once per frame
	void Update () {

	}
}
