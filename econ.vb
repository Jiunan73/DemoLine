            ElseIf Cmd_To = 5 Then
                '使用ext_cmd 0,0,0 來控制 並使用 109來監控完成
                cmd_list(0) = "ACTION"
                cmd_list(1) = "FINSH"
                cmd_idx = 0
       

              ElseIf Cmd_From = 2 Then
                '小命令
                If Car_type = "PIN" Then

                    If Not Cmd_To = get_tagId() Then
                        cmd_list(step_idx) = Cmd_To.ToString
                        step_idx += 1
                        cmd_list(step_idx) = "Going"
                        step_idx += 1
                    End If
                    cmd_list(step_idx) = "PINUP"
                    step_idx += 1
                ElseIf Car_type = "ROBOT" Then
                    If Not Cmd_To = get_tagId() Then
                        cmd_list(step_idx) = Cmd_To.ToString
                        step_idx += 1
                        cmd_list(step_idx) = "Going"
                        step_idx += 1
                    End If
                    cmd_list(step_idx) = "ROBOT"
                    step_idx += 1


                                Case "ACTION"
                                    Dim ext_cmd_list() As String = Car(car_idx).ext_cmd.Split(",")
                                    If ext_cmd_list.Length = 3 Then
                                        If IsNumeric(ext_cmd_list(0)) And IsNumeric(ext_cmd_list(1)) Then
                                            Car(car_idx).To_AGV(6) = CInt(ext_cmd_list(0))
                                            If Car(car_idx).get_interlock = 2 And Car(car_idx).get_action = Car(car_idx).To_AGV(6) And Car(car_idx).get_Err = 0 Then
                                                Car(car_idx).To_AGV(6) = 0
                                                Car(car_idx).To_AGV(7) = 0
                                                Car(car_idx).To_AGV(8) = 0
                                                Car(car_idx).cmd_idx += 1
                                            End If
                                        End If
                                    End If


                                Case "ROBOT"


                                    Dim ext_cmd_list() As String = Car(car_idx).ext_cmd.Split(",")

                                    If ext_cmd_list.Length = 3 Then
                                        If IsNumeric(ext_cmd_list(0)) And IsNumeric(ext_cmd_list(1)) Then

                                            Car(car_idx).To_AGV(6) = CInt(ext_cmd_list(0))
                                            Car(car_idx).To_AGV(7) = CInt(ext_cmd_list(1))
                                            Car(car_idx).To_AGV(8) = CInt(ext_cmd_list(2))
                                            If Car(car_idx).get_pin = 2 And Car(car_idx).get_action = Car(car_idx).To_AGV(6) And Car(car_idx).get_Err = 0 Then
                                                Car(car_idx).To_AGV(6) = 0
                                                Car(car_idx).To_AGV(7) = 0
                                                Car(car_idx).To_AGV(8) = 0
                                                Car(car_idx).cmd_idx += 1
                                            End If
                                        End If
                                    End If
