using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private PlayerInput localPlayerInput;
    private CinemachineInputAxisController cameraInputController;
    private bool isPaused;

    void Start()
    {
        pausePanel.SetActive(false);
        isPaused = false;
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        if (isPaused)
            Resume();
        else
            Pause();
    }

    private void Pause()
    {
        isPaused = true;
        pausePanel.SetActive(true);

        if (localPlayerInput == null)
            localPlayerInput = FindLocalPlayerInput();

        if (localPlayerInput != null)
            localPlayerInput.enabled = false;

        // Disable camera input
        if (cameraInputController == null)
            cameraInputController = FindAnyObjectByType<CinemachineInputAxisController>();

        if (cameraInputController != null)
            cameraInputController.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        isPaused = false;
        pausePanel.SetActive(false);

        if (localPlayerInput != null)
            localPlayerInput.enabled = true;

        // Re-enable camera input
        if (cameraInputController != null)
            cameraInputController.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ExitToMenu()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.Shutdown();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(mainMenuSceneName);
    }

    private PlayerInput FindLocalPlayerInput()
    {
        foreach (var obj in FindObjectsByType<NetworkObject>(FindObjectsSortMode.None))
        {
            if (obj.IsOwner)
            {
                var input = obj.GetComponent<PlayerInput>();
                if (input != null)
                    return input;
            }
        }
        return null;
    }
}
