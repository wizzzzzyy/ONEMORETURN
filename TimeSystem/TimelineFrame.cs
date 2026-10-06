using System;
using UnityEngine;

[Serializable]
public struct TimelineFrame
{
    public float time;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 aimDirection;
    public bool fired;
    public bool interacted;
    public bool tookDamage;
}
