using UnityEngine;

public class EchoController : MonoBehaviour
{
    private TimelineFrame[] frames;
    private PlayerCombat combat;
    private float elapsed;
    private int nextFrame;
    private int hitsRemaining = 12;
    private bool finished;
    private bool broken;

    public bool IsAvailable => !broken && gameObject.activeInHierarchy;

    public void Begin(TimelineFrame[] timeline, PlayerCombat sourceCombat)
    {
        frames = timeline;
        combat = sourceCombat;
        elapsed = 0f;
        nextFrame = 0;
        if (frames != null && frames.Length > 0) transform.SetPositionAndRotation(frames[0].position, frames[0].rotation);
    }

    private void Update()
    {
        if (finished || frames == null || frames.Length == 0) return;
        elapsed += Time.deltaTime;
        while (nextFrame < frames.Length && frames[nextFrame].time - frames[0].time <= elapsed)
        {
            TimelineFrame frame = frames[nextFrame++];
            transform.SetPositionAndRotation(frame.position, frame.rotation);
            Vector3 muzzle = frame.position + Vector3.up * 0.8f + frame.aimDirection.normalized * 0.65f;
            if (frame.fired && combat) combat.FireRecorded(muzzle, frame.aimDirection);
            if (frame.interacted)
            {
                ObjectiveTerminal terminal = ObjectiveTerminal.FindNearest(frame.position, 2.4f);
                if (terminal) terminal.Activate();
            }
        }
        if (nextFrame >= frames.Length)
        {
            finished = true;
            if (GameManager.Instance == null || !GameManager.Instance.ExtractionActive) Destroy(gameObject, 2.5f);
        }
    }

    public void TakeHit()
    {
        if (!IsAvailable) return;
        hitsRemaining--;
        if (hitsRemaining <= 0)
        {
            broken = true;
            Destroy(gameObject);
        }
    }

    public static EchoController FindNearest(Vector3 position, float maxDistance)
    {
        EchoController[] all = FindObjectsByType<EchoController>(FindObjectsSortMode.None);
        EchoController nearest = null;
        float best = maxDistance;
        foreach (EchoController echo in all)
        {
            if (!echo || !echo.IsAvailable) continue;
            float distance = Vector3.Distance(position, echo.transform.position);
            if (distance <= best) { best = distance; nearest = echo; }
        }
        return nearest;
    }
}




