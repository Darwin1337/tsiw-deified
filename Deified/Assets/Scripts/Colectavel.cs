using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Colectavel : MonoBehaviour
{
    [SerializeField] private VisualEffect visualEffect;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(45, 30, 15) * (Time.deltaTime * 4f));
        visualEffect.SetVector3("Velocity", new Vector3(UnityEngine.Random.Range(0, 10), UnityEngine.Random.Range(0, 10), UnityEngine.Random.Range(0, 10)));
    }
}
