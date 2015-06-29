Imports System.Data.SqlClient

Public Class Functions
    Function tickData(strLoc As String, type As String, comment As String, generatingNumber As String, ev1 As EventLog)
        Dim strCompName As String = Environment.MachineName
        Dim SQLStr = "INSERT into Main(stringLoc, DateTime, stringComment, stringType, strComp) VALUES('" + strLoc + "',  '" + Now + "', '" + comment + "', '" + type + "', '" + generatingNumber + "')"
        Dim connection As SqlConnection
        Dim compNameresult As Integer = 0
        Dim connStr As String = "Data Source=sqs1;Initial Catalog=StatsDesk;Integrated Security=True"
        connection = New SqlConnection(connStr)
        Dim SQLCmd As New SqlCommand()
        'MsgBox(strCompName)
        Try
            connection.Open()
            SQLCmd.Connection = connection
            SQLCmd.CommandText = SQLStr
            SQLCmd.ExecuteNonQuery()
            SQLCmd.Dispose()
            connection.Close()
        Catch ex As Exception
            ev1.WriteEntry(ex.ToString())
        End Try
    End Function
End Class
