// <copyright>
// Mark Lee
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalanche.Models;
using Newtonsoft.Json;
using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Transactions;
using Rock.Web.Cache;

namespace Avalanche.Transactions
{
    public class AppInteractionTransaction : ITransaction
    {

        public int? PageId { get; set; }

        public string ComponentName { get; set; }

        public int? SiteId { get; set; }

        public string PageTitle { get; set; }

        public int? PersonAliasId { get; set; }

        public DateTime DateViewed { get; set; }

        public string IPAddress { get; set; }

        public string UserAgent { get; set; }

        public string InteractionData { get; set; }

        public string InteractionSummary { get; set; }

        public string Operation { get; set; }

        public void Execute()
        {
            if ( PageId.HasValue || !string.IsNullOrWhiteSpace( ComponentName ) )
            {
                using ( var rockContext = new RockContext() )
                {
                    int channelMediumTypeValueId = DefinedValueCache.Read( AvalancheUtilities.AppMediumValue.AsGuid() ).Id;
                    var interactionChannelService = new InteractionChannelService( rockContext );
                    var interactionService = new InteractionService( rockContext );
                    var interactionChannel = interactionChannelService.Queryable()
                            .Where( a =>
                                a.ChannelTypeMediumValueId == channelMediumTypeValueId &&
                                a.ChannelEntityId == this.SiteId )
                            .FirstOrDefault();
                    if ( interactionChannel == null )
                    {
                        interactionChannel = new InteractionChannel();
                        interactionChannel.Name = SiteCache.Read( SiteId ?? 1 ).Name;
                        interactionChannel.ChannelTypeMediumValueId = channelMediumTypeValueId;
                        interactionChannel.ChannelEntityId = this.SiteId;
                        interactionChannel.ComponentEntityTypeId = EntityTypeCache.Read<Rock.Model.Page>().Id;
                        interactionChannelService.Add( interactionChannel );
                        rockContext.SaveChanges();
                    }

                    InteractionComponent interactionComponent = null;
                    var interactionComponentService = new InteractionComponentService( rockContext );

                    if ( PageId.HasValue )
                    {
                        interactionComponent = interactionComponentService.GetComponentByEntityId( interactionChannel.Id, PageId.Value, PageTitle );
                    }
                    else
                    {
                        interactionComponent = interactionComponentService.GetComponentByComponentName( interactionChannel.Id, ComponentName );
                    }
                    rockContext.SaveChanges();

                    // Add the interaction
                    if ( interactionComponent != null )
                    {
                        var deviceId = Regex.Match( UserAgent, "(?<=-).+(?=\\))" ).Value.Trim();
                        if ( deviceId.Length > 20 )
                        {
                            deviceId = deviceId.Substring( 0, 20 );
                        }
                        var deviceApplication = Regex.Match( UserAgent, "^[\\S]{0,}" ).Value.Trim() + " " + deviceId;
                        var clientOs = Regex.Match( UserAgent, "(?<=;).+(?=-)" ).Value.Trim();
                        var clientType = Regex.Match( UserAgent, "(?<=\\().+(?=;)" ).Value.Trim();

                        var deviceType = interactionService.GetInteractionDeviceType( deviceApplication, clientOs, clientType, UserAgent );
                        var interactionSessionService = new InteractionSessionService( rockContext );
                        var interactionSession = interactionSessionService.Queryable().Where( s => s.IpAddress == IPAddress && s.DeviceTypeId == deviceType.Id ).FirstOrDefault();

                        if ( interactionSession == null )
                        {
                            interactionSession = new InteractionSession()
                            {
                                DeviceTypeId = deviceType.Id,
                                IpAddress = IPAddress
                            };
                            interactionSessionService.Add( interactionSession );
                            rockContext.SaveChanges();
                        }

                        var interaction = new InteractionService( rockContext ).AddInteraction( interactionComponent.Id, null, Operation, InteractionData, PersonAliasId, DateViewed,
                            deviceApplication, clientOs, clientType, UserAgent, IPAddress, interactionSession.Guid );

                        interaction.InteractionSummary = InteractionSummary;

                        PersonalDevice personalDevice = AvalancheUtilities.GetPersonalDevice( deviceId, PersonAliasId, rockContext );
                        if ( personalDevice != null )
                        {
                            interaction.PersonalDeviceId = personalDevice.Id;
                        }

                        rockContext.SaveChanges();
                    }
                }
            }
        }
    }
}
