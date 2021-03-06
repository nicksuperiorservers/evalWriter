﻿Option Strict On
Public Class frmEvalWriter
    Private FileName As String
    '
    'Create Evals subdirectory if it doesn't exist
    '
    Private Sub frmEvalWriter_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Threading.Thread.Sleep(5000)
        If My.Computer.FileSystem.DirectoryExists(My.Computer.FileSystem.CurrentDirectory.ToString() & "\Evals") = False Then
            My.Computer.FileSystem.CreateDirectory("Evals")
        End If
    End Sub
    '
    'Clock Handler
    '
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        lblProgramLocation.Text = My.Computer.FileSystem.CurrentDirectory.ToString()
        lblTime.Text = TimeOfDay.ToString("hh:mm:ss tt")
    End Sub
    '
    'Starts eval creating on button click
    '
    Private Sub btnBegin_Click(sender As Object, e As EventArgs) Handles btnBegin.Click
        'Init Variables
        Dim FileNum As Integer = FreeFile()
        Dim strPersonName As String
        Dim strPersonSteamID As String
        Dim strInput(5) As String
        Dim strTitle As String
        Dim strYourName As String
        Dim strYourSteamID As String
        Dim strPersonRank As String
        Dim steamid64 As String
        Dim blnGoodSteamID As Boolean
        Dim blnGoodName As Boolean
        Dim blnGoodRank As Boolean
        Dim trimmedFileName As String
        'If T-Admin ask for their name
        If radTAdmin.Checked Or radSSRP.Checked Then 'Since SSRP uses their name, regardless of rank
            Do
                Try
                    strYourName = InputBox("Enter your name:", "", " ")
                    If strYourName = "" Then
                        Select Case MsgBox("Are you sure you want to cancel?", vbYesNo, "You sure?")
                            Case MsgBoxResult.Yes
                                Exit Sub
                        End Select
                    End If
                    If strYourName.Contains(":") Then
                        blnGoodName = False
                        MsgBox("Invalid Name. Try again.", vbExclamation, "Yikes!")
                    Else
                        blnGoodName = True
                    End If
                Catch ex As Exception
                    MsgBox("Invalid Name. Try again.", vbExclamation, "Yikes!")
                    blnGoodName = False
                End Try
            Loop Until blnGoodName
            If radSSRP.Checked Then
                Do
                    Try
                        strYourSteamID = InputBox("Enter your SteamID:", "", " ")
                        If strYourSteamID = "" Then
                            Select Case MsgBox("Are you sure you want to cancel?", vbYesNo, "You sure?")
                                Case MsgBoxResult.Yes
                                    Exit Sub
                            End Select
                        ElseIf strYourSteamID.Contains(":") AndAlso strYourSteamID.Substring(0, 8) = "STEAM_0:" Then
                            blnGoodSteamID = True
                        ElseIf strYourSteamID.StartsWith("7") AndAlso strYourSteamID.Length = 76561197960265728.ToString.Length Then
                            If CLng(strYourSteamID) Mod 2 = 0 Then
                                strYourSteamID = "STEAM_0:0:" + ((CLng(strYourSteamID) - 76561197960265728) \ 2).ToString
                            Else
                                strYourSteamID = "STEAM_0:1:" + ((CLng(strYourSteamID) - 76561197960265728) \ 2).ToString
                            End If
                        End If
                    Catch ex As Exception
                        MsgBox("Invalid SteamID. Try again.", vbExclamation, "Yikes!")
                        blnGoodSteamID = False
                    End Try
                Loop Until blnGoodSteamID
            End If
        End If
        blnGoodName = False 'for use in evalutee writer's name
        blnGoodSteamID = False
        'Ask for evaluator name and steamID
        Do
            Try
                strPersonName = InputBox("Enter Evalutee's name:", "", " ")
                If strPersonName = "" Then
                    Select Case MsgBox("Are you sure you want to cancel?", vbYesNo, "You sure?")
                        Case MsgBoxResult.Yes
                            Exit Sub
                    End Select
                End If
                If strPersonName.Contains(":") Then
                    blnGoodName = False
                    MsgBox("Invalid Name. Try again.", vbExclamation, "Yikes!")
                Else
                    blnGoodName = True
                End If
            Catch ex As Exception
                MsgBox("Invalid Name. Try again.", vbExclamation, "Yikes!")
                blnGoodName = False
            End Try
        Loop Until blnGoodName
        Do
            Try
                strPersonSteamID = InputBox("Enter Evalutee's SteamID:", "", " ")
                If strPersonSteamID = "" Then
                    Select Case MsgBox("Are you sure you would like to cancel?", vbYesNo, "Are you sure?")
                        Case MsgBoxResult.Yes
                            Exit Sub
                    End Select
                End If
                Dim pos As Integer
                'Start Conversion to Steam64
                If strPersonSteamID.Contains(":") Then
                    pos = InStrRev(strPersonSteamID, ":")
                End If
                trimmedFileName = strPersonSteamID.Substring(pos, strPersonSteamID.Length - pos)
                If strPersonSteamID.StartsWith("STEAM_0:0:") Then
                    steamid64 = ((CInt(trimmedFileName) * 2) + 76561197960265728).ToString
                    blnGoodSteamID = True
                ElseIf strPersonSteamID.StartsWith("STEAM_0:1:") Then
                    steamid64 = ((CInt(trimmedFileName) * 2) + 76561197960265729).ToString
                    blnGoodSteamID = True
                ElseIf strPersonSteamID.StartsWith("7") AndAlso strPersonSteamID.Length = 76561197960265728.ToString.Length Then
                    steamid64 = strPersonSteamID
                    If CLng(strPersonSteamID) Mod 2 = 0 Then
                        strPersonSteamID = "STEAM_0:0:" + ((CLng(strPersonSteamID) - 76561197960265728) \ 2).ToString
                    Else
                        strPersonSteamID = "STEAM_0:1:" + ((CLng(strPersonSteamID) - 76561197960265728) \ 2).ToString
                    End If
                    blnGoodSteamID = True
                End If
            Catch ex As Exception
                MsgBox("Invalid STEAMID!", vbExclamation, "Yikes!")
                blnGoodSteamID = False
            End Try
        Loop Until blnGoodSteamID
        If radSSRP.Checked Then
            Do
                Try
                    strPersonRank = InputBox("Enter Evaluatee's Rank:", "", " ")
                    If strPersonRank = "" Then
                        Select Case MsgBox("Are you sure you want to cancel?", vbYesNo, "You sure?")
                            Case MsgBoxResult.Yes
                                Exit Sub
                        End Select
                    ElseIf IsNumeric(strPersonRank) = False Then
                        blnGoodRank = True
                    End If
                Catch ex As Exception
                    MsgBox("Invalid Rank. Try again.", vbExclamation, "Yikes!")
                    blnGoodRank = True
                End Try
            Loop Until blnGoodRank
        End If
        'Create Textfile for eval
        FileName = strPersonName & " - " & steamid64
            FileOpen(FileNum, My.Computer.FileSystem.CurrentDirectory.ToString() & "\Evals\" & FileName & ".txt", OpenMode.Append)
            'Gather eval information
            For i = 0 To 5
            If i = 0 Then
                strTitle = "How is " & strPersonName & "'s attitude in RP/sits?"
                strInput(0) = InputBox(strTitle, " ", " ")
                If strInput(0).Substring(strInput(0).Length - 1, 1) <> "." Then
                    strInput(0) += "."
                End If
            ElseIf i = 1 Then
                strTitle = "How does " & strPersonName & " handle/conduct sits?"
                    strInput(1) = InputBox(strTitle, " ", " ")
                If strInput(1).Substring(strInput(1).Length - 1, 1) <> "." Then
                    strInput(1) += "."
                End If
            ElseIf i = 2 Then
                    strTitle = "Does " & strPersonName & " understand the rules and the punishments for them?"
                    strInput(2) = InputBox(strTitle, " ", " ")
                If strInput(2).Substring(strInput(2).Length - 1, 1) <> "." Then
                    strInput(2) += "."
                End If
            ElseIf i = 3 Then
                    strTitle = "How is " & strPersonName & "'s activity?"
                    strInput(3) = InputBox(strTitle, " ", " ")
                If strInput(3).Substring(strInput(3).Length - 1, 1) <> "." Then
                    strInput(3) += "."
                End If
            ElseIf i = 4 Then
                    strTitle = "What other useful information can be provided about " & strPersonName & "?"
                    strInput(4) = InputBox(strTitle, " ", " ")
                If strInput(4).Substring(strInput(4).Length - 1, 1) <> "." Then
                    strInput(4) += "."
                End If
            ElseIf i = 5 Then
                    strTitle = "Overall, how do you rate " & strPersonName & " out of 10?"
                    strInput(5) = InputBox(strTitle, " ", " ")
                If strInput(5).Substring(strInput(5).Length - 1, 1) <> "." Then
                    strInput(5) += "."
                End If
            End If
                'Suggest copying to clipboard
            Next
        PrintLine(FileNum, strInput(0) & " " & strInput(1) & " " & strInput(2) & " " & strInput(3) & " " & strInput(4) & " " & strInput(5))
        If radDarkRP.Checked Then
            If radPAdmin.Checked Then
                'Copy to clipboard
                Clipboard.SetText(strPersonName & " - " & strPersonSteamID & Environment.NewLine & strInput(0) & " " & strInput(1) & " " & strInput(2) & " " & strInput(3) & " " & strInput(4) & " " & strInput(5))
                'Suggest opening P-Admin+ chat
                Select Case MsgBox("Would you like to open the P-Admin chat?", vbYesNo, "Open chat?")
                    Case MsgBoxResult.Yes
                        Process.Start("https://forum.superiorservers.co/messenger/71/?page=9999")
                End Select
                'If T-admin
            ElseIf radTAdmin.Checked Then
                'Copy to clipboard
                Clipboard.SetText(strYourName & "'s Eval - " & strPersonName & " - " & strPersonSteamID & Environment.NewLine & strInput(0) & " " & strInput(1) & " " & strInput(2) & " " & strInput(3) & " " & strInput(4) & " " & strInput(5))
                Process.Start("steam://url/SteamIDPage/76561198124391666")
                Process.Start("steam://friends/message/76561198124391666")
                MsgBox("Nick(STEAM_0:0:82062969)'s steam profile page & PM opened.", vbInformation, "Opened")
                Me.Activate()
            End If
        ElseIf radSSRP.Checked Then
            If radPAdmin.Checked Then
                'Copy to clipboard
                Clipboard.SetText("[b]Evaluator[/b]" & Environment.NewLine & "Name: " & strYourName & Environment.NewLine & "SteamID: " & strYourSteamID & Environment.NewLine & Environment.NewLine & "[b]Evaluatee/s[/b]" & Environment.NewLine & "Name: " & strPersonName & Environment.NewLine & "SteamID: " & strPersonSteamID & Environment.NewLine & "Staff Rank: " & strPersonRank & Environment.NewLine & strInput(0) & " " & strInput(1) & " " & strInput(2) & " " & strInput(3) & " " & strInput(4) & " " & strInput(5))
                'Suggest opening P-Admin+ chat
                Select Case MsgBox("Would you like to open the P-Admin chat?", vbYesNo, "Open chat?")
                    Case MsgBoxResult.Yes
                        Process.Start("https://forum.superiorservers.co/messenger/36/?page=9999")
                End Select
                'If T-admin
            ElseIf radTAdmin.Checked Then
                'Copy to clipboard
                Clipboard.SetText("[b]Evaluator[/b]" & Environment.NewLine & "Name: " & strYourName & Environment.NewLine & "SteamID: " & strYourSteamID & Environment.NewLine & Environment.NewLine & "[b]Evaluatee/s[/b]" & Environment.NewLine & "Name: " & strPersonName & Environment.NewLine & "SteamID: " & strPersonSteamID & Environment.NewLine & "Staff Rank: " & strPersonRank & Environment.NewLine & strInput(0) & " " & strInput(1) & " " & strInput(2) & " " & strInput(3) & " " & strInput(4) & " " & strInput(5))
                Process.Start("steam://url/SteamIDPage/76561198332673358")
                Process.Start("steam://friends/message/76561198332673358")
                MsgBox("Jonathan(STEAM_0:0:186203815)'s steam profile page & PM opened.", vbInformation, "Opened")
                Me.Activate()
            End If
        End If
        FileClose(FileNum)
    End Sub
    '
    'Read written Evals
    '
    Private Sub ReadToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ReadToolStripMenuItem.Click
        Dim myStream As IO.Stream = Nothing
        Dim strText As String
        Dim strPath As String = lblProgramLocation.Text
        Dim FileDialog As New OpenFileDialog()
        If FileDialog.ShowDialog = DialogResult.OK Then
            myStream = FileDialog.OpenFile
            If myStream IsNot Nothing Then
                strText = My.Computer.FileSystem.ReadAllText(FileDialog.FileName)
                FileName = IO.Path.GetFileName(FileDialog.FileName).Substring(0, IO.Path.GetFileName(FileDialog.FileName).Length - 4)
                MsgBox(strText, vbInformation, FileName)
                myStream.Close()
            Else
                MsgBox("File is empty", vbCritical, "Yikes!")
            End If
        End If
    End Sub
End Class
