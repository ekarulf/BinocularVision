/***************************************************************************
 *   Copyright (C) 2008-2009 by Erik Karulf                                *
 *   erik@cse.wustl.edu                                                    *
 *                                                                         *
 *   Permission is hereby granted, free of charge, to any person obtaining *
 *   a copy of this software and associated documentation files (the       *
 *   "Software"), to deal in the Software without restriction, including   *
 *   without limitation the rights to use, copy, modify, merge, publish,   *
 *   distribute, sublicense, and/or sell copies of the Software, and to    *
 *   permit persons to whom the Software is furnished to do so, subject to *
 *   the following conditions:                                             *
 *                                                                         *
 *   The above copyright notice and this permission notice shall be        *
 *   included in all copies or substantial portions of the Software.       *
 *                                                                         *
 *   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,       *
 *   EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF    *
 *   MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.*
 *   IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR     *
 *   OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, *
 *   ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR *
 *   OTHER DEALINGS IN THE SOFTWARE.                                       *
 ***************************************************************************/

using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Runtime.InteropServices;
using System.Diagnostics;

using AForge;
using AForge.Video;
using AForge.Video.DirectShow;

namespace WUSTL.CSE.BinocularVision
{
    public enum State
    {
        Disabled,
        Idle,
        Preview,
        Record,
    }

	/// <summary>
	/// Summary description for Form1.
	/// </summary>
    public class Form1 : System.Windows.Forms.Form
    {
        // UI Components
        private ComboBox camera1Combo;
        private ComboBox codecCombo;
        private Button camera1ConfigureButton;
        private Button codecConfigureButton;
        private GroupBox outputGroupBox;
        private Button recordButton;
        private Button previewButton;
        private TextBox folderTextBox;
        private Button browseFolderButton;
        private FolderBrowserDialog folderBrowserDialog1;
        private Label label1;
        private Label label2;
        private TextBox captureTextBox;
        private AForge.Controls.VideoSourcePlayer videoPlayer1;
        private IContainer components;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private AForge.Controls.VideoSourcePlayer videoPlayer2;
        private ComboBox camera2Combo;
        private Button camera2ConfigureButton;
        private Timer timer1;

        // Data Members
        private FilterInfoCollection videoDevices = null;
        private VideoCaptureDevice Camera1 = null;
        private VideoCaptureDevice Camera2 = null;
        private State state = State.Disabled;

        // statistics length
        private const int statLength = 15;
        // current statistics index
        private int statIndex = 0;
        // ready statistics values
        private int statReady = 0;
        // statistics array
        private int[] statCount1 = new int[statLength];
        private int[] statCount2 = new int[statLength];


        public Form1()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            UpdateComboBoxes();
            if (camera1Combo.Enabled && camera2Combo.Enabled)
                state = State.Idle;
            UpdateUI();

            //Initialize output folder
            folderTextBox.Text = folderBrowserDialog1.SelectedPath;
        }

        private void UpdateUI()
        {
            switch (state)
            {
                case State.Disabled:
                    previewButton.Enabled = false;
                    recordButton.Enabled = false;
                    break;
                case State.Idle:
                    previewButton.Enabled = true;
                    recordButton.Enabled = true;
                    break;
                case State.Preview:
                    previewButton.Enabled = true;
                    recordButton.Enabled = true;
                    break;
                case State.Record:
                    previewButton.Enabled = false;
                    recordButton.Enabled = true;
                    break;
                default:
                    throw new Exception("Programmer Error! Invalid State");
            }
        }

