using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using singleton;

public class EventManager : Singleton<EventManager>
{
    private IDictionary<string, List<Action<GameObject>>> _eventDatabase;
    IDictionary<string, List<Action<GameObject>>> EventDatabase =>
        _eventDatabase ?? (_eventDatabase = new Dictionary<string, List<Action<GameObject>>>());

    public static void On(string eventName, Action<GameObject> subscriber)
    {
        if (!Instance.EventDatabase.ContainsKey(eventName))
            Instance.EventDatabase.Add(eventName, new List<Action<GameObject>>());
        Instance.EventDatabase[eventName].Add(subscriber);
    }
    public static void Remove(string eventName, Action<GameObject> subscriber)
    {
        if (!Instance.EventDatabase.ContainsKey(eventName))
            return;
        Instance.EventDatabase[eventName].Remove(subscriber);
    }

    public static void Emit(string eventName, GameObject parameter = null)
    {
        if (Instance.EventDatabase.ContainsKey(eventName))
            foreach (var action in Instance.EventDatabase[eventName])
            {
                action?.Invoke(parameter);
            }
    }
}
