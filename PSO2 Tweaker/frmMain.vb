'------------------------------------------------------------------------------
' PSO2 Tweaker - A replacement for the normal PSO2JP Launcher, used to update
' and launch the original Japanese version of the game with patches,
' modifications, troubleshooting, and much more.
'
' Thanks for taking a look at this code.
' Feel free to submit bugfixes/improvements to 
' https://github.com/Arks-Layer/PSO2Tweaker
' 
' Take care, and have fun in everything you do. - AIDA
' Program uses the GNU GENERAL PUBLIC LICENSE
' Requirements: 
'
'------------------------------------------------------------------------------
Imports DevComponents.DotNetBar
Imports Microsoft.VisualBasic.FileIO
Imports Microsoft.Win32
Imports System.ComponentModel
Imports System.IO
Imports System.Net
Imports System.Runtime.Serialization.Json
Imports System.Security
Imports System.Text.RegularExpressions
Imports System.Threading
Imports System.Xml
Imports PSO2_Tweaker.My
Imports PSO2_Tweaker.VEDA
Imports System.Text
Imports ArksLayer.Tweaker.Abstractions
Imports ArksLayer.Tweaker.UpdateEngine
Imports System.Security.Permissions
Imports System.Threading.Tasks


' TODO: Replace all redundant code with functions
' TODO: Every instance of file downloading that retries ~5 times should be a function. I didn't realize there were so many.

