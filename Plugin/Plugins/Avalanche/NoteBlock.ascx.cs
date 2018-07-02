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
using System.ComponentModel;
using Rock.Model;
using Rock.Security;
using System.Web.UI;
using Rock.Web.Cache;
using Rock.Web.UI;
using System.Web;
using Rock.Data;
using System.Linq;
using System.Collections.Generic;
using Rock;
using Avalanche;
using Avalanche.Models;
using Rock.Attribute;
using Avalanche.Attribute;
using Newtonsoft.Json;

namespace RockWeb.Plugins.Avalanche
{
    [DisplayName( "Note Block" )]
    [Category( "Avalanche" )]
    [Description( "Allows the user to make a single personal note for an entity. Useful for sermons and blog posts." )]

    [NoteTypeField( "Note Type", "Type of note." )]
    [IntegerField( "Note Field Height", "The height of the editor form element.", true, 200 )]
    [TextField( "Note Field Label", "The height of the editor form element.", true, "Notes:" )]
    public partial class NoteBlock : AvalancheBlock
    {
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summarysni>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            var noteTypeString = GetAttributeValue( "NoteType" );
            var noteType = NoteTypeCache.Read( noteTypeString.AsGuid() );

            if ( noteType != null )
            {
                lEntityType.Text = "Note Type: " + noteType.Name;
            }
        }

        public override MobileBlock GetMobile( string parameter )
        {
            var noteTypeString = GetAttributeValue( "NoteType" );
            var noteType = NoteTypeCache.Read( noteTypeString.AsGuid() );

            if ( CurrentPerson == null || noteType == null )
            {
                return new MobileBlock()
                {
                    BlockType = "Avalanche.Blocks.Null",
                    Attributes = CustomAttributes
                };
            }

            var form = GetForm( noteType, parameter.AsInteger() );
            CustomAttributes.Add( "FormElementItems", JsonConvert.SerializeObject( form ) );

            return new MobileBlock()
            {
                BlockType = "Avalanche.Blocks.FormBlock",
                Attributes = CustomAttributes
            };
        }

        private List<FormElementItem> GetForm( NoteTypeCache noteType, int entityId )
        {
            var form = new List<FormElementItem>();
            RockContext rockContext = new RockContext();
            NoteService noteService = new NoteService( rockContext );
            var currentPersonAliasIds = CurrentPerson.Aliases.Select( a => a.Id );
            var note = noteService.Queryable()
                .Where( n => n.NoteTypeId == noteType.Id && n.EntityId == entityId && currentPersonAliasIds.Contains( n.CreatedByPersonAliasId ?? 0 ) )
                .FirstOrDefault();
            if ( note == null )
            {
                note = new Rock.Model.Note
                {
                    NoteTypeId = noteType.Id,
                    EntityId = entityId,
                    Text = ""
                };
            }

            var hidden = new FormElementItem
            {
                Type = FormElementType.Hidden,
                Key = "entityId",
                Value = entityId.ToString()
            };
            form.Add( hidden );

            var editor = new FormElementItem
            {
                Label = GetAttributeValue( "NoteFieldLabel" ),
                HeightRequest = GetAttributeValue( "NoteFieldHeight" ).AsInteger(),
                Type = FormElementType.Editor,
                Key = "note",
                Value = note.Text,
            };
            form.Add( editor );

            var button = new FormElementItem
            {
                Key = "save",
                Type = FormElementType.Button,
                Label = "Save"
            };
            form.Add( button );

            return form;
        }

        public override MobileBlockResponse HandleRequest( string request, Dictionary<string, string> Body )
        {
            var noteTypeString = GetAttributeValue( "NoteType" );
            var noteType = NoteTypeCache.Read( noteTypeString.AsGuid() );
            RockContext rockContext = new RockContext();
            NoteService noteService = new NoteService( rockContext );
            var entityId = Body["entityId"].AsInteger();
            var currentPersonAliasIds = CurrentPerson.Aliases.Select( a => a.Id );
            var note = noteService.Queryable()
                .Where( n => n.NoteTypeId == noteType.Id && n.EntityId == entityId && currentPersonAliasIds.Contains( n.CreatedByPersonAliasId ?? 0 ) )
                .FirstOrDefault();
            if ( note == null )
            {
                note = new Rock.Model.Note
                {
                    NoteTypeId = noteType.Id,
                    EntityId = entityId,
                    Text = "",
                    IsPrivateNote = true
                };
                noteService.Add( note );
            }

            note.Text = Body["note"];
            rockContext.SaveChanges();

            var response = new FormResponse
            {
                Success = true,
                FormElementItems = GetForm( noteType, entityId ),
                Message = "Note Saved"
            };

            return new MobileBlockResponse()
            {
                Request = "save",
                Response = JsonConvert.SerializeObject( response ),
                TTL = 0
            };

        }

        protected void btnButton_Click( object sender, EventArgs e )
        {
        }
    }
}