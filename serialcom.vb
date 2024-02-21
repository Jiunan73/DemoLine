    Dim DataReceivedstr As String
    Private Sub SerialPort1_DataReceived(ByVal sender As Object, ByVal e As System.IO.Ports.SerialDataReceivedEventArgs) Handles SerialPort1.DataReceived
      
        DataReceivedstr += SerialPort1.ReadExisting
        If DataReceivedstr.Contains(";") Then
            Dim oConn As MySqlConnection
            Dim sqlCommand As New MySqlCommand
            Dim Query = ""
            oConn = New MySqlConnection(Mysql_str)
            oConn.Open()
            Dim parts() As String = DataReceivedstr.Split(";"c)
            For i As Integer = 0 To parts.Length - 2
                Dim data As String = parts(i)
                Dim ans As Object
                sqlCommand.Connection = oConn
                str = SerialPort1.ReadExisting
                sqlCommand.CommandText = data
                Try
                    ans = sqlCommand.ExecuteNonQuery()
                Catch ex As Exception
                    ans = -1
                End Try

                ' 在这里执行对每个完整数据的处理，比如打印到文本框
                TextBox1.Invoke(Sub() TextBox1.AppendText(data + "=" + ans.ToString & vbCrLf))
            Next
            DataReceivedstr = parts(parts.Length - 1)

            oConn.Close()
            oConn.Dispose()
        End If



    End Sub
