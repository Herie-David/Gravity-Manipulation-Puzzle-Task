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

        // Selection Logic (Arrow Keys)
        if (Input.GetKeyDown(KeyCode.UpArrow)) SelectedGravityDir = Vector3.up;
        if (Input.GetKeyDown(KeyCode.DownArrow)) SelectedGravityDir = Vector3.down;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) SelectedGravityDir = Vector3.left;
        if (Input.GetKeyDown(KeyCode.RightArrow)) SelectedGravityDir = Vector3.right;

        // Z-Axis Support for 3D navigation
        if (Input.GetKeyDown(KeyCode.PageUp)) SelectedGravityDir = Vector3.forward;
        if (Input.GetKeyDown(KeyCode.PageDown)) SelectedGravityDir = Vector3.back;

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