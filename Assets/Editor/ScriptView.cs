using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;

public class ScriptView : Node
{
    public Action<ScriptView> OnNodeSeleted;
    public ScriptSO script;
    public Port input;
    public Port output;
    public TextField nameInput;
    public List<Port> outputs = new List<Port>();
    private bool isFirstNode = false;

    public ScriptView(ScriptSO script, bool isFirstNode)
    {
        this.script = script;
        this.title = script.name;
        this.viewDataKey = script.guid;
        this.isFirstNode = isFirstNode;

        style.left = script.position.x;
        style.top = script.position.y;

        CreateInputPorts();
        CreateOutputPorts();

        this.Add(nameInput);
    }

    private void CreateInputPorts()
    {
        if (isFirstNode) return;

        if (script is NormalScriptSO)
        {
            input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
        }
        else if (script is OptionSO)
        {
            input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
        }
        else if (script is BranchSO)
        {
            input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
        }
        else if (script is ImageSO)
        {
            input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
        }

        if (input != null)
        {
            input.portName = "";
            inputContainer.Add(input);
        }
    }

    private void CreateOutputPorts()
    {
        if (script is NormalScriptSO)
        {
            output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            output.portName = "NextScripts";
            outputContainer.Add(output);
        }
        else if (script is OptionSO)
        {
            for (int i = 0; i < 3; i++)
            {
                outputs.Add(InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool)));
                outputs[i].portName = $"Option-{i + 1}";
                outputContainer.Add(outputs[i]);
            }
        }
        else if (script is BranchSO)
        {
            outputs.Add(InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool)));
            outputs[0].portName = $"True";
            outputContainer.Add(outputs[0]);

            outputs.Add(InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool)));
            outputs[1].portName = $"False";
            outputContainer.Add(outputs[1]);
        }
        else if (script is ImageSO)
        {
            output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            output.portName = "NextScript";
            outputContainer.Add(output);
        }
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        script.position.x = newPos.xMin;
        script.position.y = newPos.yMin;
    }

    public override void OnSelected()
    {
        base.OnSelected();
        OnNodeSeleted?.Invoke(this);
    }
}
