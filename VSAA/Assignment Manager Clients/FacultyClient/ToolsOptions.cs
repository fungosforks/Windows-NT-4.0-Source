//
// Copyright © 2000-2003 Microsoft Corporation.  All rights reserved.
//
//
// This source code is licensed under Microsoft Shared Source License
// for the Visual Studio .NET Academic Tools Source Licensing Program
// For a copy of the license, see http://www.msdnaa.net/assignmentmanager/sourcelicense/
//
namespace FacultyClient 
{
	using System;
	using System.Drawing;
	using System.Collections;
	using System.ComponentModel;
	using System.Windows.Forms;
	using AssignmentManager.ClientUI;
  

	/// <summary>
	///  This class is the Tools Options dialog implementing the settings
	///  for code extraction.
	/// </summary>
	public class ToolsOptions : System.Windows.Forms.UserControl, EnvDTE.IDTToolsOptionsPage 
	{
		/// <summary>
		///    Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components;
		private System.Windows.Forms.Button bDelete;
		private System.Windows.Forms.Button bNew;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox cbCollapse;
		private System.Windows.Forms.CheckBox cbPromptForTodo;
		private System.Windows.Forms.DataGrid dgEntries;
		private System.Data.DataTable dtValues;

		public ToolsOptions() 
		{
			m_applicationObject = null;
			m_extComments = null;
			m_fInitialized = false;
			components = null;
		}

