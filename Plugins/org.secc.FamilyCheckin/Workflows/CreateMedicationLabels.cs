// <copyright>
// Copyright Southeast Christian Church
//
// Licensed under the  Southeast Christian Church License (the "License");
// you may not use this file except in compliance with the License.
// A copy of the License shoud be included with this file.
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using Rock;
using Rock.Attribute;
using Rock.CheckIn;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Workflow;
using Rock.Workflow.Action.CheckIn;

namespace org.secc.FamilyCheckin
{
    /// <summary>
    /// Creates Check-in Labels
    /// </summary>
    [ActionCategory( "SECC > Check-In" )]
    [Description( "Creates Labels for a student's medication needs." )]
    [Export( typeof( ActionComponent ) )]
    [ExportMetadata( "ComponentName", "Create Medication Labels" )]
    [TextField( "Group Attribute Key", "Attribute key for the check-in group which refers to the registration group." )]
    [TextField( "Matrix Attribute Key", "Attribute key for the medication matrix." )]
    [BinaryFileField( Rock.SystemGuid.BinaryFiletype.CHECKIN_LABEL, "Medication Label", "Label to print", false )]
    [TextField( "Medication Text", "Merge fields for medications", false, "Medication 1,Medication 2,Medication 3" )]
    [TextField( "Instructions Text", "Merge fields for instructions", false, "Instructions 1,Instructions 2,Instructions 3" )]
    [TextField( "Matrix Attribute Medication Key" )]
    [TextField( "Matrix Attribute Instructions Key" )]
    [TextField( "Matrix Attribute Schedule Key" )]

    public class CreateMedicationLabels : CheckInActionComponent
    {

        /// <summary>
        /// Executes the specified workflow.
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="action">The workflow action.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="errorMessages">The error messages.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override bool Execute( RockContext rockContext, WorkflowAction action, Object entity, out List<string> errorMessages )
        {
            var checkInState = GetCheckInState( entity, out errorMessages );
            var matrixAttributeMedicationKey = GetAttributeValue( action, "MatrixAttributeMedicationKey" );
            var matrixAttributeInstructionsKey = GetAttributeValue( action, "MatrixAttributeInstructionsKey" );
            var matrixAttributeScheduleKey = GetAttributeValue( action, "MatrixAttributeScheduleKey" );

            AttributeMatrixService attributeMatrixService = new AttributeMatrixService( rockContext );
            GroupMemberService groupMemberService = new GroupMemberService( rockContext );

            if ( checkInState != null )
            {
                var family = checkInState.CheckIn.CurrentFamily;
                if ( family != null )
                {
                    var commonMergeFields = Rock.Lava.LavaHelper.GetCommonMergeFields( null );

                    var people = family.GetPeople( true );
                    foreach ( var person in people.Where( p => p.Selected ) )
                    {
                        foreach ( var groupType in person.GroupTypes )
                        {
                            groupType.Labels = new List<CheckInLabel>();


                            foreach ( var group in groupType.Groups )
                            {
                                List<string> medicationText = ( ( string ) GetAttributeValue( action, "MedicationText" ) ).Split( ',' ).ToList();
                                List<string> instructionsText = ( ( string ) GetAttributeValue( action, "InstructionsText" ) ).Split( ',' ).ToList();

                                List<MedInfo> medInfos = new List<MedInfo>();
                                if ( medicationText.Count == instructionsText.Count )
                                {
                                    for ( int i = 0; i < medicationText.Count; i++ )
                                    {
                                        medInfos.Add( new MedInfo { Medication = medicationText[i], Instructions = instructionsText[i] } );
                                    }
                                }

                                group.Group.LoadAttributes();
                                var groupGuid = group.Group.GetAttributeValue( GetAttributeValue( action, "GroupAttributeKey" ) );
                                var registrationGroup = new GroupService( rockContext ).Get( groupGuid.AsGuid() );
                                if ( registrationGroup == null )
                                {
                                    continue;
                                }

                                var groupMember = groupMemberService.GetByGroupIdAndPersonId( registrationGroup.Id, person.Person.Id ).FirstOrDefault();

                                var medicationKey = GetAttributeValue( action, "MatrixAttributeKey" );
                                var medicationMatrix = person.Person.GetAttributeValue( medicationKey );


                                var attributeMatrix = attributeMatrixService.Get( medicationMatrix.AsGuid() );

                                var labelCache = KioskLabel.Get( new Guid( GetAttributeValue( action, "MedicationLabel" ) ) );

                                //Set up merge fields so we can use the lava from the merge fields
                                var mergeObjects = new Dictionary<string, object>();
                                foreach ( var keyValue in commonMergeFields )
                                {
                                    mergeObjects.Add( keyValue.Key, keyValue.Value );
                                }

                                mergeObjects.Add( "RegistrationGroup", registrationGroup );
                                mergeObjects.Add( "RegistrationGroupMember", groupMember );
                                mergeObjects.Add( "Group", group );
                                mergeObjects.Add( "Person", person );
                                mergeObjects.Add( "People", people );
                                mergeObjects.Add( "GroupType", groupType );

                                if ( attributeMatrix == null || attributeMatrix.AttributeMatrixItems.Count == 0 )
                                {
                                    // Add a No Medication Information label for anyone without data
                                    var checkInLabel = new CheckInLabel( labelCache, mergeObjects );

                                    var index = 0;
                                    foreach ( string mergeFieldText in medicationText )
                                    {
                                        checkInLabel.MergeFields.Add( mergeFieldText, index == 0 ? "No Medication Information Found" : "" );
                                        checkInLabel.MergeFields.Add( instructionsText[index], "" );
                                        index++;
                                    }
                                    addLabel( checkInLabel, checkInState, groupType, group, rockContext );
                                }
                                else
                                {
                                    var items = attributeMatrix.AttributeMatrixItems.ToList();
                                    var index = 0;

                                    while ( index < items.Count )
                                    {
                                        var checkInLabel = new CheckInLabel( labelCache, mergeObjects );

                                        foreach ( var med in medInfos )
                                        {

                                            if ( items.Count > index )
                                            {
                                                items[index].LoadAttributes();

                                                string scheduleText = "";
                                                string separator = "";
                                                var schedule = items[index].GetAttributeValue( matrixAttributeScheduleKey ).SplitDelimitedValues();
                                                foreach ( var scheduleGuid in schedule )
                                                {
                                                    scheduleText += separator + DefinedValueCache.Get( scheduleGuid );
                                                    separator = ", ";
                                                }

                                                checkInLabel.MergeFields.Add( med.Medication,
                                                    items[index].GetAttributeValue( matrixAttributeMedicationKey )
                                                    + " - "
                                                    + scheduleText
                                                );

                                                checkInLabel.MergeFields.Add( med.Instructions, items[index].GetAttributeValue( matrixAttributeInstructionsKey ) );
                                            }
                                            else
                                            {
                                                checkInLabel.MergeFields.Add( med.Medication, "" );
                                                checkInLabel.MergeFields.Add( med.Instructions, "" );
                                            }

                                            index++;
                                        }

                                        addLabel( checkInLabel, checkInState, groupType, group, rockContext );

                                        //Save that we just checked in the student's medications
                                        person.Person.SetAttributeValue( Utilities.Constants.PERSON_ATTRIBUTE_KEY_LASTMEDICATIONCHECKIN, Rock.RockDateTime.Today );
                                        person.Person.SaveAttributeValue( Utilities.Constants.PERSON_ATTRIBUTE_KEY_LASTMEDICATIONCHECKIN );
                                    }
                                }
                            }
                        }
                    }
                }
                return true;
            }

            errorMessages.Add( $"Attempted to run {this.GetType().GetFriendlyTypeName()} in check-in, but the check-in state was null." );
            return false;
        }

