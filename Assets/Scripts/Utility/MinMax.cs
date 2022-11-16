[System.Serializable]
public struct MinMax
{
    public int min;
    public int max;

    public MinMax(int min, int max)
    {
        this.min = min;
        this.max = max;
    }

    public int random => UnityEngine.Random.Range(min, max + 1);
}