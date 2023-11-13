Imports System.Net.Http
Imports Microsoft.VisualBasic.ApplicationServices
Module Module1
    Sub Main()
        GC.Collect()
        If Environment.GetCommandLineArgs(1).Contains("@") Then
            bNum(Environment.GetCommandLineArgs(1))
        Else
            If IO.File.Exists(Environment.GetCommandLineArgs(2)) Then
                For Each site In IO.File.ReadLines(Environment.GetCommandLineArgs(2))
                    Console.WriteLine($"Using domain: {site}")
                    For Each user In IO.File.ReadAllLines(Environment.GetCommandLineArgs(1))
                        bNum(user + "@" + site)
                    Next
                Next
            Else
                For Each user In IO.File.ReadAllLines(Environment.GetCommandLineArgs(1))
                    bNum(user + "@" + Environment.GetCommandLineArgs(2))
                Next
            End If
        End If
    End Sub
    Function bNum(payload As String)
        Dim url As String = "https://mail.google.com/mail/gxlu?email=" + payload

        Using httpClient As New HttpClient(New HttpClientHandler() With {
            .AllowAutoRedirect = False ' Prevent automatic redirection           
        })
            Try
                Dim response As HttpResponseMessage = httpClient.GetAsync(url).Result

                If response.StatusCode = System.Net.HttpStatusCode.NoContent Then
                    ' Check for the "Set-Cookie" header in the response headers
                    For Each header In response.Headers
                        If header.Key = "Set-Cookie" Then
                            Console.WriteLine($"User Found: {url}")
                        End If
                    Next
                Else
                    Console.WriteLine($"Received a {response.StatusCode} response.")
                End If

            Catch e As HttpRequestException
                Console.WriteLine($"An error occurred: {e.Message}")
            End Try
        End Using
        Return Nothing
    End Function
End Module