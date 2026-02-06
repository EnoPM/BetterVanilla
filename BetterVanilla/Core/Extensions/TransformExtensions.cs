using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BetterVanilla.Core.Extensions;

public static class TransformExtensions
{
    extension(Transform transform)
    {
        public void SaveChildrenPathsTo(string filePath)
        {
            var data = transform.GetAllChildrenPaths();
            File.WriteAllText(filePath, string.Join('\n', data));
        }

        public string[] GetAllChildrenPaths()
        {
            var results = new List<string>();
            CollectChildrenPaths(transform, "", results);
            return results.ToArray();
        }

        public T? FindByPath<T>(string path) where T : Component
        {
            var foundTransform = transform.FindByPath(path);
            return foundTransform?.GetComponent<T>();
        }

        public Transform? FindByPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            var parts = path.Split('/');
            var current = transform;

            foreach (var part in parts)
            {
                current = current.Find(part);
                if (current == null)
                {
                    return null;
                }
            }

            return current;
        }
    }

    private static void CollectChildrenPaths(Transform parent, string currentPath, List<string> results)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            var child = parent.GetChild(i);
            var childPath = string.IsNullOrEmpty(currentPath) ? child.name : currentPath + "/" + child.name;
            results.Add(childPath);

            if (child.childCount > 0)
            {
                CollectChildrenPaths(child, childPath, results);
            }
        }
    }
}