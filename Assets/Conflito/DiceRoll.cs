using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceRoll : MonoBehaviour
{
    public static float RollD20 ()
    {
        int d20Result = Random.Range(1, 20);
        
        float diceResult = (float)d20Result;

        return diceResult;
    }

    public static int AnyDiceRoll(int dSides, int rolls)
    {
	    int rollResult = 0;
  
  	    for (int i = 0; i < rolls; i++)
  		    rollResult += Random.Range(1, dSides + 1);
	
  	    return rollResult;
    }
}
