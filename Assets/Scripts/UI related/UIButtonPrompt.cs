using UnityEngine;
using System.Collections;

public class UIButtonPrompt : MonoBehaviour {
    [SerializeField]
    Animator animator;
    [SerializeField]
    float speed;

    // Use this for initialization
    void Start () {
        animator.speed = speed;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
