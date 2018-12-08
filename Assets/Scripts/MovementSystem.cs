using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class MovementSystem : JobComponentSystem {

    [BurstCompileAttribute(Accuracy = Accuracy.Low)]
    struct MovementSystemJob : IJobProcessComponentData<Position, MovementSpeed>
    {
        public float dt;
        public float3 mousePos;
        public bool reverse;
        public bool explode;
        public bool boost;
        public bool sw;

        public void Execute(ref Position pos, ref MovementSpeed speed)
        {
            float3 a = pos.Value;
            Vector2 vec = new Vector2(mousePos.x - a.x, mousePos.y - a.y);

            float dist = vec.magnitude;
            if (dist > 2) {

            }

            float tmp = speed.Value;
            if (boost)
            {
                tmp *= 10;
            }
            float mag =  tmp / ((float)math.pow(dist, 2));

            if (mag > 0.05f)
                mag = 0.05f;


            if (dist < 0.5f && !explode)
            {
                speed.Speed *= 1 - speed.drag*3;
                mag /= 2f;
            }

            if (sw)
            {
                speed.Speed *= -1;
                mag *= -1;
            }
            if (explode)
            {
                mag *= -3;
            }

            vec.Normalize();
            vec *= mag;

            speed.Speed *= 1 - speed.drag;
            speed.Speed += vec;

            a.x += speed.Speed.x;
            a.y += speed.Speed.y;

            

            pos.Value = a;
        }
    }
    bool reverse = false;
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        Vector3 mousepos = Input.mousePosition;
        bool sw = false;
        if (Input.GetMouseButtonDown(1)) {
            sw = true;
            reverse = !reverse; 
        }
        bool explode = Input.GetMouseButtonDown(0);
        bool boost = Input.GetMouseButton(2);
        mousepos.z = -Camera.allCameras[0].transform.position.z;
        mousepos = Camera.allCameras[0].ScreenToWorldPoint(mousepos);
        var job = new MovementSystemJob() {
            dt = Time.deltaTime,
            mousePos = mousepos,
            reverse = reverse,
            explode = explode,
            boost = boost,
            sw = sw
        };
        return job.Schedule(this, inputDeps);
    }
}
