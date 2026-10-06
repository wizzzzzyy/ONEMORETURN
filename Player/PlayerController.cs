using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float turnSpeed = 720f;
    [SerializeField] private Camera aimCamera;
    private CharacterController controller;
    private Vector3 aimDirection = Vector3.forward;
    private TimelineRecorder recorder;
    private float health = 100f;
    private bool tookDamageThisFrame;
    public Vector3 AimDirection => aimDirection;
    public float Health => health;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        recorder = GetComponent<TimelineRecorder>();
        if (!aimCamera) aimCamera = Camera.main;
    }

    private void Update()
    {
        if (Keyboard.current == null) return;
        Vector2 input = Vector2.zero;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) input.x -= 1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) input.x += 1f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) input.y -= 1f;
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) input.y += 1f;
        Vector3 move = new Vector3(input.x, 0f, input.y).normalized;
        controller.Move(move * moveSpeed * Time.deltaTime);
        UpdateAim();
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            ObjectiveTerminal terminal = ObjectiveTerminal.FindNearest(transform.position, 2.4f);
            if (terminal && terminal.Activate()) recorder?.MarkInteraction();
        }
    }

    private void UpdateAim()
    {
        if (Mouse.current == null || !aimCamera) return;
        Ray ray = aimCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane ground = new Plane(Vector3.up, Vector3.zero);
        if (ground.Raycast(ray, out float distance))
        {
            Vector3 flat = ray.GetPoint(distance) - transform.position;
            flat.y = 0f;
            if (flat.sqrMagnitude > 0.01f) aimDirection = flat.normalized;
        }
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(aimDirection), turnSpeed * Time.deltaTime);
    }

    public void TakeDamage(float amount)
    {
        health = Mathf.Max(0f, health - amount);
        tookDamageThisFrame = true;
        recorder?.MarkDamage();
        if (health <= 0f) GameManager.Instance?.SetGameOver();
    }

    public bool ConsumeDamageFlag()
    {
        bool result = tookDamageThisFrame;
        tookDamageThisFrame = false;
        return result;
    }

    public void RestoreTransform(Vector3 position, Quaternion rotation)
    {
        controller.enabled = false;
        transform.SetPositionAndRotation(position, rotation);
        controller.enabled = true;
    }
}

