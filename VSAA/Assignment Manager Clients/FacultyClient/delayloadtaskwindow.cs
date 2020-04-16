//
// Copyright © 2000-2003 Microsoft Corporation.  All rights reserved.
//
//
// This source code is licensed under Microsoft Shared Source License
// for the Visual Studio .NET Academic Tools Source Licensing Program
// For a copy of the license, see http://www.msdnaa.net/assignmentmanager/sourcelicense/
//
namespace Microsoft.VisualStudio.Academic.TaskWindow {
  using System;

  /// <summary>
  ///    This class encapsulates the logging of messages to the error window. However, it
  ///    allows delay-loading, only going through the horrifically painful proces of
  ///    retrieving the task window on the first log to it *and* only if GUI use is
  ///    enabled on the DTE automation object. It should also be noted that doing things
  ///    such as obtaining a pointer to the task window will cause it to load (at least,
  ///    as of this build...). We don't want to force the user of this tool to load
  ///    the task window if they had no errors. This functionality doesn't need to be 
  ///    exposed through COM, so it's declared 'internal'.
  /// </summary>
  internal class DelayLoadTaskWindow {
    public DelayLoadTaskWindow(EnvDTE._DTE dte) {
      if (dte == null) {
        throw new System.ArgumentNullException("dte");
      }

      this.dte = dte;
      taskItems = null;
      mainCategory = "Academic";
      subCategory = "Code Extraction";
    }

    /// <summary>
    /// LogErrorToTaskWindow creates a new entry for the specified error in the user's task window.
    /// </summary>
    /// <param name="error"> String describing the error.</param>
    /// <param name="file"> Complete path on disk to the file containing the error.</param>
    /// <param name="line"> Line number containing the described error.</param>
    public bool LogErrorToTaskWindow(string error, string file, int line) {
      if (error == null) {
        throw new System.ArgumentNullException("error");
      }
      if (file == null) {
        throw new System.ArgumentNullException("file");
      }
      if (line < 0) {
        throw new System.ArgumentOutOfRangeException("line");
      }
      try {
        if ((taskItems == null) && 
          (!LoadTaskList())) {
          return false;
        }
		
        taskItems.Add(mainCategory, subCategory, error, EnvDTE.vsTaskPriority.vsTaskPriorityMedium, EnvDTE.vsTaskIcon.vsTaskIconNone, true, file, line, true, true);
        return true;
      } catch (Exception /*e*/) {
        return false;
      }
    }

    /// <summary>
    /// LoadTaskList delay-connects the object to VS7's task list. Note that it returns 
    /// true if it manages to connect to the object and false if either it cannot connect
    /// to the object *or* if the DTE is is not-user-controlled-mode (i.e. ui is being
    /// suppressed!). It also hooks the navigate event so that we can actually the
    /// user's attempt to navigate to items.
    /// </summary>
    private bool LoadTaskList() {
      if (dte == null) {
        throw new System.InvalidOperationException();
      }

      try {
        if (dte.UserControl == false) {
          return false;
        }

        EnvDTE.TaskList tl = dte.Windows.Item(EnvDTE.Constants.vsWindowKindTaskList).Object as EnvDTE.TaskList;
        EnvDTE.TaskItem item = null;
        if (tl == null) {
          return false;
        }

        taskItems = tl.TaskItems;

        tl.Parent.Visible = true;

        // Clear out any old entries
        int i, nItems = taskItems.Count;
        for (i = 1; i <= nItems; i++) {
          item = taskItems.Item(i);
          string cat = item.Category;
          string subcat = item.SubCategory;

          // HACK: VS doesn't keep around subCategories.
          if (item.Category == mainCategory) {
            try { item.Delete(); } catch (System.Exception) {}
          }
        }

        // Register a connection point to handle the Navigated event (otherwise, no
        // navigation will be done when the user double-clicks on an item in the
        // task list!).
        TaskWindow.TaskListEvents evts = new TaskWindow.TaskListEvents(dte);
        dte.Events.get_TaskListEvents(mainCategory).TaskNavigated += new EnvDTE._dispTaskListEvents_TaskNavigatedEventHandler(evts.TaskNavigated);
        
        return true;
      } catch (Exception /*e*/) {
        return false;
      }
    }
		
    private EnvDTE._DTE dte;
    private EnvDTE.TaskItems taskItems;
    private string mainCategory;
    private string subCategory;
  }

	
  class TaskListEvents : Object {
    public TaskListEvents(EnvDTE._DTE dte) {
      this.dte = dte;
    }

    /// <summary>
    /// This method will be invoked when the user double-clicks on the item in
    /// the task list associated this instance of the object.
    /// </summary>
    public void TaskNavigated (EnvDTE.TaskItem taskItem, ref bool Navigated) {
      EnvDTE.Window w = null;
      EnvDTE.TextDocument textDoc = null;

      Navigated = false;

      try {
        w = dte.OpenFile(EnvDTE.Constants.vsViewKindTextView, taskItem.FileName);
        w.Activate();
        textDoc = dte.ActiveDocument.Object("TextDocument") as EnvDTE.TextDocument;

        textDoc.Selection.GotoLine(taskItem.Line, false);
        Navigated = true;
      } catch (Exception /*e*/) {
        Navigated = false;
      }
    }

    private EnvDTE._DTE dte;
  }
}
