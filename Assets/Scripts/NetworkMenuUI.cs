using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class NetworkMenuUI : MonoBehaviour
{
    [Header("UI References")]
    public Button hostButton;
    public Button clientButton;
    public TMP_InputField ipInputField;

    [Header("Settings")]
    public string gameplaySceneName = "GameScene";

    void Start()
    {
        hostButton.onClick.AddListener(StartHost);
        clientButton.onClick.AddListener(StartClient);
    }

    void StartHost()
    {
        NetworkManager.Singleton.StartHost();

        NetworkManager.Singleton.SceneManager.LoadScene(gameplaySceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    void StartClient()
    {
        string ipText = ipInputField.text;
        if (string.IsNullOrEmpty(ipText)) ipText = "127.0.0.1";

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.ConnectionData.Address = ipText;

        NetworkManager.Singleton.StartClient();
    }
}
