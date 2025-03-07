using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float jumpForce;
    private Vector2 curMovementInput;
    public LayerMask groundLayerMask;

    [Header("Look")]
    public Transform cameraContainer;
    public float minXLook;
    public float maxXLook;
    private float camCurXRot;
    public float lookSensitivity;
    private Vector2 mouseDelta;
    public bool canLook = true;

    public Vector3 startPosition;
    public Quaternion startRotation;  

    public Action inventory;
    public Rigidbody _rigidbody;    

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();      
    }

    void Start()
    {
        // 화면에서 커서 Lock
        Cursor.lockState = CursorLockMode.Locked;
        // 초기 위치와 회전값 저장
        startPosition = transform.position; 
        startRotation.eulerAngles = transform.eulerAngles;        
    }

    void FixedUpdate()
    {
        // 물리연산을 하는 로직은 FixedUpdate에서 하는 것이 좋음
        Move();
    }

    private void LateUpdate()
    {
        if (canLook) CameraLook();      
    }

    // 실제로 이동하는 메서드
    void Move()
    {
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        dir *= moveSpeed;
        dir.y = _rigidbody.velocity.y;

        _rigidbody.velocity = dir;
    }

    // 플레이어 보는 방향에 따라 카메라도 조정하는 메서드
    void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }

    // InputAction Move
    public void OnMove(InputAction.CallbackContext context)
    {
        // InputActionPhase 키 눌린 상태일 때 == Perfomed
        if (context.phase == InputActionPhase.Performed)
        {
            curMovementInput = context.ReadValue<Vector2>();
        }
        // InputActionPhase 키 뗐을 때 == Canceled
        else if (context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;
        }
    }

    // InputAction Look
    public void OnLook(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    // InputAction Jump
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && IsGrounded())
        {
            _rigidbody.AddForce(Vector2.up * jumpForce, ForceMode.Impulse);            
        }
    }    

    // 땅에 붙어있는지 아닌지 Ray를 통해 체크하는 bool 메서드
    bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down)
        };

        for (int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask))
            {
                return true;
            }
        }

        return false;
    }

    // InputAction Inventory
    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            inventory?.Invoke();
            ToggleCursor();
        }
    }

    void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }

    // InputAction ResetPosition
    public void OnResetPosition(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            transform.position = startPosition;
            transform.eulerAngles = startRotation.eulerAngles;       
        }
    }
}
