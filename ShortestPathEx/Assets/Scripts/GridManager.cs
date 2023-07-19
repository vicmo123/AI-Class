using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Vector2))]
[RequireComponent(typeof(Vector2))]
public class GridManager : MonoBehaviour
{ 
    enum GamePhase
    {
        Setup,
        Choose1,
        Choose2,
        GetPath,
        Reset
    }

    GamePhase gamephases;

    [SerializeField] private int width, height;

    [SerializeField] private Tile tilePrefab;

    [SerializeField] private Transform cam;

    private Dictionary<Vector2, Tile> tiles;

    private Vector2 start;
    bool startSet = false;
    private Vector2 finish;
    bool finishSet = false;

    public float timeBeforeColoration = 3f;
    public int numberObstacles = 24;

    private void Awake()
    {
        gamephases = GamePhase.Setup;

        GenerateGrid();

        Astar.MapHeight = height;
        Astar.MapWidth = width;
    }

    private void Update()
    {
        switch (gamephases)
        {
            case GamePhase.Setup:
                SetupUpdate();
                break;
            case GamePhase.Choose1:
                ChooseStartUpdate();
                break;
            case GamePhase.Choose2:
                ChooseFinishUpdate();
                break;
            case GamePhase.GetPath:
                GetPathUpdate();
                break;
            case GamePhase.Reset:
                ResetUpdate();
                break;
            default:
                Debug.Log("Unhandeled case: " + gamephases);
                break;
        }
    }

    private void SetupUpdate()
    {
        Tile.mouseClickedEvent.AddListener(ChooseStartAndFinish);
        gamephases = GamePhase.Choose1;
    }

    private void ChooseStartUpdate()
    {
        if(startSet)
        {
            gamephases = GamePhase.Choose2;
        }
    }

    private void ChooseFinishUpdate()
    {
        if (finishSet)
        {
            gamephases = GamePhase.GetPath;
        }
    }

    private void GetPathUpdate()
    {
        Tile.mouseClickedEvent.RemoveAllListeners();

        SetWalls();

        this.StartCoroutine(ColorPath(Astar.FindshortestPath(start, finish, GetPosWalls())));

        gamephases = GamePhase.Reset;
    }

    private void ResetUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        } 
    }


    void GenerateGrid()
    {
        tiles = new Dictionary<Vector2, Tile>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tilePrefab.GetComponent<SpriteRenderer>();
                var spawnedTile = Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";

                var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                spawnedTile.Initialize(isOffset);


                tiles[new Vector2(x, y)] = spawnedTile;
            }
        }

        cam.transform.position = new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f, -10);
    }

    public Tile GetTileAtPosition(Vector2 pos)
    {
        if (tiles.TryGetValue(pos, out var tile)) return tile;
        return null;
    }

    public void SetWalls()
    {
        //Walls
        for (int i = 0; i < numberObstacles; i++)
        { 
            GetTileAtPosition(GenerateRandomPos()).SetAsObstacle();
        }
    }

    IEnumerator ColorPath(List<Spot> chosenPath)
    {
        yield return new WaitForSeconds(timeBeforeColoration);

        if (chosenPath != null)
        {
            foreach (Spot spot in chosenPath)
            {
                GetTileAtPosition(new Vector2(spot.x, spot.y)).SetColor(Color.red);
            }
        }
    }

    public List<Vector2> GetPosWalls()
    {
        List<Vector2> positions = new List<Vector2>();

        foreach (Tile tile in tiles.Values)
        {
            if(tile.isObstacle == true)
            {
                positions.Add(tile.transform.position);
            }
        }

        return positions;
    }

    private Vector2 GenerateRandomPos()
    {
        var Map = new HashSet<Vector2>();

        foreach  (Tile t in tiles.Values)
        {
            Map.Add(t.transform.position);
        }

        Map.Remove(start);
        Map.Remove(finish);

        var range = Enumerable.Range(0, Map.Count - 1);

        var rand = new System.Random();
        int index = rand.Next(0, Map.Count - 1);
        
        return Map.ElementAt(range.ElementAt(index));
    }

    private void ChooseStartAndFinish(Tile tile)
    {
        if(gamephases == GamePhase.Choose1)
        {
            start = tile.transform.position;
            startSet = true;

            GetTileAtPosition(tile.transform.position).SetColor(Color.blue);
        }
        else if(gamephases == GamePhase.Choose2)
        {
            finish = tile.transform.position;
            finishSet = true;

            GetTileAtPosition(tile.transform.position).SetColor(Color.yellow);
        }
    }
}