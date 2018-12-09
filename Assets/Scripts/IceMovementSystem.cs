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

[UpdateAfter(typeof(MovementSystem))]
public class IceMovementSystem : JobComponentSystem {
    

    struct MovementSystemJob : IJobProcessComponentData<Position, IceMovement>
    {
        public float dt;
        [NativeDisableParallelForRestriction] [ReadOnly] public NativeArray<int> zombieBuckets;
        [NativeDisableParallelForRestriction] public NativeArray<int> iceStrengths;
        [NativeDisableParallelForRestriction] public NativeArray<int> iceStrengthsOld;
        public int maxZombies;

        //TO-DO pitaj za dependencies between systems
        unsafe public void Execute(ref Position pos, ref IceMovement ice)
        {
            iceStrengthsOld[ice.index] = iceStrengths[ice.index];

            if (iceStrengths[ice.index] < 0) {
                iceStrengths[ice.index] = 0;
            }

            iceStrengths[ice.index] += 2;
            iceStrengths[ice.index] = iceStrengths[ice.index]>1000?1000:iceStrengths[ice.index];

            if (iceStrengths[ice.index]>400 && zombieBuckets[ice.index] > maxZombies*ice.strength* iceStrengths[ice.index]/1000f) {

                iceStrengths[ice.index] = 0;
                List<int> list = GameMenager.GetIndex(ice.index);
                for (int i = 0; i < list.Count; i++) {
                    int* ptr = (int*)iceStrengths.GetUnsafePtr() + list[i];
                    Interlocked.Add(ref *ptr, -300);
                }

            }

        }
    }
    bool reverse = false;

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new MovementSystemJob() {
            dt = Time.deltaTime,
            zombieBuckets = GameMenager.zombieBuckets,
            iceStrengths = GameMenager.iceStrengths,
            iceStrengthsOld = GameMenager.iceStrengthsOld,
            maxZombies = GameMenager.maxZombies
        };

        return job.Schedule(this, inputDeps);
    }
}
