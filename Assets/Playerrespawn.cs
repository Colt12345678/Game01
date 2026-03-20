using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [Header("Respawn Settings")]
    [Tooltip("The location the player will teleport to when they die.")]
    public Transform currentRespawnPoint;

    // We call this method when the player touches a hazard
    public void Die()
    {
        if (currentRespawnPoint != null)
        {
            CharacterController cc = GetComponent<CharacterController>();
            
            // Turn off controller, teleport, turn back on
            cc.enabled = false;
            transform.position = currentRespawnPoint.position;
            cc.enabled = true;
            
            Debug.Log("Player Respawned!");
        }
    }
}