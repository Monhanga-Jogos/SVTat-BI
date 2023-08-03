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

//Attacker Modifiers
        public int adestramentoAttacker;
        public int raioDeAcaoAttacker;
        public int pontariaAttacker;
        public int concentracaoAttacker;
        public int vigorAttacker;

//Defender Modifiers
        public int adestramentoDefender;
        public int protecaoDefender;
        public int visibilidadeDefender;
        public int movimentoDefender;
        public int distanciaAttackerDefender;
        public int inclinacaoAttackerDefender;

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
            float waitingTime = combatUnitScript.unitSo.coolDownTime;

            targetHandler.IsBreathing();

            yield return new WaitForSeconds(waitingTime);
            
            targetHandler.IsFighting();

            AttackTurn();

        }

        private void AttackTurn ()
        {
            if (targetHandler.currentState == UnitTargetHandler.UnitState.Fighting)
            {
                int attackerCurrentModifiers = (adestramentoAttacker + raioDeAcaoAttacker + pontariaAttacker - concentracaoAttacker - vigorAttacker);
                
                int defenderCurrentModifiers = (adestramentoDefender + protecaoDefender + visibilidadeDefender + movimentoDefender + distanciaAttackerDefender + inclinacaoAttackerDefender);

                int hitProbability = (attackerCurrentModifiers - defenderCurrentModifiers);



                float attackerCurrentPower = (combatUnitScript.unitSo.attackPower + DiceRoll.RollD20());

                float defenderCurrentProtection = targetObject.GetComponent<CombatUnit>().unitSo.defense;
                
                float finalDamage = (attackerCurrentPower - defenderCurrentProtection);

                targetUnit.GetComponent<CombatUnit>().TakeDamage(finalDamage);
                Debug.LogFormat("{0} is attacking {1} with {2} damage", combatUnitScript.UnitRaceType, targetUnit.UnitRaceType, finalDamage);

            }
        }

    }   
}