using System.Collections.Generic;
using UnityEngine;

public class BarricadeScript : BaseUnitScript
{
    public override List<Tile> ShowSpecialAttackOptions()
    {
        Tile originTile = SelectionManager.Instance.selectedTile;
        if (originTile == null || originTile.currentPiece != this)
        {
            Debug.LogWarning("[Barricade] Aucune case valide pour attaque spéciale.");
            return new List<Tile>();
        }

        // Elle ne peut se cibler que soi-même
        originTile.SetHighlight(Color.yellow);
        return new List<Tile> { originTile };
    }

    public override List<Tile> GetSpecialAttackArea(Tile targetTile)
    {
        List<Tile> area = new List<Tile>();

        // Ne peut se cibler que soi-même
        Tile originTile = SelectionManager.Instance.selectedTile;
        if (targetTile == originTile)
        {
            area.Add(targetTile);
        }

        return area;
    }

    public override void SpecialAttack(BaseUnitScript target)
    {
        if (target != this)
        {
            Debug.LogWarning("[Barricade] Elle ne peut utiliser sa compétence spéciale que sur elle-même.");
            return;
        }

        int healedAmount = 3;
        int newHealth = Mathf.Min(GetCurrentHealth() + healedAmount, GetMaxHealth());

        Debug.Log($"{name} utilise sa compétence spéciale et récupère {newHealth - GetCurrentHealth()} PV.");

        SetCurrentHealth(newHealth);
        TurnManager.Instance.SpendPA();
        SetCurrentEnergy(0);
    }

    // Override mouvement désactivé
    public override List<Tile> ShowMoveOptions()
    {
        Debug.Log("[Barricade] Ne peut pas se déplacer.");
        return new List<Tile>(); // Aucune case disponible
    }
}
