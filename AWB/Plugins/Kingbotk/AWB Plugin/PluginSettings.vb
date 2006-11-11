'Namespace AWB.Plugins.SDKSoftware.Kingbotk.Components
Friend NotInheritable Class PluginSettingsControl
    ' tag categories option (where a suitable class= exists; maybe should be in template-specific options)
    ' TODO: (Bot mode) If haven't heard from AWB in a while (e.g. in tracing class), stop and restart it
    ' TODO: Might need a bot-mode tab

    ' XML parm-name constants:
    Private Const conCategoryNameParm As String = "CategoryName"
    Private Const conManuallyAssessParm As String = "ManuallyAssess"
    Private Const conCleanupParm As String = "Cleanup"
    Private Const conLogBadPages As String = "LogBadPages"
    Private Const conLogXHTML As String = "LogXHTML"
    Private Const conLogWiki As String = "LogWiki"
    Private Const conLogVerbose As String = "LogVerbose"
    Private Const conLogFolder As String = "LogFolder"
    Private Const conSkipBadTagsParm As String = "SkipBadTags"
    Private Const conSkipWhenNoChangeParm As String = "SkipWhenNoChange"
    Private Const conAssessmentsAlwaysLeaveACommentParm As String = "AlwaysLeaveAComment"

    ' Statistics:
    Friend WithEvents PluginStats As New Stats
    Private StatLabels As New List(Of Label)

    ' AWB objects:
    Private txtEdit As TextBox
    Private blnBotModeHasBeenOn As Boolean
    Private WithEvents chkBotMode As CheckBox

    'Tracing:
    Friend LogBadPages As Boolean = True, LogXHTML As Boolean = False, LogWiki As Boolean = True, _
       LogVerbose As Boolean = True, LogFolder As String = Application.StartupPath
    Friend Shared WithEvents MyTrace As New MyTrace

    ' AWB processing stopped/started:
    Friend Sub AWBProcessingStart(ByVal webcontrol As WikiFunctions.Browser.WebControl)
        For Each lbl As Label In StatLabels
            If lbl.Text = "" Then lbl.Text = "0"
        Next
        TimerStats1.Visible = True
        TimerStats1.Init(WebControl)

        If MyTrace.HaveOpenFile Then
            MyTrace.WriteBulletedLine("AWB started processing", True, True, True)
        Else
            MyTrace.Initialise(LogBadPages, LogXHTML, LogWiki, LogVerbose, LogFolder)
        End If
        PluginManager.StatusText.Text = "Started"
    End Sub

    ' Properties:
    Private mAssessmentsAlwaysLeaveAComment As Boolean

    Public Property CategoryName() As String
        Get
            Return CategoryTextBox.Text
        End Get
        Set(ByVal value As String)
            CategoryTextBox.Text = value
        End Set
    End Property
    Public Property ManuallyAssess() As Boolean
        Get
            Return ManuallyAssessCheckBox.Checked
        End Get
        Set(ByVal value As Boolean)
            Cleanup = value
        End Set
    End Property
    Public Property Cleanup() As Boolean
        Get
            Return CleanupCheckBox.Checked
        End Get
        Set(ByVal value As Boolean)
            CleanupCheckBox.Checked = value
        End Set
    End Property
    Public Property SkipBadTags() As Boolean
        Get
            Return SkipBadTagsCheckBox.Checked
        End Get
        Set(ByVal value As Boolean)
            SkipBadTagsCheckBox.Checked = value
        End Set
    End Property
    Public Property SkipWhenNoChange() As Boolean
        Get
            Return SkipNoChangesCheckBox.Checked
        End Get
        Set(ByVal value As Boolean)
            SkipNoChangesCheckBox.Checked = value
        End Set
    End Property
    Public Property AssessmentsAlwaysLeaveAComment() As Boolean
        Get
            Return mAssessmentsAlwaysLeaveAComment
        End Get
        Set(ByVal value As Boolean)
            mAssessmentsAlwaysLeaveAComment = value
        End Set
    End Property

    ' XML interface:
    Public Sub ReadXML(ByVal Reader As System.Xml.XmlTextReader)
        CategoryName = PluginManager.XMLReadString(Reader, conCategoryNameParm, CategoryName)
        ManuallyAssess = PluginManager.XMLReadBoolean(Reader, conManuallyAssessParm, ManuallyAssess)
        Cleanup = PluginManager.XMLReadBoolean(Reader, conCleanupParm, Cleanup)
        LogBadPages = PluginManager.XMLReadBoolean(Reader, conLogBadPages, LogBadPages)
        LogFolder = PluginManager.XMLReadString(Reader, conLogFolder, LogFolder)
        LogVerbose = PluginManager.XMLReadBoolean(Reader, conLogVerbose, LogVerbose)
        LogWiki = PluginManager.XMLReadBoolean(Reader, conLogWiki, LogWiki)
        LogXHTML = PluginManager.XMLReadBoolean(Reader, conLogXHTML, LogXHTML)
        SkipBadTags = PluginManager.XMLReadBoolean(Reader, conSkipBadTagsParm, SkipBadTags)
        SkipWhenNoChange = PluginManager.XMLReadBoolean(Reader, conSkipWhenNoChangeParm, SkipWhenNoChange)
        AssessmentsAlwaysLeaveAComment = PluginManager.XMLReadBoolean(Reader, _
           conAssessmentsAlwaysLeaveACommentParm, AssessmentsAlwaysLeaveAComment)
    End Sub
    Public Sub Reset()
        CategoryName = ""
        ManuallyAssess = False
        Cleanup = False
        PluginStats = New Stats
        ' don't change logging settings
        MyTrace.WriteBulletedLine("Reset", False, True, True)
        AssessmentsAlwaysLeaveAComment = False
    End Sub
    Public Sub WriteXML(ByVal Writer As System.Xml.XmlTextWriter)
        Writer.WriteAttributeString(conCategoryNameParm, CategoryName)
        Writer.WriteAttributeString(conManuallyAssessParm, ManuallyAssess.ToString)
        Writer.WriteAttributeString(conCleanupParm, Cleanup.ToString)
        Writer.WriteAttributeString(conLogBadPages, LogBadPages.ToString)
        Writer.WriteAttributeString(conLogFolder, LogFolder)
        Writer.WriteAttributeString(conLogVerbose, LogVerbose.ToString)
        Writer.WriteAttributeString(conLogWiki, LogWiki.ToString)
        Writer.WriteAttributeString(conLogXHTML, LogXHTML.ToString)
        Writer.WriteAttributeString(conSkipBadTagsParm, SkipBadTags.ToString)
        Writer.WriteAttributeString(conSkipWhenNoChangeParm, SkipWhenNoChange.ToString)
        Writer.WriteAttributeString(conAssessmentsAlwaysLeaveACommentParm, AssessmentsAlwaysLeaveAComment.ToString)
    End Sub

    ' Event handlers - menu items:
    Private Sub LoggingToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
    Handles LoggingToolStripMenuItem.Click
        Dim frm As New Logging(Me)
        If frm.ShowDialog = DialogResult.OK And MyTrace.HaveOpenFile Then
            MessageBox.Show("Log files are open. Your changes will take affect after restarting AWB.", _
            "Restart AWB", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Private Sub SetAWBToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
    Handles SetAWBToolStripMenuItem.Click
        ' TODO: SetAWBToolStripMenuItem_Click - do AWB settings (basically turn everything off)
    End Sub
    Private Sub LoadSettingsToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
    Handles LoadSettingsToolStripMenuItem.Click
        ' TODO: LoadSettingsToolStripMenuItem_Click
    End Sub
    Private Sub SaveSettingsToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
    Handles SaveSettingsToolStripMenuItem.Click
        ' TODO: SaveSettingsToolStripMenuItem_Click
    End Sub
    Private Sub LivingPeopleToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
    Handles LivingPeopleToolStripMenuItem.Click
        CategoryName = "Living people"
    End Sub
    Private Sub ClearMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ClearToolStripMenuItem.Click
        CategoryName = ""
    End Sub
    Private Sub MenuAbout_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles MenuAbout.Click
        Dim about As New AboutBox(String.Format("Version {0}", _
           System.Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString))
        about.Show()
    End Sub
    Private Sub MenuHelp_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles MenuHelp.Click
        System.Diagnostics.Process.Start("http://en.wikipedia.org/wiki/User:Kingbotk/Plugin/User guide")
    End Sub
    Private Sub MenuHelpReleaseNotes_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles MenuHelpReleaseNotes.Click
        System.Diagnostics.Process.Start("http://en.wikipedia.org/wiki/User:Kingbotk/Plugin")
    End Sub
    Private Sub CutToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
    Handles CutToolStripMenuItem.Click
        CategoryTextBox.Cut()
    End Sub
    Private Sub PasteToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
    Handles PasteToolStripMenuItem.Click
        CategoryTextBox.Paste()
    End Sub
    Private Sub CopyToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
    Handles CopyToolStripMenuItem.Click
        CategoryTextBox.Copy()
    End Sub

    ' Event handlers - our controls:
    Private Sub ManuallyAssessCheckBox_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) _
    Handles ManuallyAssessCheckBox.CheckedChanged
        If ManuallyAssess Then
            chkBotMode.Enabled = False
            chkBotMode.Checked = False
            SkipBadTagsCheckBox.Checked = False
            SkipBadTagsCheckBox.Enabled = False
            SkipNoChangesCheckBox.Checked = False
            SkipNoChangesCheckBox.Enabled = False
        Else
            If blnBotModeHasBeenOn Then ' ManuallyAssessed is now unchecked, bot has been previously enabled
                chkBotMode.Enabled = True
            End If
            SkipBadTagsCheckBox.Enabled = True
            SkipNoChangesCheckBox.Enabled = True
        End If

        CleanupCheckBox.Checked = ManuallyAssess
        CleanupCheckBox.Enabled = ManuallyAssess
        MyTrace.WriteBulletedLine(String.Format("Manual assessments mode on: {0}", _
           ManuallyAssess.ToString), True, True, True)
    End Sub

    ' Event handlers - AWB components (some additionally double-handled in Plugin Manager):
    Friend Sub AWBButtonsEnabledHandler(ByVal sender As Object, ByVal e As EventArgs)
        Dim btn As Button = DirectCast(sender, Button)

        DirectCast(Me.AWBGroupBox.Controls(btn.Name), Button).Enabled = btn.Enabled
    End Sub
    Friend Sub AWBSkipButtonClickEventHandler(ByVal sender As Object, ByVal e As EventArgs) _
    Handles btnIgnore.Click
        If Not ManuallyAssess Then ' If ManuallyAssess is True, defer to the handler in Assessments class
            MyTrace.SkippedArticle("User", "User clicked Ignore")
            PluginStats.SkippedMiscellaneousIncrement(True)
        End If
    End Sub
    Friend Sub AWBArticleStatsLabelChangeEventHandler(ByVal sender As Object, ByVal e As EventArgs)
        Dim lbl As Label = DirectCast(sender, Label)

        DirectCast(Me.ArticleStatsGroupBox.Controls(lbl.Name), Label).Text = lbl.Text
    End Sub
    Private Sub chkBotMode_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) _
    Handles chkBotMode.CheckedChanged
        If DirectCast(sender, CheckBox).Checked Then
            SkipBadTagsCheckBox.Checked = True
            SkipBadTagsCheckBox.Enabled = False
            SkipNoChangesCheckBox.Checked = True
            SkipNoChangesCheckBox.Enabled = False
            lblAWBNudges.Visible = True
        Else
            SkipBadTagsCheckBox.Enabled = True
            SkipNoChangesCheckBox.Enabled = True
            lblAWBNudges.Visible = False
        End If
    End Sub
    Private Sub chkBotMode_EnabledChanged(ByVal sender As Object, ByVal e As EventArgs) _
    Handles chkBotMode.EnabledChanged
        If DirectCast(sender, CheckBox).Enabled Then blnBotModeHasBeenOn = True
    End Sub

    ' Event handlers - plugin stats:
    Private Sub PluginStats_New(ByVal val As Integer) Handles PluginStats.New
        lblNew.Text = val.ToString
    End Sub
    Private Sub PluginStats_SkipBadTag(ByVal val As Integer) Handles PluginStats.SkipBadTag
        lblBadTag.Text = val.ToString
    End Sub
    Private Sub PluginStats_SkipMisc(ByVal val As Integer) Handles PluginStats.SkipMisc
        lblSkipped.Text = val.ToString
    End Sub
    Private Sub PluginStats_SkipNamespace(ByVal val As Integer) Handles PluginStats.SkipNamespace
        lblNamespace.Text = val.ToString
    End Sub
    Private Sub PluginStats_SkipNoChange(ByVal val As Integer) Handles PluginStats.SkipNoChange
        lblNoChange.Text = val.ToString
    End Sub
    Private Sub PluginStats_Tagged(ByVal val As Integer) Handles PluginStats.evTagged
        lblTagged.Text = val.ToString
    End Sub
    Private Sub PluginStats_RedLink(ByVal val As Integer) Handles PluginStats.RedLink
        lblRedlink.Text = val.ToString
    End Sub
