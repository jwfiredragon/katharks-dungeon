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

public class GridEntity
{
    public enum EntityType { None, Player, Enemy };

    public EntityType type;
    public int scratch;

    public GridEntity(EntityType inType)
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
    BoardParams boardParams;
    TileHelper tileHelper;
    GridTile[,] tilesGrid;
    EntityHelper entityHelper;
    GridEntity[,] entitiesGrid;
    int enemyCount;

    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] holeTiles;
    public GameObject[] enemyTiles;
    public GameObject playerTile;

    public BoardManager()
    {
        boardParams = new BoardParams(8, 12, 20, 20);
        tileHelper = new TileHelper(boardParams);
        tilesGrid = new GridTile[boardParams.cols, boardParams.rows];
        entityHelper = new EntityHelper(boardParams);
        entitiesGrid = new GridEntity[boardParams.cols, boardParams.rows];
        enemyCount = 5;
    }

    public void SetupBoard()
    {
        tileHelper.generateTiles(tilesGrid);
        instantiateTiles();
        instantiateBorder();

        entityHelper.generateEntities(entitiesGrid);
        instantiateEnemies();
        entitiesGrid[0, 0].type = GridEntity.EntityType.Player;
        Instantiate(playerTile, new Vector2(0, 0), Quaternion.identity);

        GameObject.Find("Main Camera").transform.position = new Vector3((float)(boardParams.cols - 1) / 2, (float)(boardParams.rows - 1) / 2, -10);
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

        while (enemiesLeft > 0)
        {
            int x = UnityEngine.Random.Range(0, boardParams.cols);
            int y = UnityEngine.Random.Range(0, boardParams.rows);

            if (tilesGrid[x, y].type == GridTile.TileType.Floor)
            {
                entitiesGrid[x, y].type = GridEntity.EntityType.Enemy;
                GameObject RandomEnemy = enemyTiles[UnityEngine.Random.Range(0, enemyTiles.Length)];
                Instantiate(RandomEnemy, new Vector2(x, y), Quaternion.identity);
                enemiesLeft--;
            }
        }
    }
}

