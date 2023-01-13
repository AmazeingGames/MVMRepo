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
    [SerializeField] float headRayCastNewYPosition1;
    [SerializeField] float bodyRayCastNewYPosition1;

    [SerializeField] float headRayCastNewYPosition2 = 3.85f;
    [SerializeField] float bodyRayCastNewYPosition2 = 3.45f;

    [SerializeField] bool shouldDrawLedgeRaycasts = true;
    [SerializeField] float ledgeRaycastLength;
    public float GetLedgeRaycastLength => ledgeRaycastLength;
    public Transform GetLedgeRaycastBodyOrigin => ledgeRaycastBodyOrigin;
    [SerializeField] Transform ledgeRaycastHeadOrigin; //3.85 yPos for accurate animation
    [SerializeField] Transform ledgeRaycastBodyOrigin; //3.45 yPos for accurate animation

    Vector3 directionFacing => playerGeneral.IsFacingRight ? Vector3.right : Vector3.left;

    PlayerGeneral playerGeneral;
    GameObject headRaycast;
    GameObject bodyRaycast;

    void Awake()
    {
        if (playerGeneral == null)
            playerGeneral = GetComponent<PlayerGeneral>();

        if (headRaycast == null)
            headRaycast = GameObject.FindGameObjectWithTag("headRaycast");
        if (bodyRaycast == null)
            bodyRaycast = GameObject.FindGameObjectWithTag("bodyRaycast");
    }
    // Start is called before the first frame update
    void Start()
    {
        SetLedgeRaycastYPositions(headRayCastNewYPosition1, bodyRayCastNewYPosition1);
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldDrawGroundRaycast)
            DrawGroundRaycast();

        if (shouldDrawLedgeRaycasts)
            DrawLedgeRaycast();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetLedgeRaycastYPositions(headRayCastNewYPosition1, bodyRayCastNewYPosition1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetLedgeRaycastYPositions(headRayCastNewYPosition2, bodyRayCastNewYPosition2);
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

    void DrawLedgeRaycast()
    {
        var color = Color.yellow;

        if (IsFacingLedge())
        {
            color = Color.cyan;
        }

        Debug.DrawRay(ledgeRaycastHeadOrigin.position, directionFacing * ledgeRaycastLength, color);
        Debug.DrawRay(ledgeRaycastBodyOrigin.position, directionFacing * ledgeRaycastLength, color);
    }

    void DrawGroundRaycast()
    {
        var color = Color.red;

        if (IsGrounded())
        {
            color = Color.blue;
        }

        Debug.DrawRay(groundRaycastOrigin.position, Vector2.down * groundRaycastLength, color); //
    }

    void SetLedgeRaycastYPositions(float head, float body)
    {
        if (headRaycast != null)
        {
            Debug.Log("Set raycast head position");
            headRaycast.transform.localPosition = new Vector2(headRaycast.transform.localPosition.x, head);
        }
        else
            Debug.Log("headRaycast is null");

        if (bodyRaycast != null)
        {
            Debug.Log("Set raycast body position");
            bodyRaycast.transform.localPosition = new Vector2(bodyRaycast.transform.localPosition.x, body);
        }
        else
            Debug.Log("bodyRaycast is null");

    }
}
