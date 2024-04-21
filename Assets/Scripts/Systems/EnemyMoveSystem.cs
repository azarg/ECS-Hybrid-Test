using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

public partial struct EnemyMoveSystem : ISystem
{
    EntityQuery query;

    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EnemyComponent>();
        
        query = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<EnemyComponent>()
            .Build(ref state);
    }

    public void OnUpdate(ref SystemState state) {
        bool findNearest = Spawner.Instance.FindNearest;
        var enemies = query.ToComponentDataArray<EnemyComponent>(Allocator.TempJob);
        
        new EnemyMoveJob {
            Enemies = enemies,
            SpawnRadius = Spawner.Instance.SpawnRadius,
            DeltaTime = Time.deltaTime,
            FindNearest = findNearest,
        }.ScheduleParallel();
        state.Dependency.Complete();

        if (Spawner.Instance.UseTransformCopyJob) {
            var job = new CopyTransformsJob {
                Enemies = enemies,
            };
            job.Schedule(Spawner.Instance.AccessArray).Complete();
            
            if (Spawner.Instance.FindNearest) {
                foreach (var enemy in enemies) {
                    UnityEngine.Debug.DrawLine(enemy.Position, enemy.NearestEnemyPosition);
                }
            }
        }
    }
}

[BurstCompile]
public partial struct CopyTransformsJob : IJobParallelForTransform
{
    [ReadOnly] public NativeArray<EnemyComponent> Enemies;
    public void Execute(int index, TransformAccess transform) {
        transform.localPosition = Enemies[index].Position;
    }
}

[BurstCompile]
public partial struct EnemyMoveJob : IJobEntity
{
    [ReadOnly] public NativeArray<EnemyComponent> Enemies;
    [ReadOnly] public float SpawnRadius;
    [ReadOnly] public float DeltaTime;
    [ReadOnly] public bool FindNearest;

    void Execute(ref EnemyComponent enemy) {
        var pos = enemy.Position;

        // move
        pos += enemy.Direction * enemy.Speed * DeltaTime;

        if (FindNearest) {
            // find nearest enemy
            float minDist = float.MaxValue;
            float3 nearestEnemyPos = float3.zero;
            for (int i = 0; i < Enemies.Length; i++) {

                if (enemy.Position.Equals(Enemies[i].Position)) continue;

                var dist = math.distance(enemy.Position, Enemies[i].Position);
                if (dist < minDist) {
                    minDist = dist;
                    nearestEnemyPos = Enemies[i].Position;
                }
            }
            enemy.NearestEnemyPosition = nearestEnemyPos;
        }

        // if beyond spawn circle, move back into spawn cirlce
        // and change direction to random
        if (math.length(pos) > SpawnRadius) {
            pos = math.normalize(pos) * SpawnRadius;
            enemy.Direction = new float3(enemy.Rng.NextFloat2Direction(),0f);
        }
        enemy.Position = pos;
    }
}