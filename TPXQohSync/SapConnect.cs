using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace TPXQohSync
{
    public class SapConnect
    {
        public SAPbobsCOM.Company oCompany;
        public bool IsConnected;
        public bool RestartApp;
        public int NumberOfRetries = 5;
        public int DelayOnRetry = 1000;
        private DataConfig _DataConfig;

        public SapConnect(DataConfig DCfg)
        {
            this._DataConfig = DCfg;
            this.ConnectToCompany();
        }
        public void ConnectToCompany()
        {
            try
            {
                string sErrMsg = "";
                int lErrCode = 0;
                //int lRetCode=0;

                this.oCompany = new SAPbobsCOM.Company();

                this.oCompany.Server = _DataConfig.SqlServer;
                this.oCompany.DbUserName = _DataConfig.SqlUser; //"sa"
                this.oCompany.DbPassword = _DataConfig.SqlPassword; //"sqlpassword"
                this.oCompany.UseTrusted = false;
                this.oCompany.CompanyDB = _DataConfig.SqlDataBase;// "SBODemoAU"
                this.oCompany.UserName = _DataConfig.SAP_UserName;//"manager"
                this.oCompany.Password = _DataConfig.SAP_Password;//"password"
                this.oCompany.language = _DataConfig.SAP_language;//SAPbobsCOM.BoSuppLangs.ln_English
                this.oCompany.DbServerType = _DataConfig.SAP_DbServerType;// SAPbobsCOM.BoDataServerTypes.dst_MSSQL2012
                this.oCompany.LicenseServer = _DataConfig.SAP_LicenseServer;// "SAP2015:30000"

                this.oCompany.Connect();
                // Check for errors during connect
                this.oCompany.GetLastError(out lErrCode, out sErrMsg);
                if (lErrCode == -14017 || lErrCode == -10 || lErrCode == 1200 || lErrCode == -1200 || lErrCode == -2004) //Session is killed by manager in SAP
                {
                    WriteEventLog("The function ConnectToCompany return this error: SAP Connection probleme : " + lErrCode.ToString() + ":" + sErrMsg);
                    RestartApp = true;
                    IsConnected = false;
                }
                else if (lErrCode != 0)
                {
                    WriteEventLog("The function ConnectToCompany return this error: SAP Connection probleme : " + lErrCode.ToString() + ":" + sErrMsg);
                    IsConnected = false;
                }
            }
            catch (Exception ex)
            {
                WriteEventLog("The function " + GetCurrentMethod() + " return this error:" + ex.Message);
            }
        }



        private void WriteEventLog(string Errmsg)
        {
            try
            {
                // Create the source and log, if it does not already exist.
                if (!EventLog.SourceExists("TPXQohSync"))
                {
                    EventLog.CreateEventSource("TPXQohSync", "TPXQohSyncLog");
                }
                // Create an EventLog instance and assign its source.
                EventLog eventLog = new EventLog();
                // Setting the source
                eventLog.Source = "TPXQohSync";
                // Write an entry to the event log.
                eventLog.WriteEntry(Errmsg, EventLogEntryType.Error, 1003);
            }
            catch { }
        }


        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetCurrentMethod()
        {
            try
            {
                var st = new StackTrace();
                var sf = st.GetFrame(1);

                return sf.GetMethod().Name;
            }
            catch (Exception ex)
            {
                WriteEventLog("The function GetCurrentMethod return this error:" + ex.Message);
                return "NonIdentifiedFunction";
            }
        }
    }
}
