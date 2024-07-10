using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ###############################
// 敵キャラクタのベースの動きを記述
// ###############################
public class EnemyController : MonoBehaviour
{

    [SerializeField] float initMoveSpeed = 1.0f;
    [SerializeField] float speedUpRate = 2.0f;
    [SerializeField] int rayCastHits_count = 10;
    [SerializeField] float rotationSpeed = 2.0f;



    private Animator anim = null;
    private bool isDiscovered = false;
    private RaycastHit[] rayCastHits; // 当たり判定を保持 -- startで初期化
    private float moveSpeed = 0.0f; //動いているスピード
    private bool isSpeedup = false;
    private Vector3 moveDirection = Vector3.zero; // 進む方向
    private Quaternion originalRotation; // 元の向き

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        moveSpeed = initMoveSpeed;
        rayCastHits = new RaycastHit[rayCastHits_count];
        originalRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        Moving();
        
    }


    ///// <summary>
    ///// transorm.forwardの方向に進む
    ///// </summary>
    //private void moveForward()
    //{
    //    moveDirection = transform.forward;
    //    transform.position += moveDirection * moveSpeed * Time.deltaTime;
    //}

    ///// <summary>
    ///// 発見したプレイヤの方向に進む
    ///// </summary>
    //private void move2Player()
    //{
    //    //
    //}

    private void Moving()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        Debug.Log($"moveSpeed: {moveSpeed}");
    }

    private void turnRight()
    {
        // 
    }


    private void OnDetectObj(Collider collider)
    {
        // プレイヤ検知
        if (collider.CompareTag("Player"))
        {
            // プレイヤまでのベクトルを計算
            var positionDiff = collider.transform.position - transform.position;
            var dist = positionDiff.magnitude; // プレイヤまでの距離
            var direction = positionDiff.normalized;

            // ヒットカウントにでプレイヤが視界に存在しているかを判定
            var hitCount = Physics.RaycastNonAlloc(transform.position, direction, rayCastHits, dist);
            Debug.Log($"rayCastCount: {hitCount}");
            // プレイヤが視界内に存在
            if (hitCount <= 2)
            {
                
                isDiscovered = true;
                if (!isSpeedup)
                {
                    moveSpeed *= speedUpRate;
                    isSpeedup = true;
                }
                Debug.Log("discover");

                // 動く方向を決定
                moveDirection = direction;

                // プレイヤーの方向を向く
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

            }
            // プレイヤを見失ったら
            else
            {
                isDiscovered = false;
                moveSpeed = initMoveSpeed;

                // 元の向きに戻り、動き出す
                // 元の向きに戻る
                transform.rotation = Quaternion.Slerp(transform.rotation, originalRotation, Time.deltaTime * rotationSpeed);

                // 元の位置に戻る
                //Vector3 direction = (originalPosition - transform.position).normalized;
                //if (Vector3.Distance(transform.position, originalPosition) > 0.1f)
                //{
                //    transform.position += direction * moveSpeed * Time.deltaTime;
                //}
                moveDirection = transform.forward;
            }
        }
    }

    // Triggerによるプレイヤ検知
    private void OnTriggerEnter(Collider other)
    {
        OnDetectObj(other);
    }

    private void OnTriggerStay(Collider other)
    {
        OnDetectObj(other);
    }

    private void OnTriggerExit(Collider other)
    {
        isDiscovered = false;
        moveSpeed = initMoveSpeed;
        isSpeedup = false;
        // 元の向きに戻り、動き出す
        // 元の向きに戻る
        transform.rotation = Quaternion.Slerp(transform.rotation, originalRotation, Time.deltaTime * rotationSpeed);

        // 元の位置に戻る
        //Vector3 direction = (originalPosition - transform.position).normalized;
        //if (Vector3.Distance(transform.position, originalPosition) > 0.1f)
        //{
        //    transform.position += direction * moveSpeed * Time.deltaTime;
        //}
        moveDirection = transform.forward;
    }
}
