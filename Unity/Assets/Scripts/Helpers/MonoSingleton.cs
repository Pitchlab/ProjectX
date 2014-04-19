using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
	private static T m_Instance = null;
	public static T instance
	{
		get
		{
			// Instance requiered for the first time, we look for it
			if( m_Instance == null )
			{
				m_Instance = GameObject.FindObjectOfType(typeof(T)) as T;
				
				// Object not found, we create a temporary one
				if( m_Instance == null )
				{
					Object prefab = Resources.Load(typeof(T).ToString());
					if (prefab != null) {
						GameObject tempInstance = (GameObject)Instantiate(Resources.Load(typeof(T).ToString()), Vector3.zero, Quaternion.identity);
						if (tempInstance != null) {
							m_Instance = tempInstance.GetComponent<T>();
							tempInstance.name = typeof(T).ToString();
						}
					}
					
					//               if (m_Instance == null) {
					
					//                Debug.Log("Singleton - Creating new " + typeof(T).ToString());
					//                m_Instance = new GameObject("Temp Instance of " + typeof(T).ToString(), typeof(T)).GetComponent<T>();
					// }
					
					// Problem during the creation, this should not happen
					if( m_Instance == null )
					{
						Debug.LogWarning("Singleton - Problems during the creation of " + typeof(T).ToString());
					}
				}
				if (m_Instance != null) m_Instance.Init();
			}
			return m_Instance;
		}
	}
	// If no other monobehaviour request the instance in an awake function
	// executing before this one, no need to search the object.
	private void Awake()
	{
		if ( m_Instance != null && m_Instance != this)
		{
			Debug.LogWarning(m_Instance + " already exists, destroying new " + this);
			DestroyImmediate(this.gameObject);
		}
		if( m_Instance == null )
		{
			m_Instance = this as T;
			m_Instance.Init();
		}
	}
	
	// This function is called when the instance is used the first time
	// Put all the initializations you need here, as you would do in Awake
	protected virtual void Init(){}
	
	// Make sure the instance isn't referenced anymore when the user quit, just in case.
	private void OnApplicationQuit()
	{
		m_Instance = null;
		DestroyImmediate(this.gameObject);
	}
	
	public static void Destroy()
	{
		if (m_Instance != null) {
			DestroyImmediate(m_Instance.gameObject);
			m_Instance = null;
		}
	}
	
}