        private void UpdateComboBoxes()
        {
            camera1Combo.Items.Clear();
            camera2Combo.Items.Clear();
            codecCombo.Items.Clear();

            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (videoDevices.Count == 0)
            {
                camera1Combo.Items.Add("No cameras found");
                camera2Combo.Items.Add("No cameras found");

                camera1Combo.SelectedIndex = 0;
                camera2Combo.SelectedIndex = 0;

                camera1Combo.Enabled = false;
                camera2Combo.Enabled = false;

                camera1ConfigureButton.Enabled = false;
                camera2ConfigureButton.Enabled = false;
            }
            else
            {

                for (int i = 1, n = videoDevices.Count; i <= n; i++)
                {
                    string cameraName = i + " : " + videoDevices[i - 1].Name;

                    camera1Combo.Items.Add(cameraName);
                    camera2Combo.Items.Add(cameraName);
                }

                // check cameras count
                if (videoDevices.Count == 1)
                {
                    camera2Combo.Items.Clear();

                    camera2Combo.Items.Add("Only one camera found");
                    camera2Combo.SelectedIndex = 0;
                    camera2Combo.Enabled = false;
                    camera2ConfigureButton.Enabled = false;
                }
                else
                {
                    camera2Combo.SelectedIndex = 1;
                }
                camera1Combo.SelectedIndex = 0;
                camera2ConfigureButton.Enabled = true;
            }


            //enumerate Video Compressor filters and add them to comboBox1
            FilterInfoCollection codecs = new FilterInfoCollection(FilterCategory.VideoCompressorCategory);
            foreach (FilterInfo codec in codecs)
            {
                codecCombo.Items.Add(codec.Name);
            }

            //Select first combobox item
            if (codecCombo.Items.Count > 0)
            {
                codecCombo.SelectedIndex = 0;
            }

            //Select MPEG Compression by default
            foreach (object codec in codecCombo.Items)
            {
                if (codec.ToString().Contains("MJPEG"))
                {
                    codecCombo.SelectedIndex = codecCombo.Items.IndexOf(codec);
                }
            }
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.camera1Combo = new System.Windows.Forms.ComboBox();
            this.codecCombo = new System.Windows.Forms.ComboBox();
            this.camera1ConfigureButton = new System.Windows.Forms.Button();
            this.codecConfigureButton = new System.Windows.Forms.Button();
            this.recordButton = new System.Windows.Forms.Button();
            this.previewButton = new System.Windows.Forms.Button();
            this.folderTextBox = new System.Windows.Forms.TextBox();
            this.outputGroupBox = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.captureTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.browseFolderButton = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.videoPlayer1 = new AForge.Controls.VideoSourcePlayer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.videoPlayer2 = new AForge.Controls.VideoSourcePlayer();
            this.camera2Combo = new System.Windows.Forms.ComboBox();
            this.camera2ConfigureButton = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.outputGroupBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // camera1Combo
            // 
            this.camera1Combo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.camera1Combo.Location = new System.Drawing.Point(6, 19);
            this.camera1Combo.Name = "camera1Combo";
            this.camera1Combo.Size = new System.Drawing.Size(256, 21);
            this.camera1Combo.TabIndex = 0;
            // 
            // codecCombo
            // 
            this.codecCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.codecCombo.Location = new System.Drawing.Point(429, 15);
            this.codecCombo.Name = "codecCombo";
            this.codecCombo.Size = new System.Drawing.Size(256, 21);
            this.codecCombo.TabIndex = 1;
            // 
            // camera1ConfigureButton
            // 
            this.camera1ConfigureButton.Location = new System.Drawing.Point(270, 16);
            this.camera1ConfigureButton.Name = "camera1ConfigureButton";
            this.camera1ConfigureButton.Size = new System.Drawing.Size(120, 24);
            this.camera1ConfigureButton.TabIndex = 2;
            this.camera1ConfigureButton.Text = "Configure";
            this.camera1ConfigureButton.Click += new System.EventHandler(this.VideoDeviceButton1_Click);
            // 
            // codecConfigureButton
            // 
            this.codecConfigureButton.Enabled = false;
            this.codecConfigureButton.Location = new System.Drawing.Point(693, 12);
            this.codecConfigureButton.Name = "codecConfigureButton";
            this.codecConfigureButton.Size = new System.Drawing.Size(106, 24);
            this.codecConfigureButton.TabIndex = 3;
            this.codecConfigureButton.Text = "Configure";
            this.codecConfigureButton.Click += new System.EventHandler(this.CodecButton_Click);
            // 
            // recordButton
            // 
            this.recordButton.Location = new System.Drawing.Point(693, 45);
            this.recordButton.Name = "recordButton";
            this.recordButton.Size = new System.Drawing.Size(106, 23);
            this.recordButton.TabIndex = 11;
            this.recordButton.Text = "Record";
            this.recordButton.Click += new System.EventHandler(this.RecordButton_Click);
            // 
            // previewButton
            // 
            this.previewButton.Location = new System.Drawing.Point(581, 45);
            this.previewButton.Name = "previewButton";
            this.previewButton.Size = new System.Drawing.Size(106, 23);
            this.previewButton.TabIndex = 10;
            this.previewButton.Text = "Preview";
            this.previewButton.Click += new System.EventHandler(this.previewButton_Click);
            // 
            // folderTextBox
            // 
            this.folderTextBox.Location = new System.Drawing.Point(66, 15);
            this.folderTextBox.Name = "folderTextBox";
            this.folderTextBox.Size = new System.Drawing.Size(206, 20);
            this.folderTextBox.TabIndex = 6;
            // 
            // outputGroupBox
            // 
            this.outputGroupBox.Controls.Add(this.label2);
            this.outputGroupBox.Controls.Add(this.captureTextBox);
            this.outputGroupBox.Controls.Add(this.label1);
            this.outputGroupBox.Controls.Add(this.browseFolderButton);
            this.outputGroupBox.Controls.Add(this.folderTextBox);
            this.outputGroupBox.Controls.Add(this.codecCombo);
            this.outputGroupBox.Controls.Add(this.codecConfigureButton);
            this.outputGroupBox.Controls.Add(this.recordButton);
            this.outputGroupBox.Controls.Add(this.previewButton);
            this.outputGroupBox.Location = new System.Drawing.Point(8, 342);
            this.outputGroupBox.Name = "outputGroupBox";
            this.outputGroupBox.Size = new System.Drawing.Size(805, 79);
            this.outputGroupBox.TabIndex = 11;
            this.outputGroupBox.TabStop = false;
            this.outputGroupBox.Text = "Capture";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Name";
            // 
            // captureTextBox
            // 
            this.captureTextBox.Location = new System.Drawing.Point(66, 45);
            this.captureTextBox.Name = "captureTextBox";
            this.captureTextBox.Size = new System.Drawing.Size(318, 20);
            this.captureTextBox.TabIndex = 13;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Folder";
            // 
            // browseFolderButton
            // 
            this.browseFolderButton.Location = new System.Drawing.Point(278, 13);
            this.browseFolderButton.Name = "browseFolderButton";
            this.browseFolderButton.Size = new System.Drawing.Size(106, 23);
            this.browseFolderButton.TabIndex = 7;
            this.browseFolderButton.Text = "Browse";
            this.browseFolderButton.UseVisualStyleBackColor = true;
            this.browseFolderButton.Click += new System.EventHandler(this.OutputButton_Click);
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.Description = "Select a folder to save video streams...";
            // 
            // videoPlayer1
            // 
            this.videoPlayer1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.videoPlayer1.ForeColor = System.Drawing.Color.White;
            this.videoPlayer1.Location = new System.Drawing.Point(6, 46);
            this.videoPlayer1.Name = "videoPlayer1";
            this.videoPlayer1.Size = new System.Drawing.Size(384, 272);
            this.videoPlayer1.TabIndex = 15;
            this.videoPlayer1.VideoSource = null;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.videoPlayer1);
            this.groupBox1.Controls.Add(this.camera1Combo);
            this.groupBox1.Controls.Add(this.camera1ConfigureButton);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(396, 324);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Camera 1";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.videoPlayer2);
            this.groupBox2.Controls.Add(this.camera2Combo);
            this.groupBox2.Controls.Add(this.camera2ConfigureButton);
            this.groupBox2.Location = new System.Drawing.Point(417, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(396, 324);
            this.groupBox2.TabIndex = 18;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Camera 2";
            // 
            // videoPlayer2
            // 
            this.videoPlayer2.BackColor = System.Drawing.SystemColors.ControlDark;
            this.videoPlayer2.ForeColor = System.Drawing.Color.White;
            this.videoPlayer2.Location = new System.Drawing.Point(6, 46);
            this.videoPlayer2.Name = "videoPlayer2";
            this.videoPlayer2.Size = new System.Drawing.Size(384, 272);
            this.videoPlayer2.TabIndex = 15;
            this.videoPlayer2.VideoSource = null;
            // 
            // camera2Combo
            // 
            this.camera2Combo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.camera2Combo.Location = new System.Drawing.Point(6, 19);
            this.camera2Combo.Name = "camera2Combo";
            this.camera2Combo.Size = new System.Drawing.Size(256, 21);
            this.camera2Combo.TabIndex = 0;
            // 
            // camera2ConfigureButton
            // 
            this.camera2ConfigureButton.Location = new System.Drawing.Point(270, 16);
            this.camera2ConfigureButton.Name = "camera2ConfigureButton";
            this.camera2ConfigureButton.Size = new System.Drawing.Size(120, 24);
            this.camera2ConfigureButton.TabIndex = 2;
            this.camera2ConfigureButton.Text = "Configure";
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(825, 433);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.outputGroupBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Binocular Vision";
            this.outputGroupBox.ResumeLayout(false);
            this.outputGroupBox.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        // On form closing
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            switch (state)
            {
                case State.Preview:
                    StopPreview();
                    state = State.Disabled;
                    break;
                case State.Record:
                    StopRecording();
                    state = State.Disabled;
                    break;
            }
        }

