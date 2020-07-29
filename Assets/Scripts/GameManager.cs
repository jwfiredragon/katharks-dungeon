using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public BoardManager boardScript;

    BoardParams boardParams;
    GridTile[,] tilesGrid;
    GridEntity[,] entitiesGrid;

    void Awake()
    {
        // Ensure only one GameManager instance exists
        if (instance == null)
        {
            instance = this;
        }
        else if(instance!=this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        boardScript = GetComponent<BoardManager>();
        initGame();
    }

    public void initGame()
    {
        boardScript.clearBoard();
        boardParams = new BoardParams(8, 12, 20, 20, 5);
        tilesGrid = new GridTile[boardParams.cols, boardParams.rows];
        entitiesGrid = new GridEntity[boardParams.cols, boardParams.rows];
        boardScript.setupBoard(boardParams, tilesGrid, entitiesGrid);
    }

    public void moveEntities()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemy in enemies)
        {
            enemy.GetComponent<Enemy1>().move();
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<Player>().move();
    }

    public void quit()
    {
        Application.Quit();
    }
}
