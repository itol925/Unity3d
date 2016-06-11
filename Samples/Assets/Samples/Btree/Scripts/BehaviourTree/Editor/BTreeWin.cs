using UnityEditor;
using UnityEngine;
using BHaviourTree;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;

#if UNITY_EDITOR

public class BTreeWin : EditorWindow {

    public static BTreeWin Instance = null;
    [@MenuItem("BehaviourTree/Editor")]
    static void initWin() { 
        Instance = (BTreeWin)GetWindow<BTreeWin>(); 
		Instance.position = new Rect (300, 200, 800, 400);
    }
    private BTreeWin() { }

    private float ROW_HEIGHT = 20;
    private float COL_HEIGHT = 20;
    private int GUI_WIDTH = 240;
    private Vector2 m_scrollPos = new Vector2();
    private int currentX;
    private int currentY;
    private int lastRowHeight;

    //private int m_curTypeIndex = 0;
    //private int m_curClassnameIndex = 0;

    private BehavTree m_curTree;
    private int m_selectRow = -1;
    void OnGUI() { 
        m_scrollPos = GUI.BeginScrollView(new Rect(0, 0, position.width - 240, position.height), m_scrollPos, new Rect(0, 0, maxSize.x, maxSize.y));
        DrawBackground();
        if (m_curTree != null) { 
            DrawTree();
        }
        OnEvent();
        GUI.EndScrollView();
        DrawControls();
    }
    void DrawBackground() { 
        Texture2D tex1 = new Texture2D(1, 1);
        tex1.SetPixel(0, 0, Color.black);
        tex1.Apply();
        Texture2D tex2 = new Texture2D(1, 1);
        tex2.SetPixel(0, 0, Color.gray);
        tex2.Apply();
        for (int i = 0; i < 1000; i++) {
            if (i == m_selectRow && m_treeMap.ContainsKey(m_selectRow)) {
                Texture2D tex3 = new Texture2D(1, 1);
                tex3.SetPixel(0, 0, Color.blue);
                tex3.Apply();
                GUI.DrawTexture(new Rect(0, i * ROW_HEIGHT, maxSize.x, ROW_HEIGHT), tex3);
            } else {
                if (i % 2 == 0) {
                    GUI.DrawTexture(new Rect(0, i * ROW_HEIGHT, maxSize.x, ROW_HEIGHT), tex1);
                } else { 
                    GUI.DrawTexture(new Rect(0, i * ROW_HEIGHT, maxSize.x, ROW_HEIGHT), tex2);
                }
            }
        }
    }
    //----------------- tree render ------------------------
    private IDictionary<int, BNode> m_treeMap = new Dictionary<int, BNode>();
    void CreateTree() { 
		m_treeMap.Clear ();
		m_fieldValueCache.Clear();
        m_curTree = new BehavTree("untitled");
    }
    
