Public Class SetUp
    Private Sub rdoSimple_CheckedChanged(sender As Object, e As EventArgs) Handles rdoSimple.CheckedChanged, rdoConcurrent.CheckedChanged
        'This shows or hides the Simple and Concurrent schedule controls.
        If sender.name = "rdoSimple" Then
            grpSimple.Visible = True
            grpConcurrent.Visible = False
        ElseIf sender.name = "rdoConcurrent" Then
            grpConcurrent.Visible = True
            grpSimple.Visible = False
        End If
        ResetChecks()
    End Sub
    Private Sub btnComenzar_Click(sender As Object, e As EventArgs) Handles btnComenzar.Click
        'This checks for errors or missing data in the set up and prompts the user for corrections. If no problem is found the selected programs are initiated.
        If txtSubject.Text = "" Or txtSession.Text = "" Or txtCOM.Text = "" Then
            MessageBox.Show("Some session data is missing.")
        Else
            If (rdoSimple.Checked = True And (Lever1 <> "" Or Lever2 <> "") And IsNumeric(txbValS.Text) = True And IsNumeric(txbRefs.Text) = True) Or (rdoConcurrent.Checked = True And Lever1 <> "" And Lever2 <> "" And IsNumeric(txbValCP1.Text) = True And IsNumeric(txbValCP2.Text) = True And IsNumeric(txbRefs.Text) = True) Then
                vFile = "C:\Data\" & txtSubject.Text & "_" & txtSession.Text & ".txt"
                FileOpen(1, vFile, OpenMode.Append)
                WriteLine(1, "Subject: " & txtSubject.Text)
                WriteLine(1, "Session: " & txtSession.Text)
                WriteLine(1, "Lever 1 response: 1")
                WriteLine(1, "Lever 2 response: 2")
                WriteLine(1, "Lever 1 reinforcer: 11")
                WriteLine(1, "Lever 2 reinforcer: 12")
                Dim x As New Main
                x.Show()
                x.ArduinoVB()
                Me.Visible = False
            Else
                MessageBox.Show("Some schedule or value is missing.")
            End If
        End If
    End Sub
    Private Sub Palanca1s_CheckedChanged(sender As Object, e As EventArgs) Handles rdoFRS1.CheckedChanged, rdoVRS1.CheckedChanged, rdoFIS1.CheckedChanged, rdoVIS1.CheckedChanged
        'This registers the selected option for Simple schedules on lever 1.
        Lever1 = sender.name
        Lever2 = ""
    End Sub
    Private Sub Palanca2s_CheckedChanged(sender As Object, e As EventArgs) Handles rdoFRS2.CheckedChanged, rdoVRS2.CheckedChanged, rdoFIS2.CheckedChanged, rdoVIS2.CheckedChanged
        'This registers the selected option for Simple schedules on lever 2.
        Lever2 = sender.name
        Lever1 = ""
    End Sub
    Private Sub Palanca1c_CheckedChanged(sender As Object, e As EventArgs) Handles rdoFRC1.CheckedChanged, rdoVRC1.CheckedChanged, rdoFIC1.CheckedChanged, rdoVIC1.CheckedChanged
        'This registers the selected option for Concurrent schedules on lever 1.
        Lever1 = sender.name
    End Sub

    Private Sub Palanca2c_CheckedChanged(sender As Object, e As EventArgs) Handles rdoFRC2.CheckedChanged, rdoVRC2.CheckedChanged, rdoFIC2.CheckedChanged, rdoVIC2.CheckedChanged
        'This registers the selected option for Concurrent schedules on lever 2.
        Lever2 = sender.name
    End Sub
    Private Sub ResetChecks()
        'This resets the controls when the user changes between Checkboxes. It's necessary to prevent runtime erros.
        rdoFRS1.Checked = False
        rdoVRS1.Checked = False
        rdoFIS1.Checked = False
        rdoVIS1.Checked = False
        rdoFRC1.Checked = False
        rdoVRC1.Checked = False
        rdoFIC1.Checked = False
        rdoVIC1.Checked = False
        rdoFRS2.Checked = False
        rdoVRS2.Checked = False
        rdoFIS2.Checked = False
        rdoVIS2.Checked = False
        rdoFRC2.Checked = False
        rdoVRC2.Checked = False
        rdoFIC2.Checked = False
        rdoVIC2.Checked = False
        Lever1 = ""
        Lever2 = ""
        txbValS.Text = ""
        txbValCP1.Text = ""
        txbValCP2.Text = ""
    End Sub
End Class
