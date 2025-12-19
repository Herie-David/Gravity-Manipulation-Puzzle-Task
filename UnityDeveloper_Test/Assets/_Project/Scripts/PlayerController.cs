using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(InputReader))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;
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
        Debug.Log($"Current Speed: {controller.velocity.magnitude}");
        Vector3 gravityDir = GravityManager.Instance.GetGravityDirection();
        float rayDistance = (controller.height / 2f) + 0.15f;
        bool isGrounded = Physics.Raycast(transform.position, gravityDir, rayDistance);

        // Calculate Movement Vector
        Vector3 moveDir = (transform.forward * input.MoveInput.y) + (transform.right * input.MoveInput.x);
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

        // Send 'movement.magnitude' to Animator (not controller.velocity)
        if (animator != null)
        {
            // Use the magnitude of our intended WASD movement
            float currentSpeed = movement.magnitude;
            animator.SetFloat("Speed", currentSpeed);
            animator.SetBool("isGrounded", isGrounded);

            // Debug to verify
            // Debug.Log($"Intended Speed: {currentSpeed}");
        }

        // Handle Gravity Execution Intent
        if (input.GravityCommitPressed && input.HasValidGravitySelection)
        {
            GravityManager.Instance.UpdateWorldGravity(input.SelectedGravityDir);
            input.ConsumeGravityCommit();
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