using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public struct MovementSpeed : IComponentData {
    public float Value;
}

public class MovementSpeedComponent : ComponentDataWrapper<MovementSpeed> { }


