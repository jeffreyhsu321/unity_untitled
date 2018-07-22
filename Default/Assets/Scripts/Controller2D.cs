using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour {

    public LayerMask collisionMask;

    const float skinWidth = 0.015f;

    public int horizontalRayCount = 16;
    public int verticalRayCount = 4;
    float horizontalSpacing;
    float verticalSpacing;

    BoxCollider2D collider;
    RaycastOrigins raycastOrigins;
    public CollisionInfo collisions;

    float maxSlopeAngle = 45;

    public virtual void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
    }

    void Start() {
        CalculateRaySpacing();
    }

    struct RaycastOrigins {
        public Vector2 topLeft;
        public Vector2 topRight;
        public Vector2 bottomLeft;
        public Vector2 bottomRight;
    }

    public struct CollisionInfo {
        public bool above, below, left, right;
        public bool isClimbingSlope, isDescendingSlope, isSlidingDownSlope;
        public float slopeAngle, slopeAngleOld;
        public Vector3 velocityOld;
        public Vector2 slopeNormal;

        public void Reset() {
            above = false;
            below = false;
            left = false;
            right = false;

            isClimbingSlope = false;
            isDescendingSlope = false;
            isSlidingDownSlope = false;
            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
            slopeNormal = Vector2.zero;
        }
    }

    void UpdateRaycastOrigins() {
        Bounds bounds = collider.bounds;
        raycastOrigins.topLeft = new Vector2(bounds.min.x + skinWidth, bounds.max.y - skinWidth);
        raycastOrigins.topRight = new Vector2(bounds.max.x - skinWidth, bounds.max.y - skinWidth);
        raycastOrigins.bottomLeft = new Vector2(bounds.min.x + skinWidth, bounds.min.y + skinWidth);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x - skinWidth, bounds.min.y + skinWidth);
    }

    public void CalculateRaySpacing() {
        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);
        horizontalSpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalSpacing = bounds.size.x / (verticalRayCount - 1);
    }

    void VerticalCollision(ref Vector3 velocity) {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        Vector2 rayOrigins = (directionY == 1) ? raycastOrigins.topLeft : raycastOrigins.bottomLeft;

        for (int i = 0; i < verticalRayCount; i++) {
            rayOrigins = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigins + Vector2.right * verticalSpacing * i, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigins + Vector2.right * verticalSpacing * i, Vector2.up * directionY * rayLength, Color.magenta);

            if (hit) {

                velocity.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                if (collisions.isClimbingSlope) {
                    velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad * Mathf.Sign(velocity.x));
                }

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }
        }

        if (collisions.isClimbingSlope)
        {
            float directionX = Mathf.Sign(velocity.x);
            rayLength = Mathf.Abs(velocity.x) + skinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != collisions.slopeAngle)
                {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                    collisions.slopeNormal = hit.normal;
                }
            }
        }
    }

    void HorizontalCollision(ref Vector3 velocity) {
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        Vector2 rayOrigins;
        if (directionX == 1) {
            rayOrigins = raycastOrigins.bottomRight;
        } else {
            rayOrigins = raycastOrigins.bottomLeft;
        }

        for (int i = 0; i < horizontalRayCount; i++) {
            RaycastHit2D hit = Physics2D.Raycast(rayOrigins + Vector2.up * horizontalSpacing * i, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigins + Vector2.up * horizontalSpacing * i, Vector2.right * directionX * rayLength, Color.magenta);

            if (hit) {

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (i == 0 && slopeAngle <= maxSlopeAngle)
                {
                    if (collisions.isDescendingSlope)
                    {
                        collisions.isDescendingSlope = false;
                        velocity = collisions.velocityOld;
                    }
                    float distanceToSlopeBase = 0;
                    if (slopeAngle != collisions.slopeAngleOld)
                    {
                        distanceToSlopeBase = hit.distance - skinWidth;
                        velocity.x -= distanceToSlopeBase * directionX;
                    }
                    ClimbSlope(ref velocity, slopeAngle, hit.normal);
                    velocity.x += distanceToSlopeBase * directionX;
                }

                if (!collisions.isClimbingSlope || slopeAngle > maxSlopeAngle)
                {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;

                    if (collisions.isClimbingSlope) {
                        velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad * Mathf.Abs(velocity.x));
                    }

                    if (directionX == 1) { collisions.right = true; }
                    if (directionX == -1) { collisions.left = true; }
                }
            }
        }
    }

    void ClimbSlope(ref Vector3 velocity, float slopeAngle, Vector2 slopeNormal) {
        float moveDistance = Mathf.Abs(velocity.x);
        float climbVelocityY = moveDistance * Mathf.Sin(slopeAngle * Mathf.Deg2Rad);

        if (velocity.y <= climbVelocityY)
        {
            velocity.y = climbVelocityY;
            velocity.x = moveDistance * Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);

            collisions.below = true; //assume standing on ground to jump on slopes
            collisions.isClimbingSlope = true;
            collisions.slopeAngle = slopeAngle;
            collisions.slopeNormal =slopeNormal;
        }
    }

    void DescendSlope(ref Vector3 velocity) {

        RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast(raycastOrigins.bottomLeft, Vector2.down, Mathf.Abs(velocity.y) + skinWidth, collisionMask);
        RaycastHit2D maxSlopeHitRight = Physics2D.Raycast(raycastOrigins.bottomRight, Vector2.down, Mathf.Abs(velocity.y) + skinWidth, collisionMask);
        if (maxSlopeHitLeft ^ maxSlopeHitRight)
        {
            SlideDownSlope(maxSlopeHitLeft, ref velocity);
            SlideDownSlope(maxSlopeHitRight, ref velocity);
        }

        if (!collisions.isSlidingDownSlope)
        {
            float directionX = Mathf.Sign(velocity.x);
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle)
                {
                    if (Mathf.Sign(hit.normal.x) == directionX)
                    {
                        if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
                        {
                            float moveDistance = Mathf.Abs(velocity.x);
                            float descendVelocityY = moveDistance * Mathf.Sin(slopeAngle * Mathf.Deg2Rad);
                            velocity.x = moveDistance * Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                            velocity.y -= descendVelocityY;

                            collisions.slopeAngle = slopeAngle;
                            collisions.isDescendingSlope = true;
                            collisions.below = true;
                            collisions.slopeNormal = hit.normal;
                        }
                    }
                }
            }
        }
    }

    void SlideDownSlope(RaycastHit2D hit, ref Vector3 velocity) {
        if (hit) {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle > maxSlopeAngle) {
                velocity.x = Mathf.Sign(hit.normal.x) * ((Mathf.Abs(velocity.y) - hit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad));

                collisions.slopeAngle = slopeAngle;
                collisions.isSlidingDownSlope = true;
                collisions.slopeNormal = hit.normal;
            }
        }
    }

    public void Move(Vector3 velocity) {
        UpdateRaycastOrigins();
        collisions.Reset();
        collisions.velocityOld = velocity;

        if (velocity.y < 0) {
            DescendSlope(ref velocity);
        }

        if (velocity.x != 0) {
            HorizontalCollision(ref velocity);
        }

        if (velocity.y != 0) {
            VerticalCollision(ref velocity);
        }
        transform.Translate(velocity);
    }
}