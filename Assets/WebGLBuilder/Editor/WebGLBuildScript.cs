using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.IO.Compression;
namespace WebGLTool
{
    public class WebGLBuildScript
    {

        private const string BUILD_FOR_CI_Settings = "BUILD_FOR_CI";
        
        private const string BUILD_PATH = "BUILD_PATH";
        
        private const string IS_ZIP = "IS_ZIP";

        private static WebGLBuildData Data
        {
            get {
                if (data == null) 
                {
                    data = Resources.Load<WebGLBuildData>("WebGLBuildData");
                    if (data == null)
                    {
                        data = ScriptableObject.CreateInstance<WebGLBuildData>();
                        AssetDatabase.CreateAsset(data, "Assets/WebGLBuilder/Resources/WebGLBuildData.asset");
                    }
                }
                return data;
            }
        }
        private static WebGLBuildData data = null;

        // 构建项目
        [MenuItem("WebGLBuilder/Build WebGL")]
        public static void BuildProject()
        {

            WebGLBuildData data = Data;

            string buildForCI = Environment.GetEnvironmentVariable(BUILD_FOR_CI_Settings);
            if (!string.IsNullOrEmpty(buildForCI) && buildForCI.Equals("1"))
            {
                data.BuildForCI = true;
            }
            string buildPath = Environment.GetEnvironmentVariable(BUILD_PATH);
            if (!string.IsNullOrEmpty(buildPath))
            {
                data.BuildPath = buildPath;
            }
            string isZip = Environment.GetEnvironmentVariable(IS_ZIP);
            if (!string.IsNullOrEmpty(isZip) && isZip.Equals("1"))
            {
                data.IsZip = true;
            }

            data.BuildProject();
        }
        
        [MenuItem("WebGLBuilder/Switch To WebGL Settings")]
        public static void SetWebGlSettings()
        {
            Data.ApplyWebGLSettings();
        }

        [MenuItem("WebGLBuilder/Create WebGLBuildData")]
        public static void CreateWebGLBuildData()
        {
            WebGLBuildData webGLBuildData = ScriptableObject.CreateInstance<WebGLBuildData>();
            AssetDatabase.CreateAsset(webGLBuildData, "Assets/WebGLBuildData.asset");
        }
    }
}
