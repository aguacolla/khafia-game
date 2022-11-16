using UnityEngine;

public class Config<T> : ScriptableObject where T : Config<T>
{
    private static string _nameOfType;

    static string nameOfType
    {
        get
        {
            if (_nameOfType == null)
            {
                _nameOfType = typeof(T).Name;
            }

            return _nameOfType;
        }
    }
    public static T instance
    {
        get
        {

            _loaded = _loaded ? _loaded : _loaded = Resources.Load<T>("Config/" + nameOfType);
            if (!_loaded)
            {
                Debug.LogError("Config not exist in resources: " + typeof(T).ToString());
                _loaded = CreateInstance<T>();
            }
            return _loaded;
        }
    }
    private static T _loaded;
}
