using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeScript : MonoBehaviour
{
    [SerializeField] private float estimatedMutationRate = 25f;
    [SerializeField] private GameObject treePrefab;
    [SerializeField] private Terrain terrain;

    public Tree tree = new Tree();

    public float currentHeight;

    private bool isWaitingForOffspring;
    private float amountOfChilds;

    private void Start()
    {
        terrain = GameObject.Find("Terrain").GetComponent<Terrain>();
        if (tree.parentTree != null)
        {
            //print("Ich habe einen Vater");
            float mutationRate = Random.Range(-estimatedMutationRate, estimatedMutationRate + 1);
            tree.adultHeight = tree.parentTree.adultHeight * (1 + mutationRate / 100);

            mutationRate = Random.Range(-estimatedMutationRate, estimatedMutationRate + 1);
            tree.spreadRadius = tree.parentTree.spreadRadius * (1 + mutationRate / 100);

            mutationRate = Random.Range(-estimatedMutationRate, estimatedMutationRate + 1);
            tree.reproductionRate = tree.parentTree.reproductionRate * (1 + mutationRate / 100);

            mutationRate = Random.Range(-estimatedMutationRate, estimatedMutationRate + 1);
            tree.growthRate = tree.parentTree.growthRate * (1 + mutationRate / 100);

            mutationRate = Random.Range(-estimatedMutationRate, estimatedMutationRate + 1);
            tree.howManyChilds = tree.parentTree.howManyChilds * (1 + mutationRate / 100);

            transform.localScale = Vector3.one / 10;

            //print("Höhe: " + tree.adultHeight + " spreadRadius: " + tree.spreadRadius + " reproductionRate: " + tree.reproductionRate + " growthRate: " + tree.growthRate + " Mein Vater: " + tree.parentTree + " location: " + tree.location);
        }
        else
            tree.location = transform.position;
    }
    private void Update()
    {
        if (!tree.isGrownUp && !(transform.localScale.x <= tree.adultHeight + 0.05f && (transform.localScale.x >= tree.adultHeight - 0.05f)))
        {
            transform.localScale += Vector3.one * Time.deltaTime * tree.growthRate;
            currentHeight = transform.localScale.x;
        }
      
        else if (!tree.isGrownUp)
        {
            currentHeight = tree.adultHeight;
            tree.isGrownUp = true;
        }
          

        if (!isWaitingForOffspring && tree.isGrownUp)
        {
            StartCoroutine(waitForOffSpring());
        }

        if (gameObject.transform.childCount == 1)
            Destroy(gameObject, 1f);
    }

    IEnumerator waitForOffSpring()
    {
        if (Mathf.Round(tree.howManyChilds) <= amountOfChilds)
            yield break;

        isWaitingForOffspring = true;
        float factor = Random.Range(-15, 16);
        //print("IsWaitingForOffSpring");
        yield return new WaitForSeconds(tree.reproductionRate * (1 + factor / 100));
        //print("Done");
        Vector3 locationForNewTree = Vector3.zero;
        while (true){
            float x = Random.Range(-tree.spreadRadius, tree.spreadRadius + 1);
            float z = Random.Range(-tree.spreadRadius, tree.spreadRadius + 1);
            locationForNewTree = tree.location;
            locationForNewTree.x += x;
            locationForNewTree.z += z;
            if (IsPointOutsideAllColliders(locationForNewTree) && (tree.location - locationForNewTree).magnitude <= tree.spreadRadius )
            {
                break;
            }
        }
        Vector3 terrainPos = terrain.transform.InverseTransformPoint(locationForNewTree);

        int mapX = Mathf.RoundToInt((terrainPos.x / terrain.terrainData.size.x) * terrain.terrainData.heightmapResolution);
        int mapZ = Mathf.RoundToInt((terrainPos.z / terrain.terrainData.size.z) * terrain.terrainData.heightmapResolution);

        mapX = Mathf.Clamp(mapX, 0, terrain.terrainData.heightmapResolution - 1);
        mapZ = Mathf.Clamp(mapZ, 0, terrain.terrainData.heightmapResolution - 1);

        locationForNewTree.y = terrain.terrainData.GetHeight(mapX, mapZ);

        GameObject kidTree = Instantiate(treePrefab, locationForNewTree, Quaternion.identity) as GameObject;
        kidTree.GetComponent<TreeScript>().tree.parentTree = this.tree;
        kidTree.GetComponent<TreeScript>().tree.location = locationForNewTree;
        kidTree.transform.SetParent(GameObject.Find("Trees").transform);
        isWaitingForOffspring = false;
        amountOfChilds++;

    }

    private bool IsPointOutsideAllColliders(Vector3 point, float radius = 0.01f)
    {
        Collider[] colliders = Physics.OverlapSphere(point, radius);
        return colliders.Length == 0;
    }
}
