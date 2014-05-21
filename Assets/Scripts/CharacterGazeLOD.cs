using UnityEngine;
using System.Collections;

public enum TriggerOption
{
	Mouse,
	Gaze
}

public class CharacterGazeLOD : MonoBehaviour
{
	public TriggerOption option;
	public float highLimit = 0.2f;
	public float standardLimit = 0.5f;
	public float maxDistance = 10f;
	public Mesh high;
	public Mesh medium;
	public GameObject impostor;
	public GameObject characterMesh;

	private Transform myTransform;
	private Transform cameraTransform;
	private SkinnedMeshRenderer characterRenderer;
	private LOD currentLOD;
	private Impostor impostorScript;
	
	private enum LOD
	{
		High,
		Standard,
		Low
	}

	public void Awake ()
	{
		myTransform = transform;
		cameraTransform = Camera.main.transform;
		characterRenderer = characterMesh.GetComponent<SkinnedMeshRenderer> ();
		impostorScript = impostor.GetComponent<Impostor> ();
		characterMesh.SetActive (false);
		impostor.SetActive (true);
	}

	public void Update ()
	{
		Vector3 cPos = cameraTransform.position;
		Vector3 tPos = myTransform.position;
		cPos.y = tPos.y = 0;
		float distance = Vector3.Distance (cPos, tPos);
		
		if (distance > maxDistance) {
			SetLOD (LOD.Low);
			return;
		}

		distance = GazeDistance.Instance.CalculateDistance (transform.position);
		
		SetLOD (GetLOD (distance));
	}
	
	private void SetLOD (LOD lod)
	{
		if (currentLOD == lod) {
			return;
		}
		switch (lod) {
		case LOD.High:
			SetImpostor (false);
			SetCharacterMesh (true, high);
			break;
		case LOD.Standard:
			SetImpostor (false);
			SetCharacterMesh (true, medium);
			break;
		case LOD.Low:
			SetImpostor (true);
			SetCharacterMesh (false);
			break;
		}
		currentLOD = lod;
	}

	private LOD GetLOD (float distance)
	{
		if (distance < highLimit) {
			return LOD.High;
		} else if (distance < standardLimit) {
			return LOD.Standard;
		}
		return LOD.Low;
	}


	private void SetImpostor (bool active)
	{
		impostor.SetActive (active);
		if (active) {
			impostorScript.Update ();
		}
	}

	private void SetCharacterMesh (bool active, Mesh mesh = null)
	{
		characterMesh.SetActive (active);
		if (active && mesh) {
			characterRenderer.sharedMesh = mesh;
		}
	}
}
