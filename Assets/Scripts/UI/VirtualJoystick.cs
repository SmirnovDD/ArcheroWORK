using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private Canvas gameCanvas;
    private float scaleFactor;
    private Image bgImg, joystickImg;
    private Vector2 defaultPos;
    public static Vector3 InputDirection { get; set; }
    private Vector3 startTouch = Vector2.zero;
    // Start is called before the first frame update
    void Start()
    {
        gameCanvas = GetComponentInParent<Canvas>();
        scaleFactor = gameCanvas.scaleFactor;
        InputDirection = Vector3.zero;
        bgImg = GetComponent<Image>();
        joystickImg = transform.GetChild(0).GetComponent<Image>();
        defaultPos = new Vector2(Screen.width / 2 - bgImg.rectTransform.sizeDelta.x / 2 * scaleFactor, Screen.height / 10);
        transform.position = defaultPos;
    }
    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && Time.timeScale != 0)
        {
            transform.position = Input.mousePosition - Vector3.right * bgImg.rectTransform.sizeDelta.x / 2 * scaleFactor + Vector3.down * bgImg.rectTransform.sizeDelta.y / 2 * scaleFactor;
            startTouch = Input.mousePosition;
        }
        else if(startTouch != Vector3.zero && Input.GetMouseButton(0))
        {
            Vector2 pos = Input.mousePosition - transform.position;
            pos.x = (pos.x / bgImg.rectTransform.sizeDelta.x);
            pos.y = (pos.y / bgImg.rectTransform.sizeDelta.y);

            float x = (bgImg.rectTransform.pivot.x == 1) ? pos.x * 2 + 1 : pos.x * 2 - 1;
            float y = (bgImg.rectTransform.pivot.y == 1) ? pos.y * 2 + 1 : pos.y * 2 - 1;

            InputDirection = new Vector3(x, 0, y);
            InputDirection = (InputDirection.magnitude > 1) ? InputDirection.normalized : InputDirection;
            joystickImg.rectTransform.anchoredPosition = new Vector3(InputDirection.x * (bgImg.rectTransform.sizeDelta.x / 3), InputDirection.z * (bgImg.rectTransform.sizeDelta.y / 3));
        }
        else if(Input.GetMouseButtonUp(0))
        {
            startTouch = Vector3.zero;
            joystickImg.rectTransform.anchoredPosition = Vector3.zero;
            transform.position = defaultPos;
        }
#elif UNITY_ANDROID
        if(Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began && Time.timeScale != 0)
            {
                if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                {
                    transform.position = (Vector3)Input.GetTouch(0).position - Vector3.right * bgImg.rectTransform.sizeDelta.x / 2 * scaleFactor + Vector3.down * bgImg.rectTransform.sizeDelta.y / 2 * scaleFactor;
                    startTouch = Input.GetTouch(0).position;
                }
            }
            else if (startTouch != Vector3.zero && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                Vector2 pos = Input.GetTouch(0).position - (Vector2)transform.position;
                pos.x = (pos.x / bgImg.rectTransform.sizeDelta.x);
                pos.y = (pos.y / bgImg.rectTransform.sizeDelta.y);

                float x = (bgImg.rectTransform.pivot.x == 1) ? pos.x * 2 + 1 : pos.x * 2 - 1;
                float y = (bgImg.rectTransform.pivot.y == 1) ? pos.y * 2 + 1 : pos.y * 2 - 1;

                InputDirection = new Vector3(x, 0, y);
                InputDirection = (InputDirection.magnitude > 1) ? InputDirection.normalized : InputDirection;
                joystickImg.rectTransform.anchoredPosition = new Vector3(InputDirection.x * (bgImg.rectTransform.sizeDelta.x / 3), InputDirection.z * (bgImg.rectTransform.sizeDelta.y / 3));
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled)
            {
                startTouch = Vector3.zero;
                joystickImg.rectTransform.anchoredPosition = Vector3.zero;
                transform.position = defaultPos;
            }
        }
#endif

    }

    public virtual void OnDrag(PointerEventData ped)
    {
        Vector2 pos = Vector2.zero;
        if(RectTransformUtility.ScreenPointToLocalPointInRectangle(bgImg.rectTransform, ped.position, ped.pressEventCamera, out pos))
        {
            pos.x = (pos.x / bgImg.rectTransform.sizeDelta.x);
            pos.y = (pos.y / bgImg.rectTransform.sizeDelta.y);

            float x = (bgImg.rectTransform.pivot.x == 1) ? pos.x * 2 + 1 : pos.x * 2 - 1;
            float y = (bgImg.rectTransform.pivot.y == 1) ? pos.y * 2 + 1 : pos.y * 2 - 1;

            InputDirection = new Vector3(x, 0, y);
            InputDirection = (InputDirection.magnitude > 1) ? InputDirection.normalized : InputDirection;
            joystickImg.rectTransform.anchoredPosition = new Vector3(InputDirection.x * (bgImg.rectTransform.sizeDelta.x / 3), InputDirection.z * (bgImg.rectTransform.sizeDelta.y / 3));
        }
    }

    public virtual void OnPointerDown(PointerEventData ped)
    {
        OnDrag(ped);
    }

    public virtual void OnPointerUp(PointerEventData ped)
    {
        InputDirection = Vector3.zero;
        joystickImg.rectTransform.anchoredPosition = Vector3.zero;
        transform.position = defaultPos;
    }
}
