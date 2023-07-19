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
    [SerializeField] private int width, height;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Transform cam;

    private Dictionary<Vector2, Tile> tiles;
    private List<Vector2> outsideWallsPos;
    public Transform PlayerSpawn;

    public int numberObstacles = 24;

    private void Awake()
    {
        outsideWallsPos = new List<Vector2>();

        GenerateMap();

        Astar.MapHeight = height;
        Astar.MapWidth = width;
    }

    private void GenerateMap()
    {
        GenerateGrass();
        //GeneratePaths();
        GenerateWalls();

        //Give all the wall to Astar
        Astar.wallPos = GetPosWalls();
    }

    void GenerateGrass()
    {
        tiles = new Dictionary<Vector2, Tile>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tilePrefab.GetComponent<SpriteRenderer>();
                var spawnedTile = Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";

                spawnedTile.transform.SetParent(gameObject.transform);
                spawnedTile.Initialize();

                tiles[new Vector2(x, y)] = spawnedTile;
            }
        }

        cam.transform.position = new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f, -10);
    }

    void GeneratePaths()
    {
        for (int i = 3; i < width - 2; i++)
        {
            GetTileAtPosition(new Vector2(i, 3)).SetAsPath();
            GetTileAtPosition(new Vector2(i, 4)).SetAsPath();
        }
    }

    public void GenerateWalls()
    {
        //outside walls
        for (int i = 0; i < height; i++)
        {
            Vector2 posLeftWall = new Vector2(0, i);
            Vector2 posRightWall = new Vector2(width - 1, i);

            GetTileAtPosition(posLeftWall).SetAsObstacle();
            GetTileAtPosition(posRightWall).SetAsObstacle();

            outsideWallsPos.Add(posLeftWall);
            outsideWallsPos.Add(posRightWall);
        }

        for (int i = 0; i < width; i++)
        {
            Vector2 posBottomWall = new Vector2(i, 0);
            Vector2 posTopWall = new Vector2(i, height - 1);

            GetTileAtPosition(new Vector2(i, 0)).SetAsObstacle();
            GetTileAtPosition(new Vector2(i, height - 1)).SetAsObstacle();

            outsideWallsPos.Add(posBottomWall);
            outsideWallsPos.Add(posTopWall);
        }

        //Random obstacle Walls
        for (int i = 0; i < numberObstacles; i++)
        { 
            GetTileAtPosition(GenerateRandomPos()).SetAsObstacle();
        }
    }

    public Tile GetTileAtPosition(Vector2 pos)
    {
        if (tiles.TryGetValue(pos, out var tile)) return tile;
        return null;
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

    public Vector2 GenerateRandomPos()
    {
        var Map = new HashSet<Vector2>();

        foreach  (Tile t in tiles.Values)
        {
            Map.Add(t.transform.position);
        }

        foreach  (Vector2 v in outsideWallsPos)
        {
            Map.Remove(v);
        }

        Map.Remove(PlayerSpawn.position);

        var range = Enumerable.Range(0, Map.Count - 1);

        var rand = new System.Random();
        int index = rand.Next(0, Map.Count - 1);
        
        return Map.ElementAt(range.ElementAt(index));
    }
}