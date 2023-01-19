using System.IO;
using UnityEngine;
using System.Threading.Tasks;
using UnityEditor.Recorder.Input;
using UnityEditor.Recorder;
using UnityEditor;

namespace MultipleScreenshot.Editor
{
    using Enumerators;
    public class MultipleScreenshotEditor : EditorWindow
    {
        public DeviceList Root = new DeviceList();
        private SerializedObject _soData;
        private string  _localPath = "ScreenShots/";
        private static string _guide = "Guide";
        private GUIStyle _guiStyle = new GUIStyle();
        private Vector2 _scroll = Vector2.zero;

        private int _deviceCount;
        private int _spaceA = 300;
        private static string _location;
        private static string _dataPath;
        private const string _counterHash = "counter";
        private const string _locationHash = "location";
        
        [MenuItem("Window/mr.yilanci/Multiple Screenshot Editor")]
        public static void ShowWindow()
        {
            MultipleScreenshotEditor window = (MultipleScreenshotEditor) GetWindow(typeof(MultipleScreenshotEditor), false, "Multiple Screenshot Editor");
            window.Show();
        }
        
      
        
       

         private void OnGUI()
        {
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            _dataPath = PlayerPrefs.GetString(_locationHash);
           
            GUILayout.BeginVertical();
            _guiStyle.fontSize = 11;
            _guiStyle.normal.textColor = Color.white;
            EditorGUILayout.LabelField(("Outfile Folder: "+_dataPath ),_guiStyle,GUILayout.Height(15));
            GUILayout.Space(2);
            EditorStyles.helpBox.fontSize = 12;
            EditorStyles.helpBox.normal.textColor=Color.yellow;
            EditorStyles.helpBox.fontStyle = FontStyle.Bold;
            EditorGUILayout.LabelField(_guide, EditorStyles.helpBox,GUILayout.Height(30));
            GUILayout.Space(2);
            EditorGUILayout.LabelField("Number of the output folder : "+PlayerPrefs.GetInt(_counterHash),_guiStyle,GUILayout.Height(15));
            GUILayout.EndVertical();
            
            if (_dataPath == "")
            {
                _guide = "You have to select folder for save Screenshot";

            }
            else if (Root.Device.Count == 0)
            {
                _guide = "You have to add device resolution ";

            }
            EditorGUILayout.Space();


            EditorGUILayout.BeginVertical();
            ScriptableObject target = this;
            _soData = new SerializedObject(target);
            SerializedProperty stringsProperty = _soData.FindProperty("Root");
            EditorGUILayout.PropertyField(stringsProperty, true); // True means show children
            _soData.ApplyModifiedProperties();
            EditorGUILayout.EndVertical();
            
            
            GUILayout.BeginHorizontal();
            
            if (EditorApplication.isPlaying != true &  Root.Device.Count ==0 )
            {
                GUI.backgroundColor = Color.red;
            }
            if (EditorApplication.isPlaying != true & (_dataPath !=""|| Root.Device.Count > 0) )
            {
                GUI.backgroundColor = Color.green;
            }
            else
            {
                GUI.backgroundColor = Color.yellow;
                if (_dataPath == "")
                {
                    Debug.Log(_dataPath +"You have to select folder for save Screenshot");
                    _guide = "You have to select folder for save Screenshot";
                }
            }
            
            if (GUILayout.Button( "Play Unity",GUILayout.Height(50),GUILayout.MinWidth(160)))
            {
                if (EditorApplication.isPlaying != true)
                    EditorApplication.isPlaying = true;
            }

            GUI.backgroundColor = EditorApplication.isPlaying != true ? Color.red : Color.green;
            if (GUILayout.Button( "Take Screenshot",GUILayout.Height(50),GUILayout.MinWidth(160)))
            {
            
                if (EditorApplication.isPlaying & Root.Device.Count !=0)
                {
                    
                    _guide = "Editor Working";
                    DelayUseAsync();
                    
                   
                }
                else if (EditorApplication.isPlaying & Root.Device.Count ==0)
                {
                    _guide = "You have to add device resolution ";
                }
                else
                {
                    _guide = "You have to Play Unity";
                }

            }
           
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
       
            GUI.backgroundColor = Color.yellow;

            if (GUILayout.Button ("Load Json",GUILayout.Height(50),GUILayout.MinWidth(160)))
            {
                
                TextAsset json_yazi = new TextAsset(File.ReadAllText(Application.dataPath +"/mr.yilanci/Multiple Screenshot Editor/Json_Device_Save_List.json"));
                Root = JsonUtility.FromJson<DeviceList>(json_yazi.text);
                
                _guide = "You have to Play Unity after Click to Take ScreenShot";
                Debug.Log("Loaded Device List From Json ");

            }

        
            GUI.backgroundColor = Root.Device.Count == 0 ? Color.red : Color.yellow;
            if (GUILayout.Button ( "Save to Json",GUILayout.Height(50),GUILayout.MinWidth(160)))
            {
                string Json_added = JsonUtility.ToJson(Root);
                File.WriteAllText(Application.dataPath +"/mr.yilanci/Multiple Screenshot Editor/Json_Device_Save_List.json",Json_added);
            }
          
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            
            GUI.backgroundColor = _dataPath == "" ? Color.red : Color.green;
       
            if(GUILayout.Button ( "Show Folder",GUILayout.Height(50),GUILayout.MinWidth(160)))
            {

                ShowFolder();

            }


            GUI.backgroundColor = PlayerPrefs.HasKey(_locationHash) ? Color.yellow : Color.green;

            if (GUILayout.Button ( "Where Save SS",GUILayout.Height(50),GUILayout.MinWidth(160)))
            {
                _location = EditorUtility.OpenFolderPanel(_location, "", "");
                PlayerPrefs.SetString(_locationHash,_location);
                
                if (Root.Device.Count == 0)
                {
                    _guide = "You have to add device resolution ";
                    Debug.Log("You have to add device resolution");
                }
                else 
                {
                    _guide = "You have to Play Unity after Click to Take ScreenShot";
                }
               

            }
            GUILayout.EndHorizontal();
      
            
            GUI.backgroundColor = Time.timeScale == 0 ? Color.green : Color.red;
          
            if (GUILayout.Button ( "Time Scale 1 ",GUILayout.Height(50)))
            {
                Time.timeScale = 1;
            }
            
            EditorGUILayout.EndScrollView();
            
        }

