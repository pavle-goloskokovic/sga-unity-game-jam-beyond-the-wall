using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[Serializable]
public struct IceMovement : IComponentData {
    public int index;
    public float strength;
}

public class IceMovementComponent : ComponentDataWrapper<IceMovement> { }


