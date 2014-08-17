<%@ Page Language="C#" Inherits="MonoBrickWebServer.Default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html>
<head runat="server">
	<title>Default</title>
</head>
<body>
<form id="form1" runat="server">
		<asp:ScriptManager ID="ScriptManager" runat="server" />
        <div>
            <asp:Timer ID="UpdateTimer" OnTick="UpdateSensorTable" runat="server" Interval="200">
            </asp:Timer>
        </div>
					
<center><img src="Title.png"/></center>
<p style="border-top-width: 1px; border-top-style: solid; border-top-color: #f26100; padding-top: 10px;">
</p>

	<table width="800" style="margin: 0pt auto; clear: both;">
            <thead>
		        <tr>
		            <th width="10%" style="background-color: #f26f17;"><font color="#fff">Port</font></th>
		            <th width="30%" style="background-color: #f2690d;"><font color="#fff">Sensor Type</font></th>
		            <th width="30%" style="background-color: #f26100;">
			            <table width="100%">
					            <thead>
							        <tr>
	   						            <th>
								           <font color="#fff">Sensor Mode</font> 
							            </th>
	   						            <th width="40%" align="center">
	   						            	<font color="#fff">Select</font> 	
	   						            </th>
							        </tr>
							    </thead>
							</table>
		            		
		            </th>
		            <th width="30%" style="background-color: #f26100;"><font color="#fff">Sensor Value</font></th>
		        </tr>
		    </thead>
		    <tbody>
		        <tr>
		            <td align="center" style="background-color: #e9e9e9; text-align: middel; vertical-align: middle;">&nbsp; In 1</td>
		            <td align="center" style="background-color: #d7d7d7;">
		            	<asp:UpdatePanel ID="Sensor1TypePanel" UpdateMode="Conditional" runat="server">
				            <Triggers>
					        	<asp:AsyncPostBackTrigger ControlID="UpdateTimer" EventName="Tick" />
					        </Triggers>
		                	<ContentTemplate>
		            			<asp:Label id="Sensor1TypeText" runat="server"/>
		        			</ContentTemplate>
	 	                </asp:UpdatePanel>
		            </td>
		            <td style="background-color: #b0b0b0;">
			            <table width="100%">
				            <thead>
						        <tr>
   						            <th style="font-weight:normal">
							            <asp:UpdatePanel ID="Sensor1ModePanel" UpdateMode="Conditional" runat="server">
								            <Triggers>
									        	<asp:AsyncPostBackTrigger ControlID="UpdateTimer" EventName="Tick" />
									        </Triggers>
						                	<ContentTemplate>
						            			<asp:Label id="Sensor1ModeText" runat="server"/>
						        			</ContentTemplate>
		 	                			</asp:UpdatePanel>
						            </th>
   						            <th width="20%" align="left">
   						            	<asp:UpdatePanel ID="Sensor1PrevPanel" UpdateMode="Conditional" runat="server">
								            <ContentTemplate>
						            			<asp:Button id="sensor1Prev" runat="server" Text="Prev" CommandName="Sensor1" OnClick="PrevClicked" />
						        			</ContentTemplate>
		 	                			</asp:UpdatePanel>
   						            </th>
						            <th width="20%" align="left">
						          		<asp:UpdatePanel ID="Sensor1NextPanel" UpdateMode="Conditional" runat="server">
								            <ContentTemplate>
						            			<asp:Button id="sensor1Next" runat="server" Text="Next" CommandName="Sensor1" OnClick="NextClicked" />
						        			</ContentTemplate>
		 	                			</asp:UpdatePanel>
						          	</th>
						        </tr>
						    </thead>
						</table>
						
		            </td>
		            <td align="center" style="background-color: #8f8f8f;">
			            <asp:UpdatePanel ID="Sensor1ValuePanel" UpdateMode="Conditional" runat="server">
				            <Triggers>
					        	<asp:AsyncPostBackTrigger ControlID="UpdateTimer" EventName="Tick" />
					        </Triggers>
		                	<ContentTemplate>
		            			<asp:Label id="Sensor1ValueText" runat="server"/>
		        			</ContentTemplate>
	 	                </asp:UpdatePanel>
		            </td>
		        </tr>
		        <tr>
		            <td align="center" style="background-color: #e9e9e9; text-align: middel; vertical-align: middle;">&nbsp; In 2</td>
		            <td align="center" style="background-color: #d7d7d7;">
		            	<asp:UpdatePanel ID="Sensor2TypePanel" UpdateMode="Conditional" runat="server">
				            <Triggers>
					        	<asp:AsyncPostBackTrigger ControlID="UpdateTimer" EventName="Tick" />
					        </Triggers>
		                	<ContentTemplate>
		            			<asp:Label id="Sensor2TypeText" runat="server"/>
		        			</ContentTemplate>
	 	                </asp:UpdatePanel>
		            </td>
		            <td align="center" style="background-color: #b0b0b0;">
			            
			            <table width="100%">
				            <thead>
						        <tr>
   						            <th style="font-weight:normal">
							            <asp:UpdatePanel ID="Sensor2ModePanel" UpdateMode="Conditional" runat="server">
								            <Triggers>
									        	<asp:AsyncPostBackTrigger ControlID="UpdateTimer" EventName="Tick" />
									        </Triggers>
						                	<ContentTemplate>
						            			<asp:Label id="Sensor2ModeText" runat="server"/>
						        			</ContentTemplate>
		 	                			</asp:UpdatePanel>
						            </th>
						            <th width="20%" align="left">
   						            	<asp:UpdatePanel ID="Sensor2PrevPanel" UpdateMode="Conditional" runat="server">
								            <ContentTemplate>
						            			<asp:Button id="sensor2Prev" runat="server" Text="Prev" CommandName="Sensor2" OnClick="PrevClicked" />
						        			</ContentTemplate>
		 	                			</asp:UpdatePanel>
   						            </th>
						            <th width="20%" align="left">
						          		<asp:UpdatePanel ID="Sensor2NextPanel" UpdateMode="Conditional" runat="server">
								            <ContentTemplate>
						            			<asp:Button id="sensor2Next" runat="server" Text="Next" CommandName="Sensor2" OnClick="NextClicked" />
						        			</ContentTemplate>
		 	                			</asp:UpdatePanel>
						          		
						          	</th>
						        </tr>
						    </thead>
						</table>
		            </td>
		            <td align="center" style="background-color: #8f8f8f;">
			            <asp:UpdatePanel ID="Sensor2ValuePanel" UpdateMode="Conditional" runat="server">
				            <Triggers>
					        	<asp:AsyncPostBackTrigger ControlID="UpdateTimer" EventName="Tick" />
					        </Triggers>
		                	<ContentTemplate>
		            			<asp:Label id="Sensor2ValueText" runat="server"/>
		        			</ContentTemplate>
	 	                </asp:UpdatePanel>
		            </td>
		        </tr>
		        <tr>
		            <td align="center" style="background-color: #e9e9e9; text-align: middel; vertical-align: middle;">&nbsp; In 3</td>
		           	<td align="center" style="background-color: #d7d7d7;">
		            	<asp:UpdatePanel ID="Sensor3TypePannel" UpdateMode="Conditional" runat="server">
				            <Triggers>
					        	<asp:AsyncPostBackTrigger ControlID="UpdateTimer" EventName="Tick" />
					        </Triggers>
		                	<ContentTemplate>
		            			<asp:Label id="Sensor3TypeText" runat="server"/>
		        			</ContentTemplate>
	 	                </asp:UpdatePanel>
		            </td>
		            <td align="center" style="background-color: #b0b0b0;">
			            <table width="100%">
				            <thead>
						        <tr>
   						            <th style="font-weight:normal">
							            <asp:UpdatePanel ID="Sensor3ModePanel" UpdateMode="Conditional" runat="server">
								            <Triggers>
									        	<asp:AsyncPostBackTrigger ControlID="UpdateTimer" EventName="Tick" />
									        </Triggers>
						                	<ContentTemplate>
						            			<asp:Label id="Sensor3ModeText" runat="server"/>
						        			</ContentTemplate>
		 	                			</asp:UpdatePanel>
						            </th>
						            <th width="20%" align="left">
   						            	<asp:UpdatePanel ID="Sensor3PrevPanel" UpdateMode="Conditional" runat="server">
								            <ContentTemplate>
						            			<asp:Button id="sensor3Prev" runat="server" Text="Prev" CommandName="Sensor3" OnClick="PrevClicked" />
						        			</ContentTemplate>
		 	                			</asp:UpdatePanel>
   						            </th>
						            <th width="20%" align="left">
						          		<asp:UpdatePanel ID="Sensor3NextPanel" UpdateMode="Conditional" runat="server">
								            <ContentTemplate>
						            			<asp:Button id="sensor3Next" runat="server" Text="Next" CommandName="Sensor3" OnClick="NextClicked" />
						        			</ContentTemplate>
		 	                			</asp:UpdatePanel>
						          		
						          	</th>
						        </tr>
						    </thead>
						</table>
		            </td>
		            <td align="center" style="background-color: #8f8f8f;">
			            <asp:UpdatePanel ID="Sensor3ValuePannel" UpdateMode="Conditional" runat="server">
				            <Triggers>
					        	<asp:AsyncPostBackTrigger ControlID="UpdateTimer" EventName="Tick" />
					        </Triggers>
		                	<ContentTemplate>
		            			<asp:Label id="Sensor3ValueText" runat="server"/>
		        			</ContentTemplate>
	 	                </asp:UpdatePanel>
		            </td>
		        </tr>
		        <tr>
		            <td align="center" style="background-color: #e9e9e9; text-align: middel; vertical-align: middle;">&nbsp; In 4</td>
		           	<td align="center" style="background-color: #d7d7d7;">
		            	<asp:UpdatePanel ID="Sensor4TypePanel" UpdateMode="Conditional" runat="server">
				            <Triggers>
					        	<asp:AsyncPostBackTrigger ControlID="UpdateTimer" EventName="Tick" />
					        </Triggers>
		                	<ContentTemplate>
		            			<asp:Label id="Sensor4TypeText" runat="server"/>
		        			</ContentTemplate>
	 	                </asp:UpdatePanel>
		            </td>
		            <td align="center" style="background-color: #b0b0b0;">
			            <table width="100%">
				            <thead>
						        <tr>
   						            <th style="font-weight:normal">
							            <asp:UpdatePanel ID="Sensor4ModePanel" UpdateMode="Conditional" runat="server">
								            <Triggers>
									        	<asp:AsyncPostBackTrigger ControlID="UpdateTimer" EventName="Tick" />
									        </Triggers>
						                	<ContentTemplate>
						            			<asp:Label id="Sensor4ModeText" runat="server"/>
						        			</ContentTemplate>
		 	                			</asp:UpdatePanel>
						            </th>
						            <th width="20%" align="left">
   						            	<asp:UpdatePanel ID="Sensor4PrevPanel" UpdateMode="Conditional" runat="server">
								            <ContentTemplate>
						            			<asp:Button id="sensor4Prev" runat="server" Text="Prev" CommandName="Sensor4" OnClick="PrevClicked" />
						        			</ContentTemplate>
		 	                			</asp:UpdatePanel>
   						            </th>
						            <th width="20%" align="left">
						          		<asp:UpdatePanel ID="Sensor4NextPanel" UpdateMode="Conditional" runat="server">
								            <ContentTemplate>
						            			<asp:Button id="sensor4Next" runat="server" Text="Next" CommandName="Sensor4" OnClick="NextClicked" />
						        			</ContentTemplate>
		 	                			</asp:UpdatePanel>
						          		
						          	</th>
						        </tr>
						    </thead>
						</table>
		            </td>
		            <td align="center" style="background-color: #8f8f8f;">
			            <asp:UpdatePanel ID="Sensor4VAluePanel" UpdateMode="Conditional" runat="server">
				            <Triggers>
					        	<asp:AsyncPostBackTrigger ControlID="UpdateTimer" EventName="Tick" />
					        </Triggers>
		                	<ContentTemplate>
		            			<asp:Label id="Sensor4ValueText" runat="server"/>
		        			</ContentTemplate>
	 	                </asp:UpdatePanel>
		            </td>
		        </tr>
		    </tbody>
  
	</table>
