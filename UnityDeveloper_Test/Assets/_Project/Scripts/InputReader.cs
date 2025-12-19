using UnityEngine;

public class InputReader : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public bool JumpPressed { get; private set; }
    public Vector3 SelectedGravityDir { get; private set; }
    public bool GravityCommitPressed { get; private set; }

    public bool HasValidGravitySelection => SelectedGravityDir != Vector3.zero;

    private void Update()
    {
        MoveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        JumpPressed = Input.GetKeyDown(KeyCode.Space);

        // Inside InputReader.cs Update()
        if (Input.GetKeyDown(KeyCode.UpArrow)) SelectedGravityDir = transform.up;     // Current "Ceiling"
        if (Input.GetKeyDown(KeyCode.DownArrow)) SelectedGravityDir = -transform.up;    // Current "Floor"
        if (Input.GetKeyDown(KeyCode.LeftArrow)) SelectedGravityDir = -transform.right; // Current "Left Wall"
        if (Input.GetKeyDown(KeyCode.RightArrow)) SelectedGravityDir = transform.right;  // Current "Right Wall"

        // Z-axis relative to where the player is looking
        if (Input.GetKeyDown(KeyCode.PageUp)) SelectedGravityDir = transform.forward;
        if (Input.GetKeyDown(KeyCode.PageDown)) SelectedGravityDir = -transform.forward;

        GravityCommitPressed = Input.GetKeyDown(KeyCode.Return);
    }

    /// <summary>
    /// Clears commitment flags once the action has been performed.
    /// </summary>
    public void ConsumeGravityCommit()
    {
        GravityCommitPressed = false;
        SelectedGravityDir = Vector3.zero;
    }
}