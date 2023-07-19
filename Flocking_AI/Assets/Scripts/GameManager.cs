using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    GameObject boidPrefab;
    public List<BoidController> boidsList { get; set; }
    private int numBoids = 35;

    [SerializeField] public bool showArrows = false;
    [SerializeField] public bool showNeighboorhoods = false;

    [Range(1, 5), SerializeField] public float radiusRepulsion = 5f;
    [Range(1, 10), SerializeField] public float radiusOrientation = 10f;
    [Range(1, 20), SerializeField] public float radiusAttraction = 20f;

    [Range(0, 10), SerializeField] public float maxSpeed = 10f;
    [Range(.1f, .5f), SerializeField] public float maxAcceleration = 4f;

    [Range(0, 3), SerializeField] public float separationPonderation = 1f;
    [Range(0, 3), SerializeField] public float cohesionPonderation = 1f;
    [Range(0, 3), SerializeField] public float alignmentPonderation = 1f;


    private void Awake()
    {
        boidPrefab = Resources.Load<GameObject>("Prefabs/Boid");
        boidsList = new List<BoidController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numBoids; i++)
        {
            GameObject boid = GameObject.Instantiate(boidPrefab, GenerateRandomPos(), Quaternion.identity);

            boidsList.Add(boid.GetComponent<BoidController>());
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Vector2 GenerateRandomPos()
    {
        var excludeX = new HashSet<int>();
        var excludeY = new HashSet<int>();

        foreach (var boid in boidsList)
        {
            excludeX.Add((int)boid.transform.position.x);
            excludeY.Add((int)boid.transform.position.y);
        }

        var rangeX = Enumerable.Range(1, 100).Where(i => !excludeX.Contains(i));
        var rangeY = Enumerable.Range(1, 50).Where(i => !excludeY.Contains(i));

        var randX = new System.Random();
        var randY = new System.Random();

        int indexX = randX.Next(0, 100 - excludeX.Count);
        int indexY = randY.Next(0, 50 - excludeY.Count);

        return new Vector2(rangeX.ElementAt(indexX), rangeY.ElementAt(indexY));
    }
}
