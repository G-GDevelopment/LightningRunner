using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision_Player : MonoBehaviour
{
    //Variables
    private Movement move;

    [Header("Layers")]
    public LayerMask groundLayer;

    [Space]
    public bool onGround;
    public bool onWall;
    public bool onRightWall;
    public bool onLeftWall;
    public int wallSide;
    public bool foundGround;
    public float distanceToGround = 1f;
    public float wallCheckDistance = 1f;


    public Vector2 boxSize;
    public Vector2 boxSize2;
    public float boxAngle;
    public Vector2 bottomBoxSize;


    public Vector2 bottomOffset, rightOffset, leftOffset, rightOffset2, leftOffset2;
    [Header("Ledge Check")]
    public bool onLedge;
    public bool isTouchingWall;
    public Transform wallCheck;
    public Transform ledgeCheck;

    public bool isTouchingRightLedge;
    public bool isTouchingLeftLedge;

    private void Start()
    {
        move = GetComponent<Movement>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        onGround = Physics2D.OverlapBox((Vector2)transform.position + bottomOffset, bottomBoxSize, boxAngle, groundLayer);
        onWall = Physics2D.OverlapBox((Vector2)transform.position + rightOffset, boxSize, boxAngle, groundLayer)
            || Physics2D.OverlapBox((Vector2)transform.position + leftOffset, boxSize, boxAngle, groundLayer);

        onRightWall = Physics2D.OverlapBox((Vector2)transform.position + rightOffset, boxSize, boxAngle, groundLayer);
        onLeftWall = Physics2D.OverlapBox((Vector2)transform.position + leftOffset, boxSize, boxAngle, groundLayer);

        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right * move.side, wallCheckDistance, groundLayer);

        isTouchingRightLedge = isTouchingWall = Physics2D.Raycast(ledgeCheck.position, transform.right * move.side, wallCheckDistance, groundLayer);

        /* onLedge = Physics2D.OverlapBox((Vector2)transform.position + rightOffset2, boxSize2, boxAngle, groundLayer) || Physics2D.OverlapBox((Vector2)transform.position + leftOffset2, boxSize2, boxAngle, groundLayer);
         isTouchingRightLedge = Physics2D.OverlapBox((Vector2)transform.position + rightOffset2, boxSize2, boxAngle, groundLayer);
         isTouchingLeftLedge = Physics2D.OverlapBox((Vector2)transform.position + leftOffset2, boxSize2, boxAngle, groundLayer); */

        wallSide = onRightWall ? -1 : 1;

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
        Gizmos.DrawLine(ledgeCheck.position, new Vector3(ledgeCheck.position.x + wallCheckDistance, ledgeCheck.position.y, ledgeCheck.position.z));
        
                Gizmos.DrawWireCube((Vector2)transform.position + bottomOffset, bottomBoxSize);
                Gizmos.DrawWireCube((Vector2)transform.position + rightOffset, boxSize);
                Gizmos.DrawWireCube((Vector2)transform.position + leftOffset, boxSize);
        //Gizmos.DrawWireCube((Vector2)transform.position + rightOffset2, boxSize2);
        //Gizmos.DrawWireCube((Vector2)transform.position + leftOffset2, boxSize2);

    }
}