         private void ShowFolder()
         {
             if (Application.platform == RuntimePlatform.WindowsEditor)
             {
                 Application.OpenURL(PlayerPrefs.GetString(_locationHash));
             }
               
               
               
             else if (Application.platform == RuntimePlatform.OSXEditor)
             {
                 System.Diagnostics.Process.Start("open", (PlayerPrefs.GetString(_locationHash)));
             }

         }

         private async void DelayUseAsync()
         {
             
            
            
             if (Path.IsPathRooted(_dataPath) )
             {
                 float counter = 0;
                 float DevicesCounts = Root.Device.Count;
                     
                     foreach (var unit in Root.Device)
                     { 
                         
                         EditorUtility.DisplayProgressBar("MultipleScreenshotEditor.cs", "Editor Working - Device in Progress: ''"+unit.DeviceName+"''" , counter /DevicesCounts);
                         Time.timeScale = 0;
                         Take_SS(unit.DeviceName, unit.Height, unit.Width);
                         await Task.Delay(1250);
                         counter++;
                     }
                 _guide = "Completed";
                 ShowFolder();
                 
                 PlayerPrefs.SetInt(_counterHash,PlayerPrefs.GetInt(_counterHash)+1);

                 
                 
                 if (PlayerPrefs.GetInt("Link_Opened") != 1)
                 {
                     PopUp.Init(); 
                 }
                
             }

             else
             {
                 _guide = "!!! Write Path of outfile";
             }
             EditorUtility.ClearProgressBar();
         }
         
         private static void Take_SS(string name, int Height, int width)
         {
             var controllerSettings = CreateInstance<RecorderControllerSettings>();
             var m_RecorderController = new RecorderController(controllerSettings);

             // Image sequence
             var imageRecorder = CreateInstance<ImageRecorderSettings>();
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
               
               
             Directory.CreateDirectory(_dataPath +"/"+PlayerPrefs.GetInt(_counterHash)); // returns a DirectoryInfo object
             
             imageRecorder.OutputFile = Path.Combine(_dataPath +"/"+PlayerPrefs.GetInt(_counterHash), name) + DefaultWildcard.Frame;
             var recorderWindow = GetWindow<RecorderWindow>();
             recorderWindow.StopRecording();
             recorderWindow.Close();
         }
    }
    public class PopUp : EditorWindow
    {

        private GUIStyle _guiStyle2 = new GUIStyle();
        private GUIStyle _guiStyle3 = new GUIStyle();
        
        [MenuItem("Window/mr.yilanci/popup #t")]
        public static void Init()
        {
            PopUp window = ScriptableObject.CreateInstance<PopUp>();
            window.minSize = new Vector2(600, 335);
            window.maxSize = new Vector2(600, 335);
            window.maximized = false;
            window.Show();
            
        }
        
      
        
        void OnGUI()
        {
         
            _guiStyle2.fontSize = 14;
            _guiStyle2.normal.textColor = Color.white;
            _guiStyle2.fontStyle = FontStyle.Bold;
            
            _guiStyle3.fontSize = 11;
            _guiStyle3.normal.textColor = Color.white;
            _guiStyle3.fontStyle = FontStyle.Bold;
 
            

            EditorGUI.LabelField(new Rect(5, 2 , position.width, 20),"Multiple Screenshot Editor - Rate Pop-Up ",_guiStyle3 );
            
            EditorGUI.LabelField(new Rect(280, 12 , position.width, 20),"Please",_guiStyle2);
        
            EditorGUI.LabelField(new Rect(120, 32 , position.width, 20),"Give Rate for Multiple Screenshot Editor on Asset Store",_guiStyle2);
            
            Texture banner = (Texture)AssetDatabase.LoadAssetAtPath("Assets/mr.yilanci/Multiple Screenshot Editor/Logo/ss_icon.png", typeof(Texture));
            
            
            EditorGUI.DrawPreviewTexture(new Rect(239, 58 , 128 , 128),banner); 
           
            
            
            if (GUI.Button(new Rect(50, 202 , 600 , 15),"https://assetstore.unity.com/packages/tools/utilities/multiple-screenshot-editor-235183",EditorStyles.linkLabel))
            {
                Application.OpenURL("https://assetstore.unity.com/packages/tools/utilities/multiple-screenshot-editor-235183");
                PlayerPrefs.SetInt("Link_Opened",1);
                this.Close();
            }
            
            
            GUILayout.Space(235f);
            
            
            if (GUILayout.Button("Yes,I Will Give Rate - Open The Asset's Page",GUILayout.Height(40)))
            {
                Application.OpenURL("https://assetstore.unity.com/packages/tools/utilities/multiple-screenshot-editor-235183");
                PlayerPrefs.SetInt("Link_Opened",1);
                this.Close();
            }
            
            if (GUILayout.Button("No,Close Window",GUILayout.Height(40))) this.Close();
            
            
        }
    }
    
}
