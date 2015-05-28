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
			Debug.DrawRay(this.transform.position, hit.point - this.transform.position, Color.red, 6.0f);
			Debug.Log ("hit");
			if(hit.collider.CompareTag("Solid")){
				Debug.Log("hit solid object");
				//hit.collider.gameObject.GetComponent<SoundOcclusionElement>().isHit();
				Vector3 new_dir = dir - 2.0f * Vector3.Dot(dir, hit.normal) * hit.normal;
				ray = new Ray(hit.point, new_dir);
				Vector3 point = hit.point;
				Debug.DrawRay(point, new_dir, Color.green, 6.0f);
				Physics.Raycast(ray, out hit, Mathf.Infinity);
				Debug.DrawRay(point, hit.point - point, Color.blue, 6.0f);
				Debug.Log ("hit");
			}
			else if (hit.collider.CompareTag("Empty")){
				Debug.Log("hit empty object");
				hit.collider.gameObject.GetComponent<SoundOcclusionElement>().isHit();
			}
		}
	}
}
