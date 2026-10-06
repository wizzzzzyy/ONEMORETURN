using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float speed, damage, life;
    private bool fromEcho;
    private Vector3 direction;
    private Renderer renderComponent;

    public void Initialize(Vector3 heading, bool echo, Material material, float projectileSpeed = 18f, float projectileDamage = 25f)
    {
        direction = heading.normalized;
        fromEcho = echo;
        speed = projectileSpeed;
        damage = projectileDamage;
        life = 2.5f;
        if (material && renderComponent) renderComponent.sharedMaterial = material;
    }

    private void Awake()
    {
        renderComponent = GetComponent<Renderer>();
        SphereCollider col = GetComponent<SphereCollider>();
        if (!col) col = gameObject.AddComponent<SphereCollider>();
        col.isTrigger = true;
        Rigidbody body = GetComponent<Rigidbody>();
        if (!body) body = gameObject.AddComponent<Rigidbody>();
        body.isKinematic = true;
        body.useGravity = false;
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
        life -= Time.deltaTime;
        if (life <= 0f) Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        BossController boss = other.GetComponentInParent<BossController>();
        if (boss) { boss.TakeDamage(damage, fromEcho); Destroy(gameObject); return; }
        EnemyController enemy = other.GetComponentInParent<EnemyController>();
        if (enemy && enemy.TakeDamage(damage, fromEcho)) Destroy(gameObject);
    }
}
