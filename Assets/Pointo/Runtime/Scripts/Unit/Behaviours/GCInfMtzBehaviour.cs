using System;
using System.Collections;
using UnityEngine;

namespace Pointo.Unit
{
    [RequireComponent(typeof(UnitTargetHandler))]
    [RequireComponent(typeof(CombatUnit))]
    public class GCInfMtzBehaviour : MonoBehaviour
    {
        private UnitTargetHandler targetHandler;
        private CombatUnit combatUnitScript;

        private Unit targetUnit;

        public GameObject targetObject;

        private void Start()
        {
            combatUnitScript = GetComponent<CombatUnit>();
            targetHandler = GetComponent<UnitTargetHandler>();
            targetHandler.OnObjectReached = HandleEnemyReached;
        }

        private void Update()
        {
            // colocar combate no update??
        }


        private void HandleEnemyReached(GameObject targetReached)
        {
            targetObject = targetReached;

            targetUnit = targetObject.GetComponent<Unit>();

            if (targetUnit == null) return;

            // we check on the Scriptable Object if we should attack
            if (combatUnitScript.unitSo.ShouldAttack(targetUnit.UnitRaceType) && targetObject.GetComponent<UnitTargetHandler>().currentState != UnitTargetHandler.UnitState.Destroyed)
            {
                StartCoroutine(CombatLoop());
            } else if(targetObject.GetComponent<UnitTargetHandler>().currentState == UnitTargetHandler.UnitState.Destroyed)
            {
                StopCoroutine(CombatLoop());
            }

        }

        IEnumerator CombatLoop()
        {

            targetHandler.IsBreathing();

            yield return new WaitForSeconds(5.0f);
            
            targetHandler.IsFighting();

            AttackRoll();

        }

        private void AttackRoll ()
        {
            if (targetHandler.currentState == UnitTargetHandler.UnitState.Fighting)
            {
                float attackerCurrentPower = (combatUnitScript.unitSo.attackPower + DiceRoll.RollD20());

                float defenderCurrentProtection = targetObject.GetComponent<CombatUnit>().unitSo.defense;
                
                float finalDamage = (attackerCurrentPower - defenderCurrentProtection);

                targetUnit.GetComponent<CombatUnit>().TakeDamage(finalDamage);
                Debug.LogFormat("{0} is attacking {1} with {2} damage", combatUnitScript.UnitRaceType, targetUnit.UnitRaceType, finalDamage);

            }
        }

    }   
}