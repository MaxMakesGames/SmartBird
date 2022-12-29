using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Game : MonoBehaviour
{
    public GameObject polePrefab, birdPrefab;
    public float distancePoles = 10f;
    public int birdsToSpawn = 1;

    List<GameObject> birds = new List<GameObject>();
    List<GameObject> poles = new List<GameObject>();
    List<Transform> poleTips1 = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        //spawn start poles
        for(float x = 0; x < 20; x += distancePoles)
        {
            poles.Add(Instantiate(polePrefab, new Vector3(x, Random.Range(-6f, -1f), 0f), Quaternion.identity));
            poles[poles.Count - 1].GetComponent<Rigidbody2D>().velocity = new Vector2(-5f, 0f);
            poleTips1.Add(poles[poles.Count - 1].transform.GetChild(1));
        }

        //spawn ai birds
        AI.AICount = 0;
        for (int i = 0; i < birdsToSpawn; i++)
        {
            GameObject bird = Instantiate(birdPrefab, new Vector3(-8f, 0f, 0f), Quaternion.identity);
            birds.Add(bird);
        }
    }

    public Vector3 GetClosestTipDist(Bird bird)
    {
        double closest = 100000;
        Vector3 closestPos = new Vector3(0, 0, 0);
        foreach (Transform tip in poleTips1)
        {
            if (tip.position.x < bird.transform.position.x)
                continue;
            double dist = Vector3.Distance(bird.transform.position, tip.position);
            if (dist < closest)
            {
                closest = dist;
                closestPos = tip.position;
            }
        }
        closestPos -= bird.transform.position; //dist bird-pole not just pole pos
        return closestPos;
    }

    public double[] GetInputs(Bird bird)
    {
        Vector3 closestPos = GetClosestTipDist(bird);
        return new double[] { closestPos.x, closestPos.y };
    }

    public void Lost(Bird bird)
    {
        if (!birds.Contains(bird.gameObject))
            return;
        birds.Remove(bird.gameObject);
        if (birds.Count <= 0) //last bird died, save its data because it was the last alive
        {
            NetworkSaver.SaveToFile(bird.ai.network, "Network.txt");
            SceneManager.LoadScene(0);
            return;
        }
        
        Destroy(bird.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < poles.Count; i++)
        {
            //when a pole reaches x -11f, it is off-camera so we find the previous one in the list and teleport it to the right of it with a new random height
            if(poles[i].transform.position.x < -11f)
            {
                int prev = i - 1;
                if (prev < 0)
                    prev = poles.Count - 1;
                poles[i].transform.position = poles[prev].transform.position + new Vector3(distancePoles, Random.Range(-6f, -1f) - poles[prev].transform.position.y, 0f);
            }
        }
        
    }
}
