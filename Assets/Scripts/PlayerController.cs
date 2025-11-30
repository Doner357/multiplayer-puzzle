using UnityEngine;

// 這是必備組件的屬性，防止你忘記加 Rigidbody
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 5f;
    public float rotateSpeed = 10f;
    public float jumpForce = 5f;

    private Rigidbody rb;
    private Vector3 inputVector;

    // Start 在遊戲開始時執行一次
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update 每幀執行，適合處理 Input
    void Update()
    {
        // 取得鍵盤輸入 (WASD 或 方向鍵)，回傳值為 -1 到 1
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // 建立移動向量 (忽略 Y 軸)
        inputVector = new Vector3(h, 0, v).normalized;

        // 跳躍邏輯 (簡單版：按空白鍵且垂直速度接近 0)
        if (Input.GetKeyDown(KeyCode.Space) && Mathf.Abs(rb.linearVelocity.y) < 0.1f)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    // FixedUpdate 固定頻率執行 (預設 0.02秒)，所有物理運算必須寫在這裡！
    void FixedUpdate()
    {
        MoveLogic();
    }

    void MoveLogic()
    {
        if (inputVector.magnitude > 0.1f)
        {
            // 1. 移動：直接設定速度 (保留原本的 Y 軸速度以維持重力)
            // Unity 6 建議使用 linearVelocity 取代舊版的 velocity
            Vector3 newVelocity = inputVector * moveSpeed;
            rb.linearVelocity = new Vector3(newVelocity.x, rb.linearVelocity.y, newVelocity.z);

            // 2. 轉向：讓角色面向移動方向
            Quaternion targetRotation = Quaternion.LookRotation(inputVector);
            // Slerp 是一個平滑插值函數，讓轉身有過渡動畫
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // 如果沒按鍵，水平速度歸零 (停止滑行)，但保留垂直重力
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }
}