		/// <summary>
		///    Clean up any resources being used
		/// </summary>
		protected override void Dispose(bool flag)
		{
			if (flag) 
			{
				if (m_fInitialized && (components != null)) 
				{
					components.Dispose();
					components = null;
				}
			}
			base.Dispose(flag);
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() 
		{
			// This method will be called every time that the Tools Options dialog is loaded. The first 
			// time, we initialize the internal controls, etc. All subsequent times, however, we re-initialize
			// the datagrid and checkboxes.
			try 
			{
				if (!m_fInitialized) 
				{
					m_fInitialized = true;

					this.components = new System.ComponentModel.Container ();
					this.bDelete = new System.Windows.Forms.Button ();
					this.label1 = new System.Windows.Forms.Label ();
					this.cbCollapse = new System.Windows.Forms.CheckBox ();
					this.cbPromptForTodo = new System.Windows.Forms.CheckBox ();
					this.bNew = new System.Windows.Forms.Button ();
					this.dgEntries = new System.Windows.Forms.DataGrid();
					//@this.TrayLargeIcon = false;
					//@this.TrayAutoArrange = true;
					//@this.TrayHeight = 0;
					this.Size = new System.Drawing.Size (400, 300);

					label1.Location = new System.Drawing.Point (0, 8);
					label1.Text = AMResources.GetLocalizedString("ToolsOptionsDescriptionLabel");
					label1.Size = new System.Drawing.Size (376, 26);
					label1.TabIndex = 100;

					cbCollapse.Location = new System.Drawing.Point (0, 240);
					cbCollapse.Text = AMResources.GetLocalizedString("ToolsOptionsCollapseCombo");
					cbCollapse.Size = new System.Drawing.Size (300, 24);
					cbCollapse.TabIndex = 3;

					cbPromptForTodo.Location = new System.Drawing.Point (0, 265);
					cbPromptForTodo.Text = AMResources.GetLocalizedString("ToolsOptionsTodoCommentCombo");
					cbPromptForTodo.Size = new System.Drawing.Size (300, 24);
					cbPromptForTodo.TabIndex = 4;

					bNew.Location = new System.Drawing.Point (0, 42);
					bNew.Size = new System.Drawing.Size (96, 23);
					bNew.TabIndex = 0;
					bNew.Text = AMResources.GetLocalizedString("ToolsOptionsAddRuleButton");
					bNew.Click += new System.EventHandler(bNew_OnClick);
					bDelete.Location = new System.Drawing.Point (102, 42);
					bDelete.Size = new System.Drawing.Size (132, 23);
					bDelete.TabIndex = 1;
					bDelete.Text = AMResources.GetLocalizedString("ToolsOptionsDeleteMarkedRulesButton");
					bDelete.Click += new System.EventHandler(bDelete_OnClick);

					// Create the DataTable to contain the data stored in the registry.
					dtValues = new System.Data.DataTable("ExtractionOptions");

					dtValues.Columns.Add(new System.Data.DataColumn("X", typeof(bool)));
					dtValues.Columns.Add(new System.Data.DataColumn(m_strFileExtensionField, typeof(string)));
					dtValues.Columns.Add(new System.Data.DataColumn(m_strBeginCommentField, typeof(string)));
					dtValues.Columns.Add(new System.Data.DataColumn(m_strEndCommentField, typeof(string)));

					// This supresses the automatic new row that is otherwise created by the
					// DataGrid control. However, the 'Add' button in the UI will still be able
					// to add new rows explicitly.
					dtValues.DefaultView.AllowNew = false;

					// Place the DataGrid control on the form and set some properties
					dgEntries.Location = new System.Drawing.Point (0, 71);
					dgEntries.Size = new System.Drawing.Size (395, 165);
					dgEntries.TabIndex = 2;
					dgEntries.BackgroundColor = System.Drawing.Color.White;
					dgEntries.FlatMode = true;
					dgEntries.CaptionVisible = false;
					dgEntries.AllowNavigation = false;
					dgEntries.MouseDown += new System.Windows.Forms.MouseEventHandler(dgEntries_OnMouseDown);

					// Attach the DataTable to the DataGrid
					dgEntries.SetDataBinding(dtValues, null);

					// Reconfigure all of the columns and styles of the DataGrid to better fit our
					// DataTable.
					DataGridTableStyle tableStyle = new DataGridTableStyle();
					tableStyle.MappingName = dtValues.TableName;
					tableStyle.AllowSorting = false;
					tableStyle.ColumnHeadersVisible = true;
					tableStyle.RowHeadersVisible = false;
					tableStyle.PreferredRowHeight = 25;

					// Allocate all of the column style objects (they aren't created automatically
					// when binding the DataTable to the DataGrid) and set the properites for each
					// of the individual columns.
					for (int i = 0; i < dtValues.Columns.Count; i++) 
					{
						DataGridColumnStyle gridColumn = null;
						System.Data.DataColumn dataColumn = dtValues.Columns[i];
						string sName = dataColumn.ColumnName;

						if (sName == "X") 
						{
							gridColumn = new DataGridBoolColumn(); // A checkbox
							((DataGridBoolColumn)gridColumn).AllowNull = false; // Don't allow the gray-state.
							gridColumn.Alignment = HorizontalAlignment.Left;
							gridColumn.Width = 25;
						} 
						else if (sName == m_strFileExtensionField) 
						{
							gridColumn = new DataGridTextBoxColumn();
							((DataGridTextBox)((DataGridTextBoxColumn)gridColumn).TextBox).Multiline = false; // Don't allow multi-line edits, since they are difficult to handle.
							gridColumn.Alignment = HorizontalAlignment.Left;
							gridColumn.Width = 87;
						} 
						else if (sName == m_strBeginCommentField) 
						{
							gridColumn = new DataGridTextBoxColumn();
							((DataGridTextBox)((DataGridTextBoxColumn)gridColumn).TextBox).Multiline = false; // Don't allow multi-line edits, since they are difficult to handle.
							gridColumn.Alignment = HorizontalAlignment.Left;
							gridColumn.Width = 139;
						} 
						else if (sName == m_strEndCommentField) 
						{
							gridColumn = new DataGridTextBoxColumn();
							((DataGridTextBox)((DataGridTextBoxColumn)gridColumn).TextBox).Multiline = false; // Don't allow multi-line edits, since they are difficult to handle.
							gridColumn.Alignment = HorizontalAlignment.Left;
							gridColumn.Width = 139;
						} 
						else 
						{
							System.Diagnostics.Debug.Assert(false, "A column was added to the ToolsOptions dialog, but its style was not created.");
							return;
						}

						gridColumn.MappingName = sName;
						gridColumn.HeaderText = sName;
						tableStyle.GridColumnStyles.Add(gridColumn);
					}
					dgEntries.TableStyles.Add(tableStyle);

					this.bNew.AccessibleName = "bNew";
					this.bDelete.AccessibleName = "bDelete";
					this.dgEntries.AccessibleName = "dgEntries";
					this.cbCollapse.AccessibleName = "cbCollapse";
					this.cbPromptForTodo.AccessibleName = "cbPromptForTodo";
					this.label1.AccessibleName = "label1";

					this.Controls.Add (this.bNew);
					this.Controls.Add (this.bDelete);
					this.Controls.Add (this.dgEntries);
					this.Controls.Add (this.cbCollapse);
					this.Controls.Add (this.cbPromptForTodo);
					this.Controls.Add (this.label1);
				} 
				else 
				{
					// Reset the UI.
					PopulateTable();
				}
			} 
			catch (System.Exception e) 
			{
				System.Diagnostics.Debug.Assert(false, "Unable to initialize the dialog." + e.Message);
			}
		}

		private void dgEntries_OnMouseDown(Object sender, MouseEventArgs e) 
		{
			DataGrid.HitTestInfo hi = dgEntries.HitTest(e.X, e.Y);

			// By default, the first click on a boolean column sets it into edit
			// mode, rather than toggling the value. This function overrides the
			// mousedown behavior to instead cause that to happen for just that
			// column.
			if ((hi.Column == 0) && (hi.Row >= 0)) 
			{
				dtValues.Rows[hi.Row]["X"] = !((bool)dtValues.Rows[hi.Row]["X"]);
			}
		}

		private void bNew_OnClick(object sender, EventArgs e) 
		{
			System.Data.DataRow newRow = dtValues.NewRow();
			newRow["X"] = false;
			newRow[m_strFileExtensionField] = " "; 
			newRow[m_strBeginCommentField] = " ";
			newRow[m_strEndCommentField] = " ";
			dtValues.Rows.Add(newRow);
			dtValues.AcceptChanges();
		}

		private void bDelete_OnClick(object sender, EventArgs e) 
		{
			// Set the current cell to cell 1, row 1 in order to prevent the
			// datagrid object from keeping track of the item that had just
			// been selected and remarking it as selected when a new row is
			// added, especially since that new row will NOT have the entry
			// selected and therefore the UI will be out of sync with the 
			// backing store.
			dgEntries.CurrentCell = new DataGridCell(1,1);

			// This goes backwards from the end of the list because otherwise the 
			// indices are updated in-place. Note that it is now considered invalid to
			// delete items from a DataTable while using an enumerator, which is
			// why a 'for' loop is used instead.
			for (int i = (dtValues.Rows.Count - 1); i >= 0; i--) 
			{
				if ((bool)(dtValues.Rows[i]["X"]) == true) 
				{
					dtValues.Rows[i].Delete();
				}
			}

			dtValues.AcceptChanges();
		}

		/// <summary>
		/// Return a new instance of a PropertiesObject, ignoring the 
		/// current state of the UI.
		/// </summary>
		public void GetProperties(ref object PropertiesObject) 
		{
			ExtensionComments comments = new ExtensionComments();
			comments.LoadFromRegistry();
			PropertiesObject = comments.CreatePropertiesDummy();
		}

		/// <summary>
		/// This function is only called when the UI is about to be displayed. Therefore, the
		/// InitializeComponent() call has been moved into here to avoid making users of the
		/// automation model to drive this object pay the UI initialization penalty.
		/// </summary>
		public void OnAfterCreated(EnvDTE.DTE dte) 
		{
			m_applicationObject = dte;
			m_extComments = new ExtensionComments();
			m_extComments.LoadFromRegistry();

			try 
			{
				m_strFileExtensionField = AMResources.GetLocalizedString("ToolsOptionsFileExtensionField");
				m_strBeginCommentField = AMResources.GetLocalizedString("ToolsOptionsBeginCommentsField");
				m_strEndCommentField = AMResources.GetLocalizedString("ToolsOptionsEndCommentsField");
			} 
			catch (System.Exception) 
			{
				m_strFileExtensionField = "";
				m_strBeginCommentField = "";
				m_strEndCommentField = "";
				return;
			}
      
			InitializeComponent();

			PopulateTable();
		}
    
		public void OnCancel() 
		{
			// Reset the internal state.
			m_extComments = new ExtensionComments();
			m_extComments.LoadFromRegistry();
		}

		private void PopulateTable() 
		{
			int nCount = 0;
			ExtensionComment ec;
			System.Data.DataRow newRow;
      
			try 
			{
				dtValues.Clear();
				dtValues.AcceptChanges();
        
				// Attach the DataTable to the DataGrid
				dgEntries.SetDataBinding(dtValues, null);

				nCount = m_extComments.Count();
        
				for (int i = 0; i < nCount; i++) 
				{
					ec = m_extComments[i];
					newRow = dtValues.NewRow();
					newRow["X"] = false;
					newRow[m_strFileExtensionField] = ec.Extensions;
					newRow[m_strBeginCommentField] = ec.BeginComment;
					newRow[m_strEndCommentField] = ec.EndComment;
					dtValues.Rows.Add(newRow);
				}
				dtValues.AcceptChanges();

				cbCollapse.Checked = m_extComments.Collapse;
				cbPromptForTodo.Checked = m_extComments.PromptForTodo;
			} 
			catch (System.Exception except) 
			{
				string s = except.StackTrace;
				string m = except.Message;

				System.Diagnostics.Debug.Assert(false, "Unable to initialize the DataTable." + m);
			}
		}
    
		public void OnHelp() 
		{
			try 
			{
				if (m_applicationObject != null) 
				{
					//          Microsoft.VisualStudio.VSHelp.Help help = (Microsoft.VisualStudio.VSHelp.Help)m_applicationObject.GetObject("Help");
					//          help.DisplayTopicFromF1Keyword("vs.ToolsOptionsPage.Academic.CodeExtractor");
				}
			} 
			catch(System.Exception) 
			{
				// An error thrown means that the shell could not load the help
				// topic. However, we do not need to prompt the user about this,
				// since they'll be presented with a 'topic not found' dialog.
			}
		}

		/// <summary>
		/// This is called when the user leaves the datagrid control. At that point,
		/// we check to make sure that:
		/// - All entries have a begin, end, and extension
		/// - That there are no duplicate extensions
		/// - That no entry has an identical begin & end
		/// If there is an error, we report it to the user and put the cursor
		/// on the entry with the issue.
		/// </summary>
		internal void ValidateGrid() 
		{
			System.Collections.Specialized.StringCollection allExtensions = new System.Collections.Specialized.StringCollection();
			string extensions, begin, end;
			int nRow = 0;
			string []rgExtensions = null;
			char []splitChars = {','};
			string working;

			dtValues.AcceptChanges();
			foreach (System.Data.DataRow row in dtValues.Rows) 
			{
				extensions = (string)row[m_strFileExtensionField];
				begin = (string)row[m_strBeginCommentField];
				end = (string)row[m_strEndCommentField];

				// Non-null or single-space extension(s)?
				if ((extensions == null) || (extensions == "") || (extensions == " ")) 
				{
					System.Windows.Forms.MessageBox.Show(AMResources.GetLocalizedString("ToolsOptionsInvalidExtension"),
						AMResources.GetLocalizedString("ToolsOptionsErrorTitle"),
						System.Windows.Forms.MessageBoxButtons.OK,
						System.Windows.Forms.MessageBoxIcon.Exclamation);
					dgEntries.Select(nRow);
					throw new System.Exception();
				}

				// Non-null or single-space begin comment?
				if ((begin == null) || (begin == "") || (begin == " ")) 
				{
					System.Windows.Forms.MessageBox.Show(AMResources.GetLocalizedString("ToolsOptionsInvalidBegin"),
						AMResources.GetLocalizedString("ToolsOptionsErrorTitle"),
						System.Windows.Forms.MessageBoxButtons.OK,
						System.Windows.Forms.MessageBoxIcon.Exclamation);
					dgEntries.Select(nRow);
					throw new System.Exception();
				}

				// Non-null or single-space end comment?
				if ((end == null) || (end == "") || (end == " ")) 
				{
					System.Windows.Forms.MessageBox.Show(AMResources.GetLocalizedString("ToolsOptionsInvalidEnd"),
						AMResources.GetLocalizedString("ToolsOptionsErrorTitle"),
						System.Windows.Forms.MessageBoxButtons.OK,
						System.Windows.Forms.MessageBoxIcon.Exclamation);
					dgEntries.Select(nRow);
					throw new System.Exception();
				}

				// Identical begin & end comments?
				if (begin == end) 
				{
					System.Windows.Forms.MessageBox.Show(AMResources.GetLocalizedString("ToolsOptionsInvalidBeginAndEnd"),
						AMResources.GetLocalizedString("ToolsOptionsErrorTitle"),
						System.Windows.Forms.MessageBoxButtons.OK,
						System.Windows.Forms.MessageBoxIcon.Exclamation);
					dgEntries.Select(nRow);
					throw new System.Exception();
				}

				// Validate both that there are extensions there and that they aren't
				// already elsewhere in the datagrid.
				rgExtensions = extensions.Split(splitChars);
				foreach (string extension in rgExtensions) 
				{
					working = extension.Trim().ToUpper();

					if (working[0] != '.') 
					{
						System.Windows.Forms.MessageBox.Show(AMResources.GetLocalizedString("ToolsOptionsInvalidExtension"),
							AMResources.GetLocalizedString("ToolsOptionsErrorTitle"),
							System.Windows.Forms.MessageBoxButtons.OK,
							System.Windows.Forms.MessageBoxIcon.Exclamation);
						dgEntries.Select(nRow);
						throw new System.Exception();
					}

					if (allExtensions.Contains(working)) 
					{
						System.Windows.Forms.MessageBox.Show(AMResources.GetLocalizedString("ToolsOptionsInvalidExtensionAlreadyExists"),
							AMResources.GetLocalizedString("ToolsOptionsErrorTitle"),
							System.Windows.Forms.MessageBoxButtons.OK,
							System.Windows.Forms.MessageBoxIcon.Exclamation);
						dgEntries.Select(nRow);
						throw new System.Exception();
					}

					allExtensions.Add(working);
				}

				nRow++;
			}
		}

		/// <summary>
		/// This is automatically called by VS when the Tools Options dialog has had
		/// its OK button hit. If OnAfterCreated() has been called (because the control
		/// was made visible and sited into its ActiveX container), it will have set
		/// the internal member variable, and we should serialize our contents. 
		/// </summary>
		public void OnOK() 
		{
			if (m_extComments != null) 
			{
				try 
				{
          
					// If the user has just entered a value and then clicked the mouse on the OK button, then the
					// DataGrid control will never have had a chance to update its internal data store (the
					// DataTable) to reflect the user's changes.
					try 
					{
						if ((dgEntries.CurrentCell.ColumnNumber >= 1) &&
							(dgEntries.CurrentCell.RowNumber >= 1)) 
						{
							dgEntries.EndEdit(dgEntries.TableStyles[0].GridColumnStyles[dgEntries.CurrentCell.ColumnNumber],
								dgEntries.CurrentCell.RowNumber, false);
						}
					} 
					catch (System.Exception) 
					{
						// Ignore exceptions, since they only mean that the commit failed when the
						// user had *not* made any changes yet.
					}

					// This will throw an exception (after having set the focus back to the correct
					// row and telling the user about their error).
					ValidateGrid();
          
					m_extComments.Clear();
          
					foreach (System.Data.DataRow row in dtValues.Rows) 
					{
						m_extComments.Add(new ExtensionComment((string)row[m_strFileExtensionField],
							(string)row[m_strBeginCommentField],
							(string)row[m_strEndCommentField]));
					}
					m_extComments.Collapse = cbCollapse.Checked;
					m_extComments.PromptForTodo = cbPromptForTodo.Checked;
          
					m_extComments.SaveToRegistry();
          
					// We need to re-load the settings because OnOK does NOT mean that
					// the toolsoptions page is going away; rather, the user has hit
					// the OK button and may come back to it in its CURRENT state.
					m_extComments = new ExtensionComments();
					m_extComments.LoadFromRegistry();
				} 
				catch (System.Exception) 
				{
					// Ignore exception.
				}
			}
		}

		private EnvDTE._DTE m_applicationObject;
		private ExtensionComments m_extComments;
		private string m_strFileExtensionField;
		private string m_strBeginCommentField;
		private string m_strEndCommentField;
		private bool m_fInitialized;
	}
}
