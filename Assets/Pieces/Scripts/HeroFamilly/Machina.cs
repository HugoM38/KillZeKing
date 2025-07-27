using System.Collections.Generic;
using UnityEngine;

public class MachinaScript : BaseUnitScript
{
    private bool boostActivated = false;

    public override List<Tile> ShowSpecialAttackOptions()
    {
        Tile originTile = SelectionManager.Instance.selectedTile;
        if (originTile == null)
        {
            Debug.LogWarning("[Machina] Aucun selectedTile.");
            return new List<Tile>();
        }

        originTile.SetHighlight(Color.yellow);
        return new List<Tile> { originTile };
    }

    public override List<Tile> GetSpecialAttackArea(Tile targetTile)
    {
        List<Tile> area = new List<Tile>();
        Tile originTile = SelectionManager.Instance.selectedTile;

        if (targetTile == originTile)
        {
            area.Add(originTile);
        }

        return area;
    }

    public override void SpecialAttack(BaseUnitScript target)
    {
        Tile originTile = SelectionManager.Instance.selectedTile;
        if (target != this || originTile == null)
        {
            Debug.LogWarning("[Machina] La compétence doit être utilisée sur elle-même.");
            return;
        }

        if (boostActivated)
        {
            Debug.Log("[Machina] L'amélioration passive a déjà été activée.");
            return;
        }

        boostActivated = true;
        Debug.Log("<color=cyan>[Machina]</color> améliore les futures invocations des Ingénieurs !");
        TurnManager.Instance.SpendPA();
        SetCurrentEnergy(0);
    }

    public bool IsBoostActive() => boostActivated;

    public void EnhanceUnit(BaseUnitScript unit)
    {
        if (!boostActivated || unit == null) return;

        if (unit is BarricadeScript)
        {
            unit.SetMaxHealth(unit.GetMaxHealth() + 3);
            unit.SetCurrentHealth(unit.GetCurrentHealth() + 3);
        }
        else if (unit is TourelleScript)
        {
            unit.SetAttackDamage(unit.GetAttackDamage() + 1);
            unit.SetAttackRange(unit.GetAttackRange() + 1);
        }
        else if (unit is ShieldScript)
        {
            unit.SetMaxHealth(unit.GetMaxHealth() + 1);
            unit.SetCurrentHealth(unit.GetCurrentHealth() + 1);
            unit.SetAttackRange(unit.GetAttackRange() + 1);
        }
    }
}
