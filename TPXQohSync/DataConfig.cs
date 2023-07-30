using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IniParser;
using IniParser.Model;

namespace TPXQohSync
{
    public class DataConfig
    {
        [Category("SQL Server")]
        [Description("Name of Sql Server")]
        [DisplayName("SQL Server")]
        public string SqlServer { get; set; }
        [Category("SQL Server")]
        [Description("SQL Server username")]
        [DisplayName("User Name")]
        public string SqlUser { get; set; }
        [Category("SQL Server")]
        [Description("SQL Server password")]
        [DisplayName("Password")]
        [PasswordPropertyText(true)]
        public string SqlPassword { get; set; }
        [Category("SQL Server")]
        [Description("SQL Server database")]
        [DisplayName("DataBase")]
        public string SqlDataBase { get; set; }
        [Category("Misc")]
        [Description("Wait time in second in case we have a timeout issue")]
        [DisplayName("Wait at timeout")]
        public int WaitATimeout { get; set; }
        [Category("SAP")]
        [Description("SAP UserName")]
        [DisplayName("UserName")]
        public string SAP_UserName { get; set; }
        [Category("SAP")]
        [Description("SAP Password")]
        [DisplayName("Password")]
        [PasswordPropertyText(true)]
        public string SAP_Password { get; set; }
        [Category("SAP")]
        [Description("SAP Language int value:3 English (United States), 6 English (Singapore) ,8 English (United Kingdom)  ")]
        [DisplayName("SAP Language")]
        public SAPbobsCOM.BoSuppLangs SAP_language { get; set; }
        [Category("SAP")]
        [Description("SAP Database Server type: 6 MSSQL2008,7 MSSQL2012,8 MSSQL2014,9 HANADB")]
        [DisplayName("Server type")]
        public SAPbobsCOM.BoDataServerTypes SAP_DbServerType { get; set; }
        [Category("SAP")]
        [Description("SAP License Server: servername:portnumber")]
        [DisplayName("License Server")]
        public string SAP_LicenseServer { get; set; }
        [Category("Misc")]
        [Description("Daily time to restart service")]
        [DisplayName("Daily restart time")]
        public string Daily_Restart_Time { get; set; }

        [Category("Misc")]
        [Description("URL of Web Server that conatin PHP script for syncing ")]
        [DisplayName("URL Web Server")]
        public string URL { get; set; }

        [Category("Misc")]
        [Description("Specify the number of transaction to switch to bulk mode: default 100")]
        [DisplayName("Transaction Number to Bulk mode")]
        public int BulkTransactionNumber { get; set; }

        [Category("Email Notification")]
        [Description("SMTP Server")]
        [DisplayName("SMTP Server")]
        public string SMTP_Server { get; set; }
        [Category("Email Notification")]
        [Description("SMTP Port Number")]
        [DisplayName("SMTP Port Number")]
        public int SMTP_Port_Number { get; set; }
        [Category("Email Notification")]
        [Description("SMTP Username")]
        [DisplayName("SMTP Username")]
        public string SMTP_Username { get; set; }
        [Category("Email Notification")]
        [Description("SMTP Password")]
        [DisplayName("SMTP Password")]
        [PasswordPropertyText(true)]
        public string SMTP_Password { get; set; }
        [Category("Email Notification")]
        [Description("Email From")]
        [DisplayName("Email From")]
        public string Email_From { get; set; }
        [Category("Email Notification")]
        [Description("Email To")]
        [DisplayName("Email To")]
        public string Email_To { get; set; }
        [Category("Email Notification")]
        [Description("Email Subject")]
        [DisplayName("Email Subject")]
        public string Email_Subject { get; set; }


        public string iniFile = @"C:\XSD\XSD_Version\App\QOHSync\config.ini";

