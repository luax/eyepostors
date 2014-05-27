using UnityEngine;
using System.Collections;

public class StartMusicOnLoaded : MonoBehaviour {

	void Update () {
		if (!audio.isPlaying) {
			audio.Play ();
		}
	}
}
