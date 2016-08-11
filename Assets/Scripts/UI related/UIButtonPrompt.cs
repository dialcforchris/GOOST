using UnityEngine;
using System.Collections;

public class UIButtonPrompt : MonoBehaviour {
    [SerializeField]
    Animator animator;
    [SerializeField]
    float speed=1;
    [SerializeField]
    ButtonColor ButtonColour;
    public enum ButtonColor
    {
        Red,
        White,
    }

    // Use this for initialization
    void OnEnable ()
    {
        if (!animator)
            animator = GetComponent<Animator>();
        if (ButtonColour == ButtonColor.White)
            animator.Play("button_press_white");
        animator.speed = speed;
    }
}
