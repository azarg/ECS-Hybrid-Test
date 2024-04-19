using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public partial struct EnemyMoveSystem : ISystem
{
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<Enemy>();
    }

    public void OnUpdate(ref SystemState state) {
        new EnemyMoveJob { 
            SpawnRadius = Spawner.Instance.SpawnRadius,
            DeltaTime = Time.deltaTime,
        }.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct EnemyMoveJob : IJobEntity
{
    [ReadOnly] public float SpawnRadius;
    [ReadOnly] public float DeltaTime;
    void Execute(ref Enemy enemy) {
        var pos = enemy.Position;

        // move
        pos += enemy.Direction * enemy.Speed * DeltaTime;

        // if beyond spawn circle, move back into spawn cirlce
        // and change direction to random
        if (math.length(pos) > SpawnRadius) {
            pos = math.normalize(pos) * SpawnRadius;
            enemy.Direction = new float3(enemy.Rng.NextFloat2Direction(),0f);
        }
        enemy.Position = pos;
    }
}