using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
