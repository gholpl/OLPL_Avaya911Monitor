Imports DelinkNET
Imports System.Threading
Imports Microsoft.Win32
Imports System.Net.Sockets
Imports System.Threading.Tasks
Imports System.Net
Imports System.Data.SqlClient
Imports System.Diagnostics

Public Class Service1
    Dim IPO As DevLinkNet.Devlink
    Dim st1 As Settings = New Settings()
    Dim clientsList As New Hashtable
    Dim serverSocket As TcpListener
    Dim clientSocket As TcpClient
    Dim counter As Integer
    Public Shared as1 As Alert = New Alert()
    Public Shared message As String
    Dim listener As System.Threading.Thread
    Dim clients As New Hashtable 'new database (hashtable) to hold the clients
    Public Shared connectionString As String
    Public Shared isListening As Boolean
    Public Shared cn As SqlConnection
    Dim EventLog1 As New EventLog()
    Dim callRef As String = "0"
    Dim fn1 As New Functions
    Protected Overrides Sub OnStart(ByVal args() As String)
        'EventLog1 = New EventLog()
        'If Not EventLog.Exists("911 Alert Server") Then
        ' EventLog.CreateEventSource("911 Alert Server", "911 ALert")
        ' End If
        EventLog1.Source = "911 Alert Server"
        'EventLog1.Log = "911 Alert Server"

        as1.siPressent = "0"
        IPO = New DevLinkNet.Devlink

        Dim rk As RegistryKey = Registry.LocalMachine.OpenSubKey("Software\OLPL\E911Alert", False)
        st1.IPOServerIP = rk.GetValue("IPOServerIP", Nothing)
        st1.IPOServerPassword = rk.GetValue("IPOServerPassword", "None")
        st1.AlertOnNumber1 = rk.GetValue("AlertOnNumber1", "0")
        st1.AlertOnNumber2 = rk.GetValue("AlertOnNumber2", "0")
        st1.AlertOnNumber3 = rk.GetValue("AlertOnNumber3", "0")
        st1.AlertOnNumber4 = rk.GetValue("AlertOnNumber4", "0")
        st1.ServerPort = rk.GetValue("ServerPort", 0)
        st1.SQLServer = rk.GetValue("SQLServer", 0)
        EventLog1.WriteEntry("Starting 911 Alert Server with Settings: IPOServer: " + st1.IPOServerIP + " SQLServer: " + st1.SQLServer _
                             + " Alert Number 1: " + st1.AlertOnNumber1 + " Alert Number 2: " + st1.AlertOnNumber2 + " Alert Number 3: " + st1.AlertOnNumber3 _
                             + " Alert Number 4: " + st1.AlertOnNumber4 + " Listening On Port: " + st1.ServerPort.ToString())
        IPO.CallLogEventType = DevLinkNet.CallLogType.BaseAndAdvanced
        connectionString = "Data Source=" + st1.SQLServer + ";Initial Catalog=911Alert;Integrated Security=True"
        cn = New SqlConnection(connectionString)
        AddHandler IPO.CallLog_Event_A, AddressOf CallsLog_A
        AddHandler IPO.CallLog_Event_D, AddressOf CallsLog_D
        AddHandler IPO.CallLog_Event_S, AddressOf CallsLog_S
        IPO.StartMonitor(1, st1.IPOServerIP, st1.IPOServerPassword)
        listener = New System.Threading.Thread(AddressOf listen) 'initialize a new thread for the listener so our GUI doesn't lag
        listener.IsBackground = True
        listener.Start(st1.ServerPort)
        ' DebugMode()
    End Sub
    <Conditional("DEBUG")>
    Shared Sub DebugMode()
        If Not Debugger.IsAttached Then
            Debugger.Launch()
        End If

        Debugger.Break()
    End Sub

    Protected Overrides Sub OnStop()
        listener.Abort()
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
        If Integer.Parse(callRef) >= Integer.Parse(e.LogInfo.CallID.ToString()) Then
        Else
            If e.LogInfo.CallingPartyNumber.Count < 5 Then
                ' Call from extention
                If e.LogInfo.CalledPartyNumber.Count < 5 Then
                    ' to extention  Internal Call
                    If e.LogInfo.CallingPartyNumber.Contains("877") Then
                        fn1.tickData("Phone System", "Public FAX", "", e.LogInfo.CallingPartyNumber, EventLog1)
                    Else
                        fn1.tickData("Phone System", "Internal Call", "", e.LogInfo.CallingPartyNumber, EventLog1)
                    End If
                    EventLog1.WriteEntry("Phone System Internal Call " + e.LogInfo.CallingPartyNumber)
                End If
                If e.LogInfo.CalledPartyNumber.Count >= 5 Then
                    ' to outside number outgoing call

                    fn1.tickData("Phone System", "Outgoing Call", "", e.LogInfo.CallingPartyNumber, EventLog1)


                    EventLog1.WriteEntry("Phone System Outgoing Call " + e.LogInfo.CallingPartyNumber)
                End If
            End If
            If e.LogInfo.CallingPartyNumber.Count >= 5 Then
                ' call from outside number
                If e.LogInfo.CalledPartyNumber = "7084224990" Then
                    fn1.tickData("Phone System", "Incomming Call -- Reception", "", e.LogInfo.CalledPartyNumber, EventLog1)
                Else
                    fn1.tickData("Phone System", "Incomming Call -- Other", "", e.LogInfo.CalledPartyNumber, EventLog1)
                End If

                EventLog1.WriteEntry("Phone System Incomming Call " + e.LogInfo.CalledPartyNumber)
            End If
            callRef = e.LogInfo.CallID.ToString
        End If


        ' EventLog1.WriteEntry("Alert 1 Length: " + st1.AlertOnNumber1.Length.ToString() + " " + st1.AlertOnNumber1 + " Loginfo Length: " + e.LogInfo.CalledPartyNumber.Length.ToString() + " " + e.LogInfo.CalledPartyNumber)
        If e.LogInfo.CalledPartyNumber.Contains(st1.AlertOnNumber1) And st1.AlertOnNumber1.Length = e.LogInfo.CalledPartyNumber.Length Then
            EventLog1.WriteEntry("Alert Triggered From: " + e.LogInfo.CallingPartyNumber + " To: " + e.LogInfo.CalledPartyNumber)
            Try
                senddata("CHAT|" & "From: " + e.LogInfo.CallingPartyNumber + " To: " + e.LogInfo.CalledPartyNumber)
                cn.Open()
                Dim cmd As New SqlCommand()
                cmd.CommandText = "INSERT INTO AlertHistory (DateTime, extFrom, extTo) VALUES('" + Now + "', '" + e.LogInfo.CallingPartyNumber + "', '" + e.LogInfo.CalledPartyNumber + "')"
                cmd.Connection = cn
                cmd.ExecuteNonQuery()
                cn.Close()
            Catch ex As Exception
                EventLog1.WriteEntry("Error in event capture trigger 1 Sending Alert or SQL entry error " + ex.ToString())
            End Try


        End If
        If e.LogInfo.CalledPartyNumber.Contains(st1.AlertOnNumber2) And st1.AlertOnNumber2.Length = e.LogInfo.CalledPartyNumber.Length Then
            EventLog1.WriteEntry("Alert Triggered From: " + e.LogInfo.CallingPartyNumber + " To: " + e.LogInfo.CalledPartyNumber)
            Try
                senddata("CHAT|" & "From: " + e.LogInfo.CallingPartyNumber + " To: " + e.LogInfo.CalledPartyNumber)
                cn.Open()
                Dim cmd As New SqlCommand()
                cmd.CommandText = "INSERT INTO AlertHistory (DateTime, extFrom, extTo) VALUES('" + Now + "', '" + e.LogInfo.CallingPartyNumber + "', '" + e.LogInfo.CalledPartyNumber + "')"
                cmd.Connection = cn
                cmd.ExecuteNonQuery()
                cn.Close()
            Catch ex As Exception
                EventLog1.WriteEntry("Error in event capture trigger 2 Sending Alert or SQL entry error " + ex.ToString())
            End Try
        End If
        If e.LogInfo.CalledPartyNumber.Contains(st1.AlertOnNumber3) And st1.AlertOnNumber3.Length = e.LogInfo.CalledPartyNumber.Length Then
            EventLog1.WriteEntry("Alert Triggered From: " + e.LogInfo.CallingPartyNumber + " To: " + e.LogInfo.CalledPartyNumber)
            Try
                senddata("CHAT|" & "From: " + e.LogInfo.CallingPartyNumber + " To: " + e.LogInfo.CalledPartyNumber)
                cn.Open()
                Dim cmd As New SqlCommand()
                cmd.CommandText = "INSERT INTO AlertHistory (DateTime, extFrom, extTo) VALUES('" + Now + "', '" + e.LogInfo.CallingPartyNumber + "', '" + e.LogInfo.CalledPartyNumber + "')"
                cmd.Connection = cn
                cmd.ExecuteNonQuery()
                cn.Close()
            Catch ex As Exception
                EventLog1.WriteEntry("Error in event capture trigger 3 Sending Alert or SQL entry error " + ex.ToString())
            End Try
        End If
        If e.LogInfo.CalledPartyNumber.Contains(st1.AlertOnNumber4) And st1.AlertOnNumber4.Length = e.LogInfo.CalledPartyNumber.Length Then
            EventLog1.WriteEntry("Alert Triggered From: " + e.LogInfo.CallingPartyNumber + " To: " + e.LogInfo.CalledPartyNumber)
            Try
                senddata("CHAT|" & "From: " + e.LogInfo.CallingPartyNumber + " To: " + e.LogInfo.CalledPartyNumber)
                cn.Open()
                Dim cmd As New SqlCommand()
                cmd.CommandText = "INSERT INTO AlertHistory (DateTime, extFrom, extTo) VALUES('" + Now + "', '" + e.LogInfo.CallingPartyNumber + "', '" + e.LogInfo.CalledPartyNumber + "')"
                cmd.Connection = cn
                cmd.ExecuteNonQuery()
                cn.Close()
            Catch ex As Exception
                EventLog1.WriteEntry("Error in event capture trigger 4 Sending Alert or SQL entry error " + ex.ToString())
            End Try
        End If
    End Sub

    Sub recieved(ByVal msg As String, ByVal client As ConnectedClient)
        Dim message() As String = msg.Split("|") 'make an array with elements of the message recieved
        Select Case message(0) 'process by the first element in the array
            Case "CHAT" 'if it's CHAT
                'RichTextBox1.Text &= client.name & " says: " & " " & message(1) & vbNewLine 'add the message to the chatbox
                sendallbutone(message(1), client.name) 'this will update all clients with the new message
                '                                       and it will not send the message to the client it recieved it from :)
            Case "LOGIN" 'A client has connected
                clients.Add(client, client.name) 'add the client to our database (a hashtable)
                'ListBox1.Items.Add(client.name) 'add the client to the listbox to display the new user
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
        Catch ex As Exception
            EventLog1.WriteEntry("Error sending data to client: " + ex.StackTrace.ToString() + " Messege to send: " + message)
        End Try

    End Sub
    Sub disconnected(ByVal client As ConnectedClient) 'if a client is disconnected, this is raised
        EventLog1.WriteEntry("Client Removed: " + client.name.ToString())
        clients.Remove(client) 'remove the client from the hashtable
        'ListBox1.Items.Remove(client.name) 'remove it from our listbox
    End Sub
    Sub listen(ByVal port As Integer)
        Try
            Dim t As New TcpListener(IPAddress.Any, port) 'declare a new tcplistener
            t.Start() 'start the listener
            Do

                Dim client As New ConnectedClient(t.AcceptTcpClient) 'initialize a new connected client
                AddHandler client.gotmessage, AddressOf recieved 'add the handler which will raise an event when a message is recieved
                AddHandler client.disconnected, AddressOf disconnected 'add the handler which will raise an event when the client disconnects
                EventLog1.WriteEntry("Client added: " + client.name.ToString())
            Loop Until False
        Catch ex As Exception
            EventLog1.WriteEntry("Listener Exception: " + ex.StackTrace.ToString())
        End Try

    End Sub
End Class
