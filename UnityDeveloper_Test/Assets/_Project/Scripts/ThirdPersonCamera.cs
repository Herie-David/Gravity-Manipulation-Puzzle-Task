using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 2, -4);
    [SerializeField] private float smoothSpeed = 10f;

    [Header("Collision Settings")]
    [SerializeField] private float cameraRadius = 0.2f;
    [SerializeField] private LayerMask obstacleLayers; // Set this to 'Ground' or 'Default'

    private void LateUpdate()
    {
        if (target == null) return;

        // alculate the ideal camera position relative to player orientation
        Vector3 desiredPosition = target.TransformPoint(offset);

        // Check for collisions between the player and the desired position
        Vector3 direction = (desiredPosition - target.position).normalized;
        float distance = Vector3.Distance(target.position, desiredPosition);

        // Raycast to find if a wall is in between the player and camera
        if (Physics.SphereCast(target.position, cameraRadius, direction, out RaycastHit hit, distance, obstacleLayers))
        {
            // If we hit something, move the camera to the hit point (with a tiny offset)
            desiredPosition = hit.point + (hit.normal * cameraRadius);
        }

        // Smoothly interpolate position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Always look at the player, aligning camera 'Up' with player 'Up'
        transform.LookAt(target.position, target.up);
    }
}