/***************************************************************************
 *   Copyright (C) 2008 by Erik Karulf                                     *
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

using DirectShowLib;

namespace WUSTL.CSE.BinocularVision
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ComboBox VideoDeviceList1;
		private System.Windows.Forms.ComboBox CodecList;
		private System.Windows.Forms.Button VideoButton1;
		private System.Windows.Forms.Button CodecButton;
		private System.Windows.Forms.Button StopButton;
		private System.Windows.Forms.Button RecordButton;
		private System.Windows.Forms.TextBox FolderBox;
		private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private Button VideoButton2;
        private ComboBox VideoDeviceList2;
        private GroupBox groupBox1;
        private Button OutputButton;
        private FolderBrowserDialog folderBrowserDialog1;
        private Label label1;
        private Label label2;
        private TextBox FilenameBox;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		//A (modified) definition of OleCreatePropertyFrame found here: http://groups.google.com/group/microsoft.public.dotnet.languages.csharp/browse_thread/thread/db794e9779144a46/55dbed2bab4cd772?lnk=st&q=[DllImport(%22olepro32.dll%22)]&rnum=1&hl=no#55dbed2bab4cd772
		[DllImport(@"oleaut32.dll")] 
		public static extern int OleCreatePropertyFrame( 
			IntPtr hwndOwner, 
			int x, 
			int y, 
			[MarshalAs(UnmanagedType.LPWStr)] string lpszCaption, 
			int cObjects, 
			[MarshalAs(UnmanagedType.Interface, ArraySubType=UnmanagedType.IUnknown)] 
			ref object ppUnk, 
			int cPages, 
			IntPtr lpPageClsID, 
			int lcid, 
			int dwReserved, 
			IntPtr lpvReserved);

        List<DsDevice> DeviceList = new List<DsDevice>();
        IMediaControl MediaControl1 = null;
        IMediaControl MediaControl2 = null;
        IGraphBuilder GraphBuilder1 = null;
        IGraphBuilder GraphBuilder2 = null;
        IBaseFilter VideoDevice1 = null;
        IBaseFilter VideoDevice2 = null;
        IBaseFilter Compressor1 = null;
        IBaseFilter Compressor2 = null;

#if DEBUG
		// Allow you to "Connect to remote graph" from GraphEdit
		DsROTEntry m_rot = null;
#endif

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			//enumerate Video Input filters and add them to comboBox1
			foreach (DsDevice ds in DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice))
			{
                DeviceList.Add(ds);
                VideoDeviceList1.Items.Add(ds.Name);
                VideoDeviceList2.Items.Add(ds.Name);
			}

			//enumerate Video Compressor filters and add them to comboBox1
			foreach (DsDevice ds in DsDevice.GetDevicesOfCat(FilterCategory.VideoCompressorCategory))
			{
				CodecList.Items.Add(ds.Name);
			}

			//Select first combobox item
			if (VideoDeviceList1.Items.Count > 0)
			{
				VideoDeviceList1.SelectedIndex = 0;
			}

            //Select second combobox item
            if (VideoDeviceList2.Items.Count > 1)
            {
                VideoDeviceList2.SelectedIndex = 1;
            }

            //Select first combobox item
			if (CodecList.Items.Count > 0)
			{
				CodecList.SelectedIndex = 0;
			}

            //Select MPEG Compression by default
            foreach(object codec in CodecList.Items)
            {
                if (codec.ToString().Contains("MJPEG"))
                {
                    CodecList.SelectedIndex = CodecList.Items.IndexOf(codec);
                }
            }

			//Initialize button states
			RecordButton.Enabled = true;
			StopButton.Enabled = false;

            //Initialize output folder
            FolderBox.Text = folderBrowserDialog1.SelectedPath;

		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.VideoDeviceList1 = new System.Windows.Forms.ComboBox();
            this.CodecList = new System.Windows.Forms.ComboBox();
            this.VideoButton1 = new System.Windows.Forms.Button();
            this.CodecButton = new System.Windows.Forms.Button();
            this.StopButton = new System.Windows.Forms.Button();
            this.RecordButton = new System.Windows.Forms.Button();
            this.FolderBox = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.VideoButton2 = new System.Windows.Forms.Button();
            this.VideoDeviceList2 = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.FilenameBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.OutputButton = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // VideoDeviceList1
            // 
            this.VideoDeviceList1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.VideoDeviceList1.Location = new System.Drawing.Point(8, 8);
            this.VideoDeviceList1.Name = "VideoDeviceList1";
            this.VideoDeviceList1.Size = new System.Drawing.Size(256, 21);
            this.VideoDeviceList1.TabIndex = 0;
            this.VideoDeviceList1.SelectedIndexChanged += new System.EventHandler(this.VideoDeviceList1_SelectedIndexChanged);
            // 
            // CodecList
            // 
            this.CodecList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CodecList.Location = new System.Drawing.Point(418, 13);
            this.CodecList.Name = "CodecList";
            this.CodecList.Size = new System.Drawing.Size(256, 21);
            this.CodecList.TabIndex = 1;
            this.CodecList.SelectedIndexChanged += new System.EventHandler(this.CodecList_SelectedIndexChanged);
            // 
            // VideoButton1
            // 
            this.VideoButton1.Location = new System.Drawing.Point(272, 8);
            this.VideoButton1.Name = "VideoButton1";
            this.VideoButton1.Size = new System.Drawing.Size(120, 24);
            this.VideoButton1.TabIndex = 2;
            this.VideoButton1.Text = "Configure";
            this.VideoButton1.Click += new System.EventHandler(this.VideoDeviceButton1_Click);
            // 
            // CodecButton
            // 
            this.CodecButton.Enabled = false;
            this.CodecButton.Location = new System.Drawing.Point(682, 10);
            this.CodecButton.Name = "CodecButton";
            this.CodecButton.Size = new System.Drawing.Size(106, 24);
            this.CodecButton.TabIndex = 3;
            this.CodecButton.Text = "Configure";
            this.CodecButton.Click += new System.EventHandler(this.CodecButton_Click);
            // 
            // StopButton
            // 
            this.StopButton.Location = new System.Drawing.Point(682, 43);
            this.StopButton.Name = "StopButton";
            this.StopButton.Size = new System.Drawing.Size(106, 23);
            this.StopButton.TabIndex = 11;
            this.StopButton.Text = "Stop";
            this.StopButton.Click += new System.EventHandler(this.StopButton_Click);
            // 
            // RecordButton
            // 
            this.RecordButton.Location = new System.Drawing.Point(570, 43);
            this.RecordButton.Name = "RecordButton";
            this.RecordButton.Size = new System.Drawing.Size(106, 23);
            this.RecordButton.TabIndex = 10;
            this.RecordButton.Text = "Record";
            this.RecordButton.Click += new System.EventHandler(this.RecordButton_Click);
            // 
            // FolderBox
            // 
            this.FolderBox.Location = new System.Drawing.Point(66, 15);
            this.FolderBox.Name = "FolderBox";
            this.FolderBox.Size = new System.Drawing.Size(206, 20);
            this.FolderBox.TabIndex = 6;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Location = new System.Drawing.Point(8, 47);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(384, 288);
            this.panel1.TabIndex = 7;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Black;
            this.panel2.Location = new System.Drawing.Point(426, 47);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(384, 288);
            this.panel2.TabIndex = 8;
            // 
            // VideoButton2
            // 
            this.VideoButton2.Location = new System.Drawing.Point(690, 11);
            this.VideoButton2.Name = "VideoButton2";
            this.VideoButton2.Size = new System.Drawing.Size(120, 24);
            this.VideoButton2.TabIndex = 10;
            this.VideoButton2.Text = "Configure";
            this.VideoButton2.Click += new System.EventHandler(this.VideoDeviceButton2_Click);
            // 
            // VideoDeviceList2
            // 
            this.VideoDeviceList2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.VideoDeviceList2.Location = new System.Drawing.Point(426, 11);
            this.VideoDeviceList2.Name = "VideoDeviceList2";
            this.VideoDeviceList2.Size = new System.Drawing.Size(256, 21);
            this.VideoDeviceList2.TabIndex = 9;
            this.VideoDeviceList2.SelectedIndexChanged += new System.EventHandler(this.VideoDeviceList2_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.FilenameBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.OutputButton);
            this.groupBox1.Controls.Add(this.FolderBox);
            this.groupBox1.Controls.Add(this.CodecList);
            this.groupBox1.Controls.Add(this.CodecButton);
            this.groupBox1.Controls.Add(this.StopButton);
            this.groupBox1.Controls.Add(this.RecordButton);
            this.groupBox1.Location = new System.Drawing.Point(8, 342);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(802, 79);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Output";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Filename";
            // 
            // FilenameBox
            // 
            this.FilenameBox.Location = new System.Drawing.Point(66, 45);
            this.FilenameBox.Name = "FilenameBox";
            this.FilenameBox.Size = new System.Drawing.Size(318, 20);
            this.FilenameBox.TabIndex = 13;
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
            // OutputButton
            // 
            this.OutputButton.Location = new System.Drawing.Point(278, 13);
            this.OutputButton.Name = "OutputButton";
            this.OutputButton.Size = new System.Drawing.Size(106, 23);
            this.OutputButton.TabIndex = 7;
            this.OutputButton.Text = "Browse";
            this.OutputButton.UseVisualStyleBackColor = true;
            this.OutputButton.Click += new System.EventHandler(this.OutputButton_Click);
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.Description = "Select a folder to save video streams...";
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(822, 433);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.VideoButton2);
            this.Controls.Add(this.VideoDeviceList2);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.VideoButton1);
            this.Controls.Add(this.VideoDeviceList1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Binocular Vision";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		/// <summary>
		/// Stop recording and release the graph
		/// </summary>
		public void StopRecord()
		{
			//Stop the Graph
			MediaControl1.Stop();
            MediaControl2.Stop();

			//Release COM objects
			Marshal.ReleaseComObject(MediaControl1);
            Marshal.ReleaseComObject(MediaControl2);
			Marshal.ReleaseComObject(GraphBuilder1);
            Marshal.ReleaseComObject(GraphBuilder2);

			//Reset button state
			RecordButton.Enabled = true;
			StopButton.Enabled = false;
#if DEBUG
			if (m_rot != null)
			{
				m_rot.Dispose();
				m_rot = null;
			}
#endif		
		}

		/// <summary>
		/// Start recording
		/// </summary>
		public void Record()
		{
			if (MediaControl1 != null && MediaControl2 != null)
			{
				//Reset button state
				RecordButton.Enabled = false;
				StopButton.Enabled = true;
			
				//Run the graphs
				MediaControl1.Run();
                MediaControl2.Run();
			}
		}

		/// <summary>
		/// Initialize the graph
		/// </summary>
		public static void InitGraph(IBaseFilter VideoDevice, IBaseFilter Compressor, Panel DisplayPanel, String filename, out IGraphBuilder GraphBuilder, out IMediaControl MediaControl)
		{
			//Create the Graph
			GraphBuilder = (IGraphBuilder) new FilterGraph();
			
			//Create the Capture Graph Builder
			ICaptureGraphBuilder2 captureGraphBuilder = null;
			captureGraphBuilder = (ICaptureGraphBuilder2) new CaptureGraphBuilder2();
			
			//Create the media control for controlling the graph
			MediaControl = (IMediaControl) GraphBuilder;

            // Validate inputs
            if (VideoDevice == null || Compressor == null)
                return;
            
            // Attach the filter graph to the capture graph
			int hr = captureGraphBuilder.SetFiltergraph(GraphBuilder);
			DsError.ThrowExceptionForHR(hr);

			//Add the Video input device to the graph
			hr = GraphBuilder.AddFilter(VideoDevice, "source filter");
			DsError.ThrowExceptionForHR(hr);

			
			//Add the Video compressor filter to the graph
			hr = GraphBuilder.AddFilter(Compressor, "compressor filter");
			DsError.ThrowExceptionForHR(hr);

			//Create the file writer part of the graph. SetOutputFileName does this for us, and returns the mux and sink
			IBaseFilter mux;
			IFileSinkFilter sink;
			hr = captureGraphBuilder.SetOutputFileName(MediaSubType.Avi, filename, out mux, out sink);
			DsError.ThrowExceptionForHR(hr);


			//Render any preview pin of the device
			hr = captureGraphBuilder.RenderStream(PinCategory.Preview, MediaType.Video, VideoDevice, null, null);
			DsError.ThrowExceptionForHR(hr);

			//Connect the device and compressor to the mux to render the capture part of the graph
			hr = captureGraphBuilder.RenderStream(PinCategory.Capture, MediaType.Video, VideoDevice, Compressor, mux);
			DsError.ThrowExceptionForHR(hr);

#if DEBUG
//			m_rot = new DsROTEntry(GraphBuilder);
#endif

			//get the video window from the graph
			IVideoWindow videoWindow = null;
			videoWindow = (IVideoWindow) GraphBuilder;

			//Set the owener of the videoWindow to an IntPtr of some sort (the Handle of any control - could be a form / button etc.)
			hr = videoWindow.put_Owner(DisplayPanel.Handle);
			DsError.ThrowExceptionForHR(hr);

			//Set the style of the video window
			hr = videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipChildren);
			DsError.ThrowExceptionForHR(hr);

			// Position video window in client rect of main application window
			hr = videoWindow.SetWindowPosition(0,0, DisplayPanel.Width, DisplayPanel.Height);
			DsError.ThrowExceptionForHR(hr);

			// Make the video window visible
			hr = videoWindow.put_Visible(OABool.True);
			DsError.ThrowExceptionForHR(hr);

			Marshal.ReleaseComObject(mux);
			Marshal.ReleaseComObject(sink);
			Marshal.ReleaseComObject(captureGraphBuilder);
		}


		/// <summary>
		/// Enumerates all filters of the selected category and returns the IBaseFilter for the 
		/// filter described in friendlyname
		/// </summary>
		/// <param name="category">Category of the filter</param>
		/// <param name="friendlyname">Friendly name of the filter</param>
		/// <returns>IBaseFilter for the device</returns>
		private IBaseFilter CreateFilter(Guid category, string friendlyname)
		{
			object source = null;
			Guid iid = typeof(IBaseFilter).GUID;
			foreach (DsDevice device in DsDevice.GetDevicesOfCat(category))
			{
				if (device.Name.CompareTo(friendlyname) == 0)
				{
					device.Mon.BindToObject(null, null, ref iid, out source);
					break;
				}
			}

			return (IBaseFilter)source;
		}

        private IBaseFilter CreateFilter(DsDevice device)
        {
            object source = null;
            Guid iid = typeof(IBaseFilter).GUID;
            device.Mon.BindToObject(null, null, ref iid, out source);
            return (IBaseFilter)source;
        }


		/// <summary>
		/// Displays a property page for a filter
		/// </summary>
		/// <param name="dev">The filter for which to display a property page</param>
		private void DisplayPropertyPage(IBaseFilter dev)
		{
			//Get the ISpecifyPropertyPages for the filter
			ISpecifyPropertyPages pProp = dev as ISpecifyPropertyPages;
			int hr = 0;

			if (pProp == null)
			{
				//If the filter doesn't implement ISpecifyPropertyPages, try displaying IAMVfwCompressDialogs instead!
				IAMVfwCompressDialogs compressDialog = dev as IAMVfwCompressDialogs;
				if (compressDialog != null)
				{

					hr = compressDialog.ShowDialog(VfwCompressDialogs.Config, IntPtr.Zero);
					DsError.ThrowExceptionForHR(hr);
				}
				return;
			}

			//Get the name of the filter from the FilterInfo struct
			FilterInfo filterInfo;
			hr = dev.QueryFilterInfo(out filterInfo); 
			DsError.ThrowExceptionForHR(hr);

            // Get the propertypages from the property bag
            DsCAUUID caGUID;
            hr = pProp.GetPages(out caGUID);
            DsError.ThrowExceptionForHR(hr);

            // Check for property pages on the output pin
            IPin pPin = DsFindPin.ByDirection(dev, PinDirection.Output, 0);
            ISpecifyPropertyPages pProp2 = pPin as ISpecifyPropertyPages;
            if (pProp2 != null)
            {
                DsCAUUID caGUID2;
                hr = pProp2.GetPages(out caGUID2);
                DsError.ThrowExceptionForHR(hr);

                if (caGUID2.cElems > 0)
                {
                    int soGuid = Marshal.SizeOf(typeof(Guid));

                    // Create a new buffer to hold all the GUIDs
                    IntPtr p1 = Marshal.AllocCoTaskMem((caGUID.cElems + caGUID2.cElems) * soGuid);

                    // Copy over the pages from the Filter
                    for (int x = 0; x < caGUID.cElems * soGuid; x++)
                    {
                        Marshal.WriteByte(p1, x, Marshal.ReadByte(caGUID.pElems, x));
                    }

                    // Add the pages from the pin
                    for (int x = 0; x < caGUID2.cElems * soGuid; x++)
                    {
                        Marshal.WriteByte(p1, x + (caGUID.cElems * soGuid), Marshal.ReadByte(caGUID2.pElems, x));
                    }

                    // Release the old memory
                    Marshal.FreeCoTaskMem(caGUID.pElems);
                    Marshal.FreeCoTaskMem(caGUID2.pElems);

                    // Reset caGUID to include both
                    caGUID.pElems = p1;
                    caGUID.cElems += caGUID2.cElems;
                }
            }

            // Create and display the OlePropertyFrame
			object oDevice = (object)dev;
			hr = OleCreatePropertyFrame(this.Handle, 0, 0, filterInfo.achName, 1, ref oDevice, caGUID.cElems, caGUID.pElems, 0, 0, IntPtr.Zero);
			DsError.ThrowExceptionForHR(hr);

			// Release COM objects
			Marshal.FreeCoTaskMem(caGUID.pElems);
			Marshal.ReleaseComObject(pProp);
            if (filterInfo.pGraph != null)
            {
                Marshal.ReleaseComObject(filterInfo.pGraph);
            }
		}

		private void VideoDeviceButton1_Click(object sender, System.EventArgs e)
		{
			//Display property page for the selected video input device
			DisplayPropertyPage(VideoDevice1);
		}

        private void VideoDeviceButton2_Click(object sender, System.EventArgs e)
        {
            //Display property page for the selected video input device
            DisplayPropertyPage(VideoDevice2);
        }

		private void CodecButton_Click(object sender, System.EventArgs e)
		{
			//Display property page for the selected video compressor
			DisplayPropertyPage(Compressor1);		
		}

		private void RecordButton_Click(object sender, System.EventArgs e)
		{
            if (FilenameBox.Text.Length == 0)
            {
                MessageBox.Show(this, "Please enter a filename");
                FilenameBox.Show();
                return;
            }
            if (FolderBox.Text.Length == 0)
            {
                SelectOutputFolder();
                if (FolderBox.Text.Length == 0)
                    return;
            }
            String path = FolderBox.Text + "\\"+ FilenameBox.Text;
            InitGraph(VideoDevice1, Compressor1, panel1, path + "-Device1.avi", out GraphBuilder1, out MediaControl1);
            InitGraph(VideoDevice2, Compressor2, panel2, path + "-Device2.avi", out GraphBuilder2, out MediaControl2);
			Record();
		}

		private void StopButton_Click(object sender, System.EventArgs e)
		{
			StopRecord();
		}

		private void VideoDeviceList1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			//Release COM objects
			if (VideoDevice1 != null)
			{
				Marshal.ReleaseComObject(VideoDevice1);
				VideoDevice1 = null;
			}

            VideoDevice1 = CreateFilter(DeviceList[VideoDeviceList1.SelectedIndex]);
		}

        private void VideoDeviceList2_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            //Release COM objects
            if (VideoDevice2 != null)
            {
                Marshal.ReleaseComObject(VideoDevice2);
                VideoDevice2 = null;
            }

            VideoDevice2 = CreateFilter(DeviceList[VideoDeviceList2.SelectedIndex]);
        }

		private void CodecList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			//Release COM objects
            if (Compressor1 != null)
            {
                Marshal.ReleaseComObject(Compressor1);
                Compressor1 = null;
            }
            if (Compressor2 != null)
            {
                Marshal.ReleaseComObject(Compressor2);
                Compressor2 = null;
            }
            //Create the filter for the selected video compressor
			string devicepath = CodecList.SelectedItem.ToString();
			Compressor1 = CreateFilter(FilterCategory.VideoCompressorCategory, devicepath);
            Compressor2 = CreateFilter(FilterCategory.VideoCompressorCategory, devicepath);
		}

        private void OutputButton_Click(object sender, EventArgs e)
        {
            SelectOutputFolder();
        }

        private void SelectOutputFolder()
        {
            folderBrowserDialog1.ShowDialog();
            FolderBox.Text = folderBrowserDialog1.SelectedPath;
        }
	}
}
