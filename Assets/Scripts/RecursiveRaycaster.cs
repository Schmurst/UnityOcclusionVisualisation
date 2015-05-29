using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;
using UnityEngine.UI;

public class RecursiveRaycaster : MonoBehaviour
{
	// This script is used to recursively cast rays in any and all directions
	// up to a specified number if times, each time a ray hits a solid object it 
	// will spawn a reflected ray.

	public int num_rays = 16;			// to be equally dispersed radially
	public Slider slider_rays;			// user control of the number rays
	public int num_recursions = 10;		// number of bounces
	public Slider slider_recursions;	// user controls of the number of recusions
	public float line_display_time = 10.0f; // in seconds

	public Color start_colour;
	public Color end_colour;
	private List<Tuple<Vector3, Vector3>> rays;	// contains the info for each ray, its current direction and level of recusion.
	private int current_recursion = 0;

	// Use this for initialization
	void Start ()
	{
		init ();
		slider_rays.onValueChanged.AddListener (delegate {rayValueChange ();});
		slider_recursions.onValueChanged.AddListener (delegate {recursionValueChange ();});
		rayValueChange();
		recursionValueChange();
	}

	public void rayValueChange ()
	{
		num_rays = (int) Mathf.Pow(2, slider_rays.value);;
		slider_rays.GetComponentInChildren<Text> ().text = "Rays: " + num_rays;
		init ();
	}

	public void recursionValueChange ()
	{
		if (current_recursion > (int) slider_recursions.value * 2){
			init ();
		}
		num_recursions = (int)slider_recursions.value * 2;
		slider_recursions.GetComponentInChildren<Text> ().text = "Recursions: " + num_recursions;

	}

	// init the recusion system.
	public void init ()
	{
		// angle displacement between succesive rays
		float angle = 360.0f / (float)num_rays;
		Vector3 dir = new Vector3 (0.0f, 0.0f, 1.0f);
		current_recursion = 0;

		rays = new List<Tuple<Vector3, Vector3>> ();
		for (int i = 0; i < num_rays; i++) {
			Tuple<Vector3, Vector3> temp_ray = new Tuple<Vector3, Vector3> (dir, this.transform.position);
			rays.Add (temp_ray);
			// rotate the dir for the next ray
			dir = Quaternion.Euler (0.0f, angle, 0.0f) * dir;
		}
	}
	
	// This function should call Single ray cast and should be called by the user.
	// This function should cast each ray in turn. if the ray cast returns false
	// this function should stop calling that ray, it it returns true then incrememnt
	// the recusion.
	public void StepRayCast ()
	{
		if (current_recursion < num_recursions) {
			for (int i = 0; i < num_rays; i++) {
				CastSingleRay (i, current_recursion);
			}
			current_recursion += 1;
		}
	}

	// this will fire all the rays until the user set recusion limit is hit.
	public void FireAllRays ()
	{
		while (current_recursion < num_recursions) {
			StepRayCast ();
		}
	}

	// This function should be called on button press to cast a single a ray from the sound
	// source, this function draws the required lines.
	// This function should recurse if encountering an empty tile and return when it either
	// fails to encounter a tile or hits a solid object.
	public bool CastSingleRay (int ray_index, int recursion)
	{
		// create ray and hit objects
		Vector3 dir = rays [ray_index].First;
		Vector3 pos = rays [ray_index].Second;
		Ray ray = new Ray (pos, dir);
		RaycastHit hit;
		bool recurse = true;	// set to false when ray hits a solid tile or nothing.
		// Fire the ray, and detect what it hits
		while (recurse == true) {
			recurse = Physics.Raycast (ray, out hit, Mathf.Infinity);
			// if the ray is solid reflect the ray and increment the reflection
			if (!recurse) {
				Debug.DrawRay (pos, dir, Color.red, 6.0f);
				return false;
			} else if (hit.collider.CompareTag ("Solid")) {
				Debug.DrawRay (pos, hit.point - pos, Color.blue, 6.0f);
				Vector3 point = hit.point;
				// calc the reflected ray direction and increment the recurse number
				rays [ray_index].First = dir - 2.0f * Vector3.Dot (dir, hit.normal) * hit.normal;
				rays [ray_index].Second = point;
				// draw a debug line to show that reflect has worked
				Debug.DrawRay (point, hit.normal, Color.green, 6.0f);
				recurse = false;
				return true;
			} else if (hit.collider.CompareTag ("Empty")) {
				// If the object is empty fire another ray in the same direction and turn the 
				// layer off to avoid raycast hits
				float colour_fraction = (float)recursion / (float)num_recursions;
				Color temp = Color.Lerp (start_colour, end_colour, colour_fraction);
				hit.collider.gameObject.GetComponent<SoundOcclusionElement> ().isHit (temp);
			}
		}
		Debug.LogError ("Single Ray cast control flow error");
		return false;
	}

}
