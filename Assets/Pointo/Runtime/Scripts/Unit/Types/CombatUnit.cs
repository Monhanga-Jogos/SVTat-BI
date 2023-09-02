using UnityEngine;
using UnityEngine.AI;

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
        
        void Update()
        {
            if (efetivoAtual <= 0)
            {
                UnitTargetHandler.currentState = UnitTargetHandler.UnitState.Destroyed;
                GetComponent<NavMeshAgent>().isStopped = true;
                GetComponent<AudioSource>().Stop();
                GetComponent<MeshRenderer>().material = unitSo.destroyedMat;
                GetComponent<BoxCollider>().enabled = false;
                this.enabled = false;                
            }
        }
        private void HandleObjectClicked(GameObject targetObject)
        {
            if (!IsSelected) return;
            
            Unit targetUnit = targetObject.GetComponent<Unit>();
            if (targetUnit == null) return;
            
            UnitTargetHandler.currentState = UnitTargetHandler.UnitState.Fighting;
            
            Debug.LogFormat("{0} {1} está avançando para atacar {2} {3}", UnitRaceType, gameObject.name, targetUnit.UnitRaceType, targetObject.name);
        }

        public void TakeDamage (int damageTaken)
        {
            if (efetivoAtual > 0)
            {
            efetivoAtual -= damageTaken;
            }
        }
    }   
}