        public DataConfig()
        {
            try
            {

                //Create an instance of a ini file parser
                FileIniDataParser fileIniData = new FileIniDataParser();
                fileIniData.Parser.Configuration.CommentString = "#";
                //Parse the ini file
                IniData parsedData = fileIniData.ReadFile(iniFile);

                SqlServer = parsedData["Configuration"]["SqlServer"];
                SqlUser = parsedData["Configuration"]["User"];
                SqlPassword = parsedData["Configuration"]["Password"];
                SqlDataBase = parsedData["Configuration"]["DataBase"];
                WaitATimeout = Convert.ToInt16(parsedData["Configuration"]["WaitATimeout"]);
                URL = parsedData["Configuration"]["URL"];
                SAP_UserName = parsedData["Configuration"]["SAP_UserName"];
                SAP_Password = parsedData["Configuration"]["SAP_Password"];
                SAP_language = (SAPbobsCOM.BoSuppLangs)(Convert.ToInt16(parsedData["Configuration"]["SAP_language"]));
                SAP_DbServerType = (SAPbobsCOM.BoDataServerTypes)(Convert.ToInt16(parsedData["Configuration"]["SAP_DbServerType"]));
                SAP_LicenseServer = parsedData["Configuration"]["SAP_LicenseServe"];
                Daily_Restart_Time = parsedData["Configuration"]["Daily_Restart_Time"];
                BulkTransactionNumber = Convert.ToInt16(parsedData["Configuration"]["BulkTransactionNumber"]);

                SMTP_Server = parsedData["Configuration"]["SMTP_Server"];
                SMTP_Port_Number = Convert.ToInt16(parsedData["Configuration"]["SMTP_Port_Number"]);
                SMTP_Username = parsedData["Configuration"]["SMTP_Username"];
                SMTP_Password = parsedData["Configuration"]["SMTP_Password"];
                Email_From = parsedData["Configuration"]["Email_From"];
                Email_To = parsedData["Configuration"]["Email_To"];
                Email_Subject = parsedData["Configuration"]["Email_Subject"];
            }
            catch (Exception ex)
            {
                WriteEventLog("The function " + GetCurrentMethod() + " return this error:" + ex.Message);
            }
        }
        public void SaveIniFile()
        {
            try
            {
                var parser = new FileIniDataParser();
                // Modify the loaded ini file
                FileIniDataParser fileIniData = new FileIniDataParser();
                IniData parsedData = fileIniData.ReadFile(iniFile);

                parsedData["Configuration"]["SqlServer"] = this.SqlServer;
                parsedData["Configuration"]["User"] = this.SqlUser;
                parsedData["Configuration"]["Password"] = this.SqlPassword;
                parsedData["Configuration"]["DataBase"] = this.SqlDataBase;
                parsedData["Configuration"]["WaitATimeout"] = this.WaitATimeout.ToString();
                parsedData["Configuration"]["URL"] = this.URL;

                parsedData["Configuration"]["SAP_UserName"] = this.SAP_UserName;
                parsedData["Configuration"]["SAP_Password"] = this.SAP_Password;
                parsedData["Configuration"]["SAP_language"] = ((int)(this.SAP_language)).ToString();
                parsedData["Configuration"]["SAP_DbServerType"] = ((int)(this.SAP_DbServerType)).ToString();
                parsedData["Configuration"]["SAP_LicenseServe"] = this.SAP_LicenseServer;
                parsedData["Configuration"]["Daily_Restart_Time"] = this.Daily_Restart_Time;
                parsedData["Configuration"]["BulkTransactionNumber"] = this.BulkTransactionNumber.ToString();

                parsedData["Configuration"]["SMTP_Server"] = this.SMTP_Server;
                parsedData["Configuration"]["SMTP_Port_Number"] = this.SMTP_Port_Number.ToString();
                parsedData["Configuration"]["SMTP_Username"] = this.SMTP_Username;
                parsedData["Configuration"]["SMTP_Password"] = this.SMTP_Password;
                parsedData["Configuration"]["Email_From"] = this.Email_From;
                parsedData["Configuration"]["Email_To"] = this.Email_To;
                parsedData["Configuration"]["Email_Subject"] = this.Email_Subject;

                parser.WriteFile(iniFile, parsedData);
                //MessageBox.Show("Restart Service to apply changes", "Config", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                eventLog.WriteEntry(Errmsg, EventLogEntryType.Error, 1012);
            }
            catch { }
        }


        private string GetCurrentMethod()
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
