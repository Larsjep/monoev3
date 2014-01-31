Imports MonoBrickFirmware
Imports MonoBrickFirmware.UserInput
Imports MonoBrickFirmware.HardwareIF
Imports SimuTestExample

Class MainWindow

	Private _viewModel As MWViewModelBitmap
	Dim MyTest As New Runner

	Public Sub New()

		' This call is required by the designer.
		InitializeComponent()

		' Add any initialization after the InitializeComponent() call.
		_viewModel = New MWViewModelBitmap
		Me.DataContext = _viewModel
	End Sub

	Private Sub StartClicked(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles bStart.Click
		MyTest.RunAsThread()
	End Sub

	Private Sub MainWindow_Unloaded(sender As Object, e As RoutedEventArgs) Handles Me.Unloaded
		MyTest.StopThread()
	End Sub


	Private Sub AllButtonsMouseUp(sender As Object, e As MouseButtonEventArgs) Handles _
	bEnter.PreviewMouseUp, bEscape.PreviewMouseUp, bLeft.PreviewMouseUp, bRight.PreviewMouseUp, bUp.PreviewMouseUp, bDown.PreviewMouseUp
		_viewModel.ButtonHal.CurrentButton = Buttons.ButtonStates.None
	End Sub

	Private Sub bEnter_PreviewMouseDown(sender As Object, e As MouseButtonEventArgs) Handles bEnter.PreviewMouseDown
		_viewModel.ButtonHal.CurrentButton = Buttons.ButtonStates.Enter
	End Sub


	Private Sub bEscape_PreviewMouseDown(sender As Object, e As MouseButtonEventArgs) Handles bEscape.PreviewMouseDown
		_viewModel.ButtonHal.CurrentButton = Buttons.ButtonStates.Escape
	End Sub

	Private Sub bLeft_PreviewMouseDown(sender As Object, e As MouseButtonEventArgs) Handles bLeft.PreviewMouseDown
		_viewModel.ButtonHal.CurrentButton = Buttons.ButtonStates.Left
	End Sub

	Private Sub bRight_PreviewMouseDown(sender As Object, e As MouseButtonEventArgs) Handles bRight.PreviewMouseDown
		_viewModel.ButtonHal.CurrentButton = Buttons.ButtonStates.Right
	End Sub

	Private Sub bUp_PreviewMouseDown(sender As Object, e As MouseButtonEventArgs) Handles bUp.PreviewMouseDown
		_viewModel.ButtonHal.CurrentButton = Buttons.ButtonStates.Up
	End Sub

	Private Sub bDown_PreviewMouseDown(sender As Object, e As MouseButtonEventArgs) Handles bDown.PreviewMouseDown
		_viewModel.ButtonHal.CurrentButton = Buttons.ButtonStates.Down
	End Sub


End Class
