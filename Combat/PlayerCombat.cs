using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private Transform muzzle;
    [SerializeField] private Material playerShotMaterial;
    [SerializeField] private Material echoShotMaterial;
    [SerializeField] private float fireInterval = 0.24f;
    private float nextShot;
    private TimelineRecorder recorder;
    private PlayerController player;

    private void Awake()
    {
        recorder = GetComponent<TimelineRecorder>();
        player = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.isPressed && Time.time >= nextShot)
        {
            Fire(player ? player.AimDirection : transform.forward, false);
            recorder?.MarkShot();
            nextShot = Time.time + fireInterval;
        }
    }

    public void FireRecorded(Vector3 origin, Vector3 direction) => Fire(direction, true, origin);

    private void Fire(Vector3 direction, bool echo, Vector3? originOverride = null)
    {
        Vector3 flatDirection = direction.sqrMagnitude > 0.001f ? direction.normalized : transform.forward;
        Vector3 origin = originOverride ?? (muzzle ? muzzle.position : transform.position + Vector3.up * 0.8f + flatDirection * 0.65f);
        GameObject shot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        shot.name = echo ? "Echo Shot" : "Player Shot";
        shot.transform.position = origin;
        shot.transform.localScale = Vector3.one * 0.22f;
        Projectile projectile = shot.AddComponent<Projectile>();
        projectile.Initialize(flatDirection, echo, echo ? echoShotMaterial : playerShotMaterial, 19f, echo ? 25f : 32f);
    }
}
