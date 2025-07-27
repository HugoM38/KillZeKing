using System.Collections.Generic;
using UnityEngine;

public class InspirisScript : BaseUnitScript
{
    private enum StatBoostType
    {
        HP,
        HP_MAX,
        ATTACK,
        ATTACK_RANGE,
        MOVEMENT_RANGE
    }

    public override List<Tile> ShowSpecialAttackOptions()
    {
        // On ne cible rien directement → juste sa propre case en jaune
        Tile originTile = SelectionManager.Instance.selectedTile;
        if (originTile != null)
        {
            originTile.SetHighlight(Color.yellow);
            return new List<Tile> { originTile };
        }
        return new List<Tile>();
    }

    public override List<Tile> GetSpecialAttackArea(Tile targetTile)
    {
        // L'effet est global donc on ignore la case (mais on vérifie la sélection)
        if (targetTile != SelectionManager.Instance.selectedTile)
            return new List<Tile>();

        return new List<Tile> { targetTile }; // Permet juste de valider le clic
    }

    public override void SpecialAttack(BaseUnitScript target)
    {
        Debug.Log("[Inspiris] Lance son inspiration divine sur toutes les unités alliées.");

        List<BaseUnitScript> allUnits = new List<BaseUnitScript>(FindObjectsByType<BaseUnitScript>(FindObjectsSortMode.None));
        StatBoostType selectedBoost = (StatBoostType)Random.Range(0, 5);
        Debug.Log($"[Inspiris] Boost sélectionné : {selectedBoost}");

        foreach (BaseUnitScript unit in allUnits)
        {
            if (unit.team == this.team)
            {
                switch (selectedBoost)
                {
                    case StatBoostType.HP:
                        unit.SetCurrentHealth(unit.GetCurrentHealth() + 1);
                        break;
                    case StatBoostType.HP_MAX:
                        unit.SetMaxHealth(unit.GetMaxHealth() + 1);
                        break;
                    case StatBoostType.ATTACK:
                        unit.SetAttackDamage(unit.GetAttackDamage() + 1);
                        break;
                    case StatBoostType.ATTACK_RANGE:
                        unit.SetAttackRange(unit.GetAttackRange() + 1);
                        break;
                    case StatBoostType.MOVEMENT_RANGE:
                        unit.SetMovementRange(unit.GetMovementRange() + 1);
                        break;
                }
            }
        }

        SetCurrentEnergy(0);
        TurnManager.Instance.SpendPA();
    }
}
