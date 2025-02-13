using UnityEngine;

public class Attack : MonoBehaviour
{
    public int baseDamage = 10;
    public Vector2 knockback = Vector2.zero;

    Collider2D hitCollider;

    private void Awake()
    {
        hitCollider = GetComponent<Collider2D>();
        hitCollider.enabled = false;
    }

    private void Start()
    {
        // Prevent accidental hits before attack enables collider
        hitCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();

        if (damageable != null)
        {
            // Can hit and deal damage so deal damage
            int adjustedDamage = Mathf.RoundToInt(baseDamage);

            // Flip knockback depending on hit direction
            Vector2 directionToTarget = (collision.transform.position - transform.position).normalized;
            float xSign = Mathf.Sign(directionToTarget.x);
            Vector2 finalKnockback = new Vector2(knockback.x * xSign, knockback.y);

            damageable.Hit(adjustedDamage, finalKnockback);
        }
    }
}
