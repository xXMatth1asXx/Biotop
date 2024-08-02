using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deer : Animal
{
    public Deer()
    {
        size = 1f;
        speed = 2.5f;
        energy = 4f;
        hunger = 60f;
        thirst = 60f;
        growthRate = 0.015f;
        reproductionRate = 10f;
        visualRange = 80f;
        strayDistance = 15f;
        isGrownUp = false;
        isMale = true;
    }
}
