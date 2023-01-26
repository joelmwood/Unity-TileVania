using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float runSpeed = 6f;
    [SerializeField] float jumpSpeed = 12f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] float gravityScaleAtStart;
    [SerializeField] Vector2 deathKick = new Vector2(5f, 5f);
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gun;
    int delayInSeconds = 2;

    [SerializeField] public bool isAlive = true;

    Vector2 moveInput;
    Rigidbody2D myRigidbody;
    Animator myAnimator;

    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = myRigidbody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive) { return; }
        Run();
        FlipSprite();
        ClimbLadder();
        StopClimbingLadder();
        Die();
    }



    void OnFire(InputValue value)
    {
        if (!isAlive) { return; }
        Instantiate(bullet, gun.position, Quaternion.Euler(0f, 0f, 0f));
    }

    void OnMove(InputValue value)
    {
        if (!isAlive) { return; }
        moveInput = value.Get<Vector2>();
        // Debug.Log(moveInput);
    }

    void OnJump(InputValue value)
    {
        if (!isAlive) { return; }

        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            return;
        }
        if (value.isPressed)
        {
            myRigidbody.velocity += new Vector2(0f, jumpSpeed);
        }
    }

    void OnQuit()
    {
        Application.Quit();
        Debug.Log("Quitting Game");
    }

    void OnResetScene()
    {
        FindObjectOfType<GameSession>().ResetScene();

    }



    void Run()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVelocity;

        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed)
        {
            myAnimator.SetBool("IsRunning", true);
        }
        else
        {
            myAnimator.SetBool("IsRunning", false);
        }
    }
    void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.velocity.x), 1f);
        }
    }
    void ClimbLadder()
    {
        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            myRigidbody.gravityScale = gravityScaleAtStart;
            return;
        }

        Vector2 climbVelocity = new Vector2(myRigidbody.velocity.x, moveInput.y * climbSpeed);
        myRigidbody.velocity = climbVelocity;
        myRigidbody.gravityScale = 0f;

        bool playerHasVerticalSpeed = Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon;
        if (playerHasVerticalSpeed)
        {
            myAnimator.SetBool("IsClimbing", true);
        }
        else
        {
            myAnimator.SetBool("IsClimbing", false);
        }


    }

    void StopClimbingLadder()
    {
        bool playerHasVerticalSpeed = Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon;
        if (!playerHasVerticalSpeed && !myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            myAnimator.SetBool("IsClimbing", false);
        }
    }

    void Die()
    {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemies", "Hazards")))
        {
            isAlive = false;
            myAnimator.SetTrigger("Dying");
            myRigidbody.velocity = deathKick;
            StartCoroutine(ProcessDeath());
        }
    }
    IEnumerator ProcessDeath()
    {
        yield return new WaitForSecondsRealtime(delayInSeconds);
        FindObjectOfType<GameSession>().ProcessPlayerDeath();

    }

    public bool GetIsPlayerAlive()
    {
        return isAlive;
    }



}
