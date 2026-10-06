using UnityEngine;

public class TopDownCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 18f, -10f);
    [SerializeField] private float followSharpness = 6f;
    public void SetTarget(Transform value) => target = value;
    private void LateUpdate()
    {
        if (!target) return;
        Vector3 desired = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desired, 1f - Mathf.Exp(-followSharpness * Time.deltaTime));
        transform.rotation = Quaternion.LookRotation(target.position + Vector3.up * 0.25f - transform.position, Vector3.up);
    }
}
