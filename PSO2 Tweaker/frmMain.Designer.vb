Imports PSO2_Tweaker.My

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmMain
    Inherits DevComponents.DotNetBar.Office2007Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmMain))
        Me.txtHTML = New System.Windows.Forms.TextBox()
        Me.rtbDebug = New System.Windows.Forms.RichTextBox()
        Me.cmsTextBarOptions = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.CopyAllTextToClipboardToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.chkRemovePC = New DevComponents.DotNetBar.Controls.CheckBoxX()
        Me.chkRemoveVita = New DevComponents.DotNetBar.Controls.CheckBoxX()
        Me.PBMainBar = New DevComponents.DotNetBar.Controls.ProgressBarX()
        Me.chkRemoveNVidia = New DevComponents.DotNetBar.Controls.CheckBoxX()
        Me.chkRemoveSEGA = New DevComponents.DotNetBar.Controls.CheckBoxX()
        Me.chkSwapOP = New DevComponents.DotNetBar.Controls.CheckBoxX()
        Me.lblDirectoryLabel = New DevComponents.DotNetBar.LabelX()
        Me.lblDirectory = New DevComponents.DotNetBar.LabelX()
        Me.chkRestorePC = New DevComponents.DotNetBar.Controls.CheckBoxX()
        Me.chkRestoreVita = New DevComponents.DotNetBar.Controls.CheckBoxX()
        Me.chkRestoreNVidia = New DevComponents.DotNetBar.Controls.CheckBoxX()
        Me.chkRestoreSEGA = New DevComponents.DotNetBar.Controls.CheckBoxX()
        Me.btnApplyChanges = New DevComponents.DotNetBar.ButtonX()
        Me.btnLaunchPSO2 = New DevComponents.DotNetBar.ButtonX()
        Me.cmsProgressBar = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.tsmRestartDownload = New System.Windows.Forms.ToolStripMenuItem()
        Me.CancelDownloadToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CancelProcessToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RibbonControl1 = New DevComponents.DotNetBar.RibbonControl()
        Me.Office2007StartButton1 = New DevComponents.DotNetBar.Office2007StartButton()
        Me.ItemContainer1 = New DevComponents.DotNetBar.ItemContainer()
        Me.ItemContainer2 = New DevComponents.DotNetBar.ItemContainer()
        Me.ItemContainer3 = New DevComponents.DotNetBar.ItemContainer()
        Me.btnLaunchPSO2fromORB = New DevComponents.DotNetBar.ButtonItem()
        Me.btnSelectPSODir = New DevComponents.DotNetBar.ButtonItem()
        Me.ButtonInstall = New DevComponents.DotNetBar.ButtonItem()
        Me.btnENPatch = New DevComponents.DotNetBar.ButtonItem()
        Me.btnLargeFilesTRANSAM = New DevComponents.DotNetBar.ButtonItem()
        Me.btnStoryPatchNew = New DevComponents.DotNetBar.ButtonItem()
        Me.btnInstallGermanPatch = New DevComponents.DotNetBar.ButtonItem()
        Me.btnRussianPatch = New DevComponents.DotNetBar.ButtonItem()
        Me.btnRussianBigFiles = New DevComponents.DotNetBar.ButtonItem()
        Me.btnInstallSpanishPatch = New DevComponents.DotNetBar.ButtonItem()
        Me.btnRestoreBackups = New DevComponents.DotNetBar.ButtonItem()
        Me.btnRestoreENBackup = New DevComponents.DotNetBar.ButtonItem()
        Me.btnRestoreLargeFilesBackup = New DevComponents.DotNetBar.ButtonItem()
        Me.btnRestoreStoryBackup = New DevComponents.DotNetBar.ButtonItem()
        Me.btnRestoreJPNames = New DevComponents.DotNetBar.ButtonItem()
        Me.btnRestoreJPETrials = New DevComponents.DotNetBar.ButtonItem()
        Me.ButtonItem1 = New DevComponents.DotNetBar.ButtonItem()
        Me.btnUninstallENPatch = New DevComponents.DotNetBar.ButtonItem()
        Me.btnUninstallLargeFiles = New DevComponents.DotNetBar.ButtonItem()
        Me.btnUninstallStory = New DevComponents.DotNetBar.ButtonItem()
        Me.btnJPETrials = New DevComponents.DotNetBar.ButtonItem()
        Me.btnJPEnemyNames = New DevComponents.DotNetBar.ButtonItem()
        Me.ButtonItem2 = New DevComponents.DotNetBar.ButtonItem()
        Me.btnAnalyze = New DevComponents.DotNetBar.ButtonItem()
        Me.Button2 = New DevComponents.DotNetBar.ButtonItem()
        Me.ButtonItem10 = New DevComponents.DotNetBar.ButtonItem()
        Me.ButtonItem4 = New DevComponents.DotNetBar.ButtonItem()
        Me.BtnUpdatePso2 = New DevComponents.DotNetBar.ButtonItem()
        Me.btnGameguard = New DevComponents.DotNetBar.ButtonItem()
        Me.btnFixPSO2EXEs = New DevComponents.DotNetBar.ButtonItem()
        Me.btnFixPermissions = New DevComponents.DotNetBar.ButtonItem()
        Me.btnCopyInfo = New DevComponents.DotNetBar.ButtonItem()
        Me.ButtonItem17 = New DevComponents.DotNetBar.ButtonItem()
        Me.btnResetTweaker = New DevComponents.DotNetBar.ButtonItem()
        Me.btnResumePatching = New DevComponents.DotNetBar.ButtonItem()
        Me.btnTerminate = New DevComponents.DotNetBar.ButtonItem()
        Me.btnOtherStuff = New DevComponents.DotNetBar.ButtonItem()
        Me.ButtonItem7 = New DevComponents.DotNetBar.ButtonItem()
        Me.btnPredownloadLobbyVideos = New DevComponents.DotNetBar.ButtonItem()
        Me.btnChooseProxyServer = New DevComponents.DotNetBar.ButtonItem()
        Me.btnRevertPSO2ProxyToJP = New DevComponents.DotNetBar.ButtonItem()
        Me.btnClearSACache = New DevComponents.DotNetBar.ButtonItem()
        Me.btnInstallPSO2 = New DevComponents.DotNetBar.ButtonItem()
        Me.ButtonItem3 = New DevComponents.DotNetBar.ButtonItem()
        Me.btnBumped = New DevComponents.DotNetBar.ButtonItem()
        Me.btnCirno = New DevComponents.DotNetBar.ButtonItem()
        Me.btnArksCash = New DevComponents.DotNetBar.ButtonItem()
        Me.btnErrors = New DevComponents.DotNetBar.ButtonItem()
        Me.btnOfficialPSO2JP = New DevComponents.DotNetBar.ButtonItem()
        Me.btnRegistration = New DevComponents.DotNetBar.ButtonItem()
        Me.btnTweaker = New DevComponents.DotNetBar.ButtonItem()
        Me.btnRunPSO2Linux = New DevComponents.DotNetBar.ButtonItem()
        Me.btnSymbolEditor = New DevComponents.DotNetBar.ButtonItem()
        Me.btnEXPFULL = New DevComponents.DotNetBar.ButtonItem()
        Me.btnDonations = New DevComponents.DotNetBar.ButtonItem()
        Me.btnDonateToBumped = New DevComponents.DotNetBar.ButtonItem()
        Me.btnDonateToCirno = New DevComponents.DotNetBar.ButtonItem()
        Me.btnDonateToTweaker = New DevComponents.DotNetBar.ButtonItem()
        Me.btnQUANTUMSYSTEM = New DevComponents.DotNetBar.ButtonItem()
        Me.btnDownloadPrepatch = New DevComponents.DotNetBar.ButtonItem()
        Me.btnCheckForStoryUpdates = New DevComponents.DotNetBar.ButtonItem()
        Me.chkAlwaysOnTop = New DevComponents.DotNetBar.CheckBoxItem()
        Me.chkItemTranslation = New DevComponents.DotNetBar.CheckBoxItem()
        Me.btnConfigureItemTranslation = New DevComponents.DotNetBar.ButtonItem()
        Me.ItemContainer4 = New DevComponents.DotNetBar.ItemContainer()
        Me.btnPlugins = New DevComponents.DotNetBar.ButtonItem()
        Me.btnPSO2Options = New DevComponents.DotNetBar.ButtonItem()
        Me.btnOptions = New DevComponents.DotNetBar.ButtonItem()
        Me.btnExit = New DevComponents.DotNetBar.ButtonItem()
        Me.btnAnnouncements = New DevComponents.DotNetBar.ButtonX()
        Me.WebBrowser4 = New System.Windows.Forms.WebBrowser()
        Me.txtPSO2DefaultINI = New System.Windows.Forms.TextBox()
        Me.tmrWaitingforPSO2 = New System.Windows.Forms.Timer(Me.components)
        Me.txtFiles = New System.Windows.Forms.TextBox()
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.WebBrowser1 = New System.Windows.Forms.WebBrowser()
        Me.DLS = New PSO2_Tweaker.My.MyWebClient()
        Me.cmsTextBarOptions.SuspendLayout()
        Me.cmsProgressBar.SuspendLayout()
        Me.SuspendLayout()
        '
        'txtHTML
        '
        resources.ApplyResources(Me.txtHTML, "txtHTML")
        Me.txtHTML.Name = "txtHTML"
        '
        'rtbDebug
        '
        Me.rtbDebug.BackColor = System.Drawing.Color.White
        Me.rtbDebug.ContextMenuStrip = Me.cmsTextBarOptions
        Me.rtbDebug.HideSelection = False
        resources.ApplyResources(Me.rtbDebug, "rtbDebug")
        Me.rtbDebug.Name = "rtbDebug"
        Me.rtbDebug.ReadOnly = True
        Me.rtbDebug.TabStop = False
        '
        'cmsTextBarOptions
        '
        Me.cmsTextBarOptions.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CopyAllTextToClipboardToolStripMenuItem})
        Me.cmsTextBarOptions.Name = "cmsTextBarOptions"
        resources.ApplyResources(Me.cmsTextBarOptions, "cmsTextBarOptions")
        '
        'CopyAllTextToClipboardToolStripMenuItem
        '
        Me.CopyAllTextToClipboardToolStripMenuItem.Name = "CopyAllTextToClipboardToolStripMenuItem"
        resources.ApplyResources(Me.CopyAllTextToClipboardToolStripMenuItem, "CopyAllTextToClipboardToolStripMenuItem")
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenPatchDialog"
        '
        'chkRemovePC
        '
        resources.ApplyResources(Me.chkRemovePC, "chkRemovePC")
        Me.chkRemovePC.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.chkRemovePC.BackgroundStyle.Class = ""
        Me.chkRemovePC.Name = "chkRemovePC"
        '
        'chkRemoveVita
        '
        resources.ApplyResources(Me.chkRemoveVita, "chkRemoveVita")
        Me.chkRemoveVita.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.chkRemoveVita.BackgroundStyle.Class = ""
        Me.chkRemoveVita.Name = "chkRemoveVita"
        '
        'PBMainBar
        '
        Me.PBMainBar.BackColor = System.Drawing.Color.Transparent
        resources.ApplyResources(Me.PBMainBar, "PBMainBar")
        '
        '
        '
        Me.PBMainBar.BackgroundStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(127, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.PBMainBar.BackgroundStyle.BackColor2 = System.Drawing.Color.FromArgb(CType(CType(127, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.PBMainBar.BackgroundStyle.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.PBMainBar.BackgroundStyle.BorderBottomWidth = 1
        Me.PBMainBar.BackgroundStyle.BorderColor = System.Drawing.Color.White
        Me.PBMainBar.BackgroundStyle.BorderColorLightSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarCaptionBackground2
        Me.PBMainBar.BackgroundStyle.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.PBMainBar.BackgroundStyle.BorderLeftWidth = 1
        Me.PBMainBar.BackgroundStyle.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.PBMainBar.BackgroundStyle.BorderRightWidth = 1
        Me.PBMainBar.BackgroundStyle.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.PBMainBar.BackgroundStyle.BorderTopWidth = 1
        Me.PBMainBar.BackgroundStyle.Class = ""
        Me.PBMainBar.BackgroundStyle.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center
        Me.PBMainBar.BackgroundStyle.TextColor = System.Drawing.Color.Black
        Me.PBMainBar.ChunkColor = System.Drawing.Color.Green
        Me.PBMainBar.ChunkGradientAngle = 200
        Me.PBMainBar.Name = "PBMainBar"
        Me.PBMainBar.Style = DevComponents.DotNetBar.eDotNetBarStyle.Office2000
        Me.PBMainBar.TabStop = False
        Me.PBMainBar.TextVisible = True
        '
        'chkRemoveNVidia
        '
        resources.ApplyResources(Me.chkRemoveNVidia, "chkRemoveNVidia")
        Me.chkRemoveNVidia.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.chkRemoveNVidia.BackgroundStyle.Class = ""
        Me.chkRemoveNVidia.Name = "chkRemoveNVidia"
        '
        'chkRemoveSEGA
        '
        resources.ApplyResources(Me.chkRemoveSEGA, "chkRemoveSEGA")
        Me.chkRemoveSEGA.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.chkRemoveSEGA.BackgroundStyle.Class = ""
        Me.chkRemoveSEGA.Name = "chkRemoveSEGA"
        '
        'chkSwapOP
        '
        resources.ApplyResources(Me.chkSwapOP, "chkSwapOP")
        Me.chkSwapOP.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.chkSwapOP.BackgroundStyle.Class = ""
        Me.chkSwapOP.Name = "chkSwapOP"
        '
        'lblDirectoryLabel
        '
        resources.ApplyResources(Me.lblDirectoryLabel, "lblDirectoryLabel")
        Me.lblDirectoryLabel.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.lblDirectoryLabel.BackgroundStyle.Class = ""
        Me.lblDirectoryLabel.Name = "lblDirectoryLabel"
        '
        'lblDirectory
        '
        Me.lblDirectory.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.lblDirectory.BackgroundStyle.Class = ""
        resources.ApplyResources(Me.lblDirectory, "lblDirectory")
        Me.lblDirectory.Name = "lblDirectory"
        '
        'chkRestorePC
        '
        resources.ApplyResources(Me.chkRestorePC, "chkRestorePC")
        Me.chkRestorePC.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.chkRestorePC.BackgroundStyle.Class = ""
        Me.chkRestorePC.Name = "chkRestorePC"
        '
        'chkRestoreVita
        '
        resources.ApplyResources(Me.chkRestoreVita, "chkRestoreVita")
        Me.chkRestoreVita.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.chkRestoreVita.BackgroundStyle.Class = ""
        Me.chkRestoreVita.Name = "chkRestoreVita"
        '
        'chkRestoreNVidia
        '
        resources.ApplyResources(Me.chkRestoreNVidia, "chkRestoreNVidia")
        Me.chkRestoreNVidia.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.chkRestoreNVidia.BackgroundStyle.Class = ""
        Me.chkRestoreNVidia.Name = "chkRestoreNVidia"
        '
        'chkRestoreSEGA
        '
        resources.ApplyResources(Me.chkRestoreSEGA, "chkRestoreSEGA")
        Me.chkRestoreSEGA.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.chkRestoreSEGA.BackgroundStyle.Class = ""
        Me.chkRestoreSEGA.Name = "chkRestoreSEGA"
        '
        'btnApplyChanges
        '
        Me.btnApplyChanges.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnApplyChanges.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground
        resources.ApplyResources(Me.btnApplyChanges, "btnApplyChanges")
        Me.btnApplyChanges.Name = "btnApplyChanges"
        Me.btnApplyChanges.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        '
        'btnLaunchPSO2
        '
        Me.btnLaunchPSO2.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnLaunchPSO2.BackColor = System.Drawing.Color.Transparent
        Me.btnLaunchPSO2.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground
        resources.ApplyResources(Me.btnLaunchPSO2, "btnLaunchPSO2")
        Me.btnLaunchPSO2.Name = "btnLaunchPSO2"
        Me.btnLaunchPSO2.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        '
        'cmsProgressBar
        '
        Me.cmsProgressBar.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsmRestartDownload, Me.CancelDownloadToolStripMenuItem, Me.CancelProcessToolStripMenuItem})
        Me.cmsProgressBar.Name = "ContextMenuStrip1"
        resources.ApplyResources(Me.cmsProgressBar, "cmsProgressBar")
        '
        'tsmRestartDownload
        '
        Me.tsmRestartDownload.Name = "tsmRestartDownload"
        resources.ApplyResources(Me.tsmRestartDownload, "tsmRestartDownload")
        '
        'CancelDownloadToolStripMenuItem
        '
        Me.CancelDownloadToolStripMenuItem.Name = "CancelDownloadToolStripMenuItem"
        resources.ApplyResources(Me.CancelDownloadToolStripMenuItem, "CancelDownloadToolStripMenuItem")
        '
        'CancelProcessToolStripMenuItem
        '
        Me.CancelProcessToolStripMenuItem.Name = "CancelProcessToolStripMenuItem"
        resources.ApplyResources(Me.CancelProcessToolStripMenuItem, "CancelProcessToolStripMenuItem")
        '
        'RibbonControl1
        '
        '
        '
        '
        Me.RibbonControl1.BackgroundStyle.Class = ""
        Me.RibbonControl1.CaptionVisible = True
        Me.RibbonControl1.Cursor = System.Windows.Forms.Cursors.Default
        resources.ApplyResources(Me.RibbonControl1, "RibbonControl1")
        Me.RibbonControl1.KeyTipsFont = New System.Drawing.Font("Tahoma", 7.0!)
        Me.RibbonControl1.Name = "RibbonControl1"
        Me.RibbonControl1.QuickToolbarItems.AddRange(New DevComponents.DotNetBar.BaseItem() {Me.Office2007StartButton1})
        Me.RibbonControl1.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.RibbonControl1.TabGroupHeight = 14
        '
        'Office2007StartButton1
        '
        Me.Office2007StartButton1.AutoExpandOnClick = True
        Me.Office2007StartButton1.CanCustomize = False
        Me.Office2007StartButton1.HotTrackingStyle = DevComponents.DotNetBar.eHotTrackingStyle.Image
        Me.Office2007StartButton1.ImagePaddingHorizontal = 2
        Me.Office2007StartButton1.ImagePaddingVertical = 2
        Me.Office2007StartButton1.Name = "Office2007StartButton1"
        Me.Office2007StartButton1.ShowSubItems = False
        Me.Office2007StartButton1.SubItems.AddRange(New DevComponents.DotNetBar.BaseItem() {Me.ItemContainer1})
        resources.ApplyResources(Me.Office2007StartButton1, "Office2007StartButton1")
        '
        'ItemContainer1
        '
        '
        '
        '
        Me.ItemContainer1.BackgroundStyle.Class = "RibbonFileMenuContainer"
        Me.ItemContainer1.LayoutOrientation = DevComponents.DotNetBar.eOrientation.Vertical
        Me.ItemContainer1.Name = "ItemContainer1"
        Me.ItemContainer1.SubItems.AddRange(New DevComponents.DotNetBar.BaseItem() {Me.ItemContainer2, Me.ItemContainer4})
        '
        'ItemContainer2
        '
        '
        '
        '
        Me.ItemContainer2.BackgroundStyle.Class = "RibbonFileMenuTwoColumnContainer"
        Me.ItemContainer2.ItemSpacing = 0
        Me.ItemContainer2.Name = "ItemContainer2"
        Me.ItemContainer2.SubItems.AddRange(New DevComponents.DotNetBar.BaseItem() {Me.ItemContainer3})
        '
        'ItemContainer3
        '
        '
        '
        '
        Me.ItemContainer3.BackgroundStyle.Class = "RibbonFileMenuColumnOneContainer"
        Me.ItemContainer3.LayoutOrientation = DevComponents.DotNetBar.eOrientation.Vertical
        Me.ItemContainer3.MinimumSize = New System.Drawing.Size(120, 0)
        Me.ItemContainer3.Name = "ItemContainer3"
        Me.ItemContainer3.SubItems.AddRange(New DevComponents.DotNetBar.BaseItem() {Me.btnLaunchPSO2fromORB, Me.btnSelectPSODir, Me.ButtonInstall, Me.btnRestoreBackups, Me.ButtonItem1, Me.ButtonItem2, Me.btnOtherStuff, Me.ButtonItem3, Me.btnDonations, Me.btnQUANTUMSYSTEM, Me.btnDownloadPrepatch, Me.btnCheckForStoryUpdates, Me.chkAlwaysOnTop, Me.chkItemTranslation, Me.btnConfigureItemTranslation})
        '
        'btnLaunchPSO2fromORB
        '
        Me.btnLaunchPSO2fromORB.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText
        Me.btnLaunchPSO2fromORB.Name = "btnLaunchPSO2fromORB"
        Me.btnLaunchPSO2fromORB.SubItemsExpandWidth = 24
        resources.ApplyResources(Me.btnLaunchPSO2fromORB, "btnLaunchPSO2fromORB")
        Me.btnLaunchPSO2fromORB.Visible = False
        '
        'btnSelectPSODir
        '
        Me.btnSelectPSODir.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText
        Me.btnSelectPSODir.Name = "btnSelectPSODir"
        Me.btnSelectPSODir.SubItemsExpandWidth = 24
        resources.ApplyResources(Me.btnSelectPSODir, "btnSelectPSODir")
        '
        'ButtonInstall
        '
        Me.ButtonInstall.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText
        Me.ButtonInstall.Name = "ButtonInstall"
        Me.ButtonInstall.SubItems.AddRange(New DevComponents.DotNetBar.BaseItem() {Me.btnENPatch, Me.btnLargeFilesTRANSAM, Me.btnStoryPatchNew, Me.btnInstallGermanPatch, Me.btnRussianPatch, Me.btnRussianBigFiles, Me.btnInstallSpanishPatch})
        Me.ButtonInstall.SubItemsExpandWidth = 24
        resources.ApplyResources(Me.ButtonInstall, "ButtonInstall")
        '
        'btnENPatch
        '
        Me.btnENPatch.Name = "btnENPatch"
        resources.ApplyResources(Me.btnENPatch, "btnENPatch")
        '
        'btnLargeFilesTRANSAM
        '
        Me.btnLargeFilesTRANSAM.Name = "btnLargeFilesTRANSAM"
        resources.ApplyResources(Me.btnLargeFilesTRANSAM, "btnLargeFilesTRANSAM")
        '
        'btnStoryPatchNew
        '
        Me.btnStoryPatchNew.Name = "btnStoryPatchNew"
        resources.ApplyResources(Me.btnStoryPatchNew, "btnStoryPatchNew")
        '
        'btnInstallGermanPatch
        '
        Me.btnInstallGermanPatch.Name = "btnInstallGermanPatch"
        resources.ApplyResources(Me.btnInstallGermanPatch, "btnInstallGermanPatch")
        '
        'btnRussianPatch
        '
        Me.btnRussianPatch.Name = "btnRussianPatch"
        resources.ApplyResources(Me.btnRussianPatch, "btnRussianPatch")
        '
        'btnRussianBigFiles
        '
        Me.btnRussianBigFiles.Name = "btnRussianBigFiles"
        resources.ApplyResources(Me.btnRussianBigFiles, "btnRussianBigFiles")
        '
        'btnInstallSpanishPatch
        '
        Me.btnInstallSpanishPatch.Name = "btnInstallSpanishPatch"
        resources.ApplyResources(Me.btnInstallSpanishPatch, "btnInstallSpanishPatch")
        '
        'btnRestoreBackups
        '
        Me.btnRestoreBackups.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText
        Me.btnRestoreBackups.Name = "btnRestoreBackups"
        Me.btnRestoreBackups.SubItems.AddRange(New DevComponents.DotNetBar.BaseItem() {Me.btnRestoreENBackup, Me.btnRestoreLargeFilesBackup, Me.btnRestoreStoryBackup, Me.btnRestoreJPNames, Me.btnRestoreJPETrials})
        Me.btnRestoreBackups.SubItemsExpandWidth = 24
        resources.ApplyResources(Me.btnRestoreBackups, "btnRestoreBackups")
        '
        'btnRestoreENBackup
        '
        Me.btnRestoreENBackup.Name = "btnRestoreENBackup"
        resources.ApplyResources(Me.btnRestoreENBackup, "btnRestoreENBackup")
        '
        'btnRestoreLargeFilesBackup
        '
        Me.btnRestoreLargeFilesBackup.Name = "btnRestoreLargeFilesBackup"
        resources.ApplyResources(Me.btnRestoreLargeFilesBackup, "btnRestoreLargeFilesBackup")
        '
        'btnRestoreStoryBackup
        '
        Me.btnRestoreStoryBackup.Name = "btnRestoreStoryBackup"
        resources.ApplyResources(Me.btnRestoreStoryBackup, "btnRestoreStoryBackup")
        '
        'btnRestoreJPNames
        '
        Me.btnRestoreJPNames.Name = "btnRestoreJPNames"
        resources.ApplyResources(Me.btnRestoreJPNames, "btnRestoreJPNames")
        Me.btnRestoreJPNames.Visible = False
        '
        'btnRestoreJPETrials
        '
        Me.btnRestoreJPETrials.Name = "btnRestoreJPETrials"
        resources.ApplyResources(Me.btnRestoreJPETrials, "btnRestoreJPETrials")
        Me.btnRestoreJPETrials.Visible = False
        '
        'ButtonItem1
        '
        Me.ButtonItem1.Name = "ButtonItem1"
        Me.ButtonItem1.SubItems.AddRange(New DevComponents.DotNetBar.BaseItem() {Me.btnUninstallENPatch, Me.btnUninstallLargeFiles, Me.btnUninstallStory, Me.btnJPETrials, Me.btnJPEnemyNames})
        resources.ApplyResources(Me.ButtonItem1, "ButtonItem1")
        '
        'btnUninstallENPatch
        '
        Me.btnUninstallENPatch.Name = "btnUninstallENPatch"
        resources.ApplyResources(Me.btnUninstallENPatch, "btnUninstallENPatch")
        '
        'btnUninstallLargeFiles
        '
        Me.btnUninstallLargeFiles.Name = "btnUninstallLargeFiles"
        resources.ApplyResources(Me.btnUninstallLargeFiles, "btnUninstallLargeFiles")
        '
        'btnUninstallStory
        '
        Me.btnUninstallStory.Name = "btnUninstallStory"
        resources.ApplyResources(Me.btnUninstallStory, "btnUninstallStory")
        '
        'btnJPETrials
        '
        Me.btnJPETrials.Name = "btnJPETrials"
        resources.ApplyResources(Me.btnJPETrials, "btnJPETrials")
        '
        'btnJPEnemyNames
        '
        Me.btnJPEnemyNames.Name = "btnJPEnemyNames"
        resources.ApplyResources(Me.btnJPEnemyNames, "btnJPEnemyNames")
        '
        'ButtonItem2
        '
        Me.ButtonItem2.Name = "ButtonItem2"
        Me.ButtonItem2.SubItems.AddRange(New DevComponents.DotNetBar.BaseItem() {Me.btnAnalyze, Me.Button2, Me.ButtonItem10, Me.ButtonItem4, Me.BtnUpdatePso2, Me.btnGameguard, Me.btnFixPSO2EXEs, Me.btnFixPermissions, Me.btnCopyInfo, Me.ButtonItem17, Me.btnResetTweaker, Me.btnResumePatching, Me.btnTerminate})
        resources.ApplyResources(Me.ButtonItem2, "ButtonItem2")
        '
        'btnAnalyze
        '
        Me.btnAnalyze.Name = "btnAnalyze"
        resources.ApplyResources(Me.btnAnalyze, "btnAnalyze")
        '
        'Button2
        '
        Me.Button2.Name = "Button2"
        resources.ApplyResources(Me.Button2, "Button2")
        '
        'ButtonItem10
        '
        Me.ButtonItem10.Name = "ButtonItem10"
        resources.ApplyResources(Me.ButtonItem10, "ButtonItem10")
        '
        'ButtonItem4
        '
        Me.ButtonItem4.Name = "ButtonItem4"
        resources.ApplyResources(Me.ButtonItem4, "ButtonItem4")
        '
        'BtnUpdatePso2
        '
        Me.BtnUpdatePso2.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText
        Me.BtnUpdatePso2.Name = "BtnUpdatePso2"
        Me.BtnUpdatePso2.SubItemsExpandWidth = 24
        resources.ApplyResources(Me.BtnUpdatePso2, "BtnUpdatePso2")
        '
        'btnGameguard
        '
        Me.btnGameguard.Name = "btnGameguard"
        resources.ApplyResources(Me.btnGameguard, "btnGameguard")
        '
        'btnFixPSO2EXEs
        '
        Me.btnFixPSO2EXEs.Name = "btnFixPSO2EXEs"
        resources.ApplyResources(Me.btnFixPSO2EXEs, "btnFixPSO2EXEs")
        '
        'btnFixPermissions
        '
        Me.btnFixPermissions.Name = "btnFixPermissions"
        resources.ApplyResources(Me.btnFixPermissions, "btnFixPermissions")
        '
        'btnCopyInfo
        '
        Me.btnCopyInfo.Name = "btnCopyInfo"
        resources.ApplyResources(Me.btnCopyInfo, "btnCopyInfo")
        '
        'ButtonItem17
        '
        Me.ButtonItem17.Name = "ButtonItem17"
        resources.ApplyResources(Me.ButtonItem17, "ButtonItem17")
        '
        'btnResetTweaker
        '
        Me.btnResetTweaker.Name = "btnResetTweaker"
        resources.ApplyResources(Me.btnResetTweaker, "btnResetTweaker")
        '
        'btnResumePatching
        '
        Me.btnResumePatching.Name = "btnResumePatching"
        resources.ApplyResources(Me.btnResumePatching, "btnResumePatching")
        Me.btnResumePatching.Visible = False
        '
        'btnTerminate
        '
        Me.btnTerminate.Name = "btnTerminate"
        resources.ApplyResources(Me.btnTerminate, "btnTerminate")
        '
        'btnOtherStuff
        '
        Me.btnOtherStuff.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText
        Me.btnOtherStuff.Name = "btnOtherStuff"
        Me.btnOtherStuff.SubItems.AddRange(New DevComponents.DotNetBar.BaseItem() {Me.ButtonItem7, Me.btnPredownloadLobbyVideos, Me.btnChooseProxyServer, Me.btnRevertPSO2ProxyToJP, Me.btnClearSACache, Me.btnInstallPSO2})
        Me.btnOtherStuff.SubItemsExpandWidth = 24
        resources.ApplyResources(Me.btnOtherStuff, "btnOtherStuff")
        '
        'ButtonItem7
        '
        Me.ButtonItem7.Name = "ButtonItem7"
        resources.ApplyResources(Me.ButtonItem7, "ButtonItem7")
        '
        'btnPredownloadLobbyVideos
        '
        Me.btnPredownloadLobbyVideos.Name = "btnPredownloadLobbyVideos"
        resources.ApplyResources(Me.btnPredownloadLobbyVideos, "btnPredownloadLobbyVideos")
        Me.btnPredownloadLobbyVideos.Visible = False
        '
        'btnChooseProxyServer
        '
        Me.btnChooseProxyServer.Name = "btnChooseProxyServer"
        resources.ApplyResources(Me.btnChooseProxyServer, "btnChooseProxyServer")
        '
        'btnRevertPSO2ProxyToJP
        '
        Me.btnRevertPSO2ProxyToJP.Name = "btnRevertPSO2ProxyToJP"
        resources.ApplyResources(Me.btnRevertPSO2ProxyToJP, "btnRevertPSO2ProxyToJP")
        '
        'btnClearSACache
        '
        Me.btnClearSACache.Name = "btnClearSACache"
        resources.ApplyResources(Me.btnClearSACache, "btnClearSACache")
        '
        'btnInstallPSO2
        '
        Me.btnInstallPSO2.Name = "btnInstallPSO2"
        resources.ApplyResources(Me.btnInstallPSO2, "btnInstallPSO2")
        '
        'ButtonItem3
        '
        Me.ButtonItem3.Name = "ButtonItem3"
        Me.ButtonItem3.SubItems.AddRange(New DevComponents.DotNetBar.BaseItem() {Me.btnBumped, Me.btnCirno, Me.btnArksCash, Me.btnErrors, Me.btnOfficialPSO2JP, Me.btnRegistration, Me.btnTweaker, Me.btnRunPSO2Linux, Me.btnSymbolEditor, Me.btnEXPFULL})
        resources.ApplyResources(Me.ButtonItem3, "ButtonItem3")
        '
        'btnBumped
        '
        Me.btnBumped.Name = "btnBumped"
        resources.ApplyResources(Me.btnBumped, "btnBumped")
        '
        'btnCirno
        '
        Me.btnCirno.Name = "btnCirno"
        resources.ApplyResources(Me.btnCirno, "btnCirno")
        '
        'btnArksCash
        '
        Me.btnArksCash.Name = "btnArksCash"
        resources.ApplyResources(Me.btnArksCash, "btnArksCash")
        '
        'btnErrors
        '
        Me.btnErrors.Name = "btnErrors"
        resources.ApplyResources(Me.btnErrors, "btnErrors")
        '
        'btnOfficialPSO2JP
        '
        Me.btnOfficialPSO2JP.Name = "btnOfficialPSO2JP"
        resources.ApplyResources(Me.btnOfficialPSO2JP, "btnOfficialPSO2JP")
        '
        'btnRegistration
        '
        Me.btnRegistration.Name = "btnRegistration"
        resources.ApplyResources(Me.btnRegistration, "btnRegistration")
        '
        'btnTweaker
        '
        Me.btnTweaker.Name = "btnTweaker"
        resources.ApplyResources(Me.btnTweaker, "btnTweaker")
        '
        'btnRunPSO2Linux
        '
        Me.btnRunPSO2Linux.Name = "btnRunPSO2Linux"
        resources.ApplyResources(Me.btnRunPSO2Linux, "btnRunPSO2Linux")
        '
        'btnSymbolEditor
        '
        Me.btnSymbolEditor.Name = "btnSymbolEditor"
        resources.ApplyResources(Me.btnSymbolEditor, "btnSymbolEditor")
        '
        'btnEXPFULL
        '
        Me.btnEXPFULL.Name = "btnEXPFULL"
        resources.ApplyResources(Me.btnEXPFULL, "btnEXPFULL")
        Me.btnEXPFULL.Visible = False
        '
        'btnDonations
        '
        Me.btnDonations.Name = "btnDonations"
        Me.btnDonations.SubItems.AddRange(New DevComponents.DotNetBar.BaseItem() {Me.btnDonateToBumped, Me.btnDonateToCirno, Me.btnDonateToTweaker})
        resources.ApplyResources(Me.btnDonations, "btnDonations")
        '
        'btnDonateToBumped
        '
        Me.btnDonateToBumped.Name = "btnDonateToBumped"
        resources.ApplyResources(Me.btnDonateToBumped, "btnDonateToBumped")
        '
        'btnDonateToCirno
        '
        Me.btnDonateToCirno.Name = "btnDonateToCirno"
        resources.ApplyResources(Me.btnDonateToCirno, "btnDonateToCirno")
        '
        'btnDonateToTweaker
        '
        Me.btnDonateToTweaker.Name = "btnDonateToTweaker"
        resources.ApplyResources(Me.btnDonateToTweaker, "btnDonateToTweaker")
        '
        'btnQUANTUMSYSTEM
        '
        Me.btnQUANTUMSYSTEM.BeginGroup = True
        Me.btnQUANTUMSYSTEM.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText
        Me.btnQUANTUMSYSTEM.Name = "btnQUANTUMSYSTEM"
        Me.btnQUANTUMSYSTEM.SubItemsExpandWidth = 24
        resources.ApplyResources(Me.btnQUANTUMSYSTEM, "btnQUANTUMSYSTEM")
        '
        'btnDownloadPrepatch
        '
        Me.btnDownloadPrepatch.Name = "btnDownloadPrepatch"
        resources.ApplyResources(Me.btnDownloadPrepatch, "btnDownloadPrepatch")
        '
        'btnCheckForStoryUpdates
        '
        Me.btnCheckForStoryUpdates.Name = "btnCheckForStoryUpdates"
        resources.ApplyResources(Me.btnCheckForStoryUpdates, "btnCheckForStoryUpdates")
        Me.btnCheckForStoryUpdates.Visible = False
        '
        'chkAlwaysOnTop
        '
        Me.chkAlwaysOnTop.Name = "chkAlwaysOnTop"
        resources.ApplyResources(Me.chkAlwaysOnTop, "chkAlwaysOnTop")
        '
        'chkItemTranslation
        '
        Me.chkItemTranslation.Name = "chkItemTranslation"
        resources.ApplyResources(Me.chkItemTranslation, "chkItemTranslation")
        '
        'btnConfigureItemTranslation
        '
        Me.btnConfigureItemTranslation.Name = "btnConfigureItemTranslation"
        resources.ApplyResources(Me.btnConfigureItemTranslation, "btnConfigureItemTranslation")
        Me.btnConfigureItemTranslation.Visible = False
        '
        'ItemContainer4
        '
        '
        '
        '
        Me.ItemContainer4.BackgroundStyle.Class = "RibbonFileMenuBottomContainer"
        Me.ItemContainer4.HorizontalItemAlignment = DevComponents.DotNetBar.eHorizontalItemsAlignment.Right
        Me.ItemContainer4.Name = "ItemContainer4"
        Me.ItemContainer4.SubItems.AddRange(New DevComponents.DotNetBar.BaseItem() {Me.btnPlugins, Me.btnPSO2Options, Me.btnOptions, Me.btnExit})
        '
        'btnPlugins
        '
        Me.btnPlugins.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText
        Me.btnPlugins.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground
        Me.btnPlugins.Name = "btnPlugins"
        Me.btnPlugins.SubItemsExpandWidth = 24
        resources.ApplyResources(Me.btnPlugins, "btnPlugins")
        '
        'btnPSO2Options
        '
        Me.btnPSO2Options.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText
        Me.btnPSO2Options.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground
        Me.btnPSO2Options.Name = "btnPSO2Options"
        Me.btnPSO2Options.SubItemsExpandWidth = 24
        resources.ApplyResources(Me.btnPSO2Options, "btnPSO2Options")
        '
        'btnOptions
        '
        Me.btnOptions.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText
        Me.btnOptions.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground
        Me.btnOptions.Name = "btnOptions"
        Me.btnOptions.SubItemsExpandWidth = 24
        resources.ApplyResources(Me.btnOptions, "btnOptions")
        '
        'btnExit
        '
        Me.btnExit.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText
        Me.btnExit.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground
        Me.btnExit.Name = "btnExit"
        Me.btnExit.SubItemsExpandWidth = 24
        resources.ApplyResources(Me.btnExit, "btnExit")
        '
        'btnAnnouncements
        '
        Me.btnAnnouncements.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnAnnouncements.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground
        resources.ApplyResources(Me.btnAnnouncements, "btnAnnouncements")
        Me.btnAnnouncements.Name = "btnAnnouncements"
        Me.btnAnnouncements.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        '
        'WebBrowser4
        '
        resources.ApplyResources(Me.WebBrowser4, "WebBrowser4")
        Me.WebBrowser4.Name = "WebBrowser4"
        Me.WebBrowser4.Url = New System.Uri("", System.UriKind.Relative)
        '
        'txtPSO2DefaultINI
        '
        resources.ApplyResources(Me.txtPSO2DefaultINI, "txtPSO2DefaultINI")
        Me.txtPSO2DefaultINI.Name = "txtPSO2DefaultINI"
        Me.txtPSO2DefaultINI.ReadOnly = True
        '
        'tmrWaitingforPSO2
        '
        Me.tmrWaitingforPSO2.Interval = 180000
        Me.tmrWaitingforPSO2.Tag = ""
        '
        'txtFiles
        '
        resources.ApplyResources(Me.txtFiles, "txtFiles")
        Me.txtFiles.Name = "txtFiles"
        '
        'lblStatus
        '
        Me.lblStatus.BackColor = System.Drawing.Color.Transparent
        resources.ApplyResources(Me.lblStatus, "lblStatus")
        Me.lblStatus.Name = "lblStatus"
        '
        'WebBrowser1
        '
        resources.ApplyResources(Me.WebBrowser1, "WebBrowser1")
        Me.WebBrowser1.Name = "WebBrowser1"
        Me.WebBrowser1.Url = New System.Uri("", System.UriKind.Relative)
        '
        'DLS
        '
        Me.DLS.BaseAddress = ""
        Me.DLS.CachePolicy = Nothing
        Me.DLS.Credentials = Nothing
        Me.DLS.Encoding = CType(resources.GetObject("DLS.Encoding"), System.Text.Encoding)
        Me.DLS.Headers = CType(resources.GetObject("DLS.Headers"), System.Net.WebHeaderCollection)
        Me.DLS.QueryString = CType(resources.GetObject("DLS.QueryString"), System.Collections.Specialized.NameValueCollection)
        Me.DLS.Timeout = 10000
        Me.DLS.UseDefaultCredentials = False
        '
        'FrmMain
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.GradientActiveCaption
        Me.Controls.Add(Me.WebBrowser1)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.txtFiles)
        Me.Controls.Add(Me.lblDirectoryLabel)
        Me.Controls.Add(Me.btnApplyChanges)
        Me.Controls.Add(Me.txtPSO2DefaultINI)
        Me.Controls.Add(Me.btnAnnouncements)
        Me.Controls.Add(Me.btnLaunchPSO2)
        Me.Controls.Add(Me.lblDirectory)
        Me.Controls.Add(Me.chkRestoreSEGA)
        Me.Controls.Add(Me.chkRestoreNVidia)
        Me.Controls.Add(Me.chkRestoreVita)
        Me.Controls.Add(Me.chkRestorePC)
        Me.Controls.Add(Me.chkRemoveSEGA)
        Me.Controls.Add(Me.chkRemoveNVidia)
        Me.Controls.Add(Me.PBMainBar)
        Me.Controls.Add(Me.chkRemoveVita)
        Me.Controls.Add(Me.chkRemovePC)
        Me.Controls.Add(Me.rtbDebug)
        Me.Controls.Add(Me.txtHTML)
        Me.Controls.Add(Me.chkSwapOP)
        Me.Controls.Add(Me.RibbonControl1)
        Me.Controls.Add(Me.WebBrowser4)
        Me.DoubleBuffered = True
        Me.EnableGlass = False
        Me.FlattenMDIBorder = False
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.Name = "FrmMain"
        Me.cmsTextBarOptions.ResumeLayout(False)
        Me.cmsProgressBar.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtHTML As System.Windows.Forms.TextBox
    Public WithEvents rtbDebug As System.Windows.Forms.RichTextBox
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents chkRemovePC As DevComponents.DotNetBar.Controls.CheckBoxX
    Friend WithEvents chkRemoveVita As DevComponents.DotNetBar.Controls.CheckBoxX
    Friend WithEvents PBMainBar As DevComponents.DotNetBar.Controls.ProgressBarX
    Friend WithEvents chkRemoveNVidia As DevComponents.DotNetBar.Controls.CheckBoxX
    Friend WithEvents chkRemoveSEGA As DevComponents.DotNetBar.Controls.CheckBoxX
    Friend WithEvents chkSwapOP As DevComponents.DotNetBar.Controls.CheckBoxX
    Friend WithEvents lblDirectoryLabel As DevComponents.DotNetBar.LabelX
    Friend WithEvents lblDirectory As DevComponents.DotNetBar.LabelX
    Friend WithEvents chkRestorePC As DevComponents.DotNetBar.Controls.CheckBoxX
    Friend WithEvents chkRestoreVita As DevComponents.DotNetBar.Controls.CheckBoxX
    Friend WithEvents chkRestoreNVidia As DevComponents.DotNetBar.Controls.CheckBoxX
    Friend WithEvents chkRestoreSEGA As DevComponents.DotNetBar.Controls.CheckBoxX
    Friend WithEvents btnApplyChanges As DevComponents.DotNetBar.ButtonX
    Friend WithEvents btnLaunchPSO2 As DevComponents.DotNetBar.ButtonX
    Friend WithEvents cmsProgressBar As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents CancelDownloadToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CancelProcessToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents RibbonControl1 As DevComponents.DotNetBar.RibbonControl
    Friend WithEvents Office2007StartButton1 As DevComponents.DotNetBar.Office2007StartButton
    Friend WithEvents ItemContainer1 As DevComponents.DotNetBar.ItemContainer
    Friend WithEvents ItemContainer2 As DevComponents.DotNetBar.ItemContainer
    Friend WithEvents ItemContainer4 As DevComponents.DotNetBar.ItemContainer
    Friend WithEvents btnOptions As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnExit As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnPSO2Options As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnAnnouncements As DevComponents.DotNetBar.ButtonX
    Friend WithEvents WebBrowser4 As System.Windows.Forms.WebBrowser
    Friend WithEvents txtPSO2DefaultINI As System.Windows.Forms.TextBox
    Friend WithEvents tsmRestartDownload As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ItemContainer3 As DevComponents.DotNetBar.ItemContainer
    Friend WithEvents btnSelectPSODir As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents ButtonInstall As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnENPatch As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnRussianPatch As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnInstallSpanishPatch As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnRestoreBackups As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnRestoreENBackup As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnRestoreLargeFilesBackup As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnRestoreStoryBackup As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnRestoreJPNames As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnRestoreJPETrials As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents ButtonItem1 As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnUninstallENPatch As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnUninstallLargeFiles As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnUninstallStory As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents ButtonItem2 As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnAnalyze As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents Button2 As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents ButtonItem10 As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnGameguard As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnFixPSO2EXEs As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnFixPermissions As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnCopyInfo As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents ButtonItem17 As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnResetTweaker As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnResumePatching As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnTerminate As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnOtherStuff As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents ButtonItem7 As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnPredownloadLobbyVideos As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnChooseProxyServer As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnRevertPSO2ProxyToJP As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnClearSACache As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnInstallPSO2 As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents ButtonItem3 As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnBumped As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnCirno As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnArksCash As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnErrors As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnOfficialPSO2JP As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnRegistration As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnTweaker As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnRunPSO2Linux As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnSymbolEditor As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnEXPFULL As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnDonations As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnDonateToBumped As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnDonateToCirno As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnDonateToTweaker As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnDownloadPrepatch As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnCheckForStoryUpdates As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents chkAlwaysOnTop As DevComponents.DotNetBar.CheckBoxItem
    Friend WithEvents chkItemTranslation As DevComponents.DotNetBar.CheckBoxItem
    Friend WithEvents btnConfigureItemTranslation As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnStoryPatchNew As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents DLS As MyWebClient
    Friend WithEvents tmrWaitingforPSO2 As System.Windows.Forms.Timer
    Friend WithEvents txtFiles As System.Windows.Forms.TextBox
    Friend WithEvents btnLargeFilesTRANSAM As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents cmsTextBarOptions As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents CopyAllTextToClipboardToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents WebBrowser1 As System.Windows.Forms.WebBrowser
    Friend WithEvents btnLaunchPSO2fromORB As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnPlugins As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnRussianBigFiles As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnInstallGermanPatch As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnJPEnemyNames As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnJPETrials As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents btnQUANTUMSYSTEM As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents BtnUpdatePso2 As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents ButtonItem4 As DevComponents.DotNetBar.ButtonItem
End Class
