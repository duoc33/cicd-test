using System.IO;
using System.IO.Compression;
using UnityEditor;
using UnityEngine;
namespace WebGLTool
{
    public class WebGLBuildData : ScriptableObject
    {
        public string CompanyName;

        public string ProductName;

        public bool BuildForCI = false;
        
        public bool IsZip = false;
        
        public string BuildPath = "";
        
        public TextAsset WebGLSettings;

        private void Awake()
        {
            WebGLSettings ??= Resources.Load<TextAsset>("WebGLSettings");

            if (WebGLSettings == null)
            {
                Debug.LogError("WebGLSettings or WebGLSettingsForCI Missed");
            }
        }

        public void BuildProject()
        {
            ApplyWebGLSettings();

            // 获取所有启用的场景
            string[] scenes = GetScenesToBuild();

            // 使用环境变量的路径，如果环境变量为空，则使用默认的相对路径
            string finalBuildPath = string.IsNullOrEmpty(BuildPath) ? Path.Combine(Directory.GetCurrentDirectory(), "defaultBuildOutput") : BuildPath;

            if (!Directory.Exists(finalBuildPath))
            {
                Directory.CreateDirectory(finalBuildPath);
            }

            BuildPipeline.BuildPlayer(new BuildPlayerOptions()
            {
                locationPathName = finalBuildPath,
                scenes = scenes,
                target = BuildTarget.WebGL,
                options = BuildOptions.None
            });

            if (IsZip)
            {
                // 构建完成后进行压缩
                string zipFilePath = finalBuildPath + ".zip";
                ZipBuildDirectory(finalBuildPath, zipFilePath);
                Directory.Delete(finalBuildPath, true);
            }

            Debug.Log("Build completed successfully.");
        }

        // 应用WebGL设置并切换平台
        public void ApplyWebGLSettings()
        {
            if (WebGLSettings == null || WebGLSettings.text == null)
            {
                Debug.LogWarning("WebGLSettings  null error");
            }
            else {

                string webGLSettings = WebGLSettings.text;
                // 2. 获取 ProjectSettings.asset 路径
                string projectSettingsPath = "ProjectSettings/ProjectSettings.asset";

                // 3. 修改 ProjectSettings.asset 文件内容
                if (File.Exists(projectSettingsPath))
                {
                    File.WriteAllText(projectSettingsPath, null);

                    // 将修改后的内容写回文件
                    File.WriteAllText(projectSettingsPath, webGLSettings);

                    AssetDatabase.Refresh();

                    Debug.Log("ProjectSettings.asset has been updated!");
                }
                else
                {
                    Debug.LogError("ProjectSettings.asset not found!");
                    return;
                }
            }

            PlayerSettings.companyName = CompanyName == null ? PlayerSettings.companyName : CompanyName;

            PlayerSettings.productName = ProductName == null ? PlayerSettings.productName : ProductName;

            PlayerSettings.WebGL.compressionFormat = BuildForCI ? WebGLCompressionFormat.Disabled : WebGLCompressionFormat.Gzip;
            PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.ExplicitlyThrownExceptionsOnly;
            PlayerSettings.WebGL.nameFilesAsHashes = false;
            PlayerSettings.WebGL.dataCaching = false;
            PlayerSettings.WebGL.debugSymbolMode = WebGLDebugSymbolMode.Off;
            PlayerSettings.WebGL.decompressionFallback = true;
            PlayerSettings.WebGL.powerPreference = WebGLPowerPreference.HighPerformance;

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

        // 将文件夹压缩为.zip文件
        private static void ZipBuildDirectory(string sourceDirectory, string zipFilePath)
        {
            // 确保目标文件路径的文件夹存在
            string directory = Path.GetDirectoryName(zipFilePath);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            //使用 ZipFile 类将文件夹压缩为 zip 文件
            ZipFile.CreateFromDirectory(sourceDirectory, zipFilePath, System.IO.Compression.CompressionLevel.Fastest, false);

            Debug.Log("Zip file created at: " + zipFilePath);
        }

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
    }
}

