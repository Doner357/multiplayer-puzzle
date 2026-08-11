using Unity.Netcode;
using UnityEngine;
using Unity.Services.Multiplayer;
public class SessionGameStarter : MonoBehaviour
{
    [Header("Settings")]
    public string gameplaySceneName = "GameScene";

    public void OnHostSessionCreated()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsHost)
        {
            Debug.Log("[SessionGameStarter] Host Created Session. Loading Scene...");
            
            NetworkManager.Singleton.SceneManager.LoadScene(
                gameplaySceneName, 
                UnityEngine.SceneManagement.LoadSceneMode.Single
            );
        }
    }

    public void OnClientJoinedSession(ISession session)
    {
        Debug.Log($"[SessionGameStarter] Joined Session: {session.Id}");
    }

    public void OnJoinFailed(SessionException exception)
    {
        Debug.LogError($"[SessionGameStarter] Join Failed: {exception.Message}");
    }
}
