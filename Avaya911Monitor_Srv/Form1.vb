Imports DelinkNET
Imports System
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading
Imports Microsoft.VisualBasic
Imports Microsoft.Win32

Public Class Form1
    Dim IPO As DevLinkNet.Devlink
    Dim st1 As Settings = New Settings()

    Public Shared as1 As Alert = New Alert()
    Public Shared message As String
    Dim clients As New Hashtable 'new database (hashtable) to hold the clients
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        as1.siPressent = "0"
        IPO = New DevLinkNet.Devlink
        Dim regKey As Microsoft.Win32.RegistryKey
        regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\OLPL\E911Alert", True)
        If regKey Is Nothing Then

        Else
            st1.IPOServerIP = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\OLPL\E911Alert", "IPOServerIP", Nothing)
            st1.IPOServerPassword = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\OLPL\E911Alert", "IPOServerPassword", "None")
            st1.AlertOnNumber1 = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\OLPL\E911Alert", "AlertOnNumber1", "0")
            st1.AlertOnNumber2 = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\OLPL\E911Alert", "AlertOnNumber2", "0")
            st1.AlertOnNumber3 = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\OLPL\E911Alert", "AlertOnNumber3", "0")
            st1.AlertOnNumber4 = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\OLPL\E911Alert", "AlertOnNumber4", "0")
            st1.ServerPort = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\OLPL\E911Alert", "ServerPort", 0)
            IPO.CallLogEventType = DevLinkNet.CallLogType.BaseAndAdvanced
            AddHandler IPO.CallLog_Event_A, AddressOf CallsLog_A
            AddHandler IPO.CallLog_Event_D, AddressOf CallsLog_D
            AddHandler IPO.CallLog_Event_S, AddressOf CallsLog_S
            IPO.StartMonitor(1, st1.IPOServerIP, st1.IPOServerPassword)
            Dim listener As New System.Threading.Thread(AddressOf listen) 'initialize a new thread for the listener so our GUI doesn't lag
            listener.IsBackground = True
            listener.Start(st1.ServerPort)
        End If
        

    End Sub
    Private Sub CallsLog_A(sender As Object, e As DevLinkNet.CallLogEvent_Parameter.CallLog_A_Parameter)
        as1.siPressent = 0
        as1.numTo = ""
        as1.numFrom = ""
    End Sub

    Private Sub CallsLog_D(sender As Object, e As DevLinkNet.CallLogEvent_Parameter.CallLog_D_Parameter)
        as1.siPressent = 0
        as1.numTo = ""
        as1.numFrom = ""
    End Sub


    Private Sub CallsLog_S(sender As Object, e As DevLinkNet.CallLogEvent_Parameter.CallLog_S_Parameter)
        If e.LogInfo.CalledPartyNumber.Contains(st1.AlertOnNumber1) Then
            senddata("CHAT|" & "From: " + e.LogInfo.CallingPartyNumber + " To: " + e.LogInfo.CalledPartyNumber)
            RichTextBox1.Text = "From: " + e.LogInfo.CallingPartyNumber + " To: " + e.LogInfo.CalledPartyNumber
        End If
        If e.LogInfo.CalledPartyNumber.Contains(st1.AlertOnNumber2) Then
            senddata("CHAT|" & "From: " + e.LogInfo.CallingPartyNumber + " To: " + e.LogInfo.CalledPartyNumber)
            RichTextBox1.Text = "From: " + e.LogInfo.CallingPartyNumber + " To: " + e.LogInfo.CalledPartyNumber

        End If
        If e.LogInfo.CalledPartyNumber.Contains(st1.AlertOnNumber3) Then
            senddata("CHAT|" & "From: " + e.LogInfo.CallingPartyNumber + " To: " + e.LogInfo.CalledPartyNumber)
            RichTextBox1.Text = "From: " + e.LogInfo.CallingPartyNumber + " To: " + e.LogInfo.CalledPartyNumber

        End If
        If e.LogInfo.CalledPartyNumber.Contains(st1.AlertOnNumber4) Then
            senddata("CHAT|" & "From: " + e.LogInfo.CallingPartyNumber + " To: " + e.LogInfo.CalledPartyNumber)
            RichTextBox1.Text = "From: " + e.LogInfo.CallingPartyNumber + " To: " + e.LogInfo.CalledPartyNumber

        End If
    End Sub

    Private Sub StartStopButton_Click(sender As Object, e As EventArgs) Handles StartStopButton.Click
        senddata("CHAT|" & TextBox1.Text) 'send teh data with CHAT as the header so the clietn knows to process the message as a chat message
        RichTextBox1.Text &= "You Say: " & " " & TextBox1.Text & vbNewLine 'add a message to the chat textbox showing we have sent a public message

    End Sub



    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick


    End Sub
    Sub recieved(ByVal msg As String, ByVal client As ConnectedClient)
        Dim message() As String = msg.Split("|") 'make an array with elements of the message recieved
        Select Case message(0) 'process by the first element in the array
            Case "CHAT" 'if it's CHAT
                RichTextBox1.Text &= client.name & " says: " & " " & message(1) & vbNewLine 'add the message to the chatbox
                sendallbutone(message(1), client.name) 'this will update all clients with the new message
                '                                       and it will not send the message to the client it recieved it from :)
            Case "LOGIN" 'A client has connected
                clients.Add(client, client.name) 'add the client to our database (a hashtable)
                ListBox1.Items.Add(client.name) 'add the client to the listbox to display the new user
        End Select

    End Sub
    Sub sendallbutone(ByVal message As String, ByVal exemptclientname As String) 'this sends to all clients except the one specified
        Dim entry As DictionaryEntry 'declare a variable of type dictionary entry
        Try
            For Each entry In clients 'for each dictionary entry in the hashtable with all clients (clients)
                If entry.Value <> exemptclientname Then 'if the entry IS NOT the exempt client name
                    Dim cli As ConnectedClient = CType(entry.Key, ConnectedClient) ' cast the hashtable entry to a connection class
                    cli.senddata(message) 'send the message to it
                End If
            Next
        Catch
        End Try
    End Sub
    Sub sendsingle(ByVal message As String, ByVal clientname As String)
        Dim entry As DictionaryEntry 'declare a variable of type dictionary entry
        Try
            For Each entry In clients 'for each dictionary entry in the hashtable with all clients (clients)
                If entry.Value = clientname Then 'if the entry is belongs to the client specified
                    Dim cli As ConnectedClient = CType(entry.Key, ConnectedClient) ' cast the hashtable entry to a connection class
                    cli.senddata(message) 'send the message to it
                End If
            Next
        Catch
        End Try

    End Sub
    Sub senddata(ByVal message As String) 'this sends a message to all connected clients
        Dim entry As DictionaryEntry 'declare a variable of type dictionary entry
        Try
            For Each entry In clients 'for each dictionary entry in the hashtable with all clients (clients)
                Dim cli As ConnectedClient = CType(entry.Key, ConnectedClient) ' cast the hashtable entry to a connection class
                cli.senddata(message) 'send the message to it
            Next  'go to the next client
        Catch
        End Try

    End Sub
    Sub disconnected(ByVal client As ConnectedClient) 'if a client is disconnected, this is raised
        clients.Remove(client) 'remove the client from the hashtable
        ListBox1.Items.Remove(client.name) 'remove it from our listbox
    End Sub
    Sub listen(ByVal port As Integer)
        Try
            Dim t As New TcpListener(IPAddress.Any, port) 'declare a new tcplistener
            t.Start() 'start the listener
            Do

                Dim client As New ConnectedClient(t.AcceptTcpClient) 'initialize a new connected client
                AddHandler client.gotmessage, AddressOf recieved 'add the handler which will raise an event when a message is recieved
                AddHandler client.disconnected, AddressOf disconnected 'add the handler which will raise an event when the client disconnects

            Loop Until False
        Catch
        End Try

    End Sub

    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
        Thread.Sleep(1000)
    End Sub
End Class
