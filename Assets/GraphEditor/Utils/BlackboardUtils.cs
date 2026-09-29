using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GraphEditor
{
    public static class BlackboardUtils
    {
        public static Dictionary<string, Type> NameToAddType { get; } = new()
        {
            { "Int", typeof(IntField)}
        };

        public static Dictionary<Type, string> TypeToDisplayed { get; } = new()
        {
            { typeof(IntField), "Int" },
        };

        public static Dictionary<Type, Type> FieldToAddProperty { get; } = new()
        {
            { typeof(IntField), typeof(IntProperty) }
        };

        public static string PackageRelativePath
        {
            get
            {
                if (string.IsNullOrEmpty(m_PackagePath))
                {
                    m_PackagePath = GetPackageRelativePath();
                }
                return m_PackagePath;
            }
        }

        [SerializeField]
        private static string m_PackagePath;

        private static string folderPath = "Not Found";

        private static string GetPackageRelativePath()
        {
            string packagePath = Path.GetFullPath("Assets/..");
            if (Directory.Exists(packagePath))
            {
                if (Directory.Exists(packagePath + "/Assets/GraphEditor"))
                {
                    return "Assets/GraphEditor";
                }
                string[] matchingPaths = Directory.GetDirectories(packagePath, "GraphEditor", SearchOption.AllDirectories);
                string path = ValidateLocation(matchingPaths, packagePath);
                if (path != null) return packagePath + path;
            }
            return null;
        }

        private static string ValidateLocation(string[] paths, string projectPath)
        {
            for (int i = 0; i < paths.Length; i++)
            {
                if (Directory.Exists(paths[i]))
                {
                    folderPath = paths[i].Replace(projectPath, "");
                    folderPath = folderPath.TrimStart('\\', '/');
                    return folderPath;
                }
            }

            return null;
        }
    }
}
