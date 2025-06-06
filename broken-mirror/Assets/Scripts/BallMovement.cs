using UnityEngine;

public class BallMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float moveBoundary = 3f;

    private Vector2 touchStart;
    private Vector2 touchEnd;

    void Update()
    {
        HandleKeyboardInput();
        HandleSwipeInput();
    }

    void HandleKeyboardInput()
    {
        float moveX = Input.GetAxis("Horizontal"); // A/D or arrow keys
        MoveBall(moveX);
    }

    void HandleSwipeInput()
    {
        // Touch input (real devices)
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            HandleSwipe(touch.phase, touch.position);
        }
        // Mouse-as-touch (for Editor or WebGL desktop)
        else if (Application.isEditor || Application.platform == RuntimePlatform.WebGLPlayer)
        {
            if (Input.GetMouseButtonDown(0))
            {
                touchStart = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                touchEnd = Input.mousePosition;
                float swipeDelta = touchEnd.x - touchStart.x;

                if (Mathf.Abs(swipeDelta) > 50f)
                {
                    float direction = swipeDelta > 0 ? 1f : -1f;
                    MoveBall(direction);
                }
            }
        }
    }

    void HandleSwipe(TouchPhase phase, Vector2 position)
    {
        if (phase == TouchPhase.Began)
        {
            touchStart = position;
        }
        else if (phase == TouchPhase.Ended)
        {
            touchEnd = position;
            float swipeDelta = touchEnd.x - touchStart.x;

            if (Mathf.Abs(swipeDelta) > 50f)
            {
                float direction = swipeDelta > 0 ? 1f : -1f;
                MoveBall(direction);
            }
        }
    }

    void MoveBall(float direction)
    {
        Vector3 newPos = transform.position + Vector3.right * direction * moveSpeed * Time.deltaTime;
        newPos.x = Mathf.Clamp(newPos.x, -moveBoundary, moveBoundary);
        transform.position = newPos;
    }
}
