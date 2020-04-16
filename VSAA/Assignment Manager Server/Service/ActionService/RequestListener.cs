//
// Copyright © 2000-2003 Microsoft Corporation.  All rights reserved.
//
//
// This source code is licensed under Microsoft Shared Source License
// for the Visual Studio .NET Academic Tools Source Licensing Program
// For a copy of the license, see http://www.msdnaa.net/assignmentmanager/sourcelicense/
//


using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Messaging;

namespace Microsoft.VisualStudio.Academic.AssignmentManager.ActionService
{	
	/// <summary>
	/// Summary description for RequestListener.
	/// </summary>
	public class RequestListener : System.ServiceProcess.ServiceBase
	{
		private System.Timers.Timer timer;
		// Queue constants
		internal const string ACTION_QUEUE_NAME = "AMActions";
		internal const string ACTION_QUEUE_PATH = ".\\Private$\\AMActions";
		internal const string AM_SERVICE_NAME = "Assignment Manager Actions Service";

		private System.ComponentModel.Container components = null;
		private MessageQueue mq;

		public RequestListener()
		{
			InitializeComponent();
			this.ServiceName = AM_SERVICE_NAME;
		}

		// The main entry point for the process
		public static void Main()
		{
			System.ServiceProcess.ServiceBase[] ServicesToRun;
    
			// More than one user Service may run within the same process. To add
			// another service to this process, change the following line to
			// create a second service object. For example,
			//
			//   ServicesToRun = New System.ServiceProcess.ServiceBase[] {new Listener(), new MySecondUserService()};
			//
			ServicesToRun = new System.ServiceProcess.ServiceBase[] { new RequestListener() };

			System.ServiceProcess.ServiceBase.Run(ServicesToRun);
		}

		/// <summary>
		///    Set things in motion so your service can do its work.
		/// </summary>
		protected override void OnStart(string[] args)
		{

			// Register and enable the timer
			timer = new System.Timers.Timer();
			timer.Elapsed += new System.Timers.ElapsedEventHandler(this.SendNotifications);
			
			// determine the interval until the next midnight
			timer.Interval = millisecondsToMidnight();
			timer.Enabled = true;

			string mqPath = ACTION_QUEUE_PATH;
			string mqName = ACTION_QUEUE_NAME;

			// make sure the queue exists
			queueCreate(mqPath, mqName);

			mq = new MessageQueue(mqPath);
			((XmlMessageFormatter)mq.Formatter).TargetTypeNames = new string[]{"System.String"};

			mq.ReceiveCompleted += new System.Messaging.ReceiveCompletedEventHandler(this.OnReceiveCompleted);
			mq.BeginReceive();
		}

		/// <summary>
		/// Event fires when a message arrives on the queue.  Pulls the message and hands
		/// the information off to the AssignmentManager.ServerAction class to process
		/// </summary>
		public void OnReceiveCompleted(Object source, System.Messaging.ReceiveCompletedEventArgs e)
		{
			ServerAction sa = new ServerAction();

			// grab the queue information
			MessageQueue mq = (MessageQueue)source;

			// grab the message 
			System.Messaging.Message m = mq.EndReceive(e.AsyncResult);
			m.Formatter = new System.Messaging.XmlMessageFormatter(new String[]{"System.String"});

			// process the message
			string msgBody = (string)m.Body;

			try
			{
				bool result = sa.ProcessMessage(msgBody, m.Label);
			}
			catch (System.Exception ex)
			{
				System.Diagnostics.EventLog.WriteEntry(this.ServiceName.ToString(), ex.ToString());
			}
			finally
			{
				// continue listening on the queue
				mq.BeginReceive();
			}
		}
		
		/// <summary>
		///    Stop this service.
		/// </summary>
		protected override void OnStop()
		{
			// release the mq instance
			mq.Close();

//			// disable the timer
			timer.Enabled = false;
			timer.Close();
		}
		
		/// <summary>
		/// Connect to or Create the Academic Queue
		/// </summary>
		private void queueCreate(string mqPath, string mqName)
		{
			try
			{
				mqPath = @mqPath;
				mqName = @mqName;
				
				if(!System.Messaging.MessageQueue.Exists(mqPath))
				{
					System.Messaging.MessageQueue.Create(mqPath, false);
					SecurityPermissions.ACLQueue(mqPath);

					// set other queue parameters
					System.Messaging.MessageQueue mq = new System.Messaging.MessageQueue(mqPath);
					mq.Label = mqName;
					//assumption Queue is on this box
					mq.MachineName = "."; 
					mq.QueueName = mqName;
					mq.MaximumQueueSize = 10240;  // Set max queue size to 10M 
				}
			}
			catch(System.Exception)
			{
			}
		}

