using UnityEngine;
using System.Collections;

public enum TriggerOption
{
	Mouse,
	Gaze
}

public class CharacterGazeLOD : MonoBehaviour
{
	public float NormalizedTime { get; set; }

	public TriggerOption option;
	public EyeXGazePointType gazePointType = EyeXGazePointType.GazeLightlyFiltered;
	public float highLimit = 50f; // TODO
	public float standardLimit = 180f; // TODO
	public Mesh high;
	public Mesh medium;
	public GameObject impostor;
	public GameObject characterMesh;
	public float coolDownTime = 0.5f;
	private Transform myTransform;
	private Transform cameraTransform;
	private SkinnedMeshRenderer characterRenderer;
	private LOD currentLOD;
	private Impostor impostorScript;
	private float coolDown;
	private Animator animator;

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
		animator = characterMesh.GetComponent<Animator> ();
		characterMesh.SetActive (false);
		impostor.SetActive (true);
	}

	public void Update ()
	{
		float distance = float.MaxValue;
		NormalizedTime += Time.deltaTime;
		NormalizedTime = NormalizedTime % 1.0f;

		if (impostor.activeSelf) {
			distance = GazeDistance.Instance.CalculateDistance (impostor);
		} else {
			distance = GazeDistance.Instance.CalculateDistance (characterMesh);
		}

		if (!CoolDown ()) {
			SetLOD (GetLOD (distance));
		}
	}

	private void SetLOD (LOD lod)
	{
		if (currentLOD == lod) {
			return;
		}

		SetCoolDown ();

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
			SetCharacterMesh (false);
			SetImpostor (true);
			break;
		}

		currentLOD = lod;
	}

	private void SetCoolDown ()
	{
		coolDown = coolDownTime;
	}

	private bool CoolDown ()
	{
		return (coolDown -= Time.deltaTime) > 0;
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

	private void SetImpostor (bool activate)
	{
		impostor.SetActive (activate);
		if (activate) {
			impostorScript.Update ();
		}
	}

	private void SetCharacterMesh (bool activate, Mesh mesh = null)
	{
		if (!activate) {
			NormalizedTime = animator.GetCurrentAnimatorStateInfo (0).normalizedTime - 
				Mathf.Floor (animator.GetCurrentAnimatorStateInfo (0).normalizedTime);
		}
		characterMesh.SetActive (activate);
		if (activate && mesh) {   
			//animator.speed = 0;
			characterRenderer.sharedMesh = mesh;
			//float normalizedTimeFrac = NormalizedTime - Mathf.Floor (NormalizedTime);
			animator.Play ("WalkForward", 0, NormalizedTime);
		}
	}
}
