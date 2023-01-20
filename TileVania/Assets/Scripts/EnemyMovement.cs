using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1f;

    Rigidbody2D myRigidbody;
    BoxCollider2D myBoxCollider;
    [SerializeField] PlayerMovement playerMovement;

    bool isPlayerAlive = true;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myBoxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        checkIfPlayerIsAlive();
        if (!isPlayerAlive) { myRigidbody.velocity = new Vector2(0f, 0f); return; }

        myRigidbody.velocity = new Vector2(moveSpeed, 0f);
    }


    void OnTriggerExit2D(Collider2D other)
    {
        // Debug.Log("Exited: " + other.gameObject.tag);
        moveSpeed = -moveSpeed;
        FlipEnemySprite();
    }

    void FlipEnemySprite()
    {
        transform.localScale = new Vector2(-(Mathf.Sign(myRigidbody.velocity.x)), 1f);
    }

    void checkIfPlayerIsAlive()
    {
        isPlayerAlive = playerMovement.GetIsPlayerAlive();
    }
}