        // On "Preview" button click
        private void previewButton_Click(object sender, EventArgs e)
        {
            switch (state)
            {
                case State.Idle:
                    StartPreview();
                    state = State.Preview;
                    break;
                case State.Preview:
                    StopPreview();
                    state = State.Idle;
                    break;
                default:
                    throw new Exception("Programmer Error! Invalid State");
            }
            UpdateUI();
        }

        private void ConnectCameras()
        {
            if (Camera1 != null || Camera2 != null)
                DisconnectCameras();
            if (camera1Combo.Enabled)
                Camera1 = new VideoCaptureDevice(videoDevices[camera1Combo.SelectedIndex].MonikerString);
            if (camera1Combo.Enabled && camera2Combo.Enabled)
                System.Threading.Thread.Sleep(1000);   // BUGFIX: Work around some broken webcam drivers
            if (camera2Combo.Enabled)
                Camera2 = new VideoCaptureDevice(videoDevices[camera2Combo.SelectedIndex].MonikerString);
        }

        private void DisconnectCameras()
        {
            if (Camera1 != null)
                Camera1.SignalToStop();
            if (Camera2 != null)
                Camera2.SignalToStop();
            if (Camera1 != null)
                Camera1.WaitForStop();
            if (Camera2 != null)
                Camera2.WaitForStop();
            
            Camera1 = null;
            Camera2 = null;
        }

