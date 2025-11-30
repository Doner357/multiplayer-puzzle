using UnityEngine;

public class GrabSystem : MonoBehaviour
{
    [Header("Settings")]
    public float grabRange = 1.5f; // 抓取距離
    public KeyCode grabKey = KeyCode.E; // 抓取按鍵
    public Transform holdPoint; // 抓取點 (手的位置)

    [Header("Debug")]
    public Rigidbody grabbedObject; // 目前抓到的東西
    private FixedJoint joint; // 連接關節

    void Update()
    {
        // 按下抓取鍵
        if (Input.GetKeyDown(grabKey))
        {
            TryGrab();
        }
        // 放開抓取鍵
        else if (Input.GetKeyUp(grabKey))
        {
            Release();
        }
    }

    void TryGrab()
    {
        // 1. 發射射線 (Raycast) 偵測前方
        // origin: 身體中心, direction: 面向前方, maxDistance: 抓取距離
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, grabRange))
        {
            // 2. 檢查打到的東西有沒有 Rigidbody (代表是物理物件)
            Rigidbody targetRb = hit.collider.GetComponent<Rigidbody>();

            if (targetRb != null && !targetRb.isKinematic)
            {
                // 抓到了！
                grabbedObject = targetRb;
                CreateJoint(targetRb);
            }
        }
    }

    void CreateJoint(Rigidbody targetRb)
    {
        // 在「自己身上」新增一個 FixedJoint
        joint = gameObject.AddComponent<FixedJoint>();

        // 設定連接對象
        joint.connectedBody = targetRb;

        // 設定斷裂力 (可選：拉力太大會斷開)
        joint.breakForce = Mathf.Infinity;
    }

    void Release()
    {
        if (joint != null)
        {
            // 銷毀關節組件，斷開連結
            Destroy(joint);
            joint = null;
            grabbedObject = null;
        }
    }

    // 在編輯器中畫出輔助線，方便調整距離
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * grabRange);
    }
}