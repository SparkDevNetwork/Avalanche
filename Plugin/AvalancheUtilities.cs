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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;

namespace Avalanche
{
    public static class AvalancheUtilities
    {
        //Constansts
        public const string MobileListViewComponent = "657FDF2F-FB7B-44C4-BAB0-A370893FDFB8";
        public const string AppMediumValue = "52A640E9-CB8B-4EA3-B3AA-12E1A31C6E42";
        public const string LayoutsDefinedType = "9E4EF7E8-AE7A-4920-BA59-14C24D6F24D9";

        //Methods
        public static string GetShortAssembly( Type type )
        {
            return string.Format(
                "{0}, {1}",
                type.FullName,
                type.Assembly.GetName().Name
                );
        }

        public static void SetActionItems( string ActionItemValue, Dictionary<string, string> CustomAttributes, Person CurrentPerson, Dictionary<string, object> MergeObjects = null, string EnabledLavaCommands = "", string parameter = "" )
        {
            char[] splitchar = { };
            if ( ActionItemValue.Contains( "^" ) )
            {
                splitchar = new char[] { '^' };
            }
            else
            {
                splitchar = new char[] { '|' };
            }
            var actionItems = ( ActionItemValue ?? "" ).Split( splitchar );

            if ( actionItems.Length > 0 && !string.IsNullOrWhiteSpace( actionItems[0] ) )
            {
                var actionType = actionItems[0];
                CustomAttributes.Add( "ActionType", actionType );
                if ( actionType != "0" && actionType != "3" ) //Not Do Nothing or Pop Current Page
                {
                    if ( actionItems.Length > 1 )
                    {
                        if ( MergeObjects != null )
                        {
                            CustomAttributes.Add( "Resource", ProcessLava( actionItems[1], CurrentPerson, parameter, EnabledLavaCommands, MergeObjects ) );
                        }
                        else
                        {
                            CustomAttributes.Add( "Resource", ProcessLava( actionItems[1], CurrentPerson, parameter, EnabledLavaCommands ) );
                        }
                    }
                    if ( actionItems.Length > 2 )
                    {
                        if ( MergeObjects != null )
                        {
                            CustomAttributes.Add( "Parameter", ProcessLava( actionItems[2], CurrentPerson, parameter, EnabledLavaCommands, MergeObjects ) );
                        }
                        else
                        {
                            CustomAttributes.Add( "Parameter", ProcessLava( actionItems[2], CurrentPerson, parameter, EnabledLavaCommands ) );
                        }
                    }
                }
            }
            else
            {
                CustomAttributes.Add( "ActionType", "0" );
            }
        }

        public static string ProcessLava( string lava, Person currentPerson, string parameter = "", string enabledLavaCommands = "", Dictionary<string, object> mergeObjects = null )
        {
            if ( mergeObjects == null )
            {
                mergeObjects = GetMergeFields( currentPerson );
            }

            mergeObjects["parameter"] = parameter;

            var limit = 10;
            while ( ( lava.HasLavaCommandFields() || lava.HasMergeFields() ) && limit > 0 )
            {
                lava = lava.ResolveMergeFields( mergeObjects, null, enabledLavaCommands );
                limit--;
            }
            return lava;
        }

        public static Dictionary<string, Object> GetMergeFields( Person currentPerson )
        {
            var mergeObjects = Rock.Lava.LavaHelper.GetCommonMergeFields( null, currentPerson );
            mergeObjects.Add( "IsMobileApp", true );
            return mergeObjects;
        }

        public static string GetLayout( string layoutName )
        {
            var definedType = DefinedTypeCache.Get( LayoutsDefinedType.AsGuid() );
            var value = definedType.DefinedValues.Where( d => d.Value.Replace( " ", "" ).ToLower() == layoutName.Replace( " ", "" ).ToLower() ).FirstOrDefault();
            var content = value.GetAttributeValue( "Content" );
            return content;
        }


        public static PersonalDevice GetPersonalDevice( string deviceId, int? PersonAliasId, RockContext rockContext )
        {
            if ( string.IsNullOrWhiteSpace( deviceId ) )
            {
                return null;
            }

            PersonalDeviceService personalDeviceService = new PersonalDeviceService( rockContext );
            var device = personalDeviceService.Queryable().Where( d => d.DeviceUniqueIdentifier == deviceId ).FirstOrDefault();
            if ( device != null )
            {
                if ( device.PersonAliasId == PersonAliasId )
                {
                    return device;
                }
                device.PersonAliasId = PersonAliasId;
                rockContext.SaveChanges();
                return device;
            }

            device = new PersonalDevice()
            {
                PersonAliasId = PersonAliasId,
                DeviceUniqueIdentifier = deviceId,
                PersonalDeviceTypeValueId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSONAL_DEVICE_TYPE_MOBILE.AsGuid() ).Id
            };
            personalDeviceService.Add( device );
            rockContext.SaveChanges();
            return device;
        }
    }
}