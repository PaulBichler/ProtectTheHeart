using DG.Tweening;
using UnityEngine;

public class FloorScript : MonoBehaviour
{
    public GameObject ToSpawn;
    public float space;
    public int amtX;
    public int amtY;
    
    private GameObject[] objects;
    
    private float timer;
    private float cooldown;

    private int amt = 20;
    
    
    // Start is called before the first frame update
    void Start()
    {
        objects = new GameObject[amt * amt];
        int k = 0;
        int l = 0;
        for (float i = -15; i < 15; i += 1.5f, k++)
        {
            for (float j = -15; j < 15; j += 1.5f, l++)
            {
                objects[k*amt+l] = Instantiate(ToSpawn, new Vector3(i, transform.position.y, j), Quaternion.identity, transform);
            }

            l = 0;
        }
        
        cooldown = 3;
        timer = cooldown;
    }

    private void MakeEmMove()
    {
        for (int i = 0; i < amt; i++)
        {
            for (int j = 0; j < amt; j++)
            {
                float target = Mathf.PerlinNoise((i + 0.01f + count) / amt + Time.timeSinceLevelLoad, (j + 0.01f + count) / amt + Time.timeSinceLevelLoad);
                float lerp = Mathf.Lerp(-3, 3, target);
                objects[i * amt + j].transform.DOLocalMoveY(lerp, cooldown);
                Debug.Log("Noise for : " + i + ", " + j + " returned : " + target + " lerp is : " + lerp);
            }
        }
    }

    private int count = 0;

    void Update()
    {
        timer += Time.deltaTime;
        
        if (timer >= cooldown)
        {
            MakeEmMove();
            timer = 0;
        }
    }
}
