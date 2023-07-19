using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color baseColor, offsetColor;
    [SerializeField] private SpriteRenderer renderer;
    
    public bool isObstacle { get; set; }

    private readonly int costObstacle = 10;
    private readonly int costNormal = 1;

    public static UnityEvent<Tile> mouseClickedEvent;
    public void Initialize(bool isOffset)
    {
        mouseClickedEvent = new UnityEvent<Tile>();

        renderer.color = isOffset ? offsetColor : baseColor;

        //to set opacity to Opaque
        renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 1);

        isObstacle = false;
    }

    public void SetAsObstacle()
    {
        this.isObstacle = true;
        SetColor(Color.gray);
    }

    public void SetColor(Color otherColor)
    {
        renderer.color = otherColor;

        //to set opacity to Opaque
        renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 1);
    }

    private void OnMouseDown()
    {
        mouseClickedEvent.Invoke(this.gameObject.GetComponent<Tile>());
        Debug.Log("Click");
    }
}