<br />
<table width="800" style="margin: 0pt auto; clear: both;">
            <thead>
		        <tr>
		            <th width="10%" style="background-color: #f26f17;"><font color="#fff">Port</font></th>
		            <th width="30%" style="background-color: #f2690d;"><font color="#fff">Motor Control</font></th>
		            <th width="30%" style="background-color: #f26100;"><font color="#fff">Position control</font></th>
		            <th width="30%" style="background-color: #f26100;"><font color="#fff">Tacho Value</font></th>
		        </tr>
		    </thead>
		    <tbody>
		        <tr>
		            <td align="center" style="background-color: #e9e9e9; text-align: middel; vertical-align: middle;">&nbsp; Out 1</td>
		            <td style="background-color: #d7d7d7;">
			            <table width="100%">
				            <thead>
						        <tr>
   						            <th width="25%" align="left">
   						            	<asp:UpdatePanel ID="Motor1RevPanel" UpdateMode="Conditional" runat="server">
								            <ContentTemplate>
						            			<asp:Button id="Motor1Rev" runat="server" Text=" Rev " CommandName="Motor1" OnClick="RevClicked" />
						        			</ContentTemplate>
		 	                			</asp:UpdatePanel>
   						            </th>
   						            <th width="25%" align="left">
						          		<asp:UpdatePanel ID="Motor1FwdPanel" UpdateMode="Conditional" runat="server">
								            <ContentTemplate>
						            			<asp:Button id="Motor1Fwd" runat="server" Text=" Fwd " CommandName="Motor1" OnClick="FwdClicked" />
						        			</ContentTemplate>
		 	                			</asp:UpdatePanel>
						          	</th>
						            <th width="25%" align="left">
   						            	<asp:UpdatePanel ID="Motor1OffPanel" UpdateMode="Conditional" runat="server">
								            <ContentTemplate>
						            			<asp:Button id="Motor1Off" runat="server" Text=" Off " CommandName="Motor1" OnClick="OffClicked" />
						        			</ContentTemplate>
		 	                			</asp:UpdatePanel>
   						            </th>
						            <th width="25%" align="left">
   						            	<asp:UpdatePanel ID="Motor1BreakPanel" UpdateMode="Conditional" runat="server">
								            <ContentTemplate>
						            			<asp:Button id="Motor1BreakPrev" runat="server" Text="Brake" CommandName="Motor1" OnClick="BrakeClicked" />
						        			</ContentTemplate>
		 	                			</asp:UpdatePanel>
   						            </th>
						        </tr>
						    </thead>
						</table>
						
		            </td>
		            <td align="center" style="background-color: #b0b0b0;">
		            	<table width="100%">
				            <thead>
						        <tr>
   						            <th width="50%" align="center">
   						            	<asp:UpdatePanel ID="Motor1MovePanel" UpdateMode="Conditional" runat="server">
								            <ContentTemplate>
						            			<asp:Button id="Motor1MoveTo" runat="server" Text="Move To" CommandName="Motor1" OnClick="MoveToClicked" />
						        			</ContentTemplate>
		 	                			</asp:UpdatePanel>
   						            </th>
   						            <th width="50%" align="left">
						          		<asp:UpdatePanel ID="Motor1PositionPanel" UpdateMode="Conditional" runat="server">
								            <ContentTemplate>
						            			<asp:TextBox id="Motor1PositionText" runat="server"/>
						        			</ContentTemplate>
		 	                			</asp:UpdatePanel>
						          	</th>
						        </tr>
						    </thead>
						</table>
		            </td>
		            <td align="center" style="background-color: #8f8f8f;">
			            <table width="100%">
				            <thead>
						        <tr>
   						            <th width="50%" align="center">
   						            	<asp:UpdatePanel ID="Motor1TachoPanel" UpdateMode="Conditional" runat="server">
								            <Triggers>
									        	<asp:AsyncPostBackTrigger ControlID="UpdateTimer" EventName="Tick" />
									        </Triggers>
						                	<ContentTemplate>
						            			<asp:Label id="Motor1TachoText" runat="server"/>
						        			</ContentTemplate>
				 	                	</asp:UpdatePanel>
   						            </th>
   						            <th width="50%" align="center">
						          		<asp:UpdatePanel ID="Motor1ResetPanel" UpdateMode="Conditional" runat="server">
								            <ContentTemplate>
						            			<asp:Button id="Motor1Reset" runat="server" Text="Reset" CommandName="Motor1" OnClick="ResetTachoClicked" />
						        			</ContentTemplate>
		 	                			</asp:UpdatePanel>
						          	</th>
						        </tr>
						    </thead>
						</table>
		            </td>
		        </tr>
		        <tr>
		            <td align="center" style="background-color: #e9e9e9; text-align: middel; vertical-align: middle;">&nbsp; Out 2</td>
		            <td style="background-color: #d7d7d7;">
			            <table width="100%">
				            <thead>
						        <tr>
   						            <th width="25%" align="left">
   						            	<asp:UpdatePanel ID="Motor2RevPanel" UpdateMode="Conditional" runat="server">
								            <ContentTemplate>
						            			<asp:Button id="Motor2Rev" runat="server" Text=" Rev " CommandName="Motor2" OnClick="RevClicked" />
						        			</ContentTemplate>
		 	                			</asp:UpdatePanel>
   						            </th>
   						            <th width="25%" align="left">
						          		<asp:UpdatePanel ID="Motor2FwdPanel" UpdateMode="Conditional" runat="server">
								            <ContentTemplate>
						            			<asp:Button id="Motor2Fwd" runat="server" Text=" Fwd " CommandName="Motor2" OnClick="FwdClicked" />
						        			</ContentTemplate>
		 	                			</asp:UpdatePanel>
						          	</th>
						            <th width="25%" align="left">
   						            	<asp:UpdatePanel ID="Motor2OffPanel" UpdateMode="Conditional" runat="server">
								            <ContentTemplate>
						            			<asp:Button id="Motor2Off" runat="server" Text=" Off " CommandName="Motor2" OnClick="OffClicked" />
						        			</ContentTemplate>
		 	                			</asp:UpdatePanel>
   						            </th>
						            <th width="25%" align="left">
   						            	<asp:UpdatePanel ID="Motor2BreakPanel" UpdateMode="Conditional" runat="server">
								            <ContentTemplate>
						            			<asp:Button id="Motor2BreakPrev" runat="server" Text="Brake" CommandName="Motor2" OnClick="BrakeClicked" />
						        			</ContentTemplate>
		 	                			</asp:UpdatePanel>
   						            </th>
						        </tr>
						    </thead>
						</table>
						
		            </td>
		            <td align="center" style="background-color: #b0b0b0;">
		            	<table width="100%">
				            <thead>
						        <tr>
   						            <th width="50%" align="center">
   						            	<asp:UpdatePanel ID="Motor2MovePanel" UpdateMode="Conditional" runat="server">
								            <ContentTemplate>
						            			<asp:Button id="Motor2MoveTo" runat="server" Text="Move To" CommandName="Motor2" OnClick="MoveToClicked" />
						        			</ContentTemplate>
		 	                			</asp:UpdatePanel>
   						            </th>
   						            <th width="50%" align="left">
						          		<asp:UpdatePanel ID="Motor2PositionPanel" UpdateMode="Conditional" runat="server">
								            <ContentTemplate>
						            			<asp:TextBox id="Motor2PositionText" runat="server"/>
						        			</ContentTemplate>
		 	                			</asp:UpdatePanel>
						          	</th>
						        </tr>
						    </thead>
						</table>
		            </td>
		            <td align="center" style="background-color: #8f8f8f;">
			            <table width="100%">
				            <thead>
						        <tr>
   						            <th width="50%" align="center">
   						            	<asp:UpdatePanel ID="Motor2TachoPanel" UpdateMode="Conditional" runat="server">
								            <Triggers>
									        	<asp:AsyncPostBackTrigger ControlID="UpdateTimer" EventName="Tick" />
									        </Triggers>
						                	<ContentTemplate>
						            			<asp:Label id="Motor2TachoText" runat="server"/>
						        			</ContentTemplate>
				 	                	</asp:UpdatePanel>
   						            </th>
   						            <th width="50%" align="center">
						          		<asp:UpdatePanel ID="Motor2ResetPanel" UpdateMode="Conditional" runat="server">
								            <ContentTemplate>
						            			<asp:Button id="Motor2Reset" runat="server" Text="Reset" CommandName="Motor2" OnClick="ResetTachoClicked" />
						        			</ContentTemplate>
		 	                			</asp:UpdatePanel>
						          	</th>
						        </tr>
						    </thead>
						</table>
		            </td>
		        </tr>
		        <tr>
		            <td align="center" style="background-color: #e9e9e9; text-align: middel; vertical-align: middle;">&nbsp; Out 3</td>
		            <td style="background-color: #d7d7d7;">
			            <table width="100%">
				            <thead>
						        <tr>
   						            <th width="25%" align="left">
   						            	<asp:UpdatePanel ID="Motor3RevPanel" UpdateMode="Conditional" runat="server">
								            <ContentTemplate>
						            			<asp:Button id="Motor3Rev" runat="server" Text=" Rev " CommandName="Motor3" OnClick="RevClicked" />
						        			</ContentTemplate>
		 	                			</asp:UpdatePanel>
   						            </th>
   						            <th width="25%" align="left">
						          		<asp:UpdatePanel ID="Motor3FwdPanel" UpdateMode="Conditional" runat="server">
								            <ContentTemplate>
						            			<asp:Button id="Motor3Fwd" runat="server" Text=" Fwd " CommandName="Motor3" OnClick="FwdClicked" />
						        			</ContentTemplate>
		 	                			</asp:UpdatePanel>
						          	</th>
						            <th width="25%" align="left">
   						            	<asp:UpdatePanel ID="Motor3OffPanel" UpdateMode="Conditional" runat="server">
								            <ContentTemplate>
						            			<asp:Button id="Motor3Off" runat="server" Text=" Off " CommandName="Motor3" OnClick="OffClicked" />
						        			</ContentTemplate>
		 	                			</asp:UpdatePanel>
   						            </th>
						            <th width="25%" align="left">
   						            	<asp:UpdatePanel ID="Motor3BreakPanel" UpdateMode="Conditional" runat="server">
								            <ContentTemplate>
						            			<asp:Button id="Motor3BreakPrev" runat="server" Text="Brake" CommandName="Motor3" OnClick="BrakeClicked" />
						        			</ContentTemplate>
		 	                			</asp:UpdatePanel>
   						            </th>
						        </tr>
						    </thead>
						</table>
						
		            </td>
		            <td align="center" style="background-color: #b0b0b0;">
		            	<table width="100%">
				            <thead>
						        <tr>
   						            <th width="50%" align="center">
   						            	<asp:UpdatePanel ID="Motor3MovePanel" UpdateMode="Conditional" runat="server">
								            <ContentTemplate>
						            			<asp:Button id="Motor3MoveTo" runat="server" Text="Move To" CommandName="Motor3" OnClick="MoveToClicked" />
						        			</ContentTemplate>
		 	                			</asp:UpdatePanel>
   						            </th>
   						            <th width="50%" align="left">
						          		<asp:UpdatePanel ID="Motor3PositionPanel" UpdateMode="Conditional" runat="server">
								            <ContentTemplate>
						            			<asp:TextBox id="Motor3PositionText" runat="server"/>
						        			</ContentTemplate>
		 	                			</asp:UpdatePanel>
						          	</th>
						        </tr>
						    </thead>
						</table>
		            </td>
		            <td align="center" style="background-color: #8f8f8f;">
			            <table width="100%">
				            <thead>
						        <tr>
   						            <th width="50%" align="center">
   						            	<asp:UpdatePanel ID="Motor3TachoPanel" UpdateMode="Conditional" runat="server">
								            <Triggers>
									        	<asp:AsyncPostBackTrigger ControlID="UpdateTimer" EventName="Tick" />
									        </Triggers>
						                	<ContentTemplate>
						            			<asp:Label id="Motor3TachoText" runat="server"/>
						        			</ContentTemplate>
				 	                	</asp:UpdatePanel>
   						            </th>
   						            <th width="50%" align="center">
						          		<asp:UpdatePanel ID="Motor3ResetPanel" UpdateMode="Conditional" runat="server">
								            <ContentTemplate>
						            			<asp:Button id="Motor3Reset" runat="server" Text="Reset" CommandName="Motor3" OnClick="ResetTachoClicked" />
						        			</ContentTemplate>
		 	                			</asp:UpdatePanel>
						          	</th>
						        </tr>
						    </thead>
						</table>
		            </td>
		        </tr>
		        <tr>
		            <td align="center" style="background-color: #e9e9e9; text-align: middel; vertical-align: middle;">&nbsp; Out 4</td>
		            <td style="background-color: #d7d7d7;">
			            <table width="100%">
				            <thead>
						        <tr>
   						            <th width="25%" align="left">
   						            	<asp:UpdatePanel ID="Motor4RevPanel" UpdateMode="Conditional" runat="server">
								            <ContentTemplate>
						            			<asp:Button id="Motor4Rev" runat="server" Text=" Rev " CommandName="Motor4" OnClick="RevClicked" />
						        			</ContentTemplate>
		 	                			</asp:UpdatePanel>
   						            </th>
   						            <th width="25%" align="left">
						          		<asp:UpdatePanel ID="Motor4FwdPanel" UpdateMode="Conditional" runat="server">
								            <ContentTemplate>
						            			<asp:Button id="Motor4Fwd" runat="server" Text=" Fwd " CommandName="Motor4" OnClick="FwdClicked" />
						        			</ContentTemplate>
		 	                			</asp:UpdatePanel>
						          	</th>
						            <th width="25%" align="left">
   						            	<asp:UpdatePanel ID="Motor4OffPanel" UpdateMode="Conditional" runat="server">
								            <ContentTemplate>
						            			<asp:Button id="Motor4Off" runat="server" Text=" Off " CommandName="Motor4" OnClick="OffClicked" />
						        			</ContentTemplate>
		 	                			</asp:UpdatePanel>
   						            </th>
						            <th width="25%" align="left">
   						            	<asp:UpdatePanel ID="Motor4BreakPanel" UpdateMode="Conditional" runat="server">
								            <ContentTemplate>
						            			<asp:Button id="Motor4BreakPrev" runat="server" Text="Brake" CommandName="Motor4" OnClick="BrakeClicked" />
						        			</ContentTemplate>
		 	                			</asp:UpdatePanel>
   						            </th>
						        </tr>
						    </thead>
						</table>
						
		            </td>
		            <td align="center" style="background-color: #b0b0b0;">
		            	<table width="100%">
				            <thead>
						        <tr>
   						            <th width="50%" align="center">
   						            	<asp:UpdatePanel ID="Motor4MovePanel" UpdateMode="Conditional" runat="server">
								            <ContentTemplate>
						            			<asp:Button id="Motor4MoveTo" runat="server" Text="Move To" CommandName="Motor4" OnClick="MoveToClicked" />
						        			</ContentTemplate>
		 	                			</asp:UpdatePanel>
   						            </th>
   						            <th width="50%" align="left">
						          		<asp:UpdatePanel ID="Motor4PositionPanel" UpdateMode="Conditional" runat="server">
								            <ContentTemplate>
						            			<asp:TextBox id="Motor4PositionText" runat="server"/>
						        			</ContentTemplate>
		 	                			</asp:UpdatePanel>
						          	</th>
						        </tr>
						    </thead>
						</table>
		            </td>
		            <td align="center" style="background-color: #8f8f8f;">
			            <table width="100%">
				            <thead>
						        <tr>
   						            <th width="50%" align="center">
   						            	<asp:UpdatePanel ID="Motor4TachoPanel" UpdateMode="Conditional" runat="server">
								            <Triggers>
									        	<asp:AsyncPostBackTrigger ControlID="UpdateTimer" EventName="Tick" />
									        </Triggers>
						                	<ContentTemplate>
						            			<asp:Label id="Motor4TachoText" runat="server"/>
						        			</ContentTemplate>
				 	                	</asp:UpdatePanel>
   						            </th>
   						            <th width="50%" align="center">
						          		<asp:UpdatePanel ID="Motor4ResetPanel" UpdateMode="Conditional" runat="server">
								            <ContentTemplate>
						            			<asp:Button id="Motor4Reset" runat="server" Text="Reset" CommandName="Moto4" OnClick="ResetTachoClicked" />
						        			</ContentTemplate>
		 	                			</asp:UpdatePanel>
						          	</th>
						        </tr>
						    </thead>
						</table>
		            </td>
		        </tr>
		    </tbody>
	</table>
<p align="right">
<sup><font color="#999999">Powered by Mono and <a href="http://en.wikipedia.org/wiki/XSP_(software)" target="_blank">XSP</a></font></sup>
</p>
</form>
</body>
</html>
