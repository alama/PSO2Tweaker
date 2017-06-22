Imports System.Threading
Imports System.IO
Imports System.Net
Imports System.Diagnostics
Imports System.Globalization
Imports DevComponents.DotNetBar
Imports PSO2_Tweaker.My

Public Class FrmOptions
    Dim _isLoading As Boolean = True

    Private Sub frmOptions_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            SuspendLayout()
            If (RegKey.GetValue(Of Integer)(RegKey.Color)) <> 0 Then ColorPickerButton1.SelectedColor = Color.FromArgb(RegKey.GetValue(Of Integer)(RegKey.Color))
            If (RegKey.GetValue(Of Integer)(RegKey.FontColor)) <> 0 Then ColorPickerButton2.SelectedColor = Color.FromArgb(RegKey.GetValue(Of Integer)(RegKey.FontColor))
            If (RegKey.GetValue(Of Integer)(RegKey.TextBoxBgColor)) <> 0 Then ColorPickerButton4.SelectedColor = Color.FromArgb(RegKey.GetValue(Of Integer)(RegKey.TextBoxBgColor))
            If (RegKey.GetValue(Of Integer)(RegKey.TextBoxColor)) <> 0 Then ColorPickerButton3.SelectedColor = Color.FromArgb(RegKey.GetValue(Of Integer)(RegKey.TextBoxColor))
            If (RegKey.GetValue(Of Integer)(RegKey.PBTextColor)) <> 0 Then ColorPickerButton5.SelectedColor = Color.FromArgb(RegKey.GetValue(Of Integer)(RegKey.PBTextColor))
            If (RegKey.GetValue(Of Integer)(RegKey.PBFill1)) <> 0 Then cpbFill1.SelectedColor = Color.FromArgb(RegKey.GetValue(Of Integer)(RegKey.PBFill1))
            If (RegKey.GetValue(Of Integer)(RegKey.PBFill2)) <> 0 Then cpbFill2.SelectedColor = Color.FromArgb(RegKey.GetValue(Of Integer)(RegKey.PBFill2))

            Dim backupMode = GetBackupMode(RegKey.Backup)

            If Not String.IsNullOrEmpty(backupMode) Then
                cmbBackupPreference.Text = backupMode
            End If

            Dim predownloadMode = GetPredownloadMode(RegKey.PreDownloadedRar)

            If Not String.IsNullOrEmpty(predownloadMode) Then
                cmbPredownload.Text = predownloadMode
            End If

            ' Checks if the DPI greater or equal to 120, and sets accordingly.
            ' Otherwise, we'll assume is 96 or lower.
            Using g As Graphics = CreateGraphics()
                If g.DpiX >= 120 Then
                    Size = New Size(814, 495)
                    'Size = New Size(543, 476)
                Else
                    'Size = New Size(400, 373)
                    Size = New Size(600, 392)
                End If
            End Using

            ' Here we pull the locale setting from registry and apply it to the form.
            ' Reads locale from registry and converts from LangCode (e.g "en") to Language (e.g "English")
            Try
                Dim locale As Language = DirectCast([Enum].Parse(GetType(LangCode), RegKey.GetValue(Of String)(RegKey.Locale)), Language)

                cmbLanguage.Text = locale.ToString()
                Thread.CurrentThread.CurrentUICulture = New CultureInfo(CType(locale, LangCode).ToString())
                Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture
            Catch ex As Exception
                cmbLanguage.Text = "English"
                Thread.CurrentThread.CurrentUICulture = Helper.DefaltCultureInfo
                Thread.CurrentThread.CurrentCulture = Helper.DefaltCultureInfo
            End Try

            LabelX1.Text = Resources.strChooseATheme
            LabelX2.Text = Resources.strChooseALanguage

            CheckBoxX1.Checked = Convert.ToBoolean(RegKey.GetValue(Of String)(RegKey.Pastebin))
            CheckBoxX2.Checked = Convert.ToBoolean(RegKey.GetValue(Of String)(RegKey.SteamMode))
            chkUseIcsHost.Checked = Convert.ToBoolean(RegKey.GetValue(Of String)(RegKey.UseIcsHost))

            CheckBoxX3.Checked = Convert.ToBoolean(RegKey.GetValue(Of String)(RegKey.ChecksVisible))
            CheckBoxX4.Checked = Convert.ToBoolean(RegKey.GetValue(Of String)(RegKey.PSO2DirVisible))
            CheckBoxX6.Checked = Convert.ToBoolean(RegKey.GetValue(Of String)(RegKey.UseOldProgressBar))
            CheckBoxX7.Checked = Convert.ToBoolean(RegKey.GetValue(Of String)(RegKey.LaunchPSO2fromORB))

            chkAutoRemoveCensor.Checked = Convert.ToBoolean(RegKey.GetValue(Of String)(RegKey.RemoveCensor))
            CMBStyle.Text = RegKey.GetValue(Of String)(RegKey.Style)

            ComboItem33.Text = "Last installed: " & RegKey.GetValue(Of String)(RegKey.StoryPatchVersion)
            ComboItem33.Text = "Latest version: " & RegKey.GetValue(Of String)(RegKey.NewStoryVersion)
            ComboItem35.Text = "Last installed: " & RegKey.GetValue(Of String)(RegKey.EnPatchVersion)
            ComboItem36.Text = "Latest version: " & RegKey.GetValue(Of String)(RegKey.NewEnVersion)
            ComboItem40.Text = "Last installed: " & RegKey.GetValue(Of String)(RegKey.LargeFilesVersion)
            ComboItem42.Text = "Latest version: " & RegKey.GetValue(Of String)(RegKey.NewLargeFilesVersion)
        Catch ex As Exception
            Helper.Log(ex.Message)
        Finally
            _isLoading = False
            ResumeLayout(False)
        End Try
    End Sub

    Private Shared Sub UpdateVersion(key As String, str As String)
        Dim value As String = str.Replace("Latest version: ", "").Replace("Last installed: ", "")
        RegKey.SetValue(Of String)(key, value)
        If value <> "" Then MsgBox("Selected value changed to: " & value)
    End Sub

    Private Sub frmOptions_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        SetLocale()
        CMBStyle.SelectedIndex = -1
    End Sub

    Private Sub SetLocale()
        Dim selectedLocale As String = [Enum].GetName(GetType(LangCode), cmbLanguage.SelectedIndex)

        If String.IsNullOrEmpty(selectedLocale) Then
            Thread.CurrentThread.CurrentUICulture = Helper.DefaltCultureInfo
            Thread.CurrentThread.CurrentCulture = Helper.DefaltCultureInfo
            RegKey.SetValue(Of String)(RegKey.Locale, "en")
        Else
            Thread.CurrentThread.CurrentUICulture = New CultureInfo(selectedLocale)
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture
            RegKey.SetValue(Of String)(RegKey.Locale, selectedLocale)
        End If
    End Sub

    Private Shared Function GetBackupMode(key As String) As String
        Dim value As String = RegKey.GetValue(Of String)(key)
        Select Case value
            Case "Ask"
                Return "Ask every time"
            Case "Always"
                Return "Always backup"
            Case "Never"
                Return "Never backup"
            Case Else
                Return Nothing
        End Select
    End Function

    Private Shared Function GetPredownloadMode(key As String) As String
        Dim value As String = RegKey.GetValue(Of String)(key)
        Select Case value
            Case "Ask"
                Return "Ask each time"
            Case "Never"
                Return "Never ask"
            Case Else
                Return Nothing
        End Select
    End Function

    Private Shared Sub ButtonX1_Click(sender As Object, e As EventArgs)
        Process.Start("http://arks-layer.com/credits.php")
    End Sub

    Private Sub cmbENOverride_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbENOverride.SelectedIndexChanged
        UpdateVersion(RegKey.EnPatchVersion, cmbENOverride.Text)
    End Sub

    Private Sub cmbLargeFilesOverride_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbLargeFilesOverride.SelectedIndexChanged
        UpdateVersion(RegKey.LargeFilesVersion, cmbLargeFilesOverride.Text)
    End Sub

    Private Sub cmbStoryOverride_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbStoryOverride.SelectedIndexChanged
        UpdateVersion(RegKey.StoryPatchVersion, cmbStoryOverride.Text)
    End Sub

    Private Sub ColorPickerButton4_SelectedColorChanged(sender As Object, e As EventArgs) Handles ColorPickerButton4.SelectedColorChanged
        Program.MainForm.rtbDebug.BackColor = ColorPickerButton4.SelectedColor
        RegKey.SetValue(Of Integer)(RegKey.TextBoxBgColor, (ColorPickerButton4.SelectedColor.ToArgb))
    End Sub

    Private Sub chkAutoRemoveCensor_CheckedChanged(sender As Object, e As EventArgs) Handles chkAutoRemoveCensor.CheckedChanged
        RegKey.SetValue(Of Boolean)(RegKey.RemoveCensor, chkAutoRemoveCensor.Checked)
    End Sub

    Private Sub chkUseIcsHost_CheckedChanged(sender As Object, e As EventArgs) Handles chkUseIcsHost.CheckedChanged
        RegKey.SetValue(Of Boolean)(RegKey.UseIcsHost, chkUseIcsHost.Checked)

        If Not _isLoading Then
            If chkUseIcsHost.Checked Then
                MsgBox("Please only check this value if you know that it's supposed to be checked, or someone trying to help you in the PSO2Proxy channel has told you to. Otherwise, you could break things, and then you'd be no better than ACF!")
                Program.HostsFilePath = Environment.SystemDirectory & "\drivers\etc\HOSTS.ics"
            Else
                Program.HostsFilePath = Environment.SystemDirectory & "\drivers\etc\HOSTS"
            End If
        End If
    End Sub

    Private Sub ColorPickerButton3_SelectedColorChanged(sender As Object, e As EventArgs) Handles ColorPickerButton3.SelectedColorChanged
        Program.MainForm.rtbDebug.ForeColor = ColorPickerButton3.SelectedColor
        RegKey.SetValue(Of Integer)(RegKey.TextBoxColor, (ColorPickerButton3.SelectedColor.ToArgb))
    End Sub

    Private Sub ColorPickerButton1_SelectedColorChanged(sender As Object, e As EventArgs) Handles ColorPickerButton1.SelectedColorChanged
        StyleManager.ColorTint = ColorPickerButton1.SelectedColor
        RegKey.SetValue(Of Integer)(RegKey.Color, (ColorPickerButton1.SelectedColor.ToArgb))
    End Sub

    Private Sub CheckBoxX1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxX1.CheckedChanged
        If Not CheckBoxX1.Checked Then
            MsgBox("PLEASE BE CAUTIOUS - If you turn this function off, the program will not automatically upload your logfile to pastebin, so you can report the bug to AIDA. This means that you'll need to provide the logfile yourself, or the likelyhood of your issue being resolved is very, very, slim.")
        End If
        RegKey.SetValue(Of String)(RegKey.Pastebin, CheckBoxX1.Checked.ToString())
    End Sub

    Private Sub cmbPredownload_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbPredownload.SelectedIndexChanged
        If cmbPredownload.SelectedIndex = 0 Then
            RegKey.SetValue(Of String)(RegKey.PreDownloadedRar, "Ask")
        ElseIf cmbPredownload.SelectedIndex = 1 Then
            RegKey.SetValue(Of String)(RegKey.PreDownloadedRar, "Never")
        End If
    End Sub

    Private Shared Sub btnPSO2Override_Click(sender As Object, e As EventArgs) Handles btnPSO2Override.Click
        Dim yesNo As MsgBoxResult = MsgBox("This will tell the Tweaker you have the latest version of PSO2 installed - Be aware that this cannot be undone, and should only be used if you update the game outside of the Tweaker. Do you want to continue?", vbYesNo)

        If yesNo = vbYes Then
            Dim lines3 = File.ReadAllLines("version.ver")
            Dim remoteVersion3 As String = lines3(0)
            RegKey.SetValue(Of String)(RegKey.Pso2RemoteVersion, remoteVersion3)
            MsgBox("PSO2 Installed version set to: " & remoteVersion3)
        End If
    End Sub

    Private Sub cmbBackupPreference_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbBackupPreference.SelectedIndexChanged
        Select Case cmbBackupPreference.SelectedIndex
            Case 0
                RegKey.SetValue(Of String)(RegKey.Backup, "Ask")
            Case 1
                RegKey.SetValue(Of String)(RegKey.Backup, "Always")
            Case 2
                RegKey.SetValue(Of String)(RegKey.Backup, "Never")
            Case Else
                RegKey.SetValue(Of String)(RegKey.Backup, "Ask")
        End Select
    End Sub

    Private Sub cmbLanguage_SelectedValueChanged(sender As Object, e As EventArgs) Handles cmbLanguage.SelectedValueChanged
        Using downloadClient As New WebClient
            downloadClient.DownloadFile(New Uri(Program.FreedomUrl & "LanguagePack.rar"), "LanguagePack.rar")
        End Using

        Dim processStartInfo = New ProcessStartInfo()
        processStartInfo.FileName = (Program.StartPath & "\unrar.exe").Replace("\\", "\")
        processStartInfo.Verb = "runas"
        processStartInfo.Arguments = "x -inul -o+ LanguagePack.rar"
        processStartInfo.WindowStyle = ProcessWindowStyle.Hidden
        processStartInfo.UseShellExecute = True

        Process.Start(processStartInfo).WaitForExit()
        SetLocale()
    End Sub

    Private Sub ColorPickerButton2_SelectedColorChanged(sender As Object, e As EventArgs) Handles ColorPickerButton2.SelectedColorChanged
        Program.MainForm.ForeColor = ColorPickerButton2.SelectedColor
        FrmPso2Options.ForeColor = ColorPickerButton2.SelectedColor
        FrmPso2Options.TabItem1.TextColor = ColorPickerButton2.SelectedColor
        FrmPso2Options.TabItem2.TextColor = ColorPickerButton2.SelectedColor
        FrmPso2Options.TabItem3.TextColor = ColorPickerButton2.SelectedColor
        FrmPso2Options.TabItem7.TextColor = ColorPickerButton2.SelectedColor
        ForeColor = ColorPickerButton2.SelectedColor
        CheckBoxX1.TextColor = ColorPickerButton2.SelectedColor
        chkAutoRemoveCensor.TextColor = ColorPickerButton2.SelectedColor
        chkUseIcsHost.TextColor = ColorPickerButton2.SelectedColor
        CheckBoxX2.TextColor = ColorPickerButton2.SelectedColor
        CheckBoxX3.TextColor = ColorPickerButton2.SelectedColor
        CheckBoxX4.TextColor = ColorPickerButton2.SelectedColor
        CheckBoxX6.TextColor = ColorPickerButton2.SelectedColor
        CheckBoxX7.TextColor = ColorPickerButton2.SelectedColor
        Program.MainForm.chkRemoveNVidia.TextColor = ColorPickerButton2.SelectedColor
        Program.MainForm.chkRemovePC.TextColor = ColorPickerButton2.SelectedColor
        Program.MainForm.chkRemoveSEGA.TextColor = ColorPickerButton2.SelectedColor
        Program.MainForm.chkRemoveVita.TextColor = ColorPickerButton2.SelectedColor
        Program.MainForm.chkRestoreNVidia.TextColor = ColorPickerButton2.SelectedColor
        Program.MainForm.chkRestorePC.TextColor = ColorPickerButton2.SelectedColor
        Program.MainForm.chkRestoreSEGA.TextColor = ColorPickerButton2.SelectedColor
        Program.MainForm.chkRestoreVita.TextColor = ColorPickerButton2.SelectedColor
        Program.MainForm.chkSwapOP.TextColor = ColorPickerButton2.SelectedColor
        Program.MainForm.lblStatus.ForeColor = ColorPickerButton2.SelectedColor

        RegKey.SetValue(Of Integer)(RegKey.FontColor, (ColorPickerButton2.SelectedColor.ToArgb))
    End Sub

    Private Sub CMBStyle_SelectedValueChanged(sender As Object, e As EventArgs) Handles CMBStyle.SelectedValueChanged
        If Not String.IsNullOrEmpty(CMBStyle.Text) Then

            '┻━┻ ︵ \(Ò_Ó \)
            '(╯°□°）╯︵ /(.□. \)
            '┯━┯ノ(º₋ºノ)

            Select Case CMBStyle.Text
                Case "Blue"
                    StyleManager.Style = eStyle.Office2007Blue
                    RegKey.SetValue(Of String)(RegKey.Style, CMBStyle.Text)

                Case "Silver"
                    StyleManager.Style = eStyle.Office2007Silver
                    RegKey.SetValue(Of String)(RegKey.Style, CMBStyle.Text)

                Case "Black"
                    StyleManager.Style = eStyle.Office2007Black
                    RegKey.SetValue(Of String)(RegKey.Style, CMBStyle.Text)

                Case "Vista Glass"
                    StyleManager.Style = eStyle.Office2007VistaGlass
                    RegKey.SetValue(Of String)(RegKey.Style, CMBStyle.Text)

                Case "2010 Silver"
                    StyleManager.Style = eStyle.Office2010Silver
                    RegKey.SetValue(Of String)(RegKey.Style, CMBStyle.Text)

                Case "Windows 7 Blue"
                    StyleManager.Style = eStyle.Windows7Blue
                    RegKey.SetValue(Of String)(RegKey.Style, CMBStyle.Text)

                Case Else
                    StyleManager.Style = eStyle.Office2007Black
                    RegKey.SetValue(Of String)(RegKey.Style, "Black")
            End Select
        End If
    End Sub

    Private Sub CheckBoxX2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxX2.CheckedChanged
        MsgBox("Please note that steam mode will NOT work while GN Field is required. Blame GameGuard, sorry!")
        RegKey.SetValue(Of String)(RegKey.SteamMode, CheckBoxX2.Checked.ToString)
    End Sub

    Private Sub ButtonX2_Click(sender As Object, e As EventArgs) Handles ButtonX2.Click
        OpenFileDialog1.Title = "Select a picture"
        OpenFileDialog1.FileName = ""
        OpenFileDialog1.Filter = "All Files (*.*) |*.*"
        If OpenFileDialog1.ShowDialog() = DialogResult.Cancel Then
            If Not String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.ImageLocation)) Then
                Dim RemoveYesNo As MsgBoxResult = MsgBox("Would you like to remove the current picture?", MsgBoxStyle.YesNo)
                If RemoveYesNo = MsgBoxResult.Yes Then
                    FrmMain.BackgroundImage = Nothing
                    RegKey.SetValue(Of String)(RegKey.ImageLocation, "")
                    Exit Sub
                End If
                If RemoveYesNo = MsgBoxResult.No Then Return
            Else
                Return
            End If
        End If
        FrmMain.BackgroundImage = System.Drawing.Image.FromFile(OpenFileDialog1.FileName)

        RegKey.SetValue(Of String)(RegKey.ImageLocation, OpenFileDialog1.FileName)

    End Sub

    Private Sub btnChooseOrb_Click(sender As Object, e As EventArgs) Handles btnChooseOrb.Click
        OpenFileDialog1.Title = "Select a picture"
        OpenFileDialog1.FileName = ""
        OpenFileDialog1.Filter = "All Files (*.*) |*.*"
        If OpenFileDialog1.ShowDialog() = DialogResult.Cancel Then
            If Not String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.ORBLocation)) Then
                Dim RemoveYesNo As MsgBoxResult = MsgBox("Would you like to remove the current picture?", MsgBoxStyle.YesNo)
                If RemoveYesNo = MsgBoxResult.Yes Then
                    FrmMain.Office2007StartButton1.Image = Nothing
                    RegKey.SetValue(Of String)(RegKey.ORBLocation, "")
                    Exit Sub
                End If
                If RemoveYesNo = MsgBoxResult.No Then Return
            Else
                Return
            End If
        End If
        Dim bmp As New Bitmap(OpenFileDialog1.FileName)
        If bmp.Width > 50 Or bmp.Height > 50 Then
            MsgBox("Picture must be 50 x 50 or less!")
            Exit Sub
        End If
        FrmMain.Office2007StartButton1.Image = System.Drawing.Image.FromFile(OpenFileDialog1.FileName)

        RegKey.SetValue(Of String)(RegKey.ORBLocation, OpenFileDialog1.FileName)
    End Sub

    Private Sub CheckBoxX3_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxX3.CheckedChanged
        FrmMain.chkRemovePC.Visible = CheckBoxX3.Checked
        FrmMain.chkRestorePC.Visible = CheckBoxX3.Checked
        FrmMain.chkRemoveVita.Visible = CheckBoxX3.Checked
        FrmMain.chkRestoreVita.Visible = CheckBoxX3.Checked
        FrmMain.chkRemoveNVidia.Visible = CheckBoxX3.Checked
        FrmMain.chkRestoreNVidia.Visible = CheckBoxX3.Checked
        FrmMain.chkRemoveSEGA.Visible = CheckBoxX3.Checked
        FrmMain.chkRestoreSEGA.Visible = CheckBoxX3.Checked
        FrmMain.btnApplyChanges.Visible = CheckBoxX3.Checked
        RegKey.SetValue(Of String)(RegKey.ChecksVisible, CheckBoxX3.Checked.ToString)
        '254, 219 - Default location for launch PSO2
    End Sub

    Private Sub CheckBoxX4_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxX4.CheckedChanged
        FrmMain.lblDirectory.Visible = CheckBoxX4.Checked
        FrmMain.lblDirectoryLabel.Visible = CheckBoxX4.Checked
        RegKey.SetValue(Of String)(RegKey.PSO2DirVisible, CheckBoxX4.Checked.ToString)
    End Sub

    Private Sub ColorPickerButton5_SelectedColorChanged(sender As Object, e As EventArgs) Handles ColorPickerButton5.SelectedColorChanged
        RegKey.SetValue(Of Integer)(RegKey.PBTextColor, ColorPickerButton5.SelectedColor.ToArgb)
        FrmMain.PBMainBar.BackgroundStyle.TextColor = ColorPickerButton5.SelectedColor
    End Sub

    Private Sub cpbFill1_SelectedColorChanged(sender As Object, e As EventArgs) Handles cpbFill1.SelectedColorChanged
        RegKey.SetValue(Of Integer)(RegKey.PBFill1, cpbFill1.SelectedColor.ToArgb)
        FrmMain.PBMainBar.ChunkColor = cpbFill1.SelectedColor
    End Sub

    Private Sub cpbFill2_SelectedColorChanged(sender As Object, e As EventArgs) Handles cpbFill2.SelectedColorChanged
        RegKey.SetValue(Of Integer)(RegKey.PBFill2, cpbFill2.SelectedColor.ToArgb)
        FrmMain.PBMainBar.ChunkColor2 = cpbFill2.SelectedColor
    End Sub

    Private Sub CheckBoxX6_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxX6.CheckedChanged
        RegKey.SetValue(Of Boolean)(RegKey.UseOldProgressBar, CheckBoxX6.Checked)
        If CheckBoxX6.Checked = True Then
            FrmMain.PBMainBar.Style = eDotNetBarStyle.Office2007
            FrmMain.PBMainBar.ForeColor = Color.Black
            FrmMain.PBMainBar.BackgroundStyle.TextColor = Color.Black
            Exit Sub
        End If
        If CheckBoxX6.Checked = False Then FrmMain.PBMainBar.Style = eDotNetBarStyle.Office2000
    End Sub

    Private Sub CheckBoxX7_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxX7.CheckedChanged
        FrmMain.btnLaunchPSO2fromORB.Visible = CheckBoxX7.Checked
        RegKey.SetValue(Of Boolean)(RegKey.LaunchPSO2fromORB, CheckBoxX7.Checked)
        If FrmMain.btnLaunchPSO2fromORB.Visible = True Then FrmMain.btnLaunchPSO2.Visible = False
        If FrmMain.btnLaunchPSO2fromORB.Visible = False Then FrmMain.btnLaunchPSO2.Visible = True
    End Sub

    Private Sub LabelX8_Click(sender As Object, e As EventArgs) Handles LabelX8.Click

    End Sub

    Private Sub ButtonX1_Click_1(sender As Object, e As EventArgs) Handles ButtonX1.Click
        Exit Sub
        Dim EnableBetaYesNo As MsgBoxResult = MsgBox("Do you want to enable Tweaker beta testing? This will usually include new features, but could be unstable. You can change this at any time. Changes will take effect when you restart the Tweaker.", vbYesNo)
        If EnableBetaYesNo = vbYes Then
            RegKey.SetValue(Of Boolean)(RegKey.EnableBeta, True)
            If File.Exists("client.json") = True Then Helper.DeleteFile("client.json")
            Helper.WriteDebugInfo("Tweaker beta testing enabled!")

        Else
            RegKey.SetValue(Of Boolean)(RegKey.EnableBeta, False)
            Helper.WriteDebugInfo("Tweaker beta testing disabled!")
        End If
    End Sub
End Class
