﻿using UnityEngine;

namespace Pointo.Unit
{
    public class CombatUnit : Unit
    {
        //Permanent Modifiers
        public int adestramento;
        public int raioDeAcao;
        public int pontaria;
        public int protecao;

        //Temporary Modifiers
        public int concentracao;
        public int vigor;
        public int movimento;
        public int visibilidade;
        

       
        private new void OnEnable()
        {
            base.OnEnable();
            PointoController.Actions.onObjectRightClicked += HandleObjectClicked;
            efetivoAtual = unitSo.efetivoCompleto;
        }

        private new void OnDisable()
        {
            base.OnDisable();
            PointoController.Actions.onObjectRightClicked -= HandleObjectClicked;
        }
        
        private void HandleObjectClicked(GameObject targetObject)
        {
            if (!IsSelected) return;
            
            Unit targetUnit = targetObject.GetComponent<Unit>();
            if (targetUnit == null) return;
            
            UnitTargetHandler.currentState = UnitTargetHandler.UnitState.Fighting;
            
            Debug.LogFormat("{0} is moving towards {1}", UnitRaceType, targetUnit.UnitRaceType);
        }

        public void TakeDamage (int damageTaken)
        {
            if (efetivoAtual > 0)
            {
            efetivoAtual -= damageTaken;
            } else
            {
            efetivoAtual = 0;
            UnitTargetHandler.currentState = UnitTargetHandler.UnitState.Destroyed;
            GetComponent<MeshRenderer>().material = unitSo.destroyedMat;
            }
        }
    }   
}