        private void addLabel( CheckInLabel checkInLabel, CheckInState checkInState, CheckInGroupType groupType, CheckInGroup group, RockContext rockContext )
        {

            var PrinterIPs = new Dictionary<int, string>();

            if ( checkInLabel.PrintTo == PrintTo.Default )
            {
                checkInLabel.PrintTo = groupType.GroupType.AttendancePrintTo;
            }
            else if ( checkInLabel.PrintTo == PrintTo.Location && group.Locations.Any() )
            {
                var deviceId = group.Locations.FirstOrDefault().Location.PrinterDeviceId;
                if ( deviceId != null )
                {
                    checkInLabel.PrinterDeviceId = deviceId;
                }
            }
            else
            {
                var device = checkInState.Kiosk.Device;
                if ( device != null )
                {
                    checkInLabel.PrinterDeviceId = device.PrinterDeviceId;
                }
            }


            if ( checkInLabel.PrinterDeviceId.HasValue )
            {
                if ( PrinterIPs.ContainsKey( checkInLabel.PrinterDeviceId.Value ) )
                {
                    checkInLabel.PrinterAddress = PrinterIPs[checkInLabel.PrinterDeviceId.Value];
                }
                else
                {
                    var printerDevice = new DeviceService( rockContext ).Get( checkInLabel.PrinterDeviceId.Value );
                    if ( printerDevice != null )
                    {
                        PrinterIPs.Add( printerDevice.Id, printerDevice.IPAddress );
                        checkInLabel.PrinterAddress = printerDevice.IPAddress;
                    }
                }
            }

            groupType.Labels.Insert( 0, checkInLabel );
        }

        private class MedInfo
        {
            public string Medication { get; set; }
            public string Instructions { get; set; }
        }
    }
}