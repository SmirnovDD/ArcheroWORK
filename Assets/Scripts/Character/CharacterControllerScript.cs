using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerScript : MonoBehaviour
{
    public GameObject moveDirObj;
    public const float GRAVITY = 5;
    public float verticalAccel;

    public float movementSpeed;
    private CharacterController charController;

    private Vector2 startTouchPos; //куда ткнули пальцем
    private Vector2 movementDirectionScreen; //направление движения по экрану
    private Vector3 movementDirectionGround; //направление движения по земле

    private Vector3 verticalMoveVector; //определяет падение
    void Start()
    {
        charController = GetComponent<CharacterController>();
    }

    void Update()
    {
        HorizontalMovement();
        //VerticalMovement();
    }

    private void HorizontalMovement()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            startTouchPos = Input.mousePosition;
            PlayerShoot.shoot = false;
        }
        if (Input.GetMouseButton(0))
        {
            //PlayerAnimation.SetRunAnim(true);
            movementDirectionScreen = ((Vector2)Input.mousePosition - startTouchPos);
            movementDirectionGround = new Vector3(movementDirectionScreen.x, 0, movementDirectionScreen.y);
            movementDirectionGround = movementDirectionGround.normalized;
            moveDirObj.transform.position = transform.position + Vector3.down / 2 + VirtualJoystick.InputDirection * 1.5f;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            //PlayerAnimation.SetRunAnim(false);
            startTouchPos = Vector2.zero;
            movementDirectionScreen = Vector2.zero;
            movementDirectionGround = Vector3.zero;
            PlayerShoot.shoot = true;
        }
        if (movementDirectionGround != Vector3.zero)
        {
            charController.Move(movementDirectionGround * Time.deltaTime * movementSpeed);
            transform.forward = Vector3.Lerp(transform.forward, movementDirectionGround, Time.deltaTime * 15);
        }
#elif UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                startTouchPos = Input.GetTouch(0).position;
                PlayerShoot.shoot = false;
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                //PlayerAnimation.SetRunAnim(true);
                movementDirectionScreen = (Input.GetTouch(0).position - startTouchPos).normalized;
                movementDirectionGround = new Vector3(movementDirectionScreen.x, 0, movementDirectionScreen.y);
                moveDirObj.transform.position = transform.position + Vector3.down / 2 + VirtualJoystick.InputDirection * 1.5f;
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled)
            {
                //PlayerAnimation.SetRunAnim(false);
                PlayerShoot.shoot = true;
                startTouchPos = Vector2.zero;
                movementDirectionScreen = Vector2.zero;
                movementDirectionGround = Vector3.zero;
            }
            if (movementDirectionGround != Vector3.zero)
            {
                charController.Move(movementDirectionGround * Time.deltaTime * movementSpeed);
                transform.forward = Vector3.Lerp(transform.forward, movementDirectionGround, Time.deltaTime * 15);            
            }
        }
#endif
    }

    //private void VerticalMovement()
    //{
    //    if(charController.isGrounded)
    //    {
    //        verticalMoveVector = Vector3.zero;
    //    }
    //    else
    //    {
    //        if(verticalMoveVector.y < GRAVITY)
    //            verticalMoveVector += Vector3.down * verticalAccel;
    //    }

    //    charController.Move(verticalMoveVector * Time.deltaTime);
    //}
}

