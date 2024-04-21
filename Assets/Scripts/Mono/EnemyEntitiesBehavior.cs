using Unity.Entities;
using UnityEngine;

[RequireComponent(typeof(EnemyAuthoring))]
public class EnemyEntitiesBehavior : MonoBehaviour
{
    EnemyAuthoring enemyAuthoring;
    EntityManager entityManager;

    private void Awake() {
        enemyAuthoring = GetComponent<EnemyAuthoring>();
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    // 15ms total
    // 5ms for 5000 objects (i.e.calling empty Update())
    private void Update() {
        // 6ms for 5000 objects
        var enemy = entityManager.GetComponentData<EnemyComponent>(enemyAuthoring.entity);

        // 5ms for 5000 objects
        this.transform.position = enemy.Position;

        if (Spawner.Instance.FindNearest) {
            Debug.DrawLine(enemy.Position, enemy.NearestEnemyPosition);
        }
    }
}
