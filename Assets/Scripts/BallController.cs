using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rb;
    [SerializeField]
    private Transform paddle;

    [SerializeField]
    private float velocity = 1f;

    private Vector3 offset;
    private bool released = false;
    private bool releaseEvent = false;

    private void Start()
    {
        offset = transform.position - paddle.transform.position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !released)
        {
            released = true;
            releaseEvent = true;
        }

        if (!released)
        {
            transform.position = paddle.transform.position + offset;
        }
    }

    private void FixedUpdate()
    {
        if (releaseEvent)
        {
            releaseEvent = false;
            rb.velocity = Vector3.Normalize(-transform.position);
        }

        if (released)
        {
            rb.velocity = Vector3.Normalize(rb.velocity) * velocity;
        }
    }

    public void Reset()
    {
        released = false;
        rb.velocity = Vector2.zero;
    }
}
