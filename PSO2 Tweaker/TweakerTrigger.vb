Imports System.IO
Imports System.Net
Imports ArksLayer.Tweaker.Abstractions
Imports PSO2_Tweaker.My

Public Class TweakerTrigger
    Implements ITrigger

#Region "AIDA_CUSTOM"
    Dim _downloadedfilecount As Integer = 0
    Dim patchfilecount As Integer = 0
    Dim TotalDownloadedQuantum As Long
    Dim DoneDownloading As Boolean = False
    Dim SeenMessage As Boolean = False
    Public Shared patchwriter As TextWriter = TextWriter.Synchronized(File.AppendText("patchlog.txt"))

    Public Sub WritePatchLog(s As String)
        If frmDownloader.Visible = True Then patchwriter.WriteLine(DateTime.Now.ToString("G") & " " & s)
    End Sub
#End Region

    Public Sub AppendLog(s As String) Implements ITrigger.AppendLog
        'Helper.WriteDebugInfo(s)
        If s.Contains("Downloading a file from") Then s = s.Replace("Downloading a file from ", "Downloading ")
        WritePatchLog(s.Replace("http://download.pso2.jp/patch_prod/patches/data/win32/", "").Replace("http://download.pso2.jp/patch_prod/patches_old/data/win32/", "").Replace(".pat", "").Replace("data/win32/", "data\win32\"))
    End Sub

    Public Sub IfUpdateNotNeeded() Implements ITrigger.IfUpdateNotNeeded
        FrmMain.UnlockGui()
        Helper.WriteDebugInfo("Your game appears to be up-to-date! If you believe this is incorrect, please click Troubleshooting -> Check for Old/Missing Files to do a full filecheck instead of a fast check.")
    End Sub

    Public Sub OnBackupRestore(backupFiles As IEnumerable(Of String)) Implements ITrigger.OnBackupRestore
        Helper.WriteDebugInfo($"Found {backupFiles.Count()} backup files. Restoring them...")
    End Sub

    Public Sub OnCensorRemoval() Implements ITrigger.OnCensorRemoval
        ' This is deprecated and will be no longer called!
    End Sub

    Public Sub OnClientHashReadFailed() Implements ITrigger.OnClientHashReadFailed
        Helper.WriteDebugInfo("Couldn't find previous update data, starting from scratch...")
    End Sub

    Public Sub OnClientHashReadSuccess() Implements ITrigger.OnClientHashReadSuccess
        Helper.WriteDebugInfo("Found previous update data!")
    End Sub

    Public Sub OnDownloadAborted(url As String) Implements ITrigger.OnDownloadAborted
        WritePatchLog("Download aborted for " & url.Replace("http://download.pso2.jp/patch_prod/patches/data/win32/", "").Replace("http://download.pso2.jp/patch_prod/patches_old/data/win32/", "").Replace("http://download.pso2.jp/patch_prod/patches/", "").Replace(".pat", "") & "!")
        Helper.WriteDebugInfoAndWarning("Download aborted for " & url.Replace("http://download.pso2.jp/patch_prod/patches/data/win32/", "").Replace("http://download.pso2.jp/patch_prod/patches_old/data/win32/", "").Replace("http://download.pso2.jp/patch_prod/patches/", "").Replace(".pat", "") & "!")
        patchfilecount -= 1
        'Throw New NotImplementedException()
    End Sub

    Public Sub OnDownloadFinish(url As String) Implements ITrigger.OnDownloadFinish
        Try
            If frmDownloader.ProgressBarX1.Text = url Then
                frmDownloader.ProgressBarX1.Text = ""
            End If
            If frmDownloader.ProgressBarX2.Text = url Then
                frmDownloader.ProgressBarX2.Text = ""
            End If
            If frmDownloader.ProgressBarX3.Text = url Then
                frmDownloader.ProgressBarX3.Text = ""
            End If
            If frmDownloader.ProgressBarX4.Text = url Then
                frmDownloader.ProgressBarX4.Text = ""
            End If
            If frmDownloader.ProgressBarX5.Text = url Then
                frmDownloader.ProgressBarX5.Text = ""
            End If
            If frmDownloader.ProgressBarX6.Text = url Then
                frmDownloader.ProgressBarX6.Text = ""
            End If

            WritePatchLog("Download complete - " & url.Replace("http://download.pso2.jp/patch_prod/patches/data/win32/", "").Replace("http://download.pso2.jp/patch_prod/patches_old/data/win32/", "").Replace("http://download.pso2.jp/patch_prod/patches/", "").Replace("http://download.pso2.jp/patch_prod/patches_old/", "").Replace(".pat", "") & "!")
            _downloadedfilecount += 1
            If url.Contains(".pat") And url.Contains("exe") = False Then
                If File.Exists(Program.Pso2WinDir & "\" & url.Replace("http://download.pso2.jp/patch_prod/patches/data/win32/", "").Replace("http://download.pso2.jp/patch_prod/patches_old/data/win32/", "").Replace("http://download.pso2.jp/patch_prod/patches/", "").Replace(".pat", "")) = True Then
                    TotalDownloadedQuantum += FileLen(Program.Pso2WinDir & "\" & url.Replace("http://download.pso2.jp/patch_prod/patches/data/win32/", "").Replace("http://download.pso2.jp/patch_prod/patches_old/data/win32/", "").Replace("http://download.pso2.jp/patch_prod/patches/", "").Replace(".pat", ""))
                Else
                    WritePatchLog("Warning, couldn't find downloaded file: " & Program.Pso2WinDir & "\" & url.Replace("http://download.pso2.jp/patch_prod/patches/data/win32/", "").Replace("http://download.pso2.jp/patch_prod/patches_old/data/win32/", "").Replace("http://download.pso2.jp/patch_prod/patches/", "").Replace(".pat", ""))
                End If

            End If
            If url.Contains(".exe") Then TotalDownloadedQuantum += FileLen(Program.Pso2RootDir & "\" & url.Replace("http://download.pso2.jp/patch_prod/patches/data/win32/", "").Replace("http://download.pso2.jp/patch_prod/patches_old/data/win32/", "").Replace(".pat", "").Replace("http://download.pso2.jp/patch_prod/patches/", ""))
            'FrmMain.lblStatus.Text = "Downloaded " & _downloadedfilecount & " files"
            'Throw New NotImplementedException()
        Catch ex As Exception
            Helper.LogWithException(Resources.strERROR, ex)
        End Try
    End Sub

    Public Sub OnDownloadRetry(url As String, delaySecond As Integer) Implements ITrigger.OnDownloadRetry
        WritePatchLog("Retrying download for " & url.Replace("http://download.pso2.jp/patch_prod/patches/data/win32/", "").Replace("http://download.pso2.jp/patch_prod/patches_old/data/win32/", "").Replace("http://download.pso2.jp/patch_prod/patches/", "").Replace(".pat", ""))
        'Throw New NotImplementedException()
    End Sub

    Public Sub OnDownloadStart(url As String, client As WebClient) Implements ITrigger.OnDownloadStart
        Try
            If DoneDownloading = True Then Exit Sub
            If url.Contains(".txt") Or url.Contains(".ver") Then
                If patchfilecount > 1 Then patchfilecount -= 1
                'client.CancelAsync()
                ' If patchfilecount = 0 Then
                ' DoneDownloading = True
                ' frmDownloader.Hide()
                ' FrmMain.FinalUpdateSteps()
                ' Thread.Sleep(10000)
                ' frmDownloader.Close()
            End If
            ' End If

            If patchfilecount > 0 Then frmDownloader.Show()
            'Dim lastprogress As Long
            Dim CaptureBar As Boolean = False
            Dim Filename As String
            AddHandler client.DownloadProgressChanged,
                Sub(o, e)
                    Try
                        If DoneDownloading = True Then Exit Sub
                        Filename = url.Replace("http://download.pso2.jp/patch_prod/patches/data/win32/", "").Replace("http://download.pso2.jp/patch_prod/patches_old/data/win32/", "").Replace("http://download.pso2.jp/patch_prod/patches/", "").Replace(".pat", "")

                        If frmDownloader.ProgressBarX1.Text = "" And CaptureBar = False Then
                            frmDownloader.ProgressBarX1.Text = url
                            CaptureBar = True
                        End If
                        If frmDownloader.ProgressBarX2.Text = "" And CaptureBar = False Then
                            frmDownloader.ProgressBarX2.Text = url
                            CaptureBar = True
                        End If
                        If frmDownloader.ProgressBarX3.Text = "" And CaptureBar = False Then
                            frmDownloader.ProgressBarX3.Text = url
                            CaptureBar = True
                        End If
                        If frmDownloader.ProgressBarX4.Text = "" And CaptureBar = False Then
                            frmDownloader.ProgressBarX4.Text = url
                            CaptureBar = True
                        End If
                        If frmDownloader.ProgressBarX5.Text = "" And CaptureBar = False Then
                            frmDownloader.ProgressBarX5.Text = url
                            CaptureBar = True
                        End If
                        If frmDownloader.ProgressBarX6.Text = "" And CaptureBar = False Then
                            frmDownloader.ProgressBarX6.Text = url
                            CaptureBar = True
                        End If

                        'Do something here
                        'If lastprogress = e.BytesReceived Then
                        ' Return
                        ' End If
                        '
                        'lastprogress = e.BytesReceived
                        '
                        Dim percentage As Integer = 0
                        'Dim percentage = String.Format("{0:N2}%", Math.Truncate(e.BytesReceived / CDbl(e.TotalBytesToReceive) * 100 * 100) / 100)
                        'Dim s = "DOWNLOADING {url} | {e.BytesReceived / 1024} KB out of {e.TotalBytesToReceive / 1024} KB | {percentage}"
                        'MsgBox(s)

                        If frmDownloader.ProgressBarX1.Text = url Then
                            percentage = CInt(Math.Truncate(e.BytesReceived / CDbl(e.TotalBytesToReceive) * 100 * 100) / 100)
                            frmDownloader.ProgressBarX1.Value = percentage
                            frmDownloader.LabelX1.Text = "Downloading " & Filename & " (" & String.Format("{0:N2}%", Math.Truncate(e.BytesReceived / CDbl(e.TotalBytesToReceive) * 100 * 100) / 100) & ")"
                            If frmDownloader.LabelX1.Text.Contains("version.ver") Then frmDownloader.LabelX1.Text = "Building list of files to download, please wait..."
                        End If
                        If frmDownloader.ProgressBarX2.Text = url Then
                            percentage = CInt(Math.Truncate(e.BytesReceived / CDbl(e.TotalBytesToReceive) * 100 * 100) / 100)
                            frmDownloader.ProgressBarX2.Value = percentage
                            frmDownloader.LabelX2.Text = "Downloading " & Filename & " (" & String.Format("{0:N2}%", Math.Truncate(e.BytesReceived / CDbl(e.TotalBytesToReceive) * 100 * 100) / 100) & ")"
                        End If
                        If frmDownloader.ProgressBarX3.Text = url Then
                            percentage = CInt(Math.Truncate(e.BytesReceived / CDbl(e.TotalBytesToReceive) * 100 * 100) / 100)
                            frmDownloader.ProgressBarX3.Value = percentage
                            frmDownloader.LabelX3.Text = "Downloading " & Filename & " (" & String.Format("{0:N2}%", Math.Truncate(e.BytesReceived / CDbl(e.TotalBytesToReceive) * 100 * 100) / 100) & ")"
                        End If
                        If frmDownloader.ProgressBarX4.Text = url Then
                            percentage = CInt(Math.Truncate(e.BytesReceived / CDbl(e.TotalBytesToReceive) * 100 * 100) / 100)
                            frmDownloader.ProgressBarX4.Value = percentage
                            frmDownloader.LabelX4.Text = "Downloading " & Filename & " (" & String.Format("{0:N2}%", Math.Truncate(e.BytesReceived / CDbl(e.TotalBytesToReceive) * 100 * 100) / 100) & ")"
                        End If
                        If frmDownloader.ProgressBarX5.Text = url Then
                            percentage = CInt(Math.Truncate(e.BytesReceived / CDbl(e.TotalBytesToReceive) * 100 * 100) / 100)
                            frmDownloader.ProgressBarX5.Value = percentage
                            frmDownloader.LabelX5.Text = "Downloading " & Filename & " (" & String.Format("{0:N2}%", Math.Truncate(e.BytesReceived / CDbl(e.TotalBytesToReceive) * 100 * 100) / 100) & ")"
                        End If
                        If frmDownloader.ProgressBarX6.Text = url Then
                            percentage = CInt(Math.Truncate(e.BytesReceived / CDbl(e.TotalBytesToReceive) * 100 * 100) / 100)
                            frmDownloader.ProgressBarX6.Value = percentage
                            frmDownloader.LabelX6.Text = "Downloading " & Filename & " (" & String.Format("{0:N2}%", Math.Truncate(e.BytesReceived / CDbl(e.TotalBytesToReceive) * 100 * 100) / 100) & ")"
                        End If

                        frmDownloader.lblTotal.Text = "Total amount downloaded: " & Helper.SizeSuffix(TotalDownloadedQuantum) & vbCrLf & "Total files downloaded: " & _downloadedfilecount & vbCrLf & "Files left to download: " & patchfilecount - _downloadedfilecount & "/" & patchfilecount
                        If patchfilecount - _downloadedfilecount = 1 And frmDownloader.Visible = True Then
                            DoneDownloading = True
                            patchwriter.Flush()
                            frmDownloader.Hide()
                            FrmMain.FinalUpdateSteps()
                        End If
                    Catch ex As Exception
                        Helper.LogWithException(Resources.strERROR, ex)
                    End Try
                End Sub
        Catch ex As Exception
            Helper.LogWithException(Resources.strERROR, ex)
        End Try
    End Sub

    Public Sub OnHashComplete() Implements ITrigger.OnHashComplete
        Helper.WriteDebugInfo("Game hashing successful!")
    End Sub

    Public Sub OnHashProgress(progress As Integer, total As Integer) Implements ITrigger.OnHashProgress
        Try
            Dim progressvalue As Integer = CInt((progress * 100 / total))
            If total - progress > 0 And total - progress < 10 And SeenMessage = False Then
                Helper.WriteDebugInfo("These last few files might take a bit, please wait...")
                SeenMessage = True
            End If
            If total - progress > 0 And total - progress < 5 Then
                FrmMain.PBMainBar.Text = ("Checked " & progress & " / " & total & " (99.99%) - " & Resources.strRightClickforOptions)
                FrmMain.PBMainBar.Value = 99
            Else
                FrmMain.PBMainBar.Text = ("Checked " & progress & " / " & total & " (" & Format((progress * 100 / total), "00.00") & "%) - " & Resources.strRightClickforOptions)
                FrmMain.PBMainBar.Value = progressvalue
            End If
        Catch ex As Exception
            Helper.LogWithException(Resources.strERROR, ex)
        End Try
        'FrmMain.lblStatus.Text = progress & " out of " & total & " files hashed."
    End Sub

    Public Sub OnHashStart(files As IEnumerable(Of String)) Implements ITrigger.OnHashStart
        Helper.WriteDebugInfo("Beginning QUANTUM SYSTEM update check...")
        _downloadedfilecount = 0
    End Sub

    Public Sub OnHousekeeping() Implements ITrigger.OnHousekeeping
        Helper.WriteDebugInfo("Cleaning up...")
    End Sub

    Public Sub OnLegacyFilesFound(legacyFiles As IEnumerable(Of String)) Implements ITrigger.OnLegacyFilesFound
        Helper.WriteDebugInfo($"Destroying {legacyFiles.Count()} legacy files...")
        For Each file In legacyFiles
            AppendLog("LEGACY : " + file)
        Next
    End Sub

    Public Sub OnLegacyFilesNotFound() Implements ITrigger.OnLegacyFilesNotFound
        Helper.WriteDebugInfo("No legacy files found.")
    End Sub

    Public Sub OnLegacyFilesScanning() Implements ITrigger.OnLegacyFilesScanning
        Helper.WriteDebugInfo("Searching for legacy files...")
    End Sub

    Public Sub OnMissingFilesDiscovery(missingFiles As IEnumerable(Of String)) Implements ITrigger.OnMissingFilesDiscovery
        Helper.WriteDebugInfo($"Discovered {missingFiles.Count()} missing or changed files.")
    End Sub

    Public Sub OnPatchingFailed(failCount As Integer) Implements ITrigger.OnPatchingFailed
        Helper.WriteDebugInfo($"Failed to download {failCount} files!")
    End Sub

    Public Sub OnPatchingResume(missingFiles As IEnumerable(Of String)) Implements ITrigger.OnPatchingResume
        Helper.WriteDebugInfo($"Resuming patching {missingFiles.Count()} files!")
    End Sub

    Public Sub OnPatchingStart(fileCount As Integer) Implements ITrigger.OnPatchingStart
        patchfilecount = fileCount
    End Sub

    Public Sub OnPatchingSuccess() Implements ITrigger.OnPatchingSuccess
        Helper.WriteDebugInfo("Successfully downloaded all files!")
        'MsgBox("Test")
        If frmDownloader.Visible = True Then
            frmDownloader.Close()
            FrmMain.FinalUpdateSteps()
        End If
    End Sub

    Public Sub OnPatchlistFetchCompleted(count As Integer) Implements ITrigger.OnPatchlistFetchCompleted
        Helper.WriteDebugInfo($"Patchlist contains {count} entries.")
    End Sub

    Public Sub OnPatchlistFetchStart() Implements ITrigger.OnPatchlistFetchStart
        Helper.WriteDebugInfo("Checking for PSO2 updates...")
    End Sub

    Public Sub OnUpdateCompleted() Implements ITrigger.OnUpdateCompleted
        Helper.WriteDebugInfo("Update was successful.")
    End Sub

    Public Sub OnUpdateStart(rehash As Boolean) Implements ITrigger.OnUpdateStart
        Helper.WriteDebugInfo("Update started.")
    End Sub
End Class
