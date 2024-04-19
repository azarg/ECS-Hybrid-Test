using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class EnemyEntityBehavior : MonoBehaviour
{
    Entity entity;
    EntityManager entityManager;
    EnemyBase enemyBase;

    private void Awake() {
        enemyBase = GetComponent<EnemyBase>();

        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        entity = entityManager.CreateEntity();
        
        entityManager.AddComponentData(entity, new Enemy {
            Position = transform.position,
            Direction = enemyBase.Direction,
            Speed = enemyBase.Speed,
            Rng = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(1, int.MaxValue))
        });
    }

    private void Update()
    {
        var position = entityManager.GetComponentData<Enemy>(entity).Position;
        this.transform.position = position;
    }
}

public struct Enemy : IComponentData {
    public float3 Position;
    public float3 Direction;
    public float Speed;
    public Unity.Mathematics.Random Rng;
}
