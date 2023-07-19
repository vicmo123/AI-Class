using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEatPellet : PlayerBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        Node node = other.GetComponent<Node>();
        
        if (node != null && enabled && pacman.target == null)
        {
            // Pick a random available direction
            int index = Random.Range(0, node.availableDirections.Count);

            // Prefer not to go back the same direction so increment the index to
            // the next available direction
            if (node.availableDirections.Count > 1 && node.availableDirections[index] == -pacman.movement.direction)
            {
                index++;

                // Wrap the index back around if overflowed
                if (index >= node.availableDirections.Count)
                {
                    index = 0;
                }
            }

            pacman.movement.SetDirection(node.availableDirections[index]);
        }
    }
}
