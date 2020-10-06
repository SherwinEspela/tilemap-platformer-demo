using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1f;
    Rigidbody2D _rigidBody;

    bool IsFacingRight
    {
        get { return transform.localScale.x > 0; }
    }

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        _rigidBody.velocity = new Vector2(IsFacingRight ? moveSpeed : -moveSpeed, 0f);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        transform.localScale = new Vector2(-(Mathf.Sign(_rigidBody.velocity.x)), 1f);
    }
}
