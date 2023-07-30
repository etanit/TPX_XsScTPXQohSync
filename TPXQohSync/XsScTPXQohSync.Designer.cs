namespace TPXQohSync
{
    partial class XsScTPXQohSync
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.RestartTimer = new System.Timers.Timer();
            this.Checktimer = new System.Timers.Timer();
            ((System.ComponentModel.ISupportInitialize)(this.RestartTimer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Checktimer)).BeginInit();
            // 
            // RestartTimer
            // 
            this.RestartTimer.Interval = 1000D;
            this.RestartTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.RestartTimer_Elapsed);
            // 
            // Checktimer
            // 
            this.Checktimer.Interval = 7200000D;
            this.Checktimer.Elapsed += new System.Timers.ElapsedEventHandler(this.Checktimer_Elapsed);
            // 
            // XsScTPXQohSync
            // 
            this.ServiceName = "XsScTPXQohSync";
            ((System.ComponentModel.ISupportInitialize)(this.RestartTimer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Checktimer)).EndInit();

        }

        #endregion

        private System.Timers.Timer RestartTimer;
        private System.Timers.Timer Checktimer;
    }
}
