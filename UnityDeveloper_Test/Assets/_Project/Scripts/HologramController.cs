using UnityEngine;

public class HologramController : MonoBehaviour
{
    [SerializeField] private GameObject hologramModel; // Assign 'Exo Gray_Holo'
    [SerializeField] private InputReader inputReader;

    private void Awake()
    {
        if (hologramModel == null || inputReader == null)
            Debug.LogError($"{gameObject.name}: HologramController is missing serialized references.");
    }

    private void Start()
    {
        if (hologramModel != null) hologramModel.SetActive(false);
    }

    private void Update()
    {
        // Visual Preview logic
        if (inputReader.HasValidGravitySelection)
        {
            UpdateHologram(inputReader.SelectedGravityDir);
        }
        else
        {
            hologramModel?.SetActive(false);
        }
    }

    private void UpdateHologram(Vector3 dir)
    {
        if (hologramModel == null) return;

        hologramModel.SetActive(true);
        hologramModel.transform.position = transform.position;

        // Preview Rotation (Up is opposite of target gravity)
        Quaternion targetRot = Quaternion.FromToRotation(Vector3.up, -dir);
        hologramModel.transform.rotation = targetRot;
    }
}