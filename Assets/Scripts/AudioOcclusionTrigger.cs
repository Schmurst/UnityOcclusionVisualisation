using UnityEngine;
using System.Collections;

[AddComponentMenu ("Audio/Occlusion Trigger")]

public class AudioOcclusionTrigger : MonoBehaviour {
	
	public occlusionCategory occlusionType;
	public float volumeOccludePct = 1.0f;
	public float lpfCutoff = 22000.0f;
	
	void OnTriggerEnter(Collider other) {
		if ( other.tag != "Player" ) return;
        if ( other.networkView == null ) return;
        if ( !other.networkView.isMine ) return;
		//gathers all occlusion objects whose occlusionType matches this trigger's occlusionType and attenuates them by volumeOccludEPct and lpfCutoff
		AudioOcclusionObject[] occlusionObjects = (AudioOcclusionObject[])GameObject.FindObjectsOfType(typeof(AudioOcclusionObject));
		//Debug.Log("I'm in! " + occlusionObjects.Length);		
		foreach(AudioOcclusionObject curObject in occlusionObjects) {
			if( curObject.occlusionTypes.Contains( occlusionType ) ) {
				curObject.Occlude(volumeOccludePct, lpfCutoff);
			}
		}
	}
	
	
	void OnTriggerExit(Collider other) {
		if ( other.tag != "Player" ) return;
        if ( other.networkView == null ) return;
        if ( !other.networkView.isMine ) return;
		
		//restore occluded objects
		AudioOcclusionObject[] occlusionObjects = (AudioOcclusionObject[])GameObject.FindObjectsOfType(typeof(AudioOcclusionObject));
		//Debug.Log("exiting!!!");
		foreach(AudioOcclusionObject curObject in occlusionObjects) {
			if( curObject.occlusionTypes.Contains( occlusionType ) ) {
				curObject.EndOcclude();
			}
		}		
	}
	
}
