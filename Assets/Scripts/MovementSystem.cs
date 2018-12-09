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

public class MovementSystem : JobComponentSystem {
    public NativeArray<int> pathMatrix;
    bool pathFindingFlag = false;
    //[BurstCompileAttribute(Accuracy = Accuracy.Low)]
    struct MovementSystemJob : IJobProcessComponentData<Position, Rotation, MovementSpeed>
    {
        [NativeDisableUnsafePtrRestriction]
        public float dt;
        [NativeDisableParallelForRestriction] public NativeArray<int> zombieBuckets;
        [NativeDisableParallelForRestriction] public NativeArray<int> iceStrengths;
        [NativeDisableParallelForRestriction] public NativeArray<int> pathMatrix;
        public bool pathFindingFlag;

        unsafe public void Execute(ref Position pos, ref Rotation rotation, ref MovementSpeed speed)
        {
            float3 a = pos.Value;

            int myIndex = GameMenager.GetIndex(a.x, a.z);
            int pathIndex = -1;
            if (myIndex != -1) {

            int curentDistance = pathMatrix[myIndex];
                List<int> neighbour = GameMenager.GetNeighbourPath(myIndex);
                foreach (var item in neighbour)
                {
                    if (pathMatrix[item] == curentDistance - 1) {
                        pathIndex = item;
                        break;
                    }
                }
            }

            Vector2 vel;

            if (pathIndex != -1 && pathFindingFlag) {
                int x = pathIndex % 100 - 50;
                int y = pathIndex / 100 - 50;

                vel = new Vector2(x-a.x, y-a.z);
            }
            else {
                vel = new Vector2(-a.x, -a.z);

            }

            //pomeri napred
            vel.Normalize();
            vel *= speed.Value * dt;
            a.x += vel.x;
            a.z += vel.y;


            rotation.Value = quaternion.LookRotation(new float3 { y = 1 }, new float3{ x=-vel.x, z=-vel.y});

            int index = GameMenager.GetIndex(a.x, a.z);

            if (index < 0)
            {
                pos.Value = a;
                return;
            }

            if (iceStrengths[index] <= 500)
            {
                //int* ptr2 = (int*)zombieBuckets.GetUnsafePtr() + index;
                //Interlocked.Increment(ref *ptr2);
                vel.Normalize();
                vel *= -1;
                a.x = vel.x * 80;
                a.z = vel.y * 80;
                pos.Value = a;
                return;
            }

            int* ptr = (int*)zombieBuckets.GetUnsafePtr()+index;
            Interlocked.Increment(ref *ptr);



            pos.Value = a;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {

        for (int i = 0; i < GameMenager.zombieBuckets.Length; i++)
        {
            GameMenager.zombieBuckets[i] = 0;
        }
        pathMatrix = GameMenager.pathMatrix;

        if (Input.GetKeyDown("space")) {
            pathFindingFlag = !pathFindingFlag;
        }
        if (pathFindingFlag)
            CalculatePath();

        var job = new MovementSystemJob() {
            dt = Time.deltaTime,
            zombieBuckets = GameMenager.zombieBuckets,
            iceStrengths = GameMenager.iceStrengths,
            pathMatrix = pathMatrix,
            pathFindingFlag = pathFindingFlag
        };
        return job.Schedule(this, inputDeps);
    }

    void CalculatePath()
    {
        
        for (int i = 0; i < pathMatrix.Length; i++)
        {
            pathMatrix[i] = -1;
        }
        List<int> neighbours = new List<int>();
        int firstIndex = GameMenager.GetIndex(0, 0);
        neighbours.Add(firstIndex);
        pathMatrix[firstIndex] = 0;
        bool a = true;
        while (neighbours.Count > 0)
        {
            int index = neighbours[0];
            List<int> newNeighbours = GameMenager.GetNeighbourPath(index);
            foreach (var item in newNeighbours)
            {
                if (item>=10000 || item<0 ||pathMatrix[item] != -1)
                    continue;
                if (GameMenager.iceStrengths[item] < 500)
                    continue;
                pathMatrix[item] = pathMatrix[index] + 1;
                if (a)
                    pathMatrix[item]--;
                neighbours.Add(item);
            }
            a = false;
            neighbours.RemoveAt(0);
        }
    }
}
