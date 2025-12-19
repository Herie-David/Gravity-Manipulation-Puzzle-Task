using UnityEngine;
using System;

/// <summary>
/// Centralized manager for handling world gravity changes.
/// Uses the Observer pattern to notify entities when gravity shifts.
/// </summary>
public class GravityManager : MonoBehaviour
{
    public static GravityManager Instance { get; private set; }

    // Event for other scripts (Player/Camera) to subscribe to
    public static event Action<Vector3> OnGravityChanged;

    [Header("Settings")]
    [SerializeField] private float gravityMagnitude = 9.81f;

    private Vector3 currentGravityDirection = Vector3.down;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Initialize default gravity
        UpdateWorldGravity(Vector3.down);
    }

    /// <summary>
    /// Updates the global physics gravity and notifies subscribers.
    /// </summary>
    /// <param name="newDirection">The normalized direction of the new gravity.</param>
    public void UpdateWorldGravity(Vector3 newDirection)
    {
        if (newDirection == Vector3.zero)
        {
            Debug.LogWarning("Gravity direction cannot be zero.");
            return;
        }

        Physics.gravity = newDirection.normalized * gravityMagnitude;
        OnGravityChanged?.Invoke(newDirection.normalized);
    }
    public Vector3 GetGravityDirection() => Physics.gravity.normalized;

    private void OnDestroy()
    {
        // Clear all subscribers when the manager is destroyed (on scene reload)
        OnGravityChanged = null;
    }
}