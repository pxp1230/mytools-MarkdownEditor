using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            SetTextBoxSelection(0);
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                DirectoryInfo folder = new DirectoryInfo(args[1]);
                FileInfo file = new FileInfo(args[1]);
                if (folder.Exists)
                {
                    workDirectoryPath = folder.FullName;
                }
                else if (file.Exists)
                {
                    ReadMarkdownFile(file.FullName);
                }
            }
            else
            {
                FileTypeRegister.FileTypeReg(".md");
            }
        }

        [DllImport("user32")]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, IntPtr lParam);
        private const int WM_SETREDRAW = 0xB;

        //停止控件的重绘
        private void BeginPaint()
        {
            SendMessage(textBox1.Handle, WM_SETREDRAW, 0, IntPtr.Zero);
        }

        //允许控件重绘
        private void EndPaint()
        {
            SendMessage(textBox1.Handle, WM_SETREDRAW, 1, IntPtr.Zero);
            textBox1.Refresh();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            textBox1.Focus();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //MessageBox.Show(keyData.ToString());
            this.textBox1.Focus();
            switch (keyData)
            {
                case Keys.Tab:
                    Tab();
                    return true;
                case Keys.Shift | Keys.Tab:
                    ShiftTab();
                    return true;
                case Keys.Control | Keys.Tab:
                    ControlTab();
                    return true;
                case Keys.Enter:
                    Enter_();
                    return true;
                case Keys.Down:
                    if (Down())
                        return true;
                    else
                        return base.ProcessCmdKey(ref msg, keyData);
                case Keys.Right:
                    if (Right_())
                        return true;
                    else
                        return base.ProcessCmdKey(ref msg, keyData);
                case Keys.Left:
                    if (Left_())
                        return true;
                    else
                        return base.ProcessCmdKey(ref msg, keyData);
                case Keys.Control | Keys.A:
                    CtrlA();
                    return true;
                case Keys.Control | Keys.C:
                    if (CtrlC())
                        return true;
                    else
                        return base.ProcessCmdKey(ref msg, keyData);
                case Keys.Control | Keys.X:
                    if (CtrlX())
                        return true;
                    else
                        return base.ProcessCmdKey(ref msg, keyData);
                case Keys.Control | Keys.V:
                    CtrlV();
                    return true;
                case Keys.Control | Keys.Delete:
                case Keys.Control | Keys.L:
                    CtrlDel();
                    return true;
                case Keys.Control | Keys.Shift | Keys.Delete:
                case Keys.Control | Keys.Shift | Keys.L:
                    CtrlDel(true);
                    return true;
                case Keys.Control | Keys.D:
                    CtrlD();
                    return true;
                case Keys.Insert:
                    Insert();
                    return true;
                case Keys.Shift | Keys.Insert:
                    Insert(true);
                    return true;
                case Keys.Control | Keys.Insert:
                    CtrlInsert();
                    return true;
                case Keys.Control | Keys.O:
                    CtrlO();
                    return true;
                case Keys.Control | Keys.S:
                    CtrlS();
                    return true;
                case Keys.Control | Keys.N:
                case Keys.Control | Keys.T:
                    CtrlN();
                    return true;
                case Keys.Escape:
                case Keys.Control | Keys.W:
                    Esc();
                    return true;
                case Keys.F3:
                    F3();
                    return true;
                case Keys.Shift | Keys.F3:
                    ShiftF3();
                    return true;
                //case Keys.Control | Keys.Shift | Keys.B:
                //    CtrlShiftB();
                //    return true;
                case Keys.F5://编译并预览
                    F5();
                    return true;
                //case Keys.Control | Keys.F5:
                case Keys.F12://预览
                    CtrlF5();
                    return true;
                case Keys.F11://全屏切换
                    F11();
                    return true;
                case Keys.Control | Keys.D1:
                    MarkdownPrefix("# ");
                    return true;
                case Keys.Control | Keys.D2:
                    MarkdownPrefix("## ");
                    return true;
                case Keys.Control | Keys.D3:
                    MarkdownPrefix("### ");
                    return true;
                case Keys.Control | Keys.D4:
                    MarkdownPrefix("#### ");
                    return true;
                case Keys.Control | Keys.D5:
                    MarkdownPrefix("##### ");
                    return true;
                case Keys.Control | Keys.D6:
                    MarkdownPrefix("###### ");
                    return true;
                case Keys.Control | Keys.B:
                    MarkdownTwoSide("**");
                    return true;
                case Keys.Control | Keys.I:
                    MarkdownTwoSide("*");
                    return true;
                case Keys.Control | Keys.Oemtilde:
                    MarkdownTwoSide("`");
                    return true;
                case Keys.Control | Keys.D8:
                    MarkdownPrefix("* ");
                    return true;
                case Keys.Control | Keys.ProcessKey:
                    MarkdownPrefix("> ");
                    return true;
                case Keys.Control | Keys.OemMinus:
                    MarkdownPrefix("- ");
                    return true;
                case Keys.Alt | Keys.Oemtilde:
                    MarkdownTwoSide("~~");
                    return true;
                case Keys.Control | Keys.Enter:
                    MarkdownTwoSide("```\r\n", "\r\n```");
                    return true;
                case Keys.Control | Keys.Oem7:
                    MarkdownTwoSide("'");
                    return true;
                case Keys.Control | Keys.Shift | Keys.Oem7:
                    MarkdownTwoSide("\"");
                    return true;
                case Keys.Control | Keys.D9:
                case Keys.Control | Keys.D0:
                    MarkdownTwoSide("(", ")");
                    return true;
                case Keys.Control | Keys.Shift | Keys.D9:
                    MarkdownTwoSide("[", "]()");
                    return true;
                case Keys.Control | Keys.OemOpenBrackets:
                case Keys.Control | Keys.Oem6:
                    MarkdownTwoSide("[", "]");
                    return true;
                case Keys.Control | Keys.Shift | Keys.OemOpenBrackets:
                case Keys.Control | Keys.Shift | Keys.Oem6:
                    MarkdownTwoSide("{", "}");
                    return true;
                case Keys.Control | Keys.Home:
                    CtrlHome();
                    return true;
                case Keys.Control | Keys.G:
                    CtrlG();
                    return true;
                case Keys.Alt | Keys.N:
                    AltN();
                    return true;
                default:
                    return base.ProcessCmdKey(ref msg, keyData);   //其他键按默认处理
            }
        }

        /// <summary>
        /// 设置TextBox的选择区域
        /// </summary>
        /// <param name="selectionStart"></param>
        /// <param name="selectionLength"></param>
        /// <param name="selectedText"></param>
        /// <returns>返回新的SelectionStart</returns>
        private int SetTextBoxSelection(int selectionStart, int selectionLength = 0, string selectedText = null)
        {
            if (selectedText != null)
            {
                textBox1.SelectionStart = selectionStart;
                textBox1.SelectionLength = selectionLength;
                textBox1.Paste(selectedText);
            }
            else
            {
                textBox1.Select(selectionStart, selectionLength);
            }
            return textBox1.SelectionStart;
        }

        public void Tab()
        {
            if (textBox1.SelectionLength == 0)
            {
                textBox1.Paste("\t");
            }
            else
            {
                BeginPaint();
                int oldStart = textBox1.SelectionStart;
                string oldStr = textBox1.SelectedText;
                string newStr = oldStr.Replace("\n", "\n\t");
                int lastReturnIndex = textBox1.Text.LastIndexOf('\n', oldStart == 0 ? 0 : oldStart - 1);

                SetTextBoxSelection(lastReturnIndex + 1, 0, "\t");
                SetTextBoxSelection(oldStart + 1, oldStr.Length, newStr);
                SetTextBoxSelection(oldStart + 1, newStr.Length);
                EndPaint();
            }
        }

        public void ShiftTab()
        {
            if (textBox1.Text.Length == 0)
                return;

            BeginPaint();
            int textLength = textBox1.Text.Length;
            int oldStart = textBox1.SelectionStart;
            oldStart = textBox1.Text.LastIndexOf('\n', oldStart == 0 ? 0 : oldStart - 1) + 1;
            int oldEnd = textBox1.SelectionStart + textBox1.SelectionLength;
            if (oldEnd != textLength)
            {
                int tmp = textBox1.Text.IndexOf('\r', oldEnd);
                if (tmp < 0)
                    oldEnd = textLength;
                else
                    oldEnd = tmp;
            }
            string oldStr = textBox1.Text.Substring(oldStart, oldEnd - oldStart);
            string newStr = "";
            string[] lines = oldStr.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i] == "")
                {
                    newStr += i != 0 ? "\r\n" : "";
                }
                else if (lines[i][0] == '\t')
                {
                    newStr += (i != 0 ? "\r\n" : "") + lines[i].Substring(1);
                }
                else
                {
                    int j = 0, _j = 0;
                    for (; _j < 4 && j < lines[i].Length; j++)
                    {
                        if (lines[i][j] == '　')
                            _j += 2;
                        else if (lines[i][j] == ' ')
                            _j++;
                        else
                            break;
                    }
                    newStr += (i != 0 ? "\r\n" : "") + lines[i].Substring(j);
                }
            }
            SetTextBoxSelection(oldStart, oldStr.Length, newStr);
            SetTextBoxSelection(oldStart, newStr.Length);
            EndPaint();
        }
        /// <summary>
        /// 中文缩进
        /// </summary>
        public void ControlTab()
        {
            BeginPaint();
            int oldStart = textBox1.SelectionStart;
            string oldStr = textBox1.SelectedText;
            string newStr = oldStr.Replace("\n", "\n　　").Replace("\n　　\r", "\n\r");
            if (newStr.Length >= 2 && newStr.EndsWith("　　"))
                newStr = newStr.Substring(0, newStr.Length - 2);
            int lastReturnIndex = textBox1.Text.LastIndexOf('\n', oldStart == 0 ? 0 : oldStart - 1);

            SetTextBoxSelection(lastReturnIndex + 1, 0, "　　");
            SetTextBoxSelection(oldStart + 2, oldStr.Length, newStr);
            SetTextBoxSelection(oldStart + 2, newStr.Length);
            EndPaint();
        }

        public void Enter_()
        {
            int oldStart = textBox1.SelectionStart;
            int lineStart = textBox1.Text.LastIndexOf('\n', oldStart == 0 ? 0 : oldStart - 1) + 1;
            int i = lineStart;
            int textLength = textBox1.Text.Length;
            for (; i < textLength; i++)
            {
                if (textBox1.Text[i] == '\t' || textBox1.Text[i] == ' ' || textBox1.Text[i] == '　')
                    continue;
                else
                    break;
            }
            string blankStr = textBox1.Text.Substring(lineStart, i - lineStart);
            int j = i;
            if (j < textLength && (textBox1.Text[j] == '*' || textBox1.Text[j] == '>' || textBox1.Text[j] == '-'))
            {
                if (j + 1 < textLength && textBox1.Text[j + 1] == ' ')
                {
                    string add0 = "";
                    string add1 = textBox1.Text[j] + " ";
                    int len = j + 2 - oldStart;
                    if (len >= 0)
                    {
                        add0 = textBox1.Text.Substring(j + 2 - len, len);
                        add1 = add1.Substring(0, 2 - len);
                    }
                    bool tmp = oldStart + 1 < textBox1.Text.Length && textBox1.Text[oldStart + 1] != '\n';
                    textBox1.Paste(add0 + "\r\n" + blankStr + add1);
                    if (len >= 0)
                    {
                        textBox1.SelectionStart = j + 2;
                    }
                    else if (tmp)
                    {
                        textBox1.SelectionStart = oldStart;
                    }
                    return;
                }
            }
            textBox1.Paste("\r\n" + blankStr);
        }

        public bool Down()
        {
            int oldStart = textBox1.SelectionStart + textBox1.SelectionLength;
            int textLength = textBox1.Text.Length;
            if (textLength == oldStart)
            {
                SetTextBoxSelection(textLength);
                Enter_();
                return true;
            }
            else
            {
                int nextReturnIndex = textBox1.Text.IndexOf('\r', oldStart);
                if (nextReturnIndex < 0)
                {
                    SetTextBoxSelection(textLength);
                    Enter_();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool Right_()
        {
            int oldStart = textBox1.SelectionStart + textBox1.SelectionLength;
            if (textBox1.SelectionLength > 0)
            {
                SetTextBoxSelection(oldStart);
                return true;
            }
            else if (textBox1.Text.Length == oldStart)
            {
                SetTextBoxSelection(oldStart);
                Enter_();
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool Left_()
        {
            if (textBox1.SelectionLength > 0)
            {
                SetTextBoxSelection(textBox1.SelectionStart);
                return true;
            }
            else
            {
                return false;
            }
        }
        public void CtrlDel(bool emptyLine = false)
        {
            if (textBox1.Text.Length == 0)
                return;

            int oldStart = textBox1.SelectionStart;
            int textLength = textBox1.Text.Length;
            int lineStart = textBox1.Text.LastIndexOf('\n', oldStart == 0 ? 0 : oldStart - 1) + 1;
            int lineEnd = textBox1.SelectionStart + textBox1.SelectionLength;
            if (lineEnd != textLength)
            {
                int tmp = textBox1.Text.IndexOf('\r', lineEnd);
                if (tmp < 0)
                    lineEnd = textLength;
                else
                    lineEnd = tmp;
            }

            if (lineStart != lineEnd)
            {
                Clipboard.Clear();
                Clipboard.SetText(textBox1.Text.Substring(lineStart, lineEnd - lineStart));
            }

            if (!emptyLine)
            {
                int i = lineStart;
                for (; i < lineEnd; i++)
                {
                    char testChar = textBox1.Text[i];
                    if (testChar == '\t' || testChar == ' ' || testChar == '　')
                        continue;
                    else
                        break;
                }
                bool hasBlank = i != lineStart;
                bool allBlank = i == lineEnd;

                if (lineStart - 2 >= 0)
                    lineStart = lineStart - 2;
                else if (lineEnd != textLength)
                    lineEnd = lineEnd + 2;
                SetTextBoxSelection(lineStart, lineEnd - lineStart, (lineStart != 0 && hasBlank && allBlank) ? "\r\n" : "");
            }
            else
            {
                SetTextBoxSelection(lineStart, lineEnd - lineStart, "");
            }
        }
        public void CtrlD()
        {
            if (textBox1.Text.Length == 0)
            {
                textBox1.Paste("\r\n");
                return;
            }

            int oldStart = textBox1.SelectionStart;
            int textLength = textBox1.Text.Length;
            int lineStart = textBox1.Text.LastIndexOf('\n', oldStart == 0 ? 0 : oldStart - 1) + 1;
            int lineEnd = textBox1.SelectionStart + textBox1.SelectionLength;
            int tmp = textBox1.Text.IndexOf('\r', lineEnd);
            if (tmp < 0)
                lineEnd = textLength;
            else
                lineEnd = tmp;

            string leftSide = "";
            if (lineStart - 2 >= 0)
                lineStart = lineStart - 2;
            else
                leftSide = "\r\n";

            string insertStr = leftSide + textBox1.Text.Substring(lineStart, lineEnd - lineStart);
            SetTextBoxSelection(lineEnd, 0, insertStr);
            SetTextBoxSelection(lineEnd + 2, insertStr.Length - 2);
        }
        public void CtrlO()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "打开Markdown文件";
            openFileDialog.Filter = "Markdown文件(*.md)|*.md|所有文件|*.*";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrEmpty(md_file_path) && textBox1.Text == "")
                {
                    ReadMarkdownFile(openFileDialog.FileName);
                }
                else
                {
                    Process.Start(Application.ExecutablePath, openFileDialog.FileName);
                }
            }
        }

        bool isTextSaved = false;//还需要结合判断md_file_path是否为空
        public void CtrlS()
        {
            if (string.IsNullOrEmpty(md_file_path))
            {
                if (!SaveDialog())
                    return;
            }
            else
            {
                if (isTextSaved)
                    return;
            }
            UTF8Encoding UTF8 = new UTF8Encoding(false);
            File.WriteAllText(md_file_path, textBox1.Text, UTF8);
            isTextSaved = true;
            _刷新窗口Title();
        }

        public void CtrlN()
        {
            if (string.IsNullOrEmpty(md_file_path))
                if (string.IsNullOrEmpty(workDirectoryPath))
                    Process.Start(Application.ExecutablePath);
                else
                    Process.Start(Application.ExecutablePath, workDirectoryPath);
            else
                Process.Start(Application.ExecutablePath, new FileInfo(md_file_path).Directory.FullName);
        }

        void ReadMarkdownFile(string path)
        {
            isTextSaved = false;
            InitPath(path);
            string allText = File.ReadAllText(md_file_path, Encoding.UTF8);
            int _n_index = allText.IndexOf('\n');
            bool _需要转换换行符 = false;
            if (_n_index >= 0)
            {
                if (allText.IndexOf("\r\n", Math.Max(_n_index - 1, 0), 2) < 0)
                {
                    _需要转换换行符 = true;
                }
            }
            if (_需要转换换行符)
            {
                textBox1.Text = allText.Replace("\n", "\r\n");
                isTextSaved = false;
            }
            else
            {
                textBox1.Text = allText;
                isTextSaved = true;
            }
            SetTextBoxSelection(0);
            textBox1.ScrollToCaret();
            _刷新窗口Title();
        }

        /// <summary>
        /// 选择附件（多选）
        /// </summary>
        string[] ChooseAttachmentFiles(string initialDirectory)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择附件（可多选）";
            openFileDialog.Filter = "所有文件|*.*";
            openFileDialog.RestoreDirectory = false;
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = true;
            openFileDialog.InitialDirectory = initialDirectory;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                return openFileDialog.FileNames;
            }
            else
            {
                return new string[0];
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;//取消关闭事件
            Esc();
        }
        public void Esc()
        {
            if (_退出前询问是否保存())
            {
                this.FormClosing -= new FormClosingEventHandler(this.Form1_FormClosing);//为保证Application.Exit();时不再弹出提示，所以将FormClosing事件取消
                Application.Exit();
            }
        }
        /// <summary>
        /// 退出前询问是否保存
        /// </summary>
        /// <returns>是否继续退出操作</returns>
        bool _退出前询问是否保存()
        {
            if ((textBox1.Text != "" && string.IsNullOrEmpty(md_file_path)) || (!string.IsNullOrEmpty(md_file_path) && !isTextSaved))
            {
                DialogResult a = MessageBox.Show("是否要保存修改？", "未保存的修改 - " + (string.IsNullOrEmpty(md_file_path) ? "[新笔记]" : new FileInfo(md_file_path).Name), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (a == DialogResult.Yes)
                {
                    CtrlS();
                    if (!isTextSaved)
                    {
                        return false;
                    }
                }
                else if (a == DialogResult.Cancel)
                {
                    return false;
                }
            }
            return true;
        }

        string md_file_path = "";
        string md_file_name
        {
            get
            {
                return Path.GetFileNameWithoutExtension(md_file_path);
            }
        }
        string md_folder_path = "";//附件文件夹
        public void InitPath(string path = "")
        {
            if (string.IsNullOrEmpty(path))
            {
                md_file_path = "";
                md_folder_path = "";
            }
            else
            {
                md_file_path = path;
                md_folder_path = md_file_path.Substring(0, md_file_path.LastIndexOf('.')) + "_files";
            }
        }

        /// <summary>
        /// 插入附件
        /// </summary>
        public void CtrlInsert()
        {
            if (string.IsNullOrEmpty(md_file_path))
            {
                CtrlS();
                if (!isTextSaved)
                    return;
            }

            bool is_md_folder_path_exists = Directory.Exists(md_folder_path);
            string[] files = ChooseAttachmentFiles(is_md_folder_path_exists ? md_folder_path : "");
            if (files.Length == 0)
                return;

            if (!is_md_folder_path_exists)
                Directory.CreateDirectory(md_folder_path);

            string ret = "";
            foreach (var item in files)
            {
                FileInfo file = new FileInfo(item);
                if (file.Exists)
                {
                    string test = file.Directory.FullName;
                    bool in_md_folder = test.Length >= md_folder_path.Length && (test + "\\").StartsWith(md_folder_path + "\\");
                    FileInfo newFile = in_md_folder ? file : file.CopyTo(Path.Combine(md_folder_path, file.Name));
                    ret += _GetFilePath(newFile);
                    continue;
                }
            }
            SetTextBoxSelection(textBox1.SelectionStart, textBox1.SelectionLength, ret.Length >= 2 ? ret.Substring(0, ret.Length - 2) : null);
        }

        int oldWidth, oldHeight;
        string insertedJPG;
        /// <summary>
        /// 插入图片
        /// </summary>
        public void Insert(bool forceInsert = false)
        {
            if (string.IsNullOrEmpty(md_file_path))
            {
                CtrlS();
                if (!isTextSaved)
                    return;
            }

            IDataObject data = Clipboard.GetDataObject();
            //The Clipboard was not empty
            if (data != null)
            {
                //The Data In Clipboard is as image format
                if (data.GetDataPresent(DataFormats.Bitmap))
                {
                    Image newImage = (Image)data.GetData(DataFormats.Bitmap, true);
                    int newWidth = newImage.Width, newHeight = newImage.Height;
                    if (newWidth != oldWidth || newHeight != oldHeight || forceInsert)
                    {
                        //有新图像数据，需要保存到本地
                        oldWidth = newWidth;
                        oldHeight = newHeight;

                        int curIndex = 0;
                        if (!Directory.Exists(md_folder_path))
                            Directory.CreateDirectory(md_folder_path);
                        else
                        {
                            FileInfo[] files = new DirectoryInfo(md_folder_path).GetFiles();
                            int maxIndex = -1;
                            foreach (var item in files)
                            {
                                int newIndex;
                                if (int.TryParse(item.Name.Substring(0, item.Name.LastIndexOf(".")), out newIndex))
                                {
                                    maxIndex = maxIndex < newIndex ? newIndex : maxIndex;
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            curIndex = maxIndex + 1;
                        }
                        newImage.Save(md_folder_path + "/" + curIndex + ".jpg", ImageFormat.Jpeg);
                        insertedJPG = "![](" + HttpUtility.UrlEncode(new DirectoryInfo(md_folder_path).Name, Encoding.UTF8).Replace("+", "%20") + "/" + curIndex + ".jpg)";
                        SetTextBoxSelection(textBox1.SelectionStart, textBox1.SelectionLength, insertedJPG);
                    }
                    else if (!string.IsNullOrEmpty(insertedJPG))
                    {
                        SetTextBoxSelection(textBox1.SelectionStart, textBox1.SelectionLength, insertedJPG);
                    }
                }
                else
                {
                    insertedJPG = "";
                }
            }
        }


        string workDirectoryPath = "";
        /// <summary>
        /// 保存文件对话框
        /// </summary>
        /// <returns>是否选择成功</returns>
        private bool SaveDialog()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (string.IsNullOrEmpty(workDirectoryPath))
            {
                sfd.RestoreDirectory = true;
            }
            else
            {
                sfd.RestoreDirectory = false;
                sfd.InitialDirectory = workDirectoryPath;
            }
            sfd.Filter = "Markdown文件(*.md)|*.md|所有文件|*.*";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                InitPath(sfd.FileName);
                return true;
            }
            return false;
        }

        public void CtrlA()
        {
            textBox1.SelectAll();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (isTextSaved)
            {
                isTextSaved = false;
                _刷新窗口Title();
            }
        }
        private void _刷新窗口Title()
        {
            this.Text = (isTextSaved ? "" : "*") + (string.IsNullOrEmpty(md_file_path) ? "[新笔记]" : (md_file_name + " - " + md_file_path));
        }

        /// <summary>
        /// 正向查找（查找剪切板文字）
        /// </summary>
        public void F3()
        {
            string clip = Clipboard.GetText();
            if (!string.IsNullOrEmpty(clip))
            {
                int index = textBox1.Text.IndexOf(clip, textBox1.SelectionStart + textBox1.SelectionLength);
                if (index < 0)
                {
                    index = textBox1.Text.IndexOf(clip, 0);
                }
                if (index < 0)
                {
                    return;
                }
                SetTextBoxSelection(index, clip.Length);
                textBox1.ScrollToCaret();
            }
        }
        /// <summary>
        /// 逆向查找（查找剪切板文字）
        /// </summary>
        public void ShiftF3()
        {
            string clip = Clipboard.GetText();
            if (!string.IsNullOrEmpty(clip))
            {
                int index = 0;
                if (textBox1.SelectionStart - 1 < 0)
                {
                    index = textBox1.Text.LastIndexOf(clip, textBox1.Text.Length - 1);
                }
                else
                {
                    index = textBox1.Text.LastIndexOf(clip, Math.Max(textBox1.SelectionStart - 1, 0));
                }
                if (index < 0)
                {
                    return;
                }
                SetTextBoxSelection(index, clip.Length);
                textBox1.ScrollToCaret();
            }
        }

        string pandoc_param_raw = "\"{0}.md\" -s -f gfm+hard_line_breaks -H \"" + Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "header.txt") + "\" -t html5 -o \"{0}.html\"";
        static Process pandoc;
        /// <summary>
        /// 生成HTML文件，不运行
        /// </summary>
        public void CtrlShiftB()
        {
            CtrlS();
            if (!string.IsNullOrEmpty(md_file_path) && isTextSaved)
            {
                FileInfo file = new FileInfo(md_file_path);
                string raw_fullName = file.FullName.Substring(0, file.FullName.LastIndexOf("."));
                string pandoc_param = string.Format(pandoc_param_raw, raw_fullName);
                if (pandoc == null)
                {
                    pandoc = new Process();
                    pandoc.StartInfo.FileName = "cmd";
                    pandoc.StartInfo.CreateNoWindow = true;
                    pandoc.StartInfo.UseShellExecute = false;
                    pandoc.StartInfo.RedirectStandardInput = true;
                    pandoc.StartInfo.RedirectStandardOutput = true;
                    pandoc.StartInfo.RedirectStandardError = true;
                    pandoc.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                }
                pandoc.StartInfo.Arguments = "/c pandoc " + pandoc_param;
                pandoc.Start();
                pandoc.WaitForExit();
            }
        }
        /// <summary>
        /// 全屏切换，调用窗口最大化功能
        /// </summary>
        public void F11()
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
            }
        }
        /// <summary>
        /// 生成HTML文件，并运行
        /// </summary>
        public void F5()
        {
            CtrlShiftB();
            CtrlF5();
        }
        /// <summary>
        /// 运行HTML文件（不生成）
        /// </summary>
        public void CtrlF5()
        {
            if (!string.IsNullOrEmpty(md_file_path))
            {
                FileInfo html = new FileInfo(md_file_path.Substring(0, md_file_path.LastIndexOf(".")) + ".html");
                if (html.Exists)
                    Process.Start(html.FullName);
            }
        }

        public void MarkdownPrefix(string prefix)
        {
            if (textBox1.Text == "")
            {
                textBox1.Paste(prefix);
                return;
            }

            BeginPaint();
            int oldStart = textBox1.SelectionStart;
            int lineStart = textBox1.SelectionStart == 0 ? 0 : (textBox1.Text.LastIndexOf('\n', oldStart == 0 ? 0 : oldStart - 1) + 1);
            int length = textBox1.Text.IndexOf("\r\n", textBox1.SelectionStart + textBox1.SelectionLength);
            string oldStr = length < 0 ? textBox1.Text.Substring(lineStart) : textBox1.Text.Substring(lineStart, length - lineStart);
            string newStr = "";
            string[] lines = oldStr.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            bool startsWithPrefix = false;
            int startsWithLength = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i] == "")
                {
                    newStr += (i != 0 ? "\r\n" : "") + prefix;
                }
                else
                {
                    int j = 0;
                    for (; j < lines[i].Length; j++)
                    {
                        if (lines[i][j] != ' ' && lines[i][j] != '\t')
                            break;
                    }
                    string substr = lines[i].Substring(j);
                    if (substr.StartsWith(prefix))
                    {
                        if (!startsWithPrefix)
                        {
                            startsWithPrefix = true;
                            startsWithLength = oldStart - lineStart - j - prefix.Length;
                            if (startsWithLength < 0)
                                startsWithLength = j;
                            else
                                startsWithLength = j + startsWithLength;
                        }
                        newStr += (i != 0 ? "\r\n" : "") + lines[i].Substring(0, j) + substr.Substring(prefix.Length);
                    }
                    else
                    {
                        newStr += (i != 0 ? "\r\n" : "") + lines[i].Substring(0, j) + prefix + substr;
                    }
                }
            }
            SetTextBoxSelection(lineStart, oldStr.Length, newStr);
            if (lines.Length == 1)
                SetTextBoxSelection(startsWithPrefix ? lineStart + startsWithLength : oldStart + prefix.Length);
            else
                SetTextBoxSelection(lineStart, newStr.Length);
            EndPaint();
        }

        public void MarkdownTwoSide(string oneSideStr)
        {
            MarkdownTwoSide(" " + oneSideStr, oneSideStr + " ");
        }

        public void MarkdownTwoSide(string leftSideStr, string rightSideStr)
        {
            BeginPaint();
            int oldLength = textBox1.SelectionLength;
            int oldStart = textBox1.SelectionStart;
            string oldStr = textBox1.SelectedText;
            string newStr = "";
            if (oldStr.StartsWith(leftSideStr) && oldStr.EndsWith(rightSideStr))
                newStr = oldStr.Substring(leftSideStr.Length, oldStr.Length - leftSideStr.Length - rightSideStr.Length);
            else
                newStr = leftSideStr + textBox1.SelectedText + rightSideStr;
            textBox1.Paste(newStr);
            if (oldLength == 0)
                SetTextBoxSelection(oldStart + leftSideStr.Length);
            else
                SetTextBoxSelection(oldStart, newStr.Length);
            EndPaint();
        }

        public bool CtrlC()
        {
            string select = textBox1.SelectedText;
            if (select.Length > 2 && select[0] == '$' && select[select.Length - 1] == '$')
            {
                Clipboard.Clear();
                Clipboard.SetText(select
                    .Replace(@"\\\\", @"\\")
                    .Replace(@"\left\\{", @"\left\{")
                    .Replace(@"\right\\}", @"\right\}")
                    .Replace(@"\`", @"`")
                    .Replace(@"\*", @"*")
                    .Replace(@"\_", @"_")
                    .Replace(@"\[", @"[")
                    .Replace(@"\]", @"]")
                    .Replace(@"\(", @"(")
                    .Replace(@"\)", @")")
                    .Replace(@"\#", @"#")
                    .Replace(@"\+", @"+")
                    .Replace(@"\-", @"-")
                    .Replace(@"\.", @".")
                    .Replace(@"\!", @"!")
                    );
                return true;
            }
            return false;
        }

        public bool CtrlX()
        {
            string select = textBox1.SelectedText;
            if (select.Length > 2 && select[0] == '$' && select[select.Length - 1] == '$')
            {
                Clipboard.Clear();
                Clipboard.SetText(select
                    .Replace(@"\\\\", @"\\")
                    .Replace(@"\left\\{", @"\left\{")
                    .Replace(@"\right\\}", @"\right\}")
                    .Replace(@"\`", @"`")
                    .Replace(@"\*", @"*")
                    .Replace(@"\_", @"_")
                    .Replace(@"\[", @"[")
                    .Replace(@"\]", @"]")
                    .Replace(@"\(", @"(")
                    .Replace(@"\)", @")")
                    .Replace(@"\#", @"#")
                    .Replace(@"\+", @"+")
                    .Replace(@"\-", @"-")
                    .Replace(@"\.", @".")
                    .Replace(@"\!", @"!")
                    );
                textBox1.Paste("");
                return true;
            }
            return false;
        }

        public bool CtrlV()
        {
            int oldStart = textBox1.SelectionStart;
            int lineStart = textBox1.SelectionStart == 0 ? 0 : (textBox1.Text.LastIndexOf('\n', oldStart == 0 ? 0 : oldStart - 1) + 1);
            int lineEnd = textBox1.Text.IndexOf("\r\n", textBox1.SelectionStart + textBox1.SelectionLength);
            bool doubleDollar = oldStart == lineStart && (lineEnd == -1 || lineEnd == lineStart);
            IDataObject data = Clipboard.GetDataObject();
            if (data != null)
            {
                if (data.GetDataPresent(DataFormats.Text))
                {
                    string clip = ((string)data.GetData(DataFormats.Text));
                    int index = clip.IndexOf('\n');
                    if (index == 0 || (index > 0 && clip[index - 1] != '\r'))
                        clip = clip.Replace("\n", "\r\n");
                    bool found = false;
                    if (clip.Length > 4)
                    {
                        if ((clip.StartsWith("$$") && clip.EndsWith("$$")) || (clip.StartsWith(@"\[") && clip.EndsWith(@"\]")) || (clip.StartsWith(@"\(") && clip.EndsWith(@"\)")))
                        {
                            clip = clip.Substring(2, clip.Length - 4);
                            found = true;
                        }
                    }
                    if (!found && clip.Length > 2 && clip[0] == '$' && clip[clip.Length - 1] == '$')
                    {
                        clip = clip.Substring(1, clip.Length - 2);
                        found = true;
                    }
                    if (found)
                    {
                        clip = clip
                            .Replace(@"\\\\", @"\\")
                            .Replace(@"\left\\{", @"\left\{")
                            .Replace(@"\right\\}", @"\right\}")
                            .Replace(@"\`", @"`")
                            .Replace(@"\*", @"*")
                            .Replace(@"\_", @"_")
                            .Replace(@"\#", @"#")
                            .Replace(@"\+", @"+")
                            .Replace(@"\-", @"-")
                            .Replace(@"\.", @".")
                            .Replace(@"\!", @"!")
                            .Replace(@"< ", @"<");
                        clip = clip
                            .Replace(@"\\", @"\\\\")
                            .Replace(@"\left\{", @"\left\\{")
                            .Replace(@"\right\}", @"\right\\}")
                            .Replace(@"`", @"\`")
                            .Replace(@"*", @"\*")
                            .Replace(@"_", @"\_")
                            .Replace(@"#", @"\#")
                            .Replace(@"+", @"\+")
                            .Replace(@"-", @"\-")
                            .Replace(@".", @"\.")
                            .Replace(@"!", @"\!")
                            .Replace(@"<", @"< ");
                        textBox1.Paste(doubleDollar ? ("$$" + clip + "$$") : (" $" + clip + "$ "));
                        return true;
                    }
                    else
                    {
                        textBox1.Paste(clip);
                        return false;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 打开文件所在目录
        /// </summary>
        public void CtrlHome()
        {
            if (string.IsNullOrEmpty(md_file_path))
            {
                if (!string.IsNullOrEmpty(workDirectoryPath))
                {
                    Process.Start("explorer.exe", workDirectoryPath);
                }
            }
            else
            {
                Process.Start("explorer.exe", "/select," + md_file_path);
            }
        }
        /// <summary>
        /// Open Git Gui
        /// </summary>
        void CtrlG()
        {
            if (string.IsNullOrEmpty(md_file_path))
                return;
            string dir = Path.GetDirectoryName(md_file_path);
            var si = new ProcessStartInfo(@"C:\Program Files\Git\cmd\git-gui.exe", "--working-dir \"" + dir + "\"");
            Process.Start(si);
        }
        /// <summary>
        /// Edit with Notepad++
        /// </summary>
        void AltN()
        {
            bool _是否继续退出操作 = _退出前询问是否保存();
            if (!_是否继续退出操作)
                return;

            if (string.IsNullOrEmpty(md_file_path))
                return;

            string exe_path = @"C:\Program Files (x86)\Notepad++\notepad++.exe";
            string exe_path_64 = @"C:\Program Files\Notepad++\notepad++.exe";
            if (!File.Exists(exe_path))
            {
                if (File.Exists(exe_path_64))
                    exe_path = exe_path_64;
                else
                    exe_path = "";
            }
            if (exe_path != "")
            {
                string arg = "\"" + md_file_path + "\"";
                var si = new ProcessStartInfo(exe_path, arg);
                Process.Start(si);
            }
            Esc();
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                //DragEventArgs.KeyState 属性: 4 SHIFT    8 CTRL    32 ALT
                if ((e.KeyState & 8) == 8)
                    e.Effect = DragDropEffects.Copy;
                else
                    e.Effect = DragDropEffects.Move;
            }
            else
                e.Effect = DragDropEffects.None;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files == null || files.Length == 0)
                return;

            if (files.Length == 1)
            {
                FileInfo md = new FileInfo(files[0]);
                if (md.Exists && md.Extension == ".md")
                {
                    if (string.IsNullOrEmpty(md_file_path) && textBox1.Text == "")
                    {
                        ReadMarkdownFile(md.FullName);
                    }
                    else
                    {
                        Process.Start(Application.ExecutablePath, md.FullName);
                    }
                    return;
                }
            }

            if (string.IsNullOrEmpty(md_file_path))
            {
                CtrlS();
                if (!isTextSaved)
                    return;
            }

            if (!Directory.Exists(md_folder_path))
                Directory.CreateDirectory(md_folder_path);

            string ret = "";
            foreach (var item in files)
            {
                if (item == md_folder_path)
                    continue;

                FileInfo file = new FileInfo(item);
                if (file.Exists)
                {
                    string test = file.Directory.FullName;
                    bool in_md_folder = test.Length >= md_folder_path.Length && (test + "\\").StartsWith(md_folder_path + "\\");
                    if (e.Effect == DragDropEffects.Move)
                        file.MoveTo(Path.Combine(md_folder_path, file.Name));
                    FileInfo newFile = in_md_folder ? file : e.Effect == DragDropEffects.Copy ? file.CopyTo(Path.Combine(md_folder_path, file.Name)) : new FileInfo(Path.Combine(md_folder_path, file.Name));
                    ret += _GetFilePath(newFile);
                    continue;
                }

                DirectoryInfo folder = new DirectoryInfo(item);
                if (folder.Exists)
                {
                    string test = folder.Parent.FullName;
                    bool in_md_folder = test.Length >= md_folder_path.Length && (test + "\\").StartsWith(md_folder_path + "\\");
                    DirectoryInfo newFolder = in_md_folder ? folder : e.Effect == DragDropEffects.Copy ? CopyFolder(folder.FullName, Path.Combine(md_folder_path, folder.Name)) : MoveFolder(folder.FullName, Path.Combine(md_folder_path, folder.Name));
                    ret += _GetFilesPaths(newFolder);
                    continue;
                }
            }
            SetTextBoxSelection(textBox1.SelectionStart, textBox1.SelectionLength, ret.Length >= 2 ? ret.Substring(0, ret.Length - 2) : null);
            this.Activate();
        }
        string _GetFilePath(FileInfo file)
        {
            string ret = "";
            string filename = file.Name;
            string path = file.FullName.Substring(md_folder_path.LastIndexOf('\\') + 1).Replace('\\', '/');
            //HTML URL 编码：http://www.w3school.com.cn/tags/html_ref_urlencode.html
            path = HttpUtility.UrlEncode(path, Encoding.UTF8).Replace("%2f", "/").Replace("+", "%20");
            if (file.Extension == ".jpg" || file.Extension == ".png" || file.Extension == ".gif" || file.Extension == ".webp")
            {
                ret += "![](" + path + ")\r\n";
            }
            else if (file.Extension == ".mp4")
            {
                ret += "<video src=\"" + path + "\" controls preload=\"none\" poster=\"\"></video>\r\n";
            }
            else if (file.Extension == ".mp3")
            {
                ret += "<audio src=\"" + path + "\" controls preload=\"none\"></audio>\r\n";
            }
            else
            {
                ret += "[" + filename + "](" + path + ")\r\n";
            }
            return ret;
        }
        string _GetFilesPaths(DirectoryInfo folder)
        {
            string ret = "";
            FileInfo[] files = folder.GetFiles();
            foreach (var item in files)
            {
                ret += _GetFilePath(item);
            }
            DirectoryInfo[] folders = folder.GetDirectories();
            foreach (var item in folders)
            {
                ret += _GetFilesPaths(item);
            }
            return ret;
        }

        private static DirectoryInfo CopyFolder(string from, string to)
        {
            DirectoryInfo newFolder = null;
            if (!Directory.Exists(to))
                newFolder = Directory.CreateDirectory(to);

            string[] files = Directory.GetFiles(from);
            foreach (string file in files)
                File.Copy(file, Path.Combine(to, Path.GetFileName(file)));

            string[] directories = Directory.GetDirectories(from);
            foreach (string sub in directories)
                CopyFolder(sub, Path.Combine(to, Path.GetFileName(sub)));

            return newFolder;
        }
        private static DirectoryInfo MoveFolder(string from, string to)
        {
            DirectoryInfo newFolder = null;
            if (!Directory.Exists(to))
                newFolder = Directory.CreateDirectory(to);

            string[] files = Directory.GetFiles(from);
            foreach (string file in files)
                File.Move(file, Path.Combine(to, Path.GetFileName(file)));

            string[] directories = Directory.GetDirectories(from);
            foreach (string sub in directories)
                MoveFolder(sub, Path.Combine(to, Path.GetFileName(sub)));

            return newFolder;
        }
    }
}
