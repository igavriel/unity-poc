using UnityEngine;

public class BallMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 touchStart;
    private Vector2 touchEnd;

    void Update()
    {
        rb = GetComponent<Rigidbody2D>();
        Utils.AssertObjectNotNull(rb, "Rigidbody2D component not found on the GameObject.");

        HandleKeyboardInput();
        HandleSwipeInput();
    }

    void HandleKeyboardInput()
    {
        float moveX = Input.GetAxis("Horizontal"); // A/D or arrow keys
        MoveBall(moveX);
    }

    void MoveBall(float direction)
    {
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
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
}
