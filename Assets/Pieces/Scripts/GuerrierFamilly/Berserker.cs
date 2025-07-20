using UnityEngine;
using System.Collections.Generic;

public class BerserkerScript : BaseUnitScript
{
    public override void SpecialAbility(BaseUnitScript target)
    {
        if (target == null || target.team == this.team)
        {
            Debug.LogWarning("Cible invalide pour l'attaque spéciale du Berserker.");
            return;
        }

        int previousHP = target.currentHealth;

        target.TakeDamage(attackDamage);
        Debug.Log($"{name} utilise son attaque spéciale et inflige {attackDamage} dégâts à {target.name}");

        if (target.currentHealth <= 0 && previousHP > 0)
        {
            TurnManager.Instance.CurrentStats.pa = TurnManager.Instance.CurrentStats.pa + 1;
            Heal(1);
            Debug.Log($"{name} a tué {target.name} : +1 PA pour le joueur et +1 PV !");
            UseEnergy(maxEnergy * -1);

        }
    }

    private void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($"{name} récupère {amount} PV. Santé actuelle : {currentHealth}/{maxHealth}");
    }
}
