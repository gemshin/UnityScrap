using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace ZKit
{
    public class NonPubSingleton<T> where T : class
    {
        private static T _instance;
        private static object _lock = new object();

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        ConstructorInfo constructor = null;

                        try
                        {
                            constructor = typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, new Type[0], null);
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Get Constructor Failed", e);
                        }

                        if (constructor == null || constructor.IsAssembly)
                            throw new Exception(string.Format("constructor is missing {0}", typeof(T).Name));
                        _instance = (T)constructor.Invoke(null);
                    }
                }
                return _instance;
            }
        }
    }
}