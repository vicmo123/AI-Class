using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotFollowMouse2 : MonoBehaviour
{
    private Vector2 target;
    private Vector2 position;
    private Camera cam;

    float maxSpeed = 5;
    float maxAcceleration = 5;

    Vector2 v = Vector2.zero;

    Rigidbody2D rb;

    private void Start()
    {
        target = new Vector2(0.0f, 0.0f);
        position = gameObject.transform.position;

        cam = Camera.main;

        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    private Vector2 GoToV(Vector2 mouseTarget)
    {
        Vector2 A = gameObject.transform.position;
        Vector2 B = mouseTarget;

        Vector2 VTarget = (B - A) / Time.deltaTime; // line and direction of desired velocity
        VTarget = Vector2.ClampMagnitude(VTarget, maxSpeed);

        Vector2 deltaV = (VTarget - v) / Time.deltaTime; //	line and direction of desired acceleration
        Vector2 accTarget = deltaV;  //	actual acceleration to apply
        accTarget = Vector2.ClampMagnitude(accTarget, maxAcceleration);

        //	compute the new velocity
        v += accTarget * Time.deltaTime;
        
        //compute the new position
        A += v * Time.deltaTime;
        
        return A;
    }

    private void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        target = Camera.main.ScreenToWorldPoint(mousePos);

        rb.transform.position = GoToV(target);
    }
}

