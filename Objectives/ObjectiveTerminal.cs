using UnityEngine;

public class ObjectiveTerminal : MonoBehaviour
{
    private bool activated;
    public bool Activated => activated;

    public bool Activate()
    {
        if (activated) return false;
        activated = true;
        Renderer r = GetComponentInChildren<Renderer>();
        if (r) r.material.color = new Color(0.2f, 1f, 0.48f);
        GameManager.Instance?.ObjectiveActivated();
        return true;
    }

    public static ObjectiveTerminal FindNearest(Vector3 position, float maxDistance)
    {
        ObjectiveTerminal[] all = FindObjectsByType<ObjectiveTerminal>(FindObjectsSortMode.None);
        ObjectiveTerminal nearest = null;
        float best = maxDistance;
        foreach (ObjectiveTerminal terminal in all)
        {
            if (!terminal || terminal.activated) continue;
            float distance = Vector3.Distance(position, terminal.transform.position);
            if (distance <= best) { best = distance; nearest = terminal; }
        }
        return nearest;
    }
}
