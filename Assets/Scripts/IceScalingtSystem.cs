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
public class IceScalingSystem : JobComponentSystem {
    

    struct MovementSystemJob : IJobProcessComponentData<Position, Scale, IceMovement>
    {
        [NativeDisableParallelForRestriction] public NativeArray<int> iceStrengths;
        [NativeDisableParallelForRestriction] public NativeArray<int> oldStrengths;

        //TO-DO pitaj za dependencies between systems
        unsafe public void Execute(ref Position pos, ref Scale scale, ref IceMovement ice)
        {

            float current = oldStrengths[ice.index] / 2000f;
            float wanted =  iceStrengths[ice.index] / 2000f;

            //if (iceStrengths[ice.index] < 500)
            //    wanted = 0;

            float a = 0.001f;

            scale.Value.y = wanted;

        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {

        var job = new MovementSystemJob() {
            iceStrengths = GameMenager.iceStrengths,
            oldStrengths = GameMenager.iceStrengthsOld
        };

        return job.Schedule(this, inputDeps);
    }
}
