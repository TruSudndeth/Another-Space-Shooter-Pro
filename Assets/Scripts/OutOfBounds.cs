using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class OutOfBounds
{
    // Check if transform is withing bounds on next frame if not disable inputs.
    // must send an move increment per frame not normalized inputs.
    // ---------- Variables needed ------------
    // Vector2 _xyBounds = Vector2.zero;
    // float cameraAspectRatio = 16/9 or just 1.7777778f;
    // ------------Calculations----------------
    // _xyBounds.y = Camera.main.orthographicSize;
    // _xyBounds.x = _xyBounds.y* cameraAspecRatio;

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
