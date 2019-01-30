namespace Torrent_Sorter
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button_choose_download_folder = new System.Windows.Forms.Button();
            this.label_choose_dir = new System.Windows.Forms.Label();
            this.textBox_download_dir = new System.Windows.Forms.TextBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Destination = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_movie_dir = new System.Windows.Forms.TextBox();
            this.button_choose_tv_folder = new System.Windows.Forms.Button();
            this.textBox_tv_dir = new System.Windows.Forms.TextBox();
            this.button_choose_movie_folder = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.button3 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // button_choose_download_folder
            // 
            this.button_choose_download_folder.Location = new System.Drawing.Point(142, 6);
            this.button_choose_download_folder.Margin = new System.Windows.Forms.Padding(1);
            this.button_choose_download_folder.Name = "button_choose_download_folder";
            this.button_choose_download_folder.Size = new System.Drawing.Size(119, 28);
            this.button_choose_download_folder.TabIndex = 0;
            this.button_choose_download_folder.Text = "Folder";
            this.button_choose_download_folder.UseVisualStyleBackColor = true;
            this.button_choose_download_folder.Click += new System.EventHandler(this.button1_Click);
            // 
            // label_choose_dir
            // 
            this.label_choose_dir.AutoSize = true;
            this.label_choose_dir.Location = new System.Drawing.Point(4, 14);
            this.label_choose_dir.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label_choose_dir.Name = "label_choose_dir";
            this.label_choose_dir.Size = new System.Drawing.Size(135, 13);
            this.label_choose_dir.TabIndex = 1;
            this.label_choose_dir.Text = "Choose download directory";
            // 
            // textBox_download_dir
            // 
            this.textBox_download_dir.Location = new System.Drawing.Point(272, 14);
            this.textBox_download_dir.Margin = new System.Windows.Forms.Padding(1);
            this.textBox_download_dir.Name = "textBox_download_dir";
            this.textBox_download_dir.Size = new System.Drawing.Size(302, 20);
            this.textBox_download_dir.TabIndex = 2;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Time,
            this.Title,
            this.Destination});
            this.dataGridView1.Location = new System.Drawing.Point(135, 110);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(1);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 40;
            this.dataGridView1.Size = new System.Drawing.Size(828, 315);
            this.dataGridView1.TabIndex = 3;
            // 
            // Time
            // 
            this.Time.HeaderText = "Time";
            this.Time.Name = "Time";
            // 
            // Title
            // 
            this.Title.HeaderText = "Title";
            this.Title.Name = "Title";
            this.Title.Width = 250;
            // 
            // Destination
            // 
            this.Destination.HeaderText = "Destination";
            this.Destination.Name = "Destination";
            this.Destination.Width = 430;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 44);
            this.label1.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Choose TV directory";
            // 
            // textBox_movie_dir
            // 
            this.textBox_movie_dir.Location = new System.Drawing.Point(272, 73);
            this.textBox_movie_dir.Margin = new System.Windows.Forms.Padding(1);
            this.textBox_movie_dir.Name = "textBox_movie_dir";
            this.textBox_movie_dir.Size = new System.Drawing.Size(302, 20);
            this.textBox_movie_dir.TabIndex = 6;
            // 
            // button_choose_tv_folder
            // 
            this.button_choose_tv_folder.Location = new System.Drawing.Point(142, 36);
            this.button_choose_tv_folder.Margin = new System.Windows.Forms.Padding(1);
            this.button_choose_tv_folder.Name = "button_choose_tv_folder";
            this.button_choose_tv_folder.Size = new System.Drawing.Size(119, 28);
            this.button_choose_tv_folder.TabIndex = 5;
            this.button_choose_tv_folder.Text = "Folder";
            this.button_choose_tv_folder.UseVisualStyleBackColor = true;
            this.button_choose_tv_folder.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // textBox_tv_dir
            // 
            this.textBox_tv_dir.Location = new System.Drawing.Point(272, 44);
            this.textBox_tv_dir.Margin = new System.Windows.Forms.Padding(1);
            this.textBox_tv_dir.Name = "textBox_tv_dir";
            this.textBox_tv_dir.Size = new System.Drawing.Size(302, 20);
            this.textBox_tv_dir.TabIndex = 9;
            // 
            // button_choose_movie_folder
            // 
            this.button_choose_movie_folder.Location = new System.Drawing.Point(142, 67);
            this.button_choose_movie_folder.Margin = new System.Windows.Forms.Padding(1);
            this.button_choose_movie_folder.Name = "button_choose_movie_folder";
            this.button_choose_movie_folder.Size = new System.Drawing.Size(119, 32);
            this.button_choose_movie_folder.TabIndex = 8;
            this.button_choose_movie_folder.Text = "Folder";
            this.button_choose_movie_folder.UseVisualStyleBackColor = true;
            this.button_choose_movie_folder.Click += new System.EventHandler(this.button2_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 76);
            this.label2.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(118, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Choose Movie directory";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(6, 398);
            this.button3.Margin = new System.Windows.Forms.Padding(2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(76, 27);
            this.button3.TabIndex = 10;
            this.button3.Text = "Start";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click_1);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(962, 431);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.textBox_tv_dir);
            this.Controls.Add(this.button_choose_movie_folder);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_movie_dir);
            this.Controls.Add(this.button_choose_tv_folder);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.textBox_download_dir);
            this.Controls.Add(this.label_choose_dir);
            this.Controls.Add(this.button_choose_download_folder);
            this.Margin = new System.Windows.Forms.Padding(1);
            this.Name = "Form1";
            this.Text = "Torrent-Sorter";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_choose_download_folder;
        private System.Windows.Forms.Label label_choose_dir;
        private System.Windows.Forms.TextBox textBox_download_dir;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_movie_dir;
        private System.Windows.Forms.Button button_choose_tv_folder;
        private System.Windows.Forms.TextBox textBox_tv_dir;
        private System.Windows.Forms.Button button_choose_movie_folder;
        private System.Windows.Forms.Label label2;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Time;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title;
        private System.Windows.Forms.DataGridViewTextBoxColumn Destination;
    }
}

