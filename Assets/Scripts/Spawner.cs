using UnityEngine;
using UnityEngine.Jobs;

public class Spawner : MonoBehaviour
{
    public static Spawner Instance;
    
    public GameObject EnemyPrefab;

    public int SpawnCount;
    public float SpawnRadius;

    public bool FindNearest;

    [Header("These have no effect after start")]
    public bool UseEntities;
    public bool UseTransformCopyJob;
    
    float enemyMinSpeed = 1f;
    float enemyMaxSpeed = 5f;

    [HideInInspector] public Transform[] Enemies;
    [HideInInspector] public TransformAccessArray AccessArray;


    private void Awake() {
        Instance = this;
        Enemies = new Transform[SpawnCount];
    }

    private void Start() {
        for(int i = 0; i < SpawnCount; i++) {

            // spawn
            var pos = Random.insideUnitCircle.normalized * SpawnRadius;
            var inst = Instantiate(EnemyPrefab, (Vector3) pos, Quaternion.identity);
            Enemies[i] = inst.transform;

            Vector2 direction = Random.insideUnitCircle.normalized;
            float speed = Random.Range(enemyMinSpeed, enemyMaxSpeed);

            if (UseEntities) {
                var comp = inst.AddComponent<EnemyAuthoring>();
                comp.Direction = direction;
                comp.Speed = speed;

                if (UseTransformCopyJob) {
                    AccessArray = new TransformAccessArray(Enemies);
                } else {
                    inst.AddComponent<EnemyEntitiesBehavior>();
                }
            } else {
                var comp = inst.AddComponent<EnemyMonoBehavior>();
                comp.Direction = direction;
                comp.Speed = speed;
            }
        }    
    }
}
