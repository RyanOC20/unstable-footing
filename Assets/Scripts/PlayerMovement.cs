using PurrNet;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;

    private CharacterController _controller;
    private Vector3 _velocity;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (!isOwner) return;

        var kb = Keyboard.current;
        if (kb == null) return;

        float x = (kb.dKey.isPressed ? 1f : 0f) - (kb.aKey.isPressed ? 1f : 0f);
        float z = (kb.wKey.isPressed ? 1f : 0f) - (kb.sKey.isPressed ? 1f : 0f);

        Vector3 move = transform.right * x + transform.forward * z;
        _controller.Move(move * moveSpeed * Time.deltaTime);

        if (_controller.isGrounded && _velocity.y < 0)
            _velocity.y = -2f;

        _velocity.y += gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }
}
