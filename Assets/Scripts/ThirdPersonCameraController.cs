using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float zoomSpeed     = 1.0f;
    [SerializeField] private float zoomLerpSpeed = 10f;
    [SerializeField] private float minDistance   = 3.0f;
    [SerializeField] private float maxDistance   = 15.0f;

    private CinemachineCamera cam;
    private CinemachineOrbitalFollow orbital;
    private Vector2 scrollDelta;

    private float targetZoom;
    private float currentZoom;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        cam = GetComponent<CinemachineCamera>();
        orbital = cam.GetComponent<CinemachineOrbitalFollow>();

        targetZoom = currentZoom = orbital.Radius;
    }

    public void MouseZoom(InputAction.CallbackContext context)
    {
        if (scrollDelta.y == 0.0f)
        {
            scrollDelta = context.ReadValue<Vector2>();
        }
        Debug.Log($"Mouse is scrolling. Value: {scrollDelta}");
    }

    public void GamepadZoom(InputAction.CallbackContext context)
    {
        if (scrollDelta.y == 0.0f)
        {
            scrollDelta.y = context.ReadValue<float>();
        }
        Debug.Log($"Gamepad is scrolling. Value: {scrollDelta}");
    }

    // Update is called once per frame
    void Update()
    {
        if (scrollDelta.y != 0)
        {
            if (orbital != null)
            {
                targetZoom = Mathf.Clamp(orbital.Radius - scrollDelta.y * zoomSpeed, minDistance, maxDistance);
                scrollDelta = Vector2.zero;
            }
        }

        currentZoom = Mathf.Lerp(currentZoom, targetZoom, Time.deltaTime * zoomLerpSpeed);
        orbital.Radius = currentZoom;
    }
}
