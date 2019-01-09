var launchScriptDashboardCommand = null;

var customButtonGroup = null;
var initialized = false;

// Handle the startup event for the ShellUI environment.
function OnNewShellUI(shellUI) {
	return {
		OnNewShellFrame: (shellFrame) => {
			return {
				OnStarted: function () {
					if (shellFrame.TaskPane.Available) {
						customButtonGroup = shellFrame.TaskPane.CreateGroup("Custom", 0);

						launchScriptDashboardCommand = shellFrame.Commands.CreateCustomCommand("Script");
						shellFrame.TaskPane.AddCustomCommandToGroup(launchScriptDashboardCommand, customButtonGroup, -1);
						//shellFrame.Commands.SetCommandState(launchScriptDashboardCommand, CommandLocation_All, CommandState_Hidden);

						initialized = true;
					}
				},

				// Called when commands are initialized.
				OnNewCommands: function (commands) {
					return {
						OnCustomCommand: function (commandId) {
							if (commandId === launchScriptDashboardCommand) {
								try {
									//shellUI.Window.clipboardData.setData("Text", selected.ObjVer.ID);
									//shellFrame.Commands.ExecuteCommand(BuiltinCommand_Copy, `ID: ${selected.ObjVer.ID}`);
									shellUI.ShowPopupDashboard("ScriptDashboard", false, { /*custom data*/shellUI: shellUI });
								} catch (e) {
									shellFrame.ShowMessage(e);
								}
							}
						} // end of OnCustomCommand function
					}; // end of handler for commands
				}
			}
		}
	}
}
