using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MotorWheel : BasePart
{
    public float driveForce = 11f;
    public LayerMask groundMask;

    private Rigidbody2D rb;
    public CircleCollider2D col;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (!active)
            return;

        float radius = col.radius * transform.lossyScale.x;

        RaycastHit2D hit = Physics2D.Raycast(
            col.transform.position,
            Vector2.down,
            radius + 0.15f,
            groundMask
        );
        
        if (!hit)
            return;
        
        // Surface tangent (Bad Piggies-style)
        Vector2 normal = hit.normal;
        Vector2 tangent = new Vector2(normal.y, -normal.x);

        float speed = Vector2.Dot(rb.linearVelocity, tangent);
        
        Debug.DrawRay(col.transform.position, Vector2.down * (col.radius + 0.15f), Color.red);
        
        Debug.Log("Speed: " + speed);

        if (speed < contraption.GetEnginePower() * 4)
        {
            rb.AddForceAtPosition(
                tangent * 50.0f * contraption.GetEnginePower(),
                hit.point,
                ForceMode2D.Force
            );
        }

        
    }
}