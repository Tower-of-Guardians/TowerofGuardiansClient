using UnityEngine;

public class LoadingTest : MonoBehaviour
{
    void Start()
    {
        LoadingManager.Instance.LoadScene("Game");
    }
}
