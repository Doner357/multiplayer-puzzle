using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP; // 引用 Unity Transport
using UnityEngine;
using UnityEngine.UI; // 如果用 TMP，請改用 TMPro

public class NetworkMenuUI : MonoBehaviour
{
    [Header("UI References")]
    public Button hostButton;
    public Button clientButton;
    public TMP_InputField ipInputField; // 如果用 TMP，改為 TMP_InputField

    [Header("Settings")]
    public string gameplaySceneName = "GameScene"; // 記得改成你的遊戲場景名稱

    void Start()
    {
        // 綁定按鈕事件
        hostButton.onClick.AddListener(StartHost);
        clientButton.onClick.AddListener(StartClient);
    }

    void StartHost()
    {
        // 1. 啟動 Host (同時是 Server 也是 Client)
        NetworkManager.Singleton.StartHost();

        // 2. 【關鍵】由 Server 命令切換場景
        // NetworkManager 自帶的 SceneManager 會自動把所有連進來的 Client 一起拉過去
        NetworkManager.Singleton.SceneManager.LoadScene(gameplaySceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    void StartClient()
    {
        // 1. 設定目標 IP (從輸入框讀取)
        string ipText = ipInputField.text;
        if (string.IsNullOrEmpty(ipText)) ipText = "127.0.0.1"; // 防呆

        // 獲取 Transport 組件來修改 IP
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.ConnectionData.Address = ipText;

        // 2. 啟動 Client
        // Client 不需要自己 LoadScene，因為 Server 會自動告訴 Client 該去哪個場景 (Scene Synchronization)
        NetworkManager.Singleton.StartClient();
    }
}
