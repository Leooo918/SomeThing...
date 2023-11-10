using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogGenerater : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("Dialog/DialogGeneraterr")]
    public static void ShowExample()
    {
        DialogGenerater wnd = GetWindow<DialogGenerater>();
        wnd.titleContent = new GUIContent("DialogGenerater");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        m_VisualTreeAsset.CloneTree(root);

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/DialogGenerator.uss");
        root.styleSheets.Add(styleSheet);
    }
}
