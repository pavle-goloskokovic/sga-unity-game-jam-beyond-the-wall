using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class GameMenager : MonoBehaviour {
    EntityManager manager;
    public GameObject prefab;
	// Use this for initialization
	void Start () {
        manager = World.Active.GetOrCreateManager<EntityManager>();
        AddBalls(50000);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("space")) {
            AddBalls(10000);
        }
	}

    void AddBalls(int amount) {
        NativeArray<Entity> entities = new NativeArray<Entity>(amount, Allocator.Temp);

        manager.Instantiate(prefab, entities);

        for (int i = 0; i < amount; i++) {
            float x = UnityEngine.Random.Range(-5f, 5f);
            float y = UnityEngine.Random.Range(-5f, 5f);
            manager.SetComponentData(entities[i], new Position{ Value = new float3(x, y, 0)});
        }

        entities.Dispose();
    }
}
