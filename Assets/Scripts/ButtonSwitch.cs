using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSwitch : MonoBehaviour
{
    private Animator animator;
    public Animator targetAnimator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Stone") || collision.gameObject.CompareTag("Player"))
        {
            OnPress();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Stone") || collision.gameObject.CompareTag("Player"))
        {
            OnExit();
        }
    }

    private void OnPress()
    {
        animator.SetBool("isPressed", true);
        targetAnimator.SetBool("isOpened", false);
    }

    private void OnExit()
    {
        animator.SetBool("isPressed", false);
        targetAnimator.SetBool("isOpened", true);
    }
}
