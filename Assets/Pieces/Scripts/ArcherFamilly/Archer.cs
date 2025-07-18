using UnityEngine;
using System.Collections.Generic;

public class Archer : BaseUnitScript
{
    public override void SpecialAbility(BaseUnitScript target)
    {
        Debug.Log($"{name} lance un sort spécial sur {target.name} pour {10} dégâts magiques !");
        target.TakeDamage(10);
    }

}
