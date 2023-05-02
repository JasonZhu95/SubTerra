using UnityEngine;

public class CollisionSenses : CoreComponent
{
    private Movement Movement => movement ? movement : core.GetCoreComponent(ref movement);
    private Movement movement;

    #region Check Transforms

    public Transform GroundCheck {
        get => GenericNotImplementedError<Transform>.TryGet(groundCheck, core.transform.parent.name);
        private set => groundCheck = value;
    }
    public Transform WallCheck
    {
        get => GenericNotImplementedError<Transform>.TryGet(wallCheck, core.transform.parent.name);
        private set => wallCheck = value;
    }
    public Transform LedgeCheckHorizontal
    {
        get => GenericNotImplementedError<Transform>.TryGet(ledgeCheckHorizontal, core.transform.parent.name);
        private set => ledgeCheckHorizontal = value;
    }
    public Transform LedgeCheckVertical
    {
        get => GenericNotImplementedError<Transform>.TryGet(ledgeCheckVertical, core.transform.parent.name);
        private set => ledgeCheckVertical = value;
    }
    public Transform CeilingCheck
    {
        get => GenericNotImplementedError<Transform>.TryGet(ceilingCheck, core.transform.parent.name);
        private set => ceilingCheck = value;
    }
    public Transform CornerCorrectionCheck { get => cornerCorrectionCheck; private set => cornerCorrectionCheck = value; }

    public float GroundCheckRadius { get => groundCheckRadius; set => groundCheckRadius = value; }
    public float WallCheckDistance { get => wallCheckDistance; set => wallCheckDistance = value; }
    public LayerMask WhatIsGround { get => whatIsGround; set => whatIsGround = value; }
    public LayerMask WhatIsTrampoline { get => whatIsTrampoline; set => whatIsTrampoline = value; }

    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform ledgeCheckHorizontal;
    [SerializeField] private Transform ledgeCheckVertical;
    [SerializeField] private Transform ceilingCheck;
    [SerializeField] private Transform cornerCorrectionCheck;

    [SerializeField] private float groundCheckRadius;
    [SerializeField] private float wallCheckDistance;

    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsTrampoline;

    #endregion

    #region Check Properties
    public bool WallFront
    {
        get => Physics2D.Raycast(WallCheck.position, Vector2.right * Movement.FacingDirection, wallCheckDistance, whatIsGround);
    }

    public bool WallBack
    {
        get => Physics2D.Raycast(WallCheck.position, Vector2.right * -Movement.FacingDirection, wallCheckDistance, whatIsGround);
    }

    public bool LedgeHorizontal
    {
        get => Physics2D.Raycast(LedgeCheckHorizontal.position, Vector2.right * Movement.FacingDirection, wallCheckDistance, whatIsGround);
    }

    public bool LedgeVertical
    {
        get => Physics2D.Raycast(LedgeCheckVertical.position, Vector2.down, wallCheckDistance, whatIsGround);
    }

    public bool Ground
    {
        get => Physics2D.OverlapCircle(GroundCheck.position, groundCheckRadius, whatIsGround);
    }

    public bool Trampoline
    {
        get => Physics2D.OverlapCircle(GroundCheck.position, groundCheckRadius, whatIsTrampoline);
    }

    public bool Ceiling
    {
        get => Physics2D.OverlapCircle(CeilingCheck.position, groundCheckRadius, whatIsGround);
    }

    public bool DashCorrection
    {
        get => !Physics2D.Raycast(cornerCorrectionCheck.position, Vector2.right * Movement.FacingDirection, wallCheckDistance, whatIsGround) &&
            Physics2D.Raycast(GroundCheck.position, Vector2.right * Movement.FacingDirection, wallCheckDistance, whatIsGround);
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
