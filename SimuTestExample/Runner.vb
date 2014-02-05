Imports MonoBrickFirmware.UserInput
Imports System.Threading
Imports MonoBrickFirmware.Display

Public Class Runner

	Dim WithEvents ButtEvs As New ButtonEvents
	Dim KeyScreen As New Lcd

	Dim ot As New Thread(AddressOf run)

	Public Sub RunAsThread()
		ot.Start()
	End Sub

	Public Sub run()
		'MsgBox("Let's start.")

		Dim Butt As New Buttons
		Dim LastButtonState As Buttons.ButtonStates
		Dim p As String = "C:\Users\Martin\Documents\Visual Studio 2013\Projects\MonoBrick\Ev3Simu\TestProg"
		Dim Screen1 As New Lcd
		Dim Screen2 As New Lcd
		'KeyScreen = New Lcd
		Screen1.WriteText(Font.SmallFont, New Point(0, 0), "Hello from screen 1", True)
		Screen1.Update()
		Thread.Sleep(1000)
		For i = 1 To 15
			LcdConsole.WriteLine("Text{0} from WriteLine", i)
			Thread.Sleep(250)
		Next
		Thread.Sleep(1000)
		Screen2.WriteText(Font.SmallFont, New Point(0, 20), "Hello from screen 2", True)
		Screen2.Update()
		Thread.Sleep(1000)
		Screen1.WriteText(Font.SmallFont, New Point(10, 100), "Here is screen 1 again!", True)
		Screen1.Update()
		Do
			LastButtonState = Butt.GetButtonStates
			'Console.Beep()
			'Console.WriteLine(LastButtonState.ToString())
			Threading.Thread.Sleep(50)
		Loop Until LastButtonState = Buttons.ButtonStates.Escape

		MsgBox("Done.")
	End Sub

	Private Sub KeyReleased() Handles ButtEvs.DownReleased, ButtEvs.EnterReleased, ButtEvs.EscapeReleased, _
		ButtEvs.LeftReleased, ButtEvs.RightReleased, ButtEvs.UpReleased
		KeyLogger(Nothing)
	End Sub

	Private Sub EnterPressed() Handles ButtEvs.EnterPressed
		KeyLogger("Enter")
	End Sub

	Private Sub EscapePressed() Handles ButtEvs.EscapePressed
		KeyLogger("Escape")
	End Sub
	Private Sub LeftPressed() Handles ButtEvs.LeftPressed
		KeyLogger("Left")
	End Sub
	Private Sub RightPressed() Handles ButtEvs.RightPressed
		KeyLogger("Right")
	End Sub

	Private Sub UpPressed() Handles ButtEvs.UpPressed
		KeyLogger("Up")
	End Sub
	Private Sub DownPressed() Handles ButtEvs.DownPressed
		KeyLogger("Down")
	End Sub

	Public Sub StopThread()
		If ot.IsAlive Then
			ot.Abort()
		End If
	End Sub

	Private Sub KeyLogger(Key As String)
		Dim lp As New Point(0, 0)
		Const prefix As String = "Key pressed: "
		If Key Is Nothing Then
			KeyScreen.WriteText(Font.SmallFont, lp, prefix & "           ", True)
		Else
			KeyScreen.WriteText(Font.SmallFont, lp, prefix & Key, True)
		End If
		KeyScreen.Update()
	End Sub

End Class
