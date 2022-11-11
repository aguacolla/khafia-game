using UnityEngine;

public class EventsTest : MonoBehaviour
{
    public void WordFound()
    {
        print("Word found");
    }
    
    public void WordNotFound()
    {
        print("Word not found");
    }
    
    public void WordIncorrect()
    {
        print("Word incorrect");
    }
}