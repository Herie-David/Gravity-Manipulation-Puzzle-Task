using UnityEngine;

public class CollectibleCube : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Ensure the Player object has the "Player" Tag
        if (other.CompareTag("Player"))
        {
            GameSessionManager.Instance.CollectCube();
            Destroy(gameObject); // Remove cube from world
        }
    }
}