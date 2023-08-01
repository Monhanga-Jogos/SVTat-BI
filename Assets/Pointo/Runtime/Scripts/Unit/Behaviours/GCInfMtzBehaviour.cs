using UnityEngine;

namespace Pointo.Unit
{
    [RequireComponent(typeof(UnitTargetHandler))]
    [RequireComponent(typeof(CombatUnit))]
    public class GCInfMtzBehaviour : MonoBehaviour
    {
        private UnitTargetHandler targetHandler;
        private CombatUnit combatUnitScript;


        private void Start()
        {
            combatUnitScript = GetComponent<CombatUnit>();
            targetHandler = GetComponent<UnitTargetHandler>();
            targetHandler.OnObjectReached = HandleEnemyReached;
        }

        private void HandleEnemyReached(GameObject targetObject)
        {
            Unit targetUnit = targetObject.GetComponent<Unit>();
                     
            float attackerCurrentPower = (combatUnitScript.unitSo.attackPower + DiceRoll.RollD20());

            float defenderCurrentProtection = targetObject.GetComponent<CombatUnit>().unitSo.defense;
            
            float finalDamage = (attackerCurrentPower - defenderCurrentProtection);

            if (targetUnit == null) return;

            // we check on the Scriptable Object if we should attack
            if (targetHandler.IsFighting() && combatUnitScript.unitSo.ShouldAttack(targetUnit.UnitRaceType) && targetObject.GetComponent<UnitTargetHandler>().currentState != UnitTargetHandler.UnitState.Destroyed)
            {
                // attack!
                
                targetUnit.GetComponent<CombatUnit>().TakeDamage(finalDamage);
                Debug.LogFormat("{0} is attacking {1} with {2} damage", combatUnitScript.UnitRaceType, targetUnit.UnitRaceType, finalDamage);
            }
        }
    }   
}