    void DrawTree() { 
        BNode root = m_curTree.Root;
        if (root == null) { 
            return;
        }
        int currow = 0;
        int curcol = 0;
		m_treeMap.Clear ();
        DrawNode(root, ref currow, ref curcol);
    }
    void DrawNode(BNode node, ref int curRow, ref int curCol) { 
        Vector3 start = new Vector3(curCol * COL_HEIGHT + COL_HEIGHT/2, curRow * ROW_HEIGHT + ROW_HEIGHT/2 + 5);

        GUI.Label(new Rect(curCol * COL_HEIGHT, curRow * ROW_HEIGHT, 200, ROW_HEIGHT), node.Name);
        m_treeMap[curRow] = node;

        curRow++;
        curCol++;
        for (int i = 0; i < node.ChildCount; i++) { 
            int row = curRow;
            int col = curCol;

            BNode child = node.GetChild(i);
            DrawNode(child, ref curRow, ref curCol);

            Vector3 middle = new Vector3(col * COL_HEIGHT - COL_HEIGHT/2, row * ROW_HEIGHT + ROW_HEIGHT/2);
            Vector3 end = new Vector3(col * COL_HEIGHT, row * ROW_HEIGHT + ROW_HEIGHT/2);
            Handles.color = Color.red;
            Handles.DrawPolyLine(start, middle, end);
        }
        curCol--;
    }
    //----------------- control ----------------------------
    void DrawControls() { 
        GUI.BeginGroup(new Rect(position.width - GUI_WIDTH, 0, 300, 1000));
		
		currentX = 0;
		currentY = 0;

		if (m_curTree == null) {
			AddLabel (GUI_WIDTH, 20, "please Create or Load a tree.");
			NewLine ();
		} else {
			Instance.title = m_curTree.Name;
			if(m_curTree.Root == null){
				AddLabel(GUI_WIDTH, 20, "try click the right mouse button.");
				NewLine();
			}
		}

        AddButton(100, 40, "load tree", delegate(){
			m_treeMap.Clear();
			m_fieldValueCache.Clear();
            m_curTree = BehavTreeMagager.Instance.LoadTree();
        });
        AddButton(100, 40, "create Tree", delegate(){
            CreateTree();
        });
        NewLine();
        AddButton(200, 40, "save Tree", delegate(){
            BehavTreeMagager.Instance.SaveTree(m_curTree);
        });
        NewLine();
		if (m_treeMap.ContainsKey (m_selectRow)) {
			AddLabel (GUI_WIDTH, 20, "-------- Node info :---------");
			NewLine ();
			DrawSelectNodeInfo ();
		}
        GUI.EndGroup();
    }
    void AddButton(int width, int height, string text, Action onclick = null) { 
        if (GUI.Button(new Rect(currentX, currentY, width, height), text)) {
            if (onclick != null) { 
                onclick();
            }
        }
        currentX += width + 5;
        lastRowHeight = height + 5;
    }
    void AddLabel(int width, int height, string text) { 
        GUI.Label(new Rect(currentX, currentY, width, height), text);
        currentX += width + 5;
        lastRowHeight = height + 5;
    }
    void AddPopup(int width, int height, string[] content, ref int index){ 
        index = EditorGUI.Popup(new Rect(currentX, currentY, width, height), index, content);
        currentX += width + 5;
        lastRowHeight = height + 5;
    }
    void AddTextField(int width, int height, ref string text) { 
        text = GUI.TextField(new Rect(currentX, currentY, width, height), text);
        currentX += width + 5;
        lastRowHeight = height + 5;
    }
    void AddToggle(int width, int height, ref bool b, string text) { 
        b = GUI.Toggle(new Rect(currentX, currentY, width, height), b, text);
        currentX += width + 5;
        lastRowHeight = height + 5;
    }
    void NewLine() { 
        currentX = 0;
        currentY += lastRowHeight;
    }
    //---------------- node info ------------------------
	private IDictionary<FieldInfo, string> m_fieldValueCache = new Dictionary<FieldInfo, string>();
    void DrawSelectNodeInfo() { 
        if (m_treeMap.ContainsKey(m_selectRow)) { 
            BNode selectNode = m_treeMap[m_selectRow];
            FieldInfo[] fields = selectNode.GetType().GetFields();
            for (int i = 0; i < fields.Length; i++) { 
                FieldInfo field = fields[i];
                string fieldName = field.Name;
                AddLabel(fieldName.Length * 8, 40, fieldName + ":");
                
                object value = field.GetValue(selectNode);
                if(field.FieldType == typeof(int)) { 
					string valueStr = m_fieldValueCache.ContainsKey(field) ? m_fieldValueCache[field] : value.ToString();
                    AddTextField(100, 20, ref valueStr);
					if(IsInt(valueStr)){
						int intVal = int.Parse(valueStr);
						field.SetValue(selectNode, intVal);
						m_fieldValueCache.Remove(field);
					}else{
						m_fieldValueCache[field] = valueStr;
					}
                    NewLine();
				}else if(field.FieldType == typeof(float)){
					string valueStr = m_fieldValueCache.ContainsKey(field) ? m_fieldValueCache[field] : value.ToString();
					AddTextField(100, 20, ref valueStr);
					if(IsNumeric(valueStr)){
						float floatVal = float.Parse(valueStr);
						field.SetValue(selectNode, floatVal);
						m_fieldValueCache.Remove(field);
					}else{
						m_fieldValueCache[field] = valueStr;
					}
					NewLine();
				}else if(field.FieldType == typeof(string)){
                    string valueStr = value == null ? "" : value.ToString();
					AddTextField(100, 20, ref valueStr);
					field.SetValue(selectNode, valueStr);
                    NewLine();
                }else if(field.FieldType == typeof(bool)){
                    bool valueBool = (bool)field.GetValue(selectNode);
                    AddToggle(100, 20, ref valueBool, "");
                    field.SetValue(selectNode, valueBool);
                    NewLine();
                }                
            }
        }
    }
	bool IsNumeric(string value){
		return Regex.IsMatch(value, @"^(\-?[0-9]+(\.[0-9]+)?)$");
	}
	bool IsInt(string value){
		return Regex.IsMatch(value, @"^(\-?[0-9]+)$");
	}
    //---------------- event ----------------------------
    void OnEvent() { 
        Event evt = Event.current;
        if (evt.type == EventType.ContextClick) { 
        }
        if (evt.button == 1 && evt.type == EventType.MouseUp) { 
            GenericMenu menu = new GenericMenu();
            string[] typeNames = BehavTreeMagager.Instance.GetTypenames();
            for (int i = 0; i < typeNames.Length; i++) { 
                string content = "Create/" + typeNames[i];
                List<Type> types = BehavTreeMagager.Instance.GetTypesByTypename(typeNames[i]);
                for (int j = 0; j < types.Count; j++) { 
                    string typename = types[j].Name;
                    menu.AddItem(new GUIContent(content + "/" + typename), false, OnNodeCreate(types[j]));
                }
            }
            menu.AddItem(new GUIContent("Delete"), false, OnNodeDelete());
            menu.ShowAsContext();
        }
        if (evt.button == 0 && evt.type == EventType.MouseUp) { 
            m_selectRow = (int)(evt.mousePosition.y / ROW_HEIGHT);
            if (!m_treeMap.ContainsKey(m_selectRow) || m_treeMap[m_selectRow] == null) { 
                m_selectRow = -1;
            }
            Repaint();
        }
    }
    GenericMenu.MenuFunction OnNodeCreate(Type t) {
        if (m_curTree == null) { 
            return null;
        }

        if (m_curTree.Root != null && !m_treeMap.ContainsKey(m_selectRow)) { 
            return null;
        }
        
        GenericMenu.MenuFunction func;
        func = delegate(){
            BNode newNode = Activator.CreateInstance(t) as BNode;
            if (m_curTree.Root == null) {
                m_curTree.SetRoot(newNode);
            } else { 
                BNode parent = m_treeMap[m_selectRow];
                parent.AddNode(newNode);
            }
            Repaint();
        };
        return func;
    }
    GenericMenu.MenuFunction OnNodeDelete() { 
        if (!m_treeMap.ContainsKey(m_selectRow)) { 
            return null;
        }
        GenericMenu.MenuFunction func;
        func = delegate(){
            BNode node = m_treeMap[m_selectRow];
            if (node.Parent == null) { // 根结点
                if (m_curTree != null) { 
                    node = null;
                    m_curTree.SetRoot(null);
                }
            } else { 
                node.Parent.RemoveNode(node);
                node = null;
            }
            Repaint();
        };
        return func;
    }
}

#endif