		public void SendNotifications(object sender, System.Timers.ElapsedEventArgs args)
		{
			// disable the timer
			timer.Enabled = false;

            try
            {

                // use Assignment Manager sysadmin email 
                UserM amsaUser = UserM.Load(Constants.ASSIGNMENTMANAGER_SYSTEM_ADMIN_USERID);
                string sentByEmail = amsaUser.EmailAddress;

                System.Data.DataSet dsAssignmentList = new System.Data.DataSet();
                DatabaseCall dbc = new DatabaseCall("Notifications_GetAssignmentList", DBCallType.Select);
                dbc.Fill(dsAssignmentList);
                if (dsAssignmentList.Tables[0].Rows.Count <= 0)
                {
                    return;
                }

                for(int j=0;j<dsAssignmentList.Tables[0].Rows.Count;j++)
                {
                    int assignmentID = Convert.ToInt32(dsAssignmentList.Tables[0].Rows[j]["AssignmentID"]);
                    // send the notifications
                    try
                    {
                        //////////////////////////////////////////////////////////////////////
                        /// Past Due Notifications
                        //////////////////////////////////////////////////////////////////////
                        System.Data.DataSet ds = new System.Data.DataSet();
                        dbc = new DatabaseCall("Notifications_BrowsePastDueNotifications", DBCallType.Select);
                        dbc.AddParameter("AssignmentID", assignmentID);
                        dbc.Fill(ds);

                        if (ds.Tables[0].Rows.Count <= 0)
                        {
                            continue;
                        }

                        for(int i=0;i<ds.Tables[0].Rows.Count; i++)
                        {
                            if (ds.Tables[0].Rows[i]["AssignmentID"] != DBNull.Value)
                            {
                                string assignName = ds.Tables[0].Rows[i]["ShortName"].ToString();
                                DateTime dueDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["DueDate"]);
                                string[] AssignmentInfo = new string[]{assignName, string.Format("{0:d}", dueDate)};

                                string pastDueSubject = SharedSupport.GetLocalizedString("Notification_PastDueSubject", AssignmentInfo);
                                string pastDueBody = SharedSupport.GetLocalizedString("Notification_PastDueBody", AssignmentInfo);	
                                string reminderSubject = SharedSupport.GetLocalizedString("Notification_ReminderSubject", AssignmentInfo);
                                string reminderBody = SharedSupport.GetLocalizedString("Notification_ReminderBody", AssignmentInfo);	

                                TimeSpan difference = dueDate - DateTime.Today;
                                if (Convert.ToBoolean(ds.Tables[0].Rows[i]["SendPastDue"]) && 
                                    (difference.Days == -1*Convert.ToInt32(ds.Tables[0].Rows[i]["PastDueWarningDays"])) )
                                {
                                    UserM user = UserM.Load( Convert.ToInt32( ds.Tables[0].Rows[i]["UserID"] ) );
                                    MessageM.SendMessage(sentByEmail, user.EmailAddress, pastDueSubject, pastDueBody);
                                }

                                if (Convert.ToBoolean(ds.Tables[0].Rows[i]["SendReminders"]) &&
                                    (difference.Days == Convert.ToInt32(ds.Tables[0].Rows[i]["ReminderWarningDays"])) )
                                {
                                    UserM user = UserM.Load( Convert.ToInt32( ds.Tables[0].Rows[i]["UserID"] ) );
                                    MessageM.SendMessage(sentByEmail, user.EmailAddress, reminderSubject, reminderBody);			
                                }
                            }
                        }
                    }
                    catch(System.Exception ex)
                    {
                        SharedSupport.HandleError(ex);
                    }
                }
            }
            catch(System.Exception ex)
            {
                SharedSupport.HandleError(ex);
            }            
            finally
            {
                // reset the interval for the next event
                timer.Interval = millisecondsToMidnight();

                // re-enable the timer
                timer.Enabled = true;
            }
		}

		private double millisecondsToMidnight()
		{
            System.DateTime tomorrow = System.DateTime.Today.AddDays(1);
			System.TimeSpan toGo = tomorrow.Subtract(System.DateTime.Now);
			return toGo.TotalMilliseconds;
		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion
	}
}
