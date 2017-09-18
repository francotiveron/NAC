namespace FuzzyTest {
    partial class Form1 {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.m_chart = new FuzzyTest.Class1();
            this.SuspendLayout();
            // 
            // m_chart
            // 
            this.m_chart.Location = new System.Drawing.Point(0, 0);
            this.m_chart.Name = "m_chart";
            this.m_chart.Size = new System.Drawing.Size(384, 251);
            this.m_chart.TabIndex = 0;
            this.m_chart.Text = "class11";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(635, 386);
            this.Controls.Add(this.m_chart);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private Class1 m_chart;
    }
}