#Region "TextInsertHandlers"
    ' Event handlers: Insert-text context menu:
    Private Sub StubClassMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles StubClassMenuItem.Click
        txtEdit.SelectedText = "|class=Stub"
    End Sub
    Private Sub StartClassMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles StartClassMenuItem.Click
        txtEdit.SelectedText = "|class=Start"
    End Sub
    Private Sub BClassMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BClassMenuItem.Click
        txtEdit.SelectedText = "|class=B"
    End Sub
    Private Sub GAClassMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles GAClassMenuItem.Click
        txtEdit.SelectedText = "|class=GA"
    End Sub
    Private Sub AClassMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles AClassMenuItem.Click
        txtEdit.SelectedText = "|class=A"
    End Sub
    Private Sub FAClassMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles FAClassMenuItem.Click
        txtEdit.SelectedText = "|class=FA"
    End Sub
    Private Sub NeededClassMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles NeededClassMenuItem.Click
        txtEdit.SelectedText = "|class=Needed"
    End Sub
    Private Sub CatClassMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles CatClassMenuItem.Click
        txtEdit.SelectedText = "|class=Cat"
    End Sub
    Private Sub DabClassMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles DabClassMenuItem.Click
        txtEdit.SelectedText = "|class=Dab"
    End Sub
    Private Sub TemplateClassMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles TemplateClassMenuItem.Click
        txtEdit.SelectedText = "|class=Template"
    End Sub
    Private Sub NAClassMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles NAClassMenuItem.Click
        txtEdit.SelectedText = "|class=NA"
    End Sub
    Private Sub LowImportanceMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles LowImportanceMenuItem.Click
        txtEdit.SelectedText = "|importance=Low"
    End Sub
    Private Sub MidImportanceMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles MidImportanceMenuItem.Click
        txtEdit.SelectedText = "|importance=Mid"
    End Sub
    Private Sub HighImportanceMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles HighImportanceMenuItem.Click
        txtEdit.SelectedText = "|importance=High"
    End Sub
    Private Sub TopImportanceMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles TopImportanceMenuItem.Click
        txtEdit.SelectedText = "|importance=Top"
    End Sub
    Private Sub NAImportanceMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles NAImportanceMenuItem.Click
        txtEdit.SelectedText = "|importance=NA"
    End Sub
    Private Sub LowPriorityMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles LowPriorityMenuItem.Click
        txtEdit.SelectedText = "|priority=Low"
    End Sub
    Private Sub MidPriorityMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles MidPriorityMenuItem.Click
        txtEdit.SelectedText = "|priority=Mid"
    End Sub
    Private Sub HighPriorityMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles HighPriorityMenuItem.Click
        txtEdit.SelectedText = "|priority=High"
    End Sub
    Private Sub TopPriorityMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles TopPriorityMenuItem.Click
        txtEdit.SelectedText = "|priority=Top"
    End Sub
    Private Sub NAPriorityMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles NAPriorityMenuItem.Click
        txtEdit.SelectedText = "|priority=NA"
    End Sub
