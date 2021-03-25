using System;
using System.IO;
using UnityEngine;

namespace LiteNinja.GameSystems.Platformer
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Controller2D : MonoBehaviour
    {
        [Header("Raycasting")] 
        [SerializeField] private float skinWidth = 0.015f;
        [SerializeField] private int horizontalRayCount = 4;
        [SerializeField] private int verticalRayCount = 4;

        [Header("Collisions")] 
        [SerializeField] private LayerMask collisionMask;

        private BoxCollider2D collider;
        private RaycastOrigins raycastOrigins;
        private float horizontalRaySpacing;
        private float verticalRaySpacing;

        private void Start()
        {
            collider = GetComponent<BoxCollider2D>();
            CalculateRaySpacing();
        }

        private void UpdateRaycastOrigins()
        {
            var bounds = collider.bounds;
            bounds.Expand(-2f * skinWidth);
            raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
            raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
            raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
            raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
        }

        private void CalculateRaySpacing()
        {
            var bounds = collider.bounds;
            bounds.Expand(-2f * skinWidth);

            horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
            verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

            horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
            verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
        }

        private void VerticalCollisions(ref Vector3 velocity)
        {
            var directionY = Mathf.Sign(velocity.y);
            var rayLength = Mathf.Abs(velocity.y) + skinWidth;
            for (var i = 0; i < verticalRayCount; i++)
            {
                var rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
                var hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
                Debug.DrawRay(rayOrigin, Vector2.up * (rayLength * directionY), Color.red);

                if (hit)
                {
                    velocity.y = (hit.distance - skinWidth) * directionY;
                    rayLength = hit.distance;
                }
             }
        }
        
        private void HorizontalCollisions(ref Vector3 velocity)
        {
            var directionX = Mathf.Sign(velocity.x);
            var rayLength = Mathf.Abs(velocity.x) + skinWidth;
            for (var i = 0; i < horizontalRayCount; i++)
            {
                var rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                var hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
                Debug.DrawRay(rayOrigin, Vector2.right * (rayLength * directionX), Color.red);

                if (hit)
                {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;
                }
            }
        }

        public void Move(Vector3 velocity)
        {
            UpdateRaycastOrigins();
            if (velocity.x != 0)
            {
                HorizontalCollisions(ref velocity);
            }

            if (velocity.y != 0)
            {
                VerticalCollisions(ref velocity);
            }

            transform.Translate(velocity);
        }

        private struct RaycastOrigins
        {
            public Vector2 topLeft;
            public Vector2 topRight;
            public Vector2 bottomLeft;
            public Vector2 bottomRight;
        }
    }
}