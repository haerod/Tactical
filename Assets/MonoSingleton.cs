using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : Component
{
	public static T instance { get; private set; }

	protected void Awake()
    {
		if (instance == null)
        {
			Init();
			instance = GetComponent<T>();
		}
        else
        {
			Debug.LogError("An instance of " + instance.name + " already exists ! " + gameObject.name + " will be deleted !", gameObject);
		}
	}

    protected virtual void OnDestroy()
    {
        if (instance == this) instance = null;
    }

	protected virtual void Init() { } // Called on Awake, as Awake can't be overloaded
}