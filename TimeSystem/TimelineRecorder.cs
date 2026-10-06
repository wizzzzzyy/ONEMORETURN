using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(100)]
public class TimelineRecorder : MonoBehaviour
{
    [SerializeField] private float historySeconds = 6f;
    private readonly List<TimelineFrame> history = new List<TimelineFrame>(360);
    private PlayerController player;
    private bool fired, interacted, tookDamage;

    private void Awake() => player = GetComponent<PlayerController>();

    private void LateUpdate()
    {
        bool damageEvent = player && player.ConsumeDamageFlag();
        history.Add(new TimelineFrame
        {
            time = Time.time,
            position = transform.position,
            rotation = transform.rotation,
            aimDirection = player ? player.AimDirection : transform.forward,
            fired = fired,
            interacted = interacted,
            tookDamage = tookDamage || damageEvent
        });
        fired = interacted = tookDamage = false;
        float cutoff = Time.time - historySeconds;
        while (history.Count > 2 && history[1].time < cutoff) history.RemoveAt(0);
    }

    public void MarkShot() => fired = true;
    public void MarkInteraction() => interacted = true;
    public void MarkDamage() => tookDamage = true;

    public void BeginNewBranch()
    {
        history.Clear();
        history.Add(new TimelineFrame { time = Time.time, position = transform.position, rotation = transform.rotation, aimDirection = player ? player.AimDirection : transform.forward });
        fired = interacted = tookDamage = false;
    }

    public bool TryGetRecent(float seconds, out TimelineFrame[] frames)
    {
        float cutoff = Time.time - seconds;
        int start = 0;
        while (start < history.Count - 1 && history[start].time < cutoff) start++;
        int count = history.Count - start;
        if (count < 2) { frames = null; return false; }
        frames = history.GetRange(start, count).ToArray();
        return frames[frames.Length - 1].time - frames[0].time >= seconds * 0.75f;
    }
}
