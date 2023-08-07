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

            //Permanent Modifiers
            adestramentoAttacker = combatUnitScript.adestramento;
            raioDeAcaoAttacker = combatUnitScript.raioDeAcao;
            pontariaAttacker = combatUnitScript.pontaria;

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
                concentracaoAttacker = combatUnitScript.concentracao;
                vigorAttacker = combatUnitScript.vigor;

                int attackerCurrentModifiers = (adestramentoAttacker + raioDeAcaoAttacker + pontariaAttacker - concentracaoAttacker - vigorAttacker);
                
                Debug.LogFormat("Modificadores do Atirador: Adestramento = {0}; Raio de Ação = {1}; Pontaria = {2}; Concentração = {3}; Vigor = {4}.", adestramentoAttacker, raioDeAcaoAttacker, pontariaAttacker,concentracaoAttacker,vigorAttacker);
                
                adestramentoDefender = targetObject.GetComponent<CombatUnit>().adestramento;
                protecaoDefender = targetObject.GetComponent<CombatUnit>().protecao;
                visibilidadeDefender = targetObject.GetComponent<CombatUnit>().visibilidade;
                movimentoDefender = targetObject.GetComponent<CombatUnit>().movimento;

                int defenderCurrentModifiers = (adestramentoDefender + protecaoDefender + visibilidadeDefender + movimentoDefender + distanciaAttackerDefender + inclinacaoAttackerDefender);

                Debug.LogFormat("Modificadores do Alvo: Adestramento = {0}; Proteção = {1}; Visibilidade = {2}; Movimento = {3}; Distância = {4}; Inclinação {5}.", adestramentoDefender,protecaoDefender,visibilidadeDefender,movimentoDefender,distanciaAttackerDefender,inclinacaoAttackerDefender);

//                float attackerCurrentModifiersFloat = (float)attackerCurrentModifiers;
//                float defenderCurrentModifiersFloat = (float)defenderCurrentModifiers;

                int hitProbabilityValue = (attackerCurrentModifiers - defenderCurrentModifiers);
                
                Debug.LogFormat("Valor de Referência da Probabilidade de Acerto = {0}", hitProbabilityValue);

                hitProbability = CalculateHitProbability(hitProbabilityValue);

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

        private float CalculateHitProbability(int referenceValue)
        {
            if (referenceValue <= -8) {return 0.05f;}
            else if (-8 < referenceValue && referenceValue <= -6) {return 0.15f;}
            else if (-6 < referenceValue && referenceValue <= -4) {return 0.25f;}
            else if (-4 < referenceValue && referenceValue <= -2) {return 0.35f;}
            else if (-2 < referenceValue && referenceValue <= 0) {return 0.45f;}
            else if (0 < referenceValue && referenceValue <= 2) {return 0.55f;}
            else if (2 < referenceValue && referenceValue <= 4) {return 0.65f;}
            else if (4 < referenceValue && referenceValue <= 6) {return 0.75f;}
            else if (6 < referenceValue && referenceValue <= 8) {return 0.85f;}
            else if (8 > referenceValue) {return 0.95f;}
            else return 0.0f;
        }

    }   
}