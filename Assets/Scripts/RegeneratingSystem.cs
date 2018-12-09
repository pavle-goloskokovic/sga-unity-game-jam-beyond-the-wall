using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(ShootingSystem))]
public class RegeneratingSystem : JobComponentSystem
{


    struct RegeneratingSystemJob : IJobProcessComponentData<Position, IceMovement>
    {
        [NativeDisableParallelForRestriction] public NativeArray<int> iceStrengths;
        public bool clicked;
        public Vector2 shootingPosition;

        //TO-DO pitaj za dependencies between systems
        unsafe public void Execute(ref Position pos,  ref IceMovement ice)
        {
            if (!clicked)
                return;

            float distance = Vector2.Distance(new Vector2(pos.Value.x, pos.Value.z), shootingPosition);
            if (distance > 4)
                return;
            int delta = (int)(100 / (1+distance/10));

            iceStrengths[ice.index] += delta;

        }
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        Vector3 endpoint = Vector3.zero;
        bool click = Input.GetMouseButton(1);
        if (click)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                endpoint = hit.point;
                // Do something with the object that was hit by the raycast.
            }
        }
        var job = new RegeneratingSystemJob()
        {
            clicked = click,
            shootingPosition = new Vector2(endpoint.x, endpoint.z),
            iceStrengths = GameMenager.iceStrengths

        };
        GameMenager.shoot = false;
        return job.Schedule(this, inputDeps);


    }
}