Public Class FrmMain

    Const EnglishPatch = "English Patch"
    Const RussianPatch = "Russian Patch"
    Const RussianBigPatch = "Russian Large Files Patch"
    Const SpanishPatch = "Spanish Patch"
    Const GermanPatch = "German Patch"
    Const StoryPatch = "Story Patch"
    Const LargeFiles = "Large Files"

    ReadOnly _pso2OptionsFrm As FrmPso2Options
    ReadOnly _optionsFrm As FrmOptions
    ReadOnly _myDocuments As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)

    Dim _cancelled As Boolean
    Dim _cancelledFull As Boolean
    Dim _dpiSetting As Single
    Dim _itemDownloadingDone As Boolean
    Dim _mileyCyrus As Integer
    Dim _restartplz As Boolean
    Dim _systemUnlock As Integer
    Dim _vedaUnlocked As Boolean = False
    Public SkipDialogs As Boolean = False
    Dim _totalsize2 As Long
    Dim NewInstall As Boolean = False

    Sub New()
        Try
            _pso2OptionsFrm = New FrmPso2Options()
            _optionsFrm = New FrmOptions()

            InitializeComponent()

            Program.MainForm = Me
            'Yo, fuck this shit. Shit is mad whack, yo.
            SuspendLayout()
            Text = ("PSO2 Tweaker ver " & Application.Info.Version.ToString())
            chkRemovePC.Text = Resources.strRemovePCOpening
            chkRemoveVita.Text = Resources.strRemoveVitaOpening
            chkRemoveNVidia.Text = Resources.strRemoveNVidiaVideo
            chkRemoveSEGA.Text = Resources.strRemoveSEGALogoVideo
            chkSwapOP.Text = Resources.strSwapPCVitaOpenings
            chkRestorePC.Text = Resources.strRestorePCOpeningVideo
            chkRestoreVita.Text = Resources.strRestoreVitaOpeningVideo
            chkRestoreNVidia.Text = Resources.strRestoreNVidiaLogo
            chkRestoreSEGA.Text = Resources.strRestoreSEGALogoVideo
            lblDirectoryLabel.Text = Resources.strCurrentlyselecteddirectory
            lblStatus.Text = Resources.strWaitingforacommand
            btnSelectPSODir.Text = Resources.strSelectPSO2win32folder
            ButtonInstall.Text = Resources.strInstallUpdatePatches
            btnRestoreBackups.Text = "Restore Backup of JP Files"
            btnApplyChanges.Text = Resources.strApplySelectedChanges
            CancelDownloadToolStripMenuItem.Text = Resources.strCancelCurrentDownload
            BtnUpdatePso2.Text = Resources.strCheckforPSO2Updates
            btnLaunchPSO2.Text = Resources.strLaunchPSO2
            btnFixPSO2EXEs.Text = Resources.strFixPSO2EXEs
            btnFixPermissions.Text = Resources.strFixPSO2Permissions
            rtbDebug.Text = Resources.strProgramStarted
            ButtonItem1.Text = "Redownload Original JP Files"
            ButtonItem2.Text = Resources.strTroubleshooting
            btnOtherStuff.Text = Resources.strOtherTasks
            ButtonItem3.Text = Resources.strWebLinks
            btnCheckForStoryUpdates.Text = Resources.strCheckForStoryPatchUpdates
            chkAlwaysOnTop.Text = Resources.strAlwaysonTop
            chkItemTranslation.Text = Resources.strTranslateItems
            btnPSO2Options.Text = Resources.strPSO2Options
            btnOptions.Text = Resources.strOptions
            btnExit.Text = Resources.strExit
            btnAnalyze.Text = Resources.strAnalyzeInstallforIssues
            Button2.Text = Resources.strCheckforDeletedMissingFiles
            ButtonItem10.Text = Resources.strCheckForOldMissingFiles
            btnGameguard.Text = Resources.strFixGameguardErrors
            ButtonItem17.Text = Resources.strResetPSO2Settings
            btnResumePatching.Text = Resources.strResumePatching
            btnTerminate.Text = Resources.strTerminate
            ButtonItem7.Text = Resources.strLaunchChrome

            Helper.Log("Loading color stuff...")

            Select Case RegKey.GetValue(Of String)(RegKey.Style)
                Case "Blue"
                    StyleManager.Style = eStyle.Office2007Blue

                Case "Silver"
                    StyleManager.Style = eStyle.Office2007Silver

                Case "Black"
                    StyleManager.Style = eStyle.Office2007Black

                Case "Vista Glass"
                    StyleManager.Style = eStyle.Office2007VistaGlass

                Case "2010 Silver"
                    StyleManager.Style = eStyle.Office2010Silver

                Case "Windows 7 Blue"
                    StyleManager.Style = eStyle.Windows7Blue

                Case Else
                    StyleManager.Style = eStyle.Office2007Black
            End Select



            Dim regValue As Integer

            regValue = RegKey.GetValue(Of Integer)(RegKey.TextBoxBgColor)
            If regValue = 0 Then RegKey.SetValue(Of UInteger)(RegKey.TextBoxBgColor, 4294967295)
            If regValue <> 0 Then rtbDebug.BackColor = Color.FromArgb(Convert.ToInt32(regValue))

            regValue = RegKey.GetValue(Of Integer)(RegKey.TextBoxColor)
            If regValue <> 0 Then rtbDebug.ForeColor = Color.FromArgb(Convert.ToInt32(regValue))

            If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.UseOldProgressBar.ToString)) Then RegKey.SetValue(Of Boolean)(RegKey.UseOldProgressBar, False)

            If RegKey.GetValue(Of Boolean)(RegKey.UseOldProgressBar) = True Then
                PBMainBar.Style = eDotNetBarStyle.Office2007
                PBMainBar.ForeColor = Color.Black
            Else
                PBMainBar.Style = eDotNetBarStyle.Office2000
                regValue = RegKey.GetValue(Of Integer)(RegKey.PBTextColor)
                If regValue <> 0 Then PBMainBar.BackgroundStyle.TextColor = Color.FromArgb(Convert.ToInt32(regValue))

                regValue = RegKey.GetValue(Of Integer)(RegKey.PBFill1)
                If regValue <> 0 Then PBMainBar.ChunkColor = Color.FromArgb(Convert.ToInt32(regValue))

                regValue = RegKey.GetValue(Of Integer)(RegKey.PBFill2)
                If regValue <> 0 Then PBMainBar.ChunkColor2 = Color.FromArgb(Convert.ToInt32(regValue))
            End If
            regValue = RegKey.GetValue(Of Integer)(RegKey.Color)
            If regValue <> 0 Then StyleManager.ColorTint = Color.FromArgb(Convert.ToInt32(regValue))

            regValue = RegKey.GetValue(Of Integer)(RegKey.FontColor)
            If regValue <> 0 Then
                Dim color As Color = Color.FromArgb(Convert.ToInt32(regValue))
                _pso2OptionsFrm.TabControl1.ColorScheme.TabItemSelectedText = color
                _pso2OptionsFrm.TabItem1.TextColor = color
                _pso2OptionsFrm.TabItem2.TextColor = color
                _pso2OptionsFrm.TabItem3.TextColor = color
                _pso2OptionsFrm.TabItem4.TextColor = color
                _pso2OptionsFrm.TabItem7.TextColor = color
                _pso2OptionsFrm.Slider1.TextColor = color
                _pso2OptionsFrm.SBGM.TextColor = color
                _pso2OptionsFrm.CheckBoxX1.TextColor = color
                ForeColor = color
                _optionsFrm.ForeColor = color
                _pso2OptionsFrm.ForeColor = color
                _optionsFrm.CheckBoxX3.TextColor = color
                _optionsFrm.CheckBoxX4.TextColor = color
                _optionsFrm.CheckBoxX1.TextColor = color
                _optionsFrm.chkAutoRemoveCensor.TextColor = color
                chkRemoveNVidia.TextColor = color
                chkRemovePC.TextColor = color
                chkRemoveSEGA.TextColor = color
                chkRemoveVita.TextColor = color
                lblStatus.ForeColor = color
                chkRestoreNVidia.TextColor = color
                chkRestorePC.TextColor = color
                chkRestoreSEGA.TextColor = color
                chkRestoreVita.TextColor = color
                chkSwapOP.TextColor = color
            End If
        Catch ex As Exception
            Helper.LogWithException(Resources.strERROR, ex)
        Finally
            ResumeLayout(False)
        End Try
    End Sub

    Private Shared Sub frmMain_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
        Windows.Forms.Application.Exit()
    End Sub
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub Form1_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint
        If Me.BackgroundImage IsNot Nothing Then
            Dim FormWidth As Integer
            If _dpiSetting = 96 Then FormWidth = 420
            If _dpiSetting = 120 Then FormWidth = 560
            e.Graphics.Clear(Me.BackColor)
            e.Graphics.DrawImage(Me.BackgroundImage, 0, 44, FormWidth, Me.Height - 44)
        End If
    End Sub
    Private Async Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        DLS.Headers("user-agent") = GetUserAgent()
        Using g As Graphics = CreateGraphics()
            If g.DpiX = 120 OrElse g.DpiX = 96 Then
                _dpiSetting = g.DpiX
            End If
        End Using

        If Program.CloseMe = True Then
            MsgBox("All done! Please re-run the Tweaker.")
            Close()
        End If

        Try
            If Program.SidebarEnabled Then
                OpenSideBar()
            Else
                ToggleSideBar()
            End If

            While Not Program.IsPso2Installed
                InstallPso2()
                Program.Main()
            End While

            lblDirectory.Text = Program.Pso2RootDir
            PBMainBar.Text = ""
            TopMost = Program.IsMainFormTopMost
            chkAlwaysOnTop.Checked = Program.IsMainFormTopMost
            _cancelledFull = False
            If RegKey.GetValue(Of String)(RegKey.ImageLocation) <> "" Then
                If File.Exists(RegKey.GetValue(Of String)(RegKey.ImageLocation)) Then Me.BackgroundImage = System.Drawing.Image.FromFile(RegKey.GetValue(Of String)(RegKey.ImageLocation))
            End If
            If RegKey.GetValue(Of String)(RegKey.ORBLocation) <> "" Then
                If File.Exists(RegKey.GetValue(Of String)(RegKey.ORBLocation)) Then Office2007StartButton1.Image = System.Drawing.Image.FromFile(RegKey.GetValue(Of String)(RegKey.ORBLocation))
            End If

            If RegKey.GetValue(Of String)(RegKey.ChecksVisible) <> "" Then
                FrmOptions.CheckBoxX3.Checked = Convert.ToBoolean(RegKey.GetValue(Of String)(RegKey.ChecksVisible))
                chkRemovePC.Visible = Convert.ToBoolean(RegKey.GetValue(Of String)(RegKey.ChecksVisible))
                chkRestorePC.Visible = Convert.ToBoolean(RegKey.GetValue(Of String)(RegKey.ChecksVisible))
                chkRemoveVita.Visible = Convert.ToBoolean(RegKey.GetValue(Of String)(RegKey.ChecksVisible))
                chkRestoreVita.Visible = Convert.ToBoolean(RegKey.GetValue(Of String)(RegKey.ChecksVisible))
                chkRemoveNVidia.Visible = Convert.ToBoolean(RegKey.GetValue(Of String)(RegKey.ChecksVisible))
                chkRestoreNVidia.Visible = Convert.ToBoolean(RegKey.GetValue(Of String)(RegKey.ChecksVisible))
                chkRemoveSEGA.Visible = Convert.ToBoolean(RegKey.GetValue(Of String)(RegKey.ChecksVisible))
                chkRestoreSEGA.Visible = Convert.ToBoolean(RegKey.GetValue(Of String)(RegKey.ChecksVisible))
                btnApplyChanges.Visible = Convert.ToBoolean(RegKey.GetValue(Of String)(RegKey.ChecksVisible))
            Else
                RegKey.SetValue(Of String)(RegKey.ChecksVisible, "True")
            End If


            If RegKey.GetValue(Of String)(RegKey.PSO2DirVisible) <> "" Then
                FrmOptions.CheckBoxX4.Checked = Convert.ToBoolean(RegKey.GetValue(Of String)(RegKey.PSO2DirVisible))
                lblDirectory.Visible = Convert.ToBoolean(RegKey.GetValue(Of String)(RegKey.PSO2DirVisible))
                lblDirectoryLabel.Visible = Convert.ToBoolean(RegKey.GetValue(Of String)(RegKey.PSO2DirVisible))
            Else
                RegKey.SetValue(Of String)(RegKey.PSO2DirVisible, "True")
            End If

            If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.LaunchPSO2fromORB.ToString)) Then RegKey.SetValue(Of Boolean)(RegKey.LaunchPSO2fromORB, False)
            'Re-enable this if we do more BETAs in the future
            'If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.EnableBeta.ToString)) Then RegKey.SetValue(Of Boolean)(RegKey.EnableBeta, False)
            RegKey.SetValue(Of Boolean)(RegKey.EnableBeta, False)

            If RegKey.GetValue(Of Boolean)(RegKey.LaunchPSO2fromORB) = True Then
                btnLaunchPSO2.Visible = False
                btnLaunchPSO2fromORB.Visible = True
            End If



            'Remove the next 3 lines to try sidebar theming. - AIDA
            WebBrowser1.Visible = False
            'Here is what I will call our "idiot handling" code
            '
            If Program.StartPath = Helper.GetDownloadsPath() Then
                MsgBox("Please be aware - Due to various Windows issues, this program will not work correctly while in the ""Downloads"" folder. Please move it to it's own folder, like C:\Tweaker\")
                Helper.Log("Please be aware - Due to various Windows issues, this program will not work correctly while in the ""Downloads"" folder. Please move it to it's own folder, like C:\Tweaker\")
                Environment.Exit(0)
                Stop
            End If
            If Program.StartPath = Program.Pso2RootDir.Replace("\\", "\") Then
                MsgBox("Please be aware - Due to various Windows issues, this program will not work correctly while in the pso2_bin folder. Please move it to it's own folder, like C:\Tweaker\")
                Helper.Log("Please be aware - Due to various Windows issues, this program will not work correctly while in the pso2_bin folder. Please move it to it's own folder, like C:\Tweaker\")
                Environment.Exit(0)
                Stop
            End If
            If Program.StartPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) Then
                MsgBox("Please be aware - Due to various Windows issues, this program will not work correctly while in the ""My Documents"" folder. Please move it to it's own folder, like C:\Tweaker\")
                Helper.Log("Please be aware - Due to various Windows issues, this program will not work correctly while in the ""My Documents"" folder. Please move it to it's own folder, like C:\Tweaker\")
                Environment.Exit(0)
                End
            End If
            If Program.StartPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) Then
                MsgBox("Please be aware - Due to various Windows issues, this program will not work correctly while on the Desktop. Please move it to it's own folder, like C:\Tweaker\")
                Helper.Log("Please be aware - Due to various Windows issues, this program will not work correctly while on the Desktop. Please move it to it's own folder, like C:\Tweaker\")
                Environment.Exit(0)
                End
            End If
            If Program.StartPath.Contains(Environment.GetFolderPath(Environment.SpecialFolder.Windows)) Then
                MsgBox("Please be aware - Due to various Windows issues, this program will not work correctly while in a special windows folder. Please move it to it's own folder, like C:\Tweaker\")
                Helper.Log("Please be aware - Due to various Windows issues, this program will not work correctly while in a special windows folder. Please move it to it's own folder, like C:\Tweaker\")
                Environment.Exit(0)
                End
            End If
            If Program.StartPath = Path.GetPathRoot(Environment.SystemDirectory) Then
                MsgBox("Please be aware - Due to various Windows issues, this program will not work correctly while on the root drive (" & Path.GetPathRoot(Environment.SystemDirectory) & "). Please move it to it's own folder, like C:\Tweaker\")
                Helper.Log("Please be aware - Due to various Windows issues, this program will not work correctly while on the root drive (" & Path.GetPathRoot(Environment.SystemDirectory) & "). Please move it to it's own folder, like C:\Tweaker\")
                Environment.Exit(0)
                End
            End If
            If Process.GetCurrentProcess().MainModule.ModuleName <> "PSO2 Tweaker.exe" And Process.GetCurrentProcess().MainModule.ModuleName <> "PSO2 Tweaker.vshost.exe" Then
                MsgBox("Your PSO2 Tweaker is named incorrectly. Please rename the program you just ran to ""PSO2 Tweaker.exe"" exactly, then run it again.")
                Helper.Log("Your PSO2 Tweaker is named incorrectly. Please rename the program you just ran to ""PSO2 Tweaker.exe"" exactly, then run it again.")
                Environment.Exit(0)
                End
            End If
            'string path = Path.GetPathRoot(Environment.SystemDirectory);
            Show()

            Helper.WriteDebugInfoAndOk((Resources.strProgramOpeningSuccessfully & Application.Info.Version.ToString()))

        Catch ex As Exception
            Helper.LogWithException(Resources.strERROR, ex)
        End Try

        Try
            CheckForTweakerUpdates()
        Catch ex As Exception
            Helper.LogWithException(Resources.strERROR, ex)
        End Try
        Try
            btnQUANTUMSYSTEM.Enabled = False
            ButtonItem10.Enabled = False
            Application.DoEvents()

            If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.Pso2Dir)) Then
                MsgBox(Resources.strPleaseSelectPSO2win32dir)
                Helper.SelectPso2Directory()
            Else
                Program.Pso2RootDir = RegKey.GetValue(Of String)(RegKey.Pso2Dir)
                lblDirectory.Text = Program.Pso2RootDir
            End If

            If File.Exists((Program.Pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) AndAlso File.Exists((Program.Pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585")) Then
                If Helper.GetFileSize((Program.Pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) = 167479840 AndAlso Helper.GetFileSize((Program.Pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585")) = 151540352 Then
                    chkSwapOP.Text = Resources.strSwapPCVitaOpenings & " (" & Resources.strNotSwapped & ")"
                ElseIf Helper.GetFileSize((Program.Pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) = 151540352 AndAlso Helper.GetFileSize((Program.Pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585")) = 167479840 Then
                    chkSwapOP.Text = Resources.strSwapPCVitaOpenings & " (" & Resources.strSwapped & ")"
                End If
            End If

            ' Shouldn't be doing this in this way
            Application.DoEvents()
            DownloadFile(Program.FreedomUrl & "gnfieldstatus.txt", "gnfieldstatus.txt")
            DownloadFile(Program.FreedomUrl & "gnfieldMD5.txt", "gnfieldMD5.txt")
            Dim GNFieldMD5 As String = File.ReadAllLines("gnfieldMD5.txt")(0)


            If File.Exists(RegKey.GetValue(Of String)("GNFieldName")) Then Helper.DeleteFile(RegKey.GetValue(Of String)("GNFieldName"))

            If File.ReadAllLines("gnfieldstatus.txt")(0) = "Active" And Program.NoGNFieldMode = False Then
                'GN Field needs to be active
                Program.GNFieldActive = True
                If Process.GetProcessesByName("GN Field").Length > 0 Then
                    Helper.Log("GN Field detected, disabling!")
                    For Each proc As Process In Process.GetProcessesByName("GN Field")
                        proc.Kill()
                    Next
                End If
                If Not File.Exists("GN Field.exe") Then
                    Helper.WriteDebugInfo(Resources.strDownloading & "GN Field...")
                    Application.DoEvents()
                    DownloadFile(Program.FreedomUrl & "GN Field.exe", "GN Field.exe")
                End If

                For index = 1 To 5
                    If Helper.GetMd5("GN Field.exe") <> GNFieldMD5 Then
                        Helper.WriteDebugInfo("Your GN Field appears to be corrupt or outdated, redownloading...")
                        Helper.Log("MD5 of current GN Field is " & Helper.GetMd5("GN Field.exe") & ", should have been " & GNFieldMD5 & ".")
                        Application.DoEvents()
                        DownloadFile(Program.FreedomUrl & "GN Field.exe", "GN Field.exe")
                    Else
                        Exit For
                    End If
                Next
            End If

            If File.ReadAllLines("gnfieldstatus.txt")(0) = "Random" And Program.NoGNFieldMode = False Then
                'GG trying to disable our GN Field. Time to boost it with ELS!
                Program.GNFieldActive = True
                Program.ELSActive = True
                Dim GNFieldName As String = GenerateELSName() & ".exe"
                RegKey.SetValue(Of String)("GNFieldName", GNFieldName)
                If File.Exists("GN Field.exe") Then
                    Helper.WriteDebugInfo("Removing old GN Field and updating...")
                    Helper.DeleteFile("GN Field.exe")
                    Application.DoEvents()
                End If
                DownloadFile(Program.FreedomUrl & "GN Field.exe", GNFieldName)
                Helper.WriteDebugInfo("GN Field downloaded and renamed to " & GNFieldName & " to hide it from GG.")
                For index = 1 To 5
                    If Helper.GetMd5(GNFieldName) <> GNFieldMD5 Then
                        Helper.WriteDebugInfo("Your GN Field appears to be corrupt or outdated, redownloading...")
                        Application.DoEvents()
                        DownloadFile(Program.FreedomUrl & "GN Field.exe", GNFieldName)
                    Else
                        Exit For
                    End If
                Next
            End If

            If File.ReadAllLines("gnfieldstatus.txt")(0) = "Inactive" Then Program.GNFieldActive = False

            If Not File.Exists("7za.exe") Then
                Helper.WriteDebugInfo(Resources.strDownloading & "7za.exe...")
                Application.DoEvents()
                DownloadFile(Program.FreedomUrl & "7za.exe", "7za.exe")
            End If

            For index = 1 To 5
                If Helper.GetMd5("7za.exe") <> "42BADC1D2F03A8B1E4875740D3D49336" Then
                    Helper.WriteDebugInfo(Resources.strYour7zipiscorrupt)
                    Application.DoEvents()
                    DownloadFile(Program.FreedomUrl & "7za.exe", "7za.exe")
                Else
                    Exit For
                End If
            Next

            If Not File.Exists("UnRar.exe") Then
                Helper.WriteDebugInfo(Resources.strDownloading & "UnRar.exe...")
                Application.DoEvents()
                DownloadFile(Program.FreedomUrl & "UnRAR.exe", "UnRAR.exe")
            End If

            For index = 1 To 5
                If Helper.GetMd5("UnRar.exe") <> "0C83C1293723A682577E3D0B21562B79" Then
                    Helper.WriteDebugInfo(Resources.strYourUnrariscorrupt)
                    Application.DoEvents()
                    DownloadFile(Program.FreedomUrl & "UnRAR.exe", "UnRAR.exe")
                Else
                    Exit For
                End If
            Next

            'Man, fuck GG. We have to remove the proxy stats function because things.

            If Not String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.GetProxyStats.ToString)) Then RegKey.DeleteValue("GetProxyStats")
            If Not String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.ProxyStatsURL.ToString)) Then RegKey.DeleteValue("ProxyStatsURL")
            If Not String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.StatsLastChecked.ToString)) Then RegKey.DeleteValue("StatsLastChecked")
            If Not String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.CachedStats.ToString)) Then RegKey.DeleteValue("CachedStats")

            Helper.CheckIfOfficialLauncherRunning()

            Helper.DeleteDirectory("TEMPSTORYAIDAFOOL")
            Helper.DeleteFile("launcherlist.txt")
            Helper.DeleteFile("patchlist.txt")
            Helper.DeleteFile("patchlist_old.txt")

            'Added in precede files. Stupid ass SEGA.
            Helper.DeleteFile("patchlist0.txt")
            Helper.DeleteFile("patchlist1.txt")
            Helper.DeleteFile("patchlist2.txt")
            Helper.DeleteFile("patchlist3.txt")
            Helper.DeleteFile("patchlist4.txt")
            Helper.DeleteFile("patchlist5.txt")
            Helper.DeleteFile("precede.txt")
            Helper.DeleteFile("ServerConfig.txt")
            Helper.DeleteFile("precede_apply.txt")
            Helper.DeleteFile("version.ver")
            Helper.DeleteFile("Story MD5HashList.txt")
            Helper.DeleteFile("PluginMD5HashList.txt")
            Helper.DeleteFile("working.txt")
            Helper.DeleteFile("gnfieldstatus.txt")
            Helper.DeleteFile("gnfieldMD5.txt")

            If File.Exists((Program.Pso2WinDir & "\ffbff2ac5b7a7948961212cefd4d402c")) Then
                Computer.FileSystem.DeleteFile(Program.Pso2WinDir & "\ffbff2ac5b7a7948961212cefd4d402c", UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently)
                Helper.WriteDebugInfoAndOk(Resources.strRemoving & "Censor...")
            End If

            'btnLaunchPSO2.Enabled = False

            If File.Exists("missing.json") Then
                Dim yesNoResume As MsgBoxResult = MsgBox("It seems that the last patching attempt was interrupted. Would you Like to resume patching?", vbYesNo)
                If yesNoResume = MsgBoxResult.Yes Then
                    LockGui()
                    ' Use IOC Container in the main Tweaker project to deal with dependencies.
                    Dim output As New TweakerTrigger
                    Dim Settings = New RegistryTweakerSettings("Software\AIDA")
                    Dim updater = New UpdateManager(Settings, output)

                    'await updater.CleanLegacyFiles();

                    'Console.WriteLine(settings.GameDirectory)
                    Try

                        'frmDownloader.TopMost = True
                        'Me.TopMost = False
                    Catch ex As Exception
                        Helper.LogWithException(Resources.strERROR, ex)
                    End Try
                    frmDownloader.CleanupUI()
                    Await updater.Update(False, False)
                Else
                    Helper.DeleteFile("missing.json")
                    Helper.DeleteFile("done.log")
                End If
            End If

            Helper.WriteDebugInfo(Resources.strCheckingforPSO2Updates)
            Application.DoEvents()

            Await CheckForPso2Updates(False)
            Helper.WriteDebugInfoSameLine(Resources.strDone)
            Application.DoEvents()


            'Check for PSO2 Updates here, download the version.ver thingie
            'Check for PSO2 EN Patch updates here, parse the URL and see if it's different from the saved one
            'Check for EN Story Patch
            Helper.WriteDebugInfo(Resources.strCheckingforUpdatestopatches)

            'Check for English Patches (Done! :D)
            CheckForEnPatchUpdates()
            Helper.WriteDebugInfo(Resources.strCurrentENPatchis & RegKey.GetValue(Of String)(RegKey.EnPatchVersion))
            Application.DoEvents()

            'Check for LargeFiles Update (Work-In-Progress!)
            CheckForLargeFilesUpdates()
            Helper.WriteDebugInfo(Resources.strCurrentLargeFilesis & RegKey.GetValue(Of String)(RegKey.LargeFilesVersion))
            Application.DoEvents()

            'Check for Story Patches (Done! :D)
            Application.DoEvents()
            CheckForStoryUpdates()
            Helper.WriteDebugInfo(Resources.strCurrentStoryPatchis & RegKey.GetValue(Of String)(RegKey.StoryPatchVersion))
            Application.DoEvents()


            '            Helper.WriteDebugInfo(Resources.strIfAboveVersions)



            If Program.WayuIsAFailure Then
                Helper.WriteDebugInfo("Skipping downloads for Wayu!")
            Else
                If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.UseItemTranslation)) Then
                    RegKey.SetValue(Of Boolean)(RegKey.UseItemTranslation, True)
                End If

                Program.UseItemTranslation = Convert.ToBoolean(RegKey.GetValue(Of String)(RegKey.UseItemTranslation))

                If Directory.Exists(Program.Pso2RootDir & "\plugins\") = False Then
                    Helper.WriteDebugInfoAndOk("Setting up plugin system...")
                    Directory.CreateDirectory(Program.Pso2RootDir & "\plugins\")
                    Directory.CreateDirectory(Program.Pso2RootDir & "\plugins\disabled\")
                    NewInstall = True
                End If

                If Not Dns.GetHostEntry("gs001.pso2gs.net").AddressList(0).ToString().Contains("210.189.") And File.Exists(Program.Pso2RootDir & "\plugins\disabled\PSO2Proxy.dll") = True And File.Exists(Program.Pso2RootDir & "\plugins\PSO2Proxy.dll") = False Then
                    Helper.WriteDebugInfo("PSO2Proxy usage detected! Auto-enabling PSO2Proxy plugin.")
                    File.Move((Program.Pso2RootDir & "\plugins\disabled\PSO2Proxy.dll"), (Program.Pso2RootDir & "\plugins\PSO2Proxy.dll"))
                End If
            End If
            CheckForPluginUpdates()

            'Helper.WriteDebugInfoSameLine(Resources.strDone)
        Catch ex As Exception
            Helper.LogWithException(Resources.strERROR, ex)
        End Try

        Helper.DeleteFile("Story MD5HashList.txt")
        Helper.DeleteFile("PSO2 Tweaker Updater.exe")
        Helper.WriteDebugInfo(Resources.strAllDoneSystemReady)
        btnQUANTUMSYSTEM.Enabled = True
        ButtonItem10.Enabled = True
        'btnLaunchPSO2.Enabled = True
    End Sub

    Private Sub CheckForTweakerUpdates()
        Helper.WriteDebugInfo(Resources.strCheckingforupdatesPleasewaitamoment)
        DLS.Headers("user-agent") = GetUserAgent()
        Dim source As String = DLS.DownloadString(Program.FreedomUrl & "version.xml")

        If Not String.IsNullOrEmpty(source) AndAlso source.Contains("<VersionHistory>") Then
            Dim xm As New XmlDocument
            xm.LoadXml(source)

            Dim xmlNode = xm.SelectSingleNode("//CurrentVersion")
            Dim currentVersion As String = xmlNode.ChildNodes(0).InnerText.Trim

            Helper.Log("Checking for the latest version of the program...")

            If Application.Info.Version.ToString() = currentVersion Then
                Helper.WriteDebugInfo((Resources.strYouhavethelatestversionoftheprogram & Application.Info.Version.ToString()))
            Else
                Dim changelogtotal As String = ""

                For index As Integer = 2 To 11
                    Dim innerText = xmlNode.ChildNodes(index).InnerText.Trim
                    If Not String.IsNullOrEmpty(innerText) Then changelogtotal &= vbCrLf & innerText
                Next

                Dim updateyesno As MsgBoxResult = MsgBox(Resources.strYouareusinganoutdatedversionoftheprogram & Application.Info.Version.ToString() & Resources.strAndthelatestis & currentVersion & Resources.strWouldyouliketodownloadthenewversion & vbCrLf & vbCrLf & Resources.strChanges & vbCrLf & changelogtotal, MsgBoxStyle.YesNo)
                If updateyesno = MsgBoxResult.Yes Then
                    Helper.WriteDebugInfo(Resources.strDownloadingUpdate)
                    DownloadFile(Program.FreedomUrl & "PSO2%20Tweaker%20Updater.exe", "PSO2 Tweaker Updater.exe")
                    Process.Start(Environment.CurrentDirectory & "\PSO2 Tweaker Updater.exe")
                    Threading.Thread.Sleep(30000)
                    Helper.WriteDebugInfoAndFailed("Update seems to have failed... ")
                    Application.DoEvents()
                    Return
                End If
            End If
        Else
            Helper.WriteDebugInfoAndWarning(Resources.strFailedToGetUpdateInfo)
        End If
    End Sub

    Private Shared Function IsPso2WinDirMissing() As Boolean
        If Program.Pso2RootDir = "lblDirectory" OrElse Not Directory.Exists(Program.Pso2WinDir) Then
            MsgBox(Resources.strPleaseSelectwin32Dir)
            Helper.SelectPso2Directory()
            Return True
        End If
        Return False
    End Function

    Public Sub WriteDebugInfo(ByVal addThisText As String)
        If addThisText.Contains("PSO2 Tweaker ver ") Then Exit Sub
        If rtbDebug.InvokeRequired Then
            rtbDebug.Invoke(New Action(Of String)(AddressOf WriteDebugInfo), Text)
        Else
            rtbDebug.Text &= (vbCrLf & addThisText)
        End If
    End Sub

    Public Sub WriteDebugInfoSameLine(ByVal addThisText As String)
        If rtbDebug.InvokeRequired Then
            rtbDebug.Invoke(New Action(Of String)(AddressOf WriteDebugInfoSameLine), Text)
        Else
            rtbDebug.Text &= (" " & addThisText)
        End If
    End Sub

    Public Sub WriteDebugInfoAndOk(ByVal addThisText As String)
        If rtbDebug.InvokeRequired Then
            rtbDebug.Invoke(New Action(Of String)(AddressOf WriteDebugInfoAndOk), Text)
        Else
            rtbDebug.Text &= (vbCrLf & addThisText)
            rtbDebug.Select(rtbDebug.TextLength, 0)
            rtbDebug.SelectionColor = Color.Green
            rtbDebug.AppendText(" [OK!]")
            rtbDebug.SelectionColor = rtbDebug.ForeColor
        End If
    End Sub

    Public Sub WriteDebugInfoAndWarning(ByVal addThisText As String)
        If rtbDebug.InvokeRequired Then
            rtbDebug.Invoke(New Action(Of String)(AddressOf WriteDebugInfoAndWarning), Text)
        Else
            rtbDebug.Text &= (vbCrLf & addThisText)
            rtbDebug.Select(rtbDebug.TextLength, 0)
            rtbDebug.SelectionColor = Color.Red
            rtbDebug.AppendText(" [WARNING!]")
            rtbDebug.SelectionColor = rtbDebug.ForeColor
        End If
    End Sub

    Public Sub WriteDebugInfoAndFailed(ByVal addThisText As String)
        If rtbDebug.InvokeRequired Then
            rtbDebug.Invoke(New Action(Of String)(AddressOf WriteDebugInfoAndFailed), Text)
        Else
            If addThisText = "ERROR - Index was outside the bounds of the array." Then Return
            If addThisText = "ERROR - Object reference not set to an instance of an object." Then Return
            rtbDebug.Text &= (vbCrLf & addThisText)
            rtbDebug.Select(rtbDebug.TextLength, 0)
            rtbDebug.SelectionColor = Color.Red
            rtbDebug.AppendText(Resources.strFAILED)
            rtbDebug.SelectionColor = rtbDebug.ForeColor
            UnlockGui()
        End If
    End Sub

    Private Shared Sub rtbDebug_LinkClicked(sender As Object, e As LinkClickedEventArgs) Handles rtbDebug.LinkClicked
        Process.Start(e.LinkText)
    End Sub

    Private Sub rtbDebug_MouseClick(sender As Object, e As MouseEventArgs) Handles rtbDebug.MouseClick
        If e.Button = MouseButtons.Right Then
            cmsTextBarOptions.Show(DirectCast(sender, Control), e.Location)
        End If
    End Sub

    Private Sub rtbDebug_TextChanged(sender As Object, e As EventArgs) Handles rtbDebug.TextChanged
        rtbDebug.SelectionStart = rtbDebug.Text.Length
    End Sub

    Private Sub OnDownloadProgressChanged(ByVal sender As Object, ByVal e As DownloadProgressChangedEventArgs) Handles DLS.DownloadProgressChanged
        Dim totalSize As Long = e.TotalBytesToReceive
        _totalsize2 = totalSize
        Dim downloadedBytes As Long = e.BytesReceived
        Dim percentage As Integer = e.ProgressPercentage
        PBMainBar.Value = percentage
        PBMainBar.Text = (Resources.strDownloaded & Helper.SizeSuffix(downloadedBytes) & " / " & Helper.SizeSuffix(totalSize) & " (" & percentage & "%) - " & Resources.strRightClickforOptions)
        'Put your progress UI here, you can cancel download by uncommenting the line below
    End Sub

    Private Sub OnFileDownloadCompleted(ByVal sender As Object, ByVal e As AsyncCompletedEventArgs) Handles DLS.DownloadFileCompleted
        PBMainBar.Value = 0
        PBMainBar.Text = ""
    End Sub

    Public Sub DownloadFile(ByVal address As String, ByVal filename As String)
        If address.Contains(".jp") Then
            DLS.Headers("user-agent") = "AQUA_HTTP"
        Else
            DLS.Headers("user-agent") = GetUserAgent()
        End If

        DLS.Timeout = 10000

        While DLS.IsBusy
            Application.DoEvents()
            Thread.Sleep(16)
        End While

        DLS.DownloadFileAsync(New Uri(address), filename)

        While DLS.IsBusy
            Application.DoEvents()
            Thread.Sleep(16)

            If _restartplz Then
                DLS.CancelAsync()
                _restartplz = False
                DownloadFile(address, filename)
                Exit While
            End If

            If Not Visible And SkipDialogs = False Then
                DLS.CancelAsync()
                _cancelled = True
                _cancelledFull = True
                Windows.Forms.Application.Exit()
            End If
        End While
    End Sub

    Public Sub LockGui()
        Dim ctrl As Control
        For Each ctrl In Me.Controls
            ctrl.Enabled = False
        Next
        RibbonControl.Enabled = True
        Office2007StartButton1.Enabled = False
        rtbDebug.Enabled = True
        'Enabled = False
    End Sub

    Public Sub UnlockGui()
        Enabled = True
        Dim ctrl As Control
        For Each ctrl In Me.Controls
            ctrl.Enabled = True
        Next
        Office2007StartButton1.Enabled = True
    End Sub

    Private Shared Function MergePatches() As Dictionary(Of String, String)
        Dim patchlist As String() = File.ReadAllLines("patchlist.txt")
        Dim patchlistOld As String() = File.ReadAllLines("patchlist_old.txt")

        Dim result = New Dictionary(Of String, String)

        For index As Integer = 0 To (patchlist.Length - 1)
            Dim currentLine = patchlist(index)
            If String.IsNullOrEmpty(currentLine) Then Continue For

            Dim filename = Regex.Split(currentLine, ".pat")
            If String.IsNullOrEmpty(filename(0)) Then Continue For

            Dim key = filename(0).Replace("data/win32/", "")

            If Not result.ContainsKey(key) Then
                result.Add(key, currentLine)
            End If
        Next

        For index As Integer = 0 To (patchlistOld.Length - 1)
            Dim currentLine = patchlistOld(index)
            If String.IsNullOrEmpty(currentLine) Then Continue For

            Dim filename = Regex.Split(currentLine, ".pat")
            If String.IsNullOrEmpty(filename(0)) Then Continue For

            Dim key = filename(0).Replace("data/win32/", "")

            If Not result.ContainsKey(key) Then
                result.Add(key, currentLine)
            End If
        Next

        Return result
    End Function

    Private Shared Function MergePrePatches() As Dictionary(Of String, String)
        Dim result As New Dictionary(Of String, String)

        For i As Integer = 0 To 5
            Dim patchlist = File.ReadAllLines("patchlist" & i & ".txt")
            'Dim patchlist = DlwuaString("http://download.pso2.jp/patch_prod/patches_precede/patchlist" & i & ".txt").Split(ControlChars.Cr)

            For index As Integer = 0 To (patchlist.Length - 1)
                If String.IsNullOrEmpty(patchlist(index)) Then Continue For

                Dim splitLine = patchlist(index).Split(ControlChars.Tab)
                Dim fileName = Path.GetFileNameWithoutExtension(splitLine(0).Replace("data/win32/", ""))
                Dim hash = splitLine(2)

                If (Not String.IsNullOrEmpty(fileName)) AndAlso (Not result.ContainsKey(hash)) Then
                    result.Add(hash, fileName)
                End If
            Next
        Next

        Return result
    End Function

    Private Sub frmMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        DLS.CancelAsync()
        _cancelled = True
        PBMainBar.Value = 0
        PBMainBar.Text = ""
        Helper.DeleteFile("launcherlist.txt")
        Helper.DeleteFile("patchlist.txt")
        Helper.DeleteFile("patchlist_old.txt")
        Helper.DeleteFile("version.ver")
        Windows.Forms.Application.ExitThread()
    End Sub

    Private Shared Sub frmMain_Leave(sender As Object, e As EventArgs) Handles Me.Leave
        Windows.Forms.Application.Exit()
    End Sub

    Private Sub CheckForStoryUpdates()
        Try
            If RegKey.GetValue(Of String)(RegKey.StoryPatchVersion) = "Not Installed" Then Return
            'We're going to comment this out for the moment, but DO NOT REMOVE IT as it may be used again in the future... [AIDA]
            'And we did end up using it again for the plugin system. Thank you past AIDA <3 [AIDA]
            'DownloadFile(Program.FreedomUrl & "patchfiles/Story%20MD5HashList.txt", "Story MD5HashList.txt")

            'Using oReader As StreamReader = File.OpenText("Story MD5HashList.txt")
            'Dim strNewDate As String = oReader.ReadLine()
            'RegKey.SetValue(Of String)(RegKey.NewVersionTemp, strNewDate)
            'RegKey.SetValue(Of String)(RegKey.NewStoryVersion, strNewDate)
            'If strNewDate <> RegKey.GetValue(Of String)(RegKey.StoryPatchVersion) Then
            'A new story patch update is available - Would you like to download and install it? PLEASE NOTE: This update assumes you've already downloaded and installed the latest RAR file available from http://arks-layer.com, which seems to be: 
            ' Create a match using regular exp<b></b>ressions
            'http://arks-layer.com/Story%20Patch%208-8-2013.rar.torrent
            ' Spit out the value plucked from the code
            txtHTML.Text = Regex.Match(Program.Client.DownloadString("http://arks-layer.com/story.php"), "<u>.*?</u>").Value
            Dim strDownloadMe = txtHTML.Text.Replace("<u>", "").Replace("</u>", "")
            If RegKey.GetValue(Of String)(RegKey.LatestStoryBase) <> strDownloadMe Then
                DLS.Headers("user-agent") = GetUserAgent()
                Dim StoryChangeLog As String = DLS.DownloadString("http://arks-layer.com/storyupdates.txt")
                Dim mbVisitLink As MsgBoxResult = MsgBox("A new story patch is available! Would you like to download and install it? Here's a list of changes: " & vbCrLf & StoryChangeLog, MsgBoxStyle.YesNo)
                If mbVisitLink = vbYes Then InstallStoryPatchNew()
                Return
            End If

        Catch ex As Exception
            Helper.LogWithException(Resources.strERROR, ex)
        End Try
    End Sub

    Private Sub CheckForEnPatchUpdates()
        Try
            If RegKey.GetValue(Of String)(RegKey.EnPatchVersion) = "Not Installed" Then Return
            Application.DoEvents()
            DLS.Headers("user-agent") = GetUserAgent()
            Dim lastfilename As String() = DLS.DownloadString(Program.FreedomUrl & "patches/enpatch.txt").Split("/"c)
            Dim strVersion As String = lastfilename(lastfilename.Length - 1).Replace(".rar", "")
            RegKey.SetValue(Of String)(RegKey.NewEnVersion, strVersion)

            If RegKey.GetValue(Of String)(RegKey.EnPatchVersion) <> strVersion Then
                If MsgBox(Resources.strNewENPatch, vbYesNo) = vbYes Then
                    DownloadEnglishPatch()
                End If
            End If
        Catch ex As Exception
            Helper.LogWithException(Resources.strERROR, ex)
        End Try
    End Sub

    Private Sub CheckForLargeFilesUpdates()
        Try
            If RegKey.GetValue(Of String)(RegKey.LargeFilesVersion) = "Not Installed" Then Return

            Application.DoEvents()
            DLS.Headers("user-agent") = GetUserAgent()
            Dim strVersion As String = DLS.DownloadString(Program.FreedomUrl & "patches/largefilesTRANSAM.txt").Replace("-", "/")

            RegKey.SetValue(Of String)(RegKey.NewLargeFilesVersion, strVersion)

            If RegKey.GetValue(Of String)(RegKey.LargeFilesVersion) <> strVersion Then
                If MsgBox("An update for the Large Files is available. Would you like to install it via TRANSAM?", vbYesNo) = vbYes Then
                    btnLargeFilesTRANSAM.RaiseClick()
                End If
            End If
        Catch ex As Exception
            Helper.LogWithException(Resources.strERROR, ex)
        End Try
    End Sub

    Private Async Function CheckForPso2Updates(comingFromPrePatch As Boolean) As Task

        Try
            'Precede file, syntax is Yes/No:<Dateoflastprepatch>
            DownloadFile(Program.FreedomUrl & "precede.txt", "precede.txt")
            Dim precedeSplit As String() = File.ReadAllLines("precede.txt")(0).Split(":"c)

            Dim precedeversionstring As String = precedeSplit(1)
            Helper.DeleteFile("precede.txt")
            If comingFromPrePatch Then
                DownloadPrePatch(precedeversionstring)
            Else
                Dim firstTimechecking As Boolean = String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.Pso2PrecedeVersion))
                If firstTimechecking Then RegKey.SetValue(Of String)(RegKey.Pso2PrecedeVersion, precedeversionstring)

                If precedeSplit(0) = "Yes" AndAlso (RegKey.GetValue(Of String)(RegKey.Pso2PrecedeVersion) <> precedeversionstring OrElse firstTimechecking) Then
                    Dim downloadPrepatchResult As MsgBoxResult = MsgBox("New pre-patch data is available to download - Would you like to download it? This is optional, and will let you download some of a large patch now, as opposed to having a larger download all at once when it is released.", MsgBoxStyle.YesNo)
                    If downloadPrepatchResult = vbYes Then DownloadPrePatch(precedeversionstring)
                End If
            End If

            'Check whether or not to apply pre-patch shit. Ugh.
            If Directory.Exists(Program.Pso2RootDir & "\_precede\") AndAlso Directory.Exists(Program.Pso2RootDir & "\_precede\data\win32\") Then
                DownloadFile(Program.FreedomUrl & "precede_apply.txt", "precede_apply.txt")
                If File.ReadAllLines("precede_apply.txt")(0) = "Yes" Then
                    InstallPrePatch()
                End If
            End If

            If Not comingFromPrePatch Then
                Try
                    Dim source As String = Program.AreYouAlive.DownloadString("http://arks-layer.com/vanila/version.txt")
                    If String.IsNullOrEmpty(source) Then
                        Helper.Log("ERROR: Wasn't able to contact Arks Layer, help!")
                        Helper.WriteDebugInfo("Failed to get the PSO2 Version update information. Usually, this means AIDA broke something. Please DO NOT panic, as the rest of the program will work fine. There is no need to report this error either.")
                        Exit Function
                    End If
                Catch ex As Exception
                    Helper.LogWithException(Resources.strERROR, ex)
                    Helper.WriteDebugInfo("Failed to get the PSO2 Version information. Usually, this means AIDA broke something. Please DO NOT panic, as the rest of the program will work fine. There is no need to report this error either.")
                    Exit Function
                End Try
                DownloadFile("http://arks-layer.com/vanila/version.txt", "version.ver")
                Dim version = File.ReadAllLines("version.ver")(0)
                If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.Pso2RemoteVersion)) Then
                    RegKey.SetValue(Of String)(RegKey.Pso2RemoteVersion, version)
                End If

                If RegKey.GetValue(Of String)(RegKey.Pso2RemoteVersion) <> version Then
                    If MsgBox(Resources.strNewPSO2Update, vbYesNo) = vbYes Then
                        LockGui()
                        ' Use IOC Container in the main Tweaker project to deal with dependencies.
                        Dim output As New TweakerTrigger
                        Dim Settings = New RegistryTweakerSettings("Software\AIDA")
                        Dim updater = New UpdateManager(Settings, output)

                        'await updater.CleanLegacyFiles();

                        'Console.WriteLine(settings.GameDirectory)
                        Try

                            'frmDownloader.TopMost = True
                            'Me.TopMost = False
                        Catch ex As Exception
                            Helper.LogWithException(Resources.strERROR, ex)
                        End Try
                        frmDownloader.CleanupUI()
                        RegKey.SetValue(Of String)(RegKey.StoryPatchVersion, "Not Installed")
                        RegKey.SetValue(Of String)(RegKey.EnPatchVersion, "Not Installed")
                        RegKey.SetValue(Of String)(RegKey.LargeFilesVersion, "Not Installed")
                        Await updater.Update(False, False)
                    End If
                End If
            End If
        Catch ex As Exception
            Helper.LogWithException(Resources.strERROR, ex)
        End Try
    End Function

    Private Sub InstallPrePatch()
        Dim applyPrePatchYesNo As MsgBoxResult = MsgBox("It appears that it's time to install the pre-patch download - Is this okay? If you select no, the pre-patch will not be installed.", vbYesNo)
        If applyPrePatchYesNo = vbYes Then
            'WriteDebugInfo("Restoring backup of vanilla JP files...")
            '[AIDA] Apply the precede   
            If Directory.Exists(BuildBackupPath(EnglishPatch)) Then RestoreBackup(EnglishPatch)
            If Directory.Exists(BuildBackupPath(LargeFiles)) Then RestoreBackup(LargeFiles)
            If Directory.Exists(BuildBackupPath(StoryPatch)) Then RestoreBackup(StoryPatch)
            Helper.WriteDebugInfo("Installing prepatch, please wait...")
            Application.DoEvents()

            'list the names of all files in the specified directory
            Dim files = New DirectoryInfo(Program.Pso2RootDir & "\_precede\data\win32\").GetFiles()
            Dim counter = files.Length
            Dim count As Integer = 0

            For Each dra As FileInfo In files
                Dim downloadStr As String = dra.Name
                Helper.DeleteFile((Program.Pso2WinDir & "\" & downloadStr))
                File.Move(Program.Pso2RootDir & "\_precede\data\win32\" & downloadStr, (Program.Pso2WinDir & "\" & downloadStr))
                count += 1
                lblStatus.Text = "Moved " & count & " files out of " & counter
                Application.DoEvents()
            Next
            If File.Exists("win32list_DO_NOT_DELETE_ME.txt") Then File.Delete("win32list_DO_NOT_DELETE_ME.txt")
            Helper.WriteDebugInfoSameLine("Done!")
            Helper.DeleteDirectory(Program.Pso2RootDir & "\_precede")
            RegKey.SetValue(Of String)(RegKey.JustPrepatched, "True")
        End If
    End Sub

    Private Sub DownloadPrePatch(version As String)
        _cancelledFull = False
        lblStatus.Text = ""
        Helper.WriteDebugInfo("Downloading pre-patch filelist...")
        DownloadFile("http://download.pso2.jp/patch_prod/patches_precede/patchlist0.txt", "patchlist0.txt")
        DownloadFile("http://download.pso2.jp/patch_prod/patches_precede/patchlist1.txt", "patchlist1.txt")
        DownloadFile("http://download.pso2.jp/patch_prod/patches_precede/patchlist2.txt", "patchlist2.txt")
        DownloadFile("http://download.pso2.jp/patch_prod/patches_precede/patchlist3.txt", "patchlist3.txt")
        DownloadFile("http://download.pso2.jp/patch_prod/patches_precede/patchlist4.txt", "patchlist4.txt")
        DownloadFile("http://download.pso2.jp/patch_prod/patches_precede/patchlist5.txt", "patchlist5.txt")
        Helper.WriteDebugInfoSameLine(Resources.strDone)
        If Not Directory.Exists(Program.Pso2RootDir & "\_precede\data\win32") Then Directory.CreateDirectory(Program.Pso2RootDir & "\_precede\data\win32")

        Dim mergedPrePatches = MergePrePatches()
        mergedPrePatches.Remove("GameGuard.des")
        mergedPrePatches.Remove("PSO2JP.ini")
        mergedPrePatches.Remove("script/user_default.pso2")
        mergedPrePatches.Remove("script/user_intel.pso2")
        mergedPrePatches.Remove("")

        Helper.WriteDebugInfo("Checking for already existing precede files...")

        Dim missingfiles As New List(Of String)
        Dim truefilename As String
        Dim numberofChecks As Integer = 0

        For Each sBuffer In mergedPrePatches
            If _cancelledFull Then Return

            truefilename = sBuffer.Value

            If Not File.Exists(((Program.Pso2RootDir & "\_precede\data\win32") & "\" & truefilename)) Then
                If _vedaUnlocked Then Helper.WriteDebugInfo("DEBUG: The file " & truefilename & " is missing.")
                missingfiles.Add(truefilename)
            ElseIf Helper.GetMd5(((Program.Pso2RootDir & "\_precede\data\win32") & "\" & truefilename)) <> sBuffer.Key Then
                If _vedaUnlocked Then Helper.WriteDebugInfo("DEBUG: The file " & truefilename & " must be redownloaded.")
                missingfiles.Add(truefilename)
            End If

            numberofChecks += 1
            lblStatus.Text = (Resources.strCurrentlyCheckingFile & numberofChecks & "")
            Application.DoEvents()
        Next

        Dim totaldownload As Long = missingfiles.Count
        Dim downloaded As Long = 0
        Dim totaldownloaded As Long = 0

        For Each downloadStr As String In missingfiles
            If _cancelledFull Then Return
            'Download the missing files:
            'WHAT THE FUCK IS GOING ON HERE?
            downloaded += 1
            totaldownloaded += _totalsize2

            lblStatus.Text = Resources.strDownloading & "" & downloaded & "/" & totaldownload & " (" & Helper.SizeSuffix(totaldownloaded) & ")"

            Application.DoEvents()
            _cancelled = False
            DownloadFile(("http://download.pso2.jp/patch_prod/patches_precede/data/win32/" & downloadStr & ".pat"), downloadStr)
            If New FileInfo(downloadStr).Length = 0 Then
                Helper.Log("File appears to be empty, trying to download from secondary SEGA server")
                DownloadFile(("http://download.pso2.jp/patch_prod/patches_precede/data/win32/" & downloadStr & ".pat"), downloadStr)
            End If

            If _cancelled Then Return

            Helper.DeleteFile(((Program.Pso2RootDir & "\_precede\data\win32") & "\" & downloadStr))
            File.Move(downloadStr, ((Program.Pso2RootDir & "\_precede\data\win32") & "\" & downloadStr))
            If _vedaUnlocked Then Helper.WriteDebugInfo("DEBUG: Downloaded and installed " & downloadStr & ".")
            Application.DoEvents()
        Next

        If missingfiles.Count = 0 Then Helper.WriteDebugInfo("Your precede data is up to date!")
        If missingfiles.Count <> 0 Then
            Helper.WriteDebugInfo("Precede data downloaded/updated!")
        End If
        RegKey.SetValue(Of String)(RegKey.Pso2PrecedeVersion, version)
    End Sub

    Private Sub btnApplyChanges_Click(sender As Object, e As EventArgs) Handles btnApplyChanges.Click
        Try
            If IsPso2WinDirMissing() Then Return
            Helper.Log("Restoring/Removing files...")
            If chkRemoveNVidia.Checked AndAlso chkRestoreNVidia.Checked Then
                MsgBox(Resources.strYouCannotRemoveRestore)
                Return
            End If
            If chkRemovePC.Checked AndAlso chkRestorePC.Checked Then
                MsgBox(Resources.strYouCannotRemoveRestore)
                Return
            End If
            If chkRemoveVita.Checked AndAlso chkRestoreVita.Checked Then
                MsgBox(Resources.strYouCannotRemoveRestore)
                Return
            End If
            If chkRemoveSEGA.Checked AndAlso chkRestoreSEGA.Checked Then
                MsgBox(Resources.strYouCannotRemoveRestore)
                Return
            End If

            If Directory.Exists(Program.Pso2WinDir & "\backup\") = False Then
                Helper.Log("Could not find the backup path! Are you sure you have a backup in your win32/backup folder? Let's make one.")
                Directory.CreateDirectory(Program.Pso2WinDir & "\backup\")
                Return
            End If

            'Remove PC Opening Video [Done]
            If chkRemovePC.Checked AndAlso File.Exists((Program.Pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) Then
                If File.Exists((Program.Pso2WinDir & "\backup\a44fbb2aeb8084c5a5fbe80e219a9927")) Then Computer.FileSystem.DeleteFile((Program.Pso2WinDir & "\backup\a44fbb2aeb8084c5a5fbe80e219a9927"), UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently)
                File.Move((Program.Pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927"), (Program.Pso2WinDir & "\backup\a44fbb2aeb8084c5a5fbe80e219a9927"))
                Helper.WriteDebugInfoAndOk(Resources.strRemoving & "PC Opening Video...")
            ElseIf chkRemovePC.Checked AndAlso (Not File.Exists((Program.Pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927"))) Then
                Helper.WriteDebugInfoAndWarning(Resources.strFailedToRemovePC)
            End If

            'Restore PC Opening Video [Done]
            If chkRestorePC.Checked AndAlso File.Exists((Program.Pso2WinDir & "\backup\a44fbb2aeb8084c5a5fbe80e219a9927")) Then
                If File.Exists((Program.Pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) Then Computer.FileSystem.DeleteFile((Program.Pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927"), UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently)
                File.Move((Program.Pso2WinDir & "\backup\a44fbb2aeb8084c5a5fbe80e219a9927"), (Program.Pso2WinDir & "\a44fbb2aeb8084c5a5fbe80e219a9927"))
                Helper.WriteDebugInfoAndOk(Resources.strRestoring & "PC Opening Video...")
            ElseIf chkRestorePC.Checked AndAlso (Not File.Exists((Program.Pso2WinDir & "\backup\a44fbb2aeb8084c5a5fbe80e219a9927"))) Then
                Helper.WriteDebugInfoAndWarning(Resources.strFailedToRestorePC)
            End If

            'Remove Vita Opening Video [Done]
            If chkRemoveVita.Checked AndAlso File.Exists((Program.Pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585")) Then
                If File.Exists((Program.Pso2WinDir & "\backup\a93adc766eb3510f7b5c279551a45585")) Then Computer.FileSystem.DeleteFile((Program.Pso2WinDir & "\backup\a93adc766eb3510f7b5c279551a45585"), UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently)
                File.Move((Program.Pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585"), (Program.Pso2WinDir & "\backup\a93adc766eb3510f7b5c279551a45585"))
                Helper.WriteDebugInfoAndOk(Resources.strRemoving & "Vita Opening Video...")
            ElseIf chkRemoveVita.Checked AndAlso (Not File.Exists((Program.Pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585"))) Then
                Helper.WriteDebugInfoAndWarning(Resources.strFailedToRemoveVita)
            End If

            'Restore Vita Opening Video [Done]
            If chkRestoreVita.Checked AndAlso File.Exists((Program.Pso2WinDir & "\backup\a93adc766eb3510f7b5c279551a45585")) Then
                If File.Exists((Program.Pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585")) Then Computer.FileSystem.DeleteFile((Program.Pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585"), UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently)
                File.Move((Program.Pso2WinDir & "\backup\a93adc766eb3510f7b5c279551a45585"), (Program.Pso2WinDir & "\a93adc766eb3510f7b5c279551a45585"))
                Helper.WriteDebugInfoAndOk(Resources.strRestoring & "Vita Opening Video...")
            ElseIf chkRestoreVita.Checked AndAlso (Not File.Exists((Program.Pso2WinDir & "\backup\a93adc766eb3510f7b5c279551a45585"))) Then
                Helper.WriteDebugInfoAndWarning(Resources.strFailedToRestoreVita)
            End If

            'Remove NVidia Opening Video [Done]
            If chkRemoveNVidia.Checked AndAlso File.Exists((Program.Pso2WinDir & "\" & "7f2368d207e104e8ed6086959b742c75")) Then
                If File.Exists((Program.Pso2WinDir & "\backup\7f2368d207e104e8ed6086959b742c75")) Then Computer.FileSystem.DeleteFile((Program.Pso2WinDir & "\backup\7f2368d207e104e8ed6086959b742c75"), UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently)
                File.Move((Program.Pso2WinDir & "\" & "7f2368d207e104e8ed6086959b742c75"), (Program.Pso2WinDir & "\backup\7f2368d207e104e8ed6086959b742c75"))
                Helper.WriteDebugInfoAndOk(Resources.strRemoving & "NVidia Opening Video...")
            ElseIf chkRemoveNVidia.Checked AndAlso (Not File.Exists((Program.Pso2WinDir & "\" & "7f2368d207e104e8ed6086959b742c75"))) Then
                Helper.WriteDebugInfoAndWarning(Resources.strFailedToRemoveNVidia)
            End If

            'Restore NVidia Opening Video [Done]
            If chkRestoreNVidia.Checked AndAlso File.Exists((Program.Pso2WinDir & "\backup\7f2368d207e104e8ed6086959b742c75")) Then
                If File.Exists((Program.Pso2WinDir & "\" & "7f2368d207e104e8ed6086959b742c75")) Then Computer.FileSystem.DeleteFile((Program.Pso2WinDir & "\" & "7f2368d207e104e8ed6086959b742c75"), UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently)
                File.Move((Program.Pso2WinDir & "\backup\7f2368d207e104e8ed6086959b742c75"), (Program.Pso2WinDir & "\7f2368d207e104e8ed6086959b742c75"))
                Helper.WriteDebugInfoAndOk(Resources.strRestoring & "NVidia Opening Video...")
            ElseIf chkRestoreNVidia.Checked AndAlso (Not File.Exists((Program.Pso2WinDir & "\backup\7f2368d207e104e8ed6086959b742c75"))) Then
                Helper.WriteDebugInfoAndWarning(Resources.strFailedToRestoreNVidia)
            End If

            'Remove SEGA Opening Video [Done]
            If chkRemoveSEGA.Checked AndAlso File.Exists((Program.Pso2WinDir & "\" & "009bfec69b04a34576012d50e3417771")) Then
                If File.Exists((Program.Pso2WinDir & "\backup\009bfec69b04a34576012d50e3417771")) Then Computer.FileSystem.DeleteFile((Program.Pso2WinDir & "\backup\009bfec69b04a34576012d50e3417771"), UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently)
                File.Move((Program.Pso2WinDir & "\" & "009bfec69b04a34576012d50e3417771"), (Program.Pso2WinDir & "\backup\009bfec69b04a34576012d50e3417771"))
                Helper.WriteDebugInfoAndOk(Resources.strRemoving & "SEGA Opening Video...")
            ElseIf chkRemoveSEGA.Checked AndAlso (Not File.Exists((Program.Pso2WinDir & "\" & "009bfec69b04a34576012d50e3417771"))) Then
                Helper.WriteDebugInfoAndWarning(Resources.strFailedToRemoveSEGA)
            End If

            'Restore SEGA Opening Video [Done]
            If chkRestoreSEGA.Checked AndAlso File.Exists((Program.Pso2WinDir & "\backup\009bfec69b04a34576012d50e3417771")) Then
                If File.Exists((Program.Pso2WinDir & "\" & "009bfec69b04a34576012d50e3417771")) Then Computer.FileSystem.DeleteFile((Program.Pso2WinDir & "\" & "009bfec69b04a34576012d50e3417771"), UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently)
                File.Move((Program.Pso2WinDir & "\backup\009bfec69b04a34576012d50e3417771"), (Program.Pso2WinDir & "\009bfec69b04a34576012d50e3417771"))
                Helper.WriteDebugInfoAndOk(Resources.strRestoring & "SEGA Opening Video...")
            ElseIf chkRestoreSEGA.Checked AndAlso (Not File.Exists((Program.Pso2WinDir & "\backup\009bfec69b04a34576012d50e3417771"))) Then
                Helper.WriteDebugInfoAndWarning(Resources.strFailedToRestoreSEGA)
            End If

            UnlockGui()

        Catch ex As Exception
            Helper.LogWithException(Resources.strERROR, ex)
        End Try
    End Sub

    Private Sub btnLaunchPSO2_Click(sender As Object, e As EventArgs) Handles btnLaunchPSO2.Click
        'Fuck SEGA. Stupid jerks.
        Helper.Log("Check if PSO2 is running")
        If Helper.CheckIfRunning("pso2") Then Return
        Try
            If IsPso2WinDirMissing() Then Return

            If Not File.Exists(Program.Pso2RootDir & "\pso2.exe") Then
                Helper.WriteDebugInfoAndFailed(Resources.strCouldNotFindPSO2EXE)
                Return
            End If

            DLS.CancelAsync()
            _cancelled = True
            If SkipDialogs = False Then PBMainBar.Value = 0
            If SkipDialogs = False Then PBMainBar.Text = ""
            Helper.WriteDebugInfo(Resources.strLaunchingPSO2)

            If File.Exists(Program.Pso2RootDir & "\plugins\translator.dll") = True Then
                DownloadFile(Program.FreedomUrl & "working.txt", "working.txt")
                If File.ReadAllLines("working.txt")(0) = "No" Then
                    'You baka ass mother fucker.
                    Cursor = Cursors.WaitCursor
                    Try

                        Baka.TopMost = TopMost
                        Baka.Top += 50
                        Baka.Left += 50
                        Baka.ShowDialog()
                        Exit Sub
                    Catch ex As Exception
                        Helper.LogWithException(Resources.strERROR, ex)
                    Finally
                        Cursor = Cursors.Default
                    End Try
                End If
            End If

            'End Item Translation stuff
            If Not Program.transOverride Then Helper.DeleteFile(Program.Pso2RootDir & "\ddraw.dll")
            If Not Program.transOverride Then File.WriteAllBytes(Program.Pso2RootDir & "\ddraw.dll", Resources.ddraw)
            Dim startInfo As ProcessStartInfo = New ProcessStartInfo With {.FileName = (Program.Pso2RootDir & "\pso2.exe"), .Arguments = "+0x33aca2b9", .UseShellExecute = False}
            startInfo.EnvironmentVariables("-pso2") = "+0x01e3f1e9"
            Dim shell As Process = New Process With {.StartInfo = startInfo}


            TRIALSystem("Request Access")

            'This code is no longer run because Gameguard sucks cock.
            'Maybe SEGA doesn't? WHO KNOWS. IT'S BACK IN.
            Helper.Log("Checking for extra GN Fields...")
            Dim processname As String = "GN Field"
            If Process.GetProcessesByName(processname).Length > 0 Then
                For Each proc As Process In Process.GetProcessesByName(processname)
                    proc.Kill()
                Next
            End If
            Helper.Log("Spinning GN Drives...")

            If Program.GNFieldActive = True And Program.ELSActive = False Then
                Helper.Log("GN Field Is supposed to be active! Let's start it!")
                Process.Start("GN Field.exe")
                'Maybe the sleep is the problem?
                'Thread.Sleep(100)
            End If

            If Program.GNFieldActive = True And Program.ELSActive = True Then
                Helper.Log("GN Field is supposed to be active, and the ELS are invading! Let's start it with a random name!")
                Process.Start(RegKey.GetValue(Of String)("GNFieldName"))
                'Maybe the sleep is the problem?
                'Thread.Sleep(100)
            End If

            If Program.GNFieldActive = False Then
                Try
                    Helper.Log("Start PSO2!")
                    shell.Start()
                Catch ex As Exception
                    Helper.Log("EXCEPTION, HELP! ;_;")
                    Helper.WriteDebugInfo(Resources.strItSeemsThereWasAnError)
                    DownloadFile("http://download.pso2.jp/patch_prod/patches/pso2.exe.pat", "pso2.exe")
                    If File.Exists((Program.Pso2RootDir & "\pso2.exe")) AndAlso Program.StartPath <> Program.Pso2RootDir Then Helper.DeleteFile((Program.Pso2RootDir & "\pso2.exe"))
                    File.Move("pso2.exe", (Program.Pso2RootDir & "\pso2.exe"))
                    Helper.WriteDebugInfoSameLine(Resources.strDone)
                    Helper.Log("Starting PSO2 again.")
                    shell.Start()
                End Try
            Else
                Thread.Sleep(100)
                Helper.Log("Waiting for GN Field to activate...")
                Thread.Sleep(60000)
                Helper.WriteDebugInfoAndFailed("GN Field failed to launch! Please restart the PSO2 Tweaker.")
                Helper.Log("Slept for 60 seconds, GN Field didn't launch. Exiting PSO2 Launch method.")
                Exit Sub
            End If



            Hide()
            Dim hWnd As IntPtr = External.FindWindow("Phantasy Star Online 2", Nothing)

            SkipDialogs = False
            tmrWaitingforPSO2.Enabled = True

            Do While hWnd = IntPtr.Zero
                hWnd = External.FindWindow("Phantasy Star Online 2", Nothing)
                Thread.Sleep(10)
                Application.DoEvents()
                If SkipDialogs = True Then Exit Do
            Loop

            If Not Program.transOverride Then Helper.DeleteFile(Program.Pso2RootDir & "\ddraw.dll")
            tmrWaitingforPSO2.Enabled = False
            If RegKey.GetValue(Of String)(RegKey.SteamMode) = "True" Then
                File.Copy(Program.Pso2RootDir & "\pso2.exe", Program.Pso2RootDir & "\pso2.exe_backup", True)
                Do While Helper.IsFileInUse(Program.Pso2RootDir & "\pso2.exe")
                    Thread.Sleep(1000)
                Loop
                File.Copy(Program.Pso2RootDir & "\pso2.exe_backup", Program.Pso2RootDir & "\pso2.exe", True)
                File.Delete(Program.Pso2RootDir & "\pso2.exe_backup")
            End If
            Close()

        Catch ex As Exception
            Helper.LogWithException(Resources.strERROR, ex)
        End Try
    End Sub

    Private Sub PBMainBar_MouseClick(sender As Object, e As MouseEventArgs) Handles PBMainBar.MouseClick
        If e.Button = MouseButtons.Right Then
            CancelDownloadToolStripMenuItem.Visible = DLS.IsBusy
            cmsProgressBar.Show(DirectCast(sender, Control), e.Location)
        End If
    End Sub

    Private Sub CancelDownloadToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CancelDownloadToolStripMenuItem.Click
        DLS.CancelAsync()
        Helper.WriteDebugInfo(Resources.strDownloadwasCancelled)
        _cancelled = True
        PBMainBar.Value = 0
        PBMainBar.Text = ""
        lblStatus.Text = ""
    End Sub

    Private Sub CancelProcessToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CancelProcessToolStripMenuItem.Click
        If DLS.IsBusy Then DLS.CancelAsync()
        _cancelled = True
        PBMainBar.Value = 0
        PBMainBar.Text = ""
        lblStatus.Text = ""
        _cancelledFull = True
        Helper.WriteDebugInfo(Resources.strProcessWasCancelled)
        UnlockGui()
    End Sub

    Private Shared Sub Button1_Click(sender As Object, e As EventArgs) Handles btnSelectPSODir.Click
        Helper.SelectPso2Directory()
    End Sub


    Private Sub btnRestoreENBackup_Click(sender As Object, e As EventArgs) Handles btnRestoreENBackup.Click
        Try
            If IsPso2WinDirMissing() Then Return
            If MsgBox(Resources.strAreYouSureRestoreBackup, vbYesNo) = MsgBoxResult.Yes Then
                RestoreBackup(EnglishPatch)
            End If
        Catch ex As Exception
            Helper.LogWithException(Resources.strERROR, ex)
        End Try
    End Sub

    Private Sub btnRestoreLargeFilesBackup_Click(sender As Object, e As EventArgs) Handles btnRestoreLargeFilesBackup.Click
        Try
            If IsPso2WinDirMissing() Then Return
            If MsgBox(Resources.strAreYouSureRestoreBackup, vbYesNo) = MsgBoxResult.Yes Then
                RestoreBackup(LargeFiles)
            End If
        Catch ex As Exception
            Helper.LogWithException(Resources.strERROR, ex)
        End Try
    End Sub

    Private Shared Sub btnRestoreJPNames_Click(sender As Object, e As EventArgs) Handles btnRestoreJPNames.Click
        'http://pso2.arghargh200.net/pso2/2013_06_12/ceffe0e2386e8d39f188358303a92a7d
        If File.Exists((Program.Pso2WinDir & "\backup\ceffe0e2386e8d39f188358303a92a7d")) Then
            If File.Exists((Program.Pso2WinDir & "\" & "ceffe0e2386e8d39f188358303a92a7d")) Then Computer.FileSystem.DeleteFile((Program.Pso2WinDir & "\" & "ceffe0e2386e8d39f188358303a92a7d"), UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently)
            File.Move((Program.Pso2WinDir & "\backup\ceffe0e2386e8d39f188358303a92a7d"), (Program.Pso2WinDir & "\" & "ceffe0e2386e8d39f188358303a92a7d"))
            Helper.WriteDebugInfoAndOk(Resources.strRestoring & " JP Names file...")
        Else
            Helper.WriteDebugInfoAndOk(Resources.strRestoreFailedPleaseReinstall)
        End If
    End Sub

    Private Shared Sub btnRestoreJPETrials_Click(sender As Object, e As EventArgs) Handles btnRestoreJPETrials.Click
        'http://pso2.arghargh200.net/pso2/2013_06_12/057aa975bdd2b372fe092614b0f4399e
        If File.Exists((Program.Pso2WinDir & "\backup\" & "057aa975bdd2b372fe092614b0f4399e")) Then
            If File.Exists((Program.Pso2WinDir & "\" & "057aa975bdd2b372fe092614b0f4399e")) Then Computer.FileSystem.DeleteFile((Program.Pso2WinDir & "\" & "057aa975bdd2b372fe092614b0f4399e"), UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently)
            File.Move((Program.Pso2WinDir & "\backup\057aa975bdd2b372fe092614b0f4399e"), (Program.Pso2WinDir & "\" & "057aa975bdd2b372fe092614b0f4399e"))
            Helper.WriteDebugInfoAndOk(Resources.strRestoring & " JP E-Trials file...")
        Else
            Helper.WriteDebugInfoAndOk(Resources.strRestoreFailedPleaseReinstall)
        End If
    End Sub

    Private Shared Sub btnAnalyze_Click(sender As Object, e As EventArgs) Handles btnAnalyze.Click
        Dim pso2Launchpath As String = Program.Pso2WinDir.Replace("data\win32", "")
        Helper.WriteDebugInfo(Resources.strCheckingForNecessaryFiles)
        If Not File.Exists(pso2Launchpath & "Gameguard.DES") Then Helper.WriteDebugInfoAndWarning("Missing GameGuard.DES file! " & Resources.strPleaseFixGG)
        If Not File.Exists(pso2Launchpath & "pso2.exe") Then Helper.WriteDebugInfoAndWarning("Missing pso2.exe file! " & Resources.strPleaseFixPSO2EXEs)
        If Not File.Exists(pso2Launchpath & "pso2launcher.exe") Then Helper.WriteDebugInfoAndWarning("Missing pso2launcher.exe file! " & Resources.strPleaseFixPSO2EXEs)
        If Not File.Exists(pso2Launchpath & "pso2updater.exe") Then Helper.WriteDebugInfoAndWarning("Missing pso2updater.exe file! " & Resources.strPleaseFixPSO2EXEs)
        Helper.WriteDebugInfoSameLine(Resources.strDone)
        Helper.WriteDebugInfo(Resources.strCheckingForFolders)
        If Not Directory.Exists(pso2Launchpath & "\Gameguard\") Then Helper.WriteDebugInfoAndWarning("Missing Gameguard folder! " & Resources.strPleaseFixGG)
        If Not Directory.Exists(pso2Launchpath & "\data\") Then Helper.WriteDebugInfoAndWarning("Missing data folder! " & Resources.strPleaseReinstallOrCheck)
        If Not Directory.Exists(pso2Launchpath & "\data\win32\") Then Helper.WriteDebugInfoAndWarning("Missing data\win32 folder! " & Resources.strPleaseReinstallOrCheck)
        Helper.WriteDebugInfoSameLine(Resources.strDone)
        Helper.WriteDebugInfo(Resources.strDoneTesting)
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If IsPso2WinDirMissing() Then Return
        Dim filename As String()
        Dim truefilename As String
        Dim missingfiles As New List(Of String)
        Dim filename2 As String()
        Dim truefilename2 As String
        Dim missingfiles2 As New List(Of String)
        Dim numberofChecks As Integer
        LockGui()
        Helper.WriteDebugInfo(Resources.strDownloadingPatchFile1)
        Application.DoEvents()
        DownloadFile("http://download.pso2.jp/patch_prod/patches/launcherlist.txt", "launcherlist.txt")
        Helper.WriteDebugInfoSameLine(Resources.strDone)
        Helper.WriteDebugInfo(Resources.strDownloadingPatchFile2)
        Application.DoEvents()
        DownloadFile("http://download.pso2.jp/patch_prod/patches/patchlist.txt", "patchlist.txt")
        Helper.WriteDebugInfoSameLine(Resources.strDone)
        Helper.WriteDebugInfo(Resources.strDownloadingPatchFile3)
        Application.DoEvents()
        DownloadFile("http://download.pso2.jp/patch_prod/patches_old/patchlist.txt", "patchlist_old.txt")
        Helper.WriteDebugInfoSameLine(Resources.strDone)
        Helper.WriteDebugInfo(Resources.strDownloadingPatchFile4)
        Application.DoEvents()
        DLS.DownloadFile("http://arks-layer.com/vanila/version.txt", "version.ver")
        Helper.WriteDebugInfoSameLine(Resources.strDone)
        Application.DoEvents()
        UnlockGui()
        Helper.Log("Opening patch file list...")
        Helper.WriteDebugInfo("Restoring openings/logos....")
        If _cancelledFull Then Return
        If File.Exists((Program.Pso2WinDir & "\backup\a44fbb2aeb8084c5a5fbe80e219a9927")) Then
            If File.Exists((Program.Pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) Then Helper.DeleteFile((Program.Pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927"))
            File.Move((Program.Pso2WinDir & "\backup\a44fbb2aeb8084c5a5fbe80e219a9927"), (Program.Pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927"))
        End If
        If File.Exists((Program.Pso2WinDir & "\backup\7f2368d207e104e8ed6086959b742c75")) Then
            If File.Exists((Program.Pso2WinDir & "\" & "7f2368d207e104e8ed6086959b742c75")) Then Helper.DeleteFile((Program.Pso2WinDir & "\" & "7f2368d207e104e8ed6086959b742c75"))
            File.Move((Program.Pso2WinDir & "\backup\7f2368d207e104e8ed6086959b742c75"), (Program.Pso2WinDir & "\" & "7f2368d207e104e8ed6086959b742c75"))
        End If
        If File.Exists((Program.Pso2WinDir & "\backup\009bfec69b04a34576012d50e3417771")) Then
            If File.Exists((Program.Pso2WinDir & "\" & "009bfec69b04a34576012d50e3417771")) Then Helper.DeleteFile((Program.Pso2WinDir & "\" & "009bfec69b04a34576012d50e3417771"))
            File.Move((Program.Pso2WinDir & "\backup\009bfec69b04a34576012d50e3417771"), (Program.Pso2WinDir & "\" & "009bfec69b04a34576012d50e3417771"))
        End If
        If File.Exists((Program.Pso2WinDir & "\backup\a93adc766eb3510f7b5c279551a45585")) Then
            If File.Exists((Program.Pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585")) Then Helper.DeleteFile((Program.Pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585"))
            File.Move((Program.Pso2WinDir & "\backup\a93adc766eb3510f7b5c279551a45585"), (Program.Pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585"))
        End If
        Helper.WriteDebugInfo(Resources.strCheckingFiles)
        For Each line In Helper.GetLines("patchlist.txt")
            If _cancelledFull Then Return
            filename = Regex.Split(line, ".pat")
            truefilename = filename(0).Replace("data/win32/", "")
            If truefilename <> "GameGuard.des" AndAlso truefilename <> "edition.txt" AndAlso truefilename <> "gameversion.ver" AndAlso truefilename <> "pso2.exe" AndAlso truefilename <> "PSO2JP.ini" AndAlso truefilename <> "script/user_default.pso2" AndAlso truefilename <> "script/user_intel.pso2" Then
                Dim length2 As Long
                If File.Exists(Program.Pso2WinDir & "\" & truefilename) Then length2 = New FileInfo(Program.Pso2WinDir & "\" & truefilename).Length
                If Not File.Exists((Program.Pso2WinDir & "\" & truefilename)) Then
                    Helper.WriteDebugInfo(truefilename & Resources.strIsMissing)
                    missingfiles.Add(truefilename)
                End If
                If File.Exists(Program.Pso2WinDir & "\" & truefilename) Then length2 = New FileInfo(Program.Pso2WinDir & "\" & truefilename).Length
                If Not File.Exists(Program.Pso2WinDir & "\" & truefilename) Then length2 = 1
                If length2 = 0 Then
                    Helper.WriteDebugInfo(truefilename & " has a filesize of 0!")
                    missingfiles.Add(truefilename)
                    Helper.DeleteFile(Program.Pso2WinDir & "\" & truefilename)
                End If
            End If
            numberofChecks += 1
            lblStatus.Text = (Resources.strCurrentlyCheckingFile & numberofChecks)
            Application.DoEvents()
        Next

        Helper.Log("Opening Second patch file...")
        For Each line In Helper.GetLines("patchlist_old.txt")
            If _cancelledFull Then Return
            filename2 = Regex.Split(line, ".pat")
            truefilename2 = filename2(0).Replace("data/win32/", "")
            If truefilename2 <> "GameGuard.des" AndAlso truefilename2 <> "edition.txt" AndAlso truefilename2 <> "gameversion.ver" AndAlso truefilename2 <> "pso2.exe" AndAlso truefilename2 <> "PSO2JP.ini" AndAlso truefilename2 <> "script/user_default.pso2" AndAlso truefilename2 <> "script/user_intel.pso2" Then
                Dim length2 As Long
                If File.Exists(Program.Pso2WinDir & "\" & truefilename2) Then length2 = New FileInfo(Program.Pso2WinDir & "\" & truefilename2).Length
                If Not File.Exists((Program.Pso2WinDir & "\" & truefilename2)) Then
                    If Not missingfiles.Contains(truefilename2) Then
                        Helper.WriteDebugInfo(truefilename2 & Resources.strIsMissing)
                        missingfiles2.Add(truefilename2)
                    End If
                End If
                If File.Exists(Program.Pso2WinDir & "\" & truefilename2) Then length2 = New FileInfo(Program.Pso2WinDir & "\" & truefilename2).Length
                If Not File.Exists(Program.Pso2WinDir & "\" & truefilename2) Then length2 = 1
                If length2 = 0 Then
                    Helper.WriteDebugInfo(truefilename2 & " has a filesize of 0!")
                    missingfiles.Add(truefilename2)
                    Helper.DeleteFile(Program.Pso2WinDir & "\" & truefilename2)
                End If
            End If
            numberofChecks += 1
            lblStatus.Text = (Resources.strCurrentlyCheckingFile & numberofChecks)
            Application.DoEvents()
        Next

        If missingfiles.Count = 0 AndAlso missingfiles2.Count = 0 Then
            Helper.WriteDebugInfoAndOk(Resources.strAllFilesCheckedOut)
            Return
        End If

        Dim result1 As DialogResult = MessageBox.Show(Resources.strWouldYouLikeToDownloadInstallMissing, "Download/Install?", MessageBoxButtons.YesNo)

        If result1 = DialogResult.Yes Then
            Helper.Log(Resources.strDownloading & Resources.strMissingFilesPart1)
            Dim totaldownload As Long = missingfiles.Count
            Dim downloaded As Long = 0
            Dim totaldownloaded As Long = 0
            Helper.DeleteFile("resume.txt")

            File.AppendAllLines("resume.txt", missingfiles)

            For Each downloadStr As String In missingfiles
                'Download the missing files:
                _cancelled = False
                downloaded += 1
                totaldownloaded += _totalsize2
                lblStatus.Text = Resources.strDownloading & "" & downloaded & "/" & totaldownload & " (" & Helper.SizeSuffix(totaldownloaded) & ")"

                DownloadFile(("http://download.pso2.jp/patch_prod/patches/data/win32/" & downloadStr & ".pat"), downloadStr)

                If New FileInfo(downloadStr).Length = 0 Then
                    Helper.Log("File appears to be empty, trying to download from secondary SEGA server")
                    DownloadFile(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & downloadStr & ".pat"), downloadStr)
                End If

                If _cancelled Then Return
                File.Move(downloadStr, (Program.Pso2WinDir & "\" & downloadStr))
                Helper.WriteDebugInfoAndOk((Resources.strDownloadedandInstalled & downloadStr & "."))
                Dim linesList As New List(Of String)(File.ReadAllLines("resume.txt"))

                'Remove the line to delete, e.g.
                linesList.Remove(downloadStr)

                File.WriteAllLines("resume.txt", linesList.ToArray())
                If _cancelledFull Then Return
            Next

            Helper.Log(Resources.strDownloading & Resources.strMissingFilesPart2)

            Helper.DeleteFile("resume.txt")

            File.AppendAllLines("resume.txt", missingfiles2)

            Dim totaldownload2 As Long = missingfiles2.Count
            Dim downloaded2 As Long = 0
            Dim totaldownloaded2 As Long = 0

            For Each downloadstring2 In missingfiles2
                If _cancelledFull Then Return
                'Download the missing files:
                If Not File.Exists((Program.Pso2WinDir & "\" & downloadstring2)) Then
                    _cancelled = False
                    downloaded2 += 1
                    totaldownloaded2 += _totalsize2
                    lblStatus.Text = Resources.strDownloading & "" & downloaded2 & "/" & totaldownload2 & " (" & Helper.SizeSuffix(totaldownloaded2) & ")"

                    DownloadFile(("http://download.pso2.jp/patch_prod/patches/data/win32/" & downloadstring2 & ".pat"), downloadstring2)

                    If New FileInfo(downloadstring2).Length = 0 Then
                        Helper.Log("File appears to be empty, trying to download from secondary SEGA server")
                        DownloadFile(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & downloadstring2 & ".pat"), downloadstring2)
                    End If

                    If _cancelled Then Return
                    File.Move(downloadstring2, (Program.Pso2WinDir & "\" & downloadstring2))
                    Helper.WriteDebugInfoAndOk((Resources.strDownloadedandInstalled & downloadstring2 & "."))
                    Dim linesList As New List(Of String)(File.ReadAllLines("resume.txt"))

                    'Remove the line to delete, e.g.
                    linesList.Remove(downloadstring2)
                    File.WriteAllLines("resume.txt", linesList.ToArray())
                End If
            Next
            Helper.WriteDebugInfoAndOk(Resources.strallDone)
        End If
    End Sub

    Private Async Sub ButtonItem10_Click(sender As Object, e As EventArgs) Handles ButtonItem10.Click
        ' Use IOC Container in the main Tweaker project to deal with dependencies.
        Dim output As New TweakerTrigger
        Dim Settings = New RegistryTweakerSettings("Software\AIDA")
        Dim updater = New UpdateManager(Settings, output)

        'await updater.CleanLegacyFiles();

        'Console.WriteLine(settings.GameDirectory)
        frmDownloader.CleanupUI()
        Await updater.Update(True, False)
    End Sub

    Private Sub btnGameguard_Click(sender As Object, e As EventArgs) Handles btnGameguard.Click
        Try
            Dim systempath As String
            If SkipDialogs = False Then MsgBox(Resources.strPleaseBeAwareGG)

            If Directory.Exists(Program.Pso2RootDir & "\Gameguard\") Then
                If SkipDialogs = False Then Helper.WriteDebugInfo("Removing Gameguard Directory...")
                If SkipDialogs = False Then Directory.Delete(Program.Pso2RootDir & "\Gameguard\", True)
                If SkipDialogs = False Then Helper.WriteDebugInfoSameLine(Resources.strDone)
            End If
            If File.Exists(Program.Pso2RootDir & "\GameGuard.des") Then
                If SkipDialogs = False Then Helper.WriteDebugInfo("Removing Gameguard File...")
                If SkipDialogs = False Then Helper.DeleteFile(Program.Pso2RootDir & "\GameGuard.des")
                If SkipDialogs = False Then Helper.WriteDebugInfoSameLine(Resources.strDone)
            End If
            If Environment.Is64BitOperatingSystem Then
                systempath = Environment.GetFolderPath(Environment.SpecialFolder.SystemX86)
                If File.Exists(systempath & "\npptnt2.sys") Then
                    If SkipDialogs = False Then Helper.WriteDebugInfo("Removing Hidden Gameguard Files (1 of 3)...")
                    If SkipDialogs = False Then Helper.DeleteFile(systempath & "\npptnt2.sys")
                    If SkipDialogs = False Then Helper.WriteDebugInfoSameLine(Resources.strDone)
                End If
                If File.Exists(systempath & "\nppt9x.vxd") Then
                    If SkipDialogs = False Then Helper.WriteDebugInfo("Removing Hidden Gameguard Files (2 of 3)...")
                    If SkipDialogs = False Then Helper.DeleteFile(systempath & "\nppt9x.vxd")
                    If SkipDialogs = False Then Helper.WriteDebugInfoSameLine(Resources.strDone)
                End If
                If File.Exists(systempath & "\GameMon.des") Then
                    If SkipDialogs = False Then Helper.WriteDebugInfo("Removing Hidden Gameguard Files (3 of 3)...")
                    If SkipDialogs = False Then Helper.DeleteFile(systempath & "\GameMon.des")
                    If SkipDialogs = False Then Helper.WriteDebugInfoSameLine(Resources.strDone)
                End If
            End If
            If Not Environment.Is64BitOperatingSystem Then
                systempath = Environment.GetFolderPath(Environment.SpecialFolder.System)
                If File.Exists(systempath & "\npptnt2.sys") Then
                    If SkipDialogs = False Then Helper.WriteDebugInfo("Removing Hidden Gameguard Files (1 of 3)...")
                    If SkipDialogs = False Then Helper.DeleteFile(systempath & "\npptnt2.sys")
                    If SkipDialogs = False Then Helper.WriteDebugInfoSameLine(Resources.strDone)
                End If
                If File.Exists(systempath & "\nppt9x.vxd") Then
                    If SkipDialogs = False Then Helper.WriteDebugInfo("Removing Hidden Gameguard Files (2 of 3)...")
                    If SkipDialogs = False Then Helper.DeleteFile(systempath & "\nppt9x.vxd")
                    If SkipDialogs = False Then Helper.WriteDebugInfoSameLine(Resources.strDone)
                End If
                If File.Exists(systempath & "\GameMon.des") Then
                    If SkipDialogs = False Then Helper.WriteDebugInfo("Removing Hidden Gameguard Files (3 of 3)...")
                    If SkipDialogs = False Then Helper.DeleteFile(systempath & "\GameMon.des")
                    If SkipDialogs = False Then Helper.WriteDebugInfoSameLine(Resources.strDone)
                End If
            End If
            If SkipDialogs = False Then Helper.WriteDebugInfo("Downloading Latest Gameguard file...")
            DownloadFile("http://download.pso2.jp/patch_prod/patches/GameGuard.des.pat", Program.Pso2RootDir & "\GameGuard.des")
            If SkipDialogs = False Then Helper.WriteDebugInfo("Downloading Latest Gameguard config...")
            DownloadFile("http://download.pso2.jp/patch_prod/patches/PSO2JP.ini.pat", Program.Pso2RootDir & "\PSO2JP.ini")
            If SkipDialogs = False Then Helper.WriteDebugInfoSameLine(Resources.strDone)
            'File.Move("GameGuard.des", Program.Pso2RootDir & "\GameGuard.des")

            Dim foundKey As RegistryKey = Computer.Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\Services\npggsvc", True)

            If foundKey Is Nothing Then
                If SkipDialogs = False Then Helper.WriteDebugInfo("No registry keys to delete. This is OK, they should be created the next time Gameguard launches.")
            Else
                If SkipDialogs = False Then Helper.WriteDebugInfo("Deleting Gameguard registry keys...")
                foundKey = Computer.Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\Services", True)
                foundKey.DeleteSubKeyTree("npggsvc")
                If SkipDialogs = False Then Helper.WriteDebugInfoSameLine(Resources.strDone)
            End If
            If SkipDialogs = False Then Helper.WriteDebugInfoAndOk(Resources.strGGReset)
            If SkipDialogs = True Then btnLaunchPSO2.PerformClick()
        Catch ex As Exception
            If SkipDialogs = False Then Helper.LogWithException(Resources.strERROR, ex)
            If ex.Message.Contains("Access to the ") Then MsgBox("It looks like Gameguard believes it's open, whether or not it actually is. You'll need to restart your computer to fix this problem. Sorry!")
        End Try
    End Sub

    Private Sub btnFixPSO2EXEs_Click(sender As Object, e As EventArgs) Handles btnFixPSO2EXEs.Click
        Try
            If IsPso2WinDirMissing() Then Return
            Dim directoryString As String = (Program.Pso2RootDir & "\")
            _cancelled = False
            Helper.WriteDebugInfo(Resources.strDownloading & "pso2launcher.exe...")

            Application.DoEvents()
            Dim procs As Process() = Process.GetProcessesByName("pso2launcher")

            For Each proc As Process In procs
                If proc.MainWindowTitle = "PHANTASY STAR ONLINE 2" AndAlso proc.MainModule.ToString() = "ProcessModule (pso2launcher.exe)" Then proc.Kill()
            Next

            DownloadFile("http://download.pso2.jp/patch_prod/patches/pso2launcher.exe.pat", "pso2launcher.exe")
            If _cancelled Then Return

            ' never mind I'll just rewrite it later or something idk
            If File.Exists((directoryString & "pso2launcher.exe")) AndAlso Program.StartPath <> Program.Pso2RootDir Then Helper.DeleteFile((directoryString & "pso2launcher.exe"))
            File.Move("pso2launcher.exe", (directoryString & "pso2launcher.exe"))
            Helper.WriteDebugInfoAndOk((Resources.strDownloadedandInstalled & "pso2launcher.exe"))
            Helper.WriteDebugInfo(Resources.strDownloading & "pso2updater.exe...")
            Application.DoEvents()
            procs = Process.GetProcessesByName("pso2updater")
            For Each proc As Process In procs
                If proc.MainModule.ToString() = "ProcessModule (pso2updater.exe)" Then proc.Kill()
            Next
            DownloadFile("http://download.pso2.jp/patch_prod/patches/pso2updater.exe.pat", "pso2updater.exe")
            If _cancelled Then Return

            If File.Exists((directoryString & "pso2updater.exe")) AndAlso Program.StartPath <> Program.Pso2RootDir Then Helper.DeleteFile((directoryString & "pso2updater.exe"))
            File.Move("pso2updater.exe", (directoryString & "pso2updater.exe"))
            Helper.WriteDebugInfoAndOk((Resources.strDownloadedandInstalled & "pso2updater.exe"))
            Helper.WriteDebugInfo(Resources.strDownloading & "pso2.exe...")
            Application.DoEvents()
            procs = Process.GetProcessesByName("pso2")
            For Each proc As Process In procs
                If proc.MainModule.ToString() = "ProcessModule (pso2.exe)" Then proc.Kill()
            Next
            DownloadFile("http://download.pso2.jp/patch_prod/patches/pso2.exe.pat", "pso2.exe")
            If _cancelled Then Return

            If File.Exists((directoryString & "pso2.exe")) AndAlso Program.StartPath <> Program.Pso2RootDir Then Helper.DeleteFile((directoryString & "pso2.exe"))
            File.Move("pso2.exe", (directoryString & "pso2.exe"))
            Helper.WriteDebugInfoAndOk((Resources.strDownloadedandInstalled & "pso2.exe"))
            Application.DoEvents()
            Helper.WriteDebugInfo(Resources.strAllNecessaryFiles)
            UnlockGui()
        Catch ex As Exception
            Helper.LogWithException(Resources.strERROR, ex)
        End Try
    End Sub

    Private Sub btnRestoreStoryBackup_Click(sender As Object, e As EventArgs) Handles btnRestoreStoryBackup.Click
        Try
            If IsPso2WinDirMissing() Then Return
            If MsgBox(Resources.strAreYouSureRestoreBackup, vbYesNo) = MsgBoxResult.Yes Then
                RestoreBackup(StoryPatch)
            End If
        Catch ex As Exception
            Helper.LogWithException(Resources.strERROR, ex)
        End Try
    End Sub

    Private Shared Sub btnFixPermissions_Click(sender As Object, e As EventArgs) Handles btnFixPermissions.Click
        Try
            'SystemInformation.UserName
            'cacls.exe "C:\PHANTASYSTARONLINE2\pso2_bin\pso2.exe" /e /g "AIDA":F
            'cacls.exe "C:\PHANTASYSTARONLINE2\pso2_bin\pso2launcher.exe" /e /g "AIDA":F
            'cacls.exe "C:\PHANTASYSTARONLINE2\pso2_bin\pso2download.exe" /e /g "AIDA":F
            'cacls.exe "C:\PHANTASYSTARONLINE2\pso2_bin\pso2updater.exe" /e /g "AIDA":F
            'cacls.exe "C:\PHANTASYSTARONLINE2\pso2_bin\pso2predownload.exe" /e /g "AIDA":F
            MsgBox(Resources.strFixPermissionIssuesText)
            Dim directoryString As String = (Program.Pso2RootDir & "\")
            Helper.WriteDebugInfo(Resources.strFixingPermissionsFor & "pso2.exe...")
            Application.DoEvents()
            Process.Start("cacls.exe", (directoryString & "pso2.exe") & " /e /g """ & SystemInformation.UserName & """:F")
            Helper.WriteDebugInfoSameLine(Resources.strDone)
            Helper.WriteDebugInfo(Resources.strFixingPermissionsFor & "pso2launcher.exe...")
            Application.DoEvents()
            Process.Start("cacls.exe", (directoryString & "pso2launcher.exe") & " /e /g """ & SystemInformation.UserName & """:F")
            Helper.WriteDebugInfoSameLine(Resources.strDone)
            Helper.WriteDebugInfo(Resources.strFixingPermissionsFor & "pso2download.exe...")
            Application.DoEvents()
            Process.Start("cacls.exe", (directoryString & "pso2download.exe") & " /e /g """ & SystemInformation.UserName & """:F")
            Helper.WriteDebugInfoSameLine(Resources.strDone)
            Helper.WriteDebugInfo(Resources.strFixingPermissionsFor & "pso2updater.exe...")
            Application.DoEvents()
            Process.Start("cacls.exe", (directoryString & "pso2updater.exe") & " /e /g """ & SystemInformation.UserName & """:F")
            Helper.WriteDebugInfoSameLine(Resources.strDone)
            Helper.WriteDebugInfo(Resources.strFixingPermissionsFor & "pso2predownload.exe...")
            Application.DoEvents()
            Process.Start("cacls.exe", (directoryString & "pso2predownload.exe") & " /e /g """ & SystemInformation.UserName & """:F")
            Helper.WriteDebugInfoSameLine(Resources.strDone)
            MsgBox(Resources.strFixPermissionsDone)
        Catch ex As Exception
            Helper.LogWithException(Resources.strERROR, ex)
        End Try
    End Sub

    Private Sub ButtonItem17_Click(sender As Object, e As EventArgs) Handles ButtonItem17.Click
        If MsgBox(Resources.strAreYouSureResetPSO2Settings, MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
            File.WriteAllText((_myDocuments & "\" & "SEGA\PHANTASYSTARONLINE2\user.pso2"), txtPSO2DefaultINI.Text)
            Helper.WriteDebugInfoAndOk(Resources.strPSO2SettingsReset)
        End If
    End Sub

    Private Shared Sub btnTerminate_Click(sender As Object, e As EventArgs) Handles btnTerminate.Click
        Helper.WriteDebugInfo(Resources.strTerminatePSO2)
        Dim procs As Process() = Process.GetProcessesByName("pso2")
        For Each proc As Process In procs
            If proc.MainModule.ToString() = "ProcessModule (pso2.exe)" Then proc.Kill()
            If proc.MainModule.ToString() = "ProcessModule (GameMon.des)" Then proc.Kill()
            If proc.MainModule.ToString() = "ProcessModule (GameMon64.des)" Then proc.Kill()
        Next
        Helper.WriteDebugInfoSameLine(Resources.strDone)
    End Sub

    Private Shared Sub btnBumped_Click(sender As Object, e As EventArgs) Handles btnBumped.Click
        Process.Start("http://bumped.org/psublog/")
    End Sub

    Private Shared Sub btnCirno_Click(sender As Object, e As EventArgs) Handles btnCirno.Click
        Process.Start("http://pso2.cirnopedia.info/")
    End Sub

    Private Shared Sub btnArksCash_Click(sender As Object, e As EventArgs) Handles btnArksCash.Click
        Process.Start("http://www.pso-world.com/forums/showthread.php?t=199490")
    End Sub

    Private Shared Sub btnErrors_Click(sender As Object, e As EventArgs) Handles btnErrors.Click
        Process.Start("http://www.pso-world.com/forums/showthread.php?t=204836")
    End Sub

    Private Shared Sub btnOfficialPSO2JP_Click(sender As Object, e As EventArgs) Handles btnOfficialPSO2JP.Click
        Process.Start("http://pso2.jp")
    End Sub

    Private Shared Sub btnRegistration_Click(sender As Object, e As EventArgs) Handles btnRegistration.Click
        Process.Start("http://www.pso-world.com/forums/showthread.php?t=210236")
    End Sub

    Private Shared Sub btnTweaker_Click(sender As Object, e As EventArgs) Handles btnTweaker.Click
        Process.Start("http://www.pso-world.com/forums/showthread.php?t=207248")
    End Sub

    Private Sub btnCheckForStoryUpdates_Click(sender As Object, e As EventArgs) Handles btnCheckForStoryUpdates.Click
        CheckForStoryUpdates()
    End Sub

    Private Sub chkAlwaysOnTop_Click(sender As Object, e As EventArgs) Handles chkAlwaysOnTop.Click
        If Visible Then
            If chkAlwaysOnTop.Checked Then
                _optionsFrm.TopMost = True
                TopMost = True
                RegKey.SetValue(Of Boolean)(RegKey.AlwaysOnTop, True)
            Else
                _optionsFrm.TopMost = False
                TopMost = False
                RegKey.SetValue(Of Boolean)(RegKey.AlwaysOnTop, False)
            End If
        End If
    End Sub

    Private Sub btnPSO2Options_Click(sender As Object, e As EventArgs) Handles btnPSO2Options.Click
        Cursor = Cursors.WaitCursor
        Try
            _pso2OptionsFrm.TopMost = TopMost
            _pso2OptionsFrm.Top += 50
            _pso2OptionsFrm.Left += 50
            _pso2OptionsFrm.ShowDialog()
        Catch ex As Exception
            Helper.LogWithException(Resources.strERROR, ex)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub btnOptions_Click(sender As Object, e As EventArgs) Handles btnOptions.Click
        Cursor = Cursors.WaitCursor
        Try
            _optionsFrm.CMBStyle.SelectedIndex = -1
            _optionsFrm.TopMost = TopMost
            _optionsFrm.Bounds = Bounds
            _optionsFrm.Top += 50
            _optionsFrm.Left += 50
            _optionsFrm.ShowDialog()
        Catch ex As Exception
            Helper.LogWithException(Resources.strERROR, ex)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub btnENPatch_Click(sender As Object, e As EventArgs) Handles btnENPatch.Click
        DownloadEnglishPatch()
    End Sub

    Private Sub DownloadEnglishPatch()
        ' Here we parse the text file before passing it to the DownloadPatch function.
        ' The Using statement will dispose "net" as soon as we're done with it.
        ' If we decide not to, we can do away with "url" and just pass net.DownloadString in as the parameter.
        ' Furthermore, we could also parse it from within the function.
        DLS.Headers("user-agent") = GetUserAgent()
        Dim url As String = DLS.DownloadString(Program.FreedomUrl & "patches/enpatch.txt")
        DownloadPatch(url, EnglishPatch, "ENPatch.rar", RegKey.EnPatchVersion, Resources.strBackupEN, Resources.strPleaseSelectPreDownloadENRAR)
    End Sub

    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        Close()
    End Sub

    Private Shared Sub btnEXPFULL_Click(sender As Object, e As EventArgs) Handles btnEXPFULL.Click
        Process.Start("http://www.expfull.com/chat")
    End Sub

    Private Sub OpenSideBar()
        Helper.WriteDebugInfo(Resources.strLoadingSidebar)
        ThreadPool.QueueUserWorkItem(AddressOf LoadSidebar, Nothing)

        If _dpiSetting = 96 Then Width = 796
        If _dpiSetting = 120 Then Width = 1060
        btnAnnouncements.Text = "<"

        Program.SidebarEnabled = True
        RegKey.SetValue(Of Boolean)(RegKey.SidebarEnabled, True)
    End Sub

    Private Sub ToggleSideBar()
        If _dpiSetting = 96 Then
            If Width = 420 Then
                Width = 796
            ElseIf Width = 796 Then
                Width = 420
            End If
        ElseIf _dpiSetting = 120 Then
            If Width = 560 Then
                Width = 1060
            ElseIf Width = 1060 Then
                Width = 560
            End If
        End If

        Program.SidebarEnabled = (Not Program.SidebarEnabled)
        RegKey.SetValue(Of Boolean)(RegKey.SidebarEnabled, Program.SidebarEnabled)

        If Program.SidebarEnabled Then
            ThreadPool.QueueUserWorkItem(AddressOf LoadSidebar, Nothing)
            Helper.WriteDebugInfo(Resources.strLoadingSidebarPage)
            rtbDebug.Refresh()
            btnAnnouncements.Text = "<"
        Else
            btnAnnouncements.Text = ">"
        End If

    End Sub

    Private Sub btnAnnouncements_Click(sender As Object, e As EventArgs) Handles btnAnnouncements.Click
        ToggleSideBar()
    End Sub

    Private Sub WebBrowser4_Navigating(sender As Object, e As WebBrowserNavigatingEventArgs) Handles WebBrowser4.Navigating
        If Visible Then
            'Change to Tweaker2 to test sidebar theming. - AIDA
            If e.Url.ToString() <> Program.FreedomUrl & "tweaker.php" Then
                Process.Start(e.Url.ToString())
                Helper.Log("Trying to load URL for sidebar: " & e.Url.ToString)
                ThreadPool.QueueUserWorkItem(AddressOf LoadSidebar, Nothing)
            End If
        End If
    End Sub

    Private Sub btnUninstallENPatch_Click(sender As Object, e As EventArgs) Handles btnUninstallENPatch.Click
        UninstallPatch(Program.FreedomUrl & "patches/enpatchfilelist.txt", "enpatchfilelist.txt", EnglishPatch, Resources.strENPatchUninstalled, RegKey.EnPatchVersion)
    End Sub

    Private Sub btnUninstallLargeFiles_Click(sender As Object, e As EventArgs) Handles btnUninstallLargeFiles.Click
        UninstallPatch(Program.FreedomUrl & "patches/largefilelist.txt", "largefilelist.txt", LargeFiles, Resources.strLFUninstalled, RegKey.LargeFilesVersion)
    End Sub

    Private Sub btnUninstallStory_Click(sender As Object, e As EventArgs) Handles btnUninstallStory.Click
        UninstallPatch(Program.FreedomUrl & "patches/storyfilelist.txt", "storyfilelist.txt", StoryPatch, Resources.strStoryPatchUninstalled, RegKey.StoryPatchVersion)
    End Sub

    Private Sub btnRussianPatch_Click(sender As Object, e As EventArgs) Handles btnRussianPatch.Click
        DLS.Headers("user-agent") = GetUserAgent()
        Dim url As String = DLS.DownloadString(Program.FreedomUrl & "patches/rupatch.txt")
        DownloadPatch(url, RussianPatch, "RUPatch.zip", RegKey.NullKey, "Would you like to backup your files before applying the patch? This will erase all previous Russian Patch backups." & vbCrLf & "Хотите ли вы сделать резервную копию ваших файлов перед установкой патча? Это приведёт к удалению предыдущих резервных копий русского патча.", "Please select the pre-downloaded Russian Patch ZIP file." & vbCrLf & "Пожалуйста, выберите предварительно-загруженный русский zip патч файл.")
    End Sub

    Private Sub tsmRestartDownload_Click(sender As Object, e As EventArgs) Handles tsmRestartDownload.Click
        _restartplz = True
    End Sub

    Private Shared Sub ButtonItem7_Click(sender As Object, e As EventArgs) Handles ButtonItem7.Click
        Const processName As String = "chrome"
        Dim processes = Process.GetProcessesByName("chrome")
        Dim currentProcess As Process = Process.GetCurrentProcess()
        If processes.Length > 0 Then
            Dim closeItYesNo As MsgBoxResult = MsgBox("You need to have all Chrome windows closed before launching in this mode. Would you like to close all open Chrome windows now?", vbYesNo)
            If closeItYesNo = vbYes Then
                For Each proc As Process In Process.GetProcessesByName(processName)
                    If proc.Id <> currentProcess.Id Then proc.Kill()
                Next
            Else
                Helper.WriteDebugInfoAndWarning("You need to have all Chrome windows closed before launching in this mode. Please close all Chrome windows and try again.")
                Return
            End If
        End If
        MsgBox(Resources.strPleaseBeAwareChrome)
        Process.Start("chrome", "--no-sandbox")
    End Sub

    Private Sub btnClearSACache_Click(sender As Object, e As EventArgs) Handles btnClearSACache.Click
        Dim clearYesNo As MsgBoxResult = MsgBox("This will clear all Symbol Arts from your ""History"" tab. Having 100 pages of Symbol Arts to load can sometimes cause slowdown.", vbYesNo)
        If clearYesNo = vbYes Then
            Helper.WriteDebugInfo("Deleting Symbol Art Cache...")
            For Each foundFile As String In Computer.FileSystem.GetFiles((_myDocuments & "\" & "SEGA\PHANTASYSTARONLINE2\symbolarts\cache"), FileIO.SearchOption.SearchAllSubDirectories, "*.*")
                Computer.FileSystem.DeleteFile(foundFile, UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently)
            Next
            Helper.WriteDebugInfoSameLine(Resources.strDone)
        End If
    End Sub

    Private Sub btnInstallPSO2_Click(sender As Object, e As EventArgs) Handles btnInstallPSO2.Click
        InstallPso2()
    End Sub

    Private Async Sub InstallPso2()
        Dim installFolder As String = ""
        'Const installYesNo As MsgBoxResult = vbYes
        'If installYesNo = vbNo Then
        '    WriteDebugInfo("You can view more information about the installer at:" & vbCrLf & "http://arks-layer.com/setup.php")
        '    Return
        'End If
        'If installYesNo = vbYes Then
        MsgBox("This will install Phantasy Star Online EPISODE 4! Please select a folder to install into." & vbCrLf & "A folder called PHANTASYSTARONLINE2 will be created inside the folder you choose." & vbCrLf & "(For example, if you choose the C drive, it will install to C:\PHANTASYSTARONLINE2\)" & vbCrLf & "It is HIGHLY RECOMMENDED that you do NOT install into the Program Files folder, but a normal folder like C:\PHANTASYSTARONLINE\")
        Dim installPso2 As Boolean = True
        While installPso2
            Dim myFolderBrowser As New FolderBrowserDialog With {.RootFolder = Environment.SpecialFolder.MyComputer, .Description = "Please select a folder (or drive) to install PSO2 into"}
            Dim dlgResult As DialogResult = myFolderBrowser.ShowDialog()

            If dlgResult = DialogResult.OK Then
                installFolder = myFolderBrowser.SelectedPath
            End If
            If dlgResult = DialogResult.Cancel Then
                Helper.WriteDebugInfo("Installation cancelled by user!")
                Return
            End If
            Dim correctYesNo As MsgBoxResult = MsgBox("You wish to install PSO2 into " & (installFolder & "\PHANTASYSTARONLINE2\. Is this correct?").Replace("\\", "\"), vbYesNoCancel)
            If correctYesNo = vbCancel Then
                Helper.WriteDebugInfo("Installation cancelled by user!")
                Return
            End If
            If correctYesNo = vbNo Then
                Continue While
            End If
            If correctYesNo = vbYes Then
                For Each drive In DriveInfo.GetDrives()
                    If (drive.DriveType = DriveType.Fixed) AndAlso (installFolder(0) = drive.Name(0)) Then
                        If drive.TotalSize < 42000000000 Then
                            MsgBox("There is not enough space on the selected disk to install PSO2. Please select a different drive. (Requires 41GB of free space)")
                            Continue While
                        End If
                        If drive.AvailableFreeSpace < 42000000000 Then
                            MsgBox("There is not enough free space on the selected disk to install PSO2. Please free up some space or select a different drive. (Requires 41GB of free space)")
                            Continue While
                        End If
                    End If
                Next

                Dim finalYesNo As MsgBoxResult = MsgBox("The program will now install the necessary files, create the folders, and set up the game. Afterwards, the program will automatically begin patching. Click ""OK"" to start.", MsgBoxStyle.OkCancel)
                If finalYesNo = vbCancel Then
                    Helper.WriteDebugInfo("Installation cancelled by user!")
                Else
                    Office2007StartButton1.Enabled = False
                    'set the pso2Dir to the install patch
                    Program.Pso2RootDir = (installFolder & "\PHANTASYSTARONLINE2\pso2_bin").Replace("\\", "\")
                    lblDirectory.Text = Program.Pso2RootDir
                    Show()
                    Focus()
                    Application.DoEvents()
                    Helper.WriteDebugInfo("Downloading DirectX setup...")
                    Try
                        DownloadFile("http://arks-layer.com/docs/dxwebsetup.exe", "dxwebsetup.exe")
                        Helper.WriteDebugInfoSameLine("Done!")
                        Helper.WriteDebugInfo("Checking/Installing DirectX...")
                        Dim processStartInfo As ProcessStartInfo = New ProcessStartInfo() With {.FileName = "dxwebsetup.exe", .Verb = "runas", .Arguments = "/Q", .UseShellExecute = True}
                        Process.Start(processStartInfo).WaitForExit()
                        Helper.WriteDebugInfoSameLine("Done!")
                    Catch ex As Exception
                        Helper.WriteDebugInfo("DirectX installation failed! Please install it later if neccessary!")
                    End Try

                    If File.Exists("dxwebsetup.exe") Then File.Delete("dxwebsetup.exe")
                    'Make a data folder, and a win32 folder under that
                    Directory.CreateDirectory(Program.Pso2RootDir & "\data\win32\")
                    'Download required pso2 stuff
                    Helper.WriteDebugInfo("Downloading PSO2 required files...")
                    DownloadFile("http://download.pso2.jp/patch_prod/patches/pso2launcher.exe.pat", Program.Pso2RootDir & "\pso2launcher.exe")
                    DownloadFile("http://download.pso2.jp/patch_prod/patches/pso2updater.exe.pat", Program.Pso2RootDir & "\pso2updater.exe")
                    DownloadFile("http://download.pso2.jp/patch_prod/patches/pso2.exe.pat", Program.Pso2RootDir & "\pso2.exe")
                    DownloadFile("http://download.pso2.jp/patch_prod/patches/PSO2JP.ini.pat", Program.Pso2RootDir & "\PSO2JP.ini")
                    Helper.WriteDebugInfoSameLine("Done!")
                    'Download Gameguard.des
                    Helper.WriteDebugInfo("Downloading Latest Gameguard file...")
                    DownloadFile("http://download.pso2.jp/patch_prod/patches/GameGuard.des.pat", Program.Pso2RootDir & "\GameGuard.des")
                    Helper.WriteDebugInfoSameLine(Resources.strDone)
                    Application.DoEvents()

                    RegKey.SetValue(Of String)(RegKey.Pso2Dir, Program.Pso2RootDir)
                    Helper.WriteDebugInfo(Program.Pso2RootDir & " " & Resources.strSetAsYourPSO2)
                    Program.Pso2WinDir = (Program.Pso2RootDir & "\data\win32")
                    If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.StoryPatchVersion)) Then RegKey.SetValue(Of String)(RegKey.StoryPatchVersion, "Not Installed")
                    If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.EnPatchVersion)) Then RegKey.SetValue(Of String)(RegKey.EnPatchVersion, "Not Installed")
                    If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.LargeFilesVersion)) Then RegKey.SetValue(Of String)(RegKey.LargeFilesVersion, "Not Installed")

                    LockGui()

                    ' Use IOC Container in the main Tweaker project to deal with dependencies.
                    Dim output As New TweakerTrigger
                    Dim Settings = New RegistryTweakerSettings("Software\AIDA")
                    Dim updater = New UpdateManager(Settings, output)

                    'await updater.CleanLegacyFiles();

                    'Console.WriteLine(settings.GameDirectory)
                    Try

                        'frmDownloader.TopMost = True
                        'Me.TopMost = False
                    Catch ex As Exception
                        Helper.LogWithException(Resources.strERROR, ex)
                    End Try
                    frmDownloader.CleanupUI()
                    Await updater.Update(False, False)

                    Helper.WriteDebugInfo("PSO2 successfully installed! It is now being updated, and will be ready to play once it's finished.")
                    Refresh()
                End If
            End If

            installPso2 = False
            Program.IsPso2Installed = True
        End While
        'End If
    End Sub

    Private Shared Sub btnSymbolEditor_Click(sender As Object, e As EventArgs) Handles btnSymbolEditor.Click
        Process.Start("http://www.pso-world.com/forums/showthread.php?t=215777")
    End Sub

    Private Shared Sub btnRunPSO2Linux_Click(sender As Object, e As EventArgs) Handles btnRunPSO2Linux.Click
        Process.Start("http://www.pso-world.com/forums/showthread.php?t=215642")
    End Sub

    Private Sub LoadSidebar(state As Object)
        Try
            'Change to Tweaker2 to test Sidebar theming.
            WebBrowser4.Navigate(Program.FreedomUrl & "tweaker.php")
        Catch ex As Exception
            Helper.LogWithException("Web Browser failed: ", ex)
        End Try
    End Sub

    Private Shared Sub btnDonateToTweaker_Click(sender As Object, e As EventArgs) Handles btnDonateToTweaker.Click
        Process.Start("http://arks-layer.com/donate.php")
    End Sub

    Private Shared Sub btnDonateToBumped_Click(sender As Object, e As EventArgs) Handles btnDonateToBumped.Click
        Process.Start("http://bumped.org/psublog/about/")
    End Sub

    Private Shared Sub btnDonateToCirno_Click(sender As Object, e As EventArgs) Handles btnDonateToCirno.Click
        Process.Start("http://pso2.cirnopedia.info/support.php")
    End Sub

    Private Shared Sub btnResetTweaker_Click(sender As Object, e As EventArgs) Handles btnResetTweaker.Click
        Dim resetyesno As MsgBoxResult = MsgBox("This will erase all of the PSO2 Tweaker's settings, and restart the program. Continue?", vbYesNo)
        If resetyesno = vbYes Then
            Computer.Registry.CurrentUser.DeleteSubKeyTree("Software\AIDA", False)
            Helper.Log("All settings reset, restarting program!")
            Windows.Forms.Application.Restart()
        End If
    End Sub


    Private Sub btnDownloadPrepatch_Click(sender As Object, e As EventArgs) Handles btnDownloadPrepatch.Click
        CheckForPso2Updates(True)
    End Sub

    Private Sub btnCopyInfo_Click_1(sender As Object, e As EventArgs) Handles btnCopyInfo.Click
        Try
            FrmDiagnostic.TopMost = TopMost
            FrmDiagnostic.Top += 50
            FrmDiagnostic.Left += 50
            FrmDiagnostic.ShowDialog()
        Catch ex As Exception
            Helper.LogWithException(Resources.strERROR, ex)
        End Try
    End Sub

    Private Sub btnChooseProxyServer_Click(sender As Object, e As EventArgs) Handles btnChooseProxyServer.Click
        Try
            'JSON should look like { "version": 1, "host": "0.0.0.0", "name": "Super cool proxy", "publickeyurl": "http://url.com" }

            Dim jsonurl As String = InputBox("Please input the URL of the configuration JSON:", "Configuration JSON", "")
            If String.IsNullOrEmpty(jsonurl) Then Return

            Helper.WriteDebugInfo("Downloading configuration...")
            DLS.DownloadFile(jsonurl, "ServerConfig.txt")

            ' TODO: Deserialize directly from a string instead of saving to a file.
            Dim proxyInfo As Pso2ProxyInfo
            Using stream As FileStream = File.Open("ServerConfig.txt", FileMode.Open)
                Dim serializer As DataContractJsonSerializer = New DataContractJsonSerializer(GetType(Pso2ProxyInfo))
                proxyInfo = DirectCast(serializer.ReadObject(stream), Pso2ProxyInfo)
            End Using

            If File.Exists("ServerConfig.txt") Then File.Delete("ServerConfig.txt")

            If Convert.ToInt32(proxyInfo.Version) <> 1 Then
                MsgBox("ERROR - Version is incorrect! Please recheck the JSON.")
                Return
            End If

            For index As Integer = 0 To (proxyInfo.Host.Length - 1)
                If Char.IsLetter(proxyInfo.Host(index)) Then
                    Dim ips = Dns.GetHostAddresses(proxyInfo.Host)
                    proxyInfo.Host = ips(0).ToString()
                    Exit For
                End If
            Next

            Helper.WriteDebugInfoSameLine(" Done!")

            Dim builtFile As New List(Of String)
            Dim alreadyModified As Boolean = False
            If Not File.Exists(Program.HostsFilePath) Then File.Create(Program.HostsFilePath).Dispose()

            For Each line In Helper.GetLines(Program.HostsFilePath)
                Dim splitLine = line.Split(" "c)

                If splitLine.Length > 1 Then
                    Select Case (splitLine(1))
                        Case "gs001.pso2gs.net"
                            line = proxyInfo.Host & " gs001.pso2gs.net #" & proxyInfo.Name & " Ship 01"
                            alreadyModified = True
                        Case "gs016.pso2gs.net"
                            line = proxyInfo.Host & " gs016.pso2gs.net #" & proxyInfo.Name & " Ship 02"
                            alreadyModified = True
                        Case "gs031.pso2gs.net"
                            line = proxyInfo.Host & " gs031.pso2gs.net #" & proxyInfo.Name & " Ship 03"
                            alreadyModified = True
                        Case "gs046.pso2gs.net"
                            line = proxyInfo.Host & " gs046.pso2gs.net #" & proxyInfo.Name & " Ship 04"
                            alreadyModified = True
                        Case "gs061.pso2gs.net"
                            line = proxyInfo.Host & " gs061.pso2gs.net #" & proxyInfo.Name & " Ship 05"
                            alreadyModified = True
                        Case "gs076.pso2gs.net"
                            line = proxyInfo.Host & " gs076.pso2gs.net #" & proxyInfo.Name & " Ship 06"
                            alreadyModified = True
                        Case "gs091.pso2gs.net"
                            line = proxyInfo.Host & " gs091.pso2gs.net #" & proxyInfo.Name & " Ship 07"
                            alreadyModified = True
                        Case "gs106.pso2gs.net"
                            line = proxyInfo.Host & " gs106.pso2gs.net #" & proxyInfo.Name & " Ship 08"
                            alreadyModified = True
                        Case "gs121.pso2gs.net"
                            line = proxyInfo.Host & " gs121.pso2gs.net #" & proxyInfo.Name & " Ship 09"
                            alreadyModified = True
                        Case "gs136.pso2gs.net"
                            line = proxyInfo.Host & " gs136.pso2gs.net #" & proxyInfo.Name & " Ship 10"
                            alreadyModified = True
                    End Select
                End If

                builtFile.Add(line)
            Next

            If alreadyModified Then
                Helper.WriteDebugInfo("Modifying HOSTS file...")
            Else
                builtFile.Add(proxyInfo.Host & " gs001.pso2gs.net #" & proxyInfo.Name & " Ship 01")
                builtFile.Add(proxyInfo.Host & " gs016.pso2gs.net #" & proxyInfo.Name & " Ship 02")
                builtFile.Add(proxyInfo.Host & " gs031.pso2gs.net #" & proxyInfo.Name & " Ship 03")
                builtFile.Add(proxyInfo.Host & " gs046.pso2gs.net #" & proxyInfo.Name & " Ship 04")
                builtFile.Add(proxyInfo.Host & " gs061.pso2gs.net #" & proxyInfo.Name & " Ship 05")
                builtFile.Add(proxyInfo.Host & " gs076.pso2gs.net #" & proxyInfo.Name & " Ship 06")
                builtFile.Add(proxyInfo.Host & " gs091.pso2gs.net #" & proxyInfo.Name & " Ship 07")
                builtFile.Add(proxyInfo.Host & " gs106.pso2gs.net #" & proxyInfo.Name & " Ship 08")
                builtFile.Add(proxyInfo.Host & " gs121.pso2gs.net #" & proxyInfo.Name & " Ship 09")
                builtFile.Add(proxyInfo.Host & " gs136.pso2gs.net #" & proxyInfo.Name & " Ship 10")
                Helper.WriteDebugInfo("Previous modifications not found, creating new entries...")
            End If

            File.WriteAllLines(Program.HostsFilePath, builtFile.ToArray())
            Helper.WriteDebugInfoSameLine(" Done!")

            Helper.WriteDebugInfo("Testing connection...")
            Dim gameHost As IPHostEntry = Dns.GetHostEntry("gs001.pso2gs.net")
            ' Although this is already an IP address in the case of the public proxy, there is the potnetial
            ' that it could be a hostname. I could be wrong, but better safe than sorry!
            Dim proxyHost As IPHostEntry = Dns.GetHostEntry(proxyInfo.Host)

            Dim connectSuccess As Boolean = False
            For Each address As IPAddress In gameHost.AddressList
                If proxyHost.AddressList.Contains(address) Then
                    connectSuccess = True
                    Exit For
                End If
            Next

            If Not connectSuccess Then
                Helper.WriteDebugInfoAndFailed("Connection test failed!")
                MessageBox.Show("Failed to connect to the right server. This could mean your hosts file was not properly modified. Disable your anti-virus software and try again.",
                                "Connection test failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            Helper.WriteDebugInfoAndOk("Connection success!")
            Helper.WriteDebugInfo("Downloading and installing publickey.blob...")
            DLS.DownloadFile(proxyInfo.PublicKeyUrl, Program.StartPath & "\publickey.blob")
            If File.Exists(Program.Pso2RootDir & "\publickey.blob") AndAlso Program.StartPath <> Program.Pso2RootDir Then Helper.DeleteFile(Program.Pso2RootDir & "\publickey.blob")
            If Program.StartPath <> Program.Pso2RootDir Then File.Move(Program.StartPath & "\publickey.blob", Program.Pso2RootDir & "\publickey.blob")
            Helper.WriteDebugInfoSameLine(" Done!")
            CheckForPluginUpdates()
            If File.Exists(Program.Pso2RootDir & "\plugins\disabled\PSO2Proxy.dll") Then
                Helper.WriteDebugInfo("Auto-enabling the PSO2Proxy plugin...")
                File.Move(Program.Pso2RootDir & "\plugins\disabled\PSO2Proxy.dll", Path.Combine(Program.Pso2RootDir & "\plugins\PSO2Proxy.dll"))
                Helper.WriteDebugInfoSameLine("Done!")
            End If
            Helper.WriteDebugInfo("All done! You should now be able to connect to " & proxyInfo.Name & ".")
            RegKey.SetValue(Of Boolean)(RegKey.ProxyEnabled, True)
        Catch ex As Exception
            Helper.WriteDebugInfoAndFailed(Helper.ExceptionDump("ERROR - ", ex))
            Dim exceptionType As Type = ex.GetType()
            If exceptionType = GetType(SecurityException) OrElse exceptionType = GetType(UnauthorizedAccessException) Then
                MsgBox("It seems you've gotten an error while trying to patch your HOSTS file. Please go to the " & Environment.SystemDirectory & "\drivers\etc\ folder, right click on the hosts file, and make sure ""Read Only"" is not checked. Then try again." & vbNewLine & "When you click Okay, the Tweaker will also generate a pastebin of your HOSTS file and what is locking it. Look at the bottom of the pastebin where the 'HOSTS Handle stuff' is.")
                FrmDiagnostic.Button2.PerformClick()
            End If
        End Try
    End Sub

    Private Shared Sub btnRevertPSO2ProxyToJP_Click(sender As Object, e As EventArgs) Handles btnRevertPSO2ProxyToJP.Click
        '[Revert]
        Dim builtFile = New List(Of String)
        If Not File.Exists(Program.HostsFilePath) Then File.Create(Program.HostsFilePath).Dispose()

        For Each line In Helper.GetLines(Program.HostsFilePath)
            If Not line.Contains("pso2gs.net") Then builtFile.Add(line)
        Next

        Helper.WriteDebugInfo("Modifying HOSTS file...")
        File.WriteAllLines(Program.HostsFilePath, builtFile.ToArray())
        Helper.WriteDebugInfoSameLine(" Done!")
        Helper.DeleteFile(Program.Pso2RootDir & "\publickey.blob")
        If File.Exists(Program.Pso2RootDir & "\plugins\PSO2Proxy.dll") Then
            Helper.WriteDebugInfo("Auto-disabling the PSO2Proxy plugin...")
            File.Move(Program.Pso2RootDir & "\plugins\PSO2Proxy.dll", Path.Combine(Program.Pso2RootDir & "\plugins\disabled\PSO2Proxy.dll"))
            Helper.WriteDebugInfoSameLine("Done!")
        End If
        Helper.WriteDebugInfoAndOk("All normal JP connection settings restored!")
        RegKey.SetValue(Of Boolean)(RegKey.ProxyEnabled, False)
    End Sub

    Private Sub btnStoryPatchNew_Click(sender As Object, e As EventArgs) Handles btnStoryPatchNew.Click
        InstallStoryPatchNew()
    End Sub

    Private Sub InstallStoryPatchNew()
        'Don't forget to make GUI changes!
        Try
            If IsPso2WinDirMissing() Then Return

            ' Create a match using regular exp<b></b>ressions
            ' Spit out the value plucked from the code
            DLS.Headers("user-agent") = GetUserAgent()
            txtHTML.Text = Regex.Match(DLS.DownloadString("http://arks-layer.com/story.php"), "<u>.*?</u>").Value

            Dim backupdir As String = BuildBackupPath(StoryPatch)
            Dim strStoryPatchLatestBase As String = txtHTML.Text.Replace("<u>", "").Replace("</u>", "").Replace("/", "-")
            Helper.WriteDebugInfoAndOk("Downloading story patch info... ")
            DownloadFile(Program.FreedomUrl & "pso2.stripped.db.7z", "pso2.stripped.db.7z")
            Dim processStartInfo2 As New ProcessStartInfo With
            {
                .FileName = (Program.StartPath & "\7za.exe"),
                .Verb = "runas",
                .Arguments = ("e -y pso2.stripped.db.7z"),
                .WindowStyle = ProcessWindowStyle.Hidden,
            .UseShellExecute = True
            }
            Process.Start(processStartInfo2).WaitForExit()
            Dim DBMD5 As String = Helper.GetMd5("pso2.stripped.db")
            Helper.WriteDebugInfoAndOk("Downloading Trans-Am tool... ")
            DownloadFile(Program.FreedomUrl & "pso2-transam.exe", "pso2-transam.exe")

            'execute pso2-transam stuff with -b flag for backup
            Dim processStartInfo As ProcessStartInfo = New ProcessStartInfo() With {.FileName = "pso2-transam.exe", .Verb = "runas"}
            If Directory.Exists(backupdir) Then
                Dim counter = Computer.FileSystem.GetFiles(backupdir)
                If counter.Count > 0 Then
                    processStartInfo.Arguments = (TransamCodes2() & "story-eng-" & strStoryPatchLatestBase & " pso2.stripped.db " & """" & Program.Pso2WinDir & """")
                Else
                    Helper.Log("[TRANSAM] Creating backup directory")
                    Directory.CreateDirectory(backupdir)
                    Helper.WriteDebugInfo(Resources.strCreatingBackupDirectory)
                    processStartInfo.Arguments = (TransamCodes1() & """" & backupdir & """ " & TransamCodes2() & "story-eng-" & strStoryPatchLatestBase & " pso2.stripped.db " & """" & Program.Pso2WinDir & """")
                End If
            End If

            'We don't need to make backups anymore
            'Yes we do, shut up past AIDA.
            If Not Directory.Exists(backupdir) Then
                Helper.Log("[TRANSAM] Creating backup directory")
                Directory.CreateDirectory(backupdir)
                Helper.WriteDebugInfo(Resources.strCreatingBackupDirectory)
                processStartInfo.Arguments = (TransamCodes1() & """" & backupdir & """ " & TransamCodes2() & "story-eng-" & strStoryPatchLatestBase & " pso2.stripped.db " & """" & Program.Pso2WinDir & """")
            End If

            TRIALSystem("Request TRANSAM")

            processStartInfo.UseShellExecute = False
            Helper.Log("[TRANSAM] Starting shitstorm")
            processStartInfo.Arguments = processStartInfo.Arguments.Replace("\", "/")
            'Helper.Log("TRANSM parameters: " & processStartInfo.Arguments & vbCrLf & "TRANSAM Working Directory: " & processStartInfo.WorkingDirectory)
            Helper.Log("[TRANSAM] Program started")
            If Helper.GetMd5("pso2.stripped.db") <> DBMD5 Then
                MsgBox("ERROR: Files have been modified since download. Aborting...")
                Exit Sub
            End If

            Process.Start(processStartInfo).WaitForExit()
            Helper.DeleteFile("pso2.stripped.db")
            Helper.DeleteFile("pso2-transam.exe")
            External.FlashWindow(Handle, True)
            'Story Patch 3-12-2014.rar
            RegKey.SetValue(Of String)(RegKey.StoryPatchVersion, strStoryPatchLatestBase.Replace("-", "/"))
            RegKey.SetValue(Of String)(RegKey.LatestStoryBase, strStoryPatchLatestBase.Replace("-", "/"))
            Helper.WriteDebugInfo(Resources.strStoryPatchInstalled)
            CheckForStoryUpdates()
        Catch ex As Exception
            MsgBox("ERROR - " & ex.Message.ToString)
        End Try
    End Sub

    Private Sub RestoreJapaneseNames(filename As String, patchname As String, Optional url As String = "http://107.170.16.100/patches/")
        Try
            If IsPso2WinDirMissing() Then Return

            Helper.WriteDebugInfo(Resources.strDownloading & patchname & "...")
            Application.DoEvents()
            Dim strDownloadMe As String = url & filename
            _cancelled = False
            DownloadFile(strDownloadMe, filename)
            If _cancelled Then Return
            Helper.WriteDebugInfo((Resources.strDownloadCompleteDownloaded & filename & ")"))

            If File.Exists((Program.Pso2WinDir & "\" & filename)) Then
                If File.Exists((Program.Pso2WinDir & "\backup\" & filename)) = False Then
                    File.Move((Program.Pso2WinDir & "\" & filename), (Program.Pso2WinDir & "\backup\" & filename))
                End If
            End If

            Application.DoEvents()
            File.Move(filename, (Program.Pso2WinDir & "\" & filename))
            External.FlashWindow(Handle, True)
            Helper.WriteDebugInfo(patchname & " " & Resources.strInstalledUpdated)
            UnlockGui()
        Catch ex As Exception
            Helper.LogWithException(Resources.strERROR, ex)
        End Try
    End Sub

    Private Sub DownloadPatch(patchUrl As String, patchName As String, patchFile As String, versionStr As String, msgBackup As String, msgSelectArchive As String)
        _cancelledFull = False
        Try
            If IsPso2WinDirMissing() Then Return

            Dim backupyesno As MsgBoxResult
            Dim predownloadedyesno As MsgBoxResult
            Dim rarLocation As String = ""
            Dim strVersion As String = ""

            ' Check the patch download method preference
            Dim patchPreference As String = RegKey.GetValue(Of String)(RegKey.PreDownloadedRar)
            Select Case patchPreference
                Case "Ask"
                    predownloadedyesno = MsgBox(Resources.strWouldYouLikeToUse, vbYesNo)
                Case "Always"
                    predownloadedyesno = MsgBoxResult.Yes
                Case "Never"
                    predownloadedyesno = MsgBoxResult.No
                Case Else
                    predownloadedyesno = MsgBox(Resources.strWouldYouLikeToUse, vbYesNo)
            End Select

            ' Check the backup preference
            'Why was this changed to never? [AIDA]
            'patchPreference = "Never"
            patchPreference = RegKey.GetValue(Of String)(RegKey.Backup)
            Select Case patchPreference
                Case "Ask"
                    backupyesno = MsgBox(msgBackup, vbYesNo)
                Case "Always"
                    backupyesno = MsgBoxResult.Yes
                Case "Never"
                    backupyesno = MsgBoxResult.No
                Case Else
                    backupyesno = MsgBox(msgBackup, vbYesNo)
            End Select

            If predownloadedyesno = MsgBoxResult.No Then
                Helper.WriteDebugInfo(Resources.strDownloading & patchName & "...")
                Application.DoEvents()

                ' Might want to switch to a Uri class.
                ' Get the filename from the downloaded Path
                Dim lastfilename As String() = patchUrl.Split("/"c)
                strVersion = lastfilename(lastfilename.Length - 1)
                strVersion = Path.GetFileNameWithoutExtension(strVersion) ' We're using this so that it's not format-specific.

                _cancelled = False

                If Not Helper.CheckLink(patchUrl) Then
                    Helper.WriteDebugInfoAndFailed("Failed to contact " & patchName & " website - Patch install/update canceled!")
                    Helper.WriteDebugInfo("Please visit http://goo.gl/YzCE7 for more information!")
                    Return
                End If

                DownloadFile(patchUrl, patchFile)
                If _cancelled Then Return
                Helper.WriteDebugInfo((Resources.strDownloadCompleteDownloaded & patchUrl & ")"))
            ElseIf predownloadedyesno = MsgBoxResult.Yes Then
                OpenFileDialog1.Title = msgSelectArchive
                OpenFileDialog1.FileName = "PSO2 " & patchName & " RAR/ZIP file"
                OpenFileDialog1.Filter = "RAR Archives|*.rar|ZIP Archives|*.zip|All Files (*.*) |*.*"
                If OpenFileDialog1.ShowDialog() = DialogResult.Cancel Then Return

                rarLocation = OpenFileDialog1.FileName
                strVersion = OpenFileDialog1.SafeFileName
                strVersion = Path.GetFileNameWithoutExtension(strVersion)
            End If

            Application.DoEvents()

            Helper.DeleteDirectory("TEMPPATCHAIDAFOOL")
            Directory.CreateDirectory("TEMPPATCHAIDAFOOL")

            If patchFile.Contains(".rar") = True Then


                Dim startInfo As New ProcessStartInfo() With {.FileName = (Program.StartPath & "\unrar.exe"), .Verb = "runas", .WindowStyle = ProcessWindowStyle.Normal, .UseShellExecute = True}
                If predownloadedyesno = MsgBoxResult.No Then startInfo.Arguments = ("e """ & patchFile & """ TEMPPATCHAIDAFOOL")
                If predownloadedyesno = MsgBoxResult.Yes Then startInfo.Arguments = ("e " & """" & rarLocation & """" & " TEMPPATCHAIDAFOOL")

                Helper.WriteDebugInfo(Resources.strWaitingforPatch)
                Process.Start(startInfo).WaitForExit()
            End If

            If patchFile.Contains(".zip") = True Then
                Dim processStartInfo2 As New ProcessStartInfo With
                {
                    .FileName = (Program.StartPath & "\7za.exe"),
                    .Verb = "runas",
                    .Arguments = ("e -y """ & patchFile & """ -oTEMPPATCHAIDAFOOL"),
                    .WindowStyle = ProcessWindowStyle.Hidden,
                    .UseShellExecute = True
                }
                Helper.WriteDebugInfo(Resources.strWaitingforPatch)
                Process.Start(processStartInfo2).WaitForExit()
            End If

            If Not Directory.Exists("TEMPPATCHAIDAFOOL") Then
                Directory.CreateDirectory("TEMPPATCHAIDAFOOL")
                Helper.WriteDebugInfo("Had to manually make temp folder - Did the patch not extract right?")
            End If
            Dim diar1 As FileInfo() = New DirectoryInfo("TEMPPATCHAIDAFOOL").GetFiles()
            Helper.WriteDebugInfoAndOk((Resources.strExtractingTo & Program.Pso2WinDir))
            Application.DoEvents()
            If _cancelledFull Then Return

            Dim backupPath As String = BuildBackupPath(patchName)
            If backupyesno = MsgBoxResult.Yes Then
                If Directory.Exists(backupPath) Then
                    Directory.Delete(backupPath, True)
                    Directory.CreateDirectory(backupPath)
                    Helper.WriteDebugInfo(Resources.strErasingPreviousBackup)
                End If
                If Not Directory.Exists(backupPath) Then
                    Directory.CreateDirectory(backupPath)
                    Helper.WriteDebugInfo(Resources.strCreatingBackupDirectory)
                End If
            End If

            Helper.Log("Extracted " & diar1.Length & " files from the patch")
            If diar1.Length = 0 Then
                Helper.WriteDebugInfo("Patch failed to extract correctly! Installation failed!")
                Return
            End If

            Helper.WriteDebugInfo(Resources.strInstallingPatch)

            For Each dra As FileInfo In diar1
                If _cancelledFull Then Return
                If backupyesno = MsgBoxResult.Yes Then
                    If File.Exists((Program.Pso2WinDir & "\" & dra.ToString())) Then
                        File.Move((Program.Pso2WinDir & "\" & dra.ToString()), (backupPath & "\" & dra.ToString()))
                    End If
                End If
                If backupyesno = MsgBoxResult.No Then
                    If File.Exists((Program.Pso2WinDir & "\" & dra.ToString())) Then
                        Helper.DeleteFile((Program.Pso2WinDir & "\" & dra.ToString()))
                    End If
                End If
                File.Move(("TEMPPATCHAIDAFOOL\" & dra.ToString()), (Program.Pso2WinDir & "\" & dra.ToString()))
            Next

            Helper.DeleteDirectory("TEMPPATCHAIDAFOOL")
            If backupyesno = MsgBoxResult.No Then
                External.FlashWindow(Handle, True)
                Helper.WriteDebugInfo("Patch " & Resources.strInstalledUpdated)
                If Not String.IsNullOrEmpty(versionStr) Then RegKey.SetValue(Of String)(versionStr, strVersion)
            End If
            If backupyesno = MsgBoxResult.Yes Then
                External.FlashWindow(Handle, True)
                Helper.WriteDebugInfo(("Patch " & Resources.strInstalledUpdatedBackup & backupPath))
                If Not String.IsNullOrEmpty(versionStr) Then RegKey.SetValue(Of String)(versionStr, strVersion)
            End If
            Helper.DeleteFile(patchName)
            UnlockGui()
        Catch ex As Exception
            Helper.LogWithException(Resources.strERROR, ex)
        End Try
    End Sub

    Private Sub UninstallPatch(patchListUrl As String, patchListFile As String, patchName As String, consoleMsg As String, patchVersionKey As String)
        Try
            If IsPso2WinDirMissing() Then Return

            DownloadFile(patchListUrl, patchListFile)
            Dim missingfiles = File.ReadAllLines(patchListFile)
            Helper.DeleteFile(patchListFile)
            Helper.WriteDebugInfo(Resources.strUninstallingPatch)

            For index As Integer = 0 To (missingfiles.Length - 1)
                If _cancelledFull Then Return

                'Download JP file
                lblStatus.Text = Resources.strUninstalling & index & "/" & missingfiles.Length
                DownloadFile(("http://download.pso2.jp/patch_prod/patches/data/win32/" & missingfiles(index) & ".pat"), missingfiles(index))
                If New FileInfo(missingfiles(index)).Length = 0 Then DownloadFile(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & missingfiles(index) & ".pat"), missingfiles(index))

                'Move JP file to win32
                Helper.DeleteFile((Program.Pso2WinDir & "\" & missingfiles(index)))
                File.Move(missingfiles(index), (Program.Pso2WinDir & "\" & missingfiles(index)))
            Next

            Helper.DeleteDirectory(BuildBackupPath(patchName))
            External.FlashWindow(Handle, True)
            Helper.WriteDebugInfo(consoleMsg)
            RegKey.SetValue(Of String)(patchVersionKey, "Not Installed")
            UnlockGui()
        Catch ex As Exception
            Helper.LogWithException(Resources.strERROR, ex)
        End Try
    End Sub

    Private Sub RestoreBackup(patchName As String)
        Dim backupPath As String = BuildBackupPath(patchName)
        If Directory.Exists(backupPath) = False Then
            Helper.WriteDebugInfoAndWarning("Could not find the backup path! Are you sure you have a backup in your win32/backup folder?")
            Return
        End If

        Dim di As New DirectoryInfo(backupPath)
        Helper.WriteDebugInfoAndOk("Restoring " & patchName & " backup...")
        Application.DoEvents()

        'list the names of all files in the specified directory
        For Each dra As FileInfo In di.GetFiles()
            If File.Exists(Program.Pso2WinDir & "\" & dra.ToString()) Then
                Helper.DeleteFile(Program.Pso2WinDir & "\" & dra.ToString())
            End If
            File.Move(backupPath & "\" & dra.ToString(), Program.Pso2WinDir & "\" & dra.ToString())
        Next

        Helper.DeleteDirectory(backupPath)
        External.FlashWindow(Handle, True)

        Select Case patchName
            Case EnglishPatch
                If Not String.IsNullOrEmpty(RegKey.EnPatchVersion) Then RegKey.SetValue(Of String)(RegKey.EnPatchVersion, "Not Installed")
            Case LargeFiles
                If Not String.IsNullOrEmpty(RegKey.LargeFilesVersion) Then RegKey.SetValue(Of String)(RegKey.LargeFilesVersion, "Not Installed")
            Case StoryPatch
                If Not String.IsNullOrEmpty(RegKey.StoryPatchVersion) Then RegKey.SetValue(Of String)(RegKey.StoryPatchVersion, "Not Installed")
        End Select
        'WriteDebugInfoSameLine(" Done!")
        UnlockGui()
    End Sub

    Private Shared Function BuildBackupPath(ByVal patchName As String) As String
        Return Program.Pso2WinDir & "\backup\" & patchName & "\"
    End Function

    Private Sub tmrWaitingforPSO2_Tick(sender As Object, e As EventArgs) Handles tmrWaitingforPSO2.Tick
        tmrWaitingforPSO2.Enabled = False

        Dim YesNo As MsgBoxResult = MsgBox("PSO2 doesn't appear to have opened yet. This can be caused by Gameguard Issues. Would you like the Tweaker to try to solve the issues and relaunch the game?", vbYesNo)
        If YesNo = MsgBoxResult.No Then
            tmrWaitingforPSO2.Enabled = False
            Exit Sub
        End If
        If YesNo = MsgBoxResult.Yes Then
            SkipDialogs = True
            btnGameguard.RaiseClick()
        End If
    End Sub

    Private Sub btnLargeFilesTRANSAM_Click(sender As Object, e As EventArgs) Handles btnLargeFilesTRANSAM.Click
        'Install Large Files with TRANSAM to cut down on net costs for Agrajag and friends.
        'Need to speak with Agrajag and get some files before I can do this, though.
        Try
            If IsPso2WinDirMissing() Then Return

            ' Create a match using regular exp<b></b>ressions
            ' Spit out the value plucked from the code
            Dim backupdir As String = BuildBackupPath(LargeFiles)
            DLS.Headers("user-agent") = GetUserAgent()
            Dim LFDate As String = DLS.DownloadString(Program.FreedomUrl & "patches/largefilesTRANSAM.txt")

            Helper.WriteDebugInfoAndOk("Downloading Large Files info... ")
            DownloadFile(Program.FreedomUrl & "patches/lf.stripped.db.7z", "lf.stripped.db.7z")
            Dim processStartInfo2 As New ProcessStartInfo With
            {
                .FileName = (Program.StartPath & "\7za.exe"),
                .Verb = "runas",
                .Arguments = ("e -y lf.stripped.db.7z"),
                .WindowStyle = ProcessWindowStyle.Hidden,
            .UseShellExecute = True
            }
            Process.Start(processStartInfo2).WaitForExit()
            Dim DBMD5 As String = Helper.GetMd5("lf.stripped.db")
            Helper.WriteDebugInfoAndOk("Downloading Trans-Am tool... ")
            DownloadFile(Program.FreedomUrl & "pso2-transam.exe", "pso2-transam.exe")

            Dim processStartInfo As ProcessStartInfo = New ProcessStartInfo() With {.FileName = "pso2-transam.exe", .Verb = "runas"}
            If Directory.Exists(backupdir) Then
                Dim counter = Computer.FileSystem.GetFiles(backupdir)
                If counter.Count > 0 Then
                    processStartInfo.Arguments = (TransamCodes2() & "largefiles-" & LFDate & " lf.stripped.db " & """" & Program.Pso2WinDir & """")
                Else
                    Helper.Log("[TRANSAM] Creating backup directory")
                    Directory.CreateDirectory(backupdir)
                    Helper.WriteDebugInfo(Resources.strCreatingBackupDirectory)
                    processStartInfo.Arguments = (TransamCodes1() & """" & backupdir & """ " & TransamCodes2() & "largefiles-" & LFDate & " lf.stripped.db " & """" & Program.Pso2WinDir & """")
                End If
            End If

            'We don't need to make backups anymore
            'Yes we do, shut up past AIDA.
            If Not Directory.Exists(backupdir) Then
                Helper.Log("[TRANSAM] Creating backup directory")
                Directory.CreateDirectory(backupdir)
                Helper.WriteDebugInfo(Resources.strCreatingBackupDirectory)
                processStartInfo.Arguments = (TransamCodes1() & """" & backupdir & """ " & TransamCodes2() & "largefiles-" & LFDate & " lf.stripped.db " & """" & Program.Pso2WinDir & """")
            End If

            TRIALSystem("Request TRANSAM")

            processStartInfo.UseShellExecute = False
            Helper.Log("[TRANSAM] Starting shitstorm")
            processStartInfo.Arguments = processStartInfo.Arguments.Replace("\", "/")
            Helper.Log("[TRANSAM] Program started")
            If Helper.GetMd5("lf.stripped.db") <> DBMD5 Then
                MsgBox("ERROR: Files have been modified since download. Aborting...")
                Exit Sub
            End If
            Process.Start(processStartInfo).WaitForExit()
            Helper.DeleteFile("lf.stripped.db")
            Helper.DeleteFile("pso2-transam.exe")
            External.FlashWindow(Handle, True)
            'Story Patch 3-12-2014.rar
            RegKey.SetValue(Of String)(RegKey.LargeFilesVersion, LFDate.Replace("-", "/"))
            Helper.WriteDebugInfo("Large Files Patch installed!")
        Catch ex As Exception
            MsgBox("ERROR - " & ex.Message.ToString)
        End Try
    End Sub

    Public ReadOnly Property Epoch() As DateTime
        Get
            Return New DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        End Get
    End Property

    Public Function FromUnix(ByVal seconds As Integer) As DateTime
        Dim dt = Epoch.AddSeconds(seconds)
        Return dt
    End Function

    Private Sub CopyAllTextToClipboardToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CopyAllTextToClipboardToolStripMenuItem.Click
        Clipboard.SetText(rtbDebug.Text)
        Helper.WriteDebugInfo("All text copied to clipboard.")
    End Sub


    Private Sub btnLaunchPSO2fromORB_Click(sender As Object, e As EventArgs) Handles btnLaunchPSO2fromORB.Click
        btnLaunchPSO2.PerformClick()
    End Sub

    Private Sub btnPlugins_Click(sender As Object, e As EventArgs) Handles btnPlugins.Click
        'Show the plugin form (GEE THIS CODE LOOKS FAMILIAR)
        Cursor = Cursors.WaitCursor
        Try

            frmPlugins.TopMost = TopMost
            frmPlugins.Top += 50
            frmPlugins.Left += 50
            frmPlugins.ShowDialog()
            Exit Sub
        Catch ex As Exception
            Helper.LogWithException(Resources.strERROR, ex)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub
    Public Sub CheckForPluginUpdates()
        Dim source As String
        Try
            source = Program.AreYouAlive.DownloadString("http://107.170.16.100/Plugins/PluginMD5HashList.txt")
        Catch ex As Exception
            Helper.LogWithException("Failed to get the plugin update information. Usually, this means AIDA broke something. Please DO NOT panic, as the rest of the program will work fine. There is no need to report this error either.", ex)
            Exit Sub
        End Try
        DownloadFile("http://107.170.16.100/Plugins/PluginMD5HashList.txt", "PluginMD5HashList.txt")
        source = File.ReadAllText("PluginMD5HashList.txt")
        If Not String.IsNullOrEmpty(source) AndAlso source.Contains(".dll") AndAlso source.Contains(",") Then
            Using oReader As StreamReader = File.OpenText("PluginMD5HashList.txt")
                Dim strNewDate As String = oReader.ReadLine()
                RegKey.SetValue(Of String)(RegKey.NewPluginVersionTemp, strNewDate)
                RegKey.SetValue(Of String)(RegKey.NewPluginVersion, strNewDate)

                Dim HasParser As Boolean = False

                If File.Exists(Program.Pso2RootDir & "\plugins\PSO2DamageDump.dll") Or File.Exists(Program.Pso2RootDir & "\plugins\disabled\PSO2DamageDump.dll") Then HasParser = True

                If RegKey.GetValue(Of String)(RegKey.DeletedParserOnce) = "" And HasParser = True Then
                    If File.Exists(Program.Pso2RootDir & "\plugins\PSO2DamageDump.dll") Then File.Delete(Program.Pso2RootDir & "\plugins\PSO2DamageDump.dll")
                    If File.Exists(Program.Pso2RootDir & "\plugins\disabled\PSO2DamageDump.dll") Then File.Delete(Program.Pso2RootDir & "\plugins\disabled\PSO2DamageDump.dll")
                    MsgBox("The PSO2 Damage Parser Plugin has been automatically deleted as it may cause your account to be banned. For more information (and how to use it if you'd still like to), please visit http://aida.moe/parser/")
                    Helper.WriteDebugInfoAndWarning("The PSO2 Damage Parser Plugin has been automatically deleted as it may cause your account to be banned. For more information (and how to use it if you'd still like to), please visit http://aida.moe/parser/")
                    RegKey.SetValue(Of String)(RegKey.DeletedParserOnce, "True")
                End If

                If strNewDate <> RegKey.GetValue(Of String)(RegKey.PluginVersion) Or (Directory.GetFiles(Program.Pso2RootDir & "\plugins\").Count = 0 And Directory.GetFiles(Program.Pso2RootDir & "\plugins\disabled").Count = 0) Or File.Exists(Program.Pso2RootDir & "\pso2h.dll") = False Or File.Exists(Program.Pso2RootDir & "\translation_titles.bin") = False Or File.Exists(Program.Pso2RootDir & "\translation.bin") = False Then
                    'Update plugins [AIDA]

                    Dim missingfiles As New List(Of String)
                    Dim numberofChecks As Integer = 0
                    Dim truefilename As String
                    Dim filename As String()
                    Dim FinalExportString As String = ""
                    Helper.WriteDebugInfo("Beginning plugin update...")
                    'Move all plugins to the disabled folder instead. [AIDA]

                    RegKey.SetValue(Of String)(RegKey.PluginsEnabled, FinalExportString)

                    For Each fi As FileInfo In New DirectoryInfo(Program.Pso2RootDir & "\plugins\").GetFiles
                            If File.Exists(Path.Combine(Program.Pso2RootDir & "\plugins\disabled", fi.Name)) Then File.Delete(Path.Combine(Program.Pso2RootDir & "\plugins\disabled", fi.Name))
                            File.Move(fi.FullName, Path.Combine(Program.Pso2RootDir & "\plugins\disabled", fi.Name))
                            FinalExportString += fi.Name & ","
                        Next
                        If FinalExportString.Length > 0 Then FinalExportString = FinalExportString.Remove(FinalExportString.Length - 1, 1)
                        RegKey.SetValue(Of String)(RegKey.PluginsEnabled, FinalExportString)
                        While Not (oReader.EndOfStream)
                            filename = oReader.ReadLine().Split(","c)
                            truefilename = filename(0)

                            If truefilename = "pso2h.dll" Or truefilename = "translation_titles.bin" Or truefilename = "translation.bin" Then
                                If Not File.Exists((Program.Pso2RootDir & "\" & truefilename)) Then
                                    missingfiles.Add(truefilename)
                                ElseIf Helper.GetMd5((Program.Pso2RootDir & "\" & truefilename)) <> filename(1) Then
                                    missingfiles.Add(truefilename)
                                End If
                            Else
                                If Not File.Exists((Program.Pso2RootDir & "\plugins\disabled\" & truefilename)) Then
                                    missingfiles.Add(truefilename)
                                ElseIf Helper.GetMd5((Program.Pso2RootDir & "\plugins\disabled\" & truefilename)) <> filename(1) Then
                                    missingfiles.Add(truefilename)
                                End If
                            End If
                            numberofChecks += 1
                            lblStatus.Text = (Resources.strCurrentlyCheckingFile & numberofChecks & "")
                            Application.DoEvents()
                        End While

                        Helper.WriteDebugInfo("Downloading/Installing updates...")
                        Dim totaldownload As Long = missingfiles.Count
                        Dim downloaded As Long = 0

                    For Each downloadStr As String In missingfiles
                        'Download the missing files:
                        downloaded += 1
                        lblStatus.Text = Resources.strUpdating & downloaded & "/" & totaldownload
                        Application.DoEvents()
                        _cancelled = False
                        DownloadFile(("http://107.170.16.100/Plugins/" & downloadStr), downloadStr)
                        If _cancelled Then Return
                        'Delete the existing file FIRST
                        If Not File.Exists(downloadStr) Then
                            Helper.WriteDebugInfoAndFailed("File " & downloadStr & " does not exist! Perhaps it wasn't downloaded properly?")
                        End If
                        'If this code works, it is only because the GODDESS ZELDA HAS WISHED IT SO.
                        If downloadStr = "pso2h.dll" Or downloadStr = "translation_titles.bin" Or downloadStr = "translation.bin" Then
                            If Environment.CurrentDirectory <> Program.Pso2RootDir Then
                                Helper.DeleteFile((Program.Pso2RootDir & "\" & downloadStr))
                                File.Move(downloadStr, (Program.Pso2RootDir & "\" & downloadStr))
                            End If
                        Else
                            If downloadStr = "PSO2Proxy.dll" Then
                                'If Not Dns.GetHostEntry("gs001.pso2gs.net").AddressList(0).ToString().Contains("210.189.") Then
                                If Not Dns.GetHostEntry("gs001.pso2gs.net").AddressList(0).ToString().Contains("210.189.") Then
                                    Helper.WriteDebugInfo("PSO2Proxy usage detected! Auto-enabling PSO2Proxy plugin...")
                                    File.Move(downloadStr, (Program.Pso2RootDir & "\plugins\" & downloadStr))
                                Else
                                    Helper.DeleteFile((Program.Pso2RootDir & "\plugins\disabled\" & downloadStr))
                                    File.Move(downloadStr, (Program.Pso2RootDir & "\plugins\disabled\" & downloadStr))
                                End If
                            Else
                                Helper.DeleteFile((Program.Pso2RootDir & "\plugins\disabled\" & downloadStr))
                                File.Move(downloadStr, (Program.Pso2RootDir & "\plugins\disabled\" & downloadStr))
                            End If
                        End If

                        If File.Exists(downloadStr) = True And Environment.CurrentDirectory <> Program.Pso2RootDir Then Helper.DeleteFile(downloadStr)
                        Application.DoEvents()
                    Next

                    If NewInstall = True Then
                        Helper.WriteDebugInfo("Auto-enabling item/title translations for fresh plugin install...")
                        RegKey.SetValue(Of String)(RegKey.PluginsEnabled, "PSO2TitleTranslator.dll,translator.dll")
                    End If


                    'Restore the plugins to their proper folders now
                    'If there's enabled plugins, do stuff.
                    Dim FileToMove As String = ""
                        If RegKey.GetValue(Of String)(RegKey.PluginsEnabled).ToString.Length > 1 Then
                            'Check to see if it's more than one file by seeing if there are any commas
                            If RegKey.GetValue(Of String)(RegKey.PluginsEnabled).Contains(",") = False Then
                                'It's just one file
                                'MsgBox("One plugin enabled - " & RegKey.GetValue(Of String)(RegKey.PluginsEnabled).ToString)
                                FileToMove = RegKey.GetValue(Of String)(RegKey.PluginsEnabled).ToString
                                If File.Exists(Program.Pso2RootDir & "\plugins\" & FileToMove) = True Then Helper.DeleteFile((Program.Pso2RootDir & "\plugins\" & FileToMove))
                                If File.Exists(Program.Pso2RootDir & "\plugins\disabled\" & FileToMove) Then File.Move((Program.Pso2RootDir & "\plugins\disabled\" & FileToMove), (Program.Pso2RootDir & "\plugins\" & FileToMove))
                            Else
                                'It's multiple files
                                Dim EnabledPlugins() As String = RegKey.GetValue(Of String)(RegKey.PluginsEnabled).Split(CType(",", Char()))
                                For Each EnabledFilename In EnabledPlugins
                                    'MsgBox(EnabledFilename)
                                    If File.Exists(Program.Pso2RootDir & "\plugins\" & EnabledFilename) = True Then Helper.DeleteFile((Program.Pso2RootDir & "\plugins\" & EnabledFilename))
                                    If File.Exists(Program.Pso2RootDir & "\plugins\disabled\" & EnabledFilename) Then File.Move((Program.Pso2RootDir & "\plugins\disabled\" & EnabledFilename), (Program.Pso2RootDir & "\plugins\" & EnabledFilename))
                                Next
                            End If
                        Else
                            'MsgBox("No plugins enabled!")
                        End If

                        Helper.WriteDebugInfoAndOk("Plugins updated successfully.")
                        RegKey.SetValue(Of String)(RegKey.PluginVersion, RegKey.GetValue(Of String)(RegKey.NewPluginVersionTemp))
                        RegKey.SetValue(Of String)(RegKey.NewPluginVersionTemp, "")
                    Else
                        Helper.WriteDebugInfoAndOk("No plugin updates available.")
                End If
            End Using
        Else
            Helper.WriteDebugInfoAndFailed("Failed to get the plugin update information. Usually, this means AIDA broke something. Please DO NOT panic, as the rest of the program will work fine. There is no need to report this error either.")
        End If
        If File.Exists("PluginMD5HashList.txt") = True Then Helper.DeleteFile("PluginMD5HashList.txt")
    End Sub

    Private Sub chkItemTranslation_Click(sender As Object, e As EventArgs) Handles chkItemTranslation.Click
        MsgBox("The item translation is now controlled through the plugin menu. Please click the Plugins button at the bottom of the menu.")
        chkItemTranslation.Checked = False
    End Sub

    Private Sub btnConfigureItemTranslation_Click(sender As Object, e As EventArgs) Handles btnConfigureItemTranslation.Click
        MsgBox("The item translation is now controlled through the plugin menu. Please click the Plugins button at the bottom of this menu.")
    End Sub

    Private Function GenerateELSName() As String
        Dim s As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"
        Dim r As New Random
        Dim sb As New StringBuilder
        For i As Integer = 1 To 8
            Dim idx As Integer = r.Next(0, 35)
            sb.Append(s.Substring(idx, 1))
        Next
        Return sb.ToString()
    End Function

    Private Sub Office2007StartButton1_Click(sender As Object, e As EventArgs) Handles Office2007StartButton1.Click

    End Sub

    Private Sub btnRussianBigFiles_Click(sender As Object, e As EventArgs) Handles btnRussianBigFiles.Click
        DLS.Headers("user-agent") = GetUserAgent()
        Dim url As String = DLS.DownloadString(Program.FreedomUrl & "patches/rulargefiles.txt")
        DownloadPatch(url, RussianBigPatch, "RUBigFiles.zip", RegKey.NullKey, "Would you like to backup your files before applying the patch? This will erase all previous Russian Patch backups." & vbCrLf & "Хотите ли вы сделать резервную копию ваших файлов перед установкой патча? Это приведёт к удалению предыдущих резервных копий русского патча.", "Please select the pre-downloaded Russian Patch ZIP file." & vbCrLf & "Пожалуйста, выберите предварительно-загруженный русский zip патч файл.")
    End Sub

    Private Sub btnInstallSpanishPatch_Click(sender As Object, e As EventArgs) Handles btnInstallSpanishPatch.Click
        DLS.Headers("user-agent") = GetUserAgent()
        Dim url As String = DLS.DownloadString("http://107.170.16.100/patches/espatch.txt")
        'Really need to rewrite this code to detect the filetype for me. [AIDA]
        DownloadPatch(url, SpanishPatch, "ESPatch.zip", RegKey.NullKey, "Would you like to backup your files before applying the patch? This will erase all previous Spanish Patch backups." & vbCrLf & "¿Deseas hacer una copia de tus ficheros antes de aplicar el parche? Esto eliminará las copias de seguridad anteriores del Parche español.", "Please select the pre-downloaded Spanish Patch ZIP file." & vbCrLf & "Por favor seleccione el fichero ZIP del parche español predescargado.")
    End Sub

    Private Sub btnInstallGermanPatch_Click(sender As Object, e As EventArgs) Handles btnInstallGermanPatch.Click
        DLS.Headers("user-agent") = GetUserAgent()
        Dim url As String = DLS.DownloadString("http://107.170.16.100/patches/depatch.txt")
        DownloadPatch(url, GermanPatch, "DEPatch.zip", RegKey.NullKey, "Would you like to backup your files before applying the patch? This will erase all previous German Patch backups." & vbCrLf & "Möchtest du eine Sicherung erstellen, bevor Änderungen am Spiel vorgenommen werden? Damit werden alle vorherigen Sicherungen des deutschen Patchs gelöscht.", "Please select the pre-downloaded German Patch ZIP file." & vbCrLf & "Bitte wähle die zuvor heruntergeladene ZIP-Datei des deutschen Patchs aus.")
    End Sub

    Private Sub btnJPETrials_Click_1(sender As Object, e As EventArgs) Handles btnJPETrials.Click
        RestoreJapaneseNames("057aa975bdd2b372fe092614b0f4399e", "JP E-Trials file")
    End Sub

    Private Sub btnJPEnemyNames_Click_1(sender As Object, e As EventArgs) Handles btnJPEnemyNames.Click
        RestoreJapaneseNames("ceffe0e2386e8d39f188358303a92a7d", "JP enemy names")
    End Sub

    Private Async Sub btnQUANTUMSYSTEM_Click(sender As Object, e As EventArgs) Handles btnQUANTUMSYSTEM.Click
        LockGui()
        ' Use IOC Container in the main Tweaker project to deal with dependencies.
        Dim output As New TweakerTrigger
        Dim Settings = New RegistryTweakerSettings("Software\AIDA")
        Dim updater = New UpdateManager(Settings, output)

        'await updater.CleanLegacyFiles();

        'Console.WriteLine(settings.GameDirectory)
        Try

            'frmDownloader.TopMost = True
            'Me.TopMost = False
        Catch ex As Exception
            Helper.LogWithException(Resources.strERROR, ex)
        End Try
        frmDownloader.CleanupUI()
        Await updater.Update(False, False)

    End Sub

    Public Sub FinalUpdateSteps()
        TweakerTrigger.patchwriter.Close()
        'Final update steps - Set the version file, reset patches. [AIDA]
        _cancelled = False
        'Helper.WriteDebugInfo(Resources.strDownloading & "version file...")
        DLS.DownloadFile("http://arks-layer.com/vanila/version.txt", "version.ver")


        If File.Exists((_myDocuments & "\SEGA\PHANTASYSTARONLINE2\version.ver")) Then Helper.DeleteFile((_myDocuments & "\SEGA\PHANTASYSTARONLINE2\version.ver"))
        File.Copy("version.ver", (_myDocuments & "\SEGA\PHANTASYSTARONLINE2\version.ver"))
        'Helper.WriteDebugInfoAndOk((Resources.strDownloadedandInstalled & "version file"))

        'If RegKey.GetValue(Of String)(RegKey.StoryPatchVersion) <> "Not Installed" Then RegKey.SetValue(Of String)(RegKey.StoryPatchVersion, "Not Updated")
        'If RegKey.GetValue(Of String)(RegKey.EnPatchVersion) <> "Not Installed" Then RegKey.SetValue(Of String)(RegKey.EnPatchVersion, "Not Updated")
        'If RegKey.GetValue(Of String)(RegKey.LargeFilesVersion) <> "Not Installed" Then RegKey.SetValue(Of String)(RegKey.LargeFilesVersion, "Not Updated")

        Helper.WriteDebugInfo("Game updated to the latest version. Don't forget to re-install/update the patches, as some of the files might have been untranslated.")
        RegKey.SetValue(Of String)(RegKey.Pso2RemoteVersion, File.ReadAllLines("version.ver")(0))
        UnlockGui()
    End Sub

    Private Sub BtnUpdatePso2_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub BtnUpdatePso2_Click_1(sender As Object, e As EventArgs) Handles BtnUpdatePso2.Click
        Dim ContinueYesNo As MsgBoxResult = MsgBox("WARNING: Checking with this old method will erase your saved QUANTUM data, and you'll have to recheck everything through QUANTUM next time you run it. Continue?", vbYesNo)
        If ContinueYesNo = vbYes Then UpdatePso2(False)
    End Sub
    Private Sub UpdatePso2(comingFromOldFiles As Boolean)
        _cancelledFull = False
        If IsPso2WinDirMissing() Then Return
        Dim missingfiles As New List(Of String)
        Dim missingfiles2 As New List(Of String)
        Dim numberofChecks As Integer = 0
        Dim totalfilesize As Long = 0
        Dim testfilesize As String()
        lblStatus.Text = ""

        If File.Exists("client.json") Then Helper.DeleteFile("client.json")
        If File.Exists("missing.json") Then Helper.DeleteFile("missing.json")
        If File.Exists("patchlist.json") Then Helper.DeleteFile("patchlist.json")

        If Directory.Exists(BuildBackupPath(EnglishPatch)) Then
            Helper.WriteDebugInfo(Resources.strENBackupFound)
            RestoreBackup(EnglishPatch)
        End If

        If Directory.Exists(BuildBackupPath(LargeFiles)) Then
            Helper.WriteDebugInfo(Resources.strLFBackupFound)
            RestoreBackup(LargeFiles)
        End If

        If Directory.Exists(BuildBackupPath(StoryPatch)) Then
            Helper.WriteDebugInfo(Resources.strStoryBackupFound)
            RestoreBackup(StoryPatch)
        End If

        LockGui()
        Helper.WriteDebugInfo(Resources.strDownloadingPatchFile1)

        Application.DoEvents()
        DownloadFile("http://download.pso2.jp/patch_prod/patches/launcherlist.txt", "launcherlist.txt")
        Helper.WriteDebugInfoSameLine(Resources.strDone)
        Helper.WriteDebugInfo(Resources.strDownloadingPatchFile2)

        Application.DoEvents()
        DownloadFile("http://download.pso2.jp/patch_prod/patches/patchlist.txt", "patchlist.txt")
        Helper.WriteDebugInfoSameLine(Resources.strDone)
        Helper.WriteDebugInfo(Resources.strDownloadingPatchFile3)

        Application.DoEvents()
        DownloadFile("http://download.pso2.jp/patch_prod/patches_old/patchlist.txt", "patchlist_old.txt")
        Helper.WriteDebugInfoSameLine(Resources.strDone)
        Helper.WriteDebugInfo(Resources.strDownloadingPatchFile4)

        Application.DoEvents()
        Program.Client.DownloadFile("http://arks-layer.com/vanila/version.txt", "version.ver")
        Helper.WriteDebugInfoSameLine(Resources.strDone)
        Application.DoEvents()
        UnlockGui()

        'Const result As MsgBoxResult = MsgBoxResult.No
        'If result = MsgBoxResult.Yes OrElse _comingFromOldFiles Then
        If comingFromOldFiles Then
            Helper.WriteDebugInfo(Resources.strCheckingforNewContent)
            numberofChecks = 0

            If _cancelledFull Then Return
            For Each line In Helper.GetLines("patchlist.txt")
                If _cancelledFull Then Return
                Dim filename As String() = Regex.Split(line, ".pat")
                Dim truefilename As String = filename(0).Replace("data/win32/", "")
                Dim trueMd5 As String = filename(1).Split(ControlChars.Tab)(2)
                If truefilename <> "GameGuard.des" AndAlso truefilename <> "edition.txt" AndAlso truefilename <> "gameversion.ver" AndAlso truefilename <> "pso2.exe" AndAlso truefilename <> "PSO2JP.ini" AndAlso truefilename <> "script/user_default.pso2" AndAlso truefilename <> "script/user_intel.pso2" Then
                    If Not File.Exists((Program.Pso2WinDir & "\" & truefilename)) Then
                        If _vedaUnlocked Then Helper.WriteDebugInfo("DEBUG: The file " & truefilename & " is missing.")
                        missingfiles.Add(truefilename)
                    ElseIf Helper.GetMd5((Program.Pso2WinDir & "\" & truefilename)) <> trueMd5 Then
                        If _vedaUnlocked Then Helper.WriteDebugInfo("DEBUG: The file " & truefilename & " must be redownloaded.")
                        missingfiles.Add(truefilename)
                    End If
                End If

                numberofChecks += 1
                lblStatus.Text = (Resources.strCurrentlyCheckingFile & numberofChecks & "")
                Application.DoEvents()
            Next

            Helper.DeleteFile("resume.txt")
            File.AppendAllLines("resume.txt", missingfiles)
            Dim totaldownload As Long = missingfiles.Count
            Dim downloaded As Long = 0
            Dim totaldownloadedthings As Long = 0

            For Each downloadStr In missingfiles
                If _cancelledFull Then Return
                'Download the missing files:
                'WHAT THE FUCK IS GOING ON HERE?
                downloaded += 1
                totaldownloadedthings += _totalsize2
                lblStatus.Text = Resources.strDownloading & "" & downloaded & "/" & totaldownload & " (" & Helper.SizeSuffix(totaldownloadedthings) & ")"

                Application.DoEvents()
                _cancelled = False
                DownloadFile(("http://download.pso2.jp/patch_prod/patches/data/win32/" & downloadStr & ".pat"), downloadStr)
                If New FileInfo(downloadStr).Length = 0 Then
                    Helper.Log("File appears to be empty, trying to download from secondary SEGA server")
                    DownloadFile(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & downloadStr & ".pat"), downloadStr)
                End If

                If _cancelled Then Return
                Helper.DeleteFile((Program.Pso2WinDir & "\" & downloadStr))
                File.Move(downloadStr, (Program.Pso2WinDir & "\" & downloadStr))
                If _vedaUnlocked Then Helper.WriteDebugInfo("DEBUG: Downloaded and installed " & downloadStr & ".")
                Dim linesList As New List(Of String)(File.ReadAllLines("resume.txt"))

                'Remove the line to delete, e.g.
                linesList.Remove(downloadStr)

                File.WriteAllLines("resume.txt", linesList.ToArray())
                Application.DoEvents()
            Next

            If missingfiles.Count = 0 Then Helper.WriteDebugInfo(Resources.strYouAppearToBeUpToDate)
            Dim directoryStringthing As String = (Program.Pso2RootDir & "\")
            Helper.WriteDebugInfo(Resources.strDownloading & "version file...")
            Application.DoEvents()
            _cancelled = False
            Program.Client.DownloadFile("http://arks-layer.com/vanila/version.txt", "version.ver")
            If _cancelled Then Return
            If File.Exists((_myDocuments & "\SEGA\PHANTASYSTARONLINE2\version.ver")) Then Helper.DeleteFile((_myDocuments & "\SEGA\PHANTASYSTARONLINE2\version.ver"))
            File.Copy("version.ver", (_myDocuments & "\SEGA\PHANTASYSTARONLINE2\version.ver"))
            Helper.WriteDebugInfoAndOk((Resources.strDownloadedandInstalled & "version file"))

            Helper.WriteDebugInfo(Resources.strDownloading & "pso2launcher.exe...")
            Application.DoEvents()
            For Each proc As Process In Process.GetProcessesByName("pso2launcher")
                If proc.MainWindowTitle = "PHANTASY STAR ONLINE 2" AndAlso proc.MainModule.ToString() = "ProcessModule (pso2launcher.exe)" Then proc.Kill()
            Next

            DownloadFile("http://download.pso2.jp/patch_prod/patches/pso2launcher.exe.pat", "pso2launcher.exe")
            If _cancelled Then Return
            If File.Exists((directoryStringthing & "pso2launcher.exe")) AndAlso Program.StartPath <> Program.Pso2RootDir Then Helper.DeleteFile((directoryStringthing & "pso2launcher.exe"))
            File.Move("pso2launcher.exe", (directoryStringthing & "pso2launcher.exe"))
            Helper.WriteDebugInfoAndOk((Resources.strDownloadedandInstalled & "pso2launcher.exe"))
            Helper.WriteDebugInfo(Resources.strDownloading & "pso2updater.exe...")
            Application.DoEvents()
            For Each proc As Process In Process.GetProcessesByName("pso2updater")
                If proc.MainModule.ToString() = "ProcessModule (pso2updater.exe)" Then proc.Kill()
            Next

            DownloadFile("http://download.pso2.jp/patch_prod/patches/pso2updater.exe.pat", "pso2updater.exe")
            If _cancelled Then Return
            If File.Exists((directoryStringthing & "pso2updater.exe")) AndAlso Program.StartPath <> Program.Pso2RootDir Then Helper.DeleteFile((directoryStringthing & "pso2updater.exe"))
            File.Move("pso2updater.exe", (directoryStringthing & "pso2updater.exe"))
            Helper.WriteDebugInfoAndOk((Resources.strDownloadedandInstalled & "pso2updater.exe"))
            Application.DoEvents()

            Helper.WriteDebugInfo(Resources.strDownloading & "pso2.exe...")
            For Each proc As Process In Process.GetProcessesByName("pso2")
                If proc.MainModule.ToString() = "ProcessModule (pso2.exe)" Then proc.Kill()
            Next

            DownloadFile("http://download.pso2.jp/patch_prod/patches/pso2.exe.pat", "pso2.exe")
            If _cancelled Then Return

            If File.Exists((directoryStringthing & "pso2.exe")) AndAlso Program.StartPath <> Program.Pso2RootDir Then Helper.DeleteFile((directoryStringthing & "pso2.exe"))
            File.Move("pso2.exe", (directoryStringthing & "pso2.exe"))
            If _cancelledFull Then Return
            Helper.WriteDebugInfoAndOk((Resources.strDownloadedandInstalled & "pso2.exe"))
            RegKey.SetValue(Of String)(RegKey.StoryPatchVersion, "Not Installed")
            RegKey.SetValue(Of String)(RegKey.EnPatchVersion, "Not Installed")
            RegKey.SetValue(Of String)(RegKey.LargeFilesVersion, "Not Installed")
            RegKey.SetValue(Of String)(RegKey.Pso2PatchlistMd5, Helper.GetMd5("patchlist.txt"))
            Helper.WriteDebugInfo(Resources.strGameUpdatedVanilla)
            Helper.DeleteFile("resume.txt")
            RegKey.SetValue(Of String)(RegKey.Pso2RemoteVersion, File.ReadAllLines("version.ver")(0))
            UnlockGui()

            Helper.WriteDebugInfoAndOk(Resources.strallDone)
            Return
        Else
            TopMost = chkAlwaysOnTop.Checked
        End If

        If _cancelledFull Then Return
        Dim mergedPatches = MergePatches()
        Helper.WriteDebugInfo(Resources.strCheckingforAllFiles)

        mergedPatches.Remove("GameGuard.des")
        mergedPatches.Remove("PSO2JP.ini")
        mergedPatches.Remove("script/user_default.pso2")
        mergedPatches.Remove("script/user_intel.pso2")
        mergedPatches.Remove("")

        If mergedPatches.ContainsKey("pso2.exe") Then
            mergedPatches.Remove("pso2.exe")
        End If

        Dim dataPath = Program.Pso2RootDir & "\data\win32\"
        Dim length = mergedPatches.Count
        Dim oldmax = PBMainBar.Maximum
        PBMainBar.Maximum = length
        _cancelled = False

        Dim fileLengths = New DirectoryInfo(dataPath).EnumerateFiles().ToDictionary(Function(fileinfo) fileinfo.Name, Function(fileinfo) fileinfo.Length)
        Dim fileNames = fileLengths.Keys

        For Each kvp In mergedPatches

            If _cancelled Then
                PBMainBar.Text = ""
                PBMainBar.Value = 0
                PBMainBar.Maximum = oldmax
                _cancelled = False
                Return
            End If

            lblStatus.Text = (Resources.strCurrentlyCheckingFile & numberofChecks)
            PBMainBar.Text = numberofChecks & " / " & length
            If (numberofChecks Mod 8) = 0 Then Application.DoEvents()
            numberofChecks += 1
            PBMainBar.Value += 1

            If Not fileNames.Contains(kvp.Key) Then
                If _vedaUnlocked Then Helper.WriteDebugInfo("DEBUG: The file " & (dataPath & kvp.Key) & Resources.strIsMissing)
                testfilesize = kvp.Value.Split(ControlChars.Tab)
                totalfilesize += Convert.ToInt32(testfilesize(1))
                missingfiles2.Add(kvp.Key)
                Continue For
            End If

            testfilesize = kvp.Value.Split(ControlChars.Tab)
            Dim fileSize = Convert.ToInt32(testfilesize(1))

            If fileSize <> fileLengths(kvp.Key) Then
                If _vedaUnlocked Then Helper.WriteDebugInfo("DEBUG: The file " & kvp.Key & " must be redownloaded.")
                totalfilesize += fileSize
                missingfiles2.Add(kvp.Key)
                Continue For
            End If

            Using stream = New FileStream(dataPath & kvp.Key, FileMode.Open)
                If Helper.GetMd5(stream) <> testfilesize(2) Then
                    If _vedaUnlocked Then Helper.WriteDebugInfo("DEBUG: The file " & kvp.Key & " must be redownloaded.")
                    totalfilesize += fileSize
                    missingfiles2.Add(kvp.Key)
                End If
            End Using
        Next

        PBMainBar.Text = ""
        PBMainBar.Value = 0
        PBMainBar.Maximum = oldmax

        Dim totaldownload2 As Long = missingfiles2.Count
        Dim downloaded2 As Long = 0
        Dim totaldownloaded As Long = 0
        Helper.DeleteFile("resume.txt")
        File.WriteAllLines("resume.txt", missingfiles2.ToArray())

        For Each downloadStr In missingfiles2
            If _cancelledFull Then Return
            'Download the missing files:
            'WHAT THE FUCK IS GOING ON HERE?
            downloaded2 += 1
            totaldownloaded += _totalsize2

            lblStatus.Text = Resources.strDownloading & "" & downloaded2 & "/" & totaldownload2 & " (" & Helper.SizeSuffix(totaldownloaded) & " / " & Helper.SizeSuffix(totalfilesize) & ")"

            Application.DoEvents()
            DownloadFile(("http://download.pso2.jp/patch_prod/patches/data/win32/" & downloadStr & ".pat"), downloadStr)
            If New FileInfo(downloadStr).Length = 0 Then
                Helper.Log("File appears to be empty, trying to download from secondary SEGA server")
                DownloadFile(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & downloadStr & ".pat"), downloadStr)
            End If
            If New FileInfo(downloadStr).Length = 0 Then
                Helper.DeleteFile(downloadStr)
                DownloadFile(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & downloadStr & ".pat"), downloadStr)
            End If

            If File.Exists(downloadStr) Then
                Helper.DeleteFile((Program.Pso2WinDir & "\" & downloadStr))
                File.Move(downloadStr, (Program.Pso2WinDir & "\" & downloadStr))
                If _vedaUnlocked Then Helper.WriteDebugInfo("DEBUG: Downloaded and installed " & downloadStr & ".")
                Dim linesList As New List(Of String)(File.ReadAllLines("resume.txt"))

                'Remove the line to delete, e.g.
                linesList.Remove(downloadStr)

                File.WriteAllLines("resume.txt", linesList.ToArray())
            End If
            Application.DoEvents()
        Next

        If missingfiles.Count = 0 Then Helper.WriteDebugInfo(Resources.strYouAppearToBeUpToDate)
        Dim directoryString As String = (Program.Pso2RootDir & "\")
        Helper.WriteDebugInfo(Resources.strDownloading & "version file...")
        Application.DoEvents()
        Program.Client.DownloadFile("http://arks-layer.com/vanila/version.txt", "version.ver")
        If File.Exists((_myDocuments & "\SEGA\PHANTASYSTARONLINE2\version.ver")) Then Helper.DeleteFile((_myDocuments & "\SEGA\PHANTASYSTARONLINE2\version.ver"))
        File.Copy("version.ver", (_myDocuments & "\SEGA\PHANTASYSTARONLINE2\version.ver"))
        Helper.WriteDebugInfoAndOk((Resources.strDownloadedandInstalled & "version file"))

        Helper.WriteDebugInfo(Resources.strDownloading & "pso2launcher.exe...")
        Application.DoEvents()
        DownloadFile("http://download.pso2.jp/patch_prod/patches/pso2launcher.exe.pat", "pso2launcher.exe")
        If File.Exists((directoryString & "pso2launcher.exe")) AndAlso Program.StartPath <> Program.Pso2RootDir Then Helper.DeleteFile((directoryString & "pso2launcher.exe"))
        File.Move("pso2launcher.exe", (directoryString & "pso2launcher.exe"))
        Helper.WriteDebugInfoAndOk((Resources.strDownloadedandInstalled & "pso2launcher.exe"))
        Helper.WriteDebugInfo(Resources.strDownloading & "pso2updater.exe...")
        Application.DoEvents()

        Helper.WriteDebugInfo(Resources.strDownloading & "pso2.exe...")
        DownloadFile("http://download.pso2.jp/patch_prod/patches/pso2.exe.pat", "pso2.exe")
        If _cancelled Then Return

        If File.Exists((directoryString & "pso2.exe")) AndAlso Program.StartPath <> Program.Pso2RootDir Then Helper.DeleteFile((directoryString & "pso2.exe"))
        File.Move("pso2.exe", (directoryString & "pso2.exe"))
        If _cancelledFull Then Return
        Helper.WriteDebugInfoAndOk((Resources.strDownloadedandInstalled & "pso2.exe"))

        RegKey.SetValue(Of String)(RegKey.StoryPatchVersion, "Not Installed")
        RegKey.SetValue(Of String)(RegKey.EnPatchVersion, "Not Installed")
        RegKey.SetValue(Of String)(RegKey.LargeFilesVersion, "Not Installed")
        RegKey.SetValue(Of String)(RegKey.Pso2PatchlistMd5, Helper.GetMd5("patchlist.txt"))
        Helper.WriteDebugInfo(Resources.strGameUpdatedVanilla)
        Helper.DeleteFile("resume.txt")
        RegKey.SetValue(Of String)(RegKey.Pso2RemoteVersion, File.ReadAllLines("version.ver")(0))
        UnlockGui()

        Helper.WriteDebugInfoAndOk(Resources.strallDone)
    End Sub

    Private Sub ButtonItem4_Click(sender As Object, e As EventArgs) Handles ButtonItem4.Click
        Dim ContinueYesNo As MsgBoxResult = MsgBox("WARNING: Checking with this old method will erase your saved QUANTUM data, and you'll have to recheck everything through QUANTUM next time you run it. Continue?", vbYesNo)
        If ContinueYesNo = vbNo Then Exit Sub
        LockGui()
        Helper.WriteDebugInfo(Resources.strDownloadingPatchFile1)
        Application.DoEvents()
        DownloadFile("http://download.pso2.jp/patch_prod/patches/launcherlist.txt", "launcherlist.txt")
        Helper.WriteDebugInfoSameLine(Resources.strDone)
        Helper.WriteDebugInfo(Resources.strDownloadingPatchFile2)
        Application.DoEvents()
        DownloadFile("http://download.pso2.jp/patch_prod/patches/patchlist.txt", "patchlist.txt")
        Helper.WriteDebugInfoSameLine(Resources.strDone)
        Helper.WriteDebugInfo(Resources.strDownloadingPatchFile3)
        Application.DoEvents()
        DownloadFile("http://download.pso2.jp/patch_prod/patches_old/patchlist.txt", "patchlist_old.txt")
        Helper.WriteDebugInfoSameLine(Resources.strDone)
        Helper.WriteDebugInfo(Resources.strDownloadingPatchFile4)
        Application.DoEvents()
        Program.Client.DownloadFile("http://arks-layer.com/vanila/version.txt", "version.ver")
        Helper.WriteDebugInfoSameLine(Resources.strDone)
        Application.DoEvents()
        UnlockGui()
        Helper.WriteDebugInfo(Resources.strCheckingForMissingOldFiles)
        _cancelledFull = False
        UpdatePso2(True)
    End Sub

End Class
