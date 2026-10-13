using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementMobile : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public DynamicJoystick joystick; // drag your DynamicJoystick here

    private Rigidbody2D rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Vector2 moveInput = new Vector2(joystick.Horizontal, joystick.Vertical);

        rb.linearVelocity = moveInput * moveSpeed;

        bool isWalking = moveInput.magnitude > 0.1f;
        animator.SetBool("isWalking", isWalking);

        if (!isWalking)
        {
            animator.SetFloat("LastInputX", moveInput.x);
            animator.SetFloat("LastInputY", moveInput.y);
        }
        else
        {
            animator.SetFloat("InputX", moveInput.x);
            animator.SetFloat("InputY", moveInput.y);
        }
    }
}
