using UnityEngine;

public class CameraToggler : MonoBehaviour
{
    [Header("Cameras")]
    public GameObject firstPersonCam;  // Drag your original Main Camera here
    public GameObject thirdPersonCam; // Drag your new ThirdPersonCamera here

    [Header("Toggle Key")]
    public KeyCode toggleKey = KeyCode.C; // Press 'C' to switch cameras!

    private bool isThirdPerson = false;

    private void Start()
    {
        // Start the game with the normal camera ON and third person OFF
        if (firstPersonCam != null) firstPersonCam.SetActive(true);
        if (thirdPersonCam != null) thirdPersonCam.SetActive(false);
    }

    private void Update()
    {
        // Check if the player pressed the switch key
        if (Input.GetKeyDown(toggleKey))
        {
            isThirdPerson = !isThirdPerson; // Flip the switch

            if (isThirdPerson)
            {
                firstPersonCam.SetActive(false);
                thirdPersonCam.SetActive(true);
            }
            else
            {
                firstPersonCam.SetActive(true);
                thirdPersonCam.SetActive(false);
            }
        }
    }
}