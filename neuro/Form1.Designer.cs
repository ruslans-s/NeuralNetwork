
namespace neuro
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.button2 = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.файлToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.настройкиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.загрузитьВесыИзФайлаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.записатьВесыВФайлToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(2, 51);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(94, 55);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(102, 51);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(251, 420);
            this.listBox1.TabIndex = 1;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(376, 51);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(292, 219);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(2, 490);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(685, 23);
            this.progressBar1.TabIndex = 3;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(2, 112);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(94, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.файлToolStripMenuItem,
            this.настройкиToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(699, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // файлToolStripMenuItem
            // 
            this.файлToolStripMenuItem.Name = "файлToolStripMenuItem";
            this.файлToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.файлToolStripMenuItem.Text = "Файл";
            // 
            // настройкиToolStripMenuItem
            // 
            this.настройкиToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.загрузитьВесыИзФайлаToolStripMenuItem,
            this.записатьВесыВФайлToolStripMenuItem});
            this.настройкиToolStripMenuItem.Name = "настройкиToolStripMenuItem";
            this.настройкиToolStripMenuItem.Size = new System.Drawing.Size(79, 20);
            this.настройкиToolStripMenuItem.Text = "Настройки";
            // 
            // загрузитьВесыИзФайлаToolStripMenuItem
            // 
            this.загрузитьВесыИзФайлаToolStripMenuItem.Name = "загрузитьВесыИзФайлаToolStripMenuItem";
            this.загрузитьВесыИзФайлаToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.загрузитьВесыИзФайлаToolStripMenuItem.Text = "Загрузить весы из файла";
            this.загрузитьВесыИзФайлаToolStripMenuItem.Click += new System.EventHandler(this.загрузитьВесыИзФайлаToolStripMenuItem_Click);
            // 
            // записатьВесыВФайлToolStripMenuItem
            // 
            this.записатьВесыВФайлToolStripMenuItem.Name = "записатьВесыВФайлToolStripMenuItem";
            this.записатьВесыВФайлToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.записатьВесыВФайлToolStripMenuItem.Text = "Записать весы в файл";
            this.записатьВесыВФайлToolStripMenuItem.Click += new System.EventHandler(this.записатьВесыВФайлToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(699, 524);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem файлToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem настройкиToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem загрузитьВесыИзФайлаToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem записатьВесыВФайлToolStripMenuItem;
    }
}

