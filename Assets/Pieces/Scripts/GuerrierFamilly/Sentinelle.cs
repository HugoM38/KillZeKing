using UnityEngine;

public class SentinelleScript : BaseUnitScript
{   /*
    public override void SpecialAttack(BaseUnitScript target)
    {
        if (target == null || target.team == this.team)
        {
            Debug.LogWarning("Cible invalide pour l'attaque spéciale de la Sentinelle.");
            return;
        }

        int previousHP = target.currentHealth;

        target.TakeDamage(attackDamage);
        Debug.Log($"{name} utilise son attaque spéciale et inflige {attackDamage} dégâts à {target.name}");

        if (target.currentHealth <= 0 && previousHP > 0)
        {
            maxHealth += 1;
            Heal(2);

            Debug.Log($"{name} a tué {target.name} : +2 PV et +1 PV Max. Santé actuelle : {currentHealth}/{maxHealth}");
        }
    }

    private void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }*/
}
