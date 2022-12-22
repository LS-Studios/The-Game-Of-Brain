using Zenject;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    public Rigidbody2D rb;
    Vector2 moveDirection;

    public Animator animator;

    public bool isMobile = false;

    public Joystick moveJoyStick;

    public Joystick attackJoyStick;

    public GameObject sight;

    public bool doAim = false;
    public bool shootAfterRotate = false;

    private bool rotateToMoveDir = true;

    private float moveAngle = 0;
    private float aimAngle = 0;

    private HealtComponent healthComponent;

    public CurrentItemHandler currentItemHandler;
    private bool holdingShootButton = false;

    [Inject]
    private GameManager gameManager;

    private void Awake()
    {
        healthComponent = GetComponent<HealtComponent>();

        isMobile = GameInstance.instance.debugValues.isMobile;
    }
    void Update()
    {
        if (isMobile)
        {
            MoveMobileValues();
        }
        else
        {
            ProcessInputs();

            if (gameManager.isAim)
            {
                LoockAtMouse();

                sight.SetActive(true);
            }
            else
            {
                sight.SetActive(false);

                if (currentItemHandler.CurrentItem != null &&
                currentItemHandler.CurrentItem.GetComponent<ItemInfo>().ItemData.generalData.sightTyp ==
                ItemData.GeneralData.SightTyp.Place)
                {
                    sight.SetActive(true);
                }


                if (rb.velocity == Vector2.zero)
                    StartCoroutine(freezDelay());
            }
        }

        if (currentItemHandler.CurrentItem != null && currentItemHandler.CurrentItem.GetComponent<WeaponBase>() != null)
            holdingShootButton = currentItemHandler.CurrentItem.GetComponent<WeaponBase>().IsShoot;
        else
            holdingShootButton = false;

        Animation();
    }

    void FixedUpdate()
    {
        if (isMobile)
            JoyStickAction();
        else
            KeyboardMove();
    }

    #region PC input
    void ProcessInputs()
    {
        moveDirection = gameManager.controlls.KeyBoard.Movement.ReadValue<Vector2>().normalized;
    }

    void KeyboardMove()
    {
        rb.velocity = new Vector2(moveDirection.x * healthComponent.currentSpeed, moveDirection.y * healthComponent.currentSpeed);

        //Loock to move direction if not aiming
        if (!Mouse.current.rightButton.IsPressed() && (rb.velocity.x != 0 || rb.velocity.y != 0))
        {
            Vector3 moveAngle = Quaternion.LookRotation(Vector3.forward, moveDirection).eulerAngles;
            rb.MoveRotation(Mathf.LerpAngle(rb.rotation, moveAngle.z, 15 * Time.deltaTime));
        }
    }

    void LoockAtMouse()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        Vector2 direction = new Vector2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y);
        transform.up = direction;
    }
    #endregion

    #region Mobile input
    public void MoveMobileValues()
    {
        float moveLookY = moveJoyStick.Vertical;
        float moveLookX = moveJoyStick.Horizontal;

        moveDirection = new Vector2(moveJoyStick.Horizontal, moveJoyStick.Vertical);

        if (moveLookY > 0.2f || moveLookX > 0.2f)
        {
            moveDirection = new Vector2(moveJoyStick.Horizontal, moveJoyStick.Vertical);
        }
        else if (moveLookY < -0.2f || moveLookX < -0.2)
        {
            moveDirection = new Vector2(moveJoyStick.Horizontal, moveJoyStick.Vertical);
        }
    }

    private void JoyStickAction()
    {
        float moveLookY = moveJoyStick.Vertical;
        float moveLookX = moveJoyStick.Horizontal;

        float aimLookY = attackJoyStick.Vertical;
        float aimLookX = attackJoyStick.Horizontal;

        rb.velocity = new Vector2(moveDirection.x * healthComponent.currentSpeed, moveDirection.y * healthComponent.currentSpeed);

        bool doMove = (moveLookY > 0.2f || moveLookY < -0.2f || moveLookX > 0.2f || moveLookX < -0.2) && rotateToMoveDir;

        //Lock to move direction
        if (doMove)
        {
            moveAngle = Mathf.Atan2(moveLookY, moveLookX) * Mathf.Rad2Deg - 90f;

            rb.MoveRotation(Mathf.LerpAngle(rb.rotation, moveAngle, 15 * Time.deltaTime));
        }

        doAim = (aimLookY > 0.01f || aimLookY < -0.01f || aimLookX > 0.01f || aimLookX < -0.01f);


        //Freeze posible roatation after hitting any object
        if (!doMove && !doAim && !holdingShootButton)
            StartCoroutine(freezDelay());

        //Aim 
        if (doAim)
        {
            aimAngle = Mathf.Atan2(aimLookY, aimLookX) * Mathf.Rad2Deg - 90f;

            rb.rotation = aimAngle;

            sight.SetActive(true);
        }
        //Dont aim
        else
        {
            if (holdingShootButton)
                AimToClosestTarget();

            sight.SetActive(false);

            if (currentItemHandler.CurrentItem != null &&
                currentItemHandler.CurrentItem.GetComponent<ItemInfo>().ItemData.generalData.sightTyp ==
                ItemData.GeneralData.SightTyp.Place)
            {
                sight.SetActive(true);
            }
        }
    }

    public void AimToClosestTarget()
    {
        Transform closestTarget = GetClosestTarget(4f);
        if (closestTarget != null)
        {
            Vector3 pos = closestTarget.position - transform.position;
            aimAngle = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg - 90;

            rb.rotation = aimAngle;
        }
    }

    public IEnumerator ShootDelay()
    {
        while (rb.rotation != aimAngle)
        {
            yield return null;
        }
        print("You");
        currentItemHandler.CallCurrentItemAction();
        shootAfterRotate = false;
    }

    private Transform GetClosestTarget(float radius)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);

        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (Collider2D potentialTarget in colliders)
        {
            if (potentialTarget.GetComponent<Enemy>() != null)
            {
                Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;

                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = potentialTarget.transform;
                }
            }
        }

        return bestTarget;
    }
    #endregion

    public void SetRun(bool isRun)
    {
        switch (isRun)
        {
            case true:
                healthComponent.currentSpeed = healthComponent.runSpeed;
                break;
            case false:
                healthComponent.currentSpeed = healthComponent.normalSpeed;
                break;
        }
    }

    private void Animation()
    {
        if (moveDirection.x != 0 || moveDirection.y != 0)
        {
            animator.SetFloat("Speed", 1);
        }
        else
        {
            animator.SetFloat("Speed", 0);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        StartCoroutine(freezDelay());
    }

    private IEnumerator freezDelay()
    {
        rb.freezeRotation = true;

        yield return new WaitForSeconds(0.05f);

        rb.freezeRotation = false;
    }
}
