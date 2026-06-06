using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    public GameObject Target;
    public GameObject VelTarget;
    public GameObject DireTarget;
    public GameObject SizeTarget;
    public GameObject AddBTarget;

    int[,] Target_whole;
    public int NumberOfTargetsOnX = 10;
    public int NumberOfTargetsOnY = 3;
    float x_range = 17.8f;
    public float y_range = 3.5f;
    Vector3 TargetScale = Vector3.zero;
    public float x_betweenSpace = 0.1f;
    public float y_betweenSpace = 0.1f;
    
    public int NumberOfVelTargets = 0;
    public int NumberOfDireTargets = 0;
    public int NumberOfSizeTargets = 0;
    public int NumberOfAddBTargets = 0;
    int NumberOfNotTargets;

    float x, y;

    // Start is called before the first frame update
    void Start()
    {
        Target_whole = new int[NumberOfTargetsOnX, NumberOfTargetsOnY];
        NumberOfNotTargets = NumberOfVelTargets + NumberOfDireTargets + NumberOfSizeTargets + NumberOfAddBTargets;
        GameObject[] TargetDetermination = {Target, VelTarget, DireTarget, SizeTarget, AddBTarget};
        
        fill_zero();
        
        if (NumberOfNotTargets != 0)
        {
            Insert_NotTarget();
        }

        for (int i = 0; i < NumberOfTargetsOnX; i++)
        {
            for(int j = 0; j < NumberOfTargetsOnY; j++)
            {
                Debug.Log(Target_whole[i, j]);
            }
        }

        TargetScale.x = x_range/ NumberOfTargetsOnX - x_betweenSpace;
        TargetScale.y = y_range / NumberOfTargetsOnY - y_betweenSpace;

        Target.transform.localScale = TargetScale;
        VelTarget.transform.localScale = TargetScale;
        DireTarget.transform.localScale = TargetScale;
        SizeTarget.transform.localScale = TargetScale;
        AddBTarget.transform.localScale = TargetScale;
        
        y = transform.position.y - (y_betweenSpace + TargetScale.y / 2);
        
        for (int i = 0; i < NumberOfTargetsOnY; i++)
        {
            x = transform.position.x + x_betweenSpace + TargetScale.x / 2;

            for (int j = 0; j < NumberOfTargetsOnX; j++)
            {
                Instantiate(TargetDetermination[Target_whole[j, i]], new Vector3(x, y, 0), Quaternion.identity);
                x += (TargetScale.x + x_betweenSpace);
            }

            y -= (TargetScale.y + y_betweenSpace);
        }
    }

    void fill_zero ()
    {
        for(int i = 0; i < NumberOfTargetsOnX; i++)
        {
            for (int j = 0; j < NumberOfTargetsOnY; j++)
            {
                Target_whole[i, j] = 0;
            }
        }
    }

    void Insert_NotTarget ()
    {
        int k = 0, o, randam;
        float interval;
        int[] NotTargets_distribution = {NumberOfVelTargets, NumberOfDireTargets, NumberOfSizeTargets, NumberOfAddBTargets};
        int[] gun_NotTargets;
        
        interval = (float)(NumberOfTargetsOnX * NumberOfTargetsOnY) / NumberOfNotTargets;
        gun_NotTargets = new int[NumberOfNotTargets];

        for (int i = 0; i < 4; i++)
        {
            while( NotTargets_distribution[i] != 0 )
            {
                gun_NotTargets[k] = i + 1;
                NotTargets_distribution[i]--;
                k++;
            }
        }

        for (int i = 0; i < NumberOfNotTargets; i++)
        {
            o = gun_NotTargets[i];
            randam = Random.Range(0, NumberOfNotTargets);
            gun_NotTargets[i] = gun_NotTargets[randam];
            gun_NotTargets[randam] = o;
        }

        for (int i = 0; i < NumberOfNotTargets; i++)
        {
            randam = Random.Range((int)(i * interval), (int)((i + 1) * interval)); // Random.Range(以上, 未満);
            Target_whole[randam % NumberOfTargetsOnX, randam / NumberOfTargetsOnX] = gun_NotTargets[i];
        }

    }

}
