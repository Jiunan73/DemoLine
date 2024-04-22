Imports System.Net.Sockets
Imports System.Threading
Imports Bilgic.Net
Imports MySql.Data.MySqlClient
Public Class Form1
    'Dim AGV_status(30) As Integer

    Dim PLCData(5000) As Integer

    Dim tcp_client As TcpClient
    Dim TCPstream As NetworkStream
    Dim map_i As Integer = -1
    Dim map_cnt As Integer = 0
    Dim map_interval As Integer = 3

    Dim agv_tagid(240) As Integer
    Dim agv_action1(240) As Integer
    Dim agv_action2(240) As Integer
    Dim Tag_Point_list(500) As Tag_Point
    Dim offset_X As Integer = 0
    Dim offset_y As Integer = 0


    Public Structure Tag_Point
        '劃地圖專用
        Dim TagId As Integer
        Dim X As Integer
        Dim Y As Integer
        Dim th As Integer
        Dim Retreat_Flag As Integer
        Dim name As String
        Dim floor As String
        Dim floor_no As Integer
        Dim site As String
        Dim tagtype As Integer
        Dim ZONE_NAME As String
        Dim LOC As String
        Dim stkval As String
        Dim CarrierID As String
        Sub setval(ByVal set_ID As Integer, ByVal set_x As Integer, ByVal set_y As Integer, ByVal set_D As String, ByVal set_Len As Integer)
            TagId = set_ID
            X = set_x
            Y = set_y
        End Sub
    End Structure
    Delegate Sub settextcallback(ByVal logout As String, ByVal append As Boolean)
    Dim cnt As Integer = 0
    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        'Econ_0.Text = Now.ToString("ss")
        map_interval = CInt(interval_cb.Text)
        PLCData(100) = CInt(Now.ToString("ss")) 'hartbit


     
        PLCData(118) = PLCData(220)

        cnt += 1

        If PLCData(215) = 1 Then
            PLCData(115) = 1 'AGV狀態 0,1,2,4
        ElseIf PLCData(215) = 2 Then
            PLCData(115) = 2 '停止
            map_i = -1 '命令完了
            map_cnt = 0
        ElseIf PLCData(220) > 0 Then
            PLCData(118) = PLCData(220) '停止
            map_i = -1 '命令完了
            map_cnt = 0
        ElseIf PLCData(115) = 2 And PLCData(215) = 0 Then
            PLCData(115) = 0
        End If

        If PLCData(106) >= 10 And PLCData(108) = 1 And cnt > 10 Then
            If PLCData(106) = &H20 Or PLCData(106) = &H21 Then
                '20 in 

                Button5_Click(sender, e)

            ElseIf PLCData(106) = &H10 Or PLCData(106) = &H11 Then
                Button6_Click(sender, e)
            End If

            PLCData(108) = 2
        End If

        If PLCData(206) = 0 And PLCData(106) >= 0 Then
            PLCData(106) = 0

        ElseIf PLCData(206) = 0 And PLCData(106) = 0 Then
            PLCData(124) = PLCData(206)
            PLCData(106) = PLCData(206) '車上班送命
            PLCData(108) = 0
        ElseIf PLCData(206) = 2 Then
            PLCData(106) = PLCData(206) '車上班送命令
            PLCData(108) = 10
            PLCData(107) = 3

        ElseIf PLCData(206) = 4 Then
            PLCData(106) = PLCData(206) '車上班送命令
            PLCData(108) = 5
            PLCData(107) = 0
        ElseIf PLCData(206) = &H30 Then
            PLCData(106) = PLCData(206) '車上班送命令
            PLCData(108) = 1
        ElseIf PLCData(206) >= 10 And PLCData(108) = 0 Then
            PLCData(106) = PLCData(206) '車上班送命令
            PLCData(108) = 1
            cnt = 0
        End If



        If PLCData(215) = 1 And map_i = -1 Then
            map_i = 0
        End If


        PLCData(113) = PLCData(213) '地圖ON
        If PLCData(213) = 1 Then
            For i As Integer = 0 To 30
                agv_tagid(i) = PLCData(1000 + i)
                agv_action1(i) = PLCData(1240 + 2 * i)
                agv_action2(i) = PLCData(1240 + 2 * i + 1)
            Next
            map_i = 0
        End If

        If map_i >= 0 Then
            '開始走行
            If (PLCData(118) > 0 Or PLCData(119) > 0 Or PLCData(120)) Then
                PLCData(115) = 0 '停止
                map_i = -1 '命令完了
                map_cnt = 0
            Else

                For i As Integer = 0 To 30
                    If PLCData(116) = agv_tagid(i) Then
                    Else

                    End If
                Next

                If Not PLCData(1000 + map_i) = 0 Then
                    PLCData(116) = agv_tagid(map_i) '位置
                    PLCData(101) = agv_action1(map_i) Mod 16 '行進方向
                    PLCData(102) = agv_action2(map_i) Mod 8 '動作 
                    PLCData(103) = (agv_action1(map_i) >> 4) Mod 16 '速度
                    PLCData(104) = (agv_action1(map_i) >> 8) Mod 16 '障礙物
                    PLCData(115) = 4 '走行
                    map_cnt += 1
                    If agv_action2(map_i) = 3 Then
                        '停止
                        PLCData(115) = 0
                        map_i = -1 '命令完了
                        map_cnt = 0

                    End If
                    If map_cnt > map_interval Then
                        map_cnt = 0
                        map_i += 1
                    End If
                Else
                    PLCData(115) = 0
                    map_i = -1 '命令完了
                    map_cnt = 0
                End If


            End If
        End If

        For i As Integer = 0 To 30

            Dim TextBox1 As TextBox = Me.Controls.Find("Econ_" + i.ToString(), True)(0)
            TextBox1.Text = PLCData(i + 100).ToString
            If i <= 20 Then
                Dim TextBox As TextBox = Me.Controls.Find("txtToAGV" + i.ToString(), True)(0)
                TextBox.Text = PLCData(i + 200).ToString
            End If




        Next
        map_action2.Text = ""
        map_tagid.Text = ""
        map_action1.Text = ""

        For i As Integer = 0 To 30
            If PLCData(1000 + i) > 0 Then
                map_tagid.Text += agv_tagid(i).ToString + " "
                map_action1.Text += Hex(agv_action1(i)).PadLeft(3, "0") + " "
                map_action2.Text += Hex(agv_action1(i)).PadLeft(3, "0") + " "

            End If

        Next
        For i As Integer = 0 To Tag_Point_list.Length - 1
            If PLCData(116) = Tag_Point_list(i).TagId Then
                'PLCData(123) = (Tag_Point_list(i).X + offset_X)
                'PLCData(124) = Tag_Point_list(i).Y + offset_y
                'PLCData(125) = Tag_Point_list(i).th * 100
            End If
        Next

        Log_txt.Text = a
        Try
            Timer1.Interval = CInt(TextBox4.Text)
        Catch ex As Exception
            Timer1.Interval = 1000

        End Try

    End Sub
    Dim flag As Long = 0
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
     
        If (My.Computer.Network.Ping(TextBox2.Text)) Then
            tcp_client = New TcpClient
            tcp_client.Connect(TextBox2.Text, CInt(TextBox3.Text))
            If tcp_client.Connected Then
                TCPstream = tcp_client.GetStream
                TCPstream.WriteByte(80)
                TCPstream.ReadTimeout = 1000
                flag = Now.Ticks
                Dim ReceiveThread As New Thread(AddressOf ReceiveData)
                ReceiveThread.IsBackground = True
                ReceiveThread.Start(TCPstream)
            End If
        End If
  
    End Sub
    Dim a As String
    Public Sub ReceiveData(ByVal TCPstream As Object)
        Dim Net_stream As NetworkStream = CType(TCPstream, NetworkStream)
        'timeout
        Dim Rbyte(300) As Byte
        Dim Wbyte(1000) As Byte
        Dim isize As Integer
        Dim len As Integer = 0
        Dim c As Long
        Dim str As String
        Dim crc As New Bilgic.Net.CRC
        Dim modbus_addr As Integer = 0
        Dim modbus_len As Integer = 0
        Dim thread_flag As Long = flag
        While thread_flag = flag
            If Net_stream.DataAvailable Then
                isize = Net_stream.Read(Rbyte, 0, 300)
                str = byte2str(Rbyte, 0, isize)
                If Rbyte(0) = CInt(TextBox1.Text) Then
                    'crc check 
                    If crc_check(Rbyte, isize) Then
                        'settext("R:" + str)
                        '分成三種命令 
                        If Rbyte(1) = 3 Then
                            '讀取
                            modbus_addr = Rbyte(2) * 256 + Rbyte(3)
                            modbus_len = Rbyte(4) * 256 + Rbyte(5)
                            'ReadData(modbus_addr) = 0
                            Wbyte(0) = Rbyte(0)
                            Wbyte(1) = Rbyte(1)
                            Wbyte(2) = Rbyte(5) * 2 + Rbyte(4) * 256 * 2

                            For i As Integer = 0 To modbus_len - 1
                                Wbyte(2 * i + 3) = PLCData(modbus_addr + i) \ 256
                                Wbyte(2 * i + 4) = PLCData(modbus_addr + i) Mod 256
                            Next
                            len = (modbus_len - 1) * 2 + 4 + 1
                        ElseIf Rbyte(1) = 16 Then
                            '寫入 addr分三種  200,1000 ,1240
                            modbus_addr = Rbyte(2) * 256 + Rbyte(3)
                            modbus_len = Rbyte(4) * 256 + Rbyte(5)
                            For i As Integer = 0 To 5
                                Wbyte(i) = Rbyte(i)
                            Next
                            For i As Integer = 0 To modbus_len - 1
                                PLCData(modbus_addr + i) = Rbyte(7 + 2 * i) * 256 + Rbyte(8 + 2 * i)
                            Next
                            len = 6
                        End If
                        c = crc.CRC16(Wbyte, len)
                        Wbyte(len) = c \ 256
                        Wbyte(len + 1) = c Mod 256
                        Net_stream.Write(Wbyte, 0, len + 2)
                        str = byte2str(Wbyte, 0, len + 2)
                        a = ""
                        For i As Integer = 0 To isize - 1
                            a += Rbyte(i).ToString() + ","

                        Next
                        a += vbCrLf
                        For i As Integer = 0 To len + 2 - 1
                            a += Wbyte(i).ToString() + ","
                        Next
                        a += vbCrLf

                        ' settext("W:" + str)
                    End If
                End If
            End If
            Threading.Thread.Sleep(100)
        End While

    End Sub
    Function crc_check(ByVal cmd() As Byte, ByVal len As Integer)
        Dim crc As New Bilgic.Net.CRC
        Dim c As Long
        If len > 3 Then
            c = crc.CRC16(cmd, len - 2)
            If (c = cmd(len - 2) * 256 + cmd(len - 1)) Then
                Return True
            Else
                If len = 47 Then
                    ' MsgBox(byte2str(cmd, 0, len) + ":" + Hex(c))
                End If
                Return False
            End If
        End If
        Return False
    End Function
    Sub settext(ByVal logout As String, Optional ByVal append As Boolean = True, Optional ByVal car_no As Integer = 0)
        Try
            If Not logout = "" Then



              

                If Me.Log_txt.InvokeRequired Then
                    Dim d As New settextcallback(AddressOf settext)
                    Me.Invoke(d, New Object() {logout, append})
                ElseIf append = True Then                
                    Me.Log_txt.Text += logout + vbCrLf
                Else

                    Me.Log_txt.Text = logout + vbCrLf
                End If

            End If

        Catch ex As Exception

        End Try
    End Sub
    Function byte2str(ByVal cmd() As Byte, ByVal start As Integer, ByVal isize As Integer) As String
        byte2str = ""
        For i As Integer = start To isize - 1
            ' byte2str += Hex(cmd(i)) + " "
            byte2str += cmd(i).ToString + " "
        Next
    End Function

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        PLCData(116) = CInt(Cur_tagid.Text)
        PLCData(110) = CInt(Cur_V.Text)
        PLCData(117) = CInt(Cur_shelf.Text)
        PLCData(107) = CInt(Cur_Sensor.Text)
        PLCData(108) = CInt(Cur_PIN.Text)
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        If PLCData(105) = 1 Then
            PLCData(105) = 0
        Else
            PLCData(105) = 1
        End If

    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        'MsgBox(ReadData(0))
        Dim oConn As MySqlConnection
        Dim sqlCommand As New MySqlCommand
        Dim i As Integer = 0
        Dim tagid_len As Integer = 0
        Dim Mysql_str As String = "charset=utf8 ;Database=agvl7a; Data Source=10.97.211.25;User id=root;Password=root; Allow Zero Datetime=True;"
        oConn = New MySqlConnection(Mysql_str)
        Dim Query As String = "SELECT A.Tag_ID, X, Y, Retreat_Flag, Tag_name, floor, floor_no, site, IF( B.`Tag_ID` IS NULL , IF( C.`Tag_ID` IS NULL , 0, 3 ) , 1 ) , "
        Query += " IF( B.`Tag_ID` IS NULL , IF( C.`Tag_ID` IS NULL , '', C.ZONE_NAME ) , B.ZONE_NAME	 )  as ZONENAME, IF( B.`Tag_ID` IS NULL , IF( C.`Tag_ID` IS NULL , '', C.PORT_ID) , B.SHELF_LOC	 )  as LOC , IF( B.`Tag_ID` IS NULL , IF( C.`Tag_ID` IS NULL , '0', PORT_STN_NO ) , B.SHELF_STN_NO )   as stkval ,if (D.CARRIER_ID is null,'',D.CARRIER_ID ),th "
        Query += " FROM `point` A LEFT JOIN shelf B ON A.`Tag_ID` = B.`Tag_ID` LEFT JOIN port C ON A.`Tag_ID` = C.`Tag_ID` LEFT JOIN `carrier` D on A.Tag_name=D.SUB_LOC  WHERE  a.Tag_ID   BETWEEN 0 AND 19999 "
        Dim mReader As MySqlDataReader

        oConn.Open()
        sqlCommand.Connection = oConn
        sqlCommand.CommandText = Query
        mReader = sqlCommand.ExecuteReader()
        i = 0
        While (mReader.Read)

            Tag_Point_list(i).TagId = CInt(mReader.Item(0))
            Tag_Point_list(i).X = CInt(mReader.Item(1))
            Tag_Point_list(i).Y = CInt(mReader.Item(2))
            Tag_Point_list(i).Retreat_Flag = CInt(mReader.Item(3))
            Tag_Point_list(i).name = mReader.Item(4).ToString
            Tag_Point_list(i).floor = mReader.Item(5).ToString
            Tag_Point_list(i).floor_no = CInt(mReader.Item(6))
            Tag_Point_list(i).site = mReader.Item(7)
            Tag_Point_list(i).tagtype = CInt(mReader.Item(8))
            Tag_Point_list(i).ZONE_NAME = mReader.Item(9)
            Tag_Point_list(i).LOC = mReader.Item(10)
            Tag_Point_list(i).stkval = CInt(mReader.Item(11))
            Tag_Point_list(i).CarrierID = (mReader.Item(12))
            Tag_Point_list(i).th = (mReader.Item(13))
            i += 1
            tagid_len = i
        End While
        mReader.Close()
        Array.Resize(Tag_Point_list, i)
        Timer1.Interval = CInt(TextBox4.Text)
       
        Cur_tagid.Text = 5010
        PLCData(116) = CInt(Cur_tagid.Text)
        Timer1.Start()
    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        '載到


        If TextBox5.Text = "" Then
            PLCData(107) = 3
            PLCData(126) = PLCData(209)
            PLCData(127) = PLCData(210)
            PLCData(128) = PLCData(211)
            PLCData(129) = PLCData(212)
        Else
            Dim cst(7) As Byte
            cst = System.Text.Encoding.UTF8.GetBytes(TextBox5.Text)
            Array.Resize(cst, 8)

            PLCData(107) = 3
            PLCData(126) = cst(1) * 256 + cst(0)
            PLCData(127) = cst(3) * 256 + cst(2)
            PLCData(128) = cst(5) * 256 + cst(4)
            PLCData(129) = cst(7) * 256 + cst(6)
        End If

    End Sub

    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        PLCData(107) = 0
        PLCData(126) = 0
        PLCData(127) = 0
        PLCData(128) = 0
        PLCData(129) = 0
    End Sub

    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
        MsgBox(check_path("1,2,3,11,12,13,14,15,25,24,23,22,21,20,10", "1,2,3,11,10"))
    End Sub
    Function check_path(ByVal maincmd As String, ByVal fastcmd As String) As String
        check_path = ""

        Dim main() As String = maincmd.Split(",")
        Dim fast() As String = fastcmd.Split(",")
        Dim flag As Boolean = False
        For i As Integer = 1 To fast.Length - 1
            For j As Integer = 1 To main.Length - 1
                If fast(i) = main(j) And Not i = j Then
                    '相同

                    Return String.Join(",", fast, 0, i + 1)

                End If
            Next
        Next
    End Function


End Class
