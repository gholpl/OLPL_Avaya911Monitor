Imports System.IO
Imports System.Net.Sockets
Public Class ConnectedClient
    Private cli As TcpClient 'decleare a tcp client which will be the client that we assign to an instance of this class
    Private uniqueid As String 'this will be used for the name property
    Public Property name ''This will be the name of the ID containing its Unique ID 
        Get
            Return uniqueid 'when we want to get it, it will return the Unique ID 
        End Get
        Set(ByVal value)
            uniqueid = value 'Used for setting the name
        End Set
    End Property
    Sub New(ByVal client As TcpClient)

        Dim r As New Random 'create a new random to serve as way to create our unique ID
        Dim x As String = String.Empty 'declare a new variable to hold the ID
        For i = 0 To 7 'we are goign to have an ID of 7 randomly generated characters
            x &= Chr(r.Next(65, 89)) 'create a generate dnumber between 65 and 89 and get the letter that has the same ascii value (A-Z)
            '                         and add it onto the ID string
        Next
        Me.name = client.Client.RemoteEndPoint.ToString().Remove(client.Client.RemoteEndPoint.ToString().LastIndexOf(":")) & " - " & x 'set the name to the Unique ID 
        cli = client 'assign the client specified to the TCP client variable to we can operate with it
        cli.GetStream.BeginRead(New Byte() {0}, 0, 0, AddressOf read, Nothing) 'start reading using the read subroutine

    End Sub
    Public Event gotmessage(ByVal message As String, ByVal client As ConnectedClient)    'this is raised when we get a message from the client
    Public Event disconnected(ByVal client As ConnectedClient)    'this is raised when we get the client disconnects
    Sub read(ByVal ar As IAsyncResult) 'this will process all messages being recieved
        Try
            Dim sr As New StreamReader(cli.GetStream) 'initialize a new streamreader which will read from the client's stream
            Dim msg As String = sr.ReadLine() 'create a new variable which will be used to hold the message being read
            RaiseEvent gotmessage(msg, Me) 'tell the server a message has been recieved. Me is passed as an argument which represents 
            '                               the current client which it has recieved the message from to perform any client specific
            '                               tasks if needed
            cli.GetStream.BeginRead(New Byte() {0}, 0, 0, AddressOf read, Nothing) 'continue reading from the stream
        Catch ex As Exception
            Try 'if an error occurs in the reading purpose, we will try to read again to see if we still can read
                Dim sr As New StreamReader(cli.GetStream) 'initialize a new streamreader which will read from the client's stream
                Dim msg As String = sr.ReadLine() 'create a new variable which will be used to hold the message being read
                RaiseEvent gotmessage(msg, Me) 'tell the server a message has been recieved. Me is passed as an argument which represents 
                '                               the current client which it has recieved the message from to perform any client specific
                '                               tasks if needed
                cli.GetStream.BeginRead(New Byte() {0}, 0, 0, AddressOf read, Nothing) 'continue reading from the stream
            Catch ' IF WE STILL CANNOT READ
                RaiseEvent disconnected(Me)  'WE CAN ASSUME THE CLIENT HAS DISCONNECTED
            End Try

        End Try
    End Sub
    Sub senddata(ByVal message As String) 'this is used to deal with sending out messages
        Dim sw As New StreamWriter(cli.GetStream) 'declare a new streamwrite to write to the stream between the client and the server
        sw.WriteLine(message) 'write the message to the stream
        sw.Flush()
    End Sub
End Class