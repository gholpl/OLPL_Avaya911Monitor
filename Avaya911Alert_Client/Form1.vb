Imports System
Imports System.Net
Imports System.Net.Sockets
Imports System.Threading
Imports System.Text
Imports System.IO
Imports Microsoft.Win32

Public Class Form1
    Public onForm As Boolean
    Public Shared alertON As Boolean
    Dim message As String
    ' The port number for the remote device.
    Dim st1 As Settings = New Settings()
    Dim t As New TcpClient
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        alertON = False
        onForm = True
        Dim regKey As Microsoft.Win32.RegistryKey
        regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\OLPL\E911Alert", True)
        st1.ServerIP = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\OLPL\E911Alert", "ServerIP", Nothing)
        st1.ServerPort = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\OLPL\E911Alert", "ServerPort", 0)
        If regKey Is Nothing Or st1.ServerIP Is Nothing Or st1.ServerPort = 0 Then
            Dim frmAdmin As New Admin
            frmAdmin.isAlertON = False
            Me.onForm = False
            frmAdmin.Show()
            Me.Close()
        Else

            st1.ServerIP = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\OLPL\E911Alert", "ServerIP", Nothing)
            st1.ServerPort = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\OLPL\E911Alert", "ServerPort", 0)
            connect(st1.ServerIP, st1.ServerPort)

            Timer1.Start()
        End If
    End Sub

    Sub connect(ByVal ip As String, ByVal port As Integer)

        Try
            t.Connect(ip, port) 'tries to connect
            If t.Connected Then 'if connected, start the reading procedure
                t.GetStream.BeginRead(New Byte() {0}, 0, 0, AddressOf doread, Nothing)
                login() 'send our details to the server
            End If
        Catch ex As Exception
            NotifyIcon1.BalloonTipText = "Not Connected to server. Please contact Admin"
            NotifyIcon1.ShowBalloonTip(5000)
            NotifyIcon1.Visible = True
            t.Close()
            t = New TcpClient
            System.Threading.Thread.Sleep(60000) 'if an error occurs sleep for 10 seconds
            connect(ip, port) 'try to reconnect
        End Try
    End Sub
    Sub login()
        senddata("LOGIN|") 'log in to the chatserver
    End Sub
    Sub senddata(ByVal message As String)
        Dim sw As New StreamWriter(t.GetStream) 'declare a new streamwriter
        sw.WriteLine(message) 'write the message
        sw.Flush()

    End Sub
    Sub messagerecieved(ByVal message As String)
        Dim msg() As String = message.Split("|") ' if a message is recieved, split it to process it
        Select Case msg(0) 'process it by the first element in the split array

            Case "CHAT"
                If alertON = False Then
                    alertON = True
                    message = msg(1)
                End If
                RichTextBox1.Text = msg(1)
        End Select

    End Sub

    Sub doread(ByVal ar As IAsyncResult)
        Try
            Dim sr As New StreamReader(t.GetStream) 'declare a new streamreader to read fromt eh network stream
            Dim msg As String = sr.ReadLine() 'the msg is what is bing read
            messagerecieved(msg) 'start processing the message
            t.GetStream.BeginRead(New Byte() {0}, 0, 0, AddressOf doread, Nothing) 'continue to read

        Catch ex As Exception
            System.Threading.Thread.Sleep(10000) 'if an error occurs, wait for 10 seconds
            connect(st1.ServerIP, st1.ServerPort) 'try to reconnect
        End Try
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If alertON = True Then
            Me.Visible = True
            If Me.BackColor = Color.White Then
                Me.BackColor = Color.Red
            Else
                Me.BackColor = Color.White
            End If
            If RichTextBox1.BackColor = Color.White And Me.BackColor = Color.White Then
                RichTextBox1.BackColor = Color.Red
            Else
                RichTextBox1.BackColor = Color.White
            End If

        End If
        If alertON = False Then
            Me.BackColor = Color.White
            RichTextBox1.BackColor = Color.White
            Me.Visible = False
        End If
    End Sub

    Private Sub NotifyIcon1_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles NotifyIcon1.MouseDoubleClick
        Dim frmAdmin As New Admin
        frmAdmin.isAlertON = False
        Me.onForm = False
        frmAdmin.Show()
        Me.Close()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        alertON = False
        RichTextBox1.Text = ""
    End Sub

    Private Sub Form1_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        If Me.onForm = True Then
            Dim frmAlerts As New Form1
            frmAlerts.Show()
        End If
        

    End Sub
End Class