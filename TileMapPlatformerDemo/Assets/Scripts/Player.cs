using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    // Configs
    [SerializeField] float runspeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Vector2 killedKick = new Vector2(25f, 25f);

    // State
    private bool _isAlive = true;

    // cache
    private Rigidbody2D _playerRigidBody;
    private Animator _playerAnimator;
    private CapsuleCollider2D _playerBodyCollider;
    private BoxCollider2D _playerFeetCollider;
    private float gravityScaleAtStart;

    // constants
    private const string ANIMATION_RUNNING = "Running";
    private const string ANIMATION_CLIMBING = "Climbing";
    private const string ANIMATION_KILLED = "Killed";
    private const string INPUT_AXIS_HORIZONTAL = "Horizontal";
    private const string INPUT_AXIS_VERTICAL = "Vertical";
    private const string INPUT_JUMP = "Jump";
    private const string LAYER_GROUND = "Ground";
    private const string LAYER_LADDER = "Ladder";

    private bool PlayerHasHorizontalSpeed {
        get
        {
            return Mathf.Abs(_playerRigidBody.velocity.x) > Mathf.Epsilon;
        }
    }

    private bool PlayerHasVerticalSpeed
    {
        get
        {
            return Mathf.Abs(_playerRigidBody.velocity.y) > Mathf.Epsilon;
        }
    }

    void Start()
    {
        _playerRigidBody = GetComponent<Rigidbody2D>();
        _playerAnimator = GetComponent<Animator>();
        _playerBodyCollider = GetComponent<CapsuleCollider2D>();
        _playerFeetCollider = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = _playerRigidBody.gravityScale;
    }

    void Update()
    {
        if (!_isAlive)
        {
            return;
        }

        Run();
        Jump();
        FlipSprite();
        ClimbLadder();
        Killed();
    }

    private void Run()
    {
        float controlThrow = CrossPlatformInputManager.GetAxis(INPUT_AXIS_HORIZONTAL);
        Vector2 playerVelocity = new Vector2(controlThrow * runspeed, _playerRigidBody.velocity.y);
        _playerRigidBody.velocity = playerVelocity;
        _playerAnimator.SetBool(ANIMATION_RUNNING, PlayerHasHorizontalSpeed);
    }

    private void Jump()
    {
        if (!IsTouchingLayer(LAYER_GROUND)) { return; }

        if (CrossPlatformInputManager.GetButtonDown(INPUT_JUMP))
        {
            Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
            _playerRigidBody.velocity += jumpVelocityToAdd;
        }
    }

    private void ClimbLadder()
    {
        if (!IsTouchingLayer(LAYER_LADDER)) {
            _playerAnimator.SetBool(ANIMATION_CLIMBING, false);
            _playerRigidBody.gravityScale = gravityScaleAtStart;
            return;
        }

        float controlThrow = CrossPlatformInputManager.GetAxis(INPUT_AXIS_VERTICAL);
        Vector2 climbVelocity = new Vector2(_playerRigidBody.velocity.x, controlThrow * climbSpeed);
        _playerRigidBody.velocity = climbVelocity;
        _playerRigidBody.gravityScale = 0f;

        _playerAnimator.SetBool(ANIMATION_CLIMBING, PlayerHasVerticalSpeed);
    }

    private void Killed()
    {
        if (_playerBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemy", "Hazards")))
        {
            _isAlive = false;
            _playerAnimator.SetTrigger(ANIMATION_KILLED);
            GetComponent<Rigidbody2D>().velocity = killedKick;
        }
    }

    private void FlipSprite()
    {
        if (PlayerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(_playerRigidBody.velocity.x), 1f);
        }
    }

    private bool IsTouchingLayer(string layer)
    {
        return _playerFeetCollider.IsTouchingLayers(LayerMask.GetMask(layer));
    }
}
