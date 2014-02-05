Imports MonoBrickFirmware.Display

Public Class BytesToImageConverter
	Implements IValueConverter

	Private PixelFormat As PixelFormat = PixelFormats.BlackWhite
	Private CorrectBytes As Byte()
	Private BytesPerLine As Integer = CInt((Lcd.Width * PixelFormat.BitsPerPixel + 7) / 8)

	Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
		Dim BmpSrc As System.Windows.Media.Imaging.BitmapSource

		If value Is Nothing Then
			' create white screen
			CreateEmptyScreen()
		Else
			'CorrectBitOrder(CType(value, Byte()))
			CorrectBytes = CType(value, Byte())
		End If

		BmpSrc = ByteArrayToBitmapSource(CorrectBytes)
		Return BmpSrc
	End Function

	Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
		Return Nothing
	End Function

	Private Sub CreateEmptyScreen()
		If CorrectBytes Is Nothing Then
			ReDim CorrectBytes(Lcd.Height * BytesPerLine - 1)
		End If
		For i = LBound(CorrectBytes) To UBound(CorrectBytes)
			CorrectBytes(i) = 255
		Next
	End Sub

	'Private Sub CorrectBitOrder(ReverseBitByteArray As Byte())
	'	For i = LBound(ReverseBitByteArray) To UBound(ReverseBitByteArray)
	'		CorrectBytes(i) = ConvertBits(ReverseBitByteArray(i))
	'	Next
	'End Sub

	'Private Function ConvertBits(WrongByte As Byte) As Byte
	'	' Reverse sequence and invert bits
	'	Dim NewValue As Byte = 0
	'	If WrongByte = 0 Then
	'		NewValue = 255
	'	Else
	'		Dim Mask As Byte = 128
	'		Dim NewBit As Byte = 1
	'		For i = 0 To 7
	'			If (WrongByte And Mask) > 0 Then
	'				' nothing to do
	'			Else
	'				NewValue = NewValue + NewBit
	'			End If
	'			Mask = Mask >> 1
	'			NewBit = NewBit << 1
	'		Next
	'	End If
	'	Return NewValue
	'End Function


	Private Function ByteArrayToBitmapSource(ByteArray() As Byte) As System.Windows.Media.Imaging.BitmapSource
		Dim BmpSrc As System.Windows.Media.Imaging.BitmapSource
		Dim Width As Integer = Lcd.Width
		Dim Height As Integer = Lcd.Height
		Dim dpiX As Double = 200
		Dim dpiY As Double = 200
		Dim BitmapPalette As BitmapPalette = BitmapPalettes.BlackAndWhite
		BmpSrc = System.Windows.Media.Imaging.BitmapSource.Create _
			(Width, Height, dpiX, dpiY, PixelFormat, BitmapPalette, ByteArray, BytesPerLine)
		Return BmpSrc
	End Function

	Private Function ByteToBinary(Value As Byte) As String
		Dim Mask As Byte = 128
		Dim s As String = ""
		For i = 1 To 8
			If (Value And Mask) > 0 Then
				s = s & "1"
			Else
				s = s & "0"
			End If
			Mask = CByte(Mask \ 2)
		Next
		Return s
	End Function

	Private Function BinaryToByte(Bits As String) As Byte
		Dim Mask As Byte = 128
		Dim s As String = Bits
		Dim b As Byte = 0
		For i = 1 To 8
			If Mid(s, i, 1) = "1" Then
				b = b + Mask
			End If
			Mask = CByte(Mask \ 2)
		Next
		Return b
	End Function


End Class
