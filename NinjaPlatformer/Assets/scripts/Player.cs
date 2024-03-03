using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 10f;
    [SerializeField] int maxJumps = 2; // Maximum number of jumps allowed
    [SerializeField] float coyoteTime = 0.1f;
    [SerializeField] float maxFallSpeed = -10f;
    [SerializeField] float groundCheckRadius = 0.1f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] string tagToCheck = "Dash"; // Tag to check for
    [SerializeField] float checkRadius = 5f; // Radius to check within
    [SerializeField] float dashForce = 20f; // Dash force
    [SerializeField] float dashDur = .5f;
    [SerializeField] float gravityAfterDash = 2;
    [SerializeField] float afterDashJumpForce = 5f;
    [SerializeField] GameObject deathParticle;

    private Camera mainCam;
    private Rigidbody2D rb;
    private bool isGrounded = false;
    private float lastGroundedTime;
    private float lastJumpTime = -10f; // Set to a negative value to indicate no recent jump
    private bool isJumping = false;
    private int remainingJumps; // Number of jumps remaining
    private bool isDashing = false;
    private float startingGravity;
    private float origionalJumpForce;
    private Vector2 startingPos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        remainingJumps = maxJumps;
        startingGravity = rb.gravityScale;
        origionalJumpForce = jumpForce;
        mainCam = Camera.main;
        startingPos = transform.position;
    }

    private void Update()
    {
        HandleMovement();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryJump();
        }
        if (Input.GetMouseButtonDown(0) && !isDashing)
        {
            DashToClosestObject();
        }
        if (isDashing && rb.velocity.magnitude < 0.1f)
        {
            //isDashing = false;
        }
        if(transform.position.y < -10)
        {
            transform.position = startingPos;
        }
    }

    private void FixedUpdate()
    {
        ApplyGravity();
        CheckGrounded();
    }

    private void HandleMovement()
    {
        if (!isDashing) // Check if the player is not dashing
        {
            float moveInput = Input.GetAxisRaw("Horizontal");
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        }
    }

    private void TryJump()
    {
        // Check if the player is grounded or within the coyote time
        if (remainingJumps > 0 || (isGrounded || Time.time - lastGroundedTime < coyoteTime))
        {
            Jump();
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        isGrounded = false;
        isJumping = true;
        lastJumpTime = Time.time;
        remainingJumps--;
    }

    private void ApplyGravity()
    {
        if (!isGrounded && !isJumping && rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * Time.deltaTime;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, maxFallSpeed, Mathf.Infinity));
        }
    }

    private void CheckGrounded()
    {
        Collider2D characterCollider = GetComponent<Collider2D>();
        Vector2 circleCenter = new Vector2(transform.position.x, characterCollider.bounds.min.y);
        Vector2 castDirection = Vector2.down;

        // Perform a circle cast downwards
        RaycastHit2D hit = Physics2D.CircleCast(circleCenter, groundCheckRadius, castDirection, 0.01f, groundLayer);

        // Check if the circle cast hit anything
        isGrounded = hit.collider != null;

        if (isGrounded)
        {
            lastGroundedTime = Time.time;
            isJumping = false;
            remainingJumps = maxJumps;
            rb.gravityScale = startingGravity;
            jumpForce = origionalJumpForce;
        }
    }

    private GameObject GetClosestObjectWithTag(string tag)
    {
        GameObject closestObject = null;
        float closestDistance = Mathf.Infinity;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, checkRadius);

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag(tag))
            {
                float distanceToCollider = Vector2.Distance(transform.position, collider.transform.position);
                if (distanceToCollider < closestDistance)
                {
                    closestDistance = distanceToCollider;
                    closestObject = collider.gameObject;
                }
            }
        }

        return closestObject;
    }

    private void DashToClosestObject()
    {
        GameObject closestObject = GetClosestObjectWithTag(tagToCheck);
        if (closestObject != null)
        {
            Vector2 targetPosition = closestObject.transform.position;

            // Set the flag to indicate that the player is dashing
            isDashing = true;

            StartCoroutine(DashCoroutine(targetPosition, closestObject));
        }
    }

    private IEnumerator DashCoroutine(Vector2 targetPosition, GameObject closestObject)
    {
        float elapsedTime = 0f;
        Vector2 startingPosition = transform.position;

        while (elapsedTime < dashDur)
        {
            // Calculate the interpolated position between starting and target positions
            Vector2 newPosition = Vector2.Lerp(startingPosition, targetPosition, elapsedTime / dashDur);

            // Move the player to the new position
            rb.MovePosition(newPosition);

            // Update elapsed time
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Ensure the player ends up exactly at the target position
        rb.MovePosition(targetPosition);
        rb.velocity = Vector3.zero;

        Screenshake screenShake = mainCam.GetComponent<Screenshake>();
        // Trigger screen shake if the ScreenShake component exists
        if (screenShake != null)
        {
            screenShake.Shake();
        }

        Instantiate(deathParticle, closestObject.transform.position, Quaternion.identity);
        closestObject.GetComponent<Target>().StarDisableHelper();
        remainingJumps = maxJumps;
        rb.gravityScale = gravityAfterDash;
        jumpForce = afterDashJumpForce;
        // Reset the dashing flag
        isDashing = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("newZone"))
        {
            mainCam.GetComponent<Screenshake>().SetTargetPos(collision.transform.GetChild(0).transform);
        }
    }

}
