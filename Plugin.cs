using System;
using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace CaptainAudio
{
    [BepInPlugin("captain.CaptainAudio", "Captain Audio", "1.2.2")]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin instance;

        // 설정
        public static ConfigEntry<bool> ModEnabled;
        public static ConfigEntry<float> MusicVolume;
        public static ConfigEntry<float> AmbientVolume;
        public static ConfigEntry<float> LocationVolMultiplier;
        public static ConfigEntry<int> LoadRetryCount;
        public static ConfigEntry<bool> EnableFallback;

        // 오디오 딕셔너리
        public static Dictionary<string, AudioClip> CustomMusic = new Dictionary<string, AudioClip>();
        public static Dictionary<string, Dictionary<string, AudioClip>> CustomMusicList = new Dictionary<string, Dictionary<string, AudioClip>>();
        public static Dictionary<string, AudioClip> CustomAmbient = new Dictionary<string, AudioClip>();
        public static Dictionary<string, Dictionary<string, AudioClip>> CustomAmbientList = new Dictionary<string, Dictionary<string, AudioClip>>();
        public static Dictionary<string, AudioClip> CustomSFX = new Dictionary<string, AudioClip>();
        public static Dictionary<string, Dictionary<string, AudioClip>> CustomSFXList = new Dictionary<string, Dictionary<string, AudioClip>>();

        // 로깅
        public static ManualLogSource Log { get; private set; }

        private void Awake()
        {
            instance = this;
            Log = Logger;

            // 설정 초기화
            ModEnabled = Config.Bind("General", "Enabled", true, "Enable this mod");
            MusicVolume = Config.Bind("General", "MusicVolume", 0.6f, "Music volume (0.0 - 1.0)");
            AmbientVolume = Config.Bind("General", "AmbientVolume", 0.3f, "Ambient volume (0.0 - 1.0)");
            LocationVolMultiplier = Config.Bind("General", "LocationVolumeMultiplier", 5f, "Location music volume multiplier");
            LoadRetryCount = Config.Bind("Advanced", "LoadRetryCount", 3, "Number of retries when audio loading fails");
            EnableFallback = Config.Bind("Advanced", "EnableFallback", true, "Use fallback to vanilla music on load failure");

            if (!ModEnabled.Value) return;

            try
            {
                Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
                StartCoroutine(AudioLoader.InitializeAsync());
                Log.LogInfo("<color=#00BFFF>Captain Audio v1.2.2 loaded</color>");
            }
            catch (Exception ex)
            {
                Log.LogError($"<color=#00BFFF>Critical error: {ex.Message}</color>");
            }
        }
    }

    // 유틸리티 클래스
    public static class AudioUtils
    {
        public static string GetZSFXName(ZSFX zsfx)
        {
            string name = zsfx.name;
            char[] separators = { '(', ' ' };
            int index = name.IndexOfAny(separators);
            return index != -1 ? name.Remove(index) : name;
        }

        public static void SafeSetVolume(AudioSource source, float volume)
        {
            if (source != null)
            {
                source.volume = Mathf.Clamp01(volume);
            }
        }
    }

    // 리플렉션 캐시 시스템
    public static class ReflectionCache
    {
        private static readonly Dictionary<string, FieldInfo> _fieldCache = new Dictionary<string, FieldInfo>();
        private static readonly Dictionary<string, System.Reflection.PropertyInfo> _propertyCache = new Dictionary<string, System.Reflection.PropertyInfo>();

        public static FieldInfo GetField(Type type, string fieldName)
        {
            string key = $"{type.FullName}.{fieldName}";
            if (!_fieldCache.ContainsKey(key))
            {
                _fieldCache[key] = type.GetField(fieldName);
            }
            return _fieldCache[key];
        }

        public static void SetValue(object instance, string fieldName, object value)
        {
            var field = GetField(instance.GetType(), fieldName);
            field?.SetValue(instance, value);
        }

        public static T GetValue<T>(object instance, string fieldName)
        {
            var field = GetField(instance.GetType(), fieldName);
            return field != null ? (T)field.GetValue(instance) : default;
        }
    }

    // 문자열 매칭 캐시
    public static class StringMatchCache
    {
        private static readonly Dictionary<string, string> _lowerCaseCache = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> _musicMappingCache = new Dictionary<string, string>();
        private static bool _initialized = false;

        public static string GetLowerCase(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            if (!_lowerCaseCache.ContainsKey(input))
            {
                _lowerCaseCache[input] = input.ToLower();
            }
            return _lowerCaseCache[input];
        }

        public static void Initialize()
        {
            if (_initialized) return;

            // 특수 매핑 규칙
            AddMapping("menu", "menu", "start", "intro", "mainmenu", "main_menu", "title");
            AddMapping("meadows", "meadow", "meadows");
            AddMapping("blackforest", "forest", "dark", "blackforest");
            AddMapping("swamp", "swamp");

            _initialized = true;
        }

        private static void AddMapping(string target, params string[] sources)
        {
            foreach (string source in sources)
            {
                _musicMappingCache[$"{source}->{target}"] = target;
            }
        }

        public static string FindMatch(string musicName)
        {
            if (string.IsNullOrEmpty(musicName)) return null;

            // 정확한 매칭
            if (Plugin.CustomMusicList.ContainsKey(musicName))
                return musicName;

            // 캐시된 매핑
            if (_musicMappingCache.ContainsKey(musicName))
                return _musicMappingCache[musicName];

            // 부분 매칭
            string lowerName = GetLowerCase(musicName);
            foreach (var key in Plugin.CustomMusicList.Keys)
            {
                string lowerKey = GetLowerCase(key);
                if (lowerName.Contains(lowerKey) || lowerKey.Contains(lowerName))
                {
                    _musicMappingCache[musicName] = key;
                    return key;
                }
            }

            return null;
        }
    }
}
