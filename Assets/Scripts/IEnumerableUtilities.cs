using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IEnumerableUtilities
{
    public static void ForEach<T>(this IEnumerable<T> collection, System.Action<T> actionOnElement)
    {
        foreach(T el in collection)
        {
            actionOnElement(el);
        }
    }
}
