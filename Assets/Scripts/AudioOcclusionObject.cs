using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//the category is used to determine which sounds to occlude when based on the setting here and in the AudioOcclusionTrigger
public enum occlusionCategory {
	ExtAmbience = 0,
	IntAmbience,
	Hallway,
	Custom1,
	Custom2
}

[AddComponentMenu ("Audio/Occlusion Object")]

public class AudioOcclusionObject : MonoBehaviour {
	
	//simple state setup to track status of occlusion
	private enum OcclusionState {
		EnterOcclusion = 0,
		Idle,
		ExitOcclusion
	}
	
	private OcclusionState occlusionState = OcclusionState.Idle;
	
	public List<occlusionCategory> occlusionTypes = new List<occlusionCategory>();
	public float timeToFullOcclusion = 0.6f;
	internal int numTimesOccluded = 0;
	private bool isInitialized = false;
	internal bool isOccluded = false;
	private float timer = 0;
	private float originalVol = 1.0f;
	private float startingVol = 1.0f;
	private float originalFilter = 22000.0f;
	internal float volumePct = 1.0f;
	internal float lpfCutoff = 22000.0f;
	private AudioLowPassFilter lpf;
	
	public void Init() {
		originalVol = GetComponent<AudioSource>().volume;	
		lpf = gameObject.GetComponent<AudioLowPassFilter>();
		if(lpf == null)
			lpf = gameObject.AddComponent<AudioLowPassFilter>();
		lpf.enabled = false;

		isInitialized = true;
	}
	
	public void Update() {
		//ramp down to the occlusion values
		if(occlusionState == OcclusionState.EnterOcclusion) {
			timer -= Time.deltaTime;
			
			float timePct = 1.0f - timer / timeToFullOcclusion;		

			GetComponent<AudioSource>().volume = Mathf.Lerp( startingVol, originalVol * volumePct, timePct );
				
			if(lpf.enabled)
				lpf.cutoffFrequency = Mathf.Lerp(originalFilter, lpfCutoff, timePct);
				//	lpf.cutoffFrequency = (originalFilter * timePct) - originalFilter;
			if(GetComponent<AudioSource>().volume <= originalVol*volumePct) 
				GetComponent<AudioSource>().volume = originalVol*volumePct;
			if(lpf.enabled && lpf.cutoffFrequency <= lpfCutoff)
				lpf.cutoffFrequency = lpfCutoff;

			
			if( timer <= 0.0f )
			{
				occlusionState = OcclusionState.Idle;
			}
		
		}	
		
		if(occlusionState == OcclusionState.Idle) {
			//if(lpf != null && lpf.enabled)
			//	lpf.enabled = false;
		}
		
		//bring the values back to normal
		if(occlusionState == OcclusionState.ExitOcclusion) {
			timer -= Time.deltaTime;
			
			float timePct = 1.0f - timer / timeToFullOcclusion;

			GetComponent<AudioSource>().volume = Mathf.Lerp( startingVol, originalVol, timePct );
			
			if(lpf.enabled)
				lpf.cutoffFrequency = Mathf.Lerp(lpfCutoff, originalFilter, timePct);
			if(GetComponent<AudioSource>().volume >= originalVol)
				GetComponent<AudioSource>().volume = originalVol;
			if(lpf.cutoffFrequency >= originalFilter)
				lpf.cutoffFrequency = originalFilter;

			if( timer <= 0.0f )
			{
				lpf.enabled = false;
				GetComponent<AudioSource>().volume = originalVol;

				occlusionState = OcclusionState.Idle;
			}
		}
	}
	
	public void Occlude(float volPct, float cutoff) {
		if(!isInitialized)
				Init();
		//only occlude if the object is not already occluded
		if(numTimesOccluded == 0) {
			lpf.enabled = true;
			startingVol = GetComponent<AudioSource>().volume;

			lpfCutoff = cutoff;
			volumePct = volPct;

			float delta = originalVol - originalVol * volumePct;		
			float curVolumePct = (startingVol - originalVol * volumePct) / delta;
			
			timer = curVolumePct * timeToFullOcclusion;
			occlusionState = OcclusionState.EnterOcclusion;
		}
		isOccluded = true;
		numTimesOccluded++;		
	}
	
	public void EndOcclude() {
		numTimesOccluded--;
		if(numTimesOccluded == 0) {
			lpf.enabled = true;
			startingVol = GetComponent<AudioSource>().volume;

			float delta = originalVol - originalVol * volumePct;		
			float curVolumePct = (originalVol - startingVol) / delta;
			
			timer = curVolumePct * timeToFullOcclusion;
			isOccluded = false;
			occlusionState = OcclusionState.ExitOcclusion;
		}
	}
		
}
