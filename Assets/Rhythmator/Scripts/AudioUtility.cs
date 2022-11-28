#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEngine;

namespace UnityEditor {
    public static class AudioUtility {

        public static void PlayClip(AudioClip clip, int startSample, bool loop) {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

#if UNITY_2019
            MethodInfo method = audioUtilClass.GetMethod(
                "PlayClip",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new System.Type[] {
                            typeof(AudioClip),
                            typeof(Int32),
                            typeof(Boolean)
                },
                null
                );
#else
			MethodInfo method = audioUtilClass.GetMethod(
				"PlayPreviewClip",
				BindingFlags.Static | BindingFlags.Public,
				null,
				new System.Type[] {
				typeof(AudioClip),
				typeof(Int32),
				typeof(Boolean)
				},
				null
				);
#endif

            if (method == null) Debug.LogError("Method not found");
            method.Invoke(
                null,
                new object[] {
                clip,
                startSample,
                loop
                }
            );
        }

        public static void PlayClip(AudioClip clip, int startSample) {
            PlayClip(clip, startSample, false);
        }

        public static void PlayClip(AudioClip clip) {
            PlayClip(clip, 0);
        }

        public static void StopClip(AudioClip clip) {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
#if UNITY_2019
            MethodInfo method = audioUtilClass.GetMethod(
                "StopClip",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new System.Type[] {
                            typeof(AudioClip)
            },
            null
);
#else
			MethodInfo method = audioUtilClass.GetMethod(
				"StopPreviewClip",
				BindingFlags.Static | BindingFlags.Public,
				null,
				new System.Type[] {
				typeof(AudioClip)
			},
			null
			);
#endif

            if (method == null) Debug.LogError("Method not found");
            method.Invoke(
                null,
                new object[] {
                clip
            }
            );
        }

        public static void PauseClip(AudioClip clip) {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
#if UNITY_2019
            MethodInfo method = audioUtilClass.GetMethod(
                    "PauseClip",
                    BindingFlags.Static | BindingFlags.Public,
                    null,
                    new System.Type[] {
        typeof(AudioClip)
                },
                null
                );
#else
MethodInfo method = audioUtilClass.GetMethod(
				"PausePreviewClip",
				BindingFlags.Static | BindingFlags.Public,
				null,
				new System.Type[] {
				typeof(AudioClip)
			},
			null
			);
#endif

            if (method == null) Debug.LogError("Method not found");
            method.Invoke(
                null,
                new object[] {
                clip
            }
            );
        }

        public static void ResumeClip(AudioClip clip) {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
#if UNITY_2019
            MethodInfo method = audioUtilClass.GetMethod(
                "ResumeClip",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new System.Type[] {
                typeof(AudioClip)
            },
            null
            );
#else
MethodInfo method = audioUtilClass.GetMethod(
                "ResumePreviewClip",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new System.Type[] {
                typeof(AudioClip)
            },
            null
            );
#endif

            if (method == null) Debug.LogError("Method not found");
            method.Invoke(
                null,
                new object[] {
                clip
            }
            );
        }

        public static void LoopClip(AudioClip clip, bool on) {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
#if UNITY_2019
            MethodInfo method = audioUtilClass.GetMethod(
                            "LoopClip",
                            BindingFlags.Static | BindingFlags.Public,
                            null,
                            new System.Type[] {
                typeof(AudioClip),
                typeof(bool)
                        },
                        null
                        );
#else
MethodInfo method = audioUtilClass.GetMethod(
                "LoopPreviewClip",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new System.Type[] {
                typeof(AudioClip),
                typeof(bool)
            },
            null
            );
#endif

            if (method == null) Debug.LogError("Method not found");
            method.Invoke(
                null,
                new object[] {
                clip,
                on
            }
            );
        }

        public static bool IsClipPlaying(AudioClip clip) {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
#if UNITY_2019
            MethodInfo method = audioUtilClass.GetMethod(
                "IsClipPlaying",
                BindingFlags.Static | BindingFlags.Public
                );
#else
MethodInfo method = audioUtilClass.GetMethod(
                "IsPreviewClipPlaying",
                BindingFlags.Static | BindingFlags.Public
                );
#endif

            if (method == null) Debug.LogError("Method not found");
            bool playing = (bool)method.Invoke(
                null,
                new object[] {
                clip,
            }
            );

            return playing;
        }

