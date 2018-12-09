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


[UpdateAfter(typeof(IceMovementSystem))]
public class ShootingSystem : JobComponentSystem
{


    struct MovementSystemJob : IJobProcessComponentData<Position, IceMovement>
    {
        public bool clicked;
        [NativeDisableParallelForRestriction] public NativeArray<int> iceStrengths;
        public Vector2 shootingPosition;

        //TO-DO pitaj za dependencies between systems
        unsafe public void Execute(ref Position pos,  ref IceMovement ice)
        {
            if (!clicked)
                return;

            float distance = Vector2.Distance(new Vector2(pos.Value.x, pos.Value.z), shootingPosition);
            if (distance > 4)
                return;
            int delta = (int)(1000 / (1+distance/10));

            iceStrengths[ice.index] -= delta;

        }
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new MovementSystemJob()
        {
            clicked = GameMenager.shoot,
            shootingPosition = new Vector2(GameMenager.hitPoint.x, GameMenager.hitPoint.z),
            iceStrengths = GameMenager.iceStrengths

        };
        GameMenager.shoot = false;
        return job.Schedule(this, inputDeps);


    }
}

