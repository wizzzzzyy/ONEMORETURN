using UnityEngine;

public class ExtractionZone : MonoBehaviour
{
    [SerializeField] private float arrivalRadius = 1.65f;
    [SerializeField] private Color dormantColor = new Color(0.25f, 0.28f, 0.32f);
    [SerializeField] private Color activeColor = new Color(1f, 0.62f, 0.12f);
    [SerializeField] private Color unlockedColor = new Color(0.15f, 1f, 0.42f);
    private Renderer zoneRenderer;
    private PlayerController player;
    private bool activated;
    private bool unlocked;
    private bool completed;

    private void Awake()
    {
        zoneRenderer = GetComponentInChildren<Renderer>();
        player = FindFirstObjectByType<PlayerController>();
        SetColor(dormantColor);
    }

    public void Activate()
    {
        if (activated) return;
        activated = true;
        SetColor(activeColor);
    }

    public void Unlock()
    {
        if (!activated || unlocked) return;
        unlocked = true;
        SetColor(unlockedColor);
    }

    private void Update()
    {
        if (!activated || !unlocked || completed || GameManager.Instance == null || !player) return;
        if (Vector3.Distance(player.transform.position, transform.position) > arrivalRadius) return;
        completed = true;
        GameManager.Instance.CompleteExtraction();
    }

    private void SetColor(Color color)
    {
        if (zoneRenderer) zoneRenderer.material.color = color;
    }
}
