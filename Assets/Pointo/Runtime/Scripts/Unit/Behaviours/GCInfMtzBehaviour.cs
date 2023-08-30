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
        public int adestramentoAtirador;
        public int raioDeAcaoAtirador;
        public int pontariaAtirador;
        public int concentracaoAtirador;
        public int vigorAtirador;

//Defender Modifiers
        public int adestramentoAlvo;
        public int protecaoAlvo;
        public int visibilidadeAlvo;
        public int movimentoAlvo;
        public int distanciaAtiradorAlvo;
        public int inclinacaoAtiradorAlvo;

        public float hitProbability = 0;
        public float ntzProbability = 0;
        private float groupsOfShooters = 1.0f;
        private int numberOfTests;

        private void Start()
        {
            combatUnitScript = GetComponent<CombatUnit>();
            targetHandler = GetComponent<UnitTargetHandler>();
            targetHandler.OnObjectReached = HandleEnemyReached;

            //Permanent Modifiers
            adestramentoAtirador = combatUnitScript.adestramento;
            raioDeAcaoAtirador = combatUnitScript.raioDeAcao;
            pontariaAtirador = combatUnitScript.pontaria;

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

            AttackerTurn();

        }

        private void AttackerTurn ()
        {
            if (targetHandler.currentState == UnitTargetHandler.UnitState.Fighting)
            {
                concentracaoAtirador = combatUnitScript.concentracao;
                vigorAtirador = combatUnitScript.vigor;

                int attackerCurrentModifiers = (adestramentoAtirador + raioDeAcaoAtirador + pontariaAtirador - concentracaoAtirador - vigorAtirador);
                
                Debug.LogFormat("Modificadores do Atirador: Adestramento = {0}; Raio de Ação = {1}; Pontaria = {2}; Concentração = {3}; Vigor = {4}.", adestramentoAtirador, raioDeAcaoAtirador, pontariaAtirador,concentracaoAtirador,vigorAtirador);
                
                adestramentoAlvo = targetObject.GetComponent<CombatUnit>().adestramento;
                protecaoAlvo = targetObject.GetComponent<CombatUnit>().protecao;
                visibilidadeAlvo = targetObject.GetComponent<CombatUnit>().visibilidade;
                movimentoAlvo = targetObject.GetComponent<CombatUnit>().movimento;

                int defenderCurrentModifiers = (adestramentoAlvo + protecaoAlvo + visibilidadeAlvo + movimentoAlvo + distanciaAtiradorAlvo + inclinacaoAtiradorAlvo);
                Debug.LogFormat("Modificadores do Alvo: Adestramento = {0}; Proteção = {1}; Visibilidade = {2}; Movimento = {3}; Distância = {4}; Inclinação {5}.", adestramentoAlvo,protecaoAlvo,visibilidadeAlvo,movimentoAlvo,distanciaAtiradorAlvo,inclinacaoAtiradorAlvo);

                int hitProbabilityValue = (attackerCurrentModifiers - defenderCurrentModifiers);                
                Debug.LogFormat("Valor de Referência da Probabilidade de Acerto = {0}", hitProbabilityValue);
                hitProbability = CalculateHitProbability(hitProbabilityValue);
                Debug.LogFormat("Probabilidade de Acerto = {0}%", hitProbability*100);

                ntzProbability = CalculateNeutralizationProbability(hitProbability);
                Debug.LogFormat("Probabilidade de Neutralização = {0}%", ntzProbability*100);
                Debug.LogFormat("Quantidade de Testes = {0}", numberOfTests);

                for (int i = 1; i < numberOfTests+1; i++)
                {
                    int luckResult = DiceRoll.RollD100();
                    Debug.LogFormat("Rolagem Nr {0}, d100 = {1}", i, luckResult);

                    if (luckResult <= ntzProbability*100)
                    {
                        int finalDamage = 1;
                        targetUnit.GetComponent<CombatUnit>().TakeDamage(finalDamage);
                        Debug.LogFormat("{0} is attacking {1} with {2} damage", combatUnitScript.UnitRaceType, targetUnit.UnitRaceType, finalDamage);
                    } else 
                    {
                        Debug.LogFormat("{0} attack failed", combatUnitScript.UnitRaceType);
                    }
                }
            }
        }

        private void DefenderTurn ()
        {
            if (targetHandler.currentState == UnitTargetHandler.UnitState.Fighting)
            {
                concentracaoAtirador = combatUnitScript.concentracao;
                vigorAtirador = combatUnitScript.vigor;

                int attackerCurrentModifiers = (adestramentoAtirador + raioDeAcaoAtirador + pontariaAtirador - concentracaoAtirador - vigorAtirador);
                
                Debug.LogFormat("Modificadores do Atirador: Adestramento = {0}; Raio de Ação = {1}; Pontaria = {2}; Concentração = {3}; Vigor = {4}.", adestramentoAtirador, raioDeAcaoAtirador, pontariaAtirador,concentracaoAtirador,vigorAtirador);
                
                adestramentoAlvo = targetObject.GetComponent<CombatUnit>().adestramento;
                protecaoAlvo = targetObject.GetComponent<CombatUnit>().protecao;
                visibilidadeAlvo = targetObject.GetComponent<CombatUnit>().visibilidade;
                movimentoAlvo = targetObject.GetComponent<CombatUnit>().movimento;

                int defenderCurrentModifiers = (adestramentoAlvo + protecaoAlvo + visibilidadeAlvo + movimentoAlvo + distanciaAtiradorAlvo + inclinacaoAtiradorAlvo);
                Debug.LogFormat("Modificadores do Alvo: Adestramento = {0}; Proteção = {1}; Visibilidade = {2}; Movimento = {3}; Distância = {4}; Inclinação {5}.", adestramentoAlvo,protecaoAlvo,visibilidadeAlvo,movimentoAlvo,distanciaAtiradorAlvo,inclinacaoAtiradorAlvo);

                int hitProbabilityValue = (attackerCurrentModifiers - defenderCurrentModifiers);                
                Debug.LogFormat("Valor de Referência da Probabilidade de Acerto = {0}", hitProbabilityValue);
                hitProbability = CalculateHitProbability(hitProbabilityValue);
                Debug.LogFormat("Probabilidade de Acerto = {0}%", hitProbability*100);

                ntzProbability = CalculateNeutralizationProbability(hitProbability);
                Debug.LogFormat("Probabilidade de Neutralização = {0}%", ntzProbability*100);
                Debug.LogFormat("Quantidade de Testes = {0}", numberOfTests);

                for (int i = 1; i < numberOfTests+1; i++)
                {
                    int luckResult = DiceRoll.RollD100();
                    Debug.LogFormat("Rolagem Nr {0}, d100 = {1}", i, luckResult);

                    if (luckResult <= ntzProbability*100)
                    {
                        int finalDamage = 1;
                        targetUnit.GetComponent<CombatUnit>().TakeDamage(finalDamage);
                        Debug.LogFormat("{0} is attacking {1} with {2} damage", combatUnitScript.UnitRaceType, targetUnit.UnitRaceType, finalDamage);
                    } else 
                    {
                        Debug.LogFormat("{0} attack failed", combatUnitScript.UnitRaceType);
                    }
                }
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

        private float CalculateNeutralizationProbability(float singleHitProbability)
        {
            int shootersQuantity = Mathf.RoundToInt(combatUnitScript.efetivoAtual);
            int targetsQuantity = Mathf.RoundToInt(targetObject.GetComponent<CombatUnit>().efetivoAtual);
            Debug.LogFormat("Efetivo do Atacante = {0}; Efetivo do Defensor = {1}.", shootersQuantity,targetsQuantity); 

            if (shootersQuantity <= targetsQuantity)
            {
                numberOfTests = shootersQuantity;
                groupsOfShooters = 1.0f;
            } else
            {
                numberOfTests = targetsQuantity;
                groupsOfShooters = Mathf.RoundToInt(shootersQuantity/targetsQuantity);
            }

            Debug.LogFormat("Grupos de Atiradores = {0} Homens / Alvo", groupsOfShooters);

            float ntzValue = 1 - Mathf.Pow((1 - singleHitProbability), groupsOfShooters);
            float ntzPercentage = Mathf.Round(ntzValue * 100.0f) * 0.01f;
            return ntzPercentage;
        }

    }   
}