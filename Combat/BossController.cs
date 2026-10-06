using UnityEngine;

public class BossController : MonoBehaviour
{
    [SerializeField] private float maxHealth = 450f;
    [SerializeField] private float maxShield = 150f;
    [SerializeField] private float vulnerableSeconds = 3f;
    [SerializeField] private float moveSpeed = 1.15f;
    [SerializeField] private float stopDistance = 3.5f;
    [SerializeField] private float turnSpeed = 120f;
    [SerializeField] private float pulseDamage = 12f;
    [SerializeField] private float pulseRadius = 8f;
    [SerializeField] private float pulseInterval = 3.2f;
    [SerializeField] private Renderer bodyRenderer;
    [SerializeField] private Collider bodyCollider;
    [SerializeField] private GameObject shieldVisual;

    private float health;
    private float shield;
    private float vulnerableUntil;
    private float nextPulse;
    private bool activated;
    private bool defeated;
    private PlayerController player;

    public bool IsDefeated => defeated;
    public bool IsActive => activated && !defeated;
    public float HealthNormalized => maxHealth <= 0f ? 0f : health / maxHealth;
    public float ShieldNormalized => maxShield <= 0f ? 0f : shield / maxShield;
    public bool ShieldIsUp => shield > 0f;

    private void Awake()
    {
        health = maxHealth;
        shield = maxShield;
        player = FindFirstObjectByType<PlayerController>();
        if (bodyRenderer) bodyRenderer.enabled = activated;
        if (bodyCollider) bodyCollider.enabled = activated;
        if (shieldVisual) shieldVisual.SetActive(activated);
    }

    public void Activate()
    {
        if (activated || defeated) return;
        activated = true;
        health = maxHealth;
        shield = maxShield;
        nextPulse = Time.time + 2f;
        if (bodyRenderer) bodyRenderer.enabled = true;
        if (bodyCollider) bodyCollider.enabled = true;
        if (shieldVisual) shieldVisual.SetActive(true);
        GameManager.Instance?.ShowBossActivated();
    }

    private void Update()
    {
        if (!IsActive) return;
        if (shield <= 0f && Time.time >= vulnerableUntil) RechargeShield();
        if (!player || player.Health <= 0f) return;

        Vector3 flatToPlayer = player.transform.position - transform.position;
        flatToPlayer.y = 0f;
        float distance = flatToPlayer.magnitude;
        if (distance > stopDistance)
        {
            Vector3 direction = flatToPlayer / distance;
            transform.position += direction * moveSpeed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), turnSpeed * Time.deltaTime);
        }

        if (Time.time >= nextPulse && distance <= pulseRadius)
        {
            player.TakeDamage(pulseDamage);
            GameManager.Instance?.ShowBossPulse();
            nextPulse = Time.time + pulseInterval;
        }
    }

    public bool TakeDamage(float amount, bool fromEcho)
    {
        if (!IsActive) return false;
        if (shield > 0f)
        {
            if (!fromEcho) return false;
            shield = Mathf.Max(0f, shield - amount);
            if (shield <= 0f)
            {
                vulnerableUntil = Time.time + vulnerableSeconds;
                if (shieldVisual) shieldVisual.SetActive(false);
                GameManager.Instance?.BossShieldBroken();
            }
            return true;
        }

        if (fromEcho) return false;
        health = Mathf.Max(0f, health - amount);
        if (health <= 0f) Defeat();
        return true;
    }

    private void RechargeShield()
    {
        shield = maxShield;
        if (shieldVisual) shieldVisual.SetActive(true);
        GameManager.Instance?.BossShieldRecharged();
    }

    private void Defeat()
    {
        defeated = true;
        if (bodyRenderer) bodyRenderer.enabled = false;
        if (bodyCollider) bodyCollider.enabled = false;
        if (shieldVisual) shieldVisual.SetActive(false);
        GameManager.Instance?.BossDefeated();
    }
}