        public static void StopAllClips() {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
#if UNITY_2019
            MethodInfo method = audioUtilClass.GetMethod(
                            "StopAllClips",
                            BindingFlags.Static | BindingFlags.Public
                            );
#else
MethodInfo method = audioUtilClass.GetMethod(
                "StopAllPreviewClips",
                BindingFlags.Static | BindingFlags.Public
                );
#endif

            if (method == null) Debug.LogError("Method not found");
            method.Invoke(
                null,
                null
                );
        }

        public static float GetClipPosition(AudioClip clip) {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
#if UNITY_2019
            MethodInfo method = audioUtilClass.GetMethod(
                "GetClipPosition",
                BindingFlags.Static | BindingFlags.Public
                );
            float position = (float)method.Invoke(
                null,
                new object[] {
                    clip
                }
            );
#else
            MethodInfo method = audioUtilClass.GetMethod(
                "GetPreviewClipPosition",
                BindingFlags.Static | BindingFlags.Public
                );
                float position = (float)method.Invoke(
                null,
                new object[] {
					//clip
				}
            );
#endif




            return position;
        }

        public static int GetClipSamplePosition(AudioClip clip) {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
#if UNITY_2019
            MethodInfo method = audioUtilClass.GetMethod(
                            "GetClipSamplePosition",
                            BindingFlags.Static | BindingFlags.Public
                            );
#else
MethodInfo method = audioUtilClass.GetMethod(
                "GetPreviewClipSamplePosition",
                BindingFlags.Static | BindingFlags.Public
                );
#endif

            if (method == null) Debug.LogError("Method not found");
            int position = (int)method.Invoke(
                null,
                new object[] {
                clip
            }
            );

            return position;
        }

        public static void SetClipSamplePosition(AudioClip clip, int iSamplePosition) {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
#if UNITY_2019
            MethodInfo method = audioUtilClass.GetMethod(
                            "SetClipSamplePosition",
                            BindingFlags.Static | BindingFlags.Public
                            );
#else
MethodInfo method = audioUtilClass.GetMethod(
                "SetPreviewClipSamplePosition",
                BindingFlags.Static | BindingFlags.Public
                );
#endif

            if (method == null) Debug.LogError("Method not found");
            method.Invoke(
                null,
                new object[] {
                clip,
                iSamplePosition
            }
            );
        }

        public static int GetSampleCount(AudioClip clip) {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
#if UNITY_2019

#else

#endif
            MethodInfo method = audioUtilClass.GetMethod(
                "GetSampleCount",
                BindingFlags.Static | BindingFlags.Public
                );
            if (method == null) Debug.LogError("Method not found");
            int samples = (int)method.Invoke(
                null,
                new object[] {
                clip
            }
            );

            return samples;
        }

        public static int GetChannelCount(AudioClip clip) {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(
                "GetChannelCount",
                BindingFlags.Static | BindingFlags.Public
                );
            if (method == null) Debug.LogError("Method not found");
            int channels = (int)method.Invoke(
                null,
                new object[] {
                clip
            }
            );

            return channels;
        }

        public static int GetBitRate(AudioClip clip) {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            MethodInfo method = audioUtilClass.GetMethod(
                "GetBitRate",
                BindingFlags.Static | BindingFlags.Public
                );
            if (method == null) Debug.LogError("Method not found");
            int bitRate = (int)method.Invoke(
                null,
                new object[] {
                clip
            }
            );

            return bitRate;
        }

        public static int GetBitsPerSample(AudioClip clip) {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            MethodInfo method = audioUtilClass.GetMethod(
                "GetBitsPerSample",
                BindingFlags.Static | BindingFlags.Public
                );
            if (method == null) Debug.LogError("Method not found");
            int bits = (int)method.Invoke(
                null,
                new object[] {
                clip
            }
            );

            return bits;
        }

        public static int GetFrequency(AudioClip clip) {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            MethodInfo method = audioUtilClass.GetMethod(
                "GetFrequency",
                BindingFlags.Static | BindingFlags.Public
                );
            if (method == null) Debug.LogError("Method not found");
            int frequency = (int)method.Invoke(
                null,
                new object[] {
                clip
            }
            );

            return frequency;
        }

        public static int GetSoundSize(AudioClip clip) {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            MethodInfo method = audioUtilClass.GetMethod(
                "GetSoundSize",
                BindingFlags.Static | BindingFlags.Public
                );
            if (method == null) Debug.LogError("Method not found");
            int size = (int)method.Invoke(
                null,
                new object[] {
                clip
            }
            );

            return size;
        }

