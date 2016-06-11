using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections;
using BHaviourTree;
using System.Collections.Generic;
using System.IO;
using LitJson;
using System.Reflection;


public class BehavTreeMagager {

    private static BehavTreeMagager instance;
    public static BehavTreeMagager Instance { 
        get{
            if (instance == null) { 
                instance = new BehavTreeMagager();
            }
            return instance;
        }
    }
    //-------------------------------------------
	private const string behavTreeScriptPath = "/Samples/Btree/Scripts/BehaviourTreeImp";

    private Dictionary<string, List<Type>> typeMap = new Dictionary<string, List<Type>>();
    private string[] typeNames;
    
    private BehavTreeMagager() {
        typeNames = new string[]{ "Composite", "Decorator", "Condition", "Action" };
        
        List<string> allFiles = GetAllFiles(Application.dataPath + behavTreeScriptPath);
        typeMap["Composite"] = GetAllSubTypes(typeof(BCompositeNode), allFiles);
        typeMap["Decorator"] = GetAllSubTypes(typeof(BDecoratorNode), allFiles);
        typeMap["Condition"] = GetAllSubTypes(typeof(BConditionNode), allFiles);
        typeMap["Action"] = GetAllSubTypes(typeof(BActionNode), allFiles);        
    }
    List<Type> GetAllSubTypes(Type type, List<string> files) {
        List<Type> types = new List<Type>();
        for (int i = 0; i < files.Count; i++) { 
            string className = files[i].Split('.')[0];  // 注：文件名必须与类名相同！
            Type t = Type.GetType(className);
            if (t != null && t.IsSubclassOf(type)) { 
                types.Add(t);
            }
        }
        return types;
    }
    List<string> GetAllFiles(string directory) { 
        DirectoryInfo dirInfo = new DirectoryInfo(directory);

        List<string> files = new List<string>();

        FileInfo[] allFiles = dirInfo.GetFiles("*.cs");
        for (int i = 0; i < allFiles.Length; i++) { 
            files.Add(allFiles[i].Name);
        }

        DirectoryInfo[] allDirectories = dirInfo.GetDirectories();
        for (int i = 0; i < allDirectories.Length; i++) { 
            List<string> dfiles = GetAllFiles(allDirectories[i].FullName);
            files.AddRange(dfiles);
        }
        return files;
    }
    public string[] GetTypenames() {
        return typeNames;
    }
    public List<Type> GetTypesByTypename(string typename) {
        if (typeMap.ContainsKey(typename)) { 
            return typeMap[typename];
        }
        return new List<Type>();
    }

    //---------------------- json ----------------------------
    public BehavTree LoadTree() { 
        string filePath = UnityEditor.EditorUtility.OpenFilePanel("Load Behaviour tree", Application.dataPath, "json");
        BehavTree tree = ReadTreeFromJson(filePath);
        return tree;
    }
    public BehavTree LoadTree(string filePath) { 
        BehavTree tree = ReadTreeFromJson(filePath);
        return tree;
    }
    public void SaveTree(BehavTree tree) { 
         if (tree == null || tree.Root == null) { 
            return;
        }
        string filePath = UnityEditor.EditorUtility.SaveFilePanel("Save Behaviour tree", Application.dataPath,"test", "json");
        if (string.IsNullOrEmpty(filePath)) { 
            return;
        }
        JsonData json = NodeToJson(tree.Root);
        string jsonStr = json.ToJson();
        File.WriteAllText(filePath, jsonStr);
        UnityEditor.EditorUtility.DisplayDialog("Tip", "save succeed!", "OK");
    }

    BehavTree ReadTreeFromJson(string path) { 
        string jsonText = File.ReadAllText(path);
        JsonData json = JsonMapper.ToObject(jsonText);

        BNode root = ReadJsonNode(json);
		FileInfo file = new FileInfo (path);
        BehavTree tree = new BehavTree(file.Name);
        tree.SetRoot(root);
        return tree;
    }
    BNode ReadJsonNode(JsonData json) { 
        string typeName = json["typeFullName"].ToString();
        Type type = Type.GetType(typeName);

        JsonData jsonNode = json["node"];
        BNode node = Activator.CreateInstance(type) as BNode;
        FieldInfo[] fields = type.GetFields();
        for (int i = 0; i < fields.Length; i++) {
            FieldInfo field = fields[i];
            string strVal = jsonNode[field.Name].ToString();
            object val = null;
            if (field.FieldType == typeof(int)) { 
                val = int.Parse(strVal);
            }else if (field.FieldType == typeof(float)) { 
                val = float.Parse(strVal);
            }else if (field.FieldType == typeof(bool)) { 
                val = bool.Parse(strVal);
            }else if (field.FieldType == typeof(string)) { 
                val = strVal;
            }
            field.SetValue(node, val);
        }
        for (int i = 0; i < json["children"].Count; i++) { 
            JsonData childJson = json["children"][i];
            BNode childNode = ReadJsonNode(childJson);
            node.AddNode(childNode);
        }
        return node;
    }
    JsonData NodeToJson(BNode node) { 
        JsonData json = new JsonData();
        json["typeFullName"] = node.GetType().FullName;
        json["typeName"] = node.GetType().Name;
        json["node"] = new JsonData();
        json["node"].SetJsonType(JsonType.Object);

        Type type = node.GetType();
        FieldInfo[] fields = type.GetFields();
        for (int i = 0; i < fields.Length; i++) { 
            FieldInfo field = fields[i];
            object value = field.GetValue(node);
            json["node"][field.Name] = value == null ? "" : value.ToString();
        }
        json["children"] = new JsonData();
        json["children"].SetJsonType(JsonType.Array);
        for(int i =0; i < node.ChildCount; i++){
            BNode child = node.GetChild(i);
            JsonData childJson = NodeToJson(child);
            json["children"].Add(childJson);
        }
        return json;
    }
}

