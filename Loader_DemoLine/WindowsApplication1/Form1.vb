Imports System
Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Net.Sockets
Imports System.Threading
Imports MySql.Data.MySqlClient
Public Class Form1

    Dim AGVTcp As TcpClient
    Dim AGVstream As NetworkStream
    Dim DemoLineTcp As TcpClient
    Dim DemoLinestream As NetworkStream

    Dim ReadWord(19) As Integer
    Dim WriteWord(9) As Integer
    Dim writeflag As Boolean = False
    Dim Mysql_str As String = "charset=utf8 ;Database=agv; Data Source=192.168.2.180;User id=agvc;Password=agvc; Allow Zero Datetime=True;"
    Dim agv As agvcar
    Dim input(1) As Integer
    Dim device_status(50) As Integer
    Dim TagList(50000) As Tag_Point
    Dim istep As Integer = -1
    Dim heartbit As Integer = 0
    Dim BMS1(52) As Integer

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
    
        AGV_Timer.Start()


        EQ.Start()


    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click

        WriteWord(0) = 32

        writeflag = True

    End Sub
    Dim step_i As Integer = 0
    Dim change_cnt As Integer = 0
    Dim write_cnt As Integer = 0
    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Dim str As String = "{ " + _
"""reqCode"": """ + Now().ToString("yyyyMMddHHmmssfff") + "01""," + _
"""interfaceName"":""getAgvStatus"", " + _
"""robotCount"":""1""," + _
"""robots"": [" + _
"""6680"" " + _
" ], " + _
"""mapShortName"":""test""} "
        Dim return_str As String
        TextBox2.Text = str
        Try


            return_str = post("http://127.0.0.1:80/cms/services/rest/hikRpcService/getAgvStatus", str)
            TextBox1.Text = return_str
            Dim JsonStr As String = return_str
            Dim Obj As Newtonsoft.Json.Linq.JObject = Newtonsoft.Json.JsonConvert.DeserializeObject(JsonStr)
            ' MsgBox(Obj.Item("data").HasValues)
            Dim bms1(52) As Integer
            Dim status As Integer
            ' Try
            Dim battery As String = Obj.Item("data")(0)("battery").ToString
            Dim direction As String = Obj.Item("data")(0)("direction").ToString
            Dim posX As String = Obj.Item("data")(0)("posX").ToString
            Dim posY As String = Obj.Item("data")(0)("posY").ToString

            Dim robotCode As String = Obj.Item("data")(0)("robotCode").ToString
            status = Obj.Item("data")(0)("status").ToString
            device_status(23) = CInt(posX)
            device_status(24) = CInt(posY)
            device_status(16) = GetPose(CInt(posX), CInt(posY), device_status(16))
            device_status(25) = CInt(direction) * 100
            device_status(0) = device_status(0) + 1
            If device_status(0) > 60 Then
                device_status(0) = 0
            End If
            bms1(14) = Obj.Item("data")(0)("battery").ToString
            SOC.Text = bms1(14).ToString
            Dim val(20) As Integer '回傳易控點位
            TextBox3.Text = GetStatus(CInt(status), val)

            For j As Integer = 0 To 49
                Dim TextBox As TextBox = Me.Controls.Find("Econ_" + j.ToString(), True)(0)
                If j = 19 Or j = 15 Or j = 8 Then
                    device_status(j) = val(j)
                End If
                TextBox.Text = device_status(j).ToString

            Next
            Label30.Text = ""
            Dim cellv(15) As Integer
            For i As Integer = 1 To 16

                Label30.Text += Obj.Item("data")(0)("cellVoltage")(0)("cellVoltage" + i.ToString).ToString()
                ' Label30.Text = Obj.Item("cellVoltage" + i.ToString).ToString() + vbCrLf
                If i Mod 4 = 0 Then
                    Label30.Text += vbCrLf
                ElseIf i < 16 Then
                    Label30.Text += ","
                End If
            Next
            Label30.Text += "SOC:" + Obj.Item("data")(0)("battery").ToString() + vbCrLf
            Label30.Text += "輸入電流:" + Obj.Item("data")(0)("inputCur").ToString() + vbCrLf
            Label30.Text += "輸出電流:" + Obj.Item("data")(0)("outCur").ToString() + vbCrLf
            Label30.Text += "溫度1:" + Obj.Item("data")(0)("temperatureLow").ToString() + vbCrLf
            Label30.Text += "溫度2:" + Obj.Item("data")(0)("temperatureOne").ToString() + vbCrLf
            Label30.Text += "總電壓:" + Obj.Item("data")(0)("volTage").ToString() + vbCrLf
            If CInt(status) = 4 Then


            End If
        Catch ex As Exception
            TextBox1.Text = "HIK斷線"
        End Try
        Dim Query As String
        Dim oConn As MySqlConnection
        Dim sqlCommand As New MySqlCommand
        oConn = New MySqlConnection(Mysql_str)
        oConn.Open()
        sqlCommand.Connection = oConn
        Query = "update  `agv_list` set CmdKey='',carWork=" + device_status(6).ToString + ",AGVAction='" + device_status(8).ToString + "',"
        Query += "Status='" + device_status(15).ToString + "',Position=" + device_status(16).ToString + ",ErrorCode='" + device_status(18).ToString + "',"
        Query += "Speed=0,BatteryVoltage=" + device_status(10).ToString + ",Shelf_Car_No=0,Loading='"
        Query += device_status(7).ToString + "',distance=0,Temp=" + device_status(21).ToString + ",tag_change_time='"
        Query += "now()',AGV_X=" + (device_status(23) / 10).ToString + " ,AGV_Y=" + (device_status(24) / 10).ToString + ",AGV_TH=" + device_status(25).ToString
        Query += ",VB1=" + BMS1(7).ToString + ",IB1=" + BMS1(8).ToString + " ,BT1=" + BMS1(10).ToString + ",SOC1=" + BMS1(14).ToString + ",SOH1=" + BMS1(15).ToString
        Query += ",PROT1=" + BMS1(16).ToString + ",STAT1=" + BMS1(17).ToString + " ,CHG_AH1=" + (BMS1(37) * 65536 + BMS1(38)).ToString + ",DSG_AH1=" + (BMS1(39) * 65536 + BMS1(40)).ToString + ",CYCLE1=" + BMS1(41).ToString
        Query += "  where AGVNo=4"
        sqlCommand.CommandText = Query
        sqlCommand.ExecuteNonQuery()
        oConn.Close()
        'Catch ex As Exception
        '    MsgBox(ex.Message)
        'End Try

        '判斷 有料 
        If istep = -1 And (Loadreq = 1 Or UnLoadreq = 1 Or heartbit = 1) Then


            If device_status(7) = 3 Then
                If device_status(15) = 0 Then
                    istep = 1
                End If
            Else
                If device_status(15) = 0 Then
                    istep = 10
                End If
            End If
        End If
        ' 0~9 是滾CV1
        ' 10~19 滾CV2
        Dim step_str As String = "待機"
        Select Case istep
            Case 1
                '移動到1002
                Button4_Click_1(sender, e)
                istep = 2
                step_str = "移動到1002"
            Case 2
                '判斷到位
                If device_status(15) = 0 And device_status(16) = 1002 And Loadreq = 1 And device_status(7) = 3 Then
                    Button9_Click(sender, e)
                    istep = 3
                    step_str = ""
                End If
                If device_status(7) = 0 Then
                    If device_status(15) = 0 Then
                        istep = 10
                    End If
                End If
                step_str = "判斷到位"
            Case 3
                '等待交握完成
                If device_status(6) = 16 And device_status(9) = 2 Then
                    '206->0
                    Button1_Click_1(sender, e)
                    istep = -1
                End If
                step_str = "等待交握完成"
            Case 10
                '移動到1003
                Button5_Click_1(sender, e)
                istep = 11
                step_str = "移動到1003"
            Case 11
                '判斷到位
                If device_status(15) = 0 And device_status(16) = 1003 And UnLoadreq = 1 And device_status(7) = 0 Then
                    '第三站滾
                    Button3_Click(sender, e)
                    istep = 12
                End If
                If device_status(7) = 3 Then
                    If device_status(15) = 0 Then
                        istep = 1
                    End If
                End If

                step_str = "判斷到位"
            Case 12
                '等待交握完成
                If device_status(6) = 32 And device_status(9) = 2 Then
                    '206->0
                    Button1_Click_1(sender, e)
                    istep = -1
                End If
                step_str = "等待交握完成"

        End Select
        TextBox7.Text = istep.ToString + ":" + step_str
        TextBox1.Text = Now.ToString("yyyyMMdd HHmmssfff")
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Timer1.Start()

        TagList(0).X = 1
        TagList(0).Y = 1
        TagList(0).th = 1

        TagList(0).TagId = 1001
        Dim point As String = "100000,105180,3008,100000,103760,3007,100000,103370,3006,100000,102420,3005,100000,101470,3004,100000,101080,3003,100000,100000,3002,100000,99500,3001,98920,105180,2002,98920,100000,2001,97840,105000,1003,97840,100420,1002,97840,100000,1001"
        Dim pointlist() As String = point.Split(",")
        For i As Integer = 0 To (pointlist.Length / 3) - 1
            TagList(i).X = CInt(pointlist(3 * i))
            TagList(i).Y = CInt(pointlist(3 * i + 1))
            TagList(i).th = 0
            TagList(i).TagId = CInt(pointlist(3 * i + 2))
        Next
      
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If WriteWord(1) = 0 Then
            WriteWord(1) = 1
        Else
            WriteWord(1) = 0
        End If

        writeflag = True

    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If WriteWord(2) = 0 Then
            WriteWord(2) = 1
        Else
            WriteWord(2) = 0
        End If

        writeflag = True

    End Sub

    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        WriteWord(0) = 0
        WriteWord(1) = 1
        writeflag = True
    End Sub

    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        WriteWord(0) = 0
        WriteWord(1) = 0
        WriteWord(2) = 1
        writeflag = True
    End Sub

    Private Sub Button9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button9.Click
        WriteWord(0) = 16
        writeflag = True
    End Sub

    Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        WriteWord(0) = 0
        WriteWord(1) = 0
        WriteWord(2) = 0
        writeflag = True

    End Sub
    Function Send_CMD(ByVal car_no As Integer, ByVal From_Point As Integer, ByVal To_Point As Integer, Optional ByVal user As String = "AGVC", Optional ByVal extcmd As String = "")
        Send_CMD = 0
        Dim oConn As MySqlConnection
        Dim sqlCommand As New MySqlCommand

        oConn = New MySqlConnection(Mysql_str)
        oConn.Open()
        sqlCommand.Connection = oConn
        Dim Query As String = ""
        Dim mysql_data As Object
        Try


            If Not extcmd = "" Then
                Query = "INSERT INTO `agv_cmd_list` ( AGVNo,`CmdFrom`, `CmdTo`, `Pri_Wt`,RequestTime, `Requestor`,ext_cmd) VALUES ('" + car_no.ToString + "','" + From_Point.ToString + "', '" + To_Point.ToString + "', '50','" + Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + user + "','" + extcmd + "');"
                sqlCommand.CommandText = Query
                Send_CMD = sqlCommand.ExecuteNonQuery()
            ElseIf From_Point < 10 Then

                Query = "INSERT INTO `agv_cmd_list` ( AGVNo,`CmdFrom`, `CmdTo`, `Pri_Wt`,RequestTime, `Requestor`) VALUES ('" + car_no.ToString + "','" + From_Point.ToString + "', '" + To_Point.ToString + "', '50','" + Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + user + "');"
                Query = " insert into agv_cmd_list(AGVNo,CmdFrom,	CmdTo,Pri_Wt,RequestTime,Requestor,RequestName)					"
                Query += " SELECT A.`AGVNo` , " + From_Point.ToString + " AS from_point, " + To_Point.ToString + ",'50',now(), 'AGVC_S','AGVC_S'"
                Query += " FROM `agv_list` A LEFT JOIN agv_cmd_list B ON A.`AGVNo` = B.`AGVNo`"
                Query += " WHERE A.`AGVNo` =" + car_no.ToString
                Query += " and not " + To_Point.ToString + " in (select CmdTo FROM `agv_cmd_list` ) "
                Query += " and " + To_Point.ToString + " in (select Tag_ID FROM `point` ) "
                Query += " and not " + To_Point.ToString + " in (select Position FROM `agv_list` ) limit 0,1"
                sqlCommand.CommandText = Query
                'Cmd_status.AppendText("Query=" + Query)
                Send_CMD = sqlCommand.ExecuteNonQuery()


                ' Cmd_status.AppendText(Query + ":" + Send_CMD.ToString + vbCrLf)
            Else
                Dim to_cnt, from_cnt As Integer
                Query = "select count(*) FROM `shelf_car` where LOCATION=" + From_Point.ToString
                sqlCommand.CommandText = Query
                mysql_data = sqlCommand.ExecuteScalar()
                from_cnt = CInt(mysql_data)
                'Cmd_status.AppendText(Query + ":" + from_cnt.ToString + vbCrLf)
                Query = "select count(*) FROM `shelf_car` where LOCATION=" + To_Point.ToString
                sqlCommand.CommandText = Query
                mysql_data = sqlCommand.ExecuteScalar()
                to_cnt = CInt(mysql_data)
                'Cmd_status.AppendText(Query + ":" + to_cnt.ToString + vbCrLf)
                If from_cnt = 1 And to_cnt = 0 Then
                    'TO 點位沒有架台
                    Query = "insert into  `agv_cmd_list`(`AGVNo`,`CmdFrom`,`CmdTo`,`Pri_Wt`,`Requestor`,`Shelf_Car_No`,`Shelf_Car_type`,`Shelf_Car_Size`) select '" + car_no.ToString + "' as AGVNo,'" + From_Point.ToString + "','" + To_Point.ToString + "',50,'" + user + "',Shelf_Car_No,`Shelf_Car_type`,`Shelf_Car_Size` from `shelf_car` where LOCATION='" + From_Point.ToString + "' "
                    sqlCommand.CommandText = Query
                    Send_CMD = sqlCommand.ExecuteNonQuery()
                    '  Cmd_status.AppendText(Query + ":" + Send_CMD.ToString + vbCrLf)
                ElseIf from_cnt = 0 Then
                    Send_CMD = 0
                    '   Cmd_status.AppendText("來源端無架台" + vbCrLf)
                ElseIf to_cnt = 1 Then
                    Send_CMD = 0
                    '  Cmd_status.AppendText("目的端架台" + vbCrLf)
                End If
            End If

        Catch ex As Exception
            Send_CMD = 0
            'Cmd_status.AppendText("Query=" + Query + ":" + ex.Message)
        End Try

        oConn.Close()
        oConn.Dispose()
    End Function
    Sub Del_CMD()

        Dim oConn As MySqlConnection
        Dim sqlCommand As New MySqlCommand

        oConn = New MySqlConnection(Mysql_str)
        oConn.Open()
        sqlCommand.Connection = oConn
        Dim Query As String = "delete from agv_cmd_list where `AGVNo`=58 and CmdFrom=6"
        sqlCommand.CommandText = Query
        sqlCommand.ExecuteNonQuery()

        oConn.Close()
        oConn.Dispose()
    End Sub
    Sub CarInfo()

        Dim oConn As MySqlConnection
        Dim sqlCommand As New MySqlCommand
        Dim mReader As MySqlDataReader
        oConn = New MySqlConnection(Mysql_str)
        oConn.Open()
        sqlCommand.Connection = oConn
        Dim Query As String = "SELECT `Position`,count(B.`AGVNo`) FROM `agv_list` A left join agv_cmd_list B on A.`AGVNo`=B.`AGVNo` and not CmdFrom =6 WHERE A.`AGVNo`=58 "
        sqlCommand.CommandText = Query
        mReader = sqlCommand.ExecuteReader()
        If mReader.Read() Then
            agv.Position = CInt(mReader.Item(0))
            agv.Cmd_cnt = CInt(mReader.Item(1))
        End If



        mReader.Close()
        oConn.Close()
        oConn.Dispose()
    End Sub

    Private Sub Button10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        ModbusTCP.modbus_read_input(AGVstream, 1, 0, 10, input)
        MsgBox(input(0))
    End Sub

   

    Dim Loadreq As Integer = 0
    Dim UnLoadreq As Integer = 0

    Private Sub EQ_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles EQ.Tick
        If Not EQ_BG.IsBusy Then
            EQ_BG.RunWorkerAsync()
        End If
    End Sub

    Private Sub AGV_Timer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AGV_Timer.Tick
        If Not AGV_BG.IsBusy Then
            AGV_BG.RunWorkerAsync()
        End If

    End Sub

    Private Sub Button1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        WriteWord(0) = 0
        writeflag = True
    End Sub

    Private Sub Button4_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        Dim Topose As String = "1002"
        Dim a As String = " {'agvCode':'6680','clientCode':'admin','data':{'batchNum':'','carryPro':'','errorInfo':'','materialCode':'','materialLot':'','planNum':'','realNum':'','toolCode':''},'interfaceName':'','mapCode':'','mapShortName':'','materialLot':'','needReqCode':'','podCode':'','podDir':'','podTyp':'' " + _
     ",'priority':'','reqCode':'" + Now().ToString("yyyyMMddHHmmss") + "Cmd','reqTime':'" + Now().ToString("yyyy-MM-dd HH:mm:ss") + "','robotCode':'6680','taskCode':'','taskTyp':'M01','tokenCode':'','userCallCode':'" + Topose + "'}"
        TextBox1.Text = post("http://127.0.0.1:80/cms/services/rest/hikRpcService/genAgvSchedulingTask", a)
    End Sub
    Function post(ByVal url As String, ByVal postdata As String)
        Dim req As HttpWebRequest = CType(WebRequest.Create(url), HttpWebRequest)
        req.Method = "POST"
        Dim reqstream As Stream = req.GetRequestStream
        Dim data As Byte() = Encoding.UTF8.GetBytes(postdata)
        reqstream.Write(data, 0, data.Length)
        reqstream.Close()
        Dim respons As HttpWebResponse = CType(req.GetResponse, HttpWebResponse)
        Dim reader As StreamReader = New StreamReader(respons.GetResponseStream, Encoding.UTF8)
        post = reader.ReadToEnd
        reader.Close()
    End Function

    Private Sub Button5_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        Dim Topose As String = "1003"
        Dim a As String = " {'agvCode':'6680','clientCode':'admin','data':{'batchNum':'','carryPro':'','errorInfo':'','materialCode':'','materialLot':'','planNum':'','realNum':'','toolCode':''},'interfaceName':'','mapCode':'','mapShortName':'','materialLot':'','needReqCode':'','podCode':'','podDir':'','podTyp':'' " + _
     ",'priority':'','reqCode':'" + Now().ToString("yyyyMMddHHmmss") + "Cmd','reqTime':'" + Now().ToString("yyyy-MM-dd HH:mm:ss") + "','robotCode':'6680','taskCode':'','taskTyp':'M01','tokenCode':'','userCallCode':'" + Topose + "'}"
        TextBox1.Text = post("http://127.0.0.1:80/cms/services/rest/hikRpcService/genAgvSchedulingTask", a)
    End Sub
    Function GetStatus(ByVal status As Integer, ByRef val() As Integer) As String
        ReDim val(49)
        GetStatus = status

        Select Case status
            Case 1
                GetStatus = "任務完成"
                GetStatus = 0
            Case 2
                GetStatus = "任務執行中"
                val(15) = 4
            Case 151
                GetStatus = "小車旋轉"
                val(15) = 8
            Case 3
                GetStatus = "任務異常"
            Case 4
                GetStatus = "任務空閒"
            Case 5
                GetStatus = "機器人暫停"
                val(15) = 2
            Case 6
                GetStatus = "舉升貨架狀態"
                val(8) = 10
            Case 7
                GetStatus = "充電狀態"
                val(6) = 48
            Case 12
                GetStatus = "貨架偏角過大"
                val(19) = status
            Case 13
                GetStatus = "運動庫異常"
                val(19) = status
            Case 14
                GetStatus = "貨碼無法識別"
                val(19) = status
            Case 15
                GetStatus = "貨碼不匹配"
                val(19) = status
            Case 16
                GetStatus = "舉升異常"
                val(19) = status
            Case 17
                GetStatus = "充電樁異常"
                val(19) = status
            Case 19
                GetStatus = "充電站失聯"
                val(19) = status
            Case 23
                GetStatus = "外力下放"
            Case 24
                GetStatus = "貨架位置偏移"
            Case 91
                GetStatus = "移動貨架在下定位"
                val(19) = status
            Case 246
                GetStatus = "待機模式中"
                val(15) = 0
            Case 247
                GetStatus = "低功耗模式中"
            Case 248
                GetStatus = "休眠模式中"
            Case 249
                GetStatus = "異常休眠模式"
                val(19) = status
            Case 250
                GetStatus = "電量過低"
                val(19) = status
        End Select
    End Function

    Function GetPose(ByVal X As Integer, ByVal Y As Integer, ByVal Loc As Integer) As Integer
        Dim len As Integer = 50
        GetPose = Loc
        For i As Integer = 0 To TagList.Length - 1
            If X >= TagList(i).X - len And X <= TagList(i).X + len And Y >= TagList(i).Y - len And Y <= TagList(i).Y + len Then
                GetPose = TagList(i).TagId
                Return GetPose
            End If
        Next

    End Function

    Private Sub Button6_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        istep = -1
    End Sub

    Private Sub AGV_BG_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles AGV_BG.DoWork
        Dim AGV(20) As Integer
        Dim flag As Boolean
        AGVTcp = New TcpClient
        Try



            AGVTcp.Connect("192.168.2.182", 502)
            If AGVTcp.Connected Then
                AGVstream = AGVTcp.GetStream
                'TCPstream.WriteByte(80)
                AGVstream.ReadTimeout = 1000
                input(0) = 0
                AGV_Timer.Start()

            Else
                Exit Sub
            End If

            flag = ModbusTCP.modbus_read(AGVstream, 1, 100, 20, AGV)
            If flag Then
                device_status(6) = AGV(6)
                device_status(7) = AGV(7)
                device_status(9) = AGV(9)
            End If

            If writeflag Then
                flag = ModbusTCP.modbus_write(AGVstream, 1, 206, 2, WriteWord)
                If flag Then
                    writeflag = False
                End If
            End If
            TextBox6.Text = WriteWord(0).ToString
            If AGVTcp.Connected = False Then
                Try
                    AGVTcp.Connect("192.168.2.182", 502)
                    If AGVTcp.Connected = True Then
                        AGVstream = AGVTcp.GetStream
                    End If
                Catch ex As Exception

                End Try

            End If
            AGVstream.Close()
            AGVTcp.Close()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub EQ_BG_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles EQ_BG.DoWork
        Dim CV1(2) As Integer
        Dim CV3(2) As Integer
        Dim heart(2) As Integer
        Dim flag As Boolean
        DemoLineTcp = New TcpClient
        Try



            DemoLineTcp.Connect("192.168.2.10", 502)
            If DemoLineTcp.Connected Then
                DemoLinestream = DemoLineTcp.GetStream
                'TCPstream.WriteByte(80)
                DemoLinestream.ReadTimeout = 1000
                input(0) = 0
                EQ.Start()

            Else
                MsgBox("DemoLine連線失敗")
            End If

            flag = ModbusTCP.modbus_read(DemoLinestream, 1, 1200, 2, CV1)
            Loadreq = (CV1(0) >> 14) Mod 2
            If Loadreq = 1 Then
                Label8.BackColor = Color.Green
            Else
                Label8.BackColor = Color.Gray
            End If

            flag = ModbusTCP.modbus_read(DemoLinestream, 1, 5200, 2, CV3)
            UnLoadreq = (CV3(0) >> 14) Mod 2
            If UnLoadreq = 1 Then
                Label7.BackColor = Color.Green
            Else
                Label7.BackColor = Color.Gray
            End If
            flag = ModbusTCP.modbus_read(DemoLinestream, 1, 5302, 2, heart)
            Label46.Text = heart(0).ToString
            heartbit = heart(0)
            If DemoLineTcp.Connected = False Then
                Try
                    DemoLineTcp.Connect("192.168.2.10", 502)
                    If DemoLineTcp.Connected = True Then
                        DemoLinestream = DemoLineTcp.GetStream
                    End If
                Catch ex As Exception
                    MsgBox("EQ EX connect")
                End Try
                MsgBox("EQ connect")
            End If
            DemoLinestream.Close()
            DemoLineTcp.Close()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub Button7_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
        Timer1_Tick(sender, e)

    End Sub
End Class
