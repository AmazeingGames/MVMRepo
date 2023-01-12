using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] LayerMask groundLayer;

    [Header("Ground Check")]
    [SerializeField] bool shouldDrawGroundRaycast = true;
    [SerializeField] float groundRaycastLength;
    [SerializeField] Transform groundRaycastOrigin;

    [Header("Ledge Check")]
    [SerializeField] bool shouldDrawLedgeRaycasts = true;
    [SerializeField] float ledgeRaycastLength;
    public float GetLedgeRaycastLength => ledgeRaycastLength;
    public Transform GetLedgeRaycastBodyOrigin => ledgeRaycastBodyOrigin;
    [SerializeField] Transform ledgeRaycastHeadOrigin; //3.85 yPos for accurate animation
    [SerializeField] Transform ledgeRaycastBodyOrigin; //3.45 yPos for accurate animation

    Vector3 directionFacing => playerGeneral.IsFacingRight ? Vector3.right : Vector3.left;

    PlayerGeneral playerGeneral;

    void Awake()
    {
        if (playerGeneral == null)
            playerGeneral = GetComponent<PlayerGeneral>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (shouldDrawGroundRaycast)
            Debug.DrawRay(groundRaycastOrigin.position, Vector2.down * groundRaycastLength, Color.red); //

        if (shouldDrawLedgeRaycasts)
        {
            Debug.DrawRay(ledgeRaycastHeadOrigin.position, directionFacing * ledgeRaycastLength, Color.yellow);

            Debug.DrawRay(ledgeRaycastBodyOrigin.position, directionFacing * ledgeRaycastLength, Color.yellow);
        }
    }

    public bool IsGrounded()
    {
        return Physics2D.Raycast(groundRaycastOrigin.position, Vector2.down, groundRaycastLength, groundLayer); //
    }

    public bool IsFacingLedge()
    {
        if (!Physics2D.Raycast(ledgeRaycastHeadOrigin.position, directionFacing, ledgeRaycastLength, groundLayer) && Physics2D.Raycast(ledgeRaycastBodyOrigin.position, directionFacing, ledgeRaycastLength, groundLayer))
        {
            return true;
        }
        return false;
    }

    
}
