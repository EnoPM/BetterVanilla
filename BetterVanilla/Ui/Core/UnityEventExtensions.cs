using System;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BetterVanilla.Ui.Core;

/// <summary>
/// Extension methods for Unity events to handle IL2CPP delegate conversions.
/// </summary>
public static class UnityEventExtensions
{
    extension(UnityEvent unityEvent)
    {
        public void AddListener(Action handler)
        {
            unityEvent.AddListener(new Action(handler));
        }

        public void RemoveListener(Action handler)
        {
            unityEvent.RemoveListener(new Action(handler));
        }
    }

    extension(UnityEvent<bool> unityEvent)
    {
        public void AddListener(Action<bool> handler)
        {
            unityEvent.AddListener(new Action<bool>(handler));
        }

        public void RemoveListener(Action<bool> handler)
        {
            unityEvent.RemoveListener(new Action<bool>(handler));
        }
    }

    extension(UnityEvent<int> unityEvent)
    {
        public void AddListener(Action<int> handler)
        {
            unityEvent.AddListener(new Action<int>(handler));
        }
        
        public void RemoveListener(Action<int> handler)
        {
            unityEvent.RemoveListener(new Action<int>(handler));
        }
    }

    extension(UnityEvent<float> unityEvent)
    {
        public void AddListener(Action<float> handler)
        {
            unityEvent.AddListener(new Action<float>(handler));
        }
        
        public void RemoveListener(Action<float> handler)
        {
            unityEvent.RemoveListener(new Action<float>(handler));
        }
    }

    extension(UnityEvent<string> unityEvent)
    {
        public void AddListener(Action<string> handler)
        {
            unityEvent.AddListener(new Action<string>(handler));
        }
        
        public void RemoveListener(Action<string> handler)
        {
            unityEvent.RemoveListener(new Action<string>(handler));
        }
    }
}