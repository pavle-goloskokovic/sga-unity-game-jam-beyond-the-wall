using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class GameMenager : MonoBehaviour {
    EntityManager manager;
    public GameObject zombiePrefab;
    public GameObject icePrefab;
    public static NativeArray<int> zombieBuckets;
    public static NativeArray<int> iceStrengths;
    public static NativeArray<int> iceStrengthsOld;
    public static NativeArray<int> pathMatrix;
    
    public static int maxZombies = 3;
    public Mesh[] meshes;
    public Mesh mesh;
    public Material mat;
    public static Vector3 hitPoint;
    public static Vector3 startPosition;
    public static Vector3 endPosition;
    public static bool shoot = false;

    public GameObject explosion;

    public static GameMenager instance;

    // Use this for initialization
    void Start() {
        instance = this;
        manager = World.Active.GetOrCreateManager<EntityManager>();
        zombieBuckets = new NativeArray<int>(10000, Allocator.Persistent);
        iceStrengths = new NativeArray<int>(10000, Allocator.Persistent);
        iceStrengthsOld = new NativeArray<int>(10000, Allocator.Persistent);
        pathMatrix = new NativeArray<int>(10000, Allocator.Persistent);

        StartCoroutine(Spawner());
        StartCoroutine(Animation());
        for (int i = 0; i < GameMenager.zombieBuckets.Length; i++)
        {
            iceStrengths[i] = 1000;
        }
        AddIce();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                startPosition = hit.point;
                // Do something with the object that was hit by the raycast.
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                endPosition = hit.point;
                StartCoroutine(ShootFire());
                // Do something with the object that was hit by the raycast.
            }
        }
    }

    IEnumerator ShootFire() {

        float distance = Vector3.Distance(startPosition, endPosition);
        float tmp = 0;
        if (distance == 0) {
            hitPoint = startPosition;
            GameObject ex = Instantiate(explosion, hitPoint, Quaternion.identity);
            Destroy(ex, 5);
            shoot = true;
        }
        while (tmp < distance) {
            hitPoint = Vector3.Lerp(startPosition, endPosition, tmp/distance);
            GameObject ex = Instantiate(explosion, hitPoint, Quaternion.identity);
            Destroy(ex, 5);
            shoot = true;
            yield return new WaitForSeconds(0.08f);
            tmp += 3;
        }
    }

    public static int currentAnimFrame = 0;
    IEnumerator Animation()
    {
        while (true)
        {
            mesh = meshes[currentAnimFrame];
            currentAnimFrame++;
            currentAnimFrame %= meshes.Length;
            yield return new WaitForSeconds(0.1f);
        }


    }

    IEnumerator Spawner()
    {
        int counter = 0;
        int increment = 200;
        while (counter <= 10000)
        {
            AddBalls(increment);
            counter += increment;
            yield return new WaitForSeconds(0.5f);
        }


    }

    public static List<int> GetIndex(int index)
    {
        int x = index % 100;
        int y = index / 100;

        List<int> list = new List<int>();

        if (x - 1 >= 0)
            list.Add(x - 1 + y * 100);
        if (x + 1 < 100)
            list.Add(x + 1 + y * 100);
        if (y - 1 >= 0)
            list.Add(x + (y-1) * 100);
        if (y + 1 <100)
            list.Add(x + (y+1) * 100);

        if (x - 1 >= 0 && y - 1 >= 0)
            list.Add(x - 1 + (y - 1) * 100);
        if (x + 1 < 100 && y + 1 < 100)
            list.Add(x + 1 + (y + 1) * 100);
        if (y - 1 >= 0 && x + 1 < 100)
            list.Add(x + 1 + (y - 1) * 100);
        if (y + 1 < 100 && x - 1 >= 0)
            list.Add(x - 1 + (y + 1) * 100);


        return list;
    }

    public static List<int> GetNeighbourPath(int index)
    {
        int x = index % 100;
        int y = index / 100;

        List<int> list = new List<int>();

        if (x - 1 >= 0)
            list.Add(x - 1 + y * 100);
        if (x + 1 < 100)
            list.Add(x + 1 + y * 100);
        if (y - 1 >= 0)
            list.Add(x + (y - 1) * 100);
        if (y + 1 < 100)
            list.Add(x + (y + 1) * 100);

        return list;
    }

    public static int GetIndex(float x, float y)
    {
        if (x < -50 || x > 50 || y < -50 || y > 50)
        {
            return -1;
        }
        int ret = (int)x + 50 + ((int)y + 50) * 100;
        if (ret < 0 || ret >= 10000)
            ret = -1;
        return ret;
    }

    void AddBalls(int amount) {
        NativeArray<Entity> entities = new NativeArray<Entity>(amount, Allocator.Temp);

        manager.Instantiate(zombiePrefab, entities);

        for (int i = 0; i < amount; i++) {
            float angle = UnityEngine.Random.Range(0, (float)math.PI * 2);
            float x = math.cos(angle) * 60;
            float y = math.sin(angle) * 60;
            manager.SetComponentData(entities[i], new Position{ Value = new float3(x, 1, y)});
            manager.SetComponentData(entities[i], new MovementSpeed { Value = 2f * UnityEngine.Random.Range(0.7f, 1.4f) });
        }

        entities.Dispose();
    }

    private void OnDrawGizmos()
    {
        /*
        for (int i = 0; i < zombieBuckets.Length; i++)
        {
            float x = i % 100 - 50;
            float y = i / 100 - 50;
            Gizmos.DrawCube(new Vector3(x, 0, y), new Vector3(1, zombieBuckets[i], 1));
        }*/
    }



    void AddIce()
    {
        NativeArray<Entity> entities = new NativeArray<Entity>(10000, Allocator.Temp);

        manager.Instantiate(icePrefab, entities);

        for (int i = 0; i < 10000; i++)
        {
            float x = i % 100 - 50;
            float y = i / 100 - 50;
            //dodaj promenu strengtha u odnosu na distance od centra
            float distance = Vector2.Distance(new Vector2(x, y), Vector2.zero);
            float strength = 1 + distance / 25;

            manager.SetComponentData(entities[i], new Position { Value = new float3(x, 0, y) });
            manager.SetComponentData(entities[i], new IceMovement { index = i, strength = strength });
            manager.SetComponentData(entities[i], new Rotation { Value = quaternion.RotateY(UnityEngine.Random.Range(0, 90)) });
        }

        entities.Dispose();
    }

    private void OnDestroy()
    {
        zombieBuckets.Dispose();
        iceStrengths.Dispose();
        iceStrengthsOld.Dispose();
        pathMatrix.Dispose();
    }


}
