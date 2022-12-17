
//Editor Window Class
using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Threading.Tasks;
using UnityEditor.Recorder.Input;

namespace UnityEditor.Recorder.MultipleScreenshotEditor
{

    public class MultipleScreenshotEditor : EditorWindow
    {
        public Device_List root = new Device_List();
        private SerializedObject so;
        string  localPath = "ScreenShots/";
        public static string guide = "Guide";
        GUIStyle guiStyle = new GUIStyle();
        Vector2 scroll = Vector2.zero;

        [MenuItem("Tool/Multiple Screenshot Editor")]
        public static void ShowWindow()
        {
            MultipleScreenshotEditor window = (MultipleScreenshotEditor) EditorWindow.GetWindow(typeof(MultipleScreenshotEditor), true, "Multiple Screenshot Editor");
            window.Show();
        }

        private int device_count;
        private int Space_a = 300;
        public static string location;
        public static string datapath;
        

        void OnGUI()
        {
          
            scroll = EditorGUILayout.BeginScrollView(scroll);
            datapath = PlayerPrefs.GetString("location");
           
            GUILayout.BeginVertical();
            
            guiStyle.fontSize = 11;
            guiStyle.normal.textColor = Color.white;
            EditorGUILayout.LabelField(("Outfile Folder: "+datapath ),guiStyle,GUILayout.Height(15));
            GUILayout.Space(2);
            EditorStyles.helpBox.fontSize = 12;
            EditorStyles.helpBox.normal.textColor=Color.yellow;
            EditorStyles.helpBox.fontStyle = FontStyle.Bold;
            EditorGUILayout.LabelField(guide, EditorStyles.helpBox,GUILayout.Height(30));
            GUILayout.Space(2);
            EditorGUILayout.LabelField("Number of the output folder : "+(PlayerPrefs.GetInt("sayac").ToString()),guiStyle,GUILayout.Height(15));
            GUILayout.EndVertical();
            
            if (datapath == "")
            {
                guide = "You have to select folder for save Screenshot";

            }
            else if (root.Device.Count == 0)
            {
                guide = "You have to add device resolution ";

            }
            EditorGUILayout.Space();


            EditorGUILayout.BeginVertical();
            
            ScriptableObject target = this;
            so = new SerializedObject(target);
            SerializedProperty stringsProperty = so.FindProperty("root");
            EditorGUILayout.PropertyField(stringsProperty, true); // True means show children
            so.ApplyModifiedProperties();
            
            EditorGUILayout.EndVertical();
    
            
            
            GUILayout.BeginHorizontal();
            
            if (EditorApplication.isPlaying != true &  root.Device.Count ==0 )
            {
                GUI.backgroundColor = Color.red;
            }
            if (EditorApplication.isPlaying != true & (datapath !=""|| root.Device.Count > 0) )
            {
                GUI.backgroundColor = Color.green;
            }
            else
            {
                GUI.backgroundColor = Color.yellow;

                if (datapath == "")
                {
                    Debug.Log(datapath +"You have to select folder for save Screenshot");
                    guide = "You have to select folder for save Screenshot";
                }
              
                
            }
            
            if (GUILayout.Button( "Play Unity",GUILayout.Height(50),GUILayout.MinWidth(160)))
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
            
           
            
            if (GUILayout.Button( "Take Screenshot",GUILayout.Height(50),GUILayout.MinWidth(160)))
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
           
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
       
            GUI.backgroundColor = Color.yellow;

            if (GUILayout.Button ("Load Json",GUILayout.Height(50),GUILayout.MinWidth(160)))
            {
                
                
                TextAsset json_yazi = new TextAsset(File.ReadAllText(Application.dataPath +"/mr.yilanci/Multiple Screenshot Editor/Json_Device_Save_List.json"));
                root = JsonUtility.FromJson<Device_List>(json_yazi.text);
                
                guide = "You have to Play Unity after Click to Take ScreenShot";
                Debug.Log("Loaded Device List From Json ");

            }

            
          
          
            if (root.Device.Count == 0)
            {
                GUI.backgroundColor = Color.red;
                
            }
            else
            {
                GUI.backgroundColor = Color.yellow;
            }

            if (GUILayout.Button ( "Save to Json",GUILayout.Height(50),GUILayout.MinWidth(160)))
            {
                string Json_added = JsonUtility.ToJson(root);
                File.WriteAllText(Application.dataPath +"/mr.yilanci/Multiple Screenshot Editor/Json_Device_Save_List.json",Json_added);
            }
          
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            
            if (datapath == "")
            {
                GUI.backgroundColor = Color.red;
            }
            else
            {
                GUI.backgroundColor = Color.green;
            }
            
            
            
            if(GUILayout.Button ( "Show Folder",GUILayout.Height(50),GUILayout.MinWidth(160)))
            {
               
                Application.OpenURL(PlayerPrefs.GetString("location"));

            }
            
            
            if (PlayerPrefs.GetString("savepath")!= null)
            {
                GUI.backgroundColor = Color.yellow;
                
            }
            else
            {
                GUI.backgroundColor = Color.red;
            }
            if (GUILayout.Button ( "Where Save SS",GUILayout.Height(50),GUILayout.MinWidth(160)))
            {
                location = EditorUtility.OpenFolderPanel(location, "", "");
                PlayerPrefs.SetString("location",location);
                
                if (root.Device.Count == 0)
                {
                    guide = "You have to add device resolution ";
                    Debug.Log("You have to add device resolution");
                }
                else 
                {
                    guide = "You have to Play Unity after Click to Take ScreenShot";
                }
               

            }
           
            GUILayout.EndHorizontal();
            
           
           
            if (Time.timeScale == 0)
            {
                GUI.backgroundColor = Color.green;
                
            }
            else
            {
                GUI.backgroundColor = Color.red;
            }
          
            if (GUILayout.Button ( "Time Scale 1 ",GUILayout.Height(50)))
            {
                Time.timeScale = 1;
            }
            
            EditorGUILayout.EndScrollView();
           
          
            async void DelayUseAsync()
            {
                
                if (Path.IsPathRooted(datapath) )
                {
                    for (int i = 0; i < root.Device.Count; i++)
                    {
                        Time.timeScale = 0;
                        Take_SS(root.Device[i].DeviceName, root.Device[i].Height, root.Device[i].Width);
                        await Task.Delay(1250);
                        
                    }
                    PlayerPrefs.SetInt("sayac",PlayerPrefs.GetInt("sayac")+1); 
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
               
               
                var folder = Directory.CreateDirectory(datapath +"/"+PlayerPrefs.GetInt("sayac")); // returns a DirectoryInfo object
               

               
                imageRecorder.OutputFile = Path.Combine(datapath +"/"+PlayerPrefs.GetInt("sayac"), name) + DefaultWildcard.Frame;
                var recorderWindow = EditorWindow.GetWindow<RecorderWindow>();
                recorderWindow.StopRecording();
                recorderWindow.Close();
                
                
            }
        }
    }
}
