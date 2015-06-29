Imports Microsoft.Win32

Public Class Admin
    Public isAlertON As Boolean
    Public notCLose As Boolean

    Private Sub NotifyIcon1_MouseDoubleClick(sender As Object, e As MouseEventArgs)

    End Sub

    Private Sub Admin_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'TODO: This line of code loads data into the '_911AlertDataSet.Main' table. You can move, or remove it, as needed.
        Me.MainTableAdapter.Fill(Me._911AlertDataSet.Main)
        tb_ServerIP.Text = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\OLPL\E911Alert", "ServerIP", Nothing)
        tb_ServerPort.Text = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\OLPL\E911Alert", "ServerPort", 0)
        If isAlertON = True Then
            Button2.Enabled = False
            Button2.Text = "Alert Already Started"
        End If
        Try
            DataGridView1.DataSource = MainBindingSource

        Catch ex As Exception

        End Try


    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim frmalert As New Form1
        frmalert.Show()
        Me.notCLose = True
        Me.Close()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim regKey As Microsoft.Win32.RegistryKey
        regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\OLPL", True)
        If regKey Is Nothing Then
            regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE", True)
            regKey.CreateSubKey("OLPL")
        End If
        regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\OLPL\E911Alert", True)
        If regKey Is Nothing Then
            regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\OLPL", True)
            regKey.CreateSubKey("E911Alert")
        End If
        regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\OLPL\E911Alert", True)
        If Not regKey Is Nothing Then
            If tb_ServerIP.Text.Length > 2 Then
                regKey.SetValue("ServerIP", tb_ServerIP.Text)
            End If
            If tb_ServerPort.Text.Length > 2 Then
                regKey.SetValue("ServerPort", tb_ServerPort.Text)
            End If
        End If
    End Sub

    Private Sub Admin_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        If Me.notCLose = False Then
            Dim frmalert As New Form1
            frmalert.Show()
        End If
        
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Me.notCLose = True
        Me.Close()

    End Sub
End Class