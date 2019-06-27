using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System.IO;
using System.Threading;
using System.Globalization;
using UITimer = System.Windows.Forms.Timer;
using System.ComponentModel;

namespace Torrent_Sorter
{


    public partial class Form1 : Form
    {

        DataTable dt = new DataTable("Files");

        DataSet dataSet = new DataSet();
        static System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();





        public Form1()
        {
            InitializeComponent();

            // Get the origianl dirs from settings.
            textBox_download_dir.Text = Properties.Settings.Default.download_dir;

        }

        public List<DriveInfo> drive_info()
        {
            List<DriveInfo> drive_information = new List<DriveInfo>();
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                //There are more attributes you can use.
                //Check the MSDN link for a complete example.
                Console.WriteLine(drive.Name);
                if (drive.VolumeLabel.Contains("Plex"))
                {
                    drive_information.Add(drive);
                }
            }

            return drive_information;


        }


        public void load_data()
        {
            dataGridView2.Rows.Clear();
            string file = "data.bin";
            using (BinaryReader bw = new BinaryReader(File.Open(file, FileMode.Open)))
            {

                int n = bw.ReadInt32();
                int m = bw.ReadInt32();
                for (int i = 0; i < m; ++i)
                {
                    dataGridView2.Rows.Add();
                    for (int j = 0; j < n; ++j)
                    {
                        if (bw.ReadBoolean())
                        {
                            dataGridView2.Rows[i].Cells[j].Value = bw.ReadString();
                        }
                        else bw.ReadBoolean();
                    }
                }
            }
        }


        public void save_data()
        {
            string file = "data.bin";

            using (BinaryWriter bw = new BinaryWriter(File.Open(file, FileMode.Create)))
            {
                bw.Write(dataGridView2.Columns.Count);
                bw.Write(dataGridView2.Rows.Count);
                foreach (DataGridViewRow dgvR in dataGridView2.Rows)
                {
                    for (int j = 0; j < dataGridView2.Columns.Count; ++j)
                    {
                        object val = dgvR.Cells[j].Value;
                        if (val == null)
                        {
                            bw.Write(false);
                            bw.Write(false);
                        }
                        else
                        {
                            bw.Write(true);
                            bw.Write(val.ToString());
                        }
                    }
                }
            }
        }


