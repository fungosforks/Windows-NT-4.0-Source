//
// Copyright © 2000-2003 Microsoft Corporation.  All rights reserved.
//
//
// This source code is licensed under Microsoft Shared Source License
// for the Visual Studio .NET Academic Tools Source Licensing Program
// For a copy of the license, see http://www.msdnaa.net/assignmentmanager/sourcelicense/
//
using System;
using Microsoft.Office.Core;
using AssignmentManager.ClientUI;

namespace FacultyClient 
{
  
	/// <summary>
	///   The object for implementing an Add-in.
	/// </summary>
	public class CodeMarker : Object, IAddInCommand 
	{

		public string addInName { get { return m_strName; } }
		public string commandName { get { return m_strCommandName; } }
		public string commandText { get { return m_strCommandName; } }

		/// <summary>
		/// Registers a command and places it onto the context menu for the editor
		/// window.
		/// </summary>
		public void OnConnection(EnvDTE._DTE application, Extensibility.ext_ConnectMode connectMode, EnvDTE.AddIn addIn) 
		{
			m_applicationObject = (EnvDTE._DTE)application;
			m_addInInstance = (EnvDTE.AddIn)addIn;

			m_strCommandName = "MarkCodeForExtraction";
			m_strName = AMResources.GetLocalizedString("CodeMarkerName");
			m_strItemText= AMResources.GetLocalizedString("CodeMarkerItemText");

			string description = AMResources.GetLocalizedString("CodeMarkerDescription");
			EnvDTE.Commands commands = null;
			EnvDTE.Command command = null;
			Microsoft.Office.Core._CommandBars officeCommandBars = null;
			Microsoft.Office.Core.CommandBar officeCommandBar = null;
			Microsoft.Office.Core.CommandBarControl officeCommandControl = null;
			Microsoft.Office.Core.CommandBar officeAcademic = null;
			object []contextGuids;
			contextGuids = new object[] { };
        
			commands = m_applicationObject.Commands;
			try 
			{
				command = commands.AddNamedCommand(m_addInInstance, m_strCommandName, m_strName, description,
					true, 12, ref contextGuids, (int) (EnvDTE.vsCommandStatus.vsCommandStatusEnabled | EnvDTE.vsCommandStatus.vsCommandStatusSupported));
          
				// Add the new command to the top-level context menu for the editor 
				// code window, if possible.
				officeCommandBars = m_applicationObject.CommandBars;
				officeCommandBar= (CommandBar)officeCommandBars["Code Window"];
				officeCommandControl = command.AddControl((object)officeCommandBar, 1);
				officeCommandControl.Caption = m_strItemText;

				string amFacultyMenuItem = AMResources.GetLocalizedString("AMFacultyMenuItem");
				// Also attempt to add it to the Tools menu as well, for accessibility.
				try
				{
					officeAcademic = (CommandBar)officeCommandBars[amFacultyMenuItem];
				}
				catch
				{
				}
				if( officeAcademic == null )
				{
					officeCommandBar= (CommandBar)officeCommandBars["Tools"];
					officeAcademic = (CommandBar)m_applicationObject.Commands.AddCommandBar(amFacultyMenuItem, EnvDTE.vsCommandBarType.vsCommandBarTypeMenu, officeCommandBar,1);
				}
			    
				officeCommandControl = command.AddControl((object)officeAcademic, 1);
				officeCommandControl.Caption = AMResources.GetLocalizedString("CodeMarkerToolsMenuItemText");
			} 
			catch (System.Exception /*e*/) 
			{
				// Falling into this simply means that the command was already registered.
			}
		}

		/// <summary>
		/// Called when the AddIn is discarded. This method allows each of the commands to
		/// to unregister and close down on exiting.
		/// </summary>
		/// <param name="application"> Root object in the application </param>
		/// <param name="connectMode"> 'Mode' in which the environment is closing the addin </param>
		/// <param name="addIn"> Object representing this AddIn in the Object Model</param>
		public void OnDisconnection(EnvDTE._DTE application, Extensibility.ext_DisconnectMode disconnectMode, EnvDTE.AddIn addIn)
		{
			application.Commands.Item("FacultyClient.Connect." + m_strCommandName, 0).Delete();
		}

