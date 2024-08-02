using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree 
{
    public float adultHeight = 1f;
    public float spreadRadius = 10f;
    public float reproductionRate = 20f;
    public float growthRate = 0.1f;
    public float howManyChilds = 3f;
    public bool isGrownUp = false;
    public Vector3 location = Vector3.zero;
    public Tree parentTree = null;
}