#End Region

    ' Statistics:
    Friend NotInheritable Class Stats
        Private mTagged As Integer, mNewArticles As Integer, mSkipped As Integer, mSkippedNoChange As Integer
        Private mSkippedBadTag As Integer, mSkippedNamespace As Integer, mRedLinks As Integer

        Friend Event SkipMisc(ByVal val As Integer)
        Friend Event SkipNoChange(ByVal val As Integer)
        Friend Event SkipBadTag(ByVal val As Integer)
        Friend Event SkipNamespace(ByVal val As Integer)
        Friend Event evTagged(ByVal val As Integer)
        Friend Event [New](ByVal val As Integer)
        Friend Event RedLink(ByVal val As Integer)

        Friend Property Tagged() As Integer
            Get
                Return mTagged
            End Get
            Set(ByVal value As Integer)
                mTagged = value
                RaiseEvent evTagged(value)
            End Set
        End Property
        Friend Property NewArticles() As Integer
            Get
                Return mNewArticles
            End Get
            Set(ByVal value As Integer)
                mNewArticles = value
                RaiseEvent [New](value)
            End Set
        End Property
        Private Property Skipped() As Integer
            Get
                Return mSkipped
            End Get
            Set(ByVal value As Integer)
                mSkipped = value
                RaiseEvent SkipMisc(value)
            End Set
        End Property
        Public Sub SkippedMiscellaneousIncrement()
            Skipped += 1
        End Sub
        Public Sub SkippedMiscellaneousIncrement(ByVal DeincrementTagged As Boolean)
            Skipped += 1
            If DeincrementTagged Then Tagged -= 1
        End Sub
        Friend Property SkippedRedLink() As Integer
            Get
                Return mRedLinks
            End Get
            Set(ByVal value As Integer)
                mRedLinks = value
                RaiseEvent RedLink(value)
            End Set
        End Property
        Public Sub SkippedRedLinkIncrement()
            Skipped += 1
            SkippedRedLink += 1
        End Sub
        Private Property SkippedNoChange() As Integer
            Get
                Return mSkippedNoChange
            End Get
            Set(ByVal value As Integer)
                mSkippedNoChange = value
                RaiseEvent SkipNoChange(value)
            End Set
        End Property
        Public Sub SkippedNoChangeIncrement()
            SkippedNoChange += 1
            Skipped += 1
        End Sub
        Private Property SkippedBadTag() As Integer
            Get
                Return mSkippedBadTag
            End Get
            Set(ByVal value As Integer)
                mSkippedBadTag = value
                RaiseEvent SkipBadTag(value)
            End Set
        End Property
        Public Sub SkippedBadTagIncrement()
            SkippedBadTag += 1
            Skipped += 1
        End Sub
        Private Property SkippedNamespace() As Integer
            Get
                Return mSkippedNamespace
            End Get
            Set(ByVal value As Integer)
                mSkippedNamespace = value
                RaiseEvent SkipNamespace(value)
            End Set
        End Property
        Public Sub SkippedNamespaceIncrement()
            SkippedNamespace += 1
            Skipped += 1
        End Sub

        Public Sub New()
            Skipped = 0
            SkippedBadTag = 0
            SkippedNamespace = 0
            SkippedNoChange = 0
            NewArticles = 0
            Tagged = 0
            SkippedRedLink = 0
        End Sub
    End Class

    Public Sub New(ByVal txt As TextBox, ByVal chk As CheckBox)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        StatLabels.AddRange({lblTagged, lblSkipped, lblNoChange, lblBadTag, lblNamespace, lblNew, lblRedlink})

        txtEdit = txt
        chkBotMode = chk
    End Sub
End Class
'end namespace