using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Planet : MonoBehaviour
{
    // Start is called before the first frame update
    public double mass;
    float speed = 0;
    public bool good;
    private bool selected;
    public void setSpeed(float speed)
    {
        this.speed = speed;
    }

    void Start()
    {
        //selected = false;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(Vector3.up, speed * Time.deltaTime, Space.World);
    }
    public void delete()
    {
        Destroy(gameObject);
    }
    public void SetColourToRed()
    {
        Renderer renderer = gameObject.GetComponent<Renderer>();
        Material[] material = renderer.materials;
        material[0].color = Color.red;
        material[1].color = Color.blue;
    }

    public void setSelected(bool selected)
    {
        this.selected = selected;
    }

    public bool getSelected()
    {
        return selected;
    }
}
