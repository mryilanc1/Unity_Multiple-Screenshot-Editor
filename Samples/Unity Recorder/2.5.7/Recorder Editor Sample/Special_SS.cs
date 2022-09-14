
//Editor Window Class
using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Threading.Tasks;
using UnityEditor.Recorder.Input;

namespace UnityEditor.Recorder.Special_SS
{

    public class Special_SS : EditorWindow
    {
        public Device_List root = new Device_List();
        private SerializedObject so;
        string  localPath = "ScreenShots/";
        public static string guide = "Guide";

        [MenuItem("ScreenShoot/SS Window")]
        public static void ShowWindow()
        {
            Special_SS window = (Special_SS) EditorWindow.GetWindow(typeof(Special_SS), true, "SS Window");
            window.Show();
        }

        private int device_count;
        private int Space_a = 300;
        public static string location;
        static string datapath;


        void OnGUI()
        {
            datapath = PlayerPrefs.GetString("location");
            EditorGUILayout.LabelField("Folder: "+datapath, EditorStyles.helpBox,GUILayout.Height(20f));
           
            
            EditorGUILayout.LabelField(guide, EditorStyles.helpBox,GUILayout.Height(30f));
            if (location ==null)
            {
                guide = "You have to select folder for save Screenshot";

            }
            else if (root.Device.Count == 0)
            {
                guide = "You have to add device resolution ";

            }
            EditorGUILayout.Space();
            
            
            ScriptableObject target = this;
            so = new SerializedObject(target);
            SerializedProperty stringsProperty = so.FindProperty("root");
            EditorGUILayout.PropertyField(stringsProperty, true); // True means show children
            so.ApplyModifiedProperties();
            
            
            if (root != null)
                switch (root.Device.Count)
                {
                    case 0:
                        Space_a = 100;
                        break;

                    case 1:
                        Space_a = 175;
                        break;

                    case 2:
                        Space_a = 230;
                        break;

                    case 3:
                        Space_a = 320;
                        break;

                    case 4:
                        Space_a = 410;
                        break;

                    case 5:
                        Space_a = 500;
                        break;

                    case 6:
                        Space_a = 560;

                        break;


                    case 7:
                        Space_a = 635;

                        break;

                    case 8:
                        Space_a = 710;

                        break;

                    default:
                        Space_a = 800;

                        break;
                }

            
            if (EditorApplication.isPlaying != true)
            {
                GUI.backgroundColor = Color.red;
            }
            else
            {
                GUI.backgroundColor = Color.yellow;
            }

            if (GUI.Button(new Rect(5, Space_a + 45, 150, 50), "Play Unity"))
            {
                if (EditorApplication.isPlaying != true)
                    EditorApplication.isPlaying = true;
            }

            if (EditorApplication.isPlaying != true)
            {
                GUI.backgroundColor = Color.red;
            }
            else
            {
                GUI.backgroundColor = Color.green;
            }

            if (GUI.Button(new Rect(165, Space_a + 45, 150, 50), "Take Screenshot"))
            {
            
                if (EditorApplication.isPlaying == true & root.Device.Count !=0)
                {
                    DelayUseAsync();
                    guide = "Complated";
                }
                else if (EditorApplication.isPlaying == true & root.Device.Count ==0)
                {
                    guide = "You have to add device resolution ";
                }
                else
                {
                    guide = "You have to Play Unity";
                }

            }
            

            GUI.backgroundColor = Color.yellow;

            if (GUI.Button(new Rect(165, Space_a + 100, 150, 50), "Load Json"))
            {
                TextAsset json_yazi = new TextAsset(File.ReadAllText(Application.dataPath + "/Resources/SS_Json.json"));
                
                root = JsonUtility.FromJson<Device_List>(json_yazi.text);
                guide = "You have to Play Unity after Click to Take ScreenShot";
                Debug.Log(root.Device[0].DeviceName);

            }

            

          
            
            GUI.backgroundColor = Color.yellow;

            if (GUI.Button(new Rect(5, Space_a + 100, 150, 50), "Save to Json"))
            {
                string Json_added = JsonUtility.ToJson(root);
                File.WriteAllText(Application.dataPath + "/Resources/SS_Json.json",Json_added);
            }
            
            if (location == null)
            {
                GUI.backgroundColor = Color.red;
            }
            else
            {
                GUI.backgroundColor = Color.green;
            }
            if (GUI.Button(new Rect(165, Space_a + 155, 150, 50), "Show Folder"))
            {
               
                Application.OpenURL(PlayerPrefs.GetString("location"));

            }
            
            
            
            if (GUI.Button(new Rect(5, Space_a + 155, 150, 50), "Where Save SS"))
            {
                location = EditorUtility.OpenFolderPanel(location, "", "");
                PlayerPrefs.SetString("location",location);
                if (root.Device.Count == 0)
                {
                    guide = "You have to add device resolution ";
                }
                else 
                {
                    guide = "You have to Play Unity after Click to Take ScreenShot";
                }
                GUI.backgroundColor = Color.green;

            }
            if (Time.timeScale == 0)
            {
                GUI.backgroundColor = Color.green;
                
            }
            else
            {
                GUI.backgroundColor = Color.red;
            }
          
            if (GUI.Button(new Rect(5, Space_a + 210, 150, 50), "Time Scale 1 "))
            {
                
                Time.timeScale = 1;
            }
          
            async void DelayUseAsync()
            {
                Debug.Log(datapath+"+/+");
                Debug.Log(Path.IsPathRooted(datapath));
                if (Path.IsPathRooted(datapath) )
                {
                    for (int i = 0; i < root.Device.Count; i++)
                    {
                        Time.timeScale = 0;
                        Take_SS(root.Device[i].DeviceName, root.Device[i].Height, root.Device[i].Width);
                        await Task.Delay(1250);
                        
                    }
                   
                }
                else
                {
                    guide = "!!! Write Path of outfile";
                }
            }
            
            static void Take_SS(string name, int Height, int width)
            {
                var controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
                var m_RecorderController = new RecorderController(controllerSettings);

                // Image sequence
                var imageRecorder = ScriptableObject.CreateInstance<ImageRecorderSettings>();
                imageRecorder.name = "My Image Recorder";
                imageRecorder.Enabled = true;
                imageRecorder.OutputFormat = ImageRecorderSettings.ImageRecorderOutputFormat.PNG;
                imageRecorder.CaptureAlpha = false;

                Debug.Log(Path.Combine(Application.dataPath));
                var mediaOutputFolder = datapath;
                //  imageRecorder.OutputFile = Path.Combine(mediaOutputFolder, "image_") + DefaultWildcard.Frame;

                imageRecorder.imageInputSettings = new GameViewInputSettings
                {
                    OutputWidth = width,
                    OutputHeight = Height,
                };
                // Setup Recording

                controllerSettings.AddRecorderSettings(imageRecorder);
                controllerSettings.SetRecordModeToFrameInterval(0, 1);
                m_RecorderController.PrepareRecording();

                m_RecorderController.StartRecording();
                Debug.Log(mediaOutputFolder+"+++");
                imageRecorder.OutputFile = Path.Combine(mediaOutputFolder, name) + DefaultWildcard.Frame;
                var recorderWindow = EditorWindow.GetWindow<RecorderWindow>();
                recorderWindow.StopRecording();
            }

        }
    }
}
