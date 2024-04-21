using UnityEngine;
using Unity.Entities;

public class EnemyAuthoring : MonoBehaviour
{
    [HideInInspector] public Entity entity;
    [HideInInspector] public Vector3 Direction;
    [HideInInspector] public float Speed;

    private void Start() {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        entity = entityManager.CreateEntity();
        
        entityManager.AddComponentData(entity, new EnemyComponent {
            Position = transform.position,
            Direction = Direction,
            Speed = Speed,
            Rng = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(1, int.MaxValue))
        });
    }
}
