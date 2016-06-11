using System;
using System.Collections.Generic;

namespace BHaviourTree { 
    public enum ActionResult { 
        SUCCESS,
        RUNNING,
        FAILURE,
        NONE,
    }

    public class BNode {
        //---------- tree ------------
        protected string m_name;
        protected BNode m_parent;
        protected List<BNode> m_children = new List<BNode>();
        protected ActionResult m_state = ActionResult.NONE; 

        public BNode() { 
            m_name = this.GetType().Name;
        }
        public string Name {
            get { 
                return m_name;
            }
        }
        public BNode Parent {
            get { 
                return m_parent;
            }
        }
        public int ChildCount {
            get { 
                return m_children.Count;
            }
        }

        public BNode GetChild(int i) {
            if (i < m_children.Count) { 
                return m_children[i];
            }
            return null;
        }

        public void InsertNodeFollow(BNode preNode, BNode node) { 
            int index = m_children.FindIndex((a) => { return a == preNode; });
            if (index >= 0) { 
                m_children.Insert(index, node);
            } else { 
                m_children.Add(node);
            }
        
            node.m_parent = this;
        }
        public void AddNode(BNode node) { 
            m_children.Add(node);
            node.m_parent = this;
        }
        public void RemoveNode(BNode node) {
            if (m_children.Contains(node)) { 
                m_children.Remove(node);
                node.m_parent = null;
            }
        }
        public bool ContainNode(BNode node) { 
            return m_children.Contains(node);
        }


        //------------- behaviour -------------
        public ActionResult Run(object input, ref string param) {
            if (m_state == ActionResult.NONE) { 
                OnEnter(input);
                m_state = ActionResult.RUNNING;
            }
            ActionResult res = Excute(input, ref param);
            if (res != ActionResult.RUNNING) { 
                OnExit(input);
                m_state = ActionResult.NONE;
            }
            return res;
        }
        protected virtual ActionResult Excute(object input, ref string param) { 
            return ActionResult.SUCCESS;
        }
        protected virtual void OnEnter(object input) { 
        }
        protected virtual void OnExit(object input) { 
        }
    }
}

