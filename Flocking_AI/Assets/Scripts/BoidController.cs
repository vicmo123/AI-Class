using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoidController : MonoBehaviour
{
    GameManager gameManager;
    LineRenderer lineRenderer;

    private readonly float minX = 0;
    private readonly float maxX = 107.15f;
    private readonly float minY = 0;
    private readonly float maxY = 50.25f;

    Vector3 baseRotation = Vector3.zero;
    Vector2 acceleration = Vector2.zero;
    Vector2 velocity = Vector2.zero;

    public GameObject[] neighbourhoods;

    private Vector2 Position { get { return gameObject.transform.position; } set { gameObject.transform.position = value; } }

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        gameManager.showArrows = false;
        gameManager.showNeighboorhoods = false;
    }

    private void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            neighbourhoods[1].gameObject.SetActive(false);
        }

        lineRenderer = this.gameObject.GetComponent<LineRenderer>();

        float angle = Random.Range(0, 2 * Mathf.PI);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle) + baseRotation);
        velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    private void Update()
    {
        //check if in sight for everything except for repulsion if not strange collisisons occure
        var boidsInRepulsionZone = gameManager.boidsList.Where(o => DistanceTo(o) <= gameManager.radiusRepulsion && o != this /*&& CheckIfInDeadZone(velocity, (o.Position - this.Position).normalized) < 180.0f - 15f*/);
        var boidsInOrientationZone = gameManager.boidsList.Where(o => DistanceTo(o) <= gameManager.radiusOrientation && o != this && CheckIfInDeadZone(velocity, (o.Position - this.Position).normalized) < 180.0f - 15f);
        var boidsInAttractionZone = gameManager.boidsList.Where(o => DistanceTo(o) <= gameManager.radiusAttraction && o != this && CheckIfInDeadZone(velocity, (o.Position - this.Position).normalized) < 180.0f - 15f);

        Flock(boidsInRepulsionZone, boidsInOrientationZone, boidsInAttractionZone);
        UpdateVelocity();
        UpdatePosition();
        UpdateRotation();
        WrapAround();

        DrawVelocityArrow();
        DrawNeighbourhoods();
    }

    private void Flock(IEnumerable<BoidController> neighboursSmall, IEnumerable<BoidController> neighboursMedium, IEnumerable<BoidController> neighboursBig)
    {
        var separation = Separation(neighboursSmall);
        var alignment = Alignment(neighboursMedium);
        var cohesion = Cohesion(neighboursBig);

        acceleration = gameManager.alignmentPonderation * alignment + gameManager.cohesionPonderation * cohesion + gameManager.separationPonderation * separation;
    }

    public void UpdateVelocity()
    {
        velocity += acceleration;
        velocity = ClampMagnitude(velocity, gameManager.maxSpeed);
    }

    private void UpdatePosition()
    {
        Position += velocity * Time.deltaTime;
    }

    private void UpdateRotation()
    {
        var angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle) + baseRotation);
    }

    private Vector2 Alignment(IEnumerable<BoidController> boids)
    {
        var velocity = Vector2.zero;
        if (!boids.Any()) return velocity;

        foreach (BoidController boid in boids)
        {
            velocity += boid.velocity;
        }
        velocity /= boids.Count();

        var steer = Steer(velocity.normalized * gameManager.maxSpeed);

        return steer;
    }

    private Vector2 Cohesion(IEnumerable<BoidController> boids)
    {
        if (!boids.Any()) return Vector2.zero;

        var sumPositions = Vector2.zero;
        foreach (BoidController boid in boids)
        {
            sumPositions += boid.Position;
        }
        var average = sumPositions / boids.Count();
        var direction = average - Position;

        var steer = Steer(direction.normalized * gameManager.maxSpeed);

        return steer;
    }

    private Vector2 Separation(IEnumerable<BoidController> boids)
    {
        var direction = Vector2.zero;

        if (!boids.Any()) return direction;

        foreach (BoidController boid in boids)
        {
            var difference = Position - boid.Position;
            direction += difference.normalized / difference.magnitude;
        }
        direction /= boids.Count();

        var steer = Steer(direction.normalized * gameManager.maxSpeed);

        return steer;
    }

    private Vector2 Steer(Vector2 desired)
    {
        var steer = desired - velocity;
        steer = ClampMagnitude(steer, gameManager.maxAcceleration);

        return steer;
    }

    private float DistanceTo(BoidController boid)
    {
        return Vector3.Distance(boid.transform.position, Position);
    }

    private Vector2 ClampMagnitude(Vector2 baseVector, float maxMagnitude)
    {
        if (baseVector.sqrMagnitude > maxMagnitude * maxMagnitude)
        {
            baseVector = baseVector.normalized * maxMagnitude;
        }
        return baseVector;
    }

    private void WrapAround()
    {
        if (Position.x < minX)
            //exit right
            Position = new Vector2(maxX, Position.y);
        if (Position.x > maxX)
            //exit left
            Position = new Vector2(minX, Position.y);
        if (Position.y < minY)
            //exit bottom
            Position = new Vector2(Position.x, maxY);
        if (Position.y > maxY)
            //exit top
            Position = new Vector2(Position.x, minY);
    }

    private float CheckIfInDeadZone(Vector2 velocity, Vector2 boidToNeighbour)
    {
        return Vector2.Angle(velocity, boidToNeighbour);
    }
    
    private void DrawVelocityArrow()
    {

        if (gameManager.showArrows == true)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPositions(new Vector3[] { this.Position, (this.Position + velocity) });
            Debug.Log("Draw arrow");
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    private void DrawNeighbourhoods()
    {
        if (gameManager.showNeighboorhoods)
        {
            for (int i = 0; i < 3; i++)
            {
                neighbourhoods[1].gameObject.SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                neighbourhoods[1].gameObject.SetActive(false);
            }
        }
    }
}
