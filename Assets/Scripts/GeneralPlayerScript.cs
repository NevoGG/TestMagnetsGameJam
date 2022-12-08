using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralPlayerScript : MonoBehaviour
{

    private float boardHight = -0.1f;
    private GameObject bite = null;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Started");
        //StartCoroutine(RespawnBite());   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("General - hit trigger");
        if (other.gameObject.CompareTag("Bite"))
        {
            //Destroy(other);
            StartCoroutine(RespawnBite());
        }
    }

    IEnumerator RespawnBite()
    {
        Debug.Log("Started RespawnBite()");
        yield return new WaitForSeconds(2);
        float xPlacement = Random.Range(1, 450);
        float yPlacement = Random.Range(1, 250);
        if (Random.value < 0.5f)
        {
            xPlacement *= -1;
        }
        if (Random.value < 0.5f)
        {
            yPlacement *= -1;
        }
        bite = Instantiate(Resources.Load("Bites")) as GameObject;
        bite.transform.position = (new Vector3(xPlacement, yPlacement, boardHight));
        yield return null;
    }   


}
