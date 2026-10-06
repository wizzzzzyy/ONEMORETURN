using UnityEngine;
using UnityEngine.InputSystem;

public class RewindSystem : MonoBehaviour
{
    [SerializeField] private float rewindSeconds = 4f;
    [SerializeField] private int startingCharges = 1;
    [SerializeField] private int maxCharges = 3;
    [SerializeField] private float chargeRechargeSeconds = 30f;
    [SerializeField] private Material echoMaterial;
    private int charges;
    private float rechargeTimer;
    private TimelineRecorder recorder;
    private PlayerController player;
    private PlayerCombat combat;
    private EchoController currentEcho;
    private float lockedUntil;
    public int Charges => charges;
    public float RewindSeconds => rewindSeconds;
    public int MaxCharges => maxCharges;
    public float ChargeRechargeSeconds => chargeRechargeSeconds;
    public float SecondsUntilNextCharge => charges >= maxCharges ? 0f : Mathf.Max(0f, chargeRechargeSeconds - rechargeTimer);

    private void Awake()
    {
        charges = startingCharges;
        player = FindFirstObjectByType<PlayerController>();
        recorder = player ? player.GetComponent<TimelineRecorder>() : null;
        combat = player ? player.GetComponent<PlayerCombat>() : null;
    }

    private void Update()
    {
        if (charges < maxCharges)
        {
            rechargeTimer += Time.unscaledDeltaTime;
            if (rechargeTimer >= chargeRechargeSeconds)
            {
                rechargeTimer -= chargeRechargeSeconds;
                charges = Mathf.Min(maxCharges, charges + 1);
                GameManager.Instance?.ShowChargeRecharged();
                if (charges >= maxCharges) rechargeTimer = 0f;
            }
        }
        else rechargeTimer = 0f;

        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame) TryRewind();
    }

    public bool TryRewind()
    {
        if (Time.time < lockedUntil || charges <= 0 || !player || !recorder || !recorder.TryGetRecent(rewindSeconds, out TimelineFrame[] frames)) return false;
        charges--;
        if (currentEcho) Destroy(currentEcho.gameObject);
        player.RestoreTransform(frames[0].position, frames[0].rotation);
        recorder.BeginNewBranch();
        Time.timeScale = 0.15f;
        lockedUntil = Time.time + 0.28f;
        GameObject echoObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        echoObject.name = "Echo";
        echoObject.transform.localScale = Vector3.one * 0.88f;
        Collider collider = echoObject.GetComponent<Collider>();
        if (collider) { collider.enabled = false; Destroy(collider); }
        Renderer r = echoObject.GetComponent<Renderer>();
        if (echoMaterial && r) r.sharedMaterial = echoMaterial;
        currentEcho = echoObject.AddComponent<EchoController>();
        currentEcho.Begin(frames, combat);
        GameManager.Instance?.ShowEchoCreated();
        CancelInvoke(nameof(ResumeTime));
        Invoke(nameof(ResumeTime), 0.18f);
        return true;
    }

    private void ResumeTime() => Time.timeScale = 1f;
    public void AddCharge(int amount = 1)
    {
        charges = Mathf.Min(maxCharges, charges + amount);
        if (charges >= maxCharges) rechargeTimer = 0f;
    }
}

