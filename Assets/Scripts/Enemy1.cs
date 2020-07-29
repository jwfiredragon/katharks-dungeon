using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : EntityBase
{
    void Start()
    {
        updatePosition();
        type = EntityType.Enemy;
        health = 10;
        attack = 1;
        speed = 1;
    }

    void Update()
    {
        showHealth();
    }

    public void move()
    {
        
    }
}
