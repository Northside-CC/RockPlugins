﻿// <copyright>
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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using org.secc.ChangeManager.Model;
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;

namespace RockWeb.Plugins.org_secc.ChangeManager
{
    [DisplayName( "Change Requests" )]
    [Category( "SECC > CRM" )]
    [Description( "Shows all requests" )]

    [LinkedPage( "Details Page", "Page which contains the details of the change request." )]
    public partial class ChangeRequests : Rock.Web.UI.RockBlock
    {
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );
            gRequests.GridRebind += gRequests_GridRebind;
        }

        private void gRequests_GridRebind( object sender, Rock.Web.UI.Controls.GridRebindEventArgs e )
        {
            BindGrid();
        }


        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            if ( !Page.IsPostBack )
            {
                var rockContext = new RockContext();
                var entityTypes = new EntityTypeService( rockContext ).GetEntities()
                    .OrderBy( t => t.FriendlyName )
                    .ToList();

                pEntityType.IncludeGlobalOption = false;
                pEntityType.EntityTypes = entityTypes;

                cbShowComplete.Checked = GetBlockUserPreference( "ShowComplete" ).AsBoolean();
                pEntityType.SelectedValue = GetBlockUserPreference( "EntityType" );

                BindGrid();
            }
        }

        private void BindGrid()
        {
            RockContext rockContext = new RockContext();
            ChangeRequestService changeRequestService = new ChangeRequestService( rockContext );
            var changeRequests = changeRequestService.Queryable();

            if ( !IsUserAuthorized( Rock.Security.Authorization.EDIT ) )
            {
                var currentPersonAliasIds = CurrentPerson.Aliases.Select( a => a.Id );
                changeRequests = changeRequests.Where( cr => currentPersonAliasIds.Contains( cr.RequestorAliasId ) );
            }

            if ( !cbShowComplete.Checked )
            {
                changeRequests = changeRequests.Where( c => !c.IsComplete );
            }
            if ( pEntityType.SelectedEntityTypeId.HasValue && pEntityType.SelectedEntityTypeId.Value != 0 )
            {
                int entityTypeId = pEntityType.SelectedEntityTypeId.Value;

                //Special case for Person/Person Alias
                if ( entityTypeId == EntityTypeCache.Get( typeof( Person ) ).Id )
                {
                    entityTypeId = EntityTypeCache.Get( typeof( PersonAlias ) ).Id;
                }

                changeRequests = changeRequests.Where( c => c.EntityTypeId == entityTypeId );
            }

            var requests = changeRequests.Select( c =>
                 new ChangePOCO
                 {
                     Id = c.Id,
                     Name = c.Name,
                     EntityType = c.EntityType.FriendlyName,
                     Requestor = c.RequestorAlias.Person.NickName + " " + c.RequestorAlias.Person.LastName,
                     Requested = c.CreatedDateTime ?? Rock.RockDateTime.Today,
                     Applied = c.ChangeRecords.Any( r => r.WasApplied ),
                     WasReviewed = c.IsComplete
                 }
            ).ToList();

            foreach ( var request in requests )
            {
                if ( request.Applied == false && request.WasReviewed == false )
                {
                    request.Name = "<i class='fa fa-exclamation-triangle'></i> " + request.Name;
                }
            }

            requests = requests.OrderBy( c => c.WasReviewed )
                .ThenBy( c => c.Applied )
                .ThenByDescending( c => c.Requested )
                .ToList();
            gRequests.DataSource = requests;
            gRequests.DataBind();
        }

        protected void gRequests_RowSelected( object sender, Rock.Web.UI.Controls.RowEventArgs e )
        {
            NavigateToLinkedPage( "DetailsPage", new Dictionary<string, string> { { "ChangeRequest", e.RowKeyId.ToString() } } );
        }

        protected void fRequests_ApplyFilterClick( object sender, EventArgs e )
        {
            SetBlockUserPreference( "ShowComplete", cbShowComplete.Checked.ToString() );
            SetBlockUserPreference( "EntityType", pEntityType.SelectedValue );
            BindGrid();
        }

        private class ChangePOCO
        {
            public int Id { get; set; }
            public string Name { get; set; }
            private string entityType = "";
            public string EntityType
            {
                get
                {
                    return entityType;
                }
                set
                {
                    entityType = value;
                    if ( entityType == "Person Alias" )
                    {
                        entityType = "Person";
                    }
                }
            }
            public string Requestor { get; set; }
            public DateTime Requested { get; set; }
            public bool Applied { get; set; }
            public bool WasReviewed { get; set; }
        }
    }
}