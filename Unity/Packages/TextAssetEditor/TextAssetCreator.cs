// (c) 2022 BlindEye Softworks. All rights reserved.

using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEditor;

public class TextAssetCreator : Editor
{
    [MenuItem("Assets/Create/Text Asset/Text File")]
    public static void CreateTextFile() => CreateAsset("NewTextFile.txt");

    [MenuItem("Assets/Create/Text Asset/HTML File")]
    public static void CreateHTMLFile() => CreateAsset("NewHTMLFile.html");

    [MenuItem("Assets/Create/Text Asset/HTM File")]
    public static void CreateHTMFile() => CreateAsset("NewHTMFile.htm");

    [MenuItem("Assets/Create/Text Asset/XML File")]
    public static void CreateXMLFile() => CreateAsset("NewXMLFile.xml");

    [MenuItem("Assets/Create/Text Asset/JSON File")]
    public static void CreateJSONFile() => CreateAsset("NewJSONFile.json");

    [MenuItem("Assets/Create/Text Asset/CSV File")]
    public static void CreateCSVFile() => CreateAsset("NewCSVFile.csv");

    [MenuItem("Assets/Create/Text Asset/YAML File")]
    public static void CreateYAMLFile() => CreateAsset("NewYAMLFile.yaml");

    private static void CreateAsset(string filename)
    {
        /* Ensure that we both create and give focus to a new project browser window if one does
           not exist in the current layout. */
        EditorWindow projectBrowser = EditorWindow.GetWindow(
            Type.GetType("UnityEditor.ProjectBrowser,UnityEditor.dll"), false, "Project", true);

        // Retrieve the users project browser folder selection history currently residing in memory.
        // ReSharper disable once PossibleNullReferenceException
        var folderHistory = (string[])projectBrowser.GetType()
            .GetField("m_LastFolders", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(projectBrowser);

        string projectPath = Application.dataPath,
               relativeSelectionPath = (Selection.activeObject != null) ?
                   AssetDatabase.GetAssetPath(Selection.activeObject) :
                   (folderHistory.Length != 0) ? folderHistory[0] : "Assets",
               /* Since Selection.activeObject emits a UnityEngine.DefaultAsset object which does
                  not expose a specific type ("yet" - per Unity Technologies) we must differentiate
                  between folder and file paths ourselves for sanitization purposes. */
               relativeAssetPath = Directory.Exists(relativeSelectionPath) ?
                   relativeSelectionPath : Path.GetDirectoryName(relativeSelectionPath),
               /* The Application.dataPath property contained in Unity's engine API emits the FQPN
                  of the project folder ("Assets") whereas the AssetDatabase.GetAssetPath method
                  in Unity's editor API returns the RPN of an asset with the project folder as the
                  root. Due to this, as well for both APIs not canonicalizing paths for Windows,
                  platform-agnostic path concatenation may become problematic. Microsot's .NET
                  runtime implementation for Windows starting with Framework 4.6.2 will implicitly
                  defer to the Win32 APIs GetPathFullName function for path normalization, but can
                  also be performed explicitly via the managed Path.GetFullPath method which wraps
                  a call to the native Windows function above. For Windows, this enables us to
                  utilize path specifiers such as '..' when concatenating path segments. */
               uniqueAssetPath = AssetDatabase.GenerateUniqueAssetPath(
                   // ReSharper disable once AssignNullToNotNullAttribute
                   Path.Combine(relativeAssetPath, filename)),
               // Path.Combine incorrectly concatenates path segments when passing path specifiers.
               absoluteAssetPath = Path.GetFullPath(projectPath + Path.DirectorySeparatorChar +
                   ".." + Path.DirectorySeparatorChar + uniqueAssetPath);

        TryCreateAsset(absoluteAssetPath, out bool wasSuccessful);

        if (!wasSuccessful)
            return;

        AssetDatabase.ImportAsset(uniqueAssetPath);
        Selection.activeObject = AssetDatabase
            .LoadAssetAtPath<UnityEngine.Object>(uniqueAssetPath);
    }

    private static void TryCreateAsset(string path, out bool wasSuccessful)
    {
        wasSuccessful = true;

        try
        {
            // Mitigate locking Unity's internal file system watcher in an infinite loop.
            File.Create(path).Dispose();
        }
        catch (Exception e)
        {
            wasSuccessful = false;
            EditorUtility.DisplayDialog("Create Text Asset", e.Message, "OK");
        }
    }
}

[CustomEditor(typeof(TextAsset))]
public class TextAssetEditor : Editor
{
    private const string DateFormat = "dddd, MMMM d, yyyy, h:mm:ss tt",
                         Unknown = "Unknown";