        public static Texture2D GetWaveForm(AudioClip clip, int channel, float width, float height) {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(
                            "GetWaveForm",
                            BindingFlags.Static | BindingFlags.Public
                            );
            if (method == null) Debug.LogError("Method not found");
            string path = AssetDatabase.GetAssetPath(clip);
            AudioImporter importer = (AudioImporter)AssetImporter.GetAtPath(path);

            Texture2D texture = (Texture2D)method.Invoke(
                null,
                new object[] {
                clip,
                importer,
                channel,
                width,
                height
            }
            );

            return texture;


        }

        public static Texture2D GetWaveFormFast(AudioClip clip, int channel, int fromSample, int toSample, float width, float height) {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(
                            "GetWaveFormFast",
                            BindingFlags.Static | BindingFlags.Public
                            );
            if (method == null) Debug.LogError("Method not found");
            Texture2D texture = (Texture2D)method.Invoke(
                null,
                new object[] {
                clip,
                channel,
                fromSample,
                toSample,
                width,
                height
            }
            );

            return texture;

        }

        public static void ClearWaveForm(AudioClip clip) {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(
                "ClearWaveForm",
                BindingFlags.Static | BindingFlags.Public
                );
            if (method == null) Debug.LogError("Method not found");
            method.Invoke(
                null,
                new object[] {
                clip
            }
            );


        }

        public static bool HasPreview(AudioClip clip) {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            MethodInfo method = audioUtilClass.GetMethod(
                "HasPreview",
                BindingFlags.Static | BindingFlags.Public
                );
            if (method == null) Debug.LogError("Method not found");
            bool hasPreview = (bool)method.Invoke(
                null,
                new object[] {
                clip
            }
            );

            return hasPreview;
        }

        public static bool IsCompressed(AudioClip clip) {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(
                "IsCompressed",
                BindingFlags.Static | BindingFlags.Public
                );
            if (method == null) Debug.LogError("Method not found");
            bool isCompressed = (bool)method.Invoke(
                null,
                new object[] {
                clip
            }
            );

            return isCompressed;

        }

        public static bool IsStramed(AudioClip clip) {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(
                "IsStramed",
                BindingFlags.Static | BindingFlags.Public
                );
            if (method == null) Debug.LogError("Method not found");
            bool isStreamed = (bool)method.Invoke(
                null,
                new object[] {
                clip
            }
            );
            return isStreamed;



        }

        public static double GetDuration(AudioClip clip) {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            MethodInfo method = audioUtilClass.GetMethod(
                "GetDuration",
                BindingFlags.Static | BindingFlags.Public
                );
            if (method == null) Debug.LogError("Method not found");
            double duration = (double)method.Invoke(
                null,
                new object[] {
                clip
            }
            );

            return duration;
        }

        public static int GetFMODMemoryAllocated() {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            MethodInfo method = audioUtilClass.GetMethod(
                "GetFMODMemoryAllocated",
                BindingFlags.Static | BindingFlags.Public
                );
            if (method == null) Debug.LogError("Method not found");
            int memoryAllocated = (int)method.Invoke(
                null,
                null
            );

            return memoryAllocated;
        }

        public static float GetFMODCPUUsage() {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(
                "GetFMODCPUUsage",
                BindingFlags.Static | BindingFlags.Public
                );
            if (method == null) Debug.LogError("Method not found");
            float cpuUsage = (float)method.Invoke(
                null,
                null
                );

            return cpuUsage;
        }

        public static bool Is3D(AudioClip clip) {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(
                "Is3D",
                BindingFlags.Static | BindingFlags.Public
                );
            if (method == null) Debug.LogError("Method not found");
            bool is3D = (bool)method.Invoke(
                null,
                new object[] {
                clip
            }
            );

            return is3D;

        }

        public static bool IsMovieAudio(AudioClip clip) {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(
                "IsMovieAudio",
                BindingFlags.Static | BindingFlags.Public
                );
            if (method == null) Debug.LogError("Method not found");
            bool isMovieAudio = (bool)method.Invoke(
                null,
                new object[] {
                clip
            }
            );

            return isMovieAudio;

        }

        public static bool IsMOD(AudioClip clip) {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(
                "IsMOD",
                BindingFlags.Static | BindingFlags.Public
                );
            if (method == null) Debug.LogError("Method not found");
            bool isMOD = (bool)method.Invoke(
                null,
                new object[] {
                clip
            }
            );

            return isMOD;


        }

        public static int GetMODChannelCount() {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(
                           "GetMODChannelCount",
                           BindingFlags.Static | BindingFlags.Public
                           );
            if (method == null) Debug.LogError("Method not found");
            int channels = (int)method.Invoke(
                null,
                null
            );

            return channels;

        }

