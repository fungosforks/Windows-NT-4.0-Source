//
// Copyright © 2000-2003 Microsoft Corporation.  All rights reserved.
//
//
// This source code is licensed under Microsoft Shared Source License
// for the Visual Studio .NET Academic Tools Source Licensing Program
// For a copy of the license, see http://www.msdnaa.net/assignmentmanager/sourcelicense/
//

using System;
using System.Web.Mail;

namespace Microsoft.VisualStudio.Academic.AssignmentManager.ActionService
{
	/// <summary>
	/// Summary description for MessageM.
	/// </summary>
	public class MessageM
	{
		public MessageM()
		{

		}

		public static void sendEmailMessageToCourse(string subject, string body, string link, int courseId)
		{	
			if(!Convert.ToBoolean(SharedSupport.UsingSmtp))
			{
				throw (new System.Exception(SharedSupport.GetLocalizedString("Global_NoSMTP")));
			}

			try
			{				
				// validation
				if(body.Equals(String.Empty))
				{
					throw new  ArgumentException(SharedSupport.GetLocalizedString("SendEmailMessage_InvalidBody"));
				}
				if(subject.Equals(String.Empty))
				{
					throw new  ArgumentException(SharedSupport.GetLocalizedString("SendEmailMessage_InvalidSubject"));
				}

				string mailTo = "";
				System.Data.DataSet ds = new System.Data.DataSet();
				
				//use generic Assignment Manager From
				string sentByEmail = string.Empty;

				UserList ul = UserList.GetListFromCourse(courseId);
				int[] userIDs = ul.UserIDList;
				for(int i=0;i<userIDs.Length;i++)
				{
					UserM user = UserM.Load(userIDs[i]);
					mailTo += user.EmailAddress + ";";
				}

				// use Assignment Manager sysadmin email 
				UserM amsaUser = UserM.Load(Constants.ASSIGNMENTMANAGER_SYSTEM_ADMIN_USERID);
				sentByEmail = amsaUser.EmailAddress;
			
				// add the formatting and action link
				body += "\n" + "\n" + link;			
				
				// send email
				SendMessage(sentByEmail, mailTo, subject, body);
			}
			catch(System.Exception ex)
			{
				SharedSupport.HandleError(ex);
			}
		}

		internal static void SendMessage(string from, string mailTo, string subject, string body)
		{
			//this code calls the SendMessage function to send an EMAIL message.
			try
			{
				MailMessage mail = new MailMessage();			
				mail.From = from;
				mail.Subject = subject;
				mail.Body = body;
				mail.Bcc = mailTo;
			
				SmtpMail.Send(mail);
				
			}
			catch(System.Web.HttpException)
			{
				throw new System.Exception(SharedSupport.GetLocalizedString("ComposeMessage_SMTPError"));
			}			
		}

	}
}
