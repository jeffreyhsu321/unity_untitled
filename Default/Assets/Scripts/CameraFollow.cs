using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{

    public Controller2D player;
    public float verticalOffset;
    public float lookAheadX;
    public float lookDamperX;
    public float verticalSmoothTime;
    public Vector2 focusAreaSize;
    public Collider2D collider;

    FocusArea focusArea;

    float currentLookAheadX;
    float targetLookAheadX;
    float lookAheadDirectionX;
    float smoothLookVelocityX;
    float smoothVelocityY;

    //assign through Inspector
    public int mapBoundsRight;
    public int mapBoundsLeft;
    public int mapBoundsBottom;
    public int mapBoundsTop;

    //Main Camera properties
    private float camWidth;
    private float camHeight;

    void Start()
    {
        collider = player.GetComponent<Collider2D>();
        focusArea = new FocusArea(collider.bounds, focusAreaSize);
    }

    /// <summary>
    /// actual moving of focus area, calculate look ahead
    /// </summary>
    void LateUpdate()
    {
        //calculates new center and velocity
        focusArea.Update(collider.bounds, Camera.main);

        //focus position is center plus the vertical offset from the ground
        Vector2 focusPosition = focusArea.center + Vector2.up * verticalOffset;

        //get look ahead direction
        if (focusArea.velocity.x != 0)
        {
            lookAheadDirectionX = Mathf.Sign(focusArea.velocity.x);
        }

        //look ahead
        targetLookAheadX = lookAheadDirectionX * lookAheadX;
        currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookDamperX);

        //moves the focus position
        focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);
        focusPosition += Vector2.right * currentLookAheadX;

        //get Main Camera properties
        camHeight = 2f * Camera.main.orthographicSize;
        camWidth = 2f * Camera.main.orthographicSize * Camera.main.aspect;

        //moves the main camera if it will be within bounds after moving
        if (focusPosition.x - (camWidth / 2) > mapBoundsLeft
            && focusPosition.x + (camWidth / 2) < mapBoundsRight
            && focusPosition.y - (camHeight / 2) > mapBoundsBottom
            && focusPosition.y + (camHeight / 2) < mapBoundsTop)
        {
            transform.position = (Vector3)focusPosition + Vector3.forward * -10;
        }

    }

    /// <summary>
    /// visualizes focus area with a red rectangle (for debug)
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, .5f);
        Gizmos.DrawCube(focusArea.center, focusAreaSize);
    }

    /// <summary>
    /// structure defining a focus area
    /// </summary>
    struct FocusArea
    {
        public Vector2 center;
        public Vector2 velocity;
        float top, bottom, left, right;

        public FocusArea(Bounds targetBounds, Vector2 size)
        {
            //the four walls of the focus area
            top = targetBounds.min.y + size.y;
            bottom = targetBounds.min.y;
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            
            //initialize
            velocity = Vector2.zero;
            center = new Vector2((left + right) / 2, (top + bottom) / 2);
        }

        /// <summary>
        /// move the focus area accordingly
        /// </summary>
        /// <param name="targetBounds"></param>
        public void Update(Bounds targetBounds, Camera cam)
        {

            //calculate delta x and delta y
            float shiftX = 0;
            if (targetBounds.min.x < left)
            {
                shiftX = targetBounds.min.x - left;
            }
            else if (targetBounds.max.x > right)
            {
                shiftX = targetBounds.max.x - right;
            }
            left += shiftX;
            right += shiftX;

            float shiftY = 0;
            if (targetBounds.min.y < bottom)
            {
                shiftY = targetBounds.min.y - bottom;
            }
            else if (targetBounds.max.y > top)
            {
                shiftY = targetBounds.max.y - top;
            }
            top += shiftY;
            bottom += shiftY;
            
            //assign new center
            center = new Vector2((left + right) / 2, (top + bottom) / 2);

            //assign velocity
            velocity = new Vector2(shiftX, shiftY);
        }
    }
}