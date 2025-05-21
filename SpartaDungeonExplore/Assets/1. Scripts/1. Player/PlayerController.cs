using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UIElements.Cursor;

public class PlayerController : MonoBehaviour
{
    #region 플레이어 이동 및 점프 관련 변수들
    [Header("Movement")]
    public float moveSpeed;
    private Vector2 curMovementInput;
    public float jumpPower;
    public LayerMask groundLayerMask;
    public float jumpStaminaCost = 15f;
    #endregion

    #region 벽 타기 기능 관련 변수들
    [Header("ClimbingWall")]
    public float wallSlideSpeed;                                                    //  벽 타기 중 떨어지는 속도
    public float wallClimbSpeed;                                                    //  벽을 타고 올라가는 속도
    public LayerMask wallLayer;                                                     //  벽 레이어 
    public Transform wallCheck;                                                     //  벽 충돌 판정 위치  
    public float wallCheckRadius;                                                   //  벽 판정 범위
    public Vector3 wallJumpDirectoin = new Vector3(1, 1, 0);                        //  벽에 붙어있을 떄 점프 방향
    private bool isTouchingWall;                                                    //  벽에 닿았는지 판별
    private bool isWallSliding;                                                     //  벽에 매달려 천천히 내려가는 상태인지 판별
    private bool isWallClimbing;                                                    //  벽을 타고 올라가는 상태인지 판별
    #endregion

    //  효과 지속 시간 관련 변수들
    private Coroutine speedUpCoroutine;
    private Coroutine jumpPowerUpCoroutine;


    #region 대쉬 기능 관련 변수들
    [Header("Dash")]
    public float dashSpeedMultiplier = 10f;
    public float dashStaminaCostPerSeconds = 10f;
    private bool isDashing;
    private bool isDashKeyPressed;
    #endregion

    #region 플레이어가 바라보는 방향 관련 변수들
    [Header("Look")]
    public Transform cameraContainer;
    public float minXLook;
    public float maxXLook;
    private float camCurXRot;
    public float lookSensitivity;
    #endregion

    private Vector2 mouseDelta;

    //  인벤토리를 켤 때 마우스 커서를 보이게 하기 위한 변수
    public bool canLook = true;

    public Action inventory;

    private Rigidbody rigidbody;

    private PlayerCondition playerCondition;

    private void Awake()
    {
        playerCondition = GetComponent<PlayerCondition>();
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        //  게임 시작 시, 마우스 커서를 보이지 않게 할 예정
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        WallCheck();
        WallSlide();
        WallClimb();
    }

    private void FixedUpdate()
    {
        Dash();
        Move();
    }

    private void LateUpdate()
    {
        if(canLook)
        {
            CameraLook();
        }
    }

    public void OnLookInput(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {
            curMovementInput = context.ReadValue<Vector2>();
        }

        else if(context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;
        }
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started && IsGrounded())
        {
            //  플레이어가 위치한 곳이 땅이라면, 스태미너를 사용하여 점프한다.
            if(IsGrounded())
            {
                if(playerCondition.CanUseStamina(jumpStaminaCost))
                {
                    //  중재자에게 점프 시 스태미너 소모 요청
                    GameMediator.Instance.Notify(this, GameEvent.StaminaKey, jumpStaminaCost);

                    //playerCondition.ConsumeStamina(jumpStaminaCost);
                    rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
                }
            }

            else if(isWallSliding || isWallClimbing)
            {
                if(playerCondition.CanUseStamina(jumpStaminaCost))
                {
                    //  중재자에게 점프 시 스태미너 소모 요청
                    GameMediator.Instance.Notify(this, GameEvent.StaminaKey, jumpStaminaCost);

                    //playerCondition.ConsumeStamina(jumpStaminaCost);

                    //  벽에서 점프할 때 캐릭터 방향과 반대 방향으로 튕긴다.
                    Vector3 wallJumpDir = new Vector3(-transform.forward.x, 1, -transform.forward.z).normalized;
                    rigidbody.velocity = Vector3.zero;
                    rigidbody.AddForce(wallJumpDir * jumpPower, ForceMode.Impulse);

                    isWallSliding = false;
                    isWallClimbing = false;
                }
            }
        }
    }

    public void OnInventotyInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            inventory?.Invoke();
            ToggleCursor();
        }
    }

    public void OnDashInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            isDashKeyPressed = true;
        }

        else if(context.phase == InputActionPhase.Canceled)
        {
            isDashKeyPressed = false;
        }
    }

    private void Move()
    {
        float currentSpeed = moveSpeed;

        if(isDashing)
        {
            currentSpeed *= dashSpeedMultiplier;
        }

        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        dir *= currentSpeed;
        dir.y = rigidbody.velocity.y;

        rigidbody.velocity = dir;
    }

    void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }

    bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down)
        };

        for(int  i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask))
            {
                return true;
            }
        }

        return false;
    }

    public void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }

    //  이동 속도 증가 효과를 적용하는 메서드
    public void ApplySpeedUp(float amount, float duration)
    {
        if(speedUpCoroutine != null)
        {
            StopCoroutine(speedUpCoroutine);
        }

        speedUpCoroutine = StartCoroutine(SpeedUpRoutine(amount, duration));
    }

    private IEnumerator SpeedUpRoutine(float amount, float duration)
    {
        moveSpeed += amount;

        yield return new WaitForSeconds(duration);

        moveSpeed -= amount;
        speedUpCoroutine = null;
    }

    //  점프력 상승 효과를 적용하는 메서드
    public void ApplyJumpPowerUp(float amount, float duration)
    {
        if(jumpPowerUpCoroutine != null)
        {
            StopCoroutine(jumpPowerUpCoroutine);
        }

        jumpPowerUpCoroutine = StartCoroutine(JumpPowerUpRoutine(amount, duration));
    }

    //  점프력 상승 효과 지속 시간 코루틴 함수
    private IEnumerator JumpPowerUpRoutine(float amount, float duration)
    {
        jumpPower += amount;

        yield return new WaitForSeconds(duration);

        jumpPower -= amount;
        jumpPowerUpCoroutine = null;
    }

    //  대쉬 기능 메서드 
    private void Dash()
    {
        isDashing = false;

        if(isDashKeyPressed && curMovementInput != Vector2.zero)
        {
            float cost = dashStaminaCostPerSeconds * Time.fixedDeltaTime;

            if(playerCondition != null && playerCondition.CanUseStamina(cost))
            {
                isDashing = true;
                GameMediator.Instance.Notify(this, GameEvent.StaminaKey, cost);
                //playerCondition.ConsumeStamina(cost);
            }
        }
    }

    //  벽 근처 체크
    private void WallCheck()
    {
        isTouchingWall = Physics.CheckSphere(wallCheck.position, wallCheckRadius, wallLayer);
    }

    //  벽에 붙어있을 때 느리게 미끄러지도록 하는 메서드
    private void WallSlide()
    {
        if(isTouchingWall && !IsGrounded() && rigidbody.velocity.y < 0)
        {
            isWallSliding = true;
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, -wallSlideSpeed, rigidbody.velocity.z);
        }

        else
        {
            isWallSliding = false;
        }
    }

    //  벽 타기 처리 → W 키를 입력하여 벽을 올라간다.
    private void WallClimb()
    {
        if(isTouchingWall && curMovementInput.y > 0)
        {
            isWallClimbing = true;
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, wallClimbSpeed, rigidbody.velocity.z);
        }

        else
        {
            isWallClimbing = false;
        }
    }
  
}