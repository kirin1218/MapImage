namespace MapImage
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnLoad = new System.Windows.Forms.Button();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.subView = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.x_label = new System.Windows.Forms.Label();
            this.y_label = new System.Windows.Forms.Label();
            this.w_label = new System.Windows.Forms.Label();
            this.h_label = new System.Windows.Forms.Label();
            this.KeyPointslistView = new System.Windows.Forms.ListView();
            this.button1 = new System.Windows.Forms.Button();
            this.buildings = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.subView)).BeginInit();
            this.SuspendLayout();
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(1035, 538);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 0;
            this.btnLoad.Text = "読み込み";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // pictureBox
            // 
            this.pictureBox.Location = new System.Drawing.Point(12, 12);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(632, 389);
            this.pictureBox.TabIndex = 1;
            this.pictureBox.TabStop = false;
            this.pictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.PictureBox_Paint);
            this.pictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PictureBox_MouseDown);
            // 
            // listView1
            // 
            this.listView1.Location = new System.Drawing.Point(883, 12);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(227, 520);
            this.listView1.TabIndex = 2;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.ListView1_SelectedIndexChanged_UsingItems);
            // 
            // subView
            // 
            this.subView.Location = new System.Drawing.Point(12, 407);
            this.subView.Name = "subView";
            this.subView.Size = new System.Drawing.Size(206, 125);
            this.subView.TabIndex = 3;
            this.subView.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(242, 419);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(14, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "X:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(242, 433);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(14, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "Y:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(242, 448);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "Width:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(242, 463);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 12);
            this.label4.TabIndex = 7;
            this.label4.Text = "Height:";
            // 
            // x_label
            // 
            this.x_label.AutoSize = true;
            this.x_label.Location = new System.Drawing.Point(298, 419);
            this.x_label.Name = "x_label";
            this.x_label.Size = new System.Drawing.Size(0, 12);
            this.x_label.TabIndex = 4;
            // 
            // y_label
            // 
            this.y_label.AutoSize = true;
            this.y_label.Location = new System.Drawing.Point(298, 433);
            this.y_label.Name = "y_label";
            this.y_label.Size = new System.Drawing.Size(0, 12);
            this.y_label.TabIndex = 5;
            // 
            // w_label
            // 
            this.w_label.AutoSize = true;
            this.w_label.Location = new System.Drawing.Point(298, 448);
            this.w_label.Name = "w_label";
            this.w_label.Size = new System.Drawing.Size(0, 12);
            this.w_label.TabIndex = 6;
            // 
            // h_label
            // 
            this.h_label.AutoSize = true;
            this.h_label.Location = new System.Drawing.Point(298, 463);
            this.h_label.Name = "h_label";
            this.h_label.Size = new System.Drawing.Size(0, 12);
            this.h_label.TabIndex = 7;
            // 
            // KeyPointslistView
            // 
            this.KeyPointslistView.Location = new System.Drawing.Point(650, 12);
            this.KeyPointslistView.Name = "KeyPointslistView";
            this.KeyPointslistView.Size = new System.Drawing.Size(227, 389);
            this.KeyPointslistView.TabIndex = 8;
            this.KeyPointslistView.UseCompatibleStateImageBehavior = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(329, 509);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 9;
            this.button1.Text = "json出力";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // buildings
            // 
            this.buildings.Location = new System.Drawing.Point(329, 473);
            this.buildings.Name = "buildings";
            this.buildings.Size = new System.Drawing.Size(75, 23);
            this.buildings.TabIndex = 10;
            this.buildings.Text = "buildings";
            this.buildings.UseVisualStyleBackColor = true;
            this.buildings.Click += new System.EventHandler(this.button2_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(410, 473);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 11;
            this.button2.Text = "roads";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(410, 509);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 12;
            this.button3.Text = "json出力2";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1122, 573);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.buildings);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.KeyPointslistView);
            this.Controls.Add(this.h_label);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.w_label);
            this.Controls.Add(this.y_label);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.x_label);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.subView);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.btnLoad);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.subView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.PictureBox subView;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label x_label;
        private System.Windows.Forms.Label y_label;
        private System.Windows.Forms.Label w_label;
        private System.Windows.Forms.Label h_label;
        private System.Windows.Forms.ListView KeyPointslistView;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button buildings;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
}

