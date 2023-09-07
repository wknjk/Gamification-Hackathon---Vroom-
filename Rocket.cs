using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
//using UnityEditor.U2D.Animation;
//using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class Rocket : MonoBehaviour
{

    private Vector3 touchPosition;
    private Rigidbody rb;
    public static int touches = 0;
    private Vector3 direction;
    [SerializeField] private float speed;
    private float moveSpeed;
    private SpriteRenderer sr;
    private bool launched = false;
    private bool collided;
    public bool game_end;
    public int level = 1;
    public double dimension;
    List<Planet> planets;
    [SerializeField] private double gamma = 1;
    private CapsuleCollider capsuleCollider;

    private Points points;

    public void setRocketPoints(Points points)
    {
        this.points = points;
    }

    void Start()
    {
        game_end = false;   
        rb = GetComponent<Rigidbody>();
        sr = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        speed = 2.5f;
    }

    void Update()
    {
        if (GameManager.planet_selected == 2)
        {
            if (Input.touchCount > touches && !launched)
            {
                Touch touch = Input.GetTouch(touches);
                if (touch.phase == TouchPhase.Began)
                {
                    touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
                    touchPosition.z = 0f;
                }

                if (touch.phase == TouchPhase.Ended)
                {
                    Vector3 releasePosition = Camera.main.ScreenToWorldPoint(touch.position);
                    releasePosition.z = 0f;

                    direction = (touchPosition - releasePosition).normalized;
                    moveSpeed = speed * (touchPosition - releasePosition).magnitude;
                    rb.velocity = new Vector2(direction.x * moveSpeed, direction.y * moveSpeed);

                    launched = true;

                    transform.rotation = Quaternion.Euler(0, 0, (float)(Math.Atan2((double)direction.y, (double)direction.x) * 180 / Math.PI) - 90);
                    touches++;
                }

                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    Vector3 currPosition = Camera.main.ScreenToWorldPoint(touch.position);
                    currPosition.z = 0f;

                    Vector3 direction_tmp = (touchPosition - currPosition).normalized;
                    transform.rotation = Quaternion.Euler(0, 0, (float)(Math.Atan2((double)direction_tmp.y, (double)direction_tmp.x) * 180 / Math.PI) - 90);
                    float moveSpeed_tmp = speed * (touchPosition - currPosition).magnitude;

                    points.drawPoints(direction_tmp, moveSpeed_tmp);

                }

            }
            if (ofScreen())
            {
                game_end = true;
                resetPosition();
            }
            if (launched)
            {
                calculateVelocity();
            }
        }
    }

    public void resetPosition()
    {
        rb.velocity = new Vector2(0, 0);
        transform.position = new Vector3(0, (float)-4.3, 0);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        rb.angularVelocity = Vector3.zero;
        if (touches > 0) { touches--; }
        launched = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        int i;
        for(i = 0; i < planets.Count; i++) {
            if (planets[i].name == collision.gameObject.name)
            {
                break;
            }
        }
        if (i == planets.Count) { return; }  
        if (planets[i].good && planets[i].getSelected())
        {
            level++;
            resetPosition();
        }
        else
        {
            game_end = true;
            resetPosition();
        }
    }

    private Tuple<double, double> calculateAcceleration(int i)
    {
       
        Vector3 dir = planets[i].transform.position - transform.position;
        return Tuple.Create((gamma * planets[i].mass / Math.Pow(dir.magnitude, 2)) * dir.normalized.x, (gamma * planets[i].mass / Math.Pow(dir.magnitude, 2)) * dir.normalized.y);
    }

    private void calculateVelocity()
    {
        double a_x = 0, a_y = 0;
        for (int i = 0; i < planets.Count; i++)
        {
            a_x += calculateAcceleration(i).Item1;
            a_y += calculateAcceleration(i).Item2;
        }
        rb.velocity = new Vector2((float)(rb.velocity.x + a_x * Time.deltaTime ), (float)(rb.velocity.y + a_y * Time.deltaTime));
        Vector3 direct = (new Vector3((float)rb.velocity.x, (float)rb.velocity.y, 0f)).normalized;
        transform.rotation = Quaternion.Euler(0, 0, (float)(Math.Atan2((double)direct.y, (double)direct.x) * 180 / Math.PI) - 90);
    }

    bool ofScreen()
    {
        dimension = Math.Max(sr.sprite.rect.width / sr.sprite.pixelsPerUnit * transform.localScale.x, sr.sprite.rect.height / sr.sprite.pixelsPerUnit * transform.localScale.y);
        float width = Screen.width;
        float height = Screen.height;

        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(width, height, 0));

        float localWidth = topRight.x - bottomLeft.x;
        float localHeight = topRight.y - bottomLeft.y;
        if (Math.Abs(transform.position.x) > (localWidth / 2.0 + dimension / 2.0) || Math.Abs(transform.position.y) > (localHeight / 2.0 + dimension / 2.0))
            return true;
        return false;
    }

    public void SetPlanets(List<Planet> planets)
    {
        this.planets = planets;
    }
}
