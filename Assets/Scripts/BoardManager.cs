using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardParams
{
    public int rows;
    public int cols;
    public int wallPercent;
    public int holePercent;
    public int enemyCount;

    public BoardParams(int inRows, int inCols, int inWallPercent, int inHolePercent, int inEnemyCount)
    {
        rows = inRows;
        cols = inCols;
        wallPercent = inWallPercent;
        holePercent = inHolePercent;
        enemyCount = inEnemyCount;
    }
}

public class BoardManager : MonoBehaviour
{
    BoardParams boardParams;
    TileHelper tileHelper;
    GridTile[,] tilesGrid;
    EntityHelper entityHelper;
    GridEntity[,] entitiesGrid;

    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] holeTiles;
    public GameObject[] enemyEntities;
    public GameObject playerEntity;

    public void setupBoard(BoardParams inBoardParams)
    {
        boardParams = inBoardParams;

        tileHelper = new TileHelper(boardParams);
        tilesGrid = new GridTile[boardParams.cols, boardParams.rows];
        entityHelper = new EntityHelper(boardParams);
        entitiesGrid = new GridEntity[boardParams.cols, boardParams.rows];

        tileHelper.generateTiles(tilesGrid);
        instantiateTiles();
        instantiateBorder();

        entityHelper.generateEntities(entitiesGrid);
        entitiesGrid[0, 0].type = GridEntity.EntityType.Player;
        GameObject player = Instantiate(playerEntity, new Vector2(0, 0), Quaternion.identity);
        instantiateEnemies();

        GameObject.Find("Main Camera").transform.position = new Vector3((float)(boardParams.cols - 1) / 2, (float)(boardParams.rows - 1) / 2, -10);
    }

    // Clears all tiles and entities on board.
    public void clearBoard()
    {
        GameObject[] gameObjArray = FindObjectsOfType<GameObject>();
        foreach(GameObject obj in gameObjArray)
        {
            // Layer 8 is entites, layer 9 is tiles
            if(obj.layer == 8 || obj.layer == 9)
            {
                Destroy(obj);
            }
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

        // Generate left and right walls
        for (i = -1; i < boardParams.rows + 2; i++)
        {
            RandomTile = wallTiles[UnityEngine.Random.Range(0, wallTiles.Length)];
            Instantiate(RandomTile, new Vector2(-1, i), Quaternion.identity);

            RandomTile = wallTiles[UnityEngine.Random.Range(0, wallTiles.Length)];
            Instantiate(RandomTile, new Vector2(boardParams.cols, i), Quaternion.identity);
        }

        // Generate top and bottom walls
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
        int enemiesLeft = boardParams.enemyCount;

        while (enemiesLeft > 0)
        {
            // Start generating at x/y = 2 to prevent enemies from spawning too close to player
            int x = UnityEngine.Random.Range(2, boardParams.cols);
            int y = UnityEngine.Random.Range(2, boardParams.rows);

            // Enemies can only spawn on floor tiles with no entities on them
            if (tilesGrid[x, y].type == GridTile.TileType.Floor && entitiesGrid[x, y].type == GridEntity.EntityType.None)
            {
                // Add enemy to entity grid
                entitiesGrid[x, y].type = GridEntity.EntityType.Enemy;
                // Instantiate random enemy
                GameObject RandomEnemy = enemyEntities[UnityEngine.Random.Range(0, enemyEntities.Length)];
                Instantiate(RandomEnemy, new Vector2(x, y), Quaternion.identity);

                enemiesLeft--;
            }
        }
    }
}

