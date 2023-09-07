using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Points : MonoBehaviour
{

    List<Planet> planets;
    private double gamma = 1;
    public Circle circlePrefab;
    private List<Circle> dots = new List<Circle>();
    public static int num_of_dots;
    private Vector2 velocity;
    private Vector3 position;

    public float fixed_dist = 0.7f;


    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetPlanets(List<Planet> planets)
    {
        this.planets = planets;
    }



    public void drawPoints(Vector3 direction, float speed)
    {

        var b = Instantiate(circlePrefab);
        b.transform.position = new Vector3(0f, -4.3f, 0f);
        // dots.Add(b);

        position = b.transform.position;
        velocity = new Vector2(direction.x * speed, direction.y * speed);
        float time = fixed_dist / velocity.magnitude;


        for (int dotsIterator = 0; dotsIterator < num_of_dots; dotsIterator++)
        {
            time = fixed_dist / velocity.magnitude;

            double a_x = 0, a_y = 0;

            for (int i = 0; i < planets.Count; i++)
            {
                a_x += calculateAcceleration(i).Item1;
                a_y += calculateAcceleration(i).Item2;
            }

            velocity = new Vector2((float)(velocity.x + a_x * time), (float)(velocity.y + a_y * time));
            if (velocity.x == float.PositiveInfinity || velocity.y == float.PositiveInfinity || position.x == float.PositiveInfinity || position.y == float.PositiveInfinity) { break; }
            position = new Vector3((float)(position.x + time * velocity.x), (float)(position.y + time * velocity.y), 0f);
            //UnityEngine.Debug.Log(position+"position");
            //UnityEngine.Debug.Log(velocity );

            var tmp = Instantiate(circlePrefab);
            tmp.transform.position = position;
            //   dots.Add(tmp);
        }

        // deleteDots();

    }

    private void deleteDots()
    {
        for (int i = 0; i < dots.Count; i++)
        {
            //dots[i].delete();
        }

        dots.Clear();
    }


    private Tuple<double, double> calculateAcceleration(int i)
    {

        Vector3 dir = planets[i].transform.position - position;
        return Tuple.Create((gamma * planets[i].mass / Math.Pow(dir.magnitude, 2)) * dir.normalized.x, (gamma * planets[i].mass / Math.Pow(dir.magnitude, 2)) * dir.normalized.y);
    }



}
