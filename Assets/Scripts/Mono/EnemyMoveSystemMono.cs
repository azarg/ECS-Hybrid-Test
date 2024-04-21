using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;


[DisableAutoCreation]
public partial struct EnemyMoveSystemMono : ISystem
{
    readonly Spawner Spawner => Spawner.Instance;

    public void OnUpdate(ref SystemState state) {
        var enemies = new NativeArray<EnemyDataElement>(Spawner.EnemyData, Allocator.TempJob);
        

        new EnemyMoveJobMono() {
            DeltaTime = Time.deltaTime,
            SpawnRadius = Spawner.SpawnRadius,
            Enemies = enemies,
        }.Schedule(enemies.Length, 100).Complete();

        if (Spawner.FindNearest) {
            var enemiesCopy = new NativeArray<EnemyDataElement>(enemies, Allocator.TempJob);

            new FindNearestJob() {
                Enemies = enemies,
                EnemiesRO = enemiesCopy,
            }.Schedule(enemies.Length, 100).Complete();
            foreach (var enemy in enemies) {
                UnityEngine.Debug.DrawLine(enemy.Position, enemy.NearestEnemyPosition);
            }
        }     

        enemies.CopyTo(Spawner.EnemyData);

        var job = new CopyTransformsJobMono {
            Enemies = enemies,
        };
        job.Schedule(Spawner.Instance.AccessArray).Complete();
    }
}

[BurstCompile]
public partial struct CopyTransformsJobMono : IJobParallelForTransform
{
    [ReadOnly] public NativeArray<EnemyDataElement> Enemies;
    public void Execute(int index, TransformAccess transform) {
        transform.localPosition = Enemies[index].Position;
    }
}

[BurstCompile]
public partial struct FindNearestJob : IJobParallelFor
{
    public NativeArray<EnemyDataElement> Enemies;
    [ReadOnly] public NativeArray<EnemyDataElement> EnemiesRO;

    public void Execute(int index) {
        var enemy = Enemies[index];
        float minDist = float.MaxValue;
        float3 nearestEnemyPos = enemy.Position;

        for (int i = 0; i < Enemies.Length; i++) {

            if (enemy.Position.Equals(EnemiesRO[i].Position)) continue;

            var dist = math.distance(enemy.Position, EnemiesRO[i].Position);
            if (dist < minDist) {
                minDist = dist;
                nearestEnemyPos = EnemiesRO[i].Position;
            }
        }
        enemy.NearestEnemyPosition = nearestEnemyPos;
        Enemies[index] = enemy;
    }
}

[BurstCompile]
public partial struct EnemyMoveJobMono : IJobParallelFor
{
    public NativeArray<EnemyDataElement> Enemies;
    [ReadOnly] public float SpawnRadius;
    [ReadOnly] public float DeltaTime;

    public void Execute(int index) {
        var enemy = Enemies[index];
        var pos = enemy.Position;

        // move
        pos += enemy.Direction * enemy.Speed * DeltaTime;

        // if beyond spawn circle, move back into spawn cirlce
        // and change direction to random
        if (math.length(pos) > SpawnRadius) {
            pos = math.normalize(pos) * SpawnRadius;
            enemy.Direction = new float3(enemy.Rng.NextFloat2Direction(), 0f);
        }
        enemy.Position = pos;
        Enemies[index] = enemy;
    }
}

