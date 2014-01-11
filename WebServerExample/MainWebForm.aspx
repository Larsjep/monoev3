<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MainWebForm.aspx.cs" Inherits="WebServerExample.MainWebForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>MonoBrick WebServer Example</title>
</head>
<body>
  <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:Timer ID="SensorTimer" runat="server" OnTick="SensorTimer_Tick" Interval="1000"/>
    <asp:UpdatePanel ID="SensorUpdatePanel" runat="server">
      <triggers>
          <asp:AsyncPostBackTrigger ControlID="SensorTimer"/>
        </triggers>
      <ContentTemplate>
        <asp:Label ID="Label1" runat="server" Text="Sensor Value: "></asp:Label>
        <asp:TextBox ID="Sensor1ValueTextBox" runat="server"></asp:TextBox>
      </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
  </html>

