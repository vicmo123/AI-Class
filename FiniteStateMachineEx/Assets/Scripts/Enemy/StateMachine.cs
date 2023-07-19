using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StateMachine : MonoBehaviour
{
    GridManager gm;

    private GameObject enemyPrefab;
    private GameObject enemyInstance;

    enum States
    {
        wander,
        inRange,
        inSight,
        chase,
        playerCaught
    }

    States currentState;

    private GameObject playerInstance;

    private float chaseTreshold = 4f;
    //sight of view is equal to 20 degres from the local y saxis of the enemey
    private float sightOfView = 30f;

    private float speed = 3f;
    private float rotationSpeed = 250f;

    public List<Vector2> pathToFollow;
    public int pathCount = 0;

    public LayerMask layerMask;

    //from collider data
    public float radius = 0.5f;

    public void Initialize()
    {
        enemyPrefab = Resources.Load<GameObject>("Prefabs/Enemy");
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GridManager>();
    }

    public void SecondInitialize()
    {
        enemyInstance = GameObject.Instantiate(enemyPrefab, gm.GenerateRandomPos(), Quaternion.identity);

        playerInstance = GameObject.FindGameObjectWithTag("Player").gameObject;

        CreatPath();

        currentState = States.wander;
    }

    private void Update()
    {
        switch (currentState)
        {
            case States.wander:
                WanderUpdate();
                break;
            case States.inRange:
                InRangeUpdate();
                break;
            case States.inSight:
                InSightUpdate();
                break;
            case States.chase:
                ChaseUpdate();
                break;
            case States.playerCaught:
                PlayerCaughtUpdate();
                break;
            default:
                Debug.Log("Unhandeled case: " + currentState);
                break;
        }

        if (CheckCollisionWithPlayer())
        {
            currentState = States.playerCaught;
        }
    }

    private void WanderUpdate()
    {
        FollowPath();

        if ((playerInstance.transform.position - enemyInstance.transform.position).magnitude < chaseTreshold)
            currentState = States.inRange;
    }

    private void InRangeUpdate()
    {
        //Todo : Raycast to see if is the player is in sight
        //If is not in range anymore return to wander

        FollowPath();

        if (CanSeeTarget(playerInstance.transform, sightOfView, chaseTreshold))
        {
            currentState = States.inSight;
        }
    }

    private void InSightUpdate()
    {
        //Player see so start chase
        StartCoroutine(ChaseTargetForSomeTime());
        currentState = States.chase;
    }

    private void ChaseUpdate()
    {
        //Handeled in Update every frame
    }

    private void PlayerCaughtUpdate()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void CreatPath(GameObject player = null)
    {
        var target = player == null ? gm.GenerateRandomPos() : GetCurrentTilePos(player);
        List<Spot> spots = Astar.FindshortestPath(GetCurrentTilePos(enemyInstance), target);

        pathToFollow = new List<Vector2>();

        if (spots != null)
        {
            for (int i = spots.Count - 1; i >= 0; i--)
            {
                pathToFollow.Add(new Vector2(spots[i].x, spots[i].y));
            }
        }
    }

    private void FollowPath()
    {
        if (pathToFollow != null)
        {
            if (pathCount < pathToFollow.Count - 1)
            {
                enemyInstance.transform.position = Vector2.MoveTowards(enemyInstance.transform.position, pathToFollow[pathCount], speed * Time.deltaTime);

                Vector2 movementDirection = (pathToFollow[pathCount] - (Vector2)enemyInstance.transform.position).normalized;
                if (movementDirection != Vector2.zero)
                {
                    Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, movementDirection);
                    enemyInstance.transform.rotation = Quaternion.RotateTowards(enemyInstance.transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
                }

                if ((Vector2)enemyInstance.transform.position == pathToFollow[pathCount])
                {
                    pathCount++;
                }
            }
            else
            {
                CreatPath();
                pathCount = 0;
            }
        }
        else
        {
            pathCount = 0;
        }
    }

    private void ChaseTarget()
    {
        CreatPath(playerInstance);
        FollowPath();
    }

    private void GoAway()
    {
        CreatPath();
        FollowPath();

        currentState = States.wander;
    }

    IEnumerator ChaseTargetForSomeTime()
    {
        float timeChaseFinishes = Time.time + 7;

        while (Time.time < timeChaseFinishes)
        {
            ChaseTarget();
            yield return null;
        }

        GoAway();
    }

    private Vector2 GetCurrentTilePos(GameObject instance)
    {
        return new Vector2(Mathf.Round(instance.transform.position.x), Mathf.Round(instance.transform.position.y));
    }

    bool CanSeeTarget(Transform target, float viewAngle, float viewRange)
    {
        Vector3 toTarget = target.position - enemyInstance.transform.position;
        //Debug.Log(Vector3.Angle(enemyInstance.transform.up, toTarget));

        if (Vector3.Angle(enemyInstance.transform.up, toTarget) <= viewAngle)
        {
            //Debug.Log("In sight");

            RaycastHit2D hit = new RaycastHit2D();
            hit = Physics2D.Raycast(enemyInstance.transform.position, toTarget.normalized, chaseTreshold, layerMask);

            Debug.DrawRay(enemyInstance.transform.position, toTarget.normalized * chaseTreshold, Color.red, 10000f);
            if (hit)
            {
                if (hit.collider.gameObject.tag == "Player")
                {
                    Debug.Log("Hit");
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                Debug.Log("Not hit");
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    private bool CheckCollisionWithPlayer()
    {
        Vector2 playerToEnemy = playerInstance.transform.position - enemyInstance.transform.position;

        if ((playerToEnemy).magnitude <= (radius * enemyInstance.transform.localScale.x) + (radius * playerInstance.transform.localScale.x))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
