using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BepInEx;
using UnityEngine;
using UnityEngine.Networking;

namespace CaptainAudio
{
    public static class AudioLoader
    {
        private static bool _isInitialized = false;
        private const int MAX_RETRY_COUNT = 3;
        private const float RETRY_DELAY = 0.5f;

        // 외부 오디오 파일이 존재하는 폴더 목록 (내장 음악 오버라이드 대상)
        private static HashSet<string> _externalOverrideFolders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public static IEnumerator InitializeAsync()
        {
            if (_isInitialized) yield break;

            Plugin.CustomMusic.Clear();
            Plugin.CustomAmbient.Clear();
            Plugin.CustomSFX.Clear();
            Plugin.CustomMusicList.Clear();
            Plugin.CustomAmbientList.Clear();
            Plugin.CustomSFXList.Clear();

            yield return LoadEmbeddedResourcesParallel("CaptainAudio.asset.Resources.Music", Plugin.CustomMusic, Plugin.CustomMusicList);
            yield return LoadEmbeddedResourcesParallel("CaptainAudio.asset.Resources.Ambient", Plugin.CustomAmbient, Plugin.CustomAmbientList);
            yield return LoadEmbeddedResourcesParallel("CaptainAudio.asset.Resources.SFX", Plugin.CustomSFX, Plugin.CustomSFXList);

            LoadExternalAudioFiles();
            StringMatchCache.Initialize();

            _isInitialized = true;
            Plugin.Log.LogInfo($"<color=#00BFFF>Loaded: {Plugin.CustomMusicList.Count} folders, {Plugin.CustomMusic.Count} clips</color>");
        }

        private static IEnumerator LoadEmbeddedResourcesParallel(string resourcePrefix, Dictionary<string, AudioClip> singleDict, Dictionary<string, Dictionary<string, AudioClip>> folderDict)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string[] resourceNames = assembly.GetManifestResourceNames()
                .Where(name => name.StartsWith(resourcePrefix))
                .ToArray();

            if (resourceNames.Length == 0)
            {
                yield break;
            }

            List<LoadTask> tasks = new List<LoadTask>();

            // 병렬 로딩 작업 생성
            foreach (string resourceName in resourceNames)
            {
                Stream stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null) continue;

                byte[] audioData = new byte[stream.Length];
                stream.Read(audioData, 0, audioData.Length);
                stream.Dispose();

                string fileName = resourceName.Substring(resourcePrefix.Length + 1);
                var (folderName, clipName) = ParseResourcePath(fileName);

                if (string.IsNullOrEmpty(clipName)) continue;

                LoadTask task = new LoadTask
                {
                    FolderName = folderName,
                    ClipName = clipName,
                    AudioData = audioData,
                    IsCompleted = false,
                    LoadedClip = null
                };

                tasks.Add(task);
                Plugin.instance.StartCoroutine(LoadSingleClipWithRetry(task));
            }

            // 모든 작업 완료 대기
            while (tasks.Any(t => !t.IsCompleted))
            {
                yield return null;
            }

