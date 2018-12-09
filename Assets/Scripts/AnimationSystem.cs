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

public class AnimationSystem : JobComponentSystem {

    [Inject]
    EndFrameBarrier barrier;

    //[BurstCompileAttribute(Accuracy = Accuracy.Low)]
    struct MovementSystemJob : IJobProcessComponentDataWithEntity<MovementSpeed>
    {
        [NativeDisableUnsafePtrRestriction]
        public System.Runtime.InteropServices.GCHandle shared0;
        public System.Runtime.InteropServices.GCHandle shared1;
        public System.Runtime.InteropServices.GCHandle shared2;
        public System.Runtime.InteropServices.GCHandle shared3;
        public System.Runtime.InteropServices.GCHandle shared4;
        public System.Runtime.InteropServices.GCHandle shared5;
        public int frame;


        public EntityCommandBuffer.Concurrent cmd;

        unsafe public void Execute(Entity entity, int id, ref MovementSpeed speed)
        {
            switch ((entity.Index + frame) % 6) {
                case 0:
                    cmd.SetSharedComponent(id, entity, (MeshInstanceRenderer)shared0.Target);
                    break;
                case 1:
                    cmd.SetSharedComponent(id, entity, (MeshInstanceRenderer)shared1.Target);
                    break;
                case 2:
                    cmd.SetSharedComponent(id, entity, (MeshInstanceRenderer)shared2.Target);
                    break;
                case 3:
                    cmd.SetSharedComponent(id, entity, (MeshInstanceRenderer)shared3.Target);
                    break;
                case 4:
                    cmd.SetSharedComponent(id, entity, (MeshInstanceRenderer)shared4.Target);
                    break;
                case 5:
                    cmd.SetSharedComponent(id, entity, (MeshInstanceRenderer)shared5.Target);
                    break;
            }
            
        }
    }


    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {

        for (int i = 0; i < GameMenager.zombieBuckets.Length; i++)
        {
            GameMenager.zombieBuckets[i] = 0;
        }

        System.Runtime.InteropServices.GCHandle[] handles = new System.Runtime.InteropServices.GCHandle[6];

        for (int i = 0; i < 6; i++) {
            MeshInstanceRenderer renderer = new MeshInstanceRenderer{
                mesh=GameMenager.instance.meshes[i],
                material = GameMenager.instance.mat
            };
            handles[i] = System.Runtime.InteropServices.GCHandle.Alloc(renderer);
        }


        var job = new MovementSystemJob() {
            cmd = barrier.CreateCommandBuffer().ToConcurrent(),
            shared0 = handles[0],
            shared1 = handles[1],
            shared2 = handles[2],
            shared3 = handles[3],
            shared4 = handles[4],
            shared5 = handles[5],
            frame = GameMenager.currentAnimFrame
        };
        return job.Schedule(this, inputDeps);
    }
}
