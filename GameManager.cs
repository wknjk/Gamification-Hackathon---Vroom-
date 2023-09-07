using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    List<Planet> planets = new List<Planet>();
    public Planet planetPrefab;
    public Planet planetPrefab_selected;

    public Rocket rocket;
    public TextMeshProUGUI text;
    public Button yourButton;


    private double min_d;
    private double max_d;
    private double unit_of_total_area;
    private double total_area;
    private double max_area;
    private double localWidth;
    private double localHeight;
    private double percentage_of_screen;
    private double scale_unit_ratio;
    private double unit_mass;
    private double hardness_step = 1;
    List<Tuple<double, double>> coordinates;
    List<double> diameters;
    Touch touch;
    public static int planet_selected;
    private bool clicked_on_a_planet;

    public Points points;

    private int planned_planets;

    public Canvas canvas;

    public int level;

    //public Canvas startCanvas;
    //public Canvas gameoverCanvas;

    //public TextMeshProUGUI naslov;
    //public TextMeshProUGUI GameOverText;


    // Start is called before the first frame update
    void Start()
    {
        
        yourButton.onClick.AddListener(TaskOnClick);

        clicked_on_a_planet = false;
        planet_selected = 0;
        touch = new Touch();
        canvas.enabled = false;
        scale_unit_ratio = 1 / (2 * planetPrefab.GetComponent<SphereCollider>().radius);

        float width = Screen.width;
        float height = Screen.height;

        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(width, height, 0));

        localWidth = topRight.x - bottomLeft.x;
        localHeight = topRight.y - bottomLeft.y;

        coordinates = new List<Tuple<double, double>>();
        diameters = new List<double>();

        min_d = localWidth / 10;
        max_d = localWidth / 2;

        unit_mass = 7.5;
        percentage_of_screen = 0.67;
        level = 1;
        planned_planets = 3;
        unit_of_total_area = hardness_step * planned_planets * min_d *min_d * Math.PI / 4;

        total_area = (planned_planets * min_d * min_d * Math.PI / 4 + planned_planets * max_d * max_d * Math.PI / 4) / 2;

        setUpLevel();
        //naslov.text = "Nova igra";
        //GameOverText.text = "Game Over";

        //startCanvas.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
        }
        if (planet_selected == 0)
        {
            if(Input.touchCount > 0)
            {
                planet_selected = 1;
                Vector3 position = Camera.main.ScreenToWorldPoint(touch.position);
                position.z = 0f;
                clicked_on_a_planet = false;
                for (int i = 0; i< planets.Count; i++)
                {
                    if ((planets[i].transform.position - position).magnitude <= diameters[i])
                    {
                        if (!planets[i].good)
                            break;
                        Planet planet = Instantiate(planetPrefab_selected);
                        planet.transform.position = planets[i].transform.position;
                        planet.mass = planets[i].mass;
                        planet.good = planets[i].good;
                        planet.name = planets[i].name;
                        planet.setSpeed((i + 1) * 5f);
                        planet.transform.localScale = planets[i].transform.localScale;
                        
                        planets[i].delete();
                        planets[i] = planet;
                        planets[i].setSelected(true);
                        rocket.SetPlanets(planets);
                        rocket.setRocketPoints(points);
                        points.SetPlanets(planets);
                        clicked_on_a_planet = true;
                        break;
                    }
                }
                if(!clicked_on_a_planet)
                    planet_selected = 0;
            }
        }
        else if (planet_selected == 1 && touch.phase == TouchPhase.Ended)
        {
            planet_selected = 2;
        }
        
        if(rocket.level != level)
        {
            level = rocket.level;
            setUpLevel();
        }
        if(rocket.game_end)
        {
            endGame();
        }
    }

    void TaskOnClick()
    {
        rocket.level = 1;
        level = 1;
        rocket.game_end = false;

        clicked_on_a_planet = false;
        planet_selected = 0;
        touch = new Touch();
        canvas.enabled = false;
        scale_unit_ratio = 1 / (2 * planetPrefab.GetComponent<SphereCollider>().radius);

        float width = Screen.width;
        float height = Screen.height;

        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(width, height, 0));

        localWidth = topRight.x - bottomLeft.x;
        localHeight = topRight.y - bottomLeft.y;

        coordinates = new List<Tuple<double, double>>();
        diameters = new List<double>();

        min_d = localWidth / 10;
        max_d = localWidth / 2;

        unit_mass = 7.5;
        percentage_of_screen = 0.67;
        planned_planets = 3;
        unit_of_total_area = hardness_step * planned_planets * min_d * min_d * Math.PI / 4;

        total_area = (planned_planets * min_d * min_d * Math.PI / 4 + planned_planets * max_d * max_d * Math.PI / 4) / 2;

        setUpLevel();
    }

    void endGame()
    {
        for(int i = 0; i < planets.Count; i++)
        {
            if (Math.Abs(planets[i].gameObject.transform.position.y) < localHeight / 2)
                planets[i].delete();
        }
        planets.Clear();
        canvas.enabled = true;
        //canvas.transform.position = new Vector3(canvas.transform.position.x, canvas.transform.position.y, canvas.transform.position.z -10);
    }
    void setUpLevel()
    {
        if (level <= 7)
            Points.num_of_dots = 10 - level;
        else Points.num_of_dots = 0;
        hardness_step = hardness_step / 2.0;
        if (hardness_step < 1)
            hardness_step = 1;
        unit_of_total_area = hardness_step * planned_planets * min_d * min_d * Math.PI / 4;
        for (int i = 0; i < planets.Count; i++)
        {
            planets[i].delete();
        }
        planets.Clear();

        System.Random random = new System.Random();
        total_area += unit_of_total_area;
        max_area = 0.75 * planned_planets * max_d * max_d * Math.PI / 4;
        if (total_area > max_area)
        {
            planned_planets++;
        }
        
        double area = 0;
        bool error = true;
        int number_of_errors = 0;
        bool end = false;
        while(error)
        {
            number_of_errors++;
            if (number_of_errors > 1000000)
            {
                UnityEngine.Debug.Log("ne postoji konstalacija");
                end = true;
                break;
            }
            if (coordinates.Count > 0)
            {
                coordinates.Clear();
            }
            if (diameters.Count > 0)
            {
                diameters.Clear();
            }
            area = 0;
            for(int i = 0; i < planned_planets; i++)
            {
                coordinates.Add(new Tuple<double,double>(random.NextDouble()*localWidth-localWidth/2, random.NextDouble()*localHeight*percentage_of_screen + localHeight/2 - percentage_of_screen*localHeight));
                if (i < planned_planets - 1) {
                    double diameter = random.NextDouble() * (max_d - min_d) + min_d;
                    diameters.Add(diameter);
                    area += (diameter * diameter * Math.PI / 4.0);
                }
            }
            double d = Math.Sqrt((total_area - area)*(4.0/Math.PI));
            if(d < min_d || d > max_d || Double.IsNaN(d))
            {
                error = true;
                continue;
            }
            diameters.Add(d);
            error = isCollision();
        }
        if (end)
            return;
        double max1 = double.MinValue, max2 = double.MinValue, i1=0, i2=0;
        for(int i=0; i< planned_planets; i++)
        {
            if(max1 < coordinates[i].Item2)
            {
                i1 = i;
                max1 = coordinates[i].Item2;
            }
        }
        for(int i=0;i< planned_planets; i++)
        {
            if(i != i1 && max2 < coordinates[i].Item2)
            {
                i2 = i;
                max2 = coordinates[i].Item2;
            }
        }
        
        for (int i = 0; i < planned_planets; i++)
        {
            Planet new_planet = Instantiate(planetPrefab);
            if(i != i1 && i != i2)
            {
                new_planet.good = false;
                new_planet.SetColourToRed();
            }
            else
            {
                diameters[i] = 0.75 * diameters[i];
                new_planet.good = true;
            }
            
            new_planet.name = "planet_" + i;
            new_planet.setSpeed((i+1) * 5f);
            new_planet.transform.position = new Vector3((float)coordinates[i].Item1, (float)coordinates[i].Item2, 0);
            if (i < 0 || i >= diameters.Count) {UnityEngine.Debug.Log(i); break;}
            new_planet.transform.localScale = new Vector3((float)(diameters[i]*scale_unit_ratio), (float)(diameters[i] * scale_unit_ratio), (float)(diameters[i] * scale_unit_ratio));
            new_planet.mass = unit_mass * diameters[i] * diameters[i]*Math.PI/4;
            planets.Add(new_planet);

        }
        rocket.SetPlanets(planets);
        rocket.setRocketPoints(points);
        points.SetPlanets(planets);
        planet_selected = 0;

        text.text = "Level " + level;
    }

    bool isCollision()
    {
        
        for(int i = 0; i< planned_planets; i++)
        {
            for(int j=0;j<planned_planets;j++)
            {
                if (i != j)
                {
                    if (collide(i, j))
                        return true;
                }
            }
        }
        return false;
    }
    bool collide(int i , int j)
    {
        return (diameters[i] / 2 + diameters[j]/2 + rocket.dimension) >= Math.Sqrt(Math.Pow(coordinates[i].Item1 - coordinates[j].Item1,2)+Math.Pow(coordinates[i].Item2 - coordinates[j].Item2, 2));
    }

    public void StartGame()
    {
        //startCanvas.enabled = false;
        //gameoverCanvas.enabled = true;
    }
}
