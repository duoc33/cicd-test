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

            // ��ȡ�������õĳ���
            string[] scenes = GetScenesToBuild();

            // ʹ�û���������·���������������Ϊ�գ���ʹ��Ĭ�ϵ����·��
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
                // ������ɺ����ѹ��
                string zipFilePath = finalBuildPath + ".zip";
                ZipBuildDirectory(finalBuildPath, zipFilePath);
                Directory.Delete(finalBuildPath, true);
            }

            Debug.Log("Build completed successfully.");
        }

        // Ӧ��WebGL���ò��л�ƽ̨
        public void ApplyWebGLSettings()
        {
            if (WebGLSettings == null || WebGLSettings.text == null)
            {
                Debug.LogWarning("WebGLSettings  null error");
            }
            else {

                string webGLSettings = WebGLSettings.text;
                // 2. ��ȡ ProjectSettings.asset ·��
                string projectSettingsPath = "ProjectSettings/ProjectSettings.asset";

                // 3. �޸� ProjectSettings.asset �ļ�����
                if (File.Exists(projectSettingsPath))
                {
                    File.WriteAllText(projectSettingsPath, null);

                    // ���޸ĺ������д���ļ�
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

            // 4. �л�ƽ̨Ϊ WebGL
            SwitchToWebGLPlatform();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        // �л� Unity ��Ŀ�� WebGL ƽ̨
        private static void SwitchToWebGLPlatform()
        {
            // ȷ����ǰƽ̨�� WebGL
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

        // ���ļ���ѹ��Ϊ.zip�ļ�
        private static void ZipBuildDirectory(string sourceDirectory, string zipFilePath)
        {
            // ȷ��Ŀ���ļ�·�����ļ��д���
            string directory = Path.GetDirectoryName(zipFilePath);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            //ʹ�� ZipFile �ཫ�ļ���ѹ��Ϊ zip �ļ�
            ZipFile.CreateFromDirectory(sourceDirectory, zipFilePath, System.IO.Compression.CompressionLevel.Fastest, false);

            Debug.Log("Zip file created at: " + zipFilePath);
        }

        private static string[] GetScenesToBuild()
        {
            // �����������õĳ���·��
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