		/// <summary>
		/// This command is available if both the file type is valid and if the document
		/// contains a selection of non-zero size.
		/// </summary>
		public void QueryStatus(ref EnvDTE.vsCommandStatus status) 
		{ 
			// If there is no supported extension, remove it. If it's supported, but there's no
			// document selection currently, then disable it (gray-out).
			CommentPair comments = GetCurrentComments();

			if (comments == null) 
			{
				status = EnvDTE.vsCommandStatus.vsCommandStatusInvisible | EnvDTE.vsCommandStatus.vsCommandStatusUnsupported; 
			} 
			else 
			{
				EnvDTE.TextDocument textDocument = m_applicationObject.ActiveDocument.Object("TextDocument") as EnvDTE.TextDocument;
				if (m_applicationObject.ActiveWindow.Document == m_applicationObject.ActiveDocument)
				{
					if (textDocument == null) 
					{
						// Never can happen because of GetCurrentComments' implementation
						throw new Exception();
					}

					if (textDocument.Selection.TopPoint.AbsoluteCharOffset == textDocument.Selection.BottomPoint.AbsoluteCharOffset) 
					{
						status = EnvDTE.vsCommandStatus.vsCommandStatusUnsupported;
					} 
					else 
					{
						status = EnvDTE.vsCommandStatus.vsCommandStatusEnabled | EnvDTE.vsCommandStatus.vsCommandStatusSupported;
					}
				}
				else
				{
					status = EnvDTE.vsCommandStatus.vsCommandStatusUnsupported;
				}
			}
		}

		/// <summary>
		/// Marks a region of code for extraction. It pastes the correct comment characters for the
		/// file type at the top and bottom of the region. If the user has selected to add 'TODO' comments
		/// and the editor for this file type supports commenting regions, it will also prompt
		/// the user for a TODO comment and place that comment above the region of code to be
		/// marked for extraction.
		/// </summary>
		public void Exec(EnvDTE.vsCommandExecOption executeOption, ref object varIn, ref object varOut) 
		{
			try 
			{
				// First, get ahold of the necessary items to get the data we need
				CommentPair comments = GetCurrentComments();
				if (comments == null) 
				{
					return;
				}

				// Get the document
				EnvDTE.EditPoint startPoint, endPoint;
				EnvDTE.TextDocument textDocument = m_applicationObject.ActiveDocument.Object("TextDocument") as EnvDTE.TextDocument;
				if (textDocument != null) 
				{
					if (textDocument.Selection.BottomPoint.AtStartOfLine && textDocument.Selection.IsActiveEndGreater) 
					{
						// People whom select a whole section of text often also get the first
						// not-character of the next line (because that's how windows mouse-selection 
						// works!). They only want to go around the highlighted section...
						textDocument.Selection.CharLeft(true, 1);
					}

					textDocument.Selection.Insert(comments.BeginComment + "\n", (int)EnvDTE.vsInsertFlags.vsInsertFlagsInsertAtStart);
					textDocument.Selection.Insert("\n" + comments.EndComment, (int)EnvDTE.vsInsertFlags.vsInsertFlagsInsertAtEnd);
          
					// Store away the selection for use after the TODO comment
					// is added. Create new EditPoint objects here because they
					// cause the editor to keep track of the markers in the text, even
					// when the selection changes.
					startPoint = textDocument.Selection.TopPoint.CreateEditPoint();
					endPoint = textDocument.Selection.BottomPoint.CreateEditPoint();

					MaybeAddTodoComment(textDocument.Selection);

					// Restore the selection
					textDocument.Selection.MoveToPoint(startPoint, false);
					textDocument.Selection.MoveToPoint(endPoint, true);

					// Perform the selection hiding (code collapse) only if the editor is not
					// in auto-select mode and if the user has enabled it in the tools options
					// page for the academic toolset. This is done last because some language
					// services move the Top and Bottom points of the Selection around when
					// performing an outline.
					if (EditorSupportsHideSelection() &&
						((bool)((EnvDTE.Properties)m_applicationObject.get_Properties("Assignment Manager", "Code Extractor")).Item("Collapse").Value)) 
					{
						textDocument.Selection.TopPoint.CreateEditPoint().OutlineSection(textDocument.Selection.BottomPoint);
					}
				}
			} 
			catch (System.Exception /*e*/) 
			{
			}
		}

