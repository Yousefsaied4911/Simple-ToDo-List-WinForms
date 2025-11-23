using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using ToDoList.Properties;

namespace ToDoList
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Stack<System.Windows.Forms.TreeNode> _Undo = new Stack<System.Windows.Forms.TreeNode>();
        private Stack<System.Windows.Forms.TreeNode> _Redo = new Stack<System.Windows.Forms.TreeNode>();

   

        System.Windows.Forms.TreeNode CopyOfTreeNode()
        {
            System.Windows.Forms.TreeNode CopyTreeNode = new System.Windows.Forms.TreeNode();
            foreach (TreeNode Node in treeView1.Nodes)
            {
                CopyTreeNode.Nodes.Add((TreeNode)Node.Clone());
            }

            return CopyTreeNode;
        }

        void UpdateTreeViewOfForm(System.Windows.Forms.TreeNode NewTreeViewNode)
        {
            treeView1.Nodes.Clear();
            foreach (TreeNode Node in NewTreeViewNode.Nodes)
            {
                treeView1.Nodes.Add((TreeNode)Node.Clone());
            }

            treeView1.Refresh();
        }

        void UndoPush()
        {
            _Undo.Push(CopyOfTreeNode());
            btnUndo.Enabled = true;
            btnUndo.BackgroundImage = Resources.UndoEnabled;
        }

        void RedoPush()
        {
            _Redo.Push(CopyOfTreeNode());
            btnRedo.Enabled = true;
            btnRedo.BackgroundImage = Resources.RedoEnabled;
        }

        private bool ValidateTaskInput()
        {
            if (txtTask.Text.Trim() == "")
            {
                MessageBox.Show("Task Cannot Be Empty!", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        void AddTask()
        {
            UndoPush();

            TreeNode Node = new TreeNode(txtTask.Text);
            Node.ContextMenuStrip = contextMenuStrip1;
            treeView1.Nodes.Add(Node);
            CalculateProgressSteps();
        }

        void DeleteTask()
        {
            UndoPush();

            if (treeView1.SelectedNode != null)
            {
                treeView1.Nodes.Remove(treeView1.SelectedNode);
                CalculateProgressSteps();
                CalculateProgressBar();
            }
        }

        void EditTask()
        {
            if (treeView1.SelectedNode != null)
            {
                UndoPush();
                treeView1.SelectedNode.BeginEdit();
            }
        }

        void RemoveTask()
        {
            UndoPush();
            if (treeView1.Nodes.Count != 0)
            {
                treeView1.Nodes.RemoveAt(treeView1.Nodes.Count - 1);
                CalculateProgressSteps();
                CalculateProgressBar();
            }
        }

        void ClearAllTaske()
        {
            if (treeView1.Nodes.Count == 0)
                return;

            UndoPush();
            treeView1.Nodes.Clear();
            CalculateProgressSteps();
            CalculateProgressBar();
        }

        void ConfirmClearAllTasks()
        {
            var result = MessageBox.Show("Are you sure you want to clear all tasks?", "Confirm Clear All", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                ClearAllTaske();
            }
        }

        private void ChageTaskIconAfterCheck(TreeNode node)
        {
            node.ImageIndex = node.Checked ? 1 : 0;
            node.SelectedImageIndex = node.Checked ? 1 : 0;
        }

    

        void CalculateProgressSteps()
        {
            if (treeView1.Nodes.Count == 0)
            {
                progressBar1.Step = 0;
                return;
            }

            double Steps = (double)100 / treeView1.Nodes.Count;
            progressBar1.Step = (int)Math.Ceiling(Steps);
        }

        void CalculateProgressBar()
        {
            progressBar1.Value = 0;
            progressBar1.Maximum = 100;

            foreach (TreeNode Node in treeView1.Nodes)
            {
                if (Node.Checked)
                {
                    progressBar1.PerformStep();
                }
            }

            if (progressBar1.Maximum > 0)
            {
                lbProgressPresent.Text = (((float)progressBar1.Value / progressBar1.Maximum) * 100 + "%");
            }
            else
            {
                lbProgressPresent.Text = "0%";
            }
        }

        

        private void btnAddTask_Click(object sender, EventArgs e)
        {
            if (!ValidateTaskInput())
            {
                return;
            }

            AddTask();
            txtTask.Text = "";
            CalculateProgressSteps();
            CalculateProgressBar();
        }

        private void btnRemoveTask_Click(object sender, EventArgs e)
        {
            RemoveTask();
            CalculateProgressSteps();
            CalculateProgressBar();
        }

        private void btnClearALL_Click(object sender, EventArgs e)
        {
            ConfirmClearAllTasks();
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            if (_Undo.Count > 0)
            {
                RedoPush();
                UpdateTreeViewOfForm(_Undo.Peek());
                _Undo.Pop();
            }

            if (_Undo.Count == 0)
            {
                btnUndo.Enabled = false;
                btnUndo.BackgroundImage = Resources.UndoDisabled;
            }

            CalculateProgressSteps();
            CalculateProgressBar();
        }

        private void btnRedo_Click(object sender, EventArgs e)
        {
            if (_Redo.Count > 0)
            {
                UndoPush();
                UpdateTreeViewOfForm(_Redo.Peek());
                _Redo.Pop();
            }

            if (_Redo.Count == 0)
            {
                btnRedo.Enabled = false;
                btnRedo.BackgroundImage = Resources.RedoDisabled;
            }

            CalculateProgressSteps();
            CalculateProgressBar();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
        }

        private void treeView1_BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {
            UndoPush();
        }

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            ChageTaskIconAfterCheck(e.Node);
            CalculateProgressBar();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
        }

        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Text)
            {
                case "Delete Task":
                    DeleteTask();
                    break;
                case "Edit Task":
                    EditTask();
                    break;
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {
        }
    }
}