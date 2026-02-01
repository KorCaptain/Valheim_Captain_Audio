using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace CaptainAudio
{
    // 음악 시스템 패치
    [HarmonyPatch(typeof(MusicMan), "Awake")]
    public static class MusicMan_Awake_Patch
    {
        static void Postfix(MusicMan __instance)
        {
            if (!Plugin.ModEnabled.Value) return;

            for (int i = 0; i < __instance.m_music.Count; i++)
            {
                var music = __instance.m_music[i];
                string musicName = ReflectionCache.GetValue<string>(music, "m_name");

                // 폴더 매칭 (정확한 이름 우선)
                if (Plugin.CustomMusicList.ContainsKey(musicName))
                {
                    var clips = Plugin.CustomMusicList[musicName].Values.ToArray();
                    ReflectionCache.SetValue(music, "m_clips", clips);
                }
                // 부분 매칭
                else
                {
                    string matchedKey = StringMatchCache.FindMatch(musicName);
                    if (matchedKey != null && Plugin.CustomMusicList.ContainsKey(matchedKey))
                    {
                        var clips = Plugin.CustomMusicList[matchedKey].Values.ToArray();
                        ReflectionCache.SetValue(music, "m_clips", clips);
                    }
                    // 개별 클립 교체 (폴백)
                    else
                    {
                        AudioClip[] currentClips = ReflectionCache.GetValue<AudioClip[]>(music, "m_clips");
                        if (currentClips != null)
                        {
                            for (int j = 0; j < currentClips.Length; j++)
                            {
                                if (currentClips[j] != null && Plugin.CustomMusic.ContainsKey(currentClips[j].name))
                                {
                                    currentClips[j] = Plugin.CustomMusic[currentClips[j].name];
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(MusicMan), "UpdateMusic")]
    public static class MusicMan_UpdateMusic_Patch
    {
        static void Prefix(ref object ___m_queuedMusic, AudioSource ___m_musicSource)
        {
            if (!Plugin.ModEnabled.Value) return;

            // 볼륨 설정
            if (___m_queuedMusic != null)
            {
                ReflectionCache.SetValue(___m_queuedMusic, "m_volume", Plugin.MusicVolume.Value);
            }

            // 루프 비활성화 - 안전한 방식으로 변경
            if (___m_musicSource != null && ___m_musicSource.loop)
            {
                // 재생 중이 아닐 때만 루프 설정 변경 (FMOD seek position 에러 방지)
                if (!___m_musicSource.isPlaying)
                {
                    ___m_musicSource.loop = false;
                }
                // 재생 중이면 클립 끝에서 자연스럽게 종료되도록 설정
                else if (___m_musicSource.clip != null)
                {
                    // 클립이 거의 끝났을 때만 루프 해제
                    float remaining = ___m_musicSource.clip.length - ___m_musicSource.time;
                    if (remaining < 0.5f)
                    {
                        ___m_musicSource.loop = false;
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(MusicLocation), "Awake")]
    public static class MusicLocation_Awake_Patch
    {
        static void Postfix(ref AudioSource ___m_audioSource, ref float ___m_baseVolume)
        {
            if (!Plugin.ModEnabled.Value || ___m_audioSource == null || ___m_audioSource.clip == null) return;

            string clipName = ___m_audioSource.clip.name;
            if (Plugin.CustomMusic.ContainsKey(clipName))
            {
                AudioClip newClip = Plugin.CustomMusic[clipName];
                if (newClip == null || newClip.length <= 0) return;

                // 재생 중이면 안전하게 정지 후 교체 (FMOD seek position 에러 방지)
                bool wasPlaying = ___m_audioSource.isPlaying;
                if (wasPlaying)
                {
                    ___m_audioSource.Stop();
                }

                ___m_audioSource.clip = newClip;
                ___m_audioSource.time = 0f;  // seek position 초기화
                ___m_baseVolume *= Plugin.LocationVolMultiplier.Value;

                if (wasPlaying)
                {
                    ___m_audioSource.Play();
                }
            }
        }
    }

    // 환경음 패치
    [HarmonyPatch(typeof(AudioMan), "Awake")]
    public static class AudioMan_Awake_Patch
    {
        static void Postfix(AudioMan __instance, IList ___m_randomAmbients, AudioSource ___m_oceanAmbientSource, AudioSource ___m_windLoopSource)
        {
            if (!Plugin.ModEnabled.Value) return;

            for (int i = 0; i < ___m_randomAmbients.Count; i++)
            {
                var ambient = ___m_randomAmbients[i];
                string ambientName = ReflectionCache.GetValue<string>(ambient, "m_name");

                // 개별 클립 교체
                ReplaceAmbientClips(ambient, "m_randomAmbientClips");
                ReplaceAmbientClips(ambient, "m_randomAmbientClipsDay");
                ReplaceAmbientClips(ambient, "m_randomAmbientClipsNight");

                // 리스트 전체 교체
                ReplaceAmbientList(ambient, ambientName, "_day", "m_randomAmbientClipsDay");
                ReplaceAmbientList(ambient, ambientName, "_night", "m_randomAmbientClipsNight");
                ReplaceAmbientList(ambient, ambientName, "", "m_randomAmbientClips");
            }

            // 특수 환경음
            if (Plugin.CustomAmbient.ContainsKey("ocean"))
            {
                ___m_oceanAmbientSource.clip = Plugin.CustomAmbient["ocean"];
            }
            if (Plugin.CustomAmbient.ContainsKey("wind"))
            {
                ___m_windLoopSource.clip = Plugin.CustomAmbient["wind"];
            }
        }

        private static void ReplaceAmbientClips(object ambient, string fieldName)
        {
            IList clips = ReflectionCache.GetValue<IList>(ambient, fieldName);
            if (clips == null) return;

            for (int i = 0; i < clips.Count; i++)
            {
                AudioClip clip = clips[i] as AudioClip;
                if (clip != null && Plugin.CustomAmbient.ContainsKey(clip.name))
                {
                    clips[i] = Plugin.CustomAmbient[clip.name];
                }
            }
        }

        private static void ReplaceAmbientList(object ambient, string ambientName, string suffix, string fieldName)
        {
            string key = ambientName + suffix;
            if (!Plugin.CustomAmbientList.ContainsKey(key)) return;

            IList clips = ReflectionCache.GetValue<IList>(ambient, fieldName);
            if (clips == null) return;

            var newClips = Plugin.CustomAmbientList[key].Values.ToList();
            clips.Clear();
            foreach (var clip in newClips)
            {
                clips.Add(clip);
            }
        }
    }

    [HarmonyPatch(typeof(AudioMan), "QueueAmbientLoop")]
    public static class AudioMan_QueueAmbientLoop_Patch
    {
        static void Prefix(ref float ___m_queuedAmbientVol, ref float ___m_ambientVol, ref float vol)
        {
            if (!Plugin.ModEnabled.Value) return;

            vol = Plugin.AmbientVolume.Value;
            ___m_ambientVol = Plugin.AmbientVolume.Value;
            ___m_queuedAmbientVol = Plugin.AmbientVolume.Value;
        }
    }

    // 효과음 패치
    [HarmonyPatch(typeof(ZSFX), "Awake")]
    public static class ZSFX_Awake_Patch
    {
        static void Postfix(ZSFX __instance)
        {
            if (!Plugin.ModEnabled.Value) return;

            string zsfxName = AudioUtils.GetZSFXName(__instance);

            // 리스트 교체
            if (Plugin.CustomSFXList.TryGetValue(zsfxName, out var value))
            {
                __instance.m_audioClips = value.OrderBy(x => x.Key).Select(x => x.Value).ToArray();
                return;
            }

            // 개별 클립 교체
            if (__instance.m_audioClips != null)
            {
                for (int i = 0; i < __instance.m_audioClips.Length; i++)
                {
                    if (__instance.m_audioClips[i] != null && Plugin.CustomSFX.ContainsKey(__instance.m_audioClips[i].name))
                    {
                        __instance.m_audioClips[i] = Plugin.CustomSFX[__instance.m_audioClips[i].name];
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(TeleportWorld), "Awake")]
    public static class TeleportWorld_Awake_Patch
    {
        static void Postfix(TeleportWorld __instance)
        {
            if (!Plugin.ModEnabled.Value || !Plugin.CustomSFX.ContainsKey("portal")) return;

            AudioSource source = __instance.GetComponentInChildren<AudioSource>();
            if (source != null)
            {
                AudioClip newClip = Plugin.CustomSFX["portal"];
                if (newClip == null || newClip.length <= 0) return;

                // 안전한 클립 교체 (GameObject 토글 대신 직접 제어)
                bool wasPlaying = source.isPlaying;
                source.Stop();
                source.clip = newClip;
                source.time = 0f;

                if (wasPlaying || source.playOnAwake)
                {
                    source.Play();
                }
            }
        }
    }

    [HarmonyPatch(typeof(Fireplace), "Start")]
    public static class Fireplace_Start_Patch
    {
        static void Postfix(Fireplace __instance)
        {
            if (!Plugin.ModEnabled.Value) return;

            string name = __instance.name;
            AudioSource source = null;
            AudioClip newClip = null;

            if (name.Contains("groundtorch") && Plugin.CustomSFX.ContainsKey("groundtorch"))
            {
                source = __instance.m_enabledObject?.GetComponentInChildren<AudioSource>();
                newClip = Plugin.CustomSFX["groundtorch"];
            }
            else if (name.Contains("walltorch") && Plugin.CustomSFX.ContainsKey("walltorch"))
            {
                source = __instance.m_enabledObjectHigh?.GetComponentInChildren<AudioSource>();
                if (source == null) source = __instance.m_enabledObject?.GetComponentInChildren<AudioSource>();
                newClip = Plugin.CustomSFX["walltorch"];
            }
            else if (name.Contains("fire_pit") && Plugin.CustomSFX.ContainsKey("fire_pit"))
            {
                source = __instance.m_enabledObjectHigh?.GetComponentInChildren<AudioSource>();
                newClip = Plugin.CustomSFX["fire_pit"];
            }
            else if (name.Contains("bonfire") && Plugin.CustomSFX.ContainsKey("bonfire"))
            {
                source = __instance.m_enabledObjectHigh?.GetComponentInChildren<AudioSource>();
                newClip = Plugin.CustomSFX["bonfire"];
            }
            else if (name.Contains("hearth") && Plugin.CustomSFX.ContainsKey("hearth"))
            {
                source = __instance.m_enabledObjectHigh?.GetComponentInChildren<AudioSource>();
                newClip = Plugin.CustomSFX["hearth"];
            }

            // 안전한 클립 교체
            if (source != null && newClip != null && newClip.length > 0)
            {
                SafeReplaceClip(source, newClip);
            }
        }

        private static void SafeReplaceClip(AudioSource source, AudioClip newClip)
        {
            bool wasPlaying = source.isPlaying;
            source.Stop();
            source.clip = newClip;
            source.time = 0f;
            if (wasPlaying || source.playOnAwake)
            {
                source.Play();
            }
        }
    }

    // 환경별 음악 패치
    [HarmonyPatch(typeof(EnvMan), "Awake")]
    public static class EnvMan_Awake_Patch
    {
        static void Postfix(EnvMan __instance)
        {
            if (!Plugin.ModEnabled.Value) return;

            for (int i = 0; i < __instance.m_environments.Count; i++)
            {
                string envName = __instance.m_environments[i].m_name;

                // 직접 매칭
                foreach (var customKey in Plugin.CustomMusicList.Keys)
                {
                    string lowerEnv = envName.ToLower();
                    string lowerKey = customKey.ToLower();

                    if (lowerEnv == lowerKey || lowerEnv.Contains(lowerKey) || lowerKey.Contains(lowerEnv))
                    {
                        string originalName = __instance.m_environments[i].m_name;
                        __instance.m_environments[i].m_name = customKey;
                        AddMusicToEnvironment(__instance, i, "");
                        __instance.m_environments[i].m_name = originalName;
                        break;
                    }
                }

                // 시간대별 매칭
                AddMusicToEnvironment(__instance, i, "Morning");
                AddMusicToEnvironment(__instance, i, "Day");
                AddMusicToEnvironment(__instance, i, "Evening");
                AddMusicToEnvironment(__instance, i, "Night");
            }
        }

        private static void AddMusicToEnvironment(EnvMan envMan, int index, string timeOfDay)
        {
            string musicName = envMan.m_environments[index].m_name + timeOfDay;
            if (!Plugin.CustomMusicList.ContainsKey(musicName)) return;

            // 환경에 음악 할당
            switch (timeOfDay)
            {
                case "Morning": envMan.m_environments[index].m_musicMorning = musicName; break;
                case "Day": envMan.m_environments[index].m_musicDay = musicName; break;
                case "Evening": envMan.m_environments[index].m_musicEvening = musicName; break;
                case "Night": envMan.m_environments[index].m_musicNight = musicName; break;
            }

            // MusicMan에 새 음악 추가
            var musicType = MusicMan.instance.m_music[0].GetType();
            var newMusic = Activator.CreateInstance(musicType);

            ReflectionCache.SetValue(newMusic, "m_name", musicName);
            ReflectionCache.SetValue(newMusic, "m_clips", Plugin.CustomMusicList[musicName].OrderBy(x => x.Key).Select(x => x.Value).ToArray());
            ReflectionCache.SetValue(newMusic, "m_loop", true);
            ReflectionCache.SetValue(newMusic, "m_ambientMusic", true);
            ReflectionCache.SetValue(newMusic, "m_resume", true);

            ((IList)MusicMan.instance.m_music).Add(newMusic);
        }
    }

    // 콘솔 명령어 패치
    [HarmonyPatch(typeof(Terminal), "InputText")]
    public static class Terminal_InputText_Patch
    {
        static bool Prefix(Terminal __instance)
        {
            if (!Plugin.ModEnabled.Value) return true;

            var inputField = __instance.GetType().GetField("m_input");
            if (inputField == null) return true;

            var inputComponent = inputField.GetValue(__instance);
            if (inputComponent == null) return true;

            var textProperty = inputComponent.GetType().GetProperty("text");
            if (textProperty == null) return true;

            string text = (string)textProperty.GetValue(inputComponent);
            string lowerText = text.ToLower();

            if (lowerText == "captainaudio reset")
            {
                Plugin.instance.Config.Reload();
                Plugin.instance.Config.Save();
                AddOutput(__instance, text);
                AddOutput(__instance, "<color=#00BFFF>Captain Audio config reloaded</color>");
                return false;
            }

            if (lowerText == "captainaudio music")
            {
                AddOutput(__instance, text);
                if (EnvMan.instance != null)
                {
                    string currentMusic = Player.m_localPlayer?.IsSafeInHome() == true ? "home" : EnvMan.instance.GetAmbientMusic();
                    AddOutput(__instance, $"<color=#00BFFF>Current: {currentMusic} | Folders: {Plugin.CustomMusicList.Count} | Clips: {Plugin.CustomMusic.Count}</color>");
                }
                return false;
            }

            if (lowerText == "captainaudio env")
            {
                AddOutput(__instance, text);
                if (EnvMan.instance != null)
                {
                    AddOutput(__instance, $"<color=#00BFFF>Environment: {EnvMan.instance.GetCurrentEnvironment().m_name}</color>");
                }
                else
                {
                    AddOutput(__instance, "<color=#00BFFF>Must be called in-game</color>");
                }
                return false;
            }

            return true;
        }

        private static void AddOutput(Terminal terminal, string text)
        {
            Traverse.Create(terminal).Method("AddString", new object[] { text }).GetValue();
        }
    }
}
