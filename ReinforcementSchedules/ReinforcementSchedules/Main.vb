Imports System.Math
Imports System.IO.Ports
Public Class Main
    Public Arduino As SerialPort
    Function ArduinoVB() As Integer 'This function starts the Arduino-VB communication.
        Arduino = New SerialPort(SetUp.txtCOM.Text, 9600) 'Assigns the Arduino to the selected port at a 9600 baud rate. 
        Arduino.Open() 'Starts the Arduino-VB communication.
        vTimeStart = Environment.TickCount 'Establishes a time index for timestamps.
        BeginPrograms() 'Set up for the schedules of reinforcement.
        Do 'This code will run throughout the session to allow response collection. 
            Try
                If Arduino.BytesToRead > 0 Then 'Checks for activity on the Arduino.
                    Actual_Response = Split(Arduino.ReadLine(), ",") 'Splits data from the arduino into separate responses.
                End If
                'The next following lines compare the state of the data stream of operanda 1 and 2 with previous observations to detect responses.
                If (Actual_Response(0) <> Previous_Response(0) And Actual_Response(0) <> 1) Then
                    Response(0) 'If a response is registered at operanda 1, this code will run.
                End If
                If (Actual_Response(1) <> Previous_Response(1) And Actual_Response(1) <> 1) Then
                    Response(1) 'The same happens for operanda 2.
                End If
                Previous_Response(0) = Actual_Response(0) 'This resets the data stream observation of operanda 1 to detect further responses. 
                Previous_Response(1) = Actual_Response(1) 'The same happens for operanda 2.
                vTimeNow = Environment.TickCount - vTimeStart 'This keeps track of time for the Data output file.
                lblTime.Text = vTimeNow / 1000 'This and the following 6 lines update values of interest on the main form.
                lblResponses1.Text = ResponseCount(0)
                lblResponses2.Text = ResponseCount(1)
                lblReinforcers1.Text = RefCount(0)
                lblReinforcers2.Text = RefCount(1)
                lblRfR1.Text = refRdy(0)
                lblRfR2.Text = refRdy(1)
                If RefCount(0) >= SetUp.txbRefs.Text Or RefCount(1) >= SetUp.txbRefs.Text Then Finish() 'This sets the criteria to finish the session.
            Catch ex As Exception
            End Try
            My.Application.DoEvents() 'This will enable the rest of the program to run while executing the code from above.
        Loop
        Return 0
    End Function
    Private Sub BeginPrograms() 'This checks which programs were selected and initializes them.
        VIList(0) = New List(Of Integer)
        VIList(1) = New List(Of Integer)
        lblSubject.Text = SetUp.txtSubject.Text
        lblSession.Text = SetUp.txtSession.Text
        lblCOM.Text = SetUp.txtCOM.Text
        If SetUp.rdoSimple.Checked = True Then
            If Lever1 <> "" Then lblL1.Text = Lever1.Substring(3, 2) & SetUp.txbValS.Text
            If Lever2 <> "" Then lblL2.Text = Lever2.Substring(3, 2) & SetUp.txbValS.Text
            If Lever1 = "rdoFRS1" Or Lever2 = "rdoFRS2" Then FRGen()
            If Lever1 = "rdoVRS1" Or Lever2 = "rdoVRS2" Then VRGen()
            If Lever1 = "rdoFIS1" Or Lever2 = "rdoFIS2" Then FIGen()
            If Lever1 = "rdoVIS1" Or Lever2 = "rdoVIS2" Then VIGen(0)
        ElseIf SetUp.rdoConcurrent.Checked = True Then
            lblL1.Text = Lever1.Substring(3, 2) & SetUp.txbValCP1.Text
            lblL2.Text = Lever2.Substring(3, 2) & SetUp.txbValCP2.Text
            If Lever1 = "rdoFRC1" Then FRGen()
            If Lever1 = "rdoVRC1" Then VRGen()
            If Lever1 = "rdoFIC1" Then FIGen()
            If Lever1 = "rdoVIC1" Then VIGen(0)
            If Lever2 = "rdoFRC2" Then FRGen()
            If Lever2 = "rdoVRC2" Then VRGen()
            If Lever2 = "rdoFIC2" Then FIGen()
            If Lever2 = "rdoVIC2" Then VIGen(1)
        End If
        WriteLine(1, "Lever 1 Schedule: " & lblL1.Text)
        WriteLine(1, "Lever 2 Schedule: " & lblL2.Text)
    End Sub
    Private Sub Response(Lever As Integer) 'This registers responses and checks if the reinforcer is available for both ratio and interval schedules.
        ResponseCount(Lever) += 1
        WriteLine(1, vTimeNow, Lever + 1) 'This line prints a timestamp and response on the data file. It can print any desired value with or without timestamp.
        If refRdy(Lever) = True Then Reinforce(Lever)
        Ratio(Lever)
    End Sub
    Private Sub Ratio(Lever As Integer) 'This counts responses for ratio schedules.
        If RatioGoal(Lever) <> 0 Then
            RatioCount(Lever) += 1
            If RatioCount(Lever) >= RatioGoal(Lever) Then
                refRdy(Lever) = True
                Reinforce(Lever)
                RatioCount(Lever) = 0
            End If
        End If
    End Sub
    Private Sub Reinforce(Lever As Integer) 'This registers reinforcer deliveries and sets up the next reinforcer conditions.
        refRdy(Lever) = False
        Arduino.WriteLine("R") 'This line activates the feeder through Arduino. "R" can mean any output connected to the Arduino.
        WriteLine(1, vTimeNow, Lever + 11)
        RefCount(Lever) += 1
        If Lever = 0 And (Lever1 = "rdoFRS1" Or Lever1 = "rdoVRS1" Or Lever1 = "rdoFRC1" Or Lever1 = "rdoVRC1") Then VRGen()
        If Lever = 1 And (Lever2 = "rdoFRS2" Or Lever2 = "rdoVRS2" Or Lever2 = "rdoFRC2" Or Lever2 = "rdoVRC2") Then VRGen()
        If Lever = 0 And Lever1 = "rdoFIS1" Or Lever1 = "rdoFIC1" Then FIGen()
        If Lever = 1 And Lever2 = "rdoFIS2" Or Lever2 = "rdoFIC2" Then FIGen()
        If Lever = 0 And Lever1 = "rdoVIS1" Or Lever1 = "rdoVIC1" Then VIGen(0)
        If Lever = 1 And Lever2 = "rdoVIS2" Or Lever2 = "rdoVIC2" Then VIGen(1)
    End Sub
    Private Sub FRGen() 'This initializes Fixed Ratio schedules depending on the selected values / operanda.
        'FR schedules just check current responses against the specified schedule value.
        If Lever1 = "rdoFRS1" Then RatioGoal(0) = SetUp.txbValS.Text
        If Lever2 = "rdoFRS2" Then RatioGoal(1) = SetUp.txbValS.Text
        If Lever1 = "rdoFRC1" Then RatioGoal(0) = SetUp.txbValCP1.Text
        If Lever2 = "rdoFRC2" Then RatioGoal(1) = SetUp.txbValCP2.Text
    End Sub
    Private Sub VRGen() 'This initializes Variable Ratio schedules depending on the selected values / operanda.
        'VR schedules calculate a range between half and 1.5 times the specified schedule value and pick a random value from that range. 
        Randomize()
        Dim Rand As New Random
        If Lever1 = "rdoVRS1" Then RatioGoal(0) = Rand.Next((SetUp.txbValS.Text / 2), (SetUp.txbValS.Text * 1.5))
        If Lever2 = "rdoVRS2" Then RatioGoal(1) = Rand.Next((SetUp.txbValS.Text / 2), (SetUp.txbValS.Text * 1.5))
        If Lever1 = "rdoVRC1" Then RatioGoal(0) = Rand.Next((SetUp.txbValCP1.Text / 2), (SetUp.txbValCP1.Text * 1.5))
        If Lever2 = "rdoVRC2" Then RatioGoal(1) = Rand.Next((SetUp.txbValCP2.Text / 2), (SetUp.txbValCP2.Text * 1.5))
    End Sub
    Private Sub FIGen() 'This initializes Fixed Interval schedules depending on the selected values / operanda.
        'FI schedules use a timer to check if the specified schedule value has elapsed.
        'Visual Basic manages time in miliseconds, so values are multiplied by 1000.
        If Lever1 = "rdoFIS1" Then
            tmrLever1.Interval = SetUp.txbValS.Text * 1000
            tmrLever1.Enabled = True
        End If
        If Lever2 = "rdoFIS2" Then
            tmrLever2.Interval = SetUp.txbValS.Text * 1000
            tmrLever2.Enabled = True
        End If
        If Lever1 = "rdoFIC1" Then
            tmrLever1.Interval = SetUp.txbValCP1.Text * 1000
            tmrLever1.Enabled = True
        End If
        If Lever2 = "rdoFIC2" Then
            tmrLever2.Interval = SetUp.txbValCP2.Text * 1000
            tmrLever2.Enabled = True
        End If
    End Sub
    Private Sub VIGen(list As Integer) 'This initializes Variable Interval schedules depending on the selected values / operanda.
        'VI schedules employ Hantula's (1991) method for Fleshler & Hoffman's (1968) iterative equation on Visual Basic.
        'This Subroutine takes 'list' as a value to allocate VI iterations on separate data arrays, allowing for concurrent and independent VI schedules.
        'VI values are selected at random from the lists without replacement. 
        Dim v
        Dim n = 10 'This value represents the VI iterations. 
        Dim rd(n)
        Dim vi(n)
        Dim order
        Randomize()
        If SetUp.rdoSimple.Checked = True Then v = SetUp.txbValS.Text
        If SetUp.rdoConcurrent.Checked = True Then
            If list = 0 Then v = SetUp.txbValCP1.Text
            If list = 1 Then v = SetUp.txbValCP1.Text
        End If
        If VIList(list).Count = 0 Then
            For m As Integer = 1 To n
                If m = n Then vi(m) = v * (1 + Log(n)) : GoTo 1
                vi(m) = v * (1 + (Log(n)) + (n - m) * (Log(n - m)) - (n - m + 1) * Log(n - m + 1))
