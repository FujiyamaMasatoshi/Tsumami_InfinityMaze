//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputActionTest : MonoBehaviour
{
    private InputAction move;
    private InputAction jump;

    // Start is called before the first frame update
    void Start()
    {
        var input = GetComponent<PlayerInput>();

        // PlayerInputの default map で指定さrているアクションマップを有効化
        input.currentActionMap.Enable();


        // アクションマップから、アクションを取得するには FindAction() を使う
        move = input.currentActionMap.FindAction("Move");
        jump = input.currentActionMap.FindAction("Jump");
    }

    // Update is called once per frame
    void Update()
    {
        var moveValue = move.ReadValue<Vector2>();
        if (moveValue.magnitude > 0f)
        {
            Debug.Log($"Moveアクションの値: {moveValue}");
        }

        // ボタンタイプのアクションは状態取得メソッドで状態を取得する
        if (jump.WasPressedThisFrame())
        {
            Debug.Log("Jumpアクションが実行された");
        }
    }
}
