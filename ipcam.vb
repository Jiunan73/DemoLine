  Private Sub BackgroundWorker2_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker2.DoWork
        Dim myUri As Uri
        Dim LoginAccount, LoginPass, list, curpath As String
        Dim strUrl As String


        myUri = New Uri("http://192.168.8.92/ISAPI/Streaming/channels/101/picture?videoResolutionWidth=1280&videoResolutionHeight=720")
        LoginAccount = "admin"
        LoginPass = "hk888888"
        list = "92"

        Dim showimage As Image
        Try
            Dim request As WebRequest = WebRequest.Create(myUri)
            request.Timeout = 2000
            Dim delurl As String = "D:\wamp\www\ipcam\" + list + "\" + Now().AddDays(-7).ToString("yyyyMMdd") + "\" + Now().AddDays(-7).ToString("yyyyMMddHHmmss") + ".jpg"
            Dim cred As NetworkCredential = New NetworkCredential(LoginAccount, LoginPass)
            request.Credentials = cred
            Dim response As WebResponse = request.GetResponse()
            Dim dataStream As Stream = response.GetResponseStream
            showimage = Image.FromStream(dataStream)
            dataStream.Close()
            response.Close()
            strUrl = "D:\wamp\www\ipcam\" + list + "\" + Now().ToString("yyyyMMdd") + "\"

            If Not IO.Directory.Exists(strUrl) Then
                IO.Directory.CreateDirectory(strUrl)
            End If
            strUrl += Now().ToString("yyyyMMddHHmmss") + ".jpg"
            curpath = "D:\wamp\www\ipcam\" + list + "\Currently.jpg"
            ' PictureBox1.Image = showimage
            ' PictureBox1.Refresh()
            showimage.Save(strUrl)
            showimage.Save(curpath)
            img(1) = showimage
            If File.Exists(delurl) Then
                File.Delete(delurl)
            End If
            delurl = "D:\wamp\www\ipcam\" + list + "\" + Now().AddDays(-12).ToString("yyyyMMdd") + "\" + Now().AddDays(-12).ToString("yyyyMMddHHmmss") + ".jpg"
            If File.Exists(delurl) Then
                File.Delete(delurl)
            End If
            delurl = "D:\wamp\www\ipcam\" + list + "\" + Now().AddDays(-17).ToString("yyyyMMdd") + "\" + Now().AddDays(-17).ToString("yyyyMMddHHmmss") + ".jpg"
            If File.Exists(delurl) Then
                File.Delete(delurl)
            End If
        Catch ex As Exception

        End Try

    End Sub

