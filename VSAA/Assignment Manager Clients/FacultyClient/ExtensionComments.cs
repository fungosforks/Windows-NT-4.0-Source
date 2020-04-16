//
// Copyright © 2000-2003 Microsoft Corporation.  All rights reserved.
//
//
// This source code is licensed under Microsoft Shared Source License
// for the Visual Studio .NET Academic Tools Source Licensing Program
// For a copy of the license, see http://www.msdnaa.net/assignmentmanager/sourcelicense/
//
using System;
using System.Runtime.InteropServices;
using AssignmentManager.ClientUI;

namespace FacultyClient 
{

	/// <summary>
	/// The ExtensionComments class is the internal-only representation of the
	/// information as retrieved from the registry and stored internally. Note
	/// that it can be used to:
	///  Load from the registry
	///  Save back to the registry
	///  Create a PropertiesDummy object
	///  Retrieve entries in a convenient manner for use in the tools options dialog
	/// </summary>
	internal class ExtensionComments : Object 
	{
		public ExtensionComments() { m_Entries = null; }

		/// <summary>
		/// This checks the registry for pre-existing values for file extensions and tags. If they are there, it
		/// loads them into memory. If they are not, then it loads the defaults from the resources embedded into
		/// the module itself.
		/// If the data are inconsitant in the registry, then an InvalidOperationException will be thrown.
		/// </summary>
		public void LoadFromRegistry() 
		{
			int iCollapse = 0;
			int iPromptForTodo = 0;
			m_Entries = new System.Collections.ArrayList();
      
			Microsoft.Win32.RegistryKey keyRoot = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(s_strRoot);
			Microsoft.Win32.RegistryKey keyID = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(s_strRoot + "\\" + s_strIDKey);
			Microsoft.Win32.RegistryKey keyBegin = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(s_strRoot +  "\\" + s_strBeginTagsKey);
			Microsoft.Win32.RegistryKey keyEnd = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(s_strRoot +  "\\" + s_strEndTagsKey);

			if ((keyID != null) &&
				(keyBegin != null) &&
				(keyEnd != null) &&
				(keyRoot != null)) 
			{
				try 
				{
					// First, get the Collapse-state. Default to 'true', if the value no
					// longer exists.
					iCollapse = (int)keyRoot.GetValue(s_strCollapseValue, 1);
					if (iCollapse == 0) 
					{
						m_collapse = false;
					} 
					else 
					{
						m_collapse = true;
					}

					iPromptForTodo = (int)keyRoot.GetValue(s_strPromptForTodoValue, 1);
					if (iPromptForTodo == 0) 
					{
						m_todo = false;
					} 
					else 
					{
						m_todo = true;
					}

					// Then, all of the comment settings.
					string []values = keyID.GetValueNames();
          
					foreach (string value in values) 
					{
						string extensions = (string)keyID.GetValue(value);
						string begin = (string)keyBegin.GetValue(value);
						string end = (string)keyEnd.GetValue(value);
            
						m_Entries.Add(new ExtensionComment(extensions, begin, end));
					}
				} 
				catch (System.Exception) 
				{
					throw new System.InvalidOperationException();
				}
			} 
			else 
			{
				m_collapse = true;
				m_todo = true;

				try 
				{
					m_Entries.Add(new ExtensionComment(AMResources.GetLocalizedString("ToolsOptionsDefaultCPPExtensions"),
						AMResources.GetLocalizedString("ToolsOptionsDefaultCPPBeginTag"),
						AMResources.GetLocalizedString("ToolsOptionsDefaultCPPEndTag")));
					m_Entries.Add(new ExtensionComment(AMResources.GetLocalizedString("ToolsOptionsDefaultCExtensions"),
						AMResources.GetLocalizedString("ToolsOptionsDefaultCBeginTag"),
						AMResources.GetLocalizedString("ToolsOptionsDefaultCEndTag")));
					m_Entries.Add(new ExtensionComment(AMResources.GetLocalizedString("ToolsOptionsDefaultVBExtensions"),
						AMResources.GetLocalizedString("ToolsOptionsDefaultVBBeginTag"),
						AMResources.GetLocalizedString("ToolsOptionsDefaultVBEndTag")));
				} 
				catch (System.Exception) 
				{
					// Failed to load localization support, so just fall back on English default values and attempt to continue.
					m_Entries.Add(new ExtensionComment(".cpp, .cs, .h", "//BEGIN_STUDENT_CODE", "//END_STUDENT_CODE"));
					m_Entries.Add(new ExtensionComment(".c", "/* BEGIN_STUDENT_CODE */", "/* END_STUDENT_CODE */"));
					m_Entries.Add(new ExtensionComment(".vb", "' BEGIN_STUDENT_CODE", "' END_STUDENT_CODE"));
				}
			}
		}

