Imports System.Globalization
Imports System.IO
Imports System.Security.Principal
Imports System.Threading
Imports System.Net
Imports PSO2_Tweaker.VEDA

Namespace My
    Public Class Program
        Public Shared ReadOnly Args As String() = Environment.GetCommandLineArgs()
        Public Shared ReadOnly StartPath As String = Windows.Forms.Application.StartupPath
        Public Shared ReadOnly Client As MyWebClient = New MyWebClient() With {.Timeout = 10000, .Proxy = Nothing}
        Public Shared ReadOnly AreYouAlive As MyWebClient = New MyWebClient() With {.Timeout = 5000, .Proxy = Nothing}
        Public Shared ReadOnly ItemPatchClient As MyWebClient = New MyWebClient() With {.Timeout = 10000, .Proxy = Nothing}
        Public Shared ReadOnly Client2 As MyWebClient = New MyWebClient() With {.Timeout = 10000, .Proxy = Nothing}

        Public Shared MainForm As FrmMain
        Public Shared FreedomUrl As String = "http://108.61.203.33/freedom/"
        Public Shared HostsFilePath As String
        Public Shared Pso2RootDir As String
        Public Shared Pso2WinDir As String
        Public Shared NoGNFieldMode As Boolean = False
        Public Shared CloseMe As Boolean = False
        Public Shared GNFieldActive As Boolean = False
        Public Shared ELSActive As Boolean = False
        Public Shared UseItemTranslation As Boolean = False
        Public Shared WayuIsAFailure As Boolean = False
        Public Shared Nodiag As Boolean = False
        Public Shared IsPso2Installed As Boolean = True
        Public Shared SidebarEnabled As Boolean = True
        Public Shared IsMainFormTopMost As Boolean = False
        Public Shared transOverride As Boolean = False

        Public Shared Sub Main()

            Try
                Client.Headers("user-agent") = GetUserAgent()

                Helper.Log("Checking if the PSO2 Tweaker is running")

                If Helper.CheckIfRunning("PSO2 Tweaker") Then Environment.Exit(0)

                If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.Pso2Dir)) Then
                    Dim alreadyInstalled As MsgBoxResult = MsgBox("This appears to be the first time you've used the PSO2 Tweaker! Have you installed PSO2 already? If you select no, the PSO2 Tweaker will install it for you.", MsgBoxStyle.YesNo)
                    If alreadyInstalled = vbNo Then
                        IsPso2Installed = False
                        Return
                    End If
                End If

                Dim locale = RegKey.GetValue(Of String)(RegKey.Locale)

                If Not String.IsNullOrEmpty(locale) Then
                    Thread.CurrentThread.CurrentUICulture = New CultureInfo(locale)
                    Thread.CurrentThread.CurrentCulture = New CultureInfo(locale)
                End If

                Helper.Log("Program started! - Logging enabled!")
                Helper.Log("Attempting to auto-load pso2_bin directory from settings")
                If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.Pso2Dir)) Then
                    MsgBox(Resources.strPleaseSelectwin32Dir)
                    Helper.SelectPso2Directory()
                Else
                    Pso2RootDir = RegKey.GetValue(Of String)(RegKey.Pso2Dir)
                    Helper.Log("Loaded pso2_bin directory from settings")
                End If

                ' This sets up pso2RootDir and pso2WinDir - don't remove it
                If Pso2RootDir.Contains("\pso2_bin\data\win32") Then
                    If File.Exists(Pso2RootDir.Replace("\data\win32", "") & "\pso2.exe") Then
                        Helper.Log("win32 folder selected instead of pso2_bin folder - Fixing!")
                        Pso2RootDir = Pso2RootDir.Replace("\data\win32", "")
                        RegKey.SetValue(Of String)(RegKey.Pso2Dir, Pso2RootDir)
                        Helper.Log(Pso2RootDir & " " & Resources.strSetAsYourPSO2)
                    End If
                End If

                If Pso2RootDir = "lblDirectory" OrElse Not Directory.Exists(Pso2RootDir) Then
                    MsgBox(Resources.strPleaseSelectwin32Dir)
                    Helper.SelectPso2Directory()
                End If

                Pso2WinDir = (Pso2RootDir & "\data\win32")

                If Not Directory.Exists(Pso2WinDir) Then
                    Directory.CreateDirectory(Pso2WinDir)
                    'WriteDebugInfo("Creating win32 directory... Done!")
                End If

                Helper.Log("Pursuing freedom...")
                Dim TestURL As String = "http://arks-layer.com/freedom.txt"
                Try
                    TestURL = Client.DownloadString("http://arks-layer.com/freedom.txt")

                    'Re-Enable this if we do BETAs again
                    'If RegKey.GetValue(Of Boolean)(RegKey.EnableBeta) = True Then TestURL = TestURL.Replace("freedom/", "freedombeta/")

                    If Not TestURL.Contains("freedom") Then
                        Helper.Log("Reverting to default freedom...")
                        FreedomUrl = "http://108.61.203.33/freedom/"
                    Else
                        FreedomUrl = TestURL
                    End If

                Catch ex As Exception
                    Helper.Log("Reverting to default freedom...")
                    FreedomUrl = "http://108.61.203.33/freedom/"
                End Try

                Dim launchPso2 As Boolean = False

                For i As Integer = 1 To (Args.Length - 1)
                    Try
                        Select Case Args(i)
                            Case "-nodllcheck"
                                transOverride = True

                            Case "-fuck_you_misaki_stop_trying_to_decompile_my_shit"
                                Helper.Log("Fuck you, Misaki")
                                MsgBox("Why are you trying to decompile my program? Get outta here!")

                            Case "-item"
                                Helper.Log("Detected command argument -item")
                                UseItemTranslation = True

                            Case "-wayu"
                                Helper.Log("Detected command argument -wayu")
                                WayuIsAFailure = True

                            Case "-nodiag"
                                Helper.Log("Detected command argument -nodiag")
                                Helper.Log("Bypassing OS detection to fix compatibility!")
                                Nodiag = True

                            Case "-els"
                                Helper.Log("Detected command argument -els")
                                Helper.Log("Extraterrestrial Living-metal Shape-shifters mode activated! GN Field disabled manually!")
                                Program.NoGNFieldMode = True

                            Case "-reset"
                                Helper.Log("Detected command argument -reset")
                                Dim resetyesno As MsgBoxResult = MsgBox("This will erase all of the PSO2 Tweaker's settings, and will start the initial setup the next time you open it. Continue?", vbYesNo)
                                If resetyesno = vbYes Then
                                    Computer.Registry.CurrentUser.DeleteSubKeyTree("Software\AIDA", False)
                                    Helper.Log("All settings reset, closing program!")
                                    Program.CloseMe = True
                                End If
                            Case "-bypass"
                                Helper.Log("Detected command argument -bypass")
                                Helper.Log("Emergency bypass mode activated - Please only use this mode if the Tweaker will not start normally!")
                                MsgBox("Emergency bypass mode activated - Please only use this mode if the Tweaker will not start normally!")
                                If Pso2RootDir = "lblDirectory" OrElse Not Directory.Exists(Pso2RootDir) Then
                                    MsgBox(Resources.strPleaseSelectwin32Dir)
                                    Helper.SelectPso2Directory()
                                    Continue For
                                End If
                                File.WriteAllBytes(Pso2RootDir & "\ddraw.dll", Resources.ddraw)
                                Helper.Log("Setting environment variable")
                                Environment.SetEnvironmentVariable("-pso2", "+0x01e3f1e9")
                                Helper.Log("Launching PSO2")
                                External.ShellExecute(IntPtr.Zero, "open", (Pso2RootDir & "\pso2.exe"), "+0x33aca2b9 -pso2", "", 0)
                                Do While File.Exists(Pso2RootDir & "\ddraw.dll")
                                    For Each proc As Process In Process.GetProcessesByName("pso2")
                                        If proc.MainWindowTitle = "Phantasy Star Online 2" AndAlso proc.MainModule.ToString() = "ProcessModule (pso2.exe)" Then
                                            If Not transOverride Then Helper.DeleteFile(Pso2RootDir & "\ddraw.dll")
                                        End If
                                    Next
                                    Thread.Sleep(1000)
                                Loop

                            Case "-pso2"
                                launchPso2 = True
                                Helper.Log("Detected command argument -pso2")

                                'Fuck SEGA. Fuck them hard.
                                If Not Directory.Exists(Pso2RootDir) OrElse Pso2RootDir = "lblDirectory" Then
                                    MsgBox(Resources.strPleaseSelectwin32Dir)
                                    Helper.SelectPso2Directory()
                                    Return
                                End If

                                If UseItemTranslation Then
                                    'Download the latest translator.dll and translation.bin
                                    Dim dlLink2 As String = FreedomUrl & "translation.bin"
                                    Helper.Log(Resources.strDownloadingItemTranslationFiles)

                                    ' TODO: WTF is gonig on with this for loop
                                    ' Try up to 4 times to download the translation strings.
                                    For tries As Integer = 1 To 4
                                        Try
                                            Dim DLS As New MyWebClient
                                            DLS.Headers("user-agent") = GetUserAgent()
                                            DLS.DownloadFile(Program.FreedomUrl & "translation.bin", (Program.Pso2RootDir & "\translation.bin"))
                                            Exit For
                                        Catch ex As Exception
                                            If tries = 4 Then
                                                Helper.Log("Failed to download translation files! (" & ex.Message.ToString & "). Try rebooting your computer or making sure PSO2 isn't open.")
                                                Helper.WriteDebugInfo(Helper.ExceptionDump(Resources.strERROR, ex))
                                                Exit Try
                                            End If
                                        End Try
                                    Next

                                    File.WriteAllBytes(Pso2RootDir & "\ddraw.dll", Resources.ddraw)
                                End If

                                Helper.Log("Setting environment variable")
                                Environment.SetEnvironmentVariable("-pso2", "+0x01e3f1e9")

                                Helper.Log("Launching PSO2")
                                External.ShellExecute(IntPtr.Zero, "open", (Pso2RootDir & "\pso2.exe"), "+0x33aca2b9 -pso2", "", 0)

                                Helper.DeleteFile("LanguagePack.rar")
                                If UseItemTranslation Then
                                    Do While File.Exists(Pso2RootDir & "\ddraw.dll")
                                        For Each proc As Process In Process.GetProcessesByName("pso2")
                                            If proc.MainWindowTitle = "Phantasy Star Online 2" AndAlso proc.MainModule.ToString() = "ProcessModule (pso2.exe)" Then
                                                If Not transOverride Then Helper.DeleteFile(Pso2RootDir & "\ddraw.dll")
                                            End If
                                        Next
                                        Thread.Sleep(1000)
                                    Loop
                                End If
                        End Select

                        If Not transOverride Then Helper.DeleteFile(Pso2RootDir & "\ddraw.dll")
                        If launchPso2 Then Environment.Exit(0)

                    Catch ex As Exception
                        Helper.LogWithException(Resources.strERROR, ex)
                    End Try
                Next

                If Not transOverride Then Helper.DeleteFile(Pso2RootDir & "\ddraw.dll")
            Catch ex As Exception
                Helper.LogWithException(Resources.strERROR, ex)
            End Try

            Try
                Helper.Log("Loading settings...")

                If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.PatchServer)) Then RegKey.SetValue(Of String)(RegKey.PatchServer, "Patch Server #1")
                If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.SeenFuckSegaMessage)) Then RegKey.SetValue(Of Boolean)(RegKey.SeenFuckSegaMessage, False)
                If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.Backup)) Then RegKey.SetValue(Of String)(RegKey.Backup, "Never")
                If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.PreDownloadedRar)) Then RegKey.SetValue(Of String)(RegKey.PreDownloadedRar, "Never")
                If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.Pastebin)) Then RegKey.SetValue(Of Boolean)(RegKey.Pastebin, True)
                If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.CloseAfterLaunch)) Then RegKey.SetValue(Of Boolean)(RegKey.CloseAfterLaunch, False)
                If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.EnPatchAfterInstall)) Then RegKey.SetValue(Of Boolean)(RegKey.EnPatchAfterInstall, False)
                If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.LargeFilesAfterInstall)) Then RegKey.SetValue(Of Boolean)(RegKey.LargeFilesAfterInstall, False)
                If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.StoryPatchAfterInstall)) Then RegKey.SetValue(Of Boolean)(RegKey.StoryPatchAfterInstall, False)
                If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.LatestStoryBase)) Then RegKey.SetValue(Of String)(RegKey.LatestStoryBase, "Unknown")
                If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.ProxyEnabled)) Then RegKey.SetValue(Of Boolean)(RegKey.ProxyEnabled, False)
                If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.SteamMode)) Then RegKey.SetValue(Of String)(RegKey.SteamMode, "False")
                If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.Uid)) Then RegKey.SetValue(Of String)(RegKey.Uid, "False")
                If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.SidebarEnabled)) Then RegKey.SetValue(Of Boolean)(RegKey.SidebarEnabled, True)
                If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.RemoveCensor)) Then RegKey.SetValue(Of Boolean)(RegKey.RemoveCensor, True)
                If RegKey.GetValue(Of Object)(RegKey.UseIcsHost) Is Nothing Then RegKey.SetValue(Of Boolean)(RegKey.UseIcsHost, False)

                SidebarEnabled = RegKey.GetValue(Of Boolean)(RegKey.SidebarEnabled)

                If RegKey.GetValue(Of Boolean)(RegKey.UseIcsHost) Then
                    HostsFilePath = Environment.SystemDirectory & "\drivers\etc\HOSTS.ics"
                Else
                    HostsFilePath = Environment.SystemDirectory & "\drivers\etc\HOSTS"
                End If

                If RegKey.GetValue(Of String)(RegKey.Uid) = "False" Then
                    RegKey.SetValue(Of String)(RegKey.Uid, Client.DownloadString("http://arks-layer.com/docs/client.php"))
                End If

                Helper.Log("Load more settings...")
                If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.StoryPatchVersion)) Then RegKey.SetValue(Of String)(RegKey.StoryPatchVersion, "Not Installed")
                If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.EnPatchVersion)) Then RegKey.SetValue(Of String)(RegKey.EnPatchVersion, "Not Installed")
                If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.LargeFilesVersion)) Then RegKey.SetValue(Of String)(RegKey.LargeFilesVersion, "Not Installed")
                If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.SeenDownloadMessage)) Then RegKey.SetValue(Of String)(RegKey.SeenDownloadMessage, "No")

                If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.AlwaysOnTop)) Then RegKey.SetValue(Of Boolean)(RegKey.AlwaysOnTop, False)
                IsMainFormTopMost = Convert.ToBoolean(RegKey.GetValue(Of String)(RegKey.AlwaysOnTop))

                If File.Exists(StartPath & "\logfile.txt") AndAlso Helper.GetFileSize(StartPath & "\logfile.txt") > 30720 Then
                    File.WriteAllText(StartPath & "\logfile.txt", "")
                End If

                If File.Exists(StartPath & "\patchlog.txt") Then File.WriteAllText(StartPath & "\patchlog.txt", "")

                Application.DoEvents()

                If Nodiag Then
                    Helper.Log("Diagnostic info skipped due to -nodiag flag!")
                Else
                    Helper.Log(vbCrLf)
                    Helper.Log("----------------------------------------")
                    Helper.Log(Resources.strProgramOpeningRunningDiagnostics)
                    Helper.Log(Resources.strCurrentOSFullName & Computer.Info.OSFullName)
                    Helper.Log(Resources.strCurrentOSVersion & Computer.Info.OSVersion)
                    Helper.Log(Resources.strIsTheCurrentOS64bit & Environment.Is64BitOperatingSystem)
                    Helper.Log(Resources.strRunDirectory & StartPath)
                    Helper.Log(Resources.strSelectedPSO2win32directory & Pso2RootDir)
                    Dim identity = WindowsIdentity.GetCurrent()
                    Dim principal = New WindowsPrincipal(identity)
                    Dim isElevated As Boolean = principal.IsInRole(WindowsBuiltInRole.Administrator)
                    Helper.Log("Run as Administrator: " & isElevated)
                    Helper.Log("Is 7zip available: " & File.Exists(StartPath & "\7za.exe"))
                    Helper.Log(Resources.strIsUnrarAvailable & File.Exists(StartPath & "\UnRar.exe"))
                    Helper.Log("----------------------------------------")
                End If
            Catch ex As Exception
                Helper.LogWithException(Resources.strERROR, ex)
            End Try
        End Sub
    End Class
    Public Class MyWebClient
        Inherits WebClient

        Private _timeout As Integer

        Public Property Timeout As Integer
            Get
                Timeout = _timeout
            End Get

            Set(ByVal value As Integer)
                _timeout = value
            End Set
        End Property

        Public Sub New()
            Timeout = 60000
        End Sub

        Protected Overrides Function GetWebRequest(ByVal address As Uri) As WebRequest
            Dim result = MyBase.GetWebRequest(address)
            result.Timeout = _timeout
            Return result
        End Function

    End Class
End Namespace
