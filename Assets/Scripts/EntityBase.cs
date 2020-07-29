using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class EntityBase : MonoBehaviour
{
    public enum EntityType { Player, Enemy };

    public EntityType type;
    public Vector2 position;
    public int health;
    public TextMesh healthText;
    public int attack;
    public int speed;

    public void updatePosition()
    {
        position = gameObject.transform.position;
    }

    public void showHealth()
    {
        healthText.text = health.ToString();
    }
}
