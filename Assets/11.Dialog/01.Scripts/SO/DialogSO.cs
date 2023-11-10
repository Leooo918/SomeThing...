 using System.Linq;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine;



[CreateAssetMenu(menuName = "SO/Dialog")]
public class DialogSO : ScriptableObject
{
    public List<ScriptSO> scripts = new List<ScriptSO>();

    public ScriptSO CreateScript(System.Type type)
    {
        ScriptSO script = ScriptableObject.CreateInstance(type) as ScriptSO;
        script.name = type.Name;
        script.guid = GUID.Generate().ToString();
        scripts.Add(script);

        AssetDatabase.AddObjectToAsset(script, this);
        AssetDatabase.SaveAssets();
        return script;
    }

    public void DeleteScript(ScriptSO script)
    {
        scripts.Remove(script);
        AssetDatabase.RemoveObjectFromAsset(script);
        AssetDatabase.SaveAssets();
    }

    public void AddNextScript(ScriptSO parent, ScriptSO child, Port outputPort)
    {
        NormalScriptSO normal = parent as NormalScriptSO;
        if (normal)
        {
            normal.nextSO = child;
        }
        OptionSO option = parent as OptionSO;
        if (option)
        {
            switch (outputPort.portName)
            {
                case "Option-1":
                    option.nextScriptsByOption[0] = child;
                    break;
                case "Option-2":
                    option.nextScriptsByOption[1] = child;
                    break;
                case "Option-3":
                    option.nextScriptsByOption[2] = child;
                    break;
            }
        }
        BranchSO branch = parent as BranchSO;
        if (branch)
        {
            switch (outputPort.portName)
            {
                case "True":
                    branch.nextScriptOnTrue = child;
                    break;
                case "False":
                    branch.nextScriptOnFalse = child;
                    break;
            }
        }
        ImageSO image = parent as ImageSO;
        if (image){
            image.nextScript = child;
        }
    }

    public void RemoveNextScript(ScriptSO parent, ScriptSO child, Port outputPort)
    {
        NormalScriptSO normal = parent as NormalScriptSO;
        if (normal)
        {
            normal.nextSO = null;
        }
        OptionSO option = parent as OptionSO;
        if (option)
        {
            switch (outputPort.portName)
            {
                case "Option-1":
                    option.nextScriptsByOption[0] = null;
                    break;
                case "Option-2":
                    option.nextScriptsByOption[1] = null;
                    break;
                case "Option-3":
                    option.nextScriptsByOption[2] = null;
                    break;
            }
        }
        BranchSO branch = parent as BranchSO;
        if (branch)
        {
            switch (outputPort.portName)
            {
                case "True":
                    branch.nextScriptOnTrue = null;
                    break;
                case "False":
                    branch.nextScriptOnFalse = null;
                    break;
            }
        }
        ImageSO image = parent as ImageSO;
        if (image)
        {
            image.nextScript = null;
        }
    }

    public List<ScriptSO> GetConnectedScripts(ScriptSO parent)
    {
        List<ScriptSO> children = new List<ScriptSO>();

        NormalScriptSO normal = parent as NormalScriptSO;
        if (normal && normal.nextSO != null)
        {
            children.Add(normal.nextSO);
        }
        OptionSO option = parent as OptionSO;
        if (option && option.nextScriptsByOption.Length > 0)
        {
            children = option.nextScriptsByOption.ToList();
        }
        BranchSO branch = parent as BranchSO;
        if (branch)
        {
            if(branch.nextScriptOnTrue != null) {
                children.Add(branch.nextScriptOnTrue);
            }
            if(branch.nextScriptOnFalse != null) {
                children.Add(branch.nextScriptOnFalse);
            }
        }
        ImageSO image = parent as ImageSO;
        if (image && image.nextScript != null)
        {
            children.Add(image.nextScript);
        }

        return children;
    }
}

