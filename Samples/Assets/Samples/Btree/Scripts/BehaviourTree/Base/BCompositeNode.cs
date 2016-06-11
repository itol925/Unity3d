using System;

namespace BHaviourTree { 
    class BCompositeNode : BNode {
        protected int m_runningIndex = 0;

        public BCompositeNode() { 
            this.m_name = "Composite";
        }
    }

    //---------------------------
    /// <summary>
    /// 遍历子节点，直到一个成功的
    /// </summary>
    class BSelectorNode : BCompositeNode{
        public BSelectorNode() { 
            this.m_name = "Selector";
        }

        protected override void OnEnter(object input) {
            this.m_runningIndex = 0;
        }
        protected override ActionResult Excute(object input, ref string param) {
            if (m_runningIndex >= m_children.Count) { 
                return ActionResult.FAILURE;
            }
            BNode node = m_children[m_runningIndex];
            ActionResult res = node.Run(input, ref param);
            if (res == ActionResult.SUCCESS) { 
                return ActionResult.SUCCESS;
            }
            if (res == ActionResult.FAILURE) { 
                ++m_runningIndex;
            }
            return ActionResult.RUNNING;
        }
    }

    //---------------------------
    /// <summary>
    /// 遍历子结点，直到一个失败的
    /// </summary>
    class BSequenceNode : BCompositeNode{
        public BSequenceNode() { 
            this.m_name = "Sequence";
        }

        protected override void OnEnter(object input) {
            this.m_runningIndex = 0;
        }
        protected override ActionResult Excute(object input, ref string param) {
            if (m_runningIndex >= m_children.Count) { 
                return ActionResult.SUCCESS;
            }
            BNode node = m_children[m_runningIndex];
            ActionResult res = node.Run(input, ref param);
            if (res == ActionResult.FAILURE) { 
                return ActionResult.FAILURE;
            }
            if (res == ActionResult.SUCCESS) { 
                ++m_runningIndex;
            }
            return ActionResult.RUNNING;
        }
    }

    //---------------------------
    /// <summary>
    /// 遍历所有子结点
    /// </summary>
    class BParallelNode : BCompositeNode{
        public BParallelNode() { 
            this.m_name = "Parallel";
        }

        protected override void OnEnter(object input) {
            this.m_runningIndex = 0;
        }
        protected override ActionResult Excute(object input, ref string param) {
            if (m_runningIndex >= m_children.Count) { 
                return ActionResult.SUCCESS;
            }
            BNode node = m_children[m_runningIndex];
            ActionResult res = node.Run(input, ref param);
            if (res != ActionResult.RUNNING) { 
                ++m_runningIndex;
            }
            return ActionResult.RUNNING;
        }
    }
}

