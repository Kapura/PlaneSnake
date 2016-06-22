using UnityEngine;
using System.Collections;

public class FrameScript : MonoBehaviour {

    public GameObject[] windows;

	// Use this for initialization
	void Awake () {
        foreach (GameObject obj in windows) {
            obj.GetComponent<Renderer>().material.renderQueue = 2501;
        }
	}
}
