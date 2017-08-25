﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RemoteControl.Protocals;
using log4net;

namespace RemoteControl.Server
{
    public partial class FrmCaptureScreen : FrmBase
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(FrmCaptureScreen));
        private SocketSession oSession;
        private bool _isCaptureMouse = false;
        private bool _isCaptureKeyboard = false;

        public FrmCaptureScreen(SocketSession session)
        {
            InitializeComponent();
            this.oSession = session;
            this.FormClosed += new FormClosedEventHandler(FrmCaptureScreen_FormClosed);
        }

        void FrmCaptureScreen_FormClosed(object sender, FormClosedEventArgs e)
        {
            oSession.Send(ePacketType.PACKET_STOP_CAPTURE_SCREEN_REQUEST, null);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            ToolStripButton button = sender as ToolStripButton;
            button.Checked = !button.Checked;
            if (button.Checked)
            {
                RequestStartGetScreen req = new RequestStartGetScreen();
                req.fps = 5;
                oSession.Send(ePacketType.PACKET_START_CAPTURE_SCREEN_REQUEST, req);
            }
            else
            {
                oSession.Send(ePacketType.PACKET_STOP_CAPTURE_SCREEN_REQUEST, null);
            }
        }

        public void HandleScreen(ResponseStartGetScreen resp)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<ResponseStartGetScreen>(HandleScreen), resp);
                return;
            }
            try
            {
                this.pictureBox1.Image = resp.GetImage();
            }
            catch (Exception ex)
            {
                Logger.Error("HandleScreen", ex);
            }
        }

        private void FrmCaptureScreen_Load(object sender, EventArgs e)
        {
            // Panel增加滚动条
            this.panel1.AutoScroll = true;
            // 根据图像大小，自动调节控件和Image的尺寸
            this.pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
        }

        private void toolStripButtonSave_Click(object sender, EventArgs e)
        {
            if (this.pictureBox1.Image != null)
            {
                Image img = (Image)this.pictureBox1.Image.Clone();
                string fileName = "";
                SaveFileDialog dialog = new SaveFileDialog();
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    fileName = dialog.FileName;
                    try
                    {
                        img.Save(fileName);
                        img.Dispose();
                        MsgBox.ShowInfo("保存成功!");
                    }
                    catch (Exception ex)
                    {
                        MsgBox.ShowInfo("保存失败，" + ex.Message);
                    }
                }
            }
            else
            {
                MsgBox.ShowInfo("暂无图像，无法保存！");
            }
        }

        /// <summary>
        /// 捕获鼠标操作按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemCaptureMouse_Click(object sender, EventArgs e)
        {
            toolStripMenuItemCaptureMouse.Checked = !toolStripMenuItemCaptureMouse.Checked;
            _isCaptureMouse = toolStripMenuItemCaptureMouse.Checked;
        }

        /// <summary>
        /// 捕获键盘操作按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemCaptureKeyboard_Click(object sender, EventArgs e)
        {
            toolStripMenuItemCaptureKeyboard.Checked = !toolStripMenuItemCaptureKeyboard.Checked;
            _isCaptureKeyboard = toolStripMenuItemCaptureKeyboard.Checked;
        }

        #region 鼠标操作事件
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if(_isCaptureMouse)
            {
                RequestMouseEvent req = new RequestMouseEvent();
                req.MouseButton = (eMouseButtons)e.Button;
                req.MouseOperation = eMouseOperations.MouseDown;
                req.MouseLocation = e.Location;
                this.oSession.Send(ePacketType.PACKET_MOUSE_EVENT_REQUEST, req);
                Console.WriteLine(req);
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (_isCaptureMouse)
            {
                RequestMouseEvent req = new RequestMouseEvent();
                req.MouseButton = (eMouseButtons)e.Button;
                req.MouseOperation = eMouseOperations.MouseUp;
                req.MouseLocation = e.Location;
                this.oSession.Send(ePacketType.PACKET_MOUSE_EVENT_REQUEST, req);
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            //if (_isCaptureMouse)
            //{
            //    RequestMouseEvent req = new RequestMouseEvent();
            //    req.MouseButton = (eMouseButtons)e.Button;
            //    req.MouseOperation = eMouseOperations.MouseMove;
            //    req.MouseLocation = e.Location;
            //    this.oSession.Send(ePacketType.PACKET_MOUSE_EVENT_REQUEST, req);
            //}
        }

        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (_isCaptureMouse)
            {
                RequestMouseEvent req = new RequestMouseEvent();
                req.MouseButton = (eMouseButtons)e.Button;
                req.MouseOperation = eMouseOperations.MouseDoubleClick;
                req.MouseLocation = e.Location;
                this.oSession.Send(ePacketType.PACKET_MOUSE_EVENT_REQUEST, req);
            }
        } 
        #endregion

        /// <summary>
        /// 不同的帧率的点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemFPS_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item != null && item.Tag != null)
            {
                var parent = this.toolStripSplitButton2;
                for (int i = 0; i < parent.DropDownItems.Count; i++)
                {
                    var mItem = parent.DropDownItems[i] as ToolStripMenuItem;
                    if (mItem != null)
                    {
                        mItem.Checked = false;
                    }
                }

                int fps = Convert.ToInt32(item.Tag);
                item.Checked = true;
                RequestStartGetScreen req = new RequestStartGetScreen();
                req.fps = fps;
                oSession.Send(ePacketType.PACKET_START_CAPTURE_SCREEN_REQUEST, req);
            }
        }

        /// <summary>
        /// 帧率选择点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripSplitButton2_ButtonClick(object sender, EventArgs e)
        {
            toolStripSplitButton2.ShowDropDown();
        }

        /// <summary>
        /// 捕获操作点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {
            toolStripSplitButton1.ShowDropDown();
        }
    }
}
