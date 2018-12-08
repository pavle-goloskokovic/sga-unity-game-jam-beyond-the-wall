using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[Serializable]
public struct MovementSpeed : IComponentData {
    public float Value;
    public float drag;
    public Vector2 Speed;
}

public class MovementSpeedComponent : ComponentDataWrapper<MovementSpeed> { }


