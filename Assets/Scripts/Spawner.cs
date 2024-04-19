using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Spawner Instance;
    public GameObject EnemyPrefab;
    public int SpawnCount;
    public float SpawnRadius;

    public bool UseEntities;
    
    float enemyMinSpeed = 1f;
    float enemyMaxSpeed = 5f;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        for(int i = 0; i < SpawnCount; i++) {

            // spawn
            var pos = Random.insideUnitCircle.normalized * SpawnRadius;
            var inst = Instantiate(EnemyPrefab, (Vector3) pos, Quaternion.identity);
            
            var enemy = inst.GetComponent<EnemyBase>();
            enemy.Direction = Random.insideUnitCircle.normalized;
            enemy.Speed = Random.Range(enemyMinSpeed, enemyMaxSpeed);

            if (UseEntities) {
                inst.AddComponent<EnemyEntityBehavior>();
            } else {
                inst.AddComponent<EnemyMonoBehavior>();
            }
        }    
    }
}
