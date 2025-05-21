using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UIElements.Cursor;

public class PlayerController : MonoBehaviour
{
    #region �÷��̾� �̵� �� ���� ���� ������
    [Header("Movement")]
    public float moveSpeed;
    private Vector2 curMovementInput;
    public float jumpPower;
    public LayerMask groundLayerMask;
    public float jumpStaminaCost = 15f;
    #endregion

    #region �� Ÿ�� ��� ���� ������
    [Header("ClimbingWall")]
    public float wallSlideSpeed;                                                    //  �� Ÿ�� �� �������� �ӵ�
    public float wallClimbSpeed;                                                    //  ���� Ÿ�� �ö󰡴� �ӵ�
    public LayerMask wallLayer;                                                     //  �� ���̾� 
    public Transform wallCheck;                                                     //  �� �浹 ���� ��ġ  
    public float wallCheckRadius;                                                   //  �� ���� ����
    public Vector3 wallJumpDirectoin = new Vector3(1, 1, 0);                        //  ���� �پ����� �� ���� ����
    private bool isTouchingWall;                                                    //  ���� ��Ҵ��� �Ǻ�
    private bool isWallSliding;                                                     //  ���� �Ŵ޷� õõ�� �������� �������� �Ǻ�
    private bool isWallClimbing;                                                    //  ���� Ÿ�� �ö󰡴� �������� �Ǻ�
    #endregion

    //  ȿ�� ���� �ð� ���� ������
    private Coroutine speedUpCoroutine;
    private Coroutine jumpPowerUpCoroutine;


    #region �뽬 ��� ���� ������
    [Header("Dash")]
    public float dashSpeedMultiplier = 10f;
    public float dashStaminaCostPerSeconds = 10f;
    private bool isDashing;
    private bool isDashKeyPressed;
    #endregion

    #region �÷��̾ �ٶ󺸴� ���� ���� ������
    [Header("Look")]
    public Transform cameraContainer;
    public float minXLook;
    public float maxXLook;
    private float camCurXRot;
    public float lookSensitivity;
    #endregion

    private Vector2 mouseDelta;

    //  �κ��丮�� �� �� ���콺 Ŀ���� ���̰� �ϱ� ���� ����
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
        //  ���� ���� ��, ���콺 Ŀ���� ������ �ʰ� �� ����
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
            //  �÷��̾ ��ġ�� ���� ���̶��, ���¹̳ʸ� ����Ͽ� �����Ѵ�.
            if(IsGrounded())
            {
                if(playerCondition.CanUseStamina(jumpStaminaCost))
                {
                    //  �����ڿ��� ���� �� ���¹̳� �Ҹ� ��û
                    GameMediator.Instance.Notify(this, GameEvent.StaminaKey, jumpStaminaCost);

                    //playerCondition.ConsumeStamina(jumpStaminaCost);
                    rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
                }
            }

            else if(isWallSliding || isWallClimbing)
            {
                if(playerCondition.CanUseStamina(jumpStaminaCost))
                {
                    //  �����ڿ��� ���� �� ���¹̳� �Ҹ� ��û
                    GameMediator.Instance.Notify(this, GameEvent.StaminaKey, jumpStaminaCost);

                    //playerCondition.ConsumeStamina(jumpStaminaCost);

                    //  ������ ������ �� ĳ���� ����� �ݴ� �������� ƨ���.
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

    //  �̵� �ӵ� ���� ȿ���� �����ϴ� �޼���
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

    //  ������ ��� ȿ���� �����ϴ� �޼���
    public void ApplyJumpPowerUp(float amount, float duration)
    {
        if(jumpPowerUpCoroutine != null)
        {
            StopCoroutine(jumpPowerUpCoroutine);
        }

        jumpPowerUpCoroutine = StartCoroutine(JumpPowerUpRoutine(amount, duration));
    }

    //  ������ ��� ȿ�� ���� �ð� �ڷ�ƾ �Լ�
    private IEnumerator JumpPowerUpRoutine(float amount, float duration)
    {
        jumpPower += amount;

        yield return new WaitForSeconds(duration);

        jumpPower -= amount;
        jumpPowerUpCoroutine = null;
    }

    //  �뽬 ��� �޼��� 
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

    //  �� ��ó üũ
    private void WallCheck()
    {
        isTouchingWall = Physics.CheckSphere(wallCheck.position, wallCheckRadius, wallLayer);
    }

    //  ���� �پ����� �� ������ �̲��������� �ϴ� �޼���
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

    //  �� Ÿ�� ó�� �� W Ű�� �Է��Ͽ� ���� �ö󰣴�.
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