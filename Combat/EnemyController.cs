using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float maxHealth = 75f;
    [SerializeField] private float moveSpeed = 2.1f;
    [SerializeField] private float contactDamage = 10f;
    private float health;
    private float nextAttack;
    private PlayerController player;

    private void Awake() { health = maxHealth; player = FindFirstObjectByType<PlayerController>(); }

    private void Update()
    {
        if (!player || player.Health <= 0f) return;

        EchoController decoy = GameManager.Instance != null && GameManager.Instance.ExtractionActive
            ? EchoController.FindNearest(transform.position, 30f)
            : null;
        Vector3 targetPosition = decoy ? decoy.transform.position : player.transform.position;
        Vector3 toTarget = targetPosition - transform.position;
        toTarget.y = 0f;
        float distance = toTarget.magnitude;

        if (distance > 1.35f)
        {
            transform.position += toTarget.normalized * moveSpeed * Time.deltaTime;
            if (toTarget.sqrMagnitude > 0.01f) transform.rotation = Quaternion.LookRotation(toTarget);
        }
        else if (Time.time >= nextAttack)
        {
            if (decoy) decoy.TakeHit();
            else player.TakeDamage(contactDamage);
            nextAttack = Time.time + 1f;
        }
    }

    public bool TakeDamage(float amount, bool fromEcho)
    {
        health -= amount;
        transform.localScale = Vector3.one * (health > maxHealth * 0.5f ? 1f : 0.88f);
        if (health > 0f) return true;
        GameManager.Instance?.EnemyDefeated(fromEcho);
        Destroy(gameObject);
        return true;
    }
}
