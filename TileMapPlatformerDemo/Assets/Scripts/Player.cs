using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    // Configs
    [SerializeField] float runspeed = 5f;
    [SerializeField] float jumpSpeed = 5f;

    // State
    private bool isAlive = true;

    private Rigidbody2D playerRigidBody;
    private Animator playerAnimator;
    private Collider2D playerCollider2d;

    private const string ANIMATION_RUNNING = "Running";
    private const string HORIZONTAL_AXIS = "Horizontal";
    private const string INPUT_JUMP = "Jump";
    private const string LAYER_GROUND = "Ground";

    private bool PlayerHasHorizontalSpeed {
        get
        {
            return Mathf.Abs(playerRigidBody.velocity.x) > Mathf.Epsilon;
        }
    }

    private bool IsNotTouchingGround { get { return !playerCollider2d.IsTouchingLayers(LayerMask.GetMask(LAYER_GROUND)); } }

    void Start()
    {
        playerRigidBody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        playerCollider2d = GetComponent<Collider2D>();
    }

    void Update()
    {
        Run();
        Jump();
        FlipSprite();
    }

    private void Run()
    {
        float controlThrow = CrossPlatformInputManager.GetAxis(HORIZONTAL_AXIS);
        Vector2 playerVelocity = new Vector2(controlThrow * runspeed, playerRigidBody.velocity.y);
        playerRigidBody.velocity = playerVelocity;
        playerAnimator.SetBool(ANIMATION_RUNNING, PlayerHasHorizontalSpeed);
    }

    private void Jump()
    {
        if (IsNotTouchingGround) { return; }

        if (CrossPlatformInputManager.GetButtonDown(INPUT_JUMP))
        {
            Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
            playerRigidBody.velocity += jumpVelocityToAdd;
        }
    }

    private void FlipSprite()
    {
        if (PlayerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(playerRigidBody.velocity.x), 1f);
        }
    }
}
