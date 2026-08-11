/*
 *
 * This Boostrap class is intended to serve as the initial entry point for the Unity application,
 * the Netwrok Manager will be initialized in this scene.
 *
 */
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Boostrap : MonoBehaviour
{
    [Header("Settings")]
    public string mainMenuSceneName = "MainMenu";

    void Start()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager is not present in the scene. Please add a NetworkManager prefab.");
            return;
        }               
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
