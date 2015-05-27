using UnityEngine;
using System.Collections;

public class RecursiveRaycaster : MonoBehaviour {
	// This script is used to recursively cast rays in any and all directions
	// up to a specified number if times, each time a ray hits a solid object it 
	// will spawn a reflected ray.

	public int num_rays;		// to be equally dispersed radially
	public int num_recursions;	// number of bounces

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 dir = new Vector3 (0, 0, -1);
		Ray ray = new Ray(this.transform.position, dir);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, Mathf.Infinity) == true){
			Debug.DrawRay(this.transform.position, hit.point - this.transform.position, Color.red, 1.0f);
			Debug.Log ("hit");
			if(hit.collider.CompareTag("Solid")){
				Debug.Log("hit solid object");
				hit.collider.gameObject.GetComponent<SoundOcclusionElement>().isHit();
			}
			else if (hit.collider.CompareTag("Empty")){
				Debug.Log("hit empty object");
				hit.collider.gameObject.GetComponent<SoundOcclusionElement>().isHit();
			}
		}
	}
}
