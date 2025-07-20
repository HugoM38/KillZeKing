using UnityEngine;
using System.Collections.Generic;

public class SentinelleScript : BaseUnitScript
{
    public override void SpecialAbility(BaseUnitScript target)
    {
        int baseDamage = attackDamage;
        int damage = Mathf.CeilToInt(baseDamage * 1.5f);
        target.TakeDamage(damage);

        float chance = Random.value;
        if (chance <= 0.25f)
        {
            target.UseEnergy(1);
        }
    }

}
