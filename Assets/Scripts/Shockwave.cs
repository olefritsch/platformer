using UnityEngine;

[CreateAssetMenu(fileName = "Shockwave", menuName = "Abilities/Shockwave")]
public class Shockwave : Ability
{
    public static AbilityType Type = AbilityType.Shockwave;

    [SerializeField] float explosionForce = 500f;
    [SerializeField] float explosionRadius = 5f;

    protected override void DoAbilitySpecifics()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, 1 << LayerMask.NameToLayer("Players"));
        foreach (Collider collider in colliders)
        {
            // Don't add force to self
            if (collider.transform.root == transform)
                continue;

            Rigidbody rb = collider.GetComponentInParent<Rigidbody>();

            if (rb != null)
                rb.AddExplosionForce(explosionForce, this.transform.position, explosionRadius, 0.2f);
        }
    }
}
