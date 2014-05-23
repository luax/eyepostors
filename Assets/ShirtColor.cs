using UnityEngine;
using System.Collections;

public class ShirtColor : MonoBehaviour
{

	public Color shirtColor;
	public GameObject characterMesh;
	public GameObject impostor;
	
	
	// Use this for initialization
	void Start ()
	{
		shirtColor = new Color (Random.Range (0f, 1f), Random.Range (0f, 1f), Random.Range (0f, 1f), 1f);
		characterMesh.GetComponent<SkinnedMeshRenderer> ().material.SetColor ("_ShirtColor", shirtColor);
		impostor.GetComponent<Impostor> ().shirtColor = shirtColor;
	}
}
