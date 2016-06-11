using System;
using System.Collections.Generic;

namespace BHaviourTree { 
    public class BehavTree {
        private BNode m_root;
		private string m_name = "";

        public BehavTree(string name) {
			m_name = name;
        }
		public string Name{
			get{
				return m_name;
			}
		}
        public BNode Root {
            get { 
                return m_root;
            }
        }
        public void SetRoot(BNode root) { 
            m_root = root;
        }

        public List<string> Run(object input) { 
            List<string> allParam = new List<string>();
            ActionResult res = ActionResult.NONE;
            while(res != ActionResult.SUCCESS && res != ActionResult.FAILURE){
                string actionParam = "";
                res = this.m_root.Run(input, ref actionParam);
                if (actionParam != "") { 
                    allParam.Add(actionParam);
                }
                actionParam = "";
            }
            return allParam;
        }
    }
}
