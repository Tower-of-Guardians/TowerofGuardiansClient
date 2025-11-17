using UnityEngine;

public class ObjectPoolInjector : MonoBehaviour, IInjector
{
    [SerializeField] private ObjectPoolManager objectPoolManager;

    public void Inject()
    {
        if (objectPoolManager == null)
        {
            objectPoolManager = GetComponent<ObjectPoolManager>();
        }

        if (objectPoolManager == null)
        {
            return;
        }

        if (!DIContainer.IsRegistered<ObjectPoolManager>())
        {
            DIContainer.Register<ObjectPoolManager>(objectPoolManager);
        }
    }
}

