using UnityEngine;

public class EnemyMonoBehavior : MonoBehaviour
{
    EnemyBase enemyBase;
    Spawner spawner => Spawner.Instance;

    private void Awake() {
        enemyBase = GetComponent<EnemyBase>();
    }

    private void Update() {
        var pos = transform.position;

        // move
        pos += enemyBase.Direction * enemyBase.Speed * Time.deltaTime;

        // if beyond spawn circle, move back into spawn cirlce
        // and change direction to random
        if (pos.magnitude > spawner.SpawnRadius) {
            pos = pos.normalized * spawner.SpawnRadius;
            enemyBase.Direction = Random.insideUnitCircle.normalized;
        }
        transform.position = pos;
    }
}

