Imports System.Net.Sockets
Imports System.Threading
Module ModbusTCP
    Dim cnt As Integer = 1
    Dim err As Integer = 0

    Function modbus_read(ByRef netstream As NetworkStream, ByVal device_no As Integer, ByVal addr As Integer, ByVal iSize As Integer, ByRef Response() As Integer) As Boolean

        Dim Wbyte(20) As Byte
        Dim Rbyte(100) As Byte

        Wbyte(0) = cnt \ 256
        Wbyte(1) = cnt Mod 256 ' 流水號
        Wbyte(2) = 0
        Wbyte(3) = 0
        Wbyte(4) = 0
        Wbyte(5) = 6 '後面有幾個字元
        Wbyte(6) = device_no 'ID
        Wbyte(7) = 3 'Fn
        Wbyte(8) = addr \ 256  'Addr
        Wbyte(9) = addr Mod 256 'Addr
        Wbyte(10) = iSize \ 256 'Count
        Wbyte(11) = iSize Mod 256 'Count  
        Try
            netstream.Write(Wbyte, 0, 12)
            cnt += 1
            If cnt > 255 Then
                cnt = 1
            End If
            Thread.Sleep(100)
            '  Do
            '6+1+1+1

            Dim len As Integer = netstream.Read(Rbyte, 0, 100)

            If len >= 13 Then


                If Rbyte(5) = Rbyte(8) + 3 Then
                    Dim b As String = ""
                    For i As Integer = 0 To Rbyte(8) / 2 - 1
                        Response(i) = Rbyte(9 + i * 2) * 256 + Rbyte(10 + i * 2)
                    Next
                    For i As Integer = 0 To len - 1
                        b += Rbyte(i).ToString + " "
                    Next


                    Return True


                End If
            End If
            modbus_read = False
        Catch ex As Exception
            modbus_read = False
            netstream.Close()
        End Try



    End Function
    Function modbus_write(ByRef netstream As NetworkStream, ByVal device_no As Integer, ByVal addr As Integer, ByVal iSize As Integer, ByVal val() As Integer) As Boolean

        Dim Wbyte(200) As Byte
        Dim Rbyte(100) As Byte
        Dim len As Integer = iSize * 2 + 13
        modbus_write = False
        Wbyte(0) = cnt \ 256
        Wbyte(1) = cnt Mod 256 ' 流水號
        Wbyte(2) = 0
        Wbyte(3) = 0
        Wbyte(4) = 0
        Wbyte(5) = 7 + iSize * 2 '字數
        Wbyte(6) = 1
        Wbyte(7) = 16 'fn
        Wbyte(8) = addr \ 256  'addr
        Wbyte(9) = addr Mod 256 'addr
        Wbyte(10) = iSize \ 256  ' 
        Wbyte(11) = iSize Mod 256 '
        Wbyte(12) = iSize * 2
        For i As Integer = 0 To iSize - 1
            Wbyte(13 + i * 2) = val(i) \ 256
            Wbyte(14 + i * 2) = val(i) Mod 256
        Next
        If cnt > 255 Then
            cnt = 1
        End If
        Try
            netstream.Write(Wbyte, 0, len)
            Thread.Sleep(100)

            modbus_write = False

            If netstream.Read(Rbyte, 0, 100) = 12 Then
                If Rbyte(11) = iSize Then
                    Return True
                End If
            End If
        Catch ex As Exception
            modbus_write = False
            netstream.Close()

        End Try
    End Function

    Function modbus_read_input(ByRef netstream As NetworkStream, ByVal device_no As Integer, ByVal addr As Integer, ByVal iSize As Integer, ByRef Response() As Integer) As Boolean

        Dim Wbyte(20) As Byte
        Dim Rbyte(100) As Byte

        Wbyte(0) = cnt \ 256
        Wbyte(1) = cnt Mod 256 ' 流水號
        Wbyte(2) = 0
        Wbyte(3) = 0
        Wbyte(4) = 0
        Wbyte(5) = 6 '後面有幾個字元
        Wbyte(6) = device_no 'ID
        Wbyte(7) = 2 'Fn
        Wbyte(8) = addr \ 256  'Addr
        Wbyte(9) = addr Mod 256 'Addr
        Wbyte(10) = iSize \ 256 'Count
        Wbyte(11) = iSize Mod 256 'Count
        netstream.Write(Wbyte, 0, 12)
        cnt += 1
        If cnt > 255 Then
            cnt = 1
        End If
        Thread.Sleep(100)
        '  Do
        '6+1+1+1
        Try
            Dim len As Integer = netstream.Read(Rbyte, 0, 100)

            If len > 10 Then


                If Rbyte(5) = Rbyte(8) + 3 Then
                    Dim b As String = ""
                    For i As Integer = 0 To Rbyte(8) / 2 - 1
                        Response(i) = Rbyte(9 + i * 2) * 256 + Rbyte(10 + i * 2)
                    Next
                    For i As Integer = 0 To len - 1
                        b += Rbyte(i).ToString + " "
                    Next


                    Return True


                End If
            End If
            modbus_read_input = False
        Catch ex As Exception
            modbus_read_input = False
        End Try



    End Function
End Module
