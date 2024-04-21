using Unity.Entities;
using Unity.Mathematics;

public struct EnemyComponent : IComponentData {
    public float3 Position;
    public float3 Direction;
    public float Speed;
    public float3 NearestEnemyPosition;
    public Unity.Mathematics.Random Rng;
}
