using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float runSpeed = 6f;
    [SerializeField] float jumpSpeed = 12f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] float gravityScaleAtStart;

    Vector2 moveInput;
    Rigidbody2D myRigidbody;
    Animator myAnimator;

    CapsuleCollider2D myCapsuleCollider;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myCapsuleCollider = GetComponent<CapsuleCollider2D>();
        gravityScaleAtStart = myRigidbody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        Run();
        FlipSprite();
        ClimbLadder();
    }

    void OnMove(InputValue value){
        moveInput = value.Get<Vector2>();
        Debug.Log(moveInput);
    }

    void OnJump(InputValue value){
        if(!myCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))){
            return;
        }
        if(value.isPressed){
                myRigidbody.velocity += new Vector2(0f, jumpSpeed);
        }
    }

  

    void Run(){
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVelocity;

        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        if(playerHasHorizontalSpeed){
            myAnimator.SetBool("IsRunning", true);
        }else{
            myAnimator.SetBool("IsRunning", false);
        }
    }
    void FlipSprite(){
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        if(playerHasHorizontalSpeed){
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.velocity.x), 1f);
        }
    }
      void ClimbLadder(){
        if(!myCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"))){ 
            myRigidbody.gravityScale = gravityScaleAtStart;
            return; 
        }

        Vector2 climbVelocity = new Vector2(myRigidbody.velocity.x, moveInput.y * climbSpeed);
        myRigidbody.velocity = climbVelocity;
        myRigidbody.gravityScale = 0f;      
        
        bool playerHasVerticalSpeed = Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon;
        if(playerHasVerticalSpeed){
            myAnimator.SetBool("IsClimbing", true); 
        }else{
            myAnimator.SetBool("IsClimbing", false);  
        }
        
    }

}
