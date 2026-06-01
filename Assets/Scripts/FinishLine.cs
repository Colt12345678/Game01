using UnityEngine;

public class FinishLine : MonoBehaviour
{
    [Header("Victory Settings")]
    [Tooltip("Create an empty object inside a cool room and drag it here!")]
    public Transform winnersRoom;

    private void OnTriggerEnter(Collider other)
    {
        // Check if it's the player crossing the line
        if (other.CompareTag("Player"))
        {
            Debug.Log("🎉 YOU WIN! Course Completed! 🎉");

            // Teleport them to the Winner's Room
            if (winnersRoom != null)
            {
                CharacterController cc = other.GetComponent<CharacterController>();
                
                if (cc != null)
                {
                    // Turn off physics, teleport, turn it back on!
                    cc.enabled = false;
                    other.transform.position = winnersRoom.position;
                    cc.enabled = true;
                }
            }
        }
    }
}