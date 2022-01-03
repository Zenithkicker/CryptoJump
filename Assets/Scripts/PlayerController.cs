using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float jumpForce = 10f;    
    [SerializeField] float jumpForceMaxTimeDelay = .75f;
    [SerializeField] float lateralMovementForce = 4f;
    [SerializeField] LayerMask platformLayerMask;

    float jumpTimeCounter = 0f;
    float jumpForcePercent = 0f;
    bool isGrounded = true;
    bool isPreJumping = false;
    bool moveRightPressed = false;
    bool moveLeftPressed = false;

    Rigidbody2D rb2d;
    BoxCollider2D boxCollider2D;
    
    void Start()
    {
        rb2d = this.GetComponent<Rigidbody2D>();
        boxCollider2D = this.GetComponent<BoxCollider2D>();
        rb2d.freezeRotation = true;
    }

    void Update()
    {
        if(isGrounded)
        {
            //triggers if user presses key
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isPreJumping = true;
            }

            //triggers if user pressed key then released
            if(Input.GetKeyUp(KeyCode.Space))
            {
                isPreJumping = false;
                isGrounded = false;
                
                //calculate jump force
                jumpForcePercent = jumpTimeCounter / jumpForceMaxTimeDelay;
            }
        } 

        if(isPreJumping)
        {
            jumpTimeCounter += Time.deltaTime;
            if(jumpTimeCounter > jumpForceMaxTimeDelay)
            {
                jumpTimeCounter = jumpForceMaxTimeDelay;
            }
        }

        if(!isGrounded)
        {
            moveRightPressed = Input.GetKey(KeyCode.D);
            moveLeftPressed = Input.GetKey(KeyCode.A);
        }
    }

    void FixedUpdate()
    {
        if (!isGrounded)
        {
            float jumpForceToApply = jumpForcePercent * jumpForce;
            rb2d.AddForce(Vector2.up * jumpForceToApply, ForceMode2D.Impulse);            
            jumpForcePercent = 0f;
            jumpTimeCounter = 0f;       
        
            if (moveRightPressed)
            {
                rb2d.AddForce(Vector2.right * lateralMovementForce, ForceMode2D.Force);
            }

            if (moveLeftPressed)
            {
                rb2d.AddForce(Vector2.left * lateralMovementForce, ForceMode2D.Force);
            }
        }

        isGrounded = IsGrounded();
    }

    bool IsGrounded()
    {
        const float boxCastDistance = .3f;
        const float rot = 0f;
        var raycastHit = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.size, rot, Vector2.down, boxCastDistance, platformLayerMask);

        Color rayColor;
        if(null != raycastHit.collider)
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }

        Debug.DrawRay(boxCollider2D.bounds.center + new Vector3(boxCollider2D.bounds.extents.x, 0), Vector2.down * (boxCollider2D.bounds.extents.y + boxCastDistance), rayColor);
        Debug.DrawRay(boxCollider2D.bounds.center - new Vector3(boxCollider2D.bounds.extents.x, 0), Vector2.down * (boxCollider2D.bounds.extents.y + boxCastDistance), rayColor);
        Debug.DrawRay(boxCollider2D.bounds.center - new Vector3(boxCollider2D.bounds.extents.x, boxCollider2D.bounds.extents.y + boxCastDistance), Vector2.right * boxCollider2D.bounds.extents.x * 2, rayColor);

        return null != raycastHit.collider;
    }
}
