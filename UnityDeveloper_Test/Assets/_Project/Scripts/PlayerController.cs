using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(InputReader))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;
    private float startGracePeriod = 2.0f; // Give the game 1 second to stabilize
    [SerializeField] private Animator animator;

    private CharacterController controller;
    private InputReader input;
    private Vector3 velocity;
    private Coroutine alignRoutine;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        input = GetComponent<InputReader>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    // Subscribe/Unsubscribe during lifecycle to prevent leaks
    private void OnEnable() => GravityManager.OnGravityChanged += HandleGravityChange;
    private void OnDisable() => GravityManager.OnGravityChanged -= HandleGravityChange;

    private void Update()
    {
        // Stop all logic if the game has ended or is restarting
        if (GameSessionManager.Instance != null && !GameSessionManager.Instance.IsGameActive)
        {
            return;
        }

        Vector3 gravityDir = GravityManager.Instance.GetGravityDirection();
        float rayDistance = (controller.height / 2f) + 0.15f;
        bool isGrounded = Physics.Raycast(transform.position, gravityDir, rayDistance);

        // Calculate Movement Vector
        Vector3 moveDir = (transform.forward * input.MoveInput.y) +
                          (transform.right * input.MoveInput.x);
        Vector3 movement = moveDir * moveSpeed;

        // Handle Jumping
        if (input.JumpPressed && isGrounded)
        {
            velocity = transform.up * jumpForce;
        }

        // Gravity Accumulation
        if (isGrounded && Vector3.Dot(velocity, gravityDir) > 0)
        {
            velocity = gravityDir * 2f; // Stick to ground
        }
        else
        {
            velocity += Physics.gravity * Time.deltaTime;
        }

        // Combine movement and gravity into ONE Move call
        Vector3 finalMotion = movement + velocity;
        controller.Move(finalMotion * Time.deltaTime);

        // Animator
        if (animator != null)
        {
            float currentSpeed = movement.magnitude;
            animator.SetFloat("Speed", currentSpeed);
            animator.SetBool("isGrounded", isGrounded);
        }

        // Gravity Execution Intent
        if (input.GravityCommitPressed && input.HasValidGravitySelection)
        {
            GravityManager.Instance.UpdateWorldGravity(input.SelectedGravityDir);
            input.ConsumeGravityCommit();
        }

        // Void fall grace period
        if (startGracePeriod > 0)
        {
            startGracePeriod -= Time.deltaTime;
            return;
        }

        CheckForFreeFall();
    }


    private void CheckForFreeFall()
    {
        Vector3 gravityDir = GravityManager.Instance.GetGravityDirection();

        // Raycast: if no ground is found within 30 units in the direction of gravity
        if (!Physics.Raycast(transform.position, gravityDir, 30f))
        {
            GameSessionManager.Instance.EndGame("YOU FELL INTO THE VOID!");
        }
    }

    private void HandleGravityChange(Vector3 newDir)
    {
        velocity = Vector3.zero; // Prevent physics slingshotting

        // Coroutine Management: Stop any existing rotation to prevent stacking bugs
        if (alignRoutine != null) StopCoroutine(alignRoutine);
        alignRoutine = StartCoroutine(AlignToGravity(newDir));
    }

    private System.Collections.IEnumerator AlignToGravity(Vector3 gravityDir)
    {
        Quaternion targetRot = Quaternion.FromToRotation(transform.up, -gravityDir) * transform.rotation;
        float elapsed = 0;
        float duration = 0.5f;

        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = targetRot;
    }
}