using UnityEngine;
using System.Threading;

public class ThreadSafeSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // 静态实例
    private static T _instance;

    // 用于锁定的对象，确保线程安全
    private static readonly object _lock = new object();

    // 获取实例的方法
    public static T Instance
    {
        get
        {
            // 双重检查锁定（Double-Check Locking）
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        // 如果实例为 null，则在这里初始化
                        _instance = FindObjectOfType<T>();

                        if (_instance == null)
                        {
                            GameObject singletonObject = new GameObject(typeof(T).Name);
                            _instance = singletonObject.AddComponent<T>();
                            DontDestroyOnLoad(singletonObject);  // 防止切换场景时销毁
                        }
                    }
                }
            }
            return _instance;
        }
    }

    // 确保单例类不会在场景中被销毁
    protected virtual void Awake()
    {
        // 确保只存在一个实例
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);  // 销毁重复的实例
        }
        else
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);  // 防止切换场景时销毁
        }
    }

    // 在销毁时清理实例
    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}