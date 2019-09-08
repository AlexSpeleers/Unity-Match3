using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundTile : MonoBehaviour
{
    public int hitPoints;
    private SpriteRenderer sprite;
    delegate void MyDelegate();
    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        if (hitPoints <= 0) { Destroy(this.gameObject); }
    }
    public void TakeDamage(int damage)
    {
        hitPoints -= damage;
        MyDelegate md = new MyDelegate(MakeLighter);
        md.Invoke();
        //MakeLighter();
    }

    void MakeLighter()
    {
        Color color = sprite.color;
        //get the color's alpha and decrease it in half
        float newAlpha = color.a * 0.5f;
        sprite.color = new Color(color.r, color.g, color.b, newAlpha);
    }
}
