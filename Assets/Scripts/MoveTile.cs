using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    forward, back, left, right
}
public class MoveTile : MonoBehaviour
{
    private Direction movementDirection;
    private Vector3 direction;
    

    public Direction MovementDirection
    {
        get
        {
            return movementDirection;
        }
        set
        {
            movementDirection = value;

            switch(movementDirection)
            {
                case Direction.back:
                    direction = Vector3.back;
                    break;
                case Direction.forward:
                    direction = Vector3.forward;
                    break;
                case Direction.left:
                    direction = Vector3.left;
                    break;
                case Direction.right:
                    direction = Vector3.right;
                    break;
            }
        }
    }

    public float speed; 
    public float distance; 
    
    void Start()
    {
        StartCoroutine(ChangeDirection());
    }
    
    void Update()
    {
        transform.position += (direction * speed) * Time.deltaTime;
    }

    private IEnumerator ChangeDirection()
    {
        //print("change direction 1 ");
        while (enabled)
        {
            yield return new WaitForSeconds((distance * 2) / speed);


            //print("change direction 2 " + movementDirection);
            switch (movementDirection)
            {
                case Direction.back:
                    MovementDirection = Direction.forward;
                    break;
                case Direction.forward:
                    MovementDirection = Direction.back;
                    break;
                case Direction.left:
                    MovementDirection = Direction.right;
                    break;
                case Direction.right:
                    MovementDirection = Direction.left;
                    break;
            }
            //print("change direction 3 " + movementDirection);
        }
    }
}
