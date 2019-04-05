using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaGen : MonoBehaviour
{

    public GameObject []objects;
    public float gap =6.0f;

    public int maxX =5;
    public int maxY =5;


    void Start()
    {
        for( int i=0; i<=maxX; i++){
            for( int j=0; j<=maxY; j++){
                int rand =Random.Range(0, objects.Length);
                Instantiate(objects[rand],transform.position+ new Vector3(i*gap,0,j*gap), 
                    Quaternion.Euler( 0, Random.Range(0, 360), 0) );
            }
        }
    }
}
