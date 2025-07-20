using UnityEngine;
using System.Collections.Generic;

public class ChevalierScript : BaseUnitScript
{
    public override void SpecialAbility(BaseUnitScript target)
    {
        if (target == null)
        {
            Debug.LogWarning("Aucune cible pour l'attaque spéciale du Chevalier.");
            return;
        }

        int baseDamage = attackDamage;
        target.TakeDamage(baseDamage);
        Heal(baseDamage);

        Debug.Log($"{name} attaque {target.name} et se soigne de {baseDamage} PV !");
    }

    private void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($"{name} récupère {amount} PV. Santé actuelle : {currentHealth}/{maxHealth}");
    }

}
