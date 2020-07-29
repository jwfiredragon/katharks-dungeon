using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class TileHelper
{
    BoardParams boardParams;

    public TileHelper(BoardParams inBoardParams)
    {
        boardParams = inBoardParams;
    }

    // Randomly populates the board with floor, wall, and hole tiles.
    // Also ensures that all floor tiles are connected to each other by turning walls/holes into floors as necessary.
    public void generateTiles(GridTile[,] tilesGrid)
    {
        // Randomly generate tile type for all tiles
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

        // Repeatedly traverse and check tiles until all floor tiles are visitable from the spawn
        bool pathCleared = true;
        while (!checkTiles(tilesGrid))
        {
            if (!pathCleared)
            {
                forceClearPath(tilesGrid);
            }
            unmarkTiles(tilesGrid);
            traverseTiles(tilesGrid, 0, 0);
            pathCleared = clearPaths(tilesGrid);
        }
    }

    // Attempts to travel entire grid recursively, marking any visited tiles, and stopping traversal upon reaching a wall/hole or the map border.
    void traverseTiles(GridTile[,] tilesGrid, int x, int y)
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
            traverseTiles(tilesGrid, x + 1, y);
        }
        if (y + 1 <= boardParams.rows - 1)
        {
            traverseTiles(tilesGrid, x, y + 1);
        }
        if (x - 1 >= 0)
        {
            traverseTiles(tilesGrid, x - 1, y);
        }
        if (y - 1 >= 0)
        {
            traverseTiles(tilesGrid, x, y - 1);
        }

        return;
    }

    // Delete any visited wall/hole tiles that border unvisited floor tiles.
    // Returns true if at least one tile was deleted and false otherwise.
    bool clearPaths(GridTile[,] tilesGrid)
    {
        bool tileCleared = false;

        for (int x = 0; x < boardParams.cols; x++)
        {
            for (int y = 0; y < boardParams.rows; y++)
            {
                if (tilesGrid[x, y].scratch == 1 && (tilesGrid[x, y].type == GridTile.TileType.Wall || tilesGrid[x, y].type == GridTile.TileType.Hole))
                {
                    if (x + 1 <= boardParams.cols - 1 && tilesGrid[x + 1, y].type == GridTile.TileType.Floor && tilesGrid[x + 1, y].scratch == 0)
                    {
                        tilesGrid[x, y].type = GridTile.TileType.Floor;
                        tileCleared = true;
                        break;
                    }
                    else if (y + 1 <= boardParams.rows - 1 && tilesGrid[x, y + 1].type == GridTile.TileType.Floor && tilesGrid[x, y + 1].scratch == 0)
                    {
                        tilesGrid[x, y].type = GridTile.TileType.Floor;
                        tileCleared = true;
                        break;
                    }
                    else if (x - 1 >= 0 && tilesGrid[x - 1, y].type == GridTile.TileType.Floor && tilesGrid[x - 1, y].scratch == 0)
                    {
                        tilesGrid[x, y].type = GridTile.TileType.Floor;
                        tileCleared = true;
                        break;
                    }
                    else if (y - 1 >= 0 && tilesGrid[x, y - 1].type == GridTile.TileType.Floor && tilesGrid[x, y - 1].scratch == 0)
                    {
                        tilesGrid[x, y].type = GridTile.TileType.Floor;
                        tileCleared = true;
                        break;
                    }
                }
            }
        }

        return tileCleared;
    }

    // Deletes the first wall/hole bordering an univisited tile found.
    void forceClearPath(GridTile[,] tilesGrid)
    {
        for (int x = 0; x < boardParams.cols; x++)
        {
            for (int y = 0; y < boardParams.rows; y++)
            {
                if (tilesGrid[x, y].scratch == 1 && (tilesGrid[x, y].type == GridTile.TileType.Wall || tilesGrid[x, y].type == GridTile.TileType.Hole))
                {
                    if (x + 1 <= boardParams.cols - 1 && tilesGrid[x + 1, y].scratch == 0)
                    {
                        tilesGrid[x, y].type = GridTile.TileType.Floor;
                        return;
                    }
                    else if (y + 1 <= boardParams.rows - 1 && tilesGrid[x, y + 1].scratch == 0)
                    {
                        tilesGrid[x, y].type = GridTile.TileType.Floor;
                        return;
                    }
                    else if (x - 1 >= 0 && tilesGrid[x - 1, y].scratch == 0)
                    {
                        tilesGrid[x, y].type = GridTile.TileType.Floor;
                        return;
                    }
                    else if (y - 1 >= 0 && tilesGrid[x, y - 1].scratch == 0)
                    {
                        tilesGrid[x, y].type = GridTile.TileType.Floor;
                        return;
                    }
                }
            }
        }

        return;
    }

    // Returns true if all floor tiles on the map have been visited and false otherwise.
    bool checkTiles(GridTile[,] tilesGrid)
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
    void unmarkTiles(GridTile[,] tilesGrid)
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
