using System;
using UnityEngine;

namespace LiteNinja.GameSystems.Platformer
{
    [RequireComponent(typeof(Controller2D))]
    public class Actor : MonoBehaviour
    {
        [Header("Actor Movement")] 
        [SerializeField] private float moveSpeed = 6;
        [SerializeField] private float gravity = -20f;

        private Vector3 velocity;
        private Controller2D controller;

        private void Start()
        {
            controller = GetComponent<Controller2D>();
        }

        private void Update()
        {
            var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            velocity.x = input.x * moveSpeed;
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
    }
}