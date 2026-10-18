using UnityEngine;
using System.Collections.Generic;

public class Projectile : MonoBehaviour
{
    public float speed = 8f;
    public float maxLifetime = 4f;

    private Enemy target;
    private Vector3 lastKnownPos;
    private float damage;
    private float splashRadius;
    private float slowFactor = 1f;
    private float slowDuration;
    private float lifeTimer = 0f;

    public void Launch(Enemy target, float damage, float splashRadius, float slowFactor, float slowDuration)
    {
        this.target = target;
        this.damage = damage;
        this.splashRadius = splashRadius;
        this.slowFactor = slowFactor <= 0f ? 1f : slowFactor;
        this.slowDuration = slowDuration;

        lastKnownPos = target.transform.position;
    }

    void Update()
    {
        lifeTimer += Time.deltaTime;
        if (lifeTimer >= maxLifetime)
        {
            Destroy(gameObject);
            return;
        }

        // Keep chasing, if the target died fly to where it was
        if (target != null && !target.IsDead)
            lastKnownPos = target.transform.position;

        Vector3 toTarget = lastKnownPos - transform.position;
        float step = speed * Time.deltaTime;

        if (toTarget.magnitude <= step)
        {
            Hit();
            return;
        }

        transform.position += toTarget.normalized * step;

        float angle = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void Hit()
    {
        if (splashRadius > 0f)
        {
            // Copy the list, enemies can die (and be removed) while we loop
            List<Enemy> snapshot = new List<Enemy>(Enemy.ActiveEnemies);

            foreach (Enemy enemy in snapshot)
            {
                if (enemy == null || enemy.IsDead) continue;

                float dist = Vector2.Distance(enemy.transform.position, transform.position);
                if (dist > splashRadius) continue;

                // Edge of the blast does half damage
                float falloff = 1f - (dist / splashRadius) * 0.5f;
                ApplyTo(enemy, damage * falloff);
            }
        }
        else if (target != null && !target.IsDead)
        {
            ApplyTo(target, damage);
        }

        Destroy(gameObject);
    }

    private void ApplyTo(Enemy enemy, float dmg)
    {
        if (slowFactor < 1f)
            enemy.ApplySlow(slowFactor, slowDuration);

        enemy.TakeDamage(dmg);
    }
}
