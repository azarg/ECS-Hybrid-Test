using UnityEngine;

public class EnemyMonoBehavior : MonoBehaviour
{
    [HideInInspector] public Vector3 Direction;
    [HideInInspector] public float Speed;

    Spawner spawner => Spawner.Instance;

    private void Update() {

        var pos = transform.position;

        // find nearest if "FindNearest" is checked
        if (spawner.FindNearest) {
            var minDist = float.MaxValue;
            var nearestEnemyPosition = Vector3.zero;
            for (int i = 0; i < spawner.Enemies.Length; i++) {
                if (pos == spawner.Enemies[i].position) continue;
                var dist = Vector3.Distance(pos, spawner.Enemies[i].position);
                if (dist < minDist) {
                    minDist = dist;
                    nearestEnemyPosition = spawner.Enemies[i].position;
                }
            }
            Debug.DrawLine(pos, nearestEnemyPosition);
        }

        // move
        pos += Speed * Time.deltaTime * Direction;

        // if beyond spawn circle, move back into spawn cirlce
        // and change direction to random
        if (pos.magnitude > spawner.SpawnRadius) {
            pos = pos.normalized * spawner.SpawnRadius;
            Direction = Random.insideUnitCircle.normalized;
        }
        transform.position = pos;
    }
}

