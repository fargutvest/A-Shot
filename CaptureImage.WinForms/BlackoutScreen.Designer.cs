﻿
namespace CaptureImage.WinForms
{
    partial class BlackoutScreen
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
            this.SuspendLayout();
            // 
            // BlackoutScreen
            // 
            this.ClientSize = new System.Drawing.Size(300, 300);
            this.KeyPreview = true;
            this.Name = "BlackoutScreen";
            this.Text = "BlackoutScreen";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.BlackoutScreen_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.BlackoutScreen_KeyUp);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(BlackoutScreen_KeyPress);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BlackoutScreen_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.BlackoutScreen_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.BlackoutScreen_MouseUp);
            this.MouseWheel += BlackoutScreen_MouseWheel;
            this.ResumeLayout(false);

        }




        #endregion
    }
}