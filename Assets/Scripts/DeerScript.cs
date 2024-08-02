using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class DeerScript : MonoBehaviour
{
    [SerializeField] private float estimatedMutationRate = 25f;
    [SerializeField] private GameObject deerPrefab;
    [SerializeField] private Terrain terrain;

    public Deer deer = new Deer();
 
    private NavMeshAgent deerAgent;
    private Vector3 toStrayPoint;
    public float timeSinceLastMeal = 0;
    private float timeSinceLastTrink = 0;
    public float currentSize;
    private bool wantsToEat = false;
    private bool isWaitingForOffspring = false;

    private TextMeshPro text;

    private void Start()
    {
        deerAgent = gameObject.GetComponent<NavMeshAgent>();
        terrain = GameObject.Find("Terrain").GetComponent<Terrain>();
        text = gameObject.GetComponentInChildren<TextMeshPro>();

        if (!(deer.parentAnimals[0] == null))
        {
            deer.size = deer.parentAnimals[RandomParent()].size * MutationRate();
            deer.speed = deer.parentAnimals[RandomParent()].speed * MutationRate();
            deer.energy = deer.parentAnimals[RandomParent()].energy * MutationRate();
            deer.hunger = deer.parentAnimals[RandomParent()].hunger * MutationRate();
            deer.thirst = deer.parentAnimals[RandomParent()].thirst * MutationRate();
            deer.growthRate = deer.parentAnimals[RandomParent()].growthRate * MutationRate();
            deer.visualRange = deer.parentAnimals[RandomParent()].visualRange * MutationRate();
            deer.strayDistance = deer.parentAnimals[RandomParent()].strayDistance * MutationRate();

            if (RandomParent() == 0)
            {
                deer.isMale = true;
            }
            else
            {
                print(name + "Bin weiblich");
                deer.isMale = false;
            }

            transform.localScale = Vector3.one / 3;
        }
        else
        {
            if (RandomParent() == 0)
            {
                deer.isMale = true;
            }
            else
            {
                print(name + "Bin weiblich");
                deer.isMale = false;
            }

            deer.size *= MutationRate();
            deer.speed *= MutationRate();
            deer.energy *= MutationRate();
            deer.hunger *= MutationRate();
            deer.thirst *= MutationRate();
            deer.growthRate *= MutationRate();
            deer.visualRange *= MutationRate();
            deer.strayDistance *= MutationRate();
            deer.isGrownUp = true;
         
        }

        deerAgent.speed = deer.speed;

        MoveToStrayPoint(Vector3.zero);
        print("Größe: " + deer.size + " Speed: " + deer.speed + " Energie: " + deer.energy + " Hunger: " + deer.hunger + " Durst: " + deer.thirst + " Wachstumsrate: " + deer.growthRate + " VisualRange: " + " strayDistance: " + deer.strayDistance);
    }

    private int RandomParent()
    {
        return Random.Range(0, 2);
    }

    private float MutationRate()
    {
        float mutationRate = Random.Range(-estimatedMutationRate, estimatedMutationRate + 1);
        return 1 + mutationRate / 100;
    }

    private void Update()
    {
        HungerAndThirst();

        if (!deer.isGrownUp && !(transform.localScale.x <= deer.size + 0.05f && (transform.localScale.x >= deer.size - 0.05f)))
        {
            transform.localScale += Vector3.one * Time.deltaTime * deer.growthRate;
            currentSize = transform.localScale.x;
        }

        else if (!deer.isGrownUp)
        {
            currentSize = deer.size;
            deer.isGrownUp = true;
        }

        if (Vector3.Distance(transform.position, toStrayPoint) < 0.5f)
        {
            MoveToStrayPoint(Vector3.zero);
        }

        if (deer.isMale && !isWaitingForOffspring && deer.isGrownUp)
        {
            StartCoroutine(waitForOffSpring());
        }
        if (deer.isMale && deer.isGrownUp)
            text.text = "♂  Grown Up Hunger: " + Mathf.Round(deer.hunger/timeSinceLastMeal);
        if (!deer.isMale && deer.isGrownUp)
            text.text = "♀  Grown Up Hunger: " + Mathf.Round(deer.hunger / timeSinceLastMeal);
        if (deer.isMale && !deer.isGrownUp)
            text.text = "♂  Child Hunger: " + Mathf.Round(deer.hunger / timeSinceLastMeal);
        if (!deer.isMale && !deer.isGrownUp)
            text.text = "♀  Child Hunger: " + Mathf.Round(deer.hunger / timeSinceLastMeal);


    }

    IEnumerator waitForOffSpring()
    {
        //print("waitforOffspring");
        isWaitingForOffspring = true;
        yield return new WaitForSeconds(deer.reproductionRate);
       

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, deer.visualRange);
        float minDistance = Mathf.Infinity;
        GameObject closestDeer = null;

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.CompareTag("Deer"))
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < minDistance && hitCollider.gameObject != this.gameObject)
                {
                    if ((deer.isMale && !hitCollider.gameObject.GetComponent<DeerScript>().deer.isMale) || (!deer.isMale && hitCollider.gameObject.GetComponent<DeerScript>().deer.isMale))
                    {
                        minDistance = distance;
                        closestDeer = hitCollider.gameObject;
                    }
                }
            }
        }

        if (closestDeer != null)
        {
            print("Das nächste Reh ist " + minDistance + " Einheiten entfernt.");
            MoveToStrayPoint(closestDeer.transform.position);
            //StartCoroutine(Eat(closestDeer));
        }
        else
        {
            print("Kein anderes Reh in der Nähe");
            isWaitingForOffspring = false;
            yield break;
        }

        while (Vector3.Distance(transform.position, closestDeer.transform.position) >= 3f)
        {
            closestDeer.GetComponent<DeerScript>().MoveToStrayPoint(closestDeer.transform.position);
            yield return new WaitForSeconds(0.2f);
        }

        GameObject kidDeer = Instantiate(deerPrefab, gameObject.transform.position, Quaternion.identity) as GameObject;
        kidDeer.GetComponent<DeerScript>().deer.parentAnimals[0] = deer;
        kidDeer.GetComponent<DeerScript>().deer.parentAnimals[1] = closestDeer.GetComponent<DeerScript>().deer;
        print(kidDeer.GetComponent<DeerScript>().deer.parentAnimals + " Kind ist geboren");
        kidDeer.transform.SetParent(GameObject.Find("Deers").transform);
        isWaitingForOffspring = false;

      
    }



    private void HungerAndThirst()
    {
        timeSinceLastMeal += Time.deltaTime;
        timeSinceLastTrink += Time.deltaTime;

        if (timeSinceLastMeal >= deer.hunger * 0.66 && !wantsToEat)
        {
            wantsToEat = true;
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, deer.visualRange);
            float minDistance = Mathf.Infinity;
            GameObject closestTree = null;

            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.gameObject.CompareTag("Tree"))
                {
                    float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                    if (distance < minDistance && hitCollider.gameObject.GetComponent<TreeScript>().currentHeight <= currentSize)
                    {
                        minDistance = distance;
                        closestTree = hitCollider.gameObject;
                    }
                }
            }

            if (closestTree != null)
            {
                print("Der nächste Baum ist " + minDistance + " Einheiten entfernt.");
                MoveToStrayPoint(closestTree.transform.position);
                StartCoroutine(Eat(closestTree));
            }
            else
            {
                wantsToEat = false;
            }
                
        }

        if (timeSinceLastMeal + 0.01f >= deer.hunger && timeSinceLastMeal - 0.01f <= deer.hunger)
        {
            print("Hunger!!!");
            Destroy(gameObject);
        }
    }

    IEnumerator Eat(GameObject tree)
    {
        while (Vector3.Distance(transform.position, tree.transform.position) >= 4f)
        {
            //print("Warte");
            yield return new WaitForSeconds(0.2f);
        }

        GameObject randomChild = null;
        while (true)
        {
            randomChild = tree.transform.GetChild(Random.Range(0, tree.transform.childCount - 1)).gameObject;
            if (!randomChild.CompareTag("Log"))
            {
                break;
            }
        }


        Destroy(randomChild, 1f);

        print("Ich mag essen");
        timeSinceLastMeal = 0;

        MoveToStrayPoint(Vector3.zero);

        wantsToEat = false;
    }

    public void MoveToStrayPoint(Vector3 point)
    {
        if (point == Vector3.zero)
        {
            while (true)
            {
                float x = Random.Range(-deer.strayDistance, deer.strayDistance);
                float z = Random.Range(-deer.strayDistance, deer.strayDistance);
                toStrayPoint = transform.position;
                toStrayPoint.x += x;
                toStrayPoint.z += z;
                if (Vector3.Distance(transform.position, toStrayPoint) <= deer.strayDistance)
                    break;
            }
        }
        else
        {
            toStrayPoint = point;
        }
        Vector3 terrainPos = terrain.transform.InverseTransformPoint(toStrayPoint);

        int mapX = Mathf.RoundToInt((terrainPos.x / terrain.terrainData.size.x) * terrain.terrainData.heightmapResolution);
        int mapZ = Mathf.RoundToInt((terrainPos.z / terrain.terrainData.size.z) * terrain.terrainData.heightmapResolution);

        mapX = Mathf.Clamp(mapX, 0, terrain.terrainData.heightmapResolution - 1);
        mapZ = Mathf.Clamp(mapZ, 0, terrain.terrainData.heightmapResolution - 1);

        toStrayPoint.y = terrain.terrainData.GetHeight(mapX, mapZ);
        Debug.DrawLine(transform.position, toStrayPoint, Color.red, 5f);
        deerAgent.SetDestination(toStrayPoint);
    }


}
