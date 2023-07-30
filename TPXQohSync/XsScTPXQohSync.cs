using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace TPXQohSync
{
    public partial class XsScTPXQohSync : ServiceBase
    {
        public XsScTPXQohSync()
        {
            InitializeComponent();
        }
        string _SqlServer = "";
        string _DataBase = "";
        int _WaitATimeout = 0;
        string _Daily_Restart_Time = "";
        string _URL = "";
        int _BulkTransactionNumber = 0;


        public string con = "";
        public Boolean DoesUserHavePermission = true;
        //static public List<string> state = new List<string>();
        public Boolean isFirstRun = true;
        public Boolean isTimeoutWaiting = false;
        static public SqlDependency dep;
        static public int ii = 0;
        public DataConfig _DataConfig = new DataConfig();
        public SapConnect SapConnection;

        public void onDebug()
        {
            OnStart(null);
        }
        protected override void OnStart(string[] args)
        {
            SapConnection = new SapConnect(_DataConfig);
            _SqlServer = _DataConfig.SqlServer;
            _DataBase = _DataConfig.SqlDataBase;
            _WaitATimeout = Convert.ToInt16(_DataConfig.WaitATimeout) * 1000;
            _URL = _DataConfig.URL;
            _Daily_Restart_Time = _DataConfig.Daily_Restart_Time;
            _BulkTransactionNumber = Convert.ToInt16(_DataConfig.BulkTransactionNumber);

            this.con = @"Data Source=" + _SqlServer + ";Initial Catalog=TPXLive;User ID=sa;Password=mswtmcs";

            Checktimer.Enabled = true;
            Checktimer.Start();
            GC.KeepAlive(Checktimer);

            RestartTimer.Enabled = true;
            RestartTimer.Start();
            GC.KeepAlive(RestartTimer);
            //start Syncing 
            TimerCheckChange();
        }

        protected override void OnStop()
        {
        }

        private string UpdateQOHinSAP(string TpxItemcode, string MswtItemCode, int qty)
        {
            SAPbobsCOM.AlternateCatNum oItem;


            string ErrMsg = "";
            int RetVal;
            int ErrCode;
            string Ret = "";


            

            try
            {
                //Reconnect if we lost connection to SAP B1
                if (SapConnection.oCompany.Connected == false)
                {
                    SapConnection.ConnectToCompany();
                }
                oItem = (SAPbobsCOM.AlternateCatNum)SapConnection.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oAlternateCatNum);

                if (oItem.GetByKey(TpxItemcode,"SAMJAS",MswtItemCode))
                {
                    oItem.UserFields.Fields.Item("U_QOHSupplier").Value = qty;
                   


                    RetVal = oItem.Update();
                    if (RetVal != 0)
                    {
                        ErrMsg = "";
                        SapConnection.oCompany.GetLastError(out ErrCode, out ErrMsg);
                        WriteEventLog("The function UpdateQOHinSAP for Item:" + TpxItemcode + " return this error:Failed to Update Item. SAP error code:" + ErrCode.ToString() + " " + ErrMsg);
                        Ret = "The function UpdateQOHinSAP for Item:" + TpxItemcode + " return this error:Failed to Update Item. SAP error code:" + ErrCode.ToString() + " " + ErrMsg;


                    }
                    else
                    {
                        //SapConnection.oCompany.GetNewObjectCode(out docNum);                
                        Ret = "ok";
                    }
                }
                else
                {
                    WriteEventLog("The function " + GetCurrentMethod() + " Can't found this Item in SAP:" + TpxItemcode);
                    Ret = "The function " + GetCurrentMethod() + " Can't found this Item in SAP:" + TpxItemcode;
                }
                return Ret;
            }
            catch (Exception ex)
            {
                WriteEventLog("The function " + GetCurrentMethod() + " return this error:" + ex.Message);
                return "The function " + GetCurrentMethod() + " return this error:" + ex.Message;

            }
            finally
            {
                if (SapConnection.RestartApp)
                {
                    try
                    {
                        Environment.Exit(1);
                    }
                    catch { }
                }
                oItem = null;
                GC.Collect();
            }
        }

        private string UpdateSumQOHinSAP(string TpxItemcode , int qty)
        {
            SAPbobsCOM.Items  oItem;


            string ErrMsg = "";
            int RetVal;
            int ErrCode;
            string Ret = "";




            try
            {
                //Reconnect if we lost connection to SAP B1
                if (SapConnection.oCompany.Connected == false)
                {
                    SapConnection.ConnectToCompany();
                }
                oItem = (SAPbobsCOM.Items)SapConnection.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems);

                if (oItem.GetByKey(TpxItemcode))
                {
                    oItem.UserFields.Fields.Item("U_QOHSupplier").Value = qty.ToString();



                    RetVal = oItem.Update();
                    if (RetVal != 0)
                    {
                        ErrMsg = "";
                        SapConnection.oCompany.GetLastError(out ErrCode, out ErrMsg);
                        WriteEventLog("The function UpdateSumQOHinSAP for Item:" + TpxItemcode + " return this error:Failed to Update Item. SAP error code:" + ErrCode.ToString() + " " + ErrMsg);
                        Ret = "The function UpdateSumQOHinSAP for Item:" + TpxItemcode + " return this error:Failed to Update Item. SAP error code:" + ErrCode.ToString() + " " + ErrMsg;


                    }
                    else
                    {
                        //SapConnection.oCompany.GetNewObjectCode(out docNum);                
                        Ret = "ok";
                    }
                }
                else
                {
                    WriteEventLog("The function " + GetCurrentMethod() + " Can't found this Item in SAP:" + TpxItemcode);
                    Ret = "The function " + GetCurrentMethod() + " Can't found this Item in SAP:" + TpxItemcode;
                }
                return Ret;
            }
            catch (Exception ex)
            {
                WriteEventLog("The function " + GetCurrentMethod() + " return this error:" + ex.Message);
                return "The function " + GetCurrentMethod() + " return this error:" + ex.Message;

            }
            finally
            {
                if (SapConnection.RestartApp)
                {
                    try
                    {
                        Environment.Exit(1);
                    }
                    catch { }
                }
                oItem = null;
                GC.Collect();
            }
        }



        private void UpdateBpcatalogQoH()
        {
            string Ret = "";
            string sSQL = "select OSCN.ItemCode,OSCN.Substitute,OSCN.U_QOHSupplier,(case when isnull(Mswt.MswtQoH,0)<0 then 0 else isnull(Mswt.MswtQoH,0) end) as MswtQoH from OSCN ";
            sSQL += "left join OITM on OSCN.ItemCode=OITM.ItemCode and OSCN.Substitute=OITM.U_MSWTCode and OSCN.CardCode='SAMJAS' ";
            sSQL += "left join (select   ItemCode,isnull((isnull(OnHand,0)-isnull(IsCommited,0)+isnull(U_QOHSupplier,0)),0) AS MswtQoH  from [MSWT_Live].dbo.OITM) As Mswt on  ";
            sSQL += "Mswt.ItemCode=OSCN.Substitute where OSCN.U_QOHSupplier<>(case when isnull(Mswt.MswtQoH,0)<0 then 0 else isnull(Mswt.MswtQoH,0) end)   ";
            //sSQL += " or isnull(OITM.U_QOHSupplier,0) <> isnull(OSCN.U_QOHSupplier,0) ";

            CheckSqlConnectionAndWait();
            try
            {
                using (SqlConnection cn = new SqlConnection(this.con))
                {
                    using (SqlCommand cmd = cn.CreateCommand())
                    {
                        //Check Stock transaction
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = sSQL;
                        cmd.Notification = null;
                        if (cn.State != ConnectionState.Open)
                        {
                            cn.Open();
                        }
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                Ret=UpdateQOHinSAP(dr.GetString(0), dr.GetString(1), System.Convert.ToInt32(dr.GetDecimal(3)));
                            }
                        }


                        cn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                WriteEventLog("The function " + GetCurrentMethod() + " return this error:" + ex.Message);
            }

        }

        private void UpdateItemQoH()
        {
            string Ret = "";
            string sSQL = "select distinct OITM.ItemCode,isnull(RQ.SumQoh,0) as SumQoh,isnull(OITM.U_QOHSupplier,0) from OITM  ";
            sSQL += "left join (select ItemCode,sum(U_QOHSupplier)as SumQoh from OSCN group by ItemCode ) as RQ on RQ.ItemCode=OITM.ItemCode  ";
            sSQL += "where  isnull(OITM.U_QOHSupplier,0)<>isnull(RQ.SumQoh,0) ";

            CheckSqlConnectionAndWait();
            try
            {
                using (SqlConnection cn = new SqlConnection(this.con))
                {
                    using (SqlCommand cmd = cn.CreateCommand())
                    {
                        //Check Stock transaction
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = sSQL;
                        cmd.Notification = null;
                        if (cn.State != ConnectionState.Open)
                        {
                            cn.Open();
                        }
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                Ret = UpdateSumQOHinSAP(dr.GetString(0), dr.GetInt32(1));
                            }
                        }


                        cn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                WriteEventLog("The function " + GetCurrentMethod() + " return this error:" + ex.Message);

            }

        }

        private void TimerCheckChange()
        {
            try
            {
                
                UpdateBpcatalogQoH();
                UpdateItemQoH();

            }
            catch (Exception ex)
            {
                WriteEventLog("The function " + GetCurrentMethod() + " return this error:" + ex.Message);
            }
        }

        private void RestartTimerChange()
        {
            try
            {
                if (DateTime.Now.ToString("hh:mm:ss") == this._Daily_Restart_Time)
                {
                    try
                    {
                        Environment.Exit(1);

                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                WriteEventLog("The function " + GetCurrentMethod() + " return this error:" + ex.Message);
            }
        }






        private void CheckSqlConnectionAndWait()
        {
            try
            {
                Boolean isConnectioError = true;
                while (isConnectioError)
                {
                    try
                    {
                        SqlConnection cn = new SqlConnection(this.con);
                        isConnectioError = false;
                    }
                    catch
                    {
                        isConnectioError = true;
                        System.Threading.Thread.Sleep(1000);
                    }
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
                eventLog.WriteEntry(Errmsg, EventLogEntryType.Error, 1012);
            }
            catch (Exception ex) { }
        }

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

        private void RestartTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {

                RestartTimer.Stop();
                RestartTimerChange();
                RestartTimer.Start();
            }
            catch (Exception ex)
            {
                WriteEventLog("The function " + GetCurrentMethod() + " return this error:" + ex.Message);
                RestartTimer.Start();
            }
        }

        private void Checktimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {

                Checktimer.Stop();
                TimerCheckChange();
                Checktimer.Start();
            }
            catch
            {
                Checktimer.Start();
            }
        }
    }
}
