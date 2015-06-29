<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Admin
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.MainBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me._911AlertDataSet = New Avaya911Alert_Client._911AlertDataSet()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.tb_ServerIP = New System.Windows.Forms.TextBox()
        Me.tb_ServerPort = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.MainTableAdapter = New Avaya911Alert_Client._911AlertDataSetTableAdapters.MainTableAdapter()
        Me.Button3 = New System.Windows.Forms.Button()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.MainBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me._911AlertDataSet, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'DataGridView1
        '
        Me.DataGridView1.AllowUserToAddRows = False
        Me.DataGridView1.AllowUserToDeleteRows = False
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Location = New System.Drawing.Point(12, 181)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.ReadOnly = True
        Me.DataGridView1.Size = New System.Drawing.Size(445, 329)
        Me.DataGridView1.TabIndex = 0
        '
        'MainBindingSource
        '
        Me.MainBindingSource.DataMember = "Main"
        Me.MainBindingSource.DataSource = Me._911AlertDataSet
        '
        '_911AlertDataSet
        '
        Me._911AlertDataSet.DataSetName = "_911AlertDataSet"
        Me._911AlertDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 13.0!)
        Me.Label1.Location = New System.Drawing.Point(12, 35)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(89, 22)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Server IP:"
        '
        'tb_ServerIP
        '
        Me.tb_ServerIP.Font = New System.Drawing.Font("Microsoft Sans Serif", 13.0!)
        Me.tb_ServerIP.Location = New System.Drawing.Point(120, 32)
        Me.tb_ServerIP.Name = "tb_ServerIP"
        Me.tb_ServerIP.Size = New System.Drawing.Size(171, 27)
        Me.tb_ServerIP.TabIndex = 2
        '
        'tb_ServerPort
        '
        Me.tb_ServerPort.Font = New System.Drawing.Font("Microsoft Sans Serif", 13.0!)
        Me.tb_ServerPort.Location = New System.Drawing.Point(120, 65)
        Me.tb_ServerPort.Name = "tb_ServerPort"
        Me.tb_ServerPort.Size = New System.Drawing.Size(171, 27)
        Me.tb_ServerPort.TabIndex = 4
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 13.0!)
        Me.Label2.Location = New System.Drawing.Point(12, 68)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(106, 22)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "Server Port:"
        '
        'Button1
        '
        Me.Button1.Font = New System.Drawing.Font("Microsoft Sans Serif", 13.0!)
        Me.Button1.Location = New System.Drawing.Point(16, 132)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(224, 43)
        Me.Button1.TabIndex = 5
        Me.Button1.Text = "Update Settings"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Button2
        '
        Me.Button2.Font = New System.Drawing.Font("Microsoft Sans Serif", 13.0!)
        Me.Button2.Location = New System.Drawing.Point(246, 132)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(206, 43)
        Me.Button2.TabIndex = 6
        Me.Button2.Text = "Start Alert"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'MainTableAdapter
        '
        Me.MainTableAdapter.ClearBeforeFill = True
        '
        'Button3
        '
        Me.Button3.Font = New System.Drawing.Font("Microsoft Sans Serif", 13.0!)
        Me.Button3.Location = New System.Drawing.Point(362, 16)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(95, 43)
        Me.Button3.TabIndex = 7
        Me.Button3.Text = "Exit ALL"
        Me.Button3.UseVisualStyleBackColor = True
        '
        'Admin
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(464, 522)
        Me.Controls.Add(Me.Button3)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.tb_ServerPort)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.tb_ServerIP)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.DataGridView1)
        Me.Name = "Admin"
        Me.Text = "911 Alerts CLient Admin"
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.MainBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me._911AlertDataSet, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents DataGridView1 As System.Windows.Forms.DataGridView
    Friend WithEvents _911AlertDataSet As Avaya911Alert_Client._911AlertDataSet
    Friend WithEvents MainBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents MainTableAdapter As Avaya911Alert_Client._911AlertDataSetTableAdapters.MainTableAdapter
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents tb_ServerIP As System.Windows.Forms.TextBox
    Friend WithEvents tb_ServerPort As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents Button3 As System.Windows.Forms.Button
End Class
