using UnityEngine;

public abstract class Ability : ScriptableObject {

    [SerializeField] float cooldown = 1;

    [HideInInspector]
    public Transform transform;

    protected float lastUsed;
    protected bool onCooldown;

    public void Initialize(GameObject owner)
    {
        transform = owner.transform;
    }

    public void TriggerAbility()
    {
        if (Time.time - lastUsed > cooldown)
            DoAbilitySpecifics();
    }

    protected abstract void DoAbilitySpecifics();

}
