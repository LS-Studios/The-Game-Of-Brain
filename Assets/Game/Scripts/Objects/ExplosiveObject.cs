using Zenject;
using UnityEngine;

public class ExplosiveObject : MonoBehaviour
{
    public float damage = 65f;
    public float effectRadius = 1f;
    private GlobalData globalData;

    private void Start()
    {
        GetComponent<HealtComponent>().onDie = Die;

        globalData = GameInstance.instance.referenceValues.globalData;
    }

    private void Die()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, effectRadius);

        foreach (Collider2D nearObjects in colliders)
        {
            if (nearObjects.GetComponent<IDamageable>() != null)
                nearObjects.GetComponent<IDamageable>().TakeDamage(damage, true);
        }

        Camera.main.transform.parent.GetComponent<CameraHandler>().Shake();

        AudioManager.instance.Play("Explosion", transform);

        Instantiate(globalData.explosion, transform.position, globalData.explosion.transform.rotation);
        Camera.main.transform.parent.GetComponent<CameraHandler>().Shake();

        Destroy(gameObject);
    }
}