1:              order = Int((n * Rnd() + 1))
                If rd(order) = 0 Then
                    rd(order) = vi(m)
                Else
                    GoTo 1
                End If
            Next
            For a As Integer = 1 To n
                VIList(list).Add(rd(a))
            Next
        End If
        Dim Rand As New Random
        Dim p As Integer = Rand.Next(VIList(list).Count)
        If list = 0 Then 'The first list saves VI values for schedules on operanda 1.
            tmrLever1.Interval = (VIList(list).Item(p) + 1) * 1000
            tmrLever1.Enabled = True
        End If
        If list = 1 Then 'The second list saves VI values for schedules on operanda 2.
            tmrLever2.Interval = (VIList(list).Item(p) + 1) * 1000
            tmrLever2.Enabled = True
        End If
        VIList(list).RemoveAt(p)
    End Sub
    Private Sub tmrSchedule1_Tick(sender As Object, e As EventArgs) Handles tmrLever1.Tick 'This keeps track of time for interval schedules on operanda 1.
        tmrLever1.Enabled = False
        refRdy(0) = True
    End Sub
    Private Sub tmrSchedule2_Tick(sender As Object, e As EventArgs) Handles tmrLever2.Tick 'This keeps track of time for interval schedules on operanda 2.
        tmrLever2.Enabled = False
        refRdy(1) = True
    End Sub
    Private Sub Finish() 'This
        Arduino.WriteLine("F") 'Shuts down all outputs on the Arduino.
        Arduino.Close() 'Terminates Arduino-VB communication.
        WriteLine(1, "END") 'Signals that the session has ended on the data file.
        FileClose(1) 'Closes data file.
        End
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click 'This controls the 'Finish' button on the main form. Used to end the session by hand.
        Finish()
    End Sub
End Class