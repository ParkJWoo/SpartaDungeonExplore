using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UIElements.Cursor;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    private Vector2 curMovementInput;
    public float jumpPower;
    public LayerMask groundLayerMask;
    public float jumpStaminaCost = 15f;

    private Coroutine speedUpCoroutine;
    private Coroutine jumpPowerUpCoroutine;

    [Header("Dash")]
    public float dashSpeedMultiplier = 10f;
    public float dashStaminaCostPerSeconds = 10f;
    private bool isDashing;
    private bool isDashKeyPressed;

    [Header("Look")]
    public Transform cameraContainer;
    public float minXLook;
    public float maxXLook;
    private float camCurXRot;
    public float lookSensitivity;

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

    private void FixedUpdate()
    {
        HandleDash();
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
            playerCondition.ConsumeStamina(jumpStaminaCost);
            rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
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

    private IEnumerator JumpPowerUpRoutine(float amount, float duration)
    {
        jumpPower += amount;

        yield return new WaitForSeconds(duration);

        jumpPower -= amount;
        jumpPowerUpCoroutine = null;
    }

    private void HandleDash()
    {
        isDashing = false;

        if(isDashKeyPressed && curMovementInput != Vector2.zero)
        {
            float cost = dashStaminaCostPerSeconds * Time.fixedDeltaTime;

            if(playerCondition != null && playerCondition.CanUseStamina(cost))
            {
                isDashing = true;
                playerCondition.ConsumeStamina(cost);
            }
        }
    }
}