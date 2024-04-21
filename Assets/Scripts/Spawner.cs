using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;
using Random = UnityEngine.Random;

public enum Mode
{
    PureMonoBehaviors,
    HybridEntities,
    HybridEntitiesWithTransformCopyJob,
    MonoBehaviorWithJobs,
}

public struct EnemyDataElement
{
    public float3 Position;
    public float3 Direction;
    public float3 NearestEnemyPosition;
    public float Speed;
    public Unity.Mathematics.Random Rng;
}

public class Spawner : MonoBehaviour
{
    public static Spawner Instance;

    public Mode Mode;
    public bool FindNearest;
    public int SpawnCount;
    public float SpawnRadius;
    
    public GameObject EnemyPrefab;
   
    float enemyMinSpeed = 1f;
    float enemyMaxSpeed = 5f;

    [HideInInspector] public Transform[] Enemies;
    [HideInInspector] public EnemyDataElement[] EnemyData;
    [HideInInspector] public TransformAccessArray AccessArray;
    [HideInInspector] public bool UseTransformCopyJob;

    private void Awake() {
        Instance = this;
        Enemies = new Transform[SpawnCount];
        EnemyData = new EnemyDataElement[SpawnCount];
    }

    private void Start() {
       
        for (int i = 0; i < SpawnCount; i++) {

            // spawn
            var pos = Random.insideUnitCircle.normalized * SpawnRadius;
            var inst = Instantiate(EnemyPrefab, (Vector3) pos, Quaternion.identity);
            Enemies[i] = inst.transform;

            Vector2 direction = Random.insideUnitCircle.normalized;
            float speed = Random.Range(enemyMinSpeed, enemyMaxSpeed);

            if (Mode == Mode.PureMonoBehaviors) {
                var enemy = inst.AddComponent<EnemyMonoBehavior>();
                enemy.Direction = direction;
                enemy.Speed = speed;
            } 
            else if (Mode == Mode.HybridEntities || Mode == Mode.HybridEntitiesWithTransformCopyJob) {
                var enemy = inst.AddComponent<EnemyAuthoring>();
                enemy.Direction = direction;
                enemy.Speed = speed;
                if (Mode != Mode.HybridEntitiesWithTransformCopyJob) {
                    inst.AddComponent<EnemyEntitiesBehavior>();
                }
            } else if (Mode == Mode.MonoBehaviorWithJobs) {
                EnemyData[i].Position = new float3(pos, 0f);
                EnemyData[i].Direction = new float3(direction, 0f);
                EnemyData[i].Speed = speed;
                EnemyData[i].Rng = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(1, int.MaxValue));
            }
        }
        if (Mode == Mode.HybridEntitiesWithTransformCopyJob) {
            UseTransformCopyJob = true;
            AccessArray = new TransformAccessArray(Enemies);
        }
        else {
            UseTransformCopyJob = false;
        }
        if (Mode == Mode.MonoBehaviorWithJobs) {
            AccessArray = new TransformAccessArray(Enemies);
            World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<SimulationSystemGroup>().AddSystemToUpdateList(World.DefaultGameObjectInjectionWorld.CreateSystem<EnemyMoveSystemMono>());
        }

    }
}
