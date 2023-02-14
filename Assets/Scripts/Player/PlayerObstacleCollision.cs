using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObstacleCollision : MonoBehaviour
{
    [SerializeField]
    private PlayerData playerData;

    #region Components
    private Rigidbody2D playerRB;
    private BoxCollider2D playerCollider;
    #endregion

    #region Local Variables
    public bool trampolineDetected;

    private Vector2 currentVelocity;
    private Vector2 workspace;
    #endregion

    #region Unity Callback Functions
    private void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();
    }
    #endregion

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Trampoline"))
        {
            trampolineDetected = true; 
            SetVelocityY(playerData.trampolineVelocity);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Trampoline"))
        {
            trampolineDetected = false;
        }
    }

    #region Set Functions
    // FUNCTION: Sets the velocity of the player to 0
    public void SetVelocityZero()
    {
        playerRB.velocity = Vector2.zero;
        currentVelocity = Vector2.zero;
    }

    // FUNCTION: Sets the X velocity of the player
    public void SetVelocityX(float velocity)
    {
        workspace.Set(velocity, currentVelocity.y);
        playerRB.velocity = workspace;
        currentVelocity = workspace;
    }

    // FUNCTION: Sets the Y velocity of the player
    public void SetVelocityY(float velocity)
    {
        workspace.Set(currentVelocity.x, velocity);
        playerRB.velocity = workspace;
        currentVelocity = workspace;
    }

    // FUNCTION: Sets the Vector2D velocity of the player
    public void SetVelocity(float velocity, Vector2 angle, int direction)
    {
        angle.Normalize();
        workspace.Set(angle.x * velocity * direction, angle.y * velocity);
        playerRB.velocity = workspace;
        currentVelocity = workspace;
    }

    public void SetVelocity(float velocity, Vector2 direction)
    {
        workspace = direction * velocity;
        playerRB.velocity = workspace;
        currentVelocity = workspace;
    }
    #endregion
}
