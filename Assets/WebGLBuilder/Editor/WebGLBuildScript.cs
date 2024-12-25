using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.IO.Compression;
namespace WebGLTool
{
    public class WebGLBuildScript
    {
        // 构建项目
        [MenuItem("WebGLBuilder/Build WebGL")]
        public static void BuildProject()
        {
            try
            {
                // 获取所有启用的场景
                string[] scenes = GetScenesToBuild();

                // 构建相对于项目根目录的输出路径
                // 尝试从环境变量获取构建路径
                string customBuildPath = Environment.GetEnvironmentVariable("BUILD_PATH");

                // 使用环境变量的路径，如果环境变量为空，则使用默认的相对路径
                string finalBuildPath = string.IsNullOrEmpty(customBuildPath) ? Path.Combine(Directory.GetCurrentDirectory(), "defaultBuildOutput") : customBuildPath;

                if (!Directory.Exists(finalBuildPath))
                {
                    Directory.CreateDirectory(finalBuildPath);
                }

                // 构建目标平台
                BuildPipeline.BuildPlayer(scenes, finalBuildPath, BuildTarget.WebGL, BuildOptions.None);

                //// 构建完成后进行压缩
                //string zipFilePath = finalBuildPath + ".zip";
                //ZipBuildDirectory(finalBuildPath, zipFilePath);

                Debug.Log("Build completed successfully.");
            }
            catch (System.Exception ex)
            {
                // 捕获异常并输出到控制台
                Debug.LogError("Build failed: " + ex.Message);
                Debug.LogError("Stack Trace: " + ex.StackTrace);
            }
        }
        [MenuItem("WebGLBuilder/Switch To WebGL Settings")]
        public static void SetWebGl()
        {
            ApplyWebGLSettings();
        }
        private static string webGLSettingsPath = "Assets/WebGLBuilder/Editor/WebGLSettings.txt";
        // 应用WebGL设置并切换平台
        private static void ApplyWebGLSettings()
        {
            // 1. 读取 WebGLSettings.txt 文件
            if (!File.Exists(webGLSettingsPath))
            {
                Debug.LogError("WebGLSettings.txt not found at the specified path!");
                return;
            }

            string webGLSettings = File.ReadAllText(webGLSettingsPath);

            // 2. 获取 ProjectSettings.asset 路径
            string projectSettingsPath = "ProjectSettings/ProjectSettings.asset";

            // 3. 修改 ProjectSettings.asset 文件内容
            if (File.Exists(projectSettingsPath))
            {
                File.WriteAllText(projectSettingsPath, null);

                // 将修改后的内容写回文件
                File.WriteAllText(projectSettingsPath, webGLSettings);

                Debug.Log("ProjectSettings.asset has been updated!");
            }
            else
            {
                Debug.LogError("ProjectSettings.asset not found!");
                return;
            }

            // 4. 切换平台为 WebGL
            SwitchToWebGLPlatform();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

        }

        // 切换 Unity 项目到 WebGL 平台
        private static void SwitchToWebGLPlatform()
        {
            // 确保当前平台是 WebGL
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.WebGL)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
                Debug.Log("Switched to WebGL platform!");
            }
            else
            {
                Debug.Log("Already on WebGL platform.");
            }
        }

        // 获取当前项目中的所有场景
        private static string[] GetScenesToBuild()
        {
            // 返回所有启用的场景路径
            var scenes = new System.Collections.Generic.List<string>();
            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                {
                    scenes.Add(scene.path);
                }
            }
            return scenes.ToArray();
        }

        // 将文件夹压缩为.zip文件
        private static void ZipBuildDirectory(string sourceDirectory, string zipFilePath)
        {
            // 确保目标文件路径的文件夹存在
            string directory = Path.GetDirectoryName(zipFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // 使用 ZipFile 类将文件夹压缩为 zip 文件
            //ZipFile.CreateFromDirectory(sourceDirectory, zipFilePath, CompressionLevel.Fastest, false);

            Debug.Log("Zip file created at: " + zipFilePath);
        }
    }
}