        public static AnimationCurve GetLowpassCurve(AudioLowPassFilter lowPassFilter) {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            MethodInfo method = audioUtilClass.GetMethod(
                "GetLowpassCurve",
                BindingFlags.Static | BindingFlags.Public
                );
            if (method == null) Debug.LogError("Method not found");
            AnimationCurve curve = (AnimationCurve)method.Invoke(
                null,
                new object[] {
                lowPassFilter
            }
            );

            return curve;
        }

        public static Vector3 GetListenerPos() {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            MethodInfo method = audioUtilClass.GetMethod(
                "GetListenerPos",
                BindingFlags.Static | BindingFlags.Public
                );
            if (method == null) Debug.LogError("Method not found");
            Vector3 position = (Vector3)method.Invoke(
                null,
                null
            );

            return position;
        }

        public static void UpdateAudio() {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            MethodInfo method = audioUtilClass.GetMethod(
                "UpdateAudio",
                BindingFlags.Static | BindingFlags.Public
                );
            if (method == null) Debug.LogError("Method not found");
            method.Invoke(
                null,
                null
                );
        }

        public static void SetListenerTransform(Transform t) {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            MethodInfo method = audioUtilClass.GetMethod(
                "SetListenerTransform",
                BindingFlags.Static | BindingFlags.Public
                );
            if (method == null) Debug.LogError("Method not found");
            method.Invoke(
                null,
                new object[] {
                t
            }
            );
        }

        public static AudioType GetClipType(AudioClip clip) {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(
                "GetClipType",
                BindingFlags.Static | BindingFlags.Public
                );
            if (method == null) Debug.LogError("Method not found");
            AudioType type = (AudioType)method.Invoke(
                null,
                new object[] {
                clip
            }
            );

            return type;
        }

        [Obsolete]
        public static AudioType GetPlatformConversionType(AudioType inType, BuildTargetGroup targetGroup, AudioImporterFormat format) {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            MethodInfo method = audioUtilClass.GetMethod(
                "GetPlatformConversionType",
                BindingFlags.Static | BindingFlags.Public
                );
            if (method == null) Debug.LogError("Method not found");
            AudioType type = (AudioType)method.Invoke(
                null,
                new object[] {
                inType,
                targetGroup,
                format
            }
            );

            return type;
        }

        public static bool HaveAudioCallback(MonoBehaviour behaviour) {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
#if UNITY_2019
            MethodInfo method = audioUtilClass.GetMethod(
                "HasAudioCallback",
                BindingFlags.Static | BindingFlags.Public
                );
#else
MethodInfo method = audioUtilClass.GetMethod(
                "HaveAudioCallback",
                BindingFlags.Static | BindingFlags.Public
                );
#endif

            if (method == null) Debug.LogError("Method not found");
            bool hasCallback = (bool)method.Invoke(
                null,
                new object[] {
                behaviour
            }
            );

            return hasCallback;
        }

        public static int GetCustomFilterChannelCount(MonoBehaviour behaviour) {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(
                "GetCustomFilterChannelCount",
                BindingFlags.Static | BindingFlags.Public
                );
            if (method == null) Debug.LogError("Method not found");
            int channels = (int)method.Invoke(
                null,
                new object[] {
                behaviour
            }
            );

            return channels;
        }

        public static int GetCustomFilterProcessTime(MonoBehaviour behaviour) {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            MethodInfo method = audioUtilClass.GetMethod(
                "GetCustomFilterProcessTime",
                BindingFlags.Static | BindingFlags.Public
                );
            if (method == null) Debug.LogError("Method not found");
            int processTime = (int)method.Invoke(
                null,
                new object[] {
                behaviour
            }
            );

            return processTime;
        }

        public static float GetCustomFilterMaxIn(MonoBehaviour behaviour, int channel) {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            MethodInfo method = audioUtilClass.GetMethod(
                "GetCustomFilterMaxIn",
                BindingFlags.Static | BindingFlags.Public
                );
            if (method == null) Debug.LogError("Method not found");
            float maxIn = (float)method.Invoke(
                null,
                new object[] {
                behaviour,
                channel
            }
            );

            return maxIn;
        }

        public static float GetCustomFilterMaxOut(MonoBehaviour behaviour, int channel) {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            MethodInfo method = audioUtilClass.GetMethod(
                "GetCustomFilterMaxOut",
                BindingFlags.Static | BindingFlags.Public
                );
            if (method == null) Debug.LogError("Method not found");
            float maxOut = (float)method.Invoke(
                null,
                new object[] {
                behaviour,
                channel
            }
            );

            return maxOut;
        }
    }
}
#endif