using UnityEngine;

public class Hazard : MonoBehaviour
{
    [Header("Respawn Settings")]
    [Tooltip("Drag the object you want the player to teleport back to here.")]
    [SerializeField] private Transform respawnPoint;

    // This runs automatically when something touches the trigger collider
    private void OnTriggerEnter(Collider other)
    {
        // 1. Check if the object that touched the stone is tagged as "Player"
        if (other.CompareTag("Player"))
        {
            // 2. Get the Character Controller component from the player
            CharacterController cc = other.GetComponent<CharacterController>();

            if (cc != null)
            {
                // 3. Turn off the controller, move the player, then turn it back on!
                cc.enabled = false;
                other.transform.position = respawnPoint.position;
                cc.enabled = true;
                
                Debug.Log("Player hit the Death Stone! Respawning...");
            }
        }
    }
}