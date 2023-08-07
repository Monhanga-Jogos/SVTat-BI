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

        public float hitProbability = 0;
        public float ntzProbability = 0;

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
                
                Debug.LogFormat("Modificadores do Atirador: Adestramento = {0}; Raio de Ação = {1}; Pontaria = {2}; Concentração = {3}; Vigor = {4}.", adestramentoAttacker, raioDeAcaoAttacker, pontariaAttacker,concentracaoAttacker,vigorAttacker);

                int defenderCurrentModifiers = (adestramentoDefender + protecaoDefender + visibilidadeDefender + movimentoDefender + distanciaAttackerDefender + inclinacaoAttackerDefender);

                Debug.LogFormat("Modificadores do Alvo: Adestramento = {0}; Proteção = {1}; Visibilidade = {2}; Movimento = {3}; Distância = {4}; Inclinação {5}.", adestramentoDefender,protecaoDefender,visibilidadeDefender,movimentoDefender,distanciaAttackerDefender,inclinacaoAttackerDefender);

//                float attackerCurrentModifiersFloat = (float)attackerCurrentModifiers;
//                float defenderCurrentModifiersFloat = (float)defenderCurrentModifiers;

                int hitProbabilityValue = (attackerCurrentModifiers - defenderCurrentModifiers);

                if (hitProbabilityValue <= -8) {hitProbability = 0.05f;}

                Debug.LogFormat("Probabilidade de Acerto = {0}", hitProbability);

                int attackerSoldiersQuantity = Mathf.RoundToInt(combatUnitScript.currentHealth); 

                int defenderSoldiersQuantity = Mathf.RoundToInt(targetObject.GetComponent<CombatUnit>().currentHealth);

                Debug.LogFormat("Efetivo do Atacante = {0}; Efetivo do Defensor = {1}.", attackerSoldiersQuantity,defenderSoldiersQuantity);                

                float attackerCurrentPower = (combatUnitScript.unitSo.attackPower + DiceRoll.RollD20());

                float defenderCurrentProtection = targetObject.GetComponent<CombatUnit>().unitSo.defense;
                
                float finalDamage = (attackerCurrentPower - defenderCurrentProtection);

                targetUnit.GetComponent<CombatUnit>().TakeDamage(finalDamage);
                Debug.LogFormat("{0} is attacking {1} with {2} damage", combatUnitScript.UnitRaceType, targetUnit.UnitRaceType, finalDamage);

            }
        }

    }   
}