namespace TPXQohSync
{
    partial class ProjectInstaller
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
            this.XsScTPXQohSyncProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.XsScTPXQohSyncInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // XsScTPXQohSyncProcessInstaller
            // 
            this.XsScTPXQohSyncProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.XsScTPXQohSyncProcessInstaller.Password = null;
            this.XsScTPXQohSyncProcessInstaller.Username = null;
            // 
            // XsScTPXQohSyncInstaller
            // 
            this.XsScTPXQohSyncInstaller.ServiceName = "XsScTPXQohSync";
            this.XsScTPXQohSyncInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.XsScTPXQohSyncProcessInstaller,
            this.XsScTPXQohSyncInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller XsScTPXQohSyncProcessInstaller;
        private System.ServiceProcess.ServiceInstaller XsScTPXQohSyncInstaller;
    }
}