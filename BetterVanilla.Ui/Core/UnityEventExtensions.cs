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
    #region UnityEvent (no parameters)

    extension(UnityEvent unityEvent)
    {
        public void AddListener(Action handler)
        {
            unityEvent.AddListener((UnityAction)handler);
        }
        public void RemoveListener(Action handler)
        {
            unityEvent.RemoveListener((UnityAction)handler);
        }
    }

    #endregion

    #region UnityEvent<bool>

    extension(UnityEvent<bool> unityEvent)
    {
        public void AddListener(Action<bool> handler)
        {
            unityEvent.AddListener((UnityAction<bool>)handler);
        }
        public void RemoveListener(Action<bool> handler)
        {
            unityEvent.RemoveListener((UnityAction<bool>)handler);
        }
    }

    #endregion

    #region UnityEvent<int>

    extension(UnityEvent<int> unityEvent)
    {
        public void AddListener(Action<int> handler)
        {
            unityEvent.AddListener((UnityAction<int>)handler);
        }
        public void RemoveListener(Action<int> handler)
        {
            unityEvent.RemoveListener((UnityAction<int>)handler);
        }
    }

    #endregion

    #region UnityEvent<float>

    extension(UnityEvent<float> unityEvent)
    {
        public void AddListener(Action<float> handler)
        {
            unityEvent.AddListener((UnityAction<float>)handler);
        }
        public void RemoveListener(Action<float> handler)
        {
            unityEvent.RemoveListener((UnityAction<float>)handler);
        }
    }

    #endregion

    #region UnityEvent<string>

    extension(UnityEvent<string> unityEvent)
    {
        public void AddListener(Action<string> handler)
        {
            unityEvent.AddListener((UnityAction<string>)handler);
        }
        public void RemoveListener(Action<string> handler)
        {
            unityEvent.RemoveListener((UnityAction<string>)handler);
        }
    }

    #endregion
}
