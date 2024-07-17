using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("移動スピード")] public float initSpeed = 5.0f;
    [Header("hide状態時のスピード")] public float hideSpeed = 1.0f;
    [Header("回転スピード")] public float rotateSpeed = 5.0f;
    [Header("ジャンプ高さ")] public float jumpHeight = 5.0f;

    // Body
    [Header("回転させるBody")] public GameObject body = null;
    [Header("隠れるbody(Sphere)")] public PlayerHide hideBody = null;

    private Rigidbody rb = null;
    //private Animator anim = null;

    // jump
    private bool isOnFloor = false;
    [SerializeField] private int max_jump = 3;
    private int now_jump = 0;

    // movement
    private float moveSpeed;
    private bool isWallHit = false; // hit状態ならば前に進ませない

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //anim = GetComponent<Animator>();
        moveSpeed = initSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.isEventDoing)
        {
            PlayerMove(); // 移動
            PlayerRotate(); // 回転
            PlayerJump(); // ジャンプ
            PlayerLook(); // 見渡す
        }

        var isHide = hideBody.GetIsHide();
        if (isHide)
        {
            moveSpeed = hideSpeed;
        }
        else
        {
            moveSpeed = initSpeed;
        }
    }

    // 視点開店
    private void PlayerRotate()
    {
        //float rotation = Input.GetAxis("Mouse X") * rotateSpeed;
        float rotation = Input.GetAxis("Horizontal");

        // プレイヤーの横回転
        //transform.Rotate(new Vector3(0.0f, mouseX, 0.0f));
        transform.Rotate(new Vector3(0.0f, rotation, 0.0f));
    }

    private void PlayerLook()
    {

        float rotation = Input.GetAxis("Mouse X")*rotateSpeed/2;
        // プレイヤーの横回転
        transform.Rotate(new Vector3(0.0f, rotation, 0.0f));
    }


    private void PlayerMove()
    {

        // wasd入力を受け取る
        float forwardValue = Input.GetAxis("Vertical");
        float rightValue = Input.GetAxis("Horizontal");

        // 動く方向を決定
        Vector3 moveDirection = transform.forward * forwardValue + transform.right * rightValue;
        //Vector3 moveDirection = transform.forward * forwardValue;

        // moveDirectionの正規化
        moveDirection.Normalize();

        // 移動
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        //rb.MovePosition(moveDirection * moveSpeed * Time.deltaTime);


        // bodyの回転
        if (forwardValue > 0)
        {
            //body.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
            body.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
        else if (forwardValue < 0)
        {
            //body.transform.localEulerAngles = new Vector3(0.0f, -180.0f, 0.0f);
            body.transform.localScale = new Vector3(1.0f, 1.0f, -1.0f);
        }

        // 回転させる
        if (rightValue != 0)
        {
            body.transform.localEulerAngles = new Vector3(0.0f, body.transform.localScale.z * 90.0f * rightValue, 0.0f);
        }



    }

    private void PlayerJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isOnFloor || now_jump < max_jump)
            {
                rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
                isOnFloor = false;
                now_jump += 1;
            }
        }
    }



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("wall"))
        {
            Debug.Log("hit");
            moveSpeed = 1f;
        }
        if (collision.collider.CompareTag("floor"))
        {
            isOnFloor = true;
            now_jump = 0;
        }


        
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("wall"))
        {
            Debug.Log("hit");
            moveSpeed = 1f;
        }
        if (collision.collider.CompareTag("floor"))
        {
            isOnFloor = true;
            now_jump = 0;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("wall"))
        { 
            Debug.Log("not hit");
            moveSpeed = initSpeed;
        }
        if (collision.collider.CompareTag("floor"))
        {
            isOnFloor = false;
        }
    }
}
