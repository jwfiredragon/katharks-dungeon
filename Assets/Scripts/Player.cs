using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : EntityBase
{
    void Start()
    {
        updatePosition();
        type = EntityType.Player;
        health = 10;
        attack = 1;
        speed = 1;
    }

    void Update()
    {
        showHealth();
        highlightTiles();
    }

    public void move()
    {
        
    }

    void highlightTiles()
    {

    }
}
