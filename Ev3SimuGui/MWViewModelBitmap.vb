Imports MonoBrickFirmware.HardwareIF

Public Class MWViewModelBitmap

	Private _ButtonHAL As ButtonHAL = ButtonHal.Init
	Private WithEvents _LcdHAL As LcdHAL = LcdHal.Init


	Public Sub New()

	End Sub

	Public ReadOnly Property ButtonHal As ButtonHAL
		Get
			Return _ButtonHAL
		End Get
	End Property

	Public ReadOnly Property LcdHal As LcdHAL
		Get
			Return _LcdHAL
		End Get
	End Property

End Class
