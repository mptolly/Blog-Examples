using CortanaDemo.sectionFinder.Data;
using CortanaDemo.sectionFinder.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Resources.Core;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Storage;

namespace CortanaDemo {

    /// <summary>
    /// The VoiceCommandService implements the entrypoint for all headless voice commands
    /// invoked via Cortana. The individual commands supported are described in the
    /// VoiceCommands.xml VCD file in the sectionFinder project. The service
    /// entrypoint is defined in the Package Manifest (See section uap:Extension in 
    /// sectionFinder:Package.appxmanifest)
    /// </summary>
    sealed class VoiceCmdService : IBackgroundTask {

        /// <summary>
        /// the service connection is maintained for the lifetime of a cortana session, once a voice command
        /// has been triggered via Cortana.
        /// </summary>
        VoiceCommandServiceConnection voiceServiceConnection;

        /// <summary>
        /// Lifetime of the background service is controlled via the BackgroundTaskDeferral object, including
        /// registering for cancellation events, signalling end of execution, etc. Cortana may terminate the 
        /// background service task if it loses focus, or the background task takes too long to provide.
        /// 
        /// Background tasks can run for a maximum of 30 seconds.
        /// </summary>
        BackgroundTaskDeferral serviceDeferral;

        /// <summary>
        /// ResourceMap containing localized strings for display in Cortana.
        /// </summary>
        ResourceMap cortanaResourceMap;

        /// <summary>
        /// The context for localized strings.
        /// </summary>
        ResourceContext cortanaContext;

        /// <summary>
        /// Get globalization-aware date formats.
        /// </summary>
        DateTimeFormatInfo dateFormatInfo;

        /// <summary>
        /// Background task entrypoint. Voice Commands using the <VoiceCommandService Target="...">
        /// tag will invoke this when they are recognized by Cortana, passing along details of the 
        /// invocation. 
        /// 
        /// Background tasks must respond to activation by Cortana within 0.5 seconds, and must 
        /// report progress to Cortana every 5 seconds (unless Cortana is waiting for user
        /// input). There is no execution time limit on the background task managed by Cortana,
        /// but developers should use plmdebug (https://msdn.microsoft.com/en-us/library/windows/hardware/jj680085%28v=vs.85%29.aspx)
        /// on the Cortana app package in order to prevent Cortana timing out the task during
        /// debugging.
        /// 
        /// Cortana dismisses its UI if it loses focus. This will cause it to terminate the background
        /// task, even if the background task is being debugged. Use of Remote Debugging is recommended
        /// in order to debug background task behaviors. In order to debug background tasks, open the
        /// project properties for the app package (not the background task project), and enable
        /// Debug -> "Do not launch, but debug my code when it starts". Alternatively, add a long
        /// initial progress screen, and attach to the background task process while it executes.
        /// </summary>
        /// <param name="taskInstance">Connection to the hosting background service process.</param>
        public async void Run(IBackgroundTaskInstance taskInstance) {

            serviceDeferral = taskInstance.GetDeferral();

            // Register to receive an event if Cortana dismisses the background task. This will
            // occur if the task takes too long to respond, or if Cortana's UI is dismissed.
            // Any pending operations should be cancelled or waited on to clean up where possible.
            taskInstance.Canceled += OnTaskCanceled;

            var triggerDetails = taskInstance.TriggerDetails as AppServiceTriggerDetails;

            // Load localized resources for strings sent to Cortana to be displayed to the user.
            cortanaResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("Resources");

            // Select the system language, which is what Cortana should be running as.
            cortanaContext = ResourceContext.GetForViewIndependentUse();

            // Get the currently used system date format
            dateFormatInfo = CultureInfo.CurrentCulture.DateTimeFormat;

            // This should match the uap:AppService and VoiceCommandService references from the 
            // package manifest and VCD files, respectively. Make sure we've been launched by
            // a Cortana Voice Command.
            if (triggerDetails != null && triggerDetails.Name == "CortanaVoiceCommandService") {
                try {
                    voiceServiceConnection =
                        VoiceCommandServiceConnection.FromAppServiceTriggerDetails(
                            triggerDetails);

                    voiceServiceConnection.VoiceCommandCompleted += OnVoiceCommandCompleted;

                    VoiceCommand voiceCommand = await voiceServiceConnection.GetVoiceCommandAsync();

                    // Depending on the operation (defined in AdventureWorks:AdventureWorksCommands.xml)
                    // perform the appropriate command.
                    switch (voiceCommand.CommandName) {
/****************************************************************************************************************************************
 ************************************** MAKE EDITS TO INTEGRATE INTO THE COMMMANDS YOU WISH TO RUN **************************************
 ****************************************************************************************************************************************/
                        case "OpenPage":
                            string section = voiceCommand.Properties["section"][0];
                            await SendCompletionMessageForSection(section);
                            break;
/****************************************************************************************************************************************
 ****************************************************** END EDITS FOR THIS METHOD *******************************************************
 ****************************************************************************************************************************************/
                        default:
                            // As with app activation VCDs, we need to handle the possibility that
                            // an app update may remove a voice command that is still registered.
                            // This can happen if the user hasn't run an app since an update.
                            LaunchAppInForeground();
                            break;
                    }
                }
                catch (Exception ex) {
                    System.Diagnostics.Debug.WriteLine("Handling Voice Command failed " + ex.ToString());
                }
            }
        }

