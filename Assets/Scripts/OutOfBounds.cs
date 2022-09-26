using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class OutOfBounds
{
    //Check if transform is withing bounds on next frame if not disable inputs.
    //IncrementMovement must send an increment per frame nor normalized inputs.
    //
    // Anyone using this script needs access to cameras orthographic size 
    // Gameobject.Orthographicsize
    // cameraAspectRatio = 16/9 or just 1.7777778f;
    // camera X is calculated as (cameraAspectRatio) * OrthographicSize 
    // y is = orthographic size
    // Send x and y as Vector2 xyBounds
    public static Vector2 CalculateMove(Transform _transform, Vector2 IncrementMovement, Vector2 xyBounds)
    {
        float xBounds = xyBounds.x;
        float yBounds = xyBounds.y; 
        if (Mathf.Abs(_transform.position.x + IncrementMovement.x) > xBounds)
        {
            IncrementMovement.x = (xBounds * Mathf.Sign(IncrementMovement.x)) - _transform.position.x;
        }
        if (Mathf.Abs(_transform.position.y + IncrementMovement.y) > yBounds)
        {
            IncrementMovement.y = (yBounds * Mathf.Sign(IncrementMovement.y)) - _transform.position.y;
        }
        return IncrementMovement;
    }
}
