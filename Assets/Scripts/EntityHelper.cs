using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class EntityHelper
{
    BoardParams boardParams;

    public EntityHelper(BoardParams inBoardParams)
    {
        boardParams = inBoardParams;
    }

    // Fills grid with None entities
    public void generateEntities(GridEntity[,] entitiesGrid)
    {
        for (int x = 0; x < boardParams.cols; x++)
        {
            for (int y = 0; y < boardParams.rows; y++)
            {
                entitiesGrid[x, y] = new GridEntity(GridEntity.EntityType.None);
            }
        }
    }
}
