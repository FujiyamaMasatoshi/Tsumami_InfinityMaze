using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    // 移動速度
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpPower = 10f;
    private CharacterController characterController;
    private Vector3 moveVelocity;
    private InputAction move;
    private InputAction jump;


    

    // Start is called before the first frame update
    void Start()
    {
        // componentの取得
        characterController = GetComponent<CharacterController>();
        var input = GetComponent<PlayerInput>();
        // defaultのアクションマップを有効化
        input.currentActionMap.Enable();
        // move, jumpのアクションを取得
        move = input.currentActionMap.FindAction("Move");
        jump = input.currentActionMap.FindAction("Jump");
    }



    // Update is called once per frame
    void Update()
    {
        Debug.Log(characterController.isGrounded ? "地上にいます" : "空中です");

        // moveアクションを使った時の移動処理
        var moveValue = move.ReadValue<Vector2>();
        moveVelocity.x = moveValue.x * moveSpeed;
        moveVelocity.z = moveValue.y * moveSpeed;

        // 移動方向を向く
        transform.LookAt(transform.position + new Vector3(moveVelocity.x, 0f, moveVelocity.z));

        if (characterController.isGrounded)
        {
            if (jump.WasPressedThisFrame())
            {
                Debug.Log("Jump !");
                moveVelocity.y = jumpPower;
            }
        }
        else
        {
            moveVelocity.y += Physics.gravity.y * Time.deltaTime;
        }

        // Playerを動かす
        characterController.Move(moveVelocity * Time.deltaTime);
    }
}
