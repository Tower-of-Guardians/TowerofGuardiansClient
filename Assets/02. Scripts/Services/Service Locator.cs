using System.Collections.Generic;
using System;

public static class ServiceLocator
{
    private static Dictionary<Type, object> m_services = new();

    public static IReadOnlyDictionary<Type, object> Services => m_services;

    public static void Initialize()
    {
        // TODO: 서비스가 등록될 예정입니다.
    }

    public static void Register<T>(T service)
    {
        if(!m_services.ContainsKey(typeof(T)))
        {
            m_services.Add(typeof(T), service);
        }
    }

    public static T Get<T>()
    {
        if(!m_services.TryGetValue(typeof(T), out var service))
        {
            throw new Exception($"Service Locator에서 관리 중인 {typeof(T)}가 없습니다.");
        }

        return (T)service;
    }

    public static void Replace<T>(T service)
    {
        if(m_services.ContainsKey(typeof(T)))
        {
            m_services[typeof(T)] = service;
        }
    }
}