    private const uint SHFILEINFOW_SIZE = 0x2B8, /* Indicates how many bytes of address space should
                       should be allocated for SHFILEINFOW structures. The size of the structure
                       should never exceed 696 bytes for 64-bit processes or 692 bytes for 32-bit
                       processes. */
                       SHGFI_TYPENAME = 0x400, // Indicates that a file's type should be recieved.
                       SHGFI_USEFILEATTRIBUTES = 0x10, /* Indicates that there should be no attempt
                       to access a specified file but instead treat it as if it exists. */
                       FILE_ATTRIBUTE_NORMAL = 0x80; /* Indicates that a file does not have other
                       attributes set. This attribute is only valid when used alone. */

    /* Prior to Windows 10, version 1607, file and directory functions contained in the Win32 API
       define the maximum length for a path as 260 characters. This limitation is also enforced by
       default when targetting the .NET Framework prior to 4.6.2. Starting in Windows 10, version
       1607, said limitations have been removed from select parts of the Win32 API for users that
       opt-in to the feature. However, the parts of the API we are making use of do not support this
       feature. */
    private const int MAX_PATH = 260;

    [DllImport("Shell32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    private static extern IntPtr SHGetFileInfoW(string pszPath, uint dwFileAttributes,
        ref SHFILEINFOW psfi, uint cbFileInfo, uint uFlags);

    [StructLayout(LayoutKind.Sequential, Size = (int)SHFILEINFOW_SIZE, CharSet = CharSet.Unicode)]
    private struct SHFILEINFOW
    {
        public IntPtr hIcon;
        public int iIcon;
        public uint dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
        public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    };

    private bool showContents = true,
                 showProperties = true;

    private string path = Unknown,
                   contents = Unknown,
                   fileType = Unknown,
                   fileLocation = Unknown,
                   fileCreationTime = Unknown,
                   fileLastWriteTime = Unknown,
                   fileLastAccessTime = Unknown,
                   fileIsReadOnly = Unknown;

    private FileInfo fileInfo;

    private void OnEnable()
    {
        path = AssetDatabase.GetAssetPath(target);

        try
        {
            fileInfo = new FileInfo(path);
            contents = File.ReadAllText(path);
            fileInfo.LastAccessTime = DateTime.Now;
        }
        catch (Exception e)
        {
            EditorUtility.DisplayDialog("Open Text Asset", e.Message, "OK");

            // Terminate early in case fileInfo raised the exception.
            return;
        }

        fileType = GetFileType(fileInfo.Extension) + $" ({fileInfo.Extension})";
        fileLocation = fileInfo.DirectoryName;
        fileCreationTime = fileInfo.CreationTime.ToString(DateFormat);
        fileLastWriteTime = fileInfo.LastWriteTime.ToString(DateFormat);
        fileLastAccessTime = fileInfo.LastAccessTime.ToString(DateFormat);
        fileIsReadOnly = fileInfo.IsReadOnly.ToString();
    }

    public override void OnInspectorGUI()
    {
        GUI.enabled = true;
        EditorGUILayout.BeginVertical();
        DrawEditorFoldout();
        DrawPropertiesFoldout();
        EditorGUILayout.EndVertical();

        void DrawEditorFoldout()
        {
            var mode = (fileIsReadOnly != false.ToString()) ? " (Read-only)" : string.Empty;
            showContents = EditorGUILayout.BeginFoldoutHeaderGroup(showContents, $" File Editor{mode}");

            if (showContents)
            {
                EditorGUILayout.BeginVertical("box");

                if (fileIsReadOnly != false.ToString())
                {
                    EditorGUILayout.SelectableLabel(contents, EditorStyles.textArea,
                        GUILayout.MinHeight(300));
                }
                else
                {
                    contents = EditorGUILayout.TextArea(contents, GUILayout.MinHeight(300));

                    if (GUILayout.Button("Save"))
                    {
                        TrySaveAsset(path, out bool wasSuccessful);

                        if (wasSuccessful)
                        {
                            EditorWindow.focusedWindow.ShowNotification(
                                new GUIContent() { text = "File Saved" }, .25);

                            OnEnable();
                        }
                    }
                }

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        void DrawPropertiesFoldout()
        {
            showProperties = EditorGUILayout.BeginFoldoutHeaderGroup(showProperties, " File Properties");

            if (showProperties)
            {
                GUILayout.BeginVertical("box");
                EditorGUIUtility.fieldWidth = 180;

                GUILayout.BeginHorizontal("box");
                GUILayout.Label("File type:");
                GUILayout.FlexibleSpace();
                EditorGUILayout.SelectableLabel(fileType, EditorStyles.textField,
                    GUILayout.Height(EditorGUIUtility.singleLineHeight));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal("box");
                GUILayout.Label("Location:");
                GUILayout.FlexibleSpace();
                EditorGUILayout.SelectableLabel(fileLocation, EditorStyles.textField,
                    GUILayout.Height(EditorGUIUtility.singleLineHeight));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal("box");
                GUILayout.Label("Created:");
                GUILayout.FlexibleSpace();
                EditorGUILayout.SelectableLabel(fileCreationTime, EditorStyles.textField,
                    GUILayout.Height(EditorGUIUtility.singleLineHeight));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal("box");
                GUILayout.Label("Modified:");
                GUILayout.FlexibleSpace();
                EditorGUILayout.SelectableLabel(fileLastWriteTime, EditorStyles.textField,
                    GUILayout.Height(EditorGUIUtility.singleLineHeight));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal("box");
                GUILayout.Label("Accessed:");
                GUILayout.FlexibleSpace();
                EditorGUILayout.SelectableLabel(fileLastAccessTime, EditorStyles.textField,
                    GUILayout.Height(EditorGUIUtility.singleLineHeight));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal("box");
                GUILayout.Label("Read-only:");
                GUILayout.FlexibleSpace();
                EditorGUILayout.SelectableLabel(fileIsReadOnly, EditorStyles.textField,
                    GUILayout.Height(EditorGUIUtility.singleLineHeight));
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }

    // ReSharper disable once ParameterHidesMember
    private void TrySaveAsset(string path, out bool wasSuccessful)
    {
        wasSuccessful = true;

        try
        {
            File.WriteAllText(path, contents);
        }
        catch (Exception e)
        {
            wasSuccessful = false;
            EditorUtility.DisplayDialog("Save Text Asset", e.Message, "OK");
        }
    }

    private string GetFileType(string fileExtension)
    {
        var shellFileInfo = new SHFILEINFOW();

        /* Calling into the SHGetFileInfo function with the SHGFI_USEFILEATTRIBUTES flag passed in
           the uFlags parameter enables invalid file names to be passed in pszPath. The function
           will proceed as if the file exists with the specified name and with the file
           attributes passed in dwFileAttributes. This enables us to obtain information about a
           file type by passing just the extension in pszPath and FILE_ATTRIBUTE_NORMAL in
           dwFileAttributes. */
        if (SHGetFileInfoW(fileExtension, FILE_ATTRIBUTE_NORMAL, ref shellFileInfo, SHFILEINFOW_SIZE,
            SHGFI_TYPENAME | SHGFI_USEFILEATTRIBUTES) == IntPtr.Zero)
            return Unknown;

        return shellFileInfo.szTypeName;
    }
}
