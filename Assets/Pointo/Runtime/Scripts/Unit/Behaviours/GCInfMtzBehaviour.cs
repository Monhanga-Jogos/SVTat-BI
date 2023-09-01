﻿using System;
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

//Modificadores do Atirador
        public int adestramentoAtirador;
        public int raioDeAcaoAtirador;
        public int pontariaAtirador;
        public int concentracaoAtirador;
        public int vigorAtirador;

//Modificadores do Alvo
        public int adestramentoAlvo;
        public int protecaoAlvo;
        public int visibilidadeAlvo;
        public int movimentoAlvo;
        public int distanciaAtiradorAlvo;
        public int inclinacaoAtiradorAlvo;

        public float hitProbability = 0;
        public float ntzProbability = 0;
        private float gruposDeAtiradores = 1.0f;
        private int numberOfTests;

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
            if (combatUnitScript.unitSo.ShouldAttack(targetUnit.UnitRaceType) && 
            targetObject.GetComponent<UnitTargetHandler>().currentState != UnitTargetHandler.UnitState.Destroyed && targetHandler.currentState != UnitTargetHandler.UnitState.Destroyed)
            {
                AttackerTurn();
                DefenderTurn();
            }
        }

        private void AttackerTurn ()
        {
            if (targetHandler.currentState == UnitTargetHandler.UnitState.Fighting)
            {
                // Modificadores permanentes
                adestramentoAtirador = combatUnitScript.adestramento;
                raioDeAcaoAtirador = combatUnitScript.raioDeAcao;
                pontariaAtirador = combatUnitScript.pontaria;

                // Modificadores temporários
                concentracaoAtirador = combatUnitScript.concentracao;
                vigorAtirador = combatUnitScript.vigor;

                int modificadoresDoAtirador = (adestramentoAtirador + raioDeAcaoAtirador + pontariaAtirador - concentracaoAtirador - vigorAtirador);
                
                Debug.LogFormat("Modificadores do Atirador: Adestramento = {0}; Raio de Ação = {1}; Pontaria = {2}; Concentração = {3}; Vigor = {4}.", adestramentoAtirador, raioDeAcaoAtirador, pontariaAtirador,concentracaoAtirador,vigorAtirador);
                
                adestramentoAlvo = targetObject.GetComponent<CombatUnit>().adestramento;
                protecaoAlvo = targetObject.GetComponent<CombatUnit>().protecao;
                visibilidadeAlvo = targetObject.GetComponent<CombatUnit>().visibilidade;
                movimentoAlvo = targetObject.GetComponent<CombatUnit>().movimento;

                // Modificadores relativos
//                distanciaAtiradorAlvo = medirDistânciaEntreGameObjects
//                inclinacaoAtiradorAlvo = medirAnguloEntreGameObjects

                int modificadoresDoAlvo = (adestramentoAlvo + protecaoAlvo + visibilidadeAlvo + movimentoAlvo + distanciaAtiradorAlvo + inclinacaoAtiradorAlvo);
                Debug.LogFormat("Modificadores do Alvo: Adestramento = {0}; Proteção = {1}; Visibilidade = {2}; Movimento = {3}; Distância = {4}; Inclinação {5}.", adestramentoAlvo,protecaoAlvo,visibilidadeAlvo,movimentoAlvo,distanciaAtiradorAlvo,inclinacaoAtiradorAlvo);

                int hitProbabilityValue = (modificadoresDoAtirador - modificadoresDoAlvo);                
                Debug.LogFormat("Valor de Referência da Probabilidade de Acerto = {0}", hitProbabilityValue);

                hitProbability = CalculateHitProbability(hitProbabilityValue);
                Debug.LogFormat("Probabilidade de Acerto = {0}%", hitProbability*100);

                int efetivoAtacante = Mathf.RoundToInt(combatUnitScript.efetivoAtual);
                int efetivoDefensor = Mathf.RoundToInt(targetObject.GetComponent<CombatUnit>().efetivoAtual);

                ntzProbability = CalculateNeutralizationProbability(hitProbability, efetivoAtacante, efetivoDefensor);
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
                        Debug.LogFormat("{0} {1} neutralizou {2} militar de {3} {4}", combatUnitScript.UnitRaceType, gameObject.name, finalDamage, targetUnit.UnitRaceType, targetObject.name);
                    } else 
                    {
                        Debug.LogFormat("Ataque de {0} {1} falhou", combatUnitScript.UnitRaceType, gameObject.name);
                    }
                }
            }
        }

        private void DefenderTurn ()
        {
            if (targetHandler.currentState == UnitTargetHandler.UnitState.Fighting)
            {
                // Modificadores permanentes
                adestramentoAtirador = targetObject.GetComponent<CombatUnit>().adestramento;
                raioDeAcaoAtirador = targetObject.GetComponent<CombatUnit>().raioDeAcao;
                pontariaAtirador = targetObject.GetComponent<CombatUnit>().pontaria;

                // Modificadores temporários
                concentracaoAtirador = targetObject.GetComponent<CombatUnit>().concentracao;
                vigorAtirador = targetObject.GetComponent<CombatUnit>().vigor;

                int modificadoresDoAtirador = (adestramentoAtirador + raioDeAcaoAtirador + pontariaAtirador - concentracaoAtirador - vigorAtirador);
                
                Debug.LogFormat("Modificadores do Atirador: Adestramento = {0}; Raio de Ação = {1}; Pontaria = {2}; Concentração = {3}; Vigor = {4}.", adestramentoAtirador, raioDeAcaoAtirador, pontariaAtirador,concentracaoAtirador,vigorAtirador);
                
                adestramentoAlvo = combatUnitScript.adestramento;
                protecaoAlvo = combatUnitScript.protecao;
                visibilidadeAlvo = combatUnitScript.visibilidade;
                movimentoAlvo = combatUnitScript.movimento;

                // Modificadores relativos
//                distanciaAtiradorAlvo = medirDistânciaEntreGameObjects
//                inclinacaoAtiradorAlvo = medirAnguloEntreGameObjects

                int modificadoresDoAlvo = (adestramentoAlvo + protecaoAlvo + visibilidadeAlvo + movimentoAlvo + distanciaAtiradorAlvo + inclinacaoAtiradorAlvo);
                Debug.LogFormat("Modificadores do Alvo: Adestramento = {0}; Proteção = {1}; Visibilidade = {2}; Movimento = {3}; Distância = {4}; Inclinação {5}.", adestramentoAlvo,protecaoAlvo,visibilidadeAlvo,movimentoAlvo,distanciaAtiradorAlvo,inclinacaoAtiradorAlvo);

                int hitProbabilityValue = (modificadoresDoAtirador - modificadoresDoAlvo);                
                Debug.LogFormat("Valor de Referência da Probabilidade de Acerto = {0}", hitProbabilityValue);
                hitProbability = CalculateHitProbability(hitProbabilityValue);
                Debug.LogFormat("Probabilidade de Acerto = {0}%", hitProbability*100);

                int efetivoAtacante = Mathf.RoundToInt(combatUnitScript.efetivoAtual);
                int efetivoDefensor = Mathf.RoundToInt(targetObject.GetComponent<CombatUnit>().efetivoAtual);

                ntzProbability = CalculateNeutralizationProbability(hitProbability, efetivoDefensor, efetivoAtacante);
                Debug.LogFormat("Probabilidade de Neutralização = {0}%", ntzProbability*100);
                Debug.LogFormat("Quantidade de Testes = {0}", numberOfTests);

                for (int i = 1; i < numberOfTests+1; i++)
                {
                    int luckResult = DiceRoll.RollD100();
                    Debug.LogFormat("Rolagem Nr {0}, d100 = {1}", i, luckResult);

                    if (luckResult <= ntzProbability*100)
                    {
                        int finalDamage = 1;
                        combatUnitScript.TakeDamage(finalDamage);
                        Debug.LogFormat("{0} {1} neutralizou {2} militar de {3} {4}", targetUnit.UnitRaceType, targetObject.name, finalDamage, combatUnitScript.UnitRaceType, gameObject.name);
                    } else 
                    {
                        Debug.LogFormat("Ataque de {0} {1} falhou", targetUnit.UnitRaceType, targetObject.name);
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

        private float CalculateNeutralizationProbability(float singleHitProbability, int quantidadeDeAtiradores, int quantidadeDeAlvos)
        {
            Debug.LogFormat("Quantidade de Atiradores = {0}; Quantidade de Alvos = {1}.", quantidadeDeAtiradores,quantidadeDeAlvos); 

            if (quantidadeDeAtiradores <= quantidadeDeAlvos)
            {
                numberOfTests = quantidadeDeAtiradores;
                gruposDeAtiradores = 1.0f;
            } else
            {
                numberOfTests = quantidadeDeAlvos;
                gruposDeAtiradores = Mathf.RoundToInt(quantidadeDeAtiradores/quantidadeDeAlvos);
            }

            Debug.LogFormat("Grupos de Atiradores = {0} Homens / Alvo", gruposDeAtiradores);

            float ntzValue = 1 - Mathf.Pow((1 - singleHitProbability), gruposDeAtiradores);
            float ntzPercentage = Mathf.Round(ntzValue * 100.0f) * 0.01f;
            return ntzPercentage;
        }

    }   
}