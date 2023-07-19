using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Tile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private BoxCollider2D colli;
    [SerializeField] private Sprite GrassSprite;
    [SerializeField] private Sprite DirtSprite;
    [SerializeField] private Sprite RockSprite;

    public bool isObstacle { get; set; }

    private readonly int costObstacle = 10;
    private readonly int costNormal = 1;

    public static UnityEvent<Tile> mouseClickedEvent;

    public void Initialize()
    {
        mouseClickedEvent = new UnityEvent<Tile>();

        renderer.sprite = GrassSprite;

        isObstacle = false;
    }

    public void SetAsObstacle()
    {
        this.isObstacle = true;
        renderer.sprite = RockSprite;
        this.colli.enabled = true;
        //Wall layer
        this.gameObject.layer = 7;
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

    public void SetAsPath()
    {
        renderer.sprite = DirtSprite;
    }
}