		/// <summary>
		/// This function saves all of the current settings to the registry. It does so by first
		/// deleting whatever data may be in there and then re-creating all of the data on disk
		/// from memory.
		/// </summary>
		public void SaveToRegistry() 
		{
			Microsoft.Win32.RegistryKey keyID = null;
			Microsoft.Win32.RegistryKey keyBegin = null;
			Microsoft.Win32.RegistryKey keyEnd = null;
			Microsoft.Win32.RegistryKey keyRoot = null;
			int i = 0;
			string key = null;

			keyRoot = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(s_strRoot);

			if (keyRoot == null) 
			{
				// In this case, the user doesn't have permissions to their own HKCU hive!
				return;
			}

			try 
			{
				keyRoot.DeleteSubKey(s_strIDKey);
			} 
			catch (System.Exception) 
			{
				// Subkey didn't exist.
			}
			try 
			{
				keyRoot.DeleteSubKey(s_strBeginTagsKey);
			} 
			catch (System.Exception) 
			{
				// Subkey didn't exist.
			}
			try 
			{
				keyRoot.DeleteSubKey(s_strEndTagsKey);
			} 
			catch (System.Exception) 
			{
				// Subkey didn't exist.
			}
      
			keyID = keyRoot.CreateSubKey(s_strIDKey);
			keyBegin = keyRoot.CreateSubKey(s_strBeginTagsKey);
			keyEnd = keyRoot.CreateSubKey(s_strEndTagsKey);

			// First, serialize the collapse and todo-prompt-p flags
			keyRoot.SetValue(s_strCollapseValue, (m_collapse)?1:0);
			keyRoot.SetValue(s_strPromptForTodoValue, (m_todo)?1:0);

			// Then, serialize all of the comments
			foreach (ExtensionComment ec in m_Entries) 
			{
				key = i.ToString();
				keyID.SetValue(key, ec.Extensions);
				keyBegin.SetValue(key, ec.BeginComment);
				keyEnd.SetValue(key, ec.EndComment);
				i += 1;
			}

			// Flushes changes out to disk without relying on the garbage collector to finalize
			// the objects.
			keyID.Close();
			keyBegin.Close();
			keyEnd.Close();
			keyRoot.Close();
		}
    
		public PropertiesDummy CreatePropertiesDummy() 
		{
			return new PropertiesDummy(m_Entries, m_collapse, m_todo);
		}

		public int Count() 
		{
			return m_Entries.Count;
		}

		public ExtensionComment this[int index] 
		{
			get 
			{
				return (ExtensionComment)m_Entries[index];
			}
			set 
			{ 
				m_Entries[index] = value;
			}
		}

		public void Clear() 
		{
			m_Entries.Clear();
		}

		public void Add(ExtensionComment ec) 
		{
			m_Entries.Add(ec);
		}
    
		public bool Collapse 
		{
			get 
			{
				return m_collapse;
			}
			set 
			{
				m_collapse = value;
			}
		}

		public bool PromptForTodo 
		{
			get 
			{
				return m_todo;
			}
			set 
			{
				m_todo = value;
			}
		}

		private System.Collections.ArrayList m_Entries;
		private bool m_collapse;
		private bool m_todo;

		// NOTE: these are all hard-coded because it is impossible to guarantee a pointer to the DTE's available at the time
		// that this object is initialized and will need its values. Therefore, we cannot use dte.GetRegistryRoot() to determine the
		// top-level from which to store and retrieve settings.
		private static string s_strRoot = "SOFTWARE\\Microsoft\\VisualStudio\\7.1\\AddIns\\FacultyClient.Connect\\Code Extractor Options";

		//"SOFTWARE\\Microsoft\\VisualStudio\\7.1\\AddIns\\Microsoft.VisualStudio.Academic.FacultyTools.VS7AddIn\\Code Extractor Options";
		private static string s_strCollapseValue = "Collapse";
		private static string s_strPromptForTodoValue = "PromptForTodo";
		private static string s_strIDKey = "ID";
		private static string s_strBeginTagsKey = "BeginTags";
		private static string s_strEndTagsKey = "EndTags";
	}

}
