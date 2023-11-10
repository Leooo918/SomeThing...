using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine.UIElements;
using System.Linq;

public class DialogGraphView : GraphView
{
    public Action<ScriptView> OnNodeSeleted;
    public new class UxmlFactory : UxmlFactory<DialogGraphView, GraphView.UxmlTraits> { }

    private DialogSO dialog;

    public DialogGraphView()
    {
        Insert(0, new GridBackground());

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/DialogGenerator.uss");
        styleSheets.Add(styleSheet);

        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
    }

    private ScriptView FindScriptView(ScriptSO script)
    {
        return GetNodeByGuid(script.guid) as ScriptView;
    }

    internal void ParpurateView(DialogSO dialog)
    {
        this.dialog = dialog;

        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;

        //Create Node View
        dialog.scripts.ForEach(n =>
        {
            if (dialog.scripts[0].guid == n.guid)
            {
                CreateNodeView(n, true);
            }
            else
            {
                CreateNodeView(n, false);
            }
        });

        //Create Edge
        dialog.scripts.ForEach(n =>
        {
            var children = dialog.GetConnectedScripts(n);

            children.ForEach(c =>
            {
                if (c != null)
                {
                    ScriptView parentView = FindScriptView(n);
                    ScriptView childView = FindScriptView(c);

                    if (parentView.output != null)
                    {
                        Edge edge = parentView.output.ConnectTo(childView.input);
                        AddElement(edge);
                    }

                    for (int i = 0; i < parentView.outputs.Count; i++)
                    {
                        BranchSO branch = parentView.script as BranchSO;        //그니까 브랜치인 경우
                        OptionSO option = parentView.script as OptionSO;        //옵션인 경우

                        if (branch)     //브랜치 라면
                        {
                            if (branch.nextScriptOnFalse != null)
                            {
                                if (branch.nextScriptOnFalse.guid == childView.script.guid && i == 1)
                                {
                                    Edge edge = parentView.outputs[i].ConnectTo(childView.input);
                                    AddElement(edge);
                                }
                            }
                            if (branch.nextScriptOnTrue != null)
                            {
                                if (branch.nextScriptOnTrue.guid == childView.script.guid && i == 0)
                                {
                                    Edge edge = parentView.outputs[i].ConnectTo(childView.input);
                                    AddElement(edge);
                                }
                            }

                        }
                        else if (option)
                        {
                            if (option.nextScriptsByOption[i] != null)
                            {
                                if (option.nextScriptsByOption[i].guid == childView.script.guid)
                                {
                                    Edge edge = parentView.outputs[i].ConnectTo(childView.input);
                                    AddElement(edge);
                                }
                            }
                        }
                    }
                }


            });
        });
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(endPort =>
            endPort.direction != startPort.direction &&
            endPort.node != startPort.node).ToList();
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        if (graphViewChange.elementsToRemove != null)
        {
            graphViewChange.elementsToRemove.ForEach(elem =>
            {
                ScriptView scriptView = elem as ScriptView;
                if (scriptView != null)
                {
                    dialog.DeleteScript(scriptView.script);
                }

                Edge edge = elem as Edge;
                if (edge != null)
                {
                    ScriptView parentView = edge.output.node as ScriptView;
                    ScriptView childView = edge.input.node as ScriptView;
                    dialog.RemoveNextScript(parentView.script, childView.script, edge.output);
                }
            });
        }

        if (graphViewChange.edgesToCreate != null)
        {
            graphViewChange.edgesToCreate.ForEach(edge =>
            {
                ScriptView parentView = edge.output.node as ScriptView;
                ScriptView childView = edge.input.node as ScriptView;
                dialog.AddNextScript(parentView.script, childView.script, edge.output);
            });
        }

        return graphViewChange;
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        //base.BuildContextualMenu(evt);

        var types = TypeCache.GetTypesDerivedFrom<ScriptSO>();
        foreach (var type in types)
        {
            evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateScript(type));
        }

    }

    private void CreateScript(System.Type type)
    {
        ScriptSO script = dialog.CreateScript(type);

        if (dialog.scripts[0].guid == script.guid)
        {
            CreateNodeView(script, true);
        }

        CreateNodeView(script, false);
    }

    private void CreateNodeView(ScriptSO script, bool isFirstNode = false)
    {
        NormalScriptSO normal = script as NormalScriptSO;
        OptionSO option = script as OptionSO;
        BranchSO branch = script as BranchSO;
        ImageSO image = script as ImageSO;

        if (normal)
        {
            script.scriptType = ScriptType.NORMAL;
        }
        else if (option)
        {
            script.scriptType = ScriptType.OPTION;
        }
        else if (branch)
        {
            script.scriptType = ScriptType.BRANCH;
        }
        else if (image)
        {
            script.scriptType = ScriptType.IMAGE;
        }

        ScriptView scriptView = new ScriptView(script, isFirstNode);

        scriptView.OnNodeSeleted = OnNodeSeleted;
        AddElement(scriptView);
    }
}