        private async void MoveFile(string sourceFile, string destinationFile)
        {
            try
            {
                using (FileStream sourceStream = File.Open(sourceFile, FileMode.Open))
                {
                    using (FileStream destinationStream = File.Create(destinationFile))
                    {
                        await sourceStream.CopyToAsync(destinationStream);

                        sourceStream.Close();
                        File.Delete(sourceFile);

                    }
                }
            }
            catch (IOException ioex)
            {
                Console.WriteLine("An IOException occured during move, " + ioex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An Exception occured during move, " + ex.Message);
            }
        }

        public void Regex(object sender, DoWorkEventArgs e)
        {
            // Sleep for one second to not over work the system
            System.Threading.Thread.Sleep(1000);
            // Get the argument from e
            FileInfo fileInfo = (FileInfo)e.Argument;
            // Method for Title text
            TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
            List<DriveInfo> drive_inforation = drive_info();



            // Run PTN and convert into a dict to work with.
            var video_dict = new Dictionary<string, string>();
            ScriptEngine engine = Python.CreateEngine();
            string RunningPath = AppDomain.CurrentDomain.BaseDirectory;
            ScriptSource source = engine.CreateScriptSourceFromFile("parse.py");
            ScriptScope scope = engine.CreateScope();
            source.Execute(scope);

            dynamic python_file = scope.GetVariable("PTN");
            dynamic PTN = python_file();

            var list = PTN.parse(fileInfo.Name);

            // Loop threw list and invoke method on first go.
            foreach (var kvp in list)
            {
                video_dict.Add(kvp.ToString(), list[kvp].ToString().TrimEnd('.'));
            }

            // Defone string to have safe threads
            string destinationFile;
            string sourceFile = fileInfo.FullName;

            // Testing this to see if failed PTN with no title will skip.
            if (video_dict.ContainsKey("title") == false)
            {
                return;
            }
            // If the parse filename does not contail the key season is must be a movie.
            if (video_dict.ContainsKey("season") == false)
            {
                string drive_space_letter = "F:\\";
                // If the key year exists include this.
                if (video_dict.ContainsKey("year"))
                {
                    destinationFile = String.Format
                    ("{0}{1}\\{2}\\{3} - {4}{5}{6}",drive_space_letter,
                    "Movies",
                    myTI.ToTitleCase(video_dict["title"]),
                    myTI.ToTitleCase(video_dict["title"]),
                    video_dict["year"],
                    ".",
                    video_dict["container"]);
                }
                // else dont bother with the year. 
                else
                {
                    destinationFile = String.Format
                ("{0}{1}\\{2}\\{3}{4}{5}",drive_space_letter,
                "Movies",
                myTI.ToTitleCase(video_dict["title"]),
                myTI.ToTitleCase(video_dict["title"]),
                ".",
                video_dict["container"]);
                }

                // Create directory for movie
                Directory.CreateDirectory(String.Format("{0}\\{1}\\{2}",
                drive_space_letter,
                "Movies",
                myTI.ToTitleCase(video_dict["title"])));


                bool file_path = file_exists(destinationFile, drive_inforation);

                if (file_path is true)
                {
                    try
                    {
                        long old_file_length = new FileInfo(destinationFile).Length;
                        long new_file_length = new FileInfo(sourceFile).Length;
                        if (old_file_length > new_file_length)
                        {
                            File.Delete(sourceFile);
                            BeginInvoke((MethodInvoker)delegate
                            {

                                dataGridView2.Rows.Insert
                       (0, new string[] { DateTime.Now.ToString("dd/MM/yyyy hh:mm tt"),
                                        myTI.ToTitleCase(video_dict["title"]),
                                        "File Already Exists, File Removed" });
                            });
                        }

                        else
                        {
                            try
                            {
                                File.Delete(destinationFile);
                                MoveFile(sourceFile, destinationFile);

                            }
                            catch (System.IO.IOException)
                            {
                                Console.WriteLine("IO Exception Move File");
                                return;
                            }
                            BeginInvoke((MethodInvoker)delegate
                            {
                                //dataGridView2.Update();
                                //dataGridView2.Refresh();
                                dataGridView2.Rows.Insert
                        (0, new string[] {DateTime.Now.ToString("dd/MM/yyyy hh:mm tt"),
                                myTI.ToTitleCase(video_dict["title"]),
                                string.Format("Better Quality {0}{1}", drive_space_letter, destinationFile) });
                            });
                        }
                    }
                    catch (System.IO.DirectoryNotFoundException)
                    {
                        Console.WriteLine("Could not find DIR");
                        return;

                    }
                    catch (System.IO.IOException)
                    {
                        Console.WriteLine("IO Exception");
                        return;
                    }
                }
                else
                {
                    MoveFile(sourceFile, destinationFile);
                    BeginInvoke((MethodInvoker)delegate
                    {
                        //dataGridView2.Update();
                        //dataGridView2.Refresh();
                        dataGridView2.Rows.Insert
                (0, new string[] {DateTime.Now.ToString("dd/MM/yyyy hh:mm tt"),
                                myTI.ToTitleCase(video_dict["title"]),
                                string.Format("{0}{1}", drive_space_letter, destinationFile) });
                    });
                }







       
                //add_data(DateTime.Now.ToString("h:mm:ss tt"),
                //myTI.ToTitleCase(video_dict["title"]),
                //destinationFile);
            }

                // If the result has an episode it is a TV show.
                if (video_dict.ContainsKey("episode") == true)

                {
                    string drive_space_letter = "E:\\";

                    Directory.CreateDirectory
                            (String.Format("{0}\\{1}\\{2}",
                            drive_space_letter,
                            "TV",
                            myTI.ToTitleCase(video_dict["title"])));

                    Directory.CreateDirectory
                    (String.Format("{0}\\{1}\\{2}\\Season {3}",
                    drive_space_letter,
                    "TV",
                    myTI.ToTitleCase(video_dict["title"]),
                    video_dict["season"]));

                    string seasonFolderNumber = video_dict["season"];

                    if (video_dict["episode"].Count() < 2)
                    {
                        video_dict["episode"] = string.Format("{0}{1}", 0, video_dict["episode"]);
                    }
                    if (video_dict["season"].Count() < 2)
                    {
                        video_dict["season"] = string.Format("{0}{1}", 0, video_dict["season"]);
                    }








                    destinationFile = String.Format
                    ("{0}{1}\\{2}\\Season {3}\\{4} S{5}E{6}{7}{8}",
                    drive_space_letter, "TV",
                    myTI.ToTitleCase(video_dict["title"]),
                    seasonFolderNumber,
                    myTI.ToTitleCase(video_dict["title"]),
                    video_dict["season"],
                    myTI.ToTitleCase(video_dict["episode"]),
                    ".",
                    video_dict["container"]);

                    bool file_path_mv = file_exists(destinationFile, drive_inforation);

                if (file_path_mv is true)
                {
                    try
                    {
                        long old_file_length = new FileInfo(destinationFile).Length;
                        long new_file_length = new FileInfo(sourceFile).Length;
                        if (old_file_length > new_file_length)
                        {
                            File.Delete(sourceFile);
                            BeginInvoke((MethodInvoker)delegate
                            {

                                dataGridView2.Rows.Insert
                       (0, new string[] { DateTime.Now.ToString("dd/MM/yyyy hh:mm tt"),
                                        myTI.ToTitleCase(video_dict["title"]),
                                        "File Already Exists, File Removed" });
                            });
                        }
                        else
                        {
                            try
                            {
                                File.Delete(destinationFile);
                                MoveFile(sourceFile, destinationFile);

                            }
                            catch (System.IO.IOException)
                            {
                                return;
                            }
                            BeginInvoke((MethodInvoker)delegate
                            {
                                //dataGridView2.Update();
                                //dataGridView2.Refresh();
                                dataGridView2.Rows.Insert
                    (0, new string[] {DateTime.Now.ToString("dd/MM/yyyy hh:mm tt"),
                                myTI.ToTitleCase(video_dict["title"]),
                                string.Format("Better Quality {0}{1}", drive_space_letter, destinationFile) });
                            });

                            //add_data(DateTime.Now.ToString("h:mm:ss tt"),
                            //myTI.ToTitleCase(video_dict["title"]),
                            //destinationFile);

                        }
                    }
                    catch (System.IO.DirectoryNotFoundException)
                    {
                        return;
                    }
                    catch (System.IO.IOException)
                    {

                        return;
                    }
                }

                else
                {
                    MoveFile(sourceFile, destinationFile);
                    BeginInvoke((MethodInvoker)delegate
                    {
                        //dataGridView2.Update();
                        //dataGridView2.Refresh();
                        dataGridView2.Rows.Insert
            (0, new string[] {DateTime.Now.ToString("dd/MM/yyyy hh:mm tt"),
                                myTI.ToTitleCase(video_dict["title"]),
                                string.Format("{0}{1}", drive_space_letter, destinationFile) });
                    });
                }






               

                
            }
        }

        public string drive_letter(List<DriveInfo> drive_info)
        {
            foreach (DriveInfo drive in drive_info)
            {
                if (drive.TotalFreeSpace < 200000000000)
                {
                    continue;

                }
                else
                {
                    return drive.RootDirectory.ToString();
                }

            }
            return drive_info[0].RootDirectory.ToString();
        }


        public bool file_exists(string directory, List<DriveInfo> drive_info)
        {
            bool file_exists = false;


            foreach (DriveInfo drive in drive_info)
            {
                string file_path = string.Format(directory);
                if (File.Exists(file_path) == true)
                {
                    file_exists = true;
                }

               
            }
            return file_exists;
        }

        private static void processDirectory(string startLocation)
        {
            foreach (var directory in Directory.GetDirectories(startLocation))
            {
                Console.WriteLine(directory);
                processDirectory(directory);
                if (Directory.GetFiles(directory).Length == 0 &&
                    Directory.GetDirectories(directory).Length == 0)
                {
                    Directory.Delete(directory, false);
                }
            }
        }

        public void add_data(string time, string title, string destination)
        {
            bool tryAgain = true;
            DataRow newRow = dt.NewRow();
            newRow["Time"] = time;
            newRow["Title"] = title;
            newRow["Destination"] = destination;
            dt.Rows.Add(newRow);
            dataSet.Tables["Files"].AcceptChanges();
            while (tryAgain)
            {

                
                FileInfo fi1 = new FileInfo(@"MyData.xml");
                bool fileLocked = IsFileLocked(fi1);
                if (fileLocked == true)
                {
                    Thread.Sleep(100);

                }
                else
                {
                    try
                    {
                        
                        dataSet.WriteXml(@"MyData.xml");
                    }
                    catch (System.IO.IOException)
                    {
                        tryAgain = false;
                    }
                    tryAgain = false;
                }
                
            }

        }
        protected virtual bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        public void SearchFiles(object sender, EventArgs e)
        {
            // Delete empty dirs
            processDirectory(Properties.Settings.Default.download_dir);
            save_data();

            // Get all files in all dirs.
            DirectoryInfo d = new DirectoryInfo(Properties.Settings.Default.download_dir);
            FileInfo[] Files = d.GetFiles("*.*", SearchOption.AllDirectories);
                
            foreach (FileInfo file in Files)
            {   
                //Get file size and delete if too small
                try
                    {
                    long length = new FileInfo(file.FullName).Length;
                    if (length < 200000000)
                    {
                        try
                        {
                            MoveFile(file.FullName, string.Format("D:\\Deleted\\{0}",file.Name));
                            // File.Delete(file.FullName);
                            BeginInvoke((MethodInvoker)delegate
                            {
                                dataGridView2.Rows.Insert
                               (0, new string[] { DateTime.Now.ToString("dd/MM/yyyy hh:mm tt"),
                                   String.Format("Moved file to temp storage as it was less than 200mb"),
                                   string.Format("D:\\Deleted\\{0}",file.Name) });
                            });
                            continue;
                        }
                        catch (System.IO.IOException)
                        {
                            return;
                        }
                    }
                    else
                    {
                        // Create new background worker
                        var worker = new BackgroundWorker();
                        worker.DoWork += new DoWorkEventHandler(Regex);
                        worker.RunWorkerAsync(argument: file);
                    }
                    }

                catch (FileNotFoundException)
                    {
                        break;
                    }

            }


            
        }

     
        private void Form1_Load(object sender, EventArgs e)
        {
            //try
            //{
            //    dataSet.ReadXml(@"MyData.xml");
            //}
            //catch (System.IO.FileNotFoundException)
            //{

            //}


            //dt.Clear();
            //dt.Columns.Add("Time");
            //dt.Columns.Add("Title");
            //dt.Columns.Add("Destination");
            //try
            //{
            //    dataSet.Tables.Add(dt);
            //}
            //catch (System.Data.DuplicateNameException)
            //{

            //}
            //dataGridView2.DataSource = dataSet.Tables["Files"];



            myTimer.Tick += new EventHandler(SearchFiles);

            // Sets the timer interval to 15 seconds.
            myTimer.Interval = 15000;
            
            // Check if all setting are entered
            bool setting_missing;
            setting_missing = false;

            if (string.IsNullOrEmpty(Properties.Settings.Default.download_dir))
            {
                setting_missing = true;
            }
         

            // If all settings are entered start the timer.
            if(setting_missing == false)
                {
                    myTimer.Start();
                }
        }

        private void button_start(object sender, EventArgs e)
        {
            // Start timer in new thread.
            myTimer.Start();
        }

        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.textBox_download_dir = new System.Windows.Forms.TextBox();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.dataSet1 = new System.Data.DataSet();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(106, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Download DIR";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox_download_dir
            // 
            this.textBox_download_dir.Location = new System.Drawing.Point(124, 12);
            this.textBox_download_dir.Name = "textBox_download_dir";
            this.textBox_download_dir.Size = new System.Drawing.Size(237, 20);
            this.textBox_download_dir.TabIndex = 1;
            // 
            // dataGridView2
            // 
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3});
            this.dataGridView2.Location = new System.Drawing.Point(12, 120);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.Size = new System.Drawing.Size(1253, 207);
            this.dataGridView2.TabIndex = 2;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Time";
            this.Column1.Name = "Column1";
            this.Column1.Width = 200;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Title";
            this.Column2.Name = "Column2";
            this.Column2.Width = 400;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "Discription";
            this.Column3.Name = "Column3";
            this.Column3.Width = 600;
            // 
            // dataSet1
            // 
            this.dataSet1.DataSetName = "NewDataSet";
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(1277, 339);
            this.Controls.Add(this.dataGridView2);
            this.Controls.Add(this.textBox_download_dir);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

    

        private void button1_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    textBox_download_dir.Text = fbd.SelectedPath.ToString();
                    Properties.Settings.Default.download_dir = fbd.SelectedPath.ToString();
                    Properties.Settings.Default.Save();

                }
            }
        }
    }
    
}
