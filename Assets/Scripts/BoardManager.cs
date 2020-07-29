using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridTile
{
    public enum TileType { Floor, Wall, Hole };

    public TileType type;
    public int scratch;

    public GridTile(TileType inType)
    {
        type = inType;
        scratch = 0;
    }
}

public class BoardParams
{
    public int rows;
    public int cols;
    public int wallPercent;
    public int holePercent;

    public BoardParams(int inRows, int inCols, int inWallPercent, int inHolePercent)
    {
        rows = inRows;
        cols = inCols;
        wallPercent = inWallPercent;
        holePercent = inHolePercent;
    }
}

public class BoardManager : MonoBehaviour
{
    int enemyCount;
    BoardParams boardParams;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] holeTiles;
    public GameObject[] enemyTiles;
    public GameObject playerTile;
    private GridTile[,] tilesGrid;

    public BoardManager()
    {
        boardParams = new BoardParams(8, 12, 20, 20);
        enemyCount = 5;
        tilesGrid = new GridTile[boardParams.cols, boardParams.rows];
    }

    public void SetupBoard()
    {
        generateTiles();
        instantiateTiles();
        instantiateBorder();
        instantiateEnemies();
        Instantiate(playerTile, new Vector2(0, 0), Quaternion.identity);
        GameObject.Find("Main Camera").transform.position = new Vector3((float)(boardParams.cols - 1) / 2, (float)(boardParams.rows - 1) / 2, -10);
    }

    // Randomly populates the board with floor, wall, and hole tiles.
    // Also ensures that all floor tiles are connected to each other by turning walls/holes into floors as necessary.
    void generateTiles()
    {
        for (int x = 0; x < boardParams.cols; x++)
        {
            for (int y = 0; y < boardParams.rows; y++)
            {
                int randVal = UnityEngine.Random.Range(0, 100);

                if (randVal < boardParams.wallPercent)
                {
                    tilesGrid[x, y] = new GridTile(GridTile.TileType.Wall);
                }
                else if (randVal - boardParams.wallPercent < boardParams.holePercent)
                {
                    tilesGrid[x, y] = new GridTile(GridTile.TileType.Hole);
                }
                else
                {
                    tilesGrid[x, y] = new GridTile(GridTile.TileType.Floor);
                }
            }
        }

        // Ensure that bottom left tile (player spawn) is a floor
        tilesGrid[0, 0].type = GridTile.TileType.Floor;

        while (!checkTiles())
        {
            unmarkTiles();
            traverseTiles(0, 0);
            clearPaths();
        }
    }

    // Instantiates the generated tiles.
    void instantiateTiles()
    {
        GameObject RandomTile;

        for (int x = 0; x < boardParams.cols; x++)
        {
            for (int y = 0; y < boardParams.rows; y++)
            {
                if (tilesGrid[x, y].type == GridTile.TileType.Wall)
                {
                    RandomTile = wallTiles[UnityEngine.Random.Range(0, wallTiles.Length)];
                }
                else if (tilesGrid[x, y].type == GridTile.TileType.Hole)
                {
                    RandomTile = holeTiles[UnityEngine.Random.Range(0, holeTiles.Length)];
                }
                else
                {
                    RandomTile = floorTiles[UnityEngine.Random.Range(0, floorTiles.Length)];
                }

                Instantiate(RandomTile, new Vector2(x, y), Quaternion.identity);
            }
        }
    }

    // Generate a border of walls around the game field.
    void instantiateBorder()
    {
        int i = 0;
        GameObject RandomTile;

        for (i = -1; i < boardParams.rows + 2; i++)
        {
            RandomTile = wallTiles[UnityEngine.Random.Range(0, wallTiles.Length)];
            Instantiate(RandomTile, new Vector2(-1, i), Quaternion.identity);

            RandomTile = wallTiles[UnityEngine.Random.Range(0, wallTiles.Length)];
            Instantiate(RandomTile, new Vector2(boardParams.cols, i), Quaternion.identity);
        }

        for (i = 0; i < boardParams.cols; i++)
        {
            RandomTile = wallTiles[UnityEngine.Random.Range(0, wallTiles.Length)];
            Instantiate(RandomTile, new Vector2(i, -1), Quaternion.identity);

            RandomTile = wallTiles[UnityEngine.Random.Range(0, wallTiles.Length)];
            Instantiate(RandomTile, new Vector2(i, boardParams.rows), Quaternion.identity);
        }
    }

    // Randomly instantiates enemies on the game field.
    void instantiateEnemies()
    {
        int enemiesLeft = enemyCount;

        while (enemiesLeft >= 0)
        {
            int x = UnityEngine.Random.Range(0, boardParams.cols);
            int y = UnityEngine.Random.Range(0, boardParams.rows);

            if (tilesGrid[x, y].type == GridTile.TileType.Floor)
            {
                GameObject RandomEnemy = enemyTiles[UnityEngine.Random.Range(0, enemyTiles.Length)];
                Instantiate(RandomEnemy, new Vector2(x, y), Quaternion.identity);
                enemiesLeft--;
            }
        }
    }

    // Attempts to travel entire grid recursively, marking any visited tiles, and stopping traversal upon reaching a wall/hole or the map border.
    void traverseTiles(int x, int y)
    {
        // Return if tile has already been visited
        if (tilesGrid[x, y].scratch == 1)
        {
            return;
        }

        // Mark tile as visited
        tilesGrid[x, y].scratch = 1;

        // Return if tile is a wall or floor
        if (tilesGrid[x, y].type == GridTile.TileType.Wall || tilesGrid[x, y].type == GridTile.TileType.Hole)
        {
            return;
        }

        // Recurse in each direction if there are still tiles in that direction
        if (x + 1 <= boardParams.cols - 1)
        {
            traverseTiles(x + 1, y);
        }
        if (y + 1 <= boardParams.rows - 1)
        {
            traverseTiles(x, y + 1);
        }
        if (x - 1 >= 0)
        {
            traverseTiles(x - 1, y);
        }
        if (y - 1 >= 0)
        {
            traverseTiles(x, y - 1);
        }

        return;
    }

    // Delete any visited wall/hole tiles that border unvisited floor tiles.
    void clearPaths()
    {
        for (int x = 0; x < boardParams.cols; x++)
        {
            for (int y = 0; y < boardParams.rows; y++)
            {
                if (tilesGrid[x, y].scratch == 1 && (tilesGrid[x, y].type == GridTile.TileType.Wall || tilesGrid[x, y].type == GridTile.TileType.Hole))
                {
                    if (x + 1 <= boardParams.cols - 1 && tilesGrid[x + 1, y].type == GridTile.TileType.Floor && tilesGrid[x + 1, y].scratch == 0)
                    {
                        tilesGrid[x, y].type = GridTile.TileType.Floor;
                        break;
                    }
                    else if (y + 1 <= boardParams.rows - 1 && tilesGrid[x, y + 1].type == GridTile.TileType.Floor && tilesGrid[x, y + 1].scratch == 0)
                    {
                        tilesGrid[x, y].type = GridTile.TileType.Floor;
                        break;
                    }
                    else if (x - 1 >= 0 && tilesGrid[x - 1, y].type == GridTile.TileType.Floor && tilesGrid[x - 1, y].scratch == 0)
                    {
                        tilesGrid[x, y].type = GridTile.TileType.Floor;
                        break;
                    }
                    else if (y - 1 >= 0 && tilesGrid[x, y - 1].type == GridTile.TileType.Floor && tilesGrid[x, y - 1].scratch == 0)
                    {
                        tilesGrid[x, y].type = GridTile.TileType.Floor;
                        break;
                    }
                }
            }
        }
    }

    // Returns true if all floor tiles on the map have been visited and false otherwise.
    bool checkTiles()
    {
        for (int x = 0; x < boardParams.cols; x++)
        {
            for (int y = 0; y < boardParams.rows; y++)
            {
                if (tilesGrid[x, y].type == GridTile.TileType.Floor)
                {
                    if (tilesGrid[x, y].scratch == 0)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    // Unmarks all tiles on the map.
    void unmarkTiles()
    {
        for (int x = 0; x < boardParams.cols; x++)
        {
            for (int y = 0; y < boardParams.rows; y++)
            {
                tilesGrid[x, y].scratch = 0;
            }
        }
    }
}

