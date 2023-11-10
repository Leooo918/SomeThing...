using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public class DialogGenerator : EditorWindow
{
    private DialogGraphView graphView;
    private InspectorView inspectorView;

    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("Dialog/DialogGenerater")]
    public static void ShowExample()
    {
        DialogGenerator wnd = GetWindow<DialogGenerator>();
        wnd.titleContent = new GUIContent("DialogGenerator");
    }

    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceID, int line)
    {
        if (Selection.activeObject is DialogSO)
        {
            ShowExample();
            return true;
        }
        return false;
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        m_VisualTreeAsset.CloneTree(root);

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/DialogGenerator.uss");
        root.styleSheets.Add(styleSheet);

        graphView = root.Q<DialogGraphView>();
        inspectorView = root.Q<InspectorView>();
        graphView.OnNodeSeleted = OnNodeSelectionChanged;

        OnSelectionChange();
    }

    private void OnSelectionChange()
    {
        DialogSO dialog = Selection.activeObject as DialogSO;
        if (dialog)
        {
            graphView.ParpurateView(dialog);
        }
    }

    private void OnNodeSelectionChanged(ScriptView script)
    {
        inspectorView.UpdateSelection(script);
    }
}
