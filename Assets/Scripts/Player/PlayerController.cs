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
    private float startMoveSpeed;

    public float jumpForce;
    public float jumpStamina;
    public float superJumpForce;    
    private bool isOnSuperJumpZone = false;

    private Vector3 startPosition;
    private Quaternion startRotation;
    public float positionResetStamina;
    
    public float dashSpeed;
    public float dashStaminaPerRate;
    public float dashUseStaminaRate;
    private float lastSubStaminaTime;
    private bool isdashing = false;    

    public float teleportDistance;    
    public float teleportStamina;

    private Raft raft;
    private GameObject Fire;

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

    public Action inventory;
    public Action information;
    public Rigidbody _rigidbody;

    private Condition playerStamina;
    private HomeWarmZone homeWarmZone;         

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        homeWarmZone = FindObjectOfType<HomeWarmZone>().GetComponent<HomeWarmZone>();
        Fire = homeWarmZone.transform.GetChild(0).gameObject;
        Fire.SetActive(false);      
    }

    void Start()
    {
        // 화면에서 커서 Lock
        Cursor.lockState = CursorLockMode.Locked;

        // 초기 위치와 회전값 저장
        startPosition = transform.position; 
        startRotation.eulerAngles = transform.eulerAngles;

        // 초기 moveSpeed 저장
        startMoveSpeed = moveSpeed;

        // stamina 불러오기
        playerStamina = CharacterManager.Instance.Player.condition.stamina;        
    }

    void Update()
    {
        // dash
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (CanDash()) UseDash();
            else StopDash();
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift)) StopDash();
        
        if (homeWarmZone.turnOnFire) Fire.SetActive(true);
        else Fire.SetActive(false);
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

        // 뗏목 탔을 때 플레이어도 따라 움직이는 로직
        if (raft != null)
        {
            _rigidbody.MovePosition(_rigidbody.position + raft.GetMoveVector());
            return;
        }
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
        if (context.phase == InputActionPhase.Started && IsGrounded() && CharacterManager.Instance.Player.condition.UseStamina(jumpStamina))
        {
            _rigidbody.AddForce(Vector2.up * jumpForce, ForceMode.Impulse);
        }
    }

    // InputAction SuperJump
    public void OnSuperJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && isOnSuperJumpZone)
        {
            _rigidbody.AddForce(Vector2.up * superJumpForce, ForceMode.Impulse);
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

    // InputAction Information
    public void OnInformation(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            information?.Invoke();
            ToggleCursor();
        }
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
        if (context.phase == InputActionPhase.Started && CharacterManager.Instance.Player.condition.UseStamina(positionResetStamina))
        {
            transform.position = startPosition;
            transform.eulerAngles = startRotation.eulerAngles;       
        }
    }

    // 대쉬 가능한지 판단하는 bool 메서드
    bool CanDash()
    {
        if (playerStamina.curValue >= dashStaminaPerRate) return true;
        else return false;
    }

    // 대쉬 중 스태미나 지속적으로 빠지게 하는 메서드
    void SubtractStaminaOverTime()
    {
        if (isdashing)
        {
            if(Time.time - lastSubStaminaTime >= dashUseStaminaRate)
            {
                lastSubStaminaTime = Time.time;
                playerStamina.Subtract(dashStaminaPerRate);
            }            
        }
    }

    void UseDash()
    {
        moveSpeed = dashSpeed;
        isdashing = true;
        SubtractStaminaOverTime();
    }

    void StopDash()
    {
        moveSpeed = startMoveSpeed;
        isdashing = false;
    }

    // InputAction Teleport
    public void OnTeleport(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && CanTeleport() && CharacterManager.Instance.Player.condition.UseStamina(teleportStamina))
        {            
            transform.position += transform.forward * teleportDistance;
        }
    }

    bool CanTeleport()
    {
        Ray ray = new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 3f), Vector3.forward);

        if (Physics.Raycast(ray, teleportDistance, groundLayerMask))
        {
            return false;
        }
        else return true;       
    }

    // 뗏목, 슈퍼점프존에 올라갔을 때 감지하는 메서드
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Raft"))
        {
            raft = collision.gameObject.GetComponent<Raft>();
            return;
        }

        if (collision.gameObject.CompareTag("SuperJumpZone"))
        {
            isOnSuperJumpZone = true;
            return;
        }
    }

    // 뗏목, 슈퍼점프존에서 내렸을 때 감지하는 메서드
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Raft"))
        {
            raft = null;
            return;
        }

        if (collision.gameObject.CompareTag("SuperJumpZone"))
        {
            isOnSuperJumpZone = false;
            return;
        }
    }    

    // InputAction OnFire
    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (!homeWarmZone.turnOnFire) homeWarmZone.turnOnFire = true;
            else homeWarmZone.turnOnFire = false;
        }
    }
}