            // 결과 저장
            foreach (var task in tasks)
            {
                if (task.LoadedClip == null) continue;

                if (!string.IsNullOrEmpty(task.FolderName))
                {
                    if (!folderDict.ContainsKey(task.FolderName))
                    {
                        folderDict[task.FolderName] = new Dictionary<string, AudioClip>();
                    }
                    folderDict[task.FolderName][task.ClipName] = task.LoadedClip;
                }
                else
                {
                    singleDict[task.ClipName] = task.LoadedClip;
                }
            }
        }

        private static IEnumerator LoadSingleClipWithRetry(LoadTask task)
        {
            int retryCount = Plugin.LoadRetryCount.Value;

            for (int attempt = 0; attempt < retryCount; attempt++)
            {
                bool success = false;
                AudioClip clip = null;

                yield return LoadAudioClipFromBytes(task.AudioData, task.ClipName, (loadedClip) =>
                {
                    clip = loadedClip;
                    success = loadedClip != null;
                });

                if (success)
                {
                    task.LoadedClip = clip;
                    task.IsCompleted = true;
                    yield break;
                }

                if (attempt < retryCount - 1)
                {
                    yield return new WaitForSeconds(RETRY_DELAY);
                }
            }

            task.IsCompleted = true;
        }

        private static IEnumerator LoadAudioClipFromBytes(byte[] audioData, string clipName, Action<AudioClip> onComplete)
        {
            string fileExtension = DetectAudioFormat(audioData);
            string tempPath = Path.Combine(Application.temporaryCachePath, $"temp_audio_{clipName}_{Guid.NewGuid()}{fileExtension}");

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(tempPath));
                File.WriteAllBytes(tempPath, audioData);
            }
            catch
            {
                onComplete?.Invoke(null);
                yield break;
            }

            AudioType audioType = GetAudioType(fileExtension);
            string uri = "file:///" + tempPath.Replace("\\", "/");

            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(uri, audioType))
            {
                DownloadHandlerAudioClip handler = (DownloadHandlerAudioClip)www.downloadHandler;
                handler.streamAudio = false;
                handler.compressed = false;
                www.timeout = 15;

                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                    // 클립 유효성 검증 강화
                    if (clip != null && clip.length > 0 && clip.samples > 0 && clip.loadState == AudioDataLoadState.Loaded)
                    {
                        clip.name = clipName;
                        onComplete?.Invoke(clip);
                    }
                    else
                    {
                        Plugin.Log.LogWarning($"Invalid clip: {clipName} (length={clip?.length}, samples={clip?.samples}, state={clip?.loadState})");
                        onComplete?.Invoke(null);
                    }
                }
                else
                {
                    Plugin.Log.LogWarning($"Failed to load: {clipName} - {www.error}");
                    onComplete?.Invoke(null);
                }
            }

            // 임시 파일 정리
            if (File.Exists(tempPath))
            {
                try
                {
                    File.Delete(tempPath);
                }
                catch { }
            }
        }

        private static void LoadExternalAudioFiles()
        {
            string externalPath = Path.Combine(Paths.PluginPath, "CaptainAudio");
            if (!Directory.Exists(externalPath)) return;

            // 1단계: 외부 폴더 스캔 - 유효한 오디오 파일이 있는 폴더 감지
            _externalOverrideFolders.Clear();
            ScanExternalFolders(externalPath);

            // 2단계: 오버라이드 대상 폴더의 내장 음악 제거
            ApplyExternalOverrides();

            // 3단계: 외부 파일 로드
            if (Directory.Exists(Path.Combine(externalPath, "Music")))
            {
                CollectAudioFiles(Path.Combine(externalPath, "Music"), Plugin.CustomMusic, Plugin.CustomMusicList);
            }
            if (Directory.Exists(Path.Combine(externalPath, "SFX")))
            {
                CollectAudioFiles(Path.Combine(externalPath, "SFX"), Plugin.CustomSFX, Plugin.CustomSFXList);
            }
            if (Directory.Exists(Path.Combine(externalPath, "Ambient")))
            {
                CollectAudioFiles(Path.Combine(externalPath, "Ambient"), Plugin.CustomAmbient, Plugin.CustomAmbientList);
            }
        }

        /// <summary>
        /// 외부 폴더를 스캔하여 유효한 오디오 파일이 있는 폴더명을 수집
        /// </summary>
        private static void ScanExternalFolders(string externalPath)
        {
            string[] categoryFolders = { "Music", "Ambient", "SFX" };

            foreach (string category in categoryFolders)
            {
                string categoryPath = Path.Combine(externalPath, category);
                if (!Directory.Exists(categoryPath)) continue;

                foreach (string subFolder in Directory.GetDirectories(categoryPath))
                {
                    string folderName = Path.GetFileName(subFolder);

                    // 해당 폴더에 유효한 오디오 파일이 있는지 확인
                    bool hasValidAudio = Directory.GetFiles(subFolder)
                        .Any(file => IsValidAudioFile(file));

                    if (hasValidAudio)
                    {
                        // 카테고리와 폴더명을 결합하여 고유 키 생성
                        string overrideKey = $"{category}:{folderName}";
                        _externalOverrideFolders.Add(overrideKey);
                        Plugin.Log.LogInfo($"<color=#FFA500>[Override] {category}/{folderName}: 외부 음악 감지됨 → 내장 음악 대체</color>");
                    }
                }
            }
        }

        /// <summary>
        /// 파일이 유효한 오디오 파일인지 확인 (ogg, wav, mp3)
        /// </summary>
        private static bool IsValidAudioFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return false;

            string extension = Path.GetExtension(filePath).ToLower();
            return extension == ".ogg" || extension == ".wav" || extension == ".mp3";
        }

        /// <summary>
        /// 외부 파일이 존재하는 폴더의 내장 음악을 제거 (오버라이드 적용)
        /// </summary>
        private static void ApplyExternalOverrides()
        {
            foreach (string overrideKey in _externalOverrideFolders)
            {
                string[] parts = overrideKey.Split(':');
                if (parts.Length != 2) continue;

                string category = parts[0];
                string folderName = parts[1];

                Dictionary<string, Dictionary<string, AudioClip>> targetDict = null;

                switch (category)
                {
                    case "Music":
                        targetDict = Plugin.CustomMusicList;
                        break;
                    case "Ambient":
                        targetDict = Plugin.CustomAmbientList;
                        break;
                    case "SFX":
                        targetDict = Plugin.CustomSFXList;
                        break;
                }

                if (targetDict == null) continue;

                // 대소문자 무시 매칭으로 폴더 찾기
                string matchedKey = targetDict.Keys
                    .FirstOrDefault(k => string.Equals(k, folderName, StringComparison.OrdinalIgnoreCase));

                if (matchedKey != null)
                {
                    int removedCount = targetDict[matchedKey].Count;
                    targetDict[matchedKey].Clear();
                    Plugin.Log.LogInfo($"<color=#FFA500>[Override] {category}/{folderName}: 내장 음악 {removedCount}개 제거됨</color>");
                }
            }
        }

        private static void CollectAudioFiles(string path, Dictionary<string, AudioClip> singleDict, Dictionary<string, Dictionary<string, AudioClip>> folderDict)
        {
            // 루트 파일 수집
            foreach (string file in Directory.GetFiles(path))
            {
                LoadExternalClip(file, singleDict);
            }

            // 폴더별 파일 수집
            foreach (string folder in Directory.GetDirectories(path))
            {
                string folderName = Path.GetFileName(folder);
                folderDict[folderName] = new Dictionary<string, AudioClip>();

                foreach (string file in Directory.GetFiles(folder))
                {
                    LoadExternalClip(file, folderDict[folderName]);
                }
            }
        }

        private static void LoadExternalClip(string path, Dictionary<string, AudioClip> dict)
        {
            if (path.EndsWith(".txt") || !path.Contains(".")) return;

            string extension = Path.GetExtension(path).ToLower();
            if (extension != ".ogg" && extension != ".wav" && extension != ".mp3") return;

            string uri = "file:///" + path.Replace("\\", "/");
            AudioType audioType = GetAudioType(extension);

            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(uri, audioType);
            DownloadHandlerAudioClip handler = (DownloadHandlerAudioClip)www.downloadHandler;
            handler.streamAudio = false;
            handler.compressed = false;

            www.SendWebRequest();

            // 타임아웃 추가 (최대 10초)
            float timeout = 10f;
            float elapsed = 0f;
            while (!www.isDone && elapsed < timeout)
            {
                System.Threading.Thread.Sleep(10);
                elapsed += 0.01f;
            }

            if (www.result == UnityWebRequest.Result.Success)
            {
                AudioClip clip = handler?.audioClip;

                // 클립 유효성 검증
                if (clip != null && clip.length > 0 && clip.samples > 0)
                {
                    string clipName = Path.GetFileNameWithoutExtension(path);
                    clip.name = clipName;
                    dict[clipName] = clip;
                }
                else
                {
                    Plugin.Log.LogWarning($"Invalid external clip: {path}");
                }
            }
            else
            {
                Plugin.Log.LogWarning($"Failed to load external: {path} - {www.error}");
            }

            www.Dispose();
        }

        private static (string folderName, string clipName) ParseResourcePath(string fileName)
        {
            string folderName = null;
            string clipName = null;

            if (fileName.Contains("\\"))
            {
                var parts = fileName.Split('\\');
                if (parts.Length >= 2)
                {
                    folderName = parts[0];
                    clipName = Path.GetFileNameWithoutExtension(parts[parts.Length - 1]);
                }
            }
            else
            {
                var parts = fileName.Split('.');
                if (parts.Length >= 3)
                {
                    folderName = parts[0];
                    clipName = string.Join(".", parts.Skip(1).Take(parts.Length - 2));
                }
                else if (parts.Length >= 2)
                {
                    folderName = null;
                    clipName = parts[0];
                }
            }

            return (folderName, clipName);
        }

        private static string DetectAudioFormat(byte[] audioData)
        {
            if (audioData.Length < 4) return ".ogg";

            // WAV 헤더 (RIFF)
            if (audioData[0] == 0x52 && audioData[1] == 0x49 && audioData[2] == 0x46 && audioData[3] == 0x46)
                return ".wav";

            // MP3 헤더 (ID3 또는 0xFF 0xFB)
            if ((audioData[0] == 0x49 && audioData[1] == 0x44 && audioData[2] == 0x33) ||
                (audioData[0] == 0xFF && (audioData[1] & 0xE0) == 0xE0))
                return ".mp3";

            return ".ogg";
        }

        private static AudioType GetAudioType(string extension)
        {
            switch (extension)
            {
                case ".wav": return AudioType.WAV;
                case ".mp3": return AudioType.MPEG;
                default: return AudioType.OGGVORBIS;
            }
        }

        private class LoadTask
        {
            public string FolderName;
            public string ClipName;
            public byte[] AudioData;
            public bool IsCompleted;
            public AudioClip LoadedClip;
        }
    }
}
