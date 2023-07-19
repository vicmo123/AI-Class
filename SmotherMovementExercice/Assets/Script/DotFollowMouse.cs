using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotFollowMouse : MonoBehaviour
{
    [SerializeField] private Camera cam;
    Vector3 worldPosition;

    private void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        worldPosition = Camera.main.ScreenToWorldPoint(mousePos);
        
        this.gameObject.transform.position = Vector2.Lerp(this.gameObject.transform.position, worldPosition, 0.01f);
    }
}
