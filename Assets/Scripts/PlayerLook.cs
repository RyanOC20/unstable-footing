using PurrNet;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : NetworkBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float mouseSensitivity = 200f;

    private float _xRotation;

    protected override void OnSpawned()
    {
        base.OnSpawned();

        // Only the local player should see through their own camera
        // and hear audio from their position. Disable both on remote copies.
        var cam = cameraTransform.GetComponent<Camera>();
        var listener = cameraTransform.GetComponent<AudioListener>();

        if (cam != null) cam.enabled = isOwner;
        if (listener != null) listener.enabled = isOwner;

        if (isOwner)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        if (!isOwner) return;

        var mouse = Mouse.current;
        if (mouse == null) return;

        float mouseX = mouse.delta.x.ReadValue() * mouseSensitivity * 0.01f;
        float mouseY = mouse.delta.y.ReadValue() * mouseSensitivity * 0.01f;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }
}
