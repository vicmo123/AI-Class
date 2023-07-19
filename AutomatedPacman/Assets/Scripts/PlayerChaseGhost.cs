using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChaseGhost : PlayerBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        Node node = other.GetComponent<Node>();

        if (node != null && enabled)
        {
            Vector2 direction = Vector2.zero;
            float minDistance = float.MaxValue;

            // Find the available direction that moves closet to pacman
            foreach (Vector2 availableDirection in node.availableDirections)
            {
                // If the distance in this direction is less than the current
                // min distance then this direction becomes the new closest
                Vector3 newPosition = gameObject.transform.position + new Vector3(availableDirection.x, availableDirection.y);
                float distance = (pacman.target.position - newPosition).sqrMagnitude;

                if (distance < minDistance)
                {
                    direction = availableDirection;
                    minDistance = distance;
                }
            }
            pacman.movement.SetDirection(direction);
        }
    }
}
