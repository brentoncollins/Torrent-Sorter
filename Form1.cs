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
        private string destinationFile;

        static System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();


        public Form1()
        {
            InitializeComponent();

            // Get the origianl dirs from settings.
            textBox_download_dir.Text = Properties.Settings.Default.download_dir;
            textBox_tv_dir.Text = Properties.Settings.Default.tv_dir;
            textBox_movie_dir.Text = Properties.Settings.Default.movie_dir;
        }

        public void Regex(object sender, DoWorkEventArgs e)

        {
            System.Threading.Thread.Sleep(1000);
            FileInfo fileInfo = (FileInfo) e.Argument;
            TextInfo myTI = new CultureInfo("en-US", false).TextInfo;

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


       

            Thread.Sleep(1000);
            string sourceFile = fileInfo.FullName;
            // If the result does not have title it is a movie.
            if (video_dict.ContainsKey("season") == false)
            {

                if (video_dict.ContainsKey("year"))
                {
                    destinationFile = String.Format
                        ("{0}\\{1}\\{2} - {3}{4}{5}",
                        Properties.Settings.Default.movie_dir,
                        myTI.ToTitleCase(video_dict["title"]),
                       myTI.ToTitleCase(video_dict["title"]),
                        video_dict["year"],
                        ".",
                        video_dict["container"]);
                }
                else
                {
                    destinationFile = String.Format
                            ("{0}\\{1}\\{2}{3}{4}",
                            Properties.Settings.Default.movie_dir,
                            myTI.ToTitleCase(video_dict["title"]),
                            myTI.ToTitleCase(video_dict["title"]),
                            ".",
                            video_dict["container"]);
                }

                Directory.CreateDirectory(String.Format("{0}\\{1}",
                Properties.Settings.Default.movie_dir,
                myTI.ToTitleCase(video_dict["title"])));



                if (File.Exists(destinationFile) == true)
                {


                    try
                    {
                        File.Delete(sourceFile);
                    }
                    catch (System.IO.DirectoryNotFoundException)
                    {
                        return;
                    }
                    catch (System.IO.IOException)
                    {
                        return;
                    }
                    BeginInvoke((MethodInvoker)delegate
                    {
                        dataGridView1.Rows.Insert
                (0, new string[] { DateTime.Now.ToString("h:mm:ss tt"),
                        myTI.ToTitleCase(video_dict["title"]),
                        "File Already Exists, File Removed" });

                    });
                }

                else
                {
                    File.Move(sourceFile, destinationFile);
                    Console.WriteLine("Moved File");

                    BeginInvoke((MethodInvoker)delegate
                    {
                        dataGridView1.Rows.Insert
                (0, new string[] { DateTime.Now.ToString("h:mm:ss tt"),
                        myTI.ToTitleCase(video_dict["title"]),
                        destinationFile });

                    });
                }






            }
            // If the result has an episode it is a TV show.
            if (video_dict.ContainsKey("episode") == true)
            {
                Directory.CreateDirectory
                (String.Format("{0}\\{1}",
                Properties.Settings.Default.tv_dir,
                myTI.ToTitleCase(video_dict["title"])));

                Directory.CreateDirectory
                (String.Format("{0}\\{1}\\Season {2}",
                Properties.Settings.Default.tv_dir,
                myTI.ToTitleCase(video_dict["title"]),
                video_dict["season"]));

                if (video_dict["episode"].Count() < 2)
                {

                    video_dict["episode"] = string.Format("{0}{1}", 0, video_dict["episode"]);

                }

                if (video_dict.ContainsKey("year") == true)
                {
                    destinationFile = String.Format
                            ("{0}\\{1}\\{2} - {3}{4}{5}",
                            Properties.Settings.Default.tv_dir,
                            myTI.ToTitleCase(video_dict["title"]),
                            myTI.ToTitleCase(video_dict["title"]),
                            video_dict["year"],
                            ".",
                            video_dict["container"]);
                }
                else
                {
                    destinationFile = String.Format
                            ("{0}\\{1}\\Season {2}\\{3} S{4}E{5}{6}{7}",
                            Properties.Settings.Default.tv_dir,
                            myTI.ToTitleCase(video_dict["title"]),
                            video_dict["season"],
                            myTI.ToTitleCase(video_dict["title"]),
                            video_dict["season"],
                            myTI.ToTitleCase(video_dict["episode"]),
                            ".", video_dict["container"]);
                }

                if (File.Exists(destinationFile) == true)
                {

                    try
                    {
                        File.Delete(sourceFile);
                    }
                    catch (System.IO.DirectoryNotFoundException)
                    {
                        return;
                    }

                    catch (System.IO.IOException)
                    {
                        return;
                    }

                    BeginInvoke((MethodInvoker)delegate
                    {
                        dataGridView1.Rows.Insert
            (0, new string[] { DateTime.Now.ToString("h:mm:ss tt"),
                        String.Format("{0} S{1}E{2}",
                        myTI.ToTitleCase(video_dict["title"]),
                        video_dict["season"],
                        video_dict["episode"]),
                        "File Already Exists, File Removed" });
                });
            }
                else
                {
                    File.Move(sourceFile, destinationFile);
                    Console.WriteLine("Moved File");
                    BeginInvoke((MethodInvoker)delegate
                    {
                        dataGridView1.Rows.Insert
                    (0, new string[] { DateTime.Now.ToString("h:mm:ss tt"),
                        String.Format("{0} S{1}E{2}",
                        myTI.ToTitleCase(video_dict["title"]),
                        video_dict["season"],
                        video_dict["episode"]),
                        destinationFile });
            });
        }


               



            }
        }





        protected virtual bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.OpenWrite();
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

        public bool HasFiles(string folderData)
        {
            DirectoryInfo d = new DirectoryInfo(folderData);
            return d.GetFiles("*.*").Any();
        }

        public bool FolderHandler(FileInfo fileData)
        {
            DirectoryInfo d = new DirectoryInfo(fileData.DirectoryName);
            FileInfo[] Files = d.GetFiles("*.*", SearchOption.AllDirectories);

            foreach (FileInfo file in Files)
            {

                //Get file size and delete if too small
                long length = new FileInfo(file.FullName).Length;

                if (length > 10000000)
                {
                    return true;
                }

                return false;

            }
            return false;
        }

        public static void DeleteDirectory(string path)
        {
            foreach (string directory in Directory.GetDirectories(path))
            {
                DeleteDirectory(directory);
            }

            try
            {
                Directory.Delete(path, true);
            }
            catch (IOException)
            {
                Directory.Delete(path, true);
            }
            catch (UnauthorizedAccessException)
            {
                Directory.Delete(path, true);
            }
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

        public void SearchFiles(object sender, EventArgs e)
        {

                
             
                // Get all files in all dirs.
                DirectoryInfo d = new DirectoryInfo(Properties.Settings.Default.download_dir);
                FileInfo[] Files = d.GetFiles("*.*", SearchOption.AllDirectories);

                foreach (FileInfo file in Files)
                {
                    try
                    {
                        //Get file size and delete if too small
                        long length = new FileInfo(file.FullName).Length;

                        if (length < 10000000)
                        {
                            File.Delete(file.FullName);
                            continue;
                        }
                    var worker = new BackgroundWorker();
                    worker.DoWork += new DoWorkEventHandler(Regex);

                    worker.RunWorkerAsync(argument: file);
                    Thread.Sleep(1000);

                    
                }

                    catch (FileNotFoundException)
                    {
                        break;

                    }
                
                processDirectory(Properties.Settings.Default.download_dir);
                

            }
        }



        private void button_download_dir(object sender, EventArgs e)
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

        private void button_tv_dir(object sender, EventArgs e)
        {

            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    textBox_tv_dir.Text = fbd.SelectedPath.ToString();
                    Properties.Settings.Default.tv_dir = fbd.SelectedPath.ToString();
                    Properties.Settings.Default.Save();

                }
            }
        }

        private void button_movie_dir(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    textBox_movie_dir.Text = fbd.SelectedPath.ToString();
                    Properties.Settings.Default.movie_dir = fbd.SelectedPath.ToString();
                    Properties.Settings.Default.Save();

                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            bool setting_missing;
            setting_missing = false;

            if (string.IsNullOrEmpty(Properties.Settings.Default.download_dir))
            {
                setting_missing = true;
            }
            if (string.IsNullOrEmpty(Properties.Settings.Default.tv_dir))
            {
                setting_missing = true;
            }
            if (string.IsNullOrEmpty(Properties.Settings.Default.movie_dir))
            {
                setting_missing = true;
            }




            myTimer.Tick += new EventHandler(SearchFiles);

            // Sets the timer interval to 5 seconds.
            myTimer.Interval = 5000;
            myTimer.Start();




        }

        //new Thread(() =>
        //{
        //    Thread.CurrentThread.IsBackground = true;
        //    SearchFiles();
        //    Console.WriteLine("Hello, world");
        //}).Start();

        // Start timer in new thread.






        private void button_start(object sender, EventArgs e)
        {

            // Start timer in new thread.
            //var startTimeSpan = TimeSpan.Zero;
            //var periodTimeSpan = TimeSpan.FromSeconds(20);

            //var timer = new System.Threading.Timer((x) =>
            //{

            //    SearchFiles();
            //}, null, startTimeSpan, periodTimeSpan);
        }

    }
    
}
