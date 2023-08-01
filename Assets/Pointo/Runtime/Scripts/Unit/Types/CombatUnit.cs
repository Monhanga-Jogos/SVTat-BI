using UnityEngine;

namespace Pointo.Unit
{
    public class CombatUnit : Unit
    {
       
        private new void OnEnable()
        {
            base.OnEnable();
            PointoController.Actions.onObjectRightClicked += HandleObjectClicked;
            currentHealth = unitSo.maxHealth;
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

        public void TakeDamage (float damageTaken)
        {
            if (currentHealth > 0)
            {
            currentHealth -= damageTaken;
            } else
            {
            currentHealth = 0;
            UnitTargetHandler.currentState = UnitTargetHandler.UnitState.Destroyed;
            GetComponent<MeshRenderer>().material = unitSo.destroyedMat;
            }
        }
    }   
}