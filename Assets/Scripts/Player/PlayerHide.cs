using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHide : MonoBehaviour
{
    [SerializeField] private float hideScale = 0.6f;

    private bool isHide = false;
    private Vector3 originalTransform;

    // Start is called before the first frame update
    void Start()
    {
        originalTransform = transform.localPosition;
    }

    private void Hide()
    {
        // 元の大きさの半分の高さsphereを下げる
        transform.localPosition += Vector3.down*hideScale/2f ;

        // sphereをhideScaleに圧縮
        transform.localScale = new Vector3(hideScale, hideScale, hideScale);
    }

    private void NotHide()
    {
        transform.localPosition = originalTransform;
        transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public bool GetIsHide()
    {
        return isHide;
    }

    // Update is called once per frame
    void Update()
    {
        // 右クリックをした時
        if (Input.GetMouseButtonDown(1))
        {
            // 隠れている状態ならば、
            if (isHide)
            {
                isHide = false;
                NotHide();
            }
            else
            {
                isHide = true;
                Hide();
            }
        }
    }
}
