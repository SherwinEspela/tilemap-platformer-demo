using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    // Configs
    [SerializeField] float runspeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;

    // State
    private bool isAlive = true;

    private Rigidbody2D playerRigidBody;
    private Animator playerAnimator;
    private CapsuleCollider2D _playerBodyCollider;
    private BoxCollider2D _playerFeetCollider;
    private float gravityScaleAtStart;

    private const string ANIMATION_RUNNING = "Running";
    private const string ANIMATION_CLIMBING = "Climbing";
    private const string INPUT_AXIS_HORIZONTAL = "Horizontal";
    private const string INPUT_AXIS_VERTICAL = "Vertical";
    private const string INPUT_JUMP = "Jump";
    private const string LAYER_GROUND = "Ground";
    private const string LAYER_LADDER = "Ladder";

    private bool PlayerHasHorizontalSpeed {
        get
        {
            return Mathf.Abs(playerRigidBody.velocity.x) > Mathf.Epsilon;
        }
    }

    private bool PlayerHasVerticalSpeed
    {
        get
        {
            return Mathf.Abs(playerRigidBody.velocity.y) > Mathf.Epsilon;
        }
    }

    void Start()
    {
        playerRigidBody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        _playerBodyCollider = GetComponent<CapsuleCollider2D>();
        _playerFeetCollider = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = playerRigidBody.gravityScale;
    }

    void Update()
    {
        Run();
        Jump();
        FlipSprite();
        ClimbLadder();
    }

    private void Run()
    {
        float controlThrow = CrossPlatformInputManager.GetAxis(INPUT_AXIS_HORIZONTAL);
        Vector2 playerVelocity = new Vector2(controlThrow * runspeed, playerRigidBody.velocity.y);
        playerRigidBody.velocity = playerVelocity;
        playerAnimator.SetBool(ANIMATION_RUNNING, PlayerHasHorizontalSpeed);
    }

    private void Jump()
    {
        if (!IsTouchingLayer(LAYER_GROUND)) { return; }

        if (CrossPlatformInputManager.GetButtonDown(INPUT_JUMP))
        {
            Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
            playerRigidBody.velocity += jumpVelocityToAdd;
        }
    }

    private void ClimbLadder()
    {
        if (!IsTouchingLayer(LAYER_LADDER)) {
            playerAnimator.SetBool(ANIMATION_CLIMBING, false);
            playerRigidBody.gravityScale = gravityScaleAtStart;
            return;
        }

        float controlThrow = CrossPlatformInputManager.GetAxis(INPUT_AXIS_VERTICAL);
        Vector2 climbVelocity = new Vector2(playerRigidBody.velocity.x, controlThrow * climbSpeed);
        playerRigidBody.velocity = climbVelocity;
        playerRigidBody.gravityScale = 0f;

        playerAnimator.SetBool(ANIMATION_CLIMBING, PlayerHasVerticalSpeed);
    }

    private void FlipSprite()
    {
        if (PlayerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(playerRigidBody.velocity.x), 1f);
        }
    }

    private bool IsTouchingLayer(string layer)
    {
        return _playerFeetCollider.IsTouchingLayers(LayerMask.GetMask(layer));
    }
}
