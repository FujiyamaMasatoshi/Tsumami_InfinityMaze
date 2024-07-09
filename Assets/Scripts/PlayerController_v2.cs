using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController_v2 : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float jumpPower = 5;
    [SerializeField] private float rotateSpeed = 5;
    private CharacterController characterController;
    private Vector3 moveVelocity;
    private InputAction move;
    private InputAction jump;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        var input = GetComponent<PlayerInput>();
        input.currentActionMap.Enable();
        move = input.currentActionMap.FindAction("Move");
        jump = input.currentActionMap.FindAction("Jump");
    }


    private void Moving()
    {
        var moveValue = move.ReadValue<Vector2>();
        moveVelocity.x = moveValue.x * moveSpeed;
        moveVelocity.z = moveValue.y * moveSpeed;

        transform.LookAt(transform.position + new Vector3(moveVelocity.x, 0.0f, moveVelocity.z));

        
    }

    private void Jump()
    {
        if (characterController.isGrounded)
        {
            if (jump.WasPressedThisFrame())
            {
                moveVelocity.y = jumpPower;
            }
        }
        else
        {
            moveVelocity.y += Physics.gravity.y * Time.deltaTime;
        }

        characterController.Move(moveVelocity * Time.deltaTime);
    }

    private void LookUp()
    {
        float mouseX = Input.GetAxis("Mouse X") * rotateSpeed;

        // プレイヤーの横回転
        transform.Rotate(new Vector3(0.0f, mouseX, 0.0f));
    }

    // Update is called once per frame
    void Update()
    {
        Moving();
        LookUp();
        Jump();
    }
}