		/// <summary>
		/// If both the user has chosen in the Tools Options dialog to be prompted for a
		/// TODO comment *and* this language service supports a TODO comment, then we
		/// prompt them for their string, move the cursor around, add the text, and comment it.
		/// </summary>
		private void MaybeAddTodoComment(EnvDTE.TextSelection selection) 
		{
			EnvDTE.Command CommentSelection = null;
			EnvDTE.TaskList tl = null;
			System.Object emptyIn = null;
			System.Object emptyOut = null;
			string todoComment = null;
			TodoCommentForm input = null;
			input = new TodoCommentForm();


			if (((bool)((EnvDTE.Properties)m_applicationObject.get_Properties("Assignment Manager", "Code Extractor")).Item("PromptForTodo").Value) &&
				((CommentSelection = EditorSupportsCommentSelection()) != null) &&
				(input.ShowDialog() == System.Windows.Forms.DialogResult.OK)) 
			{
				todoComment = input.TodoComment;
        
				// Move the selection to be right before the commented region, add the new text,
				// and comment it.
				selection.MoveToPoint(selection.TopPoint, false);
				tl = m_applicationObject.Windows.Item(EnvDTE.Constants.vsWindowKindTaskList).Object as EnvDTE.TaskList;
				selection.Insert(tl.DefaultCommentToken + ": " + todoComment + "\n", (int)EnvDTE.vsInsertFlags.vsInsertFlagsInsertAtStart);
				m_applicationObject.Commands.Raise(CommentSelection.Guid, CommentSelection.ID, ref emptyIn, ref emptyOut);
			}
		}

		/// <summary>
		/// We determine if the Editor supports comment selection by first locating the command
		/// and then querying its availability (which will be set on a per-editor basis).
		/// </summary>
		private EnvDTE.Command EditorSupportsCommentSelection() 
		{
			try 
			{
				EnvDTE.Command c = m_applicationObject.Commands.Item("Edit.CommentSelection", -1);
				return (c.IsAvailable?c:null) ;
			} 
			catch (System.Exception) 
			{
				return null;
			}
		}

		/// <summary>
		/// We determine if the Editor is currently allowing the user to collapse regions
		/// by first locating the command and then querying its availability (which the language
		/// services will disable if they are in 'automatic' collapse mode and re-enable when
		/// they are no longer).
		/// </summary>
		private bool EditorSupportsHideSelection() 
		{
			try 
			{
				EnvDTE.Command c = m_applicationObject.Commands.Item("Edit.HideSelection", -1);
				return (c.IsAvailable?true:false) ;
			} 
			catch (System.Exception) 
			{
				return false;
			}
		}

		/// <summary>
		/// The current comments are retrieved by obtaining the Properites object from the
		/// Tools Options window associated with the Academic toolset and walking down the
		/// set of registered file extensions looking for the one corresponding to
		/// the file type currently opened.
		/// </summary>
		private CommentPair GetCurrentComments() 
		{
			EnvDTE.Document document = null;
			EnvDTE.TextDocument textDocument = null;
			EnvDTE.Properties extractorProperties = null;
			Extensions extensions = null;
			string extension;
			int lastIndex;

			try 
			{
				// While we don't *do* anything with the document object, we check for its presence because
				// we only know how to do insertions on the TextDocument class of object.
				document = m_applicationObject.ActiveDocument;
				if ((document == null) ||
					((textDocument = document.Object("TextDocument") as EnvDTE.TextDocument) == null)) 
				{
					return null;
				}

				extension = m_applicationObject.ActiveDocument.Name;
				lastIndex = extension.LastIndexOf('.');
				if (lastIndex == -1) 
				{
					return null;
				}
				extension = extension.Remove(0, lastIndex + 1); // Trim off the 'name.' part of 'name.ext'

				extractorProperties = m_applicationObject.get_Properties("Assignment Manager", "Code Extractor");
				if (extractorProperties == null) 
				{
					throw new Exception(AMResources.GetLocalizedString("CollapseMarkedInvalidInstallation"));
				}

				extensions = extractorProperties.Item("Extensions").Object as Extensions;
				if (extensions == null)	
				{
					throw new Exception(AMResources.GetLocalizedString("CollapseMarkedInvalidInstallation"));
				}

				return extensions[extension];
			} 
			catch (Exception) 
			{
				return null;
			}
		}
		
		private EnvDTE._DTE m_applicationObject;
		private EnvDTE.AddIn m_addInInstance;
		private string m_strCommandName;
		private string m_strName;
		private string m_strItemText;
	}
}
