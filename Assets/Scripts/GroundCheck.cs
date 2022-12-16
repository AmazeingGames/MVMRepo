using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] float groundRaycastLength;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform groundRaycastOrigin;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(groundRaycastOrigin.position, Vector2.down * groundRaycastLength, Color.red); //
    }

    public bool IsGrounded()
    {
        return Physics2D.Raycast(groundRaycastOrigin.position, Vector2.down, groundRaycastLength, groundLayer); //
    }
}
