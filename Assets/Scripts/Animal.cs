using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal
{
    public float size;
    public float speed;
    public float energy;
    public float hunger;
    public float thirst;
    public float growthRate;
    public float reproductionRate;
    public float visualRange;
    public float strayDistance;
    public bool isGrownUp;
    public bool isMale;
    public Animal[] parentAnimals = new Animal[2];
}