        private void StartPreview()
        {
            // create first video source
            ConnectCameras();

            if (Camera1 != null)
            {
                videoPlayer1.VideoSource = Camera1;
                videoPlayer1.Start();
            }
            // create second video source
            if (Camera2 != null)
            {
                videoPlayer2.VideoSource = Camera2;
                videoPlayer2.Start();
            }
        }

        // Stop cameras
        private void StopPreview()
        {
            videoPlayer1.SignalToStop();
            videoPlayer2.SignalToStop();

            videoPlayer1.WaitForStop();
            videoPlayer2.WaitForStop();
        }

        private void StartRecording() 
        {
            // TODO: Implement
        }
        private void StopRecording() 
        {
            // TODO: Implement
        }

        private void VideoDeviceButton1_Click(object sender, System.EventArgs e)
        {
            Camera1.DisplayPropertyPage(Handle);
        }

        private void VideoDeviceButton2_Click(object sender, System.EventArgs e)
        {
            Camera2.DisplayPropertyPage(Handle);
        }

        private void CodecButton_Click(object sender, System.EventArgs e)
        {
            //Display property page for the selected video compressor
            
        }

        private void RecordButton_Click(object sender, System.EventArgs e)
        {
            //if (captureTextBox.Text.Length == 0)
            //{
            //    MessageBox.Show(this, "Please enter a filename");
            //    captureTextBox.Show();
            //    return;
            //}
            //if (folderTextBox.Text.Length == 0)
            //{
            //    SelectOutputFolder();
            //    if (folderTextBox.Text.Length == 0)
            //        return;
            //}
            //String path = folderTextBox.Text + "\\" + captureTextBox.Text;
            //InitGraph(Camera1, Compressor1, panel1, path + "-Device1.avi", out GraphBuilder1, out MediaControl1);
            //InitGraph(Camera2, Compressor2, panel2, path + "-Device2.avi", out GraphBuilder2, out MediaControl2);
            //Record();
        }

        private void StopButton_Click(object sender, System.EventArgs e)
        {
            StopRecording();
        }

        private void OutputButton_Click(object sender, EventArgs e)
        {
            SelectOutputFolder();
        }

        private void SelectOutputFolder()
        {
            folderBrowserDialog1.ShowDialog();
            folderTextBox.Text = folderBrowserDialog1.SelectedPath;
        }
    }
}