        /// <summary>
        /// Show a progress screen. These should be posted at least every 5 seconds for a 
        /// long-running operation, such as accessing network resources over a mobile 
        /// carrier network.
        /// </summary>
        /// <param name="message">The message to display, relating to the task being performed.</param>
        /// <returns></returns>
        private async Task ShowProgressScreen(string message) {
            var userProgressMessage = new VoiceCommandUserMessage();
            userProgressMessage.DisplayMessage = userProgressMessage.SpokenMessage = message;

            VoiceCommandResponse response = VoiceCommandResponse.CreateResponse(userProgressMessage);
            await voiceServiceConnection.ReportProgressAsync(response);
        }

        private async void LaunchAppInForeground() {
            var userMessage = new VoiceCommandUserMessage();
            userMessage.SpokenMessage = cortanaResourceMap.GetValue("LaunchingsectionFinder", cortanaContext).ValueAsString;

            var response = VoiceCommandResponse.CreateResponse(userMessage);

            response.AppLaunchArgument = "";

            await voiceServiceConnection.RequestAppLaunchAsync(response);
        }
/****************************************************************************************************************************************
 ********************************** MAKE EDITS TO DETERMINE WHAT IS SHOWN WHEN THE COMMAND IS FOUND *************************************
 ****************************************************************************************************************************************/
        /// <summary>
        /// Search for the section requested, if the section can be
        /// found. This demonstrates a simple response flow in Cortana.
        /// </summary>
        /// <param name="section">The section, expected to be in the phrase list.</param>
        /// <returns></returns>
        private async Task SendCompletionMessageForSection(string section) {
            string loadingTripToDestination = string.Format(
                       cortanaResourceMap.GetValue("FindingSection", cortanaContext).ValueAsString,
                       section);
            await ShowProgressScreen(loadingTripToDestination);

            VoiceCommandUserMessage userMessage = new VoiceCommandUserMessage();
            List<VoiceCommandContentTile> tiles = new List<VoiceCommandContentTile>();

            if (!sectionData.sections.ContainsKey(section)) {
                // In this scenario, perhaps someone has modified data on your service outside of your 
                // control. If you're accessing a remote service, having a background task that
                // periodically refreshes the phrase list so it's likely to be in sync is ideal.
                string foundNosection = string.Format(
                       cortanaResourceMap.GetValue("FoundNoSection", cortanaContext).ValueAsString,
                       section);
                userMessage.DisplayMessage = foundNosection;
                userMessage.SpokenMessage = foundNosection;
            }
            else {
                string message = string.Format("{0} {1}", cortanaResourceMap.GetValue("sectionFound", cortanaContext).ValueAsString, section);
                userMessage.DisplayMessage = message;
                userMessage.SpokenMessage = message;

                VoiceCommandContentTile tile = new VoiceCommandContentTile();
                tile.ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText;
                tile.Image = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Square44x44Logo.scale-200.png.png")); // Image icon next to result when command with phrase found

                tile.AppLaunchArgument = section;
                tile.Title = string.Format("Section: ", section);
                tiles.Add(tile);
            }

            VoiceCommandResponse response = VoiceCommandResponse.CreateResponse(userMessage, tiles);
            response.AppLaunchArgument = section;
            await voiceServiceConnection.ReportSuccessAsync(response);
        }

        /// <summary>
        /// Handle the completion of the voice command. Your app may be cancelled
        /// for a variety of reasons, such as user cancellation or not providing 
        /// progress to Cortana in a timely fashion. Clean up any pending long-running
        /// operations (eg, network requests).
        /// </summary>
        /// <param name="sender">The voice connection associated with the command.</param>
        /// <param name="args">Contains an Enumeration indicating why the command was terminated.</param>
        private void OnVoiceCommandCompleted(VoiceCommandServiceConnection sender, VoiceCommandCompletedEventArgs args) {
            if (this.serviceDeferral != null) {
                this.serviceDeferral.Complete();
            }
        }

        /// <summary>
        /// When the background task is cancelled, clean up/cancel any ongoing long-running operations.
        /// This cancellation notice may not be due to Cortana directly. The voice command connection will
        /// typically already be destroyed by this point and should not be expected to be active.
        /// </summary>
        /// <param name="sender">This background task instance</param>
        /// <param name="reason">Contains an enumeration with the reason for task cancellation</param>
        private void OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason) {
            System.Diagnostics.Debug.WriteLine("Task cancelled, clean up");
            if (this.serviceDeferral != null) {
                //Complete the service deferral
                this.serviceDeferral.Complete();
            }